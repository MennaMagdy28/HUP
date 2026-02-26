namespace HUP.Application.DTOs.AcademicDtos.Schedule;

public class ScheduleSlotCreateDto
{
    public Guid CourseOfferingId { get; set; }
    public Guid InstructorId { get; set; } // Added InstructorId
    public DayOfWeek DayOfWeek { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Hall { get; set; }
    public string Group { get; set; }
    public int TotalSeats { get; set; } // Added for Capacity
}