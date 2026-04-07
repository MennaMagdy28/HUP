using HUP.Core.Entities.Shared;

namespace HUP.Core.Entities.Academics
{
    public class Schedule : BaseEntity
    {
        public Guid CourseOfferingId { get; set; }
        public Guid InstructorId { get; set; }
        public string Group { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Hall  { get; set; }
        public string InstructorName { get; set; } // Can keep for snapshot or remove if now redundant
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }

        public CourseOffering CourseOffering { get; set; }
        public Instructor Instructor { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}