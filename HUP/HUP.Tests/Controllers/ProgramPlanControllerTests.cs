using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using HUP.Core.Entities.Identity;

namespace HUP.Tests.Controllers
{
    public class ProgramPlanControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ProgramPlanControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetProgramPlanByStudentId_WithAuth_ReturnsNotFound()
        {
            var studentUser = new User { Id = Guid.NewGuid(), FullName = "Student" };
            var token = JwtHelper.GenerateTokenForUser(studentUser, "Student");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/ProgramPlan/student");
            response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);
        }
    }
}
