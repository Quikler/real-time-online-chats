using System.Text;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Hubs;
using real_time_online_chats.Server.Providers;
using real_time_online_chats.Server.Services.Chat;
using real_time_online_chats.Server.Services.Cloudinary;
using real_time_online_chats.Server.Services.Google;
using real_time_online_chats.Server.Services.Identity;
using real_time_online_chats.Server.Services.Message;
using real_time_online_chats.Server.Services.User;

const string CORS_POLICY = "MY_CORS";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(CORS_POLICY, policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();

builder.Services.AddSignalR();

builder.Services.AddSingleton<TokenProvider>();

builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChatAuthorizationService, ChatAuthorizationService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMessageAuthorizationService, MessageAuthorizationService>();
builder.Services.AddScoped<IGoogleService, GoogleService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// Get DB_HOST env variable to determine in which host database will run (local - localhost, Docker - see in docker-compose.yml)
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
connectionString = connectionString.Replace("${DB_HOST}", dbHost);

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services
    .AddIdentity<UserEntity, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredUniqueChars = 0;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services
    .Configure<SwaggerConfiguration>(builder.Configuration.GetSection(nameof(SwaggerConfiguration)))
    .Configure<JwtConfiguration>(builder.Configuration.GetSection(nameof(JwtConfiguration)))
    .Configure<GoogleConfiguration>(builder.Configuration.GetSection("Google"));

var cloudinary = builder.Configuration.GetSection("Cloudinary");
CloudinaryConfiguration.CloudName = cloudinary.GetValue<string>("Cloud");
CloudinaryConfiguration.ApiKey = cloudinary.GetValue<string>("ApiKey");
CloudinaryConfiguration.ApiSecret = cloudinary.GetValue<string>("ApiSecret");

var jwtConfig = builder.Configuration.GetSection(nameof(JwtConfiguration)).Get<JwtConfiguration>()
    ?? throw new InvalidOperationException("Configuration for JwtConfiguration is missing or invalid.");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        var googleConfig = builder.Configuration.GetSection("Google").Get<GoogleConfiguration>()
            ?? throw new InvalidOperationException("Configuration for GoogleConfiguration is missing or invalid.");

        options.ClientId = googleConfig.ClientId;
        options.ClientSecret = googleConfig.ClientSecret;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = "https://localhost:7183",
            ValidAudience = "https://localhost:7183",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.SecretKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/messageHub"))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "real-time-online-chats API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey, // 'Bearer 12345abcdef'
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, []
        }
    });
});

var app = builder.Build();

app.UseCors(CORS_POLICY);

app.MapControllers().RequireCors(CORS_POLICY);

app.MapHub<MessageHub>("/messageHub").RequireCors(CORS_POLICY);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var swaggerConfig = app.Services.GetRequiredService<IOptions<SwaggerConfiguration>>().Value;

    app.UseSwagger(options =>
    {
        options.RouteTemplate = swaggerConfig.JsonRoute;
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(swaggerConfig.UIEndpoint, swaggerConfig.Description);
        options.InjectJavascript("/swagger/custom-auth.js");
        //options.RoutePrefix = "swagger";
    });

    // Apply all ef core migrations before running application
    app.ApplyMigrations();
}

app.Run();

public partial class Program;