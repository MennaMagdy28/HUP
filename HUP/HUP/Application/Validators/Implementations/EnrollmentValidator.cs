using HUP.Application.DTOs.AcademicDtos.Enrollment;
using HUP.Application.Validators.Interfaces;
using HUP.Core.Entities.Academics;
using HUP.Core.Enums.AcademicEnums;
using HUP.Repositories.Interfaces;

namespace HUP.Application.Validators.Implementations
{
    public class EnrollmentValidator : IEnrollmentValidator
    {
        private readonly IStudentRepository _studentRepo;
        private readonly ICourseOfferingRepository _offeringRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly ISemesterRepository _semesterRepo;
        private readonly IScheduleRepository _scheduleRepo;

        public EnrollmentValidator(
            IStudentRepository studentRepo,
            ICourseOfferingRepository offeringRepo,
            IEnrollmentRepository enrollmentRepo,
            ISemesterRepository semesterRepo,
            IScheduleRepository scheduleRepo)
        {
            _studentRepo = studentRepo;
            _offeringRepo = offeringRepo;
            _enrollmentRepo = enrollmentRepo;
            _semesterRepo = semesterRepo;
            _scheduleRepo = scheduleRepo;
        }

        public async Task ValidateEnrollmentAsync(CreateEnrollmentDto dto)
        {
            // 1. GPA Window Check
            if (!await CanStudentEnroll(dto.StudentId))
            {
                throw new InvalidOperationException("Enrollment is not yet open for your GPA tier.");
            }

            // 2. Duplicate Check
            var existingEnrollment = await _enrollmentRepo.GetExistingAsync(dto.StudentId, dto.CourseOfferingId);
            if (existingEnrollment != null)
            {
                throw new InvalidOperationException("Student is already enrolled in this course.");
            }

            // Retrieve with Schedules
            var courseOffering = await _offeringRepo.GetWithSchedulesAsync(dto.CourseOfferingId);
            if (courseOffering == null)
                throw new InvalidOperationException("Course offering not found.");

            // 3. Prerequisite Check
            if (courseOffering.Course != null && courseOffering.Course.PrerequisiteId != null)
            {
                var hasPassed = await _enrollmentRepo.HasPassedPrerequisiteAsync(dto.StudentId, courseOffering.Course.PrerequisiteId.Value);
                if (!hasPassed)
                {
                    throw new InvalidOperationException($"Prerequisite not met for course {courseOffering.Course.CourseCode}.");
                }
            }

            // 4. Capacity & Conflict Check
            var student = await _studentRepo.GetByIdReadOnly(dto.StudentId);
            var studentGroup = student.Group;

            // Filter schedules by student group
            var offeringSchedules = courseOffering.Schedules?.Where(s => s.Group == studentGroup).ToList();

            if (offeringSchedules != null && offeringSchedules.Any())
            {
                // Fetch existing enrollments for conflict check
                var currentEnrollments = await _enrollmentRepo.GetByStudentAndSemesterAsync(dto.StudentId, courseOffering.Semester.SemesterName);

                foreach (var slot in offeringSchedules)
                {
                    // Conflict Check
                    foreach (var enrolled in currentEnrollments)
                    {
                         var enrolledSchedules = enrolled.CourseOffering.Schedules?.Where(s => s.Group == studentGroup);
                         if (enrolledSchedules != null)
                         {
                             foreach (var existingSlot in enrolledSchedules)
                             {
                                 if (slot.DayOfWeek == existingSlot.DayOfWeek)
                                 {
                                     if (slot.StartTime < existingSlot.EndTime && slot.EndTime > existingSlot.StartTime)
                                     {
                                         throw new InvalidOperationException($"Time conflict with course {enrolled.CourseOffering.Course.CourseCode} on {slot.DayOfWeek}.");
                                     }
                                 }
                             }
                         }
                    }

                    // Capacity Check
                    // Note: We only CHECK here. The Service performs the ATOMIC decrement.
                    // However, to be safe, we check available seats > 0.
                    if (slot.AvailableSeats <= 0)
                    {
                         throw new InvalidOperationException($"Seat unavailable for schedule {slot.DayOfWeek} {slot.StartTime}.");
                    }
                }
            }
        }

        public async Task ValidateDropAsync(Guid enrollmentId, Guid studentId)
        {
            var enrollment = await _enrollmentRepo.GetByIdWithDetailsAsync(enrollmentId);
            if (enrollment == null)
                throw new KeyNotFoundException("Enrollment not found.");

            if (enrollment.StudentId != studentId)
                throw new UnauthorizedAccessException("Cannot drop another student's course.");

            var activeSemester = await _semesterRepo.GetActiveSemesterAsync();
            if (activeSemester == null)
                throw new InvalidOperationException("No active semester.");

            // 1. Drop Deadline Check
            if (DateTime.UtcNow > activeSemester.DropDeadline)
            {
                throw new InvalidOperationException("Drop deadline has passed.");
            }

            // 2. Minimum Credits Check (e.g. 12 credits)
            // Fetch all current enrollments
            var currentEnrollments = await _enrollmentRepo.GetByStudentAndSemesterAsync(studentId, activeSemester.SemesterName);
            var currentCredits = currentEnrollments.Sum(e => e.CourseOffering?.Course?.Credits ?? 0);
            var courseCredits = enrollment.CourseOffering?.Course?.Credits ?? 0;

            if (currentCredits - courseCredits < 12)
            {
                // Warning: Business rule might vary (e.g. withdrawal vs drop).
                // For now, enforcing min credits for "Drop".
                throw new InvalidOperationException("Cannot drop course. Total credits would fall below minimum load (12).");
            }
        }

        private async Task<bool> CanStudentEnroll(Guid studentId)
        {
            var student = await _studentRepo.GetByIdReadOnly(studentId);
            if (student == null) return false;

            var activeSemester = await _semesterRepo.GetActiveSemesterAsync();
            if (activeSemester == null) return false;

            var startTime = activeSemester.StartDate;
            var now = DateTime.UtcNow;

            var gpa = student.Cgpa;

            if (gpa >= 3.8m)
            {
                return now >= startTime;
            }
            else if (gpa >= 3.5m)
            {
                return now >= startTime.AddHours(2);
            }
            else
            {
                return now >= startTime.AddHours(4);
            }
        }
    }
}
