using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using HUP.Core.Entities.Identity;
using HUP.Application.DTOs.IdentityDtos.UserDtos;

namespace HUP.Tests.Controllers
{
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public UserControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_WithAuth_ReturnsOk()
        {
            // Arrange
            var adminUser = new User { Id = Guid.NewGuid(), FullName = "Admin" };
            var token = JwtHelper.GenerateTokenForUser(adminUser, "SuperAdmin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/User");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AddUser_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var adminUser = new User { Id = Guid.NewGuid(), FullName = "Admin" };
            var token = JwtHelper.GenerateTokenForUser(adminUser, "SuperAdmin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsJsonAsync("/api/User", new CreateUserDto());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
