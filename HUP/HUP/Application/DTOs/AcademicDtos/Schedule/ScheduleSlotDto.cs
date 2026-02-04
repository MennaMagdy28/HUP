namespace HUP.Application.DTOs.AcademicDtos.Schedule;

public class ScheduleSlotDto
{
    public Guid CourseOfferingId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Group { get; set; }
}