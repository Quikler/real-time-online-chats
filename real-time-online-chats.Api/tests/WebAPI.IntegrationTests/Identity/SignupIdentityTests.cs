using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Api.IntegrationTests.Abstractions;
using real_time_online_chats.Server.Common.Constants;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Requests.Auth;
using Shouldly;

namespace real_time_online_chats.Api.IntegrationTests.Identity;

public class SignupIdentityTests(IntegrationTestWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private static SignupUserRequest CreateUserSignupRequest(string email, string password, string confirmPassword) => new()
    {
        Email = email,
        Password = password,
        ConfirmPassword = confirmPassword,
        Phone = "0977777777",
        FirstName = "Test",
        LastName = "Test",
    };

    private const string TestReCAPTCHAClientKey = "6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI";
    private const string TestEmail = "testing@email.com";
    private const string TestPassword = "Test123!";

    [Fact]
    public async Task Signup_WithValidData_ShouldSendConfirmationLinkToEmail()
    {
        // Arrange
        var request = CreateUserSignupRequest(TestEmail, TestPassword, TestPassword);
        HttpClient.DefaultRequestHeaders.Add(HeaderConstants.ReCAPTCHAToken, TestReCAPTCHAClientKey);

        // Act
        var response = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, request);

        // Assert
        //response.EnsureSuccessStatusCode();
        var contentString = await response.Content.ReadAsStringAsync();
        contentString.ShouldContain("Account created. Before login please confirm your email.");

        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        user.ShouldNotBeNull();
    }

    [Fact]
    public async Task Should_ReturnBadRequest_WhenEmailIsMissing()
    {
        // Arrange
        var request = CreateUserSignupRequest("", TestPassword, TestPassword);

        // Act
        var response = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_WhenPasswordLengthLessThan8()
    {
        // Arrange
        var request = CreateUserSignupRequest(TestEmail, "1234567", "1234567");

        // Act
        var response = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_WhenPasswordsDoesntMatch()
    {
        // Arrange
        var request = CreateUserSignupRequest(TestEmail, TestPassword, "test");

        // Act
        var response = await HttpClient.PostAsJsonAsync(ApiRoutes.Identity.Signup, request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
