using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using HUP.Core.Entities.Identity;

namespace HUP.Tests.Controllers
{
    public class StudentsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public StudentsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetProfile_WithAuth_ReturnsBadRequest()
        {
            // Arrange
            var studentUser = new User { Id = Guid.NewGuid(), FullName = "Student" };
            var token = JwtHelper.GenerateTokenForUser(studentUser, "Student");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/Students/Profile");

            // Assert
            // Expecting BadRequest because our token uses a random Guid instead of the Seeded user Guid
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
