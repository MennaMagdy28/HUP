using HUP.Core.Entities.Academics;
using HUP.Data;
using HUP.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HUP.Repositories.Implementations
{
    public class ExamRepository : GenericRepository<Exam>, IExamRepository
    {
        public ExamRepository(HupDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await _context.Exams
                .AsNoTracking()
                .ToListAsync();
        }

        public override Task<Exam> GetByIdReadOnly(Guid id)
        {
            var exam = _context.Exams.Include(e => e.CourseOffering)
                .ThenInclude(c => c.Department)
                .ThenInclude(d => d.Instructors)
                .ThenInclude(i => i.User)
                .AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            return exam;
        }
        public override Task<Exam> GetByIdTracking(Guid id)
        {
            var exam = _context.Exams.Include(e => e.CourseOffering)
                .ThenInclude(c => c.Department)
                .ThenInclude(d => d.Instructors)
                .ThenInclude(i => i.User)
                .AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            return exam;
        }

        public async Task<IEnumerable<Exam>> GetByCoursesAsync(List<Guid> courseIds)
        {
            if (!courseIds.Any())
                return new List<Exam>();

            return await _context.Exams
                .Include(e => e.CourseOffering)
                .ThenInclude(c => c.Department)
                .ThenInclude(d => d.Instructors)
                .ThenInclude(i => i.User)
                .Where(e => courseIds.Contains(e.CourseOffering.CourseId))
                .OrderBy(e => e.ExamDate)
                .ThenBy(e => e.ExamTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetAllActiveAsync()
        {
            return await _context.Exams
                .Include(e => e.CourseOffering)
                .ThenInclude(c => c.Department)
                .ThenInclude(d => d.Instructors)
                .ThenInclude(i => i.User)
                .OrderBy(e => e.ExamDate)
                .ThenBy(e => e.ExamTime)
                .ToListAsync();
        }

        public async Task UpdateAsync(Exam exam)
        {
            exam.UpdatedAt = DateTime.UtcNow;
            _context.Exams.Update(exam);
            await _context.SaveChangesAsync();
        }
    }
}