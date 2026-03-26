using FluentAssertions;
using HUP.Application.DTOs.AuthDtos;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace HUP.Tests.Controllers
{
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_WithValidAdminCredentials_ReturnsOk()
        {
            var loginDto = new LoginDto { NationalId = "00000000000000", Password = "Admin@123" };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_WithValidStudentCredentials_ReturnsOk()
        {
            var loginDto = new LoginDto { NationalId = "11111111111111", Password = "Student@123" };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            var loginDto = new LoginDto { NationalId = "11111111111111", Password = "wrong_password" };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_WithEmptyNationalId_ReturnsBadRequest()
        {
            var loginDto = new LoginDto { NationalId = "", Password = "password" };
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdatePassword_WithoutAuthentication_ReturnsUnauthorized()
        {
            var updateDto = new UpdatePassword { CurrentPassword = "old", NewPassword = "new" };
            var response = await _client.PostAsJsonAsync("/api/Auth/change-password", updateDto);

            response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
        }
    }
}
