using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using real_time_online_chats.Server.Data;
using Testcontainers.PostgreSql;

namespace real_time_online_chats.Api.IntegrationTests.Abstractions;

public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithName("root")
        .WithPassword("root")
        .WithDatabase("roc_db")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services => 
        {
            services.Remove(services.Single(s => s.ServiceType == typeof(DbContextOptions<AppDbContext>)));
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(_dbContainer.GetConnectionString()));
        });
    }

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }
}
