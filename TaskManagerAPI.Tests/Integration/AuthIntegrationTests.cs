using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using TaskManagerAPI.DTOs.Auth;
using Xunit.Abstractions;

namespace TaskManagerAPI.Tests.Integration
{
    public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public AuthIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task Register_ShouldReturnJwtToken()
        {
            var request = new RegisterDto { Email = "test@test.com", Password = "Password123!", Username = "testuser" };

            var response = await _client.PostAsJsonAsync("/api/auth/register", request);

            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Response Content: {content}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.Email.Should().Be(request.Email);
            result.Role.Should().Be("User");
        }

        [Fact]
        public async Task Login_ShouldReturnJwtToken()
        {
            var registerRequest = new RegisterDto { Email = "login@test.com", Password = "Password123!", Username = "loginuser" };

            await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

            var loginRequest = new LoginDto { Email = "login@test.com", Password = "Password123!" };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_ForInvalidPassword()
        {
            var registerRequest = new RegisterDto { Email = "badlogin@test.com", Password = "Password123!", Username = "badloginuser" };

            await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

            var loginRequest = new LoginDto { Email = "badlogin@test.com", Password = "WrongPassword!" };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
