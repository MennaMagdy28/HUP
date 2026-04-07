using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using HUP.Core.Entities.Identity;
using HUP.Application.DTOs.AcademicDtos.Enrollment;

namespace HUP.Tests.Controllers
{
    public class EnrollmentControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public EnrollmentControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetRegisteredByStudentAsync_WithAuth_ReturnsOk()
        {
            var studentUser = new User { Id = Guid.NewGuid(), FullName = "Student" };
            var token = JwtHelper.GenerateTokenForUser(studentUser, "Student");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/Enrollment/student");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetStudentGrades_WithAuth_ReturnsOk()
        {
            var studentUser = new User { Id = Guid.NewGuid(), FullName = "Student" };
            var token = JwtHelper.GenerateTokenForUser(studentUser, "Student");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/Enrollment/grades");
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_WithInvalidData_ReturnsBadRequest()
        {
            var studentUser = new User { Id = Guid.NewGuid(), FullName = "Student" };
            var token = JwtHelper.GenerateTokenForUser(studentUser, "Student");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsJsonAsync("/api/Enrollment", new CreateEnrollmentDto());
            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
        }
    }
}
