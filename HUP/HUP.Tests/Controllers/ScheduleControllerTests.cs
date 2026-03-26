using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using HUP.Core.Entities.Identity;

namespace HUP.Tests.Controllers
{
    public class ScheduleControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ScheduleControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AddSlot_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var adminUser = new User { Id = Guid.NewGuid(), FullName = "Admin" };
            var token = JwtHelper.GenerateTokenForUser(adminUser, "SuperAdmin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync("/api/Schedule", new { });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetRegisteredSchedule_WithAuth_ReturnsOk()
        {
            // Arrange
            var studentUser = new User { Id = Guid.NewGuid(), FullName = "Student" };
            var token = JwtHelper.GenerateTokenForUser(studentUser, "Student");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/Schedule");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAvailableToRegister_WithAuth_ReturnsOk()
        {
            // Arrange
            var studentUser = new User { Id = Guid.NewGuid(), FullName = "Student" };
            var token = JwtHelper.GenerateTokenForUser(studentUser, "Student");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/Schedule/Register");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
