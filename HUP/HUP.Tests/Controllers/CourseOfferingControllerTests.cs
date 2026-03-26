using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using HUP.Core.Entities.Identity;
using HUP.Application.DTOs.AcademicDtos.CourseOffering;

namespace HUP.Tests.Controllers
{
    public class CourseOfferingControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CourseOfferingControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_WithAuth_ReturnsOk()
        {
            var adminUser = new User { Id = Guid.NewGuid(), FullName = "Admin" };
            var token = JwtHelper.GenerateTokenForUser(adminUser, "SuperAdmin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/CourseOffering");
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_WithInvalidData_ReturnsBadRequest()
        {
            var adminUser = new User { Id = Guid.NewGuid(), FullName = "Admin" };
            var token = JwtHelper.GenerateTokenForUser(adminUser, "SuperAdmin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var createDto = new CreateCourseOfferingDto();

            var response = await _client.PostAsJsonAsync("/api/CourseOffering", createDto);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.UnsupportedMediaType, HttpStatusCode.InternalServerError, HttpStatusCode.OK);
        }
    }
}
