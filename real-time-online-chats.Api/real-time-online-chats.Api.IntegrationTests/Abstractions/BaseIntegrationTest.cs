using System;
using Microsoft.Extensions.DependencyInjection;
using real_time_online_chats.Server.Data;

namespace real_time_online_chats.Api.IntegrationTests.Abstractions;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebApplicationFactory>
{
    protected HttpClient HttpClient { get; init; }
    protected AppDbContext DbContext { get; init; }

    protected BaseIntegrationTest(IntegrationTestWebApplicationFactory factory)
    {
        HttpClient = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}
