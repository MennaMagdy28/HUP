using System.Security.Claims;
using HUP.Application.DTOs.AcademicDtos.Schedule;
using HUP.Application.DTOs.IdentityDtos.UserDtos;
using HUP.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HUP.Core.Constants;

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
    [Authorize(Policy = AppPermissions.CREATE_SCHEDULE)]
    public async Task<ActionResult<ScheduleSlotCreateDto>> AddSlot(ScheduleSlotCreateDto createDto)
    {
        await _scheduleService.Create(createDto);
        return Ok("Created Successfully");
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = AppPermissions.DELETE_SCHEDULE)]
    public async Task SoftDelete(Guid id)
    {
        await _scheduleService.SoftDelete(id);
    }

    [HttpDelete("{id}/hard")]
    [Authorize(Policy = AppPermissions.DELETE_SCHEDULE)]
    public async Task Remove(Guid id)
    {
        await _scheduleService.Remove(id);
    }
}