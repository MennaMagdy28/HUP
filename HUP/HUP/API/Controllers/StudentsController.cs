using HUP.Application.DTOs.AcademicDtos;
using HUP.Application.DTOs.AcademicDtos.Enrollment;
using HUP.Application.DTOs.AcademicDtos.Student;
using HUP.Application.Services.Interfaces;
using HUP.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HUP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }
        //TODO
        //ADD UPDATE PROFILE, REGISTER STUDENT, SOFT DELETE
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStudentDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _studentService.AddStudent(createDto);
            return Ok("Added");
        }

        [HttpGet("Profile")]
        [Authorize]
        public async Task<ActionResult<StudentProfileDto>> GetProfile([FromHeader(Name = "Accept-Language")] string lang = "ar")
        {
            var id = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var profile = await _studentService.GetStudentProfile(id, lang);
            if (profile == null)
                return BadRequest("User not found.");
            return Ok(profile);
        }

        [HttpPatch("Status")]
        [Authorize]
        public async Task<IActionResult> UpdateAcademicStatus([FromBody] StudentStatusDto statusDto)
        {
            var id = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            bool result = await _studentService.UpdateStudentStatus(id, statusDto);
            if (!result) return BadRequest("Failed to update status");
            return Ok("Status updated successfully.");
        }

    }
}
