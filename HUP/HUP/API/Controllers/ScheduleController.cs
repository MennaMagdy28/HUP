using System.Security.Claims;
using HUP.Application.DTOs.AcademicDtos.Schedule;
using HUP.Application.DTOs.IdentityDtos.UserDtos;
using HUP.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HUP.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{ 
    private readonly IScheduleService _scheduleService;
    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpPost]
    public async Task<ActionResult<ScheduleSlotDto>> AddSlot(ScheduleSlotDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        await _scheduleService.Create(dto);
        return Ok("Created Successfully");
    }

    [HttpDelete("{id}")]
    public async Task SoftDelete(Guid id)
    {
        await _scheduleService.SoftDelete(id);
    }

    [HttpDelete("{id}/hard")]
    public async Task Remove(Guid id)
    {
        await _scheduleService.Remove(id);
    }
}