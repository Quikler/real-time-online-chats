using System.Text;
using System.Text.Json.Serialization;
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
using real_time_online_chats.Server.Factories.SmtpClient;
using real_time_online_chats.Server.Filters;
using real_time_online_chats.Server.Hubs;
using real_time_online_chats.Server.Providers;
using real_time_online_chats.Server.Repositories.Chat;
using real_time_online_chats.Server.Services.Chat;
using real_time_online_chats.Server.Services.Cloudinary;
using real_time_online_chats.Server.Services.Google;
using real_time_online_chats.Server.Services.Identity;
using real_time_online_chats.Server.Services.Mail;
using real_time_online_chats.Server.Services.Message;
using real_time_online_chats.Server.Services.User;
using FluentValidation;
using real_time_online_chats.Server.Common.Constants;
using real_time_online_chats.Server.Services.Cache;
using real_time_online_chats.Server.Data.Seed.User;
using StackExchange.Redis;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using real_time_online_chats.Server.Contracts.HealthCheck;
using System.Text.Json;
using real_time_online_chats.Server.HealthChecks;

const string CORS_POLICY = "MY_CORS";

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDocker())
{
    builder.Configuration.AddJsonFile("appsettings.Docker.json");
}

// Add .env file
DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
builder.Configuration.AddEnvironmentVariables();

var clientConfiguration = builder.Configuration.GetSection("Client").Get<ClientConfiguration>()
    ?? throw new InvalidOperationException("Configuration for ClientConfiguration is missing or invalid.");

builder.Services.AddCors(options =>
{
    options.AddPolicy(CORS_POLICY, policy =>
    {
        policy
            .WithOrigins(clientConfiguration.Origin)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    })
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddHttpClient(HttpClientNameConstants.GoogleRecaptchaApi, client =>
{
    client.BaseAddress = new Uri("https://www.google.com/recaptcha/api/");
});

builder.Services.AddSignalR();

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<AppDbContext>()
    .AddCheck<RedisHealthCheck>("Redis");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

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
    .Configure<GoogleConfiguration>(builder.Configuration.GetSection("Google"))
    .Configure<MailConfiguration>(builder.Configuration.GetSection("Mail"))
    .Configure<ReCAPTCHAConfiguration>(builder.Configuration.GetSection("reCAPTCHAv2"))
    .Configure<RedisCacheConfiguration>(builder.Configuration.GetSection("Redis"))
    .Configure<ClientConfiguration>(builder.Configuration.GetSection("Client"));

# region Redis setup
var redisConfig = builder.Configuration.GetSection("Redis").Get<RedisCacheConfiguration>()
    ?? throw new InvalidOperationException("Configuration for RedisCacheConfiguration is missing or invalid.");

if (redisConfig.Enabled)
{
    builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisConfig.ConnectionString);
    builder.Services.AddSingleton<IResponseCacheService, ResponseCacheService>();

    builder.Services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(redisConfig.ConnectionString));
    builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
}
# endregion

var cloudinary = builder.Configuration.GetSection("Cloudinary");
CloudinaryConfiguration.CloudName = cloudinary.GetValue<string>("Cloud");
CloudinaryConfiguration.ApiKey = cloudinary.GetValue<string>("ApiKey");
CloudinaryConfiguration.ApiSecret = cloudinary.GetValue<string>("ApiSecret");

builder.Services.AddTransient<ICloudinary, Cloudinary>(factory =>
{
    var account = new Account(
        CloudinaryConfiguration.CloudName,
        CloudinaryConfiguration.ApiKey,
        CloudinaryConfiguration.ApiSecret);

    return new Cloudinary(account);
});
builder.Services.AddTransient<TokenProvider>();
builder.Services.AddTransient<ISmtpClientFactory, SmtpClientFactory>();
builder.Services.AddTransient<IMailService, MailService>();

builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChatUserService, ChatUserService>();
builder.Services.AddScoped<IChatMessageService, ChatMessageService>();
builder.Services.AddScoped<IChatAuthorizationService, ChatAuthorizationService>();
builder.Services.AddScoped<IChatMessageService, ChatMessageService>();
builder.Services.AddScoped<IMessageAuthorizationService, MessageAuthorizationService>();
builder.Services.AddScoped<IGoogleService, GoogleService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

builder.Services.AddScoped<IChatRepository, ChatRepository>();

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
            ValidIssuer = jwtConfig.ValidIssuer,
            ValidAudience = jwtConfig.ValidAudience,
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

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new HealthCheckResponse
        {
            Status = report.Status.ToString(),
            Checks = report.Entries.Select(e => new HealthCheck
            {
                Component = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description,
            }),
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsDocker())
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

    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.SeedDefaultUsersAsync();
}

app.Run();

public partial class Program;
