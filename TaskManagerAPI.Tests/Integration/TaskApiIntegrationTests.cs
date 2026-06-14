using FluentAssertions;
using System.Net;

namespace TaskManagerAPI.Tests.Integration
{
    public class TaskApiIntegrationTests: IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public TaskApiIntegrationTests(
            CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetTasks_ShouldReturnUnauthorized_WithoutToken()
        {
            var response = await _client.GetAsync("/api/tasks");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
