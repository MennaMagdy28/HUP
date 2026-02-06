using HUP.Application.Mappers;
using HUP.Application.Services.Interfaces;
using HUP.Application.DTOs.AcademicDtos.Enrollment;
using HUP.Core.Entities.Academics;
using HUP.Repositories.Interfaces;
using System.Threading.Tasks;
using HUP.Application.DTOs.AcademicDtos;
using HUP.Common.Helpers;
using HUP.Core.Enums.AcademicEnums;

namespace HUP.Application.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repository;
        private readonly IStudentRepository _studentRepo;
        private readonly IProgramPlanRepository _planRepo;

        public EnrollmentService(IEnrollmentRepository repository, IStudentRepository studentRepo, IProgramPlanRepository planRepo)
        {
            _repository = repository;
            _studentRepo = studentRepo;
            _planRepo = planRepo;
        }

        public async Task AddAsync(CreateEnrollmentDto dto)
        {
            var enrollment = EnrollmentMapper.ToEntityFromCreateDto(dto);
            enrollment.Id = Guid.NewGuid();
            enrollment.EnrollmentDate = DateTime.Now;
            enrollment.CreatedAt = DateTime.Now;
            enrollment.Status = EnrollmentStatus.Registered;
            await _repository.AddAsync(enrollment);
            await _repository.SaveChangesAsync();
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
            var enrollment = await _repository.GetByIdReadOnly(id);
            enrollment.IsDeleted = true;
            enrollment.UpdatedAt = DateTime.Now;
            await _repository.SaveChangesAsync();
        }

        public async Task Update(Guid id, UpdateEnrollmentDto dto)
        {
            var enrollment = await _repository.GetByIdTracking(id);
            enrollment.UpdatedAt = DateTime.Now;
            // the mapper will copy the values in it to the entity
            // ef core tracks the changes and update only specific attributes
            EnrollmentMapper.ToUpdate(dto, enrollment);
            await _repository.SaveChangesAsync();
        }
        

        public async Task<List<SemesterTranscriptDto>> GetStudentGradesAsync(Guid studentId, string lang)
        {
            var models = await _repository.GetStudentSemesterGradeModelsAsync(studentId);
            var student = await _studentRepo.GetByIdReadOnly(studentId);

            var departmentId = student.DepartmentId;

            // Accumulators for cumulative GPA
            decimal cumulativePoints = 0;
            decimal cumulativeHours = 0;

            // Temporary ungrouped list of courses with computed values
            var computedCourses = new List<SemesterGradesDto>();

            foreach (var m in models)
            {
                var totalGrade = m.ClassGrade + m.MidtermGrade + m.FinalGrade;

                var programPlan = await _planRepo.GetByIdReadOnly(departmentId, m.CourseId);
                var maxGrade = programPlan.FinalGrade;

                var grade = getGrade(totalGrade / maxGrade * 100);
                var gradePts = GetGradePoints(grade);
                var creditPts = gradePts * m.CourseCredits;

                // Add to cumulative totals
                cumulativePoints += creditPts;
                cumulativeHours += m.CourseCredits;

                // Add computed course to TEMP LIST (ungrouped!)
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

            // Compute cumulative GPA once
            var cumulativeGPA = cumulativeHours == 0
                ? 0
                : cumulativePoints / cumulativeHours;

            // Now group using the computed data
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

        public async Task<IEnumerable<EnrollmentResponseDto>> GetRegisteredByStudentAsync(Guid studentId, EnrollmentFilterDto filter, string lang)
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
