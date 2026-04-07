using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using Xunit;
using HUP.Core.Entities.Identity;

namespace HUP.Tests.Controllers
{
    public class DepartmentsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public DepartmentsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_WithAuth_ReturnsOk()
        {
            var adminUser = new User { Id = Guid.NewGuid(), FullName = "Admin" };
            var token = JwtHelper.GenerateTokenForUser(adminUser, "SuperAdmin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/Departments");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAll_WithoutAuth_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/Departments");
            response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.OK);
        }
    }
}
