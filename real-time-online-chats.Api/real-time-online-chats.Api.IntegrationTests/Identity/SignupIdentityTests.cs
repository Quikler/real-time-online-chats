using System;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Api.IntegrationTests.Abstractions;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Requests.Auth;
using real_time_online_chats.Server.Contracts.V1.Responses.Auth;

namespace real_time_online_chats.Api.IntegrationTests.Identity;

public class SignupIdentityTests : BaseIntegrationTest
{
    private static UserSignupRequest GetValidUserSignupRequest() => new()
        {
            Email = "test@test.com",
            Password = "123456789",
            ConfirmPassword = "123456789",
            Phone = "+380777777777",
            FirstName = "Test",
            LastName = "Test",
        };
    

    public SignupIdentityTests(IntegrationTestWebApplicationFactory factory) : base(factory) {}

    [Fact]
    public async Task Signup_WithValidData_SucceedsAndAddsUserToDatabase()
    {
        // Arrange
        var request = GetValidUserSignupRequest();

        // Act
        var response = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, request);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<AuthSuccessResponse>();
        
        content?.Token.Should().NotBeNull();
        content?.RefreshToken.Should().NotBeNull();

        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        user.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_ReturnBadRequest_WhenEmailIsMissing()
    {
        // Arrange
        var request = GetValidUserSignupRequest();
        request.Email = string.Empty;

        // Act
        var response = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_WhenPasswordLengthLessThan8()
    {
        // Arrange
        var request = GetValidUserSignupRequest();
        request.ConfirmPassword = request.Password = "1234567";

        // Act
        var response = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_WhenPasswordsDoesntMatch()
    {
        // Arrange
        var request = GetValidUserSignupRequest();
        request.Password = "";

        // Act
        var response = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
