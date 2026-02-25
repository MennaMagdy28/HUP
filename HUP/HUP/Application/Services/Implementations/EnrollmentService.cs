using HUP.Application.Mappers;
using HUP.Application.Services.Interfaces;
using HUP.Application.DTOs.AcademicDtos.Enrollment;
using HUP.Core.Entities.Academics;
using HUP.Repositories.Interfaces;
using System.Threading.Tasks;
using HUP.Application.DTOs.AcademicDtos;
using HUP.Common.Helpers;
using HUP.Core.Enums.AcademicEnums;
using HUP.Data;
using Microsoft.EntityFrameworkCore;

namespace HUP.Application.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repository;
        private readonly IStudentRepository _studentRepo;
        private readonly IProgramPlanRepository _planRepo;
        private readonly ICourseOfferingRepository _offeringRepo;
        private readonly ISemesterRepository _semesterRepo;
        private readonly IScheduleRepository _scheduleRepo;
        private readonly HupDbContext _dbContext;

        public EnrollmentService(IEnrollmentRepository repository, IStudentRepository studentRepo, IProgramPlanRepository planRepo, ICourseOfferingRepository offeringRepo, ISemesterRepository semesterRepo, IScheduleRepository scheduleRepo, HupDbContext dbContext)
        {
            _repository = repository;
            _studentRepo = studentRepo;
            _planRepo = planRepo;
            _offeringRepo = offeringRepo;
            _semesterRepo = semesterRepo;
            _scheduleRepo = scheduleRepo;
            _dbContext = dbContext;
        }

        public async Task AddAsync(CreateEnrollmentDto dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // 1. GPA Window Check
                if (!await CanStudentEnroll(dto.StudentId))
                {
                    throw new InvalidOperationException("Enrollment is not yet open for your GPA tier.");
                }

                // 2. Duplicate Check
                var existingEnrollment = await _repository.GetExistingAsync(dto.StudentId, dto.CourseOfferingId);
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
                     var hasPassed = await _repository.HasPassedPrerequisiteAsync(dto.StudentId, courseOffering.Course.PrerequisiteId.Value);
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
                    var currentEnrollments = await _repository.GetByStudentAndSemesterAsync(dto.StudentId, courseOffering.Semester.SemesterName);

                    foreach (var slot in offeringSchedules)
                    {
                        // Conflict Check
                        foreach (var enrolled in currentEnrollments)
                        {
                             // Ensure we check against the student's group in enrolled courses too
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

                        // Capacity Check & Seat Decrement (Atomic)
                        var booked = await _scheduleRepo.TryBookSeatAsync(slot.Id);
                        if (!booked)
                        {
                             throw new InvalidOperationException($"Seat unavailable for schedule {slot.DayOfWeek} {slot.StartTime}.");
                        }
                    }
                }
                else
                {
                    // If no schedules for this group, should we allow enrollment?
                    // Usually indicates setup error or open enrollment without schedules.
                    // Proceeding, but logging or throwing might be safer depending on business rule.
                    // For now, allowing as "Online/No Schedule" course if no schedules exist.
                }

                var enrollment = EnrollmentMapper.ToEntityFromCreateDto(dto);
                enrollment.Id = Guid.NewGuid();
                enrollment.EnrollmentDate = DateTime.Now;
                enrollment.CreatedAt = DateTime.Now;
                enrollment.Status = EnrollmentStatus.Registered;

                await _repository.AddAsync(enrollment);
                await _repository.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CanStudentEnroll(Guid studentId)
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

        public async Task<IEnumerable<EnrollmentResponseDto>> GetAllAsync(string lang)
        {
            var entities =  await _repository.GetAllAsync();
            var dtos = entities.Select(e => EnrollmentMapper.ToResponseDto(e, lang));
            return dtos;
        }

        public async Task<EnrollmentResponseDto> GetByIdAsync(Guid id, string lang)
        {
            var entity = await _repository.GetByIdReadOnly(id);
            var dto = EnrollmentMapper.ToResponseDto(entity, lang);
            return dto;
        }

        public async Task<bool> Exists(CreateEnrollmentDto dto)
        {
            var entity = await _repository.GetExistingAsync(dto.StudentId, dto.CourseOfferingId);
            return entity != null;
        }

        public async Task Remove(Guid id)
        {
            await _repository.RemoveAsync(id);
            await _repository.SaveChangesAsync();
        }
        public async Task SoftDelete(Guid id)
        {
            var enrollment = await _repository.GetByIdTracking(id);
            enrollment.IsDeleted = true;
            enrollment.UpdatedAt = DateTime.Now;
            await _repository.SaveChangesAsync();
        }

        public async Task Update(Guid id, UpdateEnrollmentDto dto)
        {
            var enrollment = await _repository.GetByIdTracking(id);
            enrollment.UpdatedAt = DateTime.Now;
            EnrollmentMapper.ToUpdate(dto, enrollment);
            await _repository.SaveChangesAsync();
        }
        

        public async Task<List<SemesterTranscriptDto>> GetStudentGradesAsync(Guid studentId, string lang)
        {
            var models = await _repository.GetStudentSemesterGradeModelsAsync(studentId);
            var student = await _studentRepo.GetByIdReadOnly(studentId);

            var departmentId = student.DepartmentId;

            decimal cumulativePoints = 0;
            decimal cumulativeHours = 0;

            var computedCourses = new List<SemesterGradesDto>();

            foreach (var m in models)
            {
                var totalGrade = m.ClassGrade + m.MidtermGrade + m.FinalGrade;

                var programPlan = await _planRepo.GetByIdReadOnly(departmentId, m.CourseId);
                var maxGrade = programPlan.FinalGrade;

                var grade = getGrade(totalGrade / maxGrade * 100);
                var gradePts = GetGradePoints(grade);
                var creditPts = gradePts * m.CourseCredits;

                cumulativePoints += creditPts;
                cumulativeHours += m.CourseCredits;

                computedCourses.Add(new SemesterGradesDto
                {
                    SemesterId = m.SemesterId,
                    SemesterName = LocalizationHelper.Get<string>(m.SemesterName, lang),
                    CourseCode = LocalizationHelper.Get<string>(m.CourseCode,lang),
                    CourseName = LocalizationHelper.Get<string>(m.CourseName,lang),
                    TotalGrade = totalGrade,
                    Grade = grade,
                    CreditHours = m.CourseCredits,
                    GradePoints = gradePts,
                    CreditPoints = creditPts
                });
            }

            var cumulativeGPA = cumulativeHours == 0
                ? 0
                : cumulativePoints / cumulativeHours;

            var grouped = computedCourses.GroupBy(c => c.SemesterName);

            var transcript = new List<SemesterTranscriptDto>();

            foreach (var semGroup in grouped)
            {
                var semCourses = semGroup.ToList();

                decimal semPoints = semCourses.Sum(c => c.CreditPoints);
                decimal semHours = semCourses.Sum(c => c.CreditHours);

                var semesterDto = new SemesterTranscriptDto
                {
                    SemesterName = semGroup.Key,
                    Courses = semCourses,

                    SemesterGPA = semHours == 0 ? 0 : semPoints / semHours,
                    CumulativeGPA = cumulativeGPA
                };

                transcript.Add(semesterDto);
            }

            return transcript;
        }

        public async Task<IEnumerable<EnrollmentResponseDto>> GetRegisteredByStudentAsync(Guid studentId, string lang, EnrollmentFilterDto filter)
        {
            var enrollments = await _repository.GetFilteredAsync(studentId, filter);

            return enrollments.Select(e => EnrollmentMapper.ToResponseDto(e, lang));
        }

        public string getGrade(decimal grade)
        {
            return grade switch
            {
                >= 90 => "A+",
                >= 85 => "A",
                >= 80 => "B+",
                >= 75 => "B",
                >= 70 => "C+",
                >= 65 => "C",
                >= 60 => "D+",
                >= 50 => "D",
                _     => "F"
            };
        }
        public decimal GetGradePoints(string grade)
        {
            return grade switch
            {
                "A+" => 4.0m,
                "A"  => 3.75m,
                "B+" => 3.4m,
                "B"  => 3.1m,
                "C+" => 2.8m,
                "C"  => 2.5m,
                "D+" => 2.2m,
                "D"  => 2.0m,
                "F"  => 0.0m,
                _    => 0.0m
            };
        }
    }
}
