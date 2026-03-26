using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using HUP.Core.Entities.Identity;

namespace HUP.Tests.Controllers
{
    public class ExamControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ExamControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetStudentExamSchedule_WithAuth_ReturnsOk()
        {
            // Arrange
            var studentUser = new User { Id = Guid.NewGuid(), FullName = "Student" };
            var token = JwtHelper.GenerateTokenForUser(studentUser, "Student");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/Exam/student");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
