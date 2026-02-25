using HUP.Repositories.Interfaces;
using HUP.Core.Entities.Academics;
using HUP.Data;
using Microsoft.EntityFrameworkCore;

namespace HUP.Repositories.Implementations
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(HupDbContext context) : base(context)
        {
        }

        public override async Task<Course> GetByIdReadOnly(Guid id) {
            var course = await _context.Courses
                .Include(c => c.Prerequisite)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            return course;
        }
        public override async Task<Course> GetByIdTracking(Guid id) {
            var course = await _context.Courses
                .Include(c => c.Prerequisite)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            return course;
        }

        public override async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Include(c => c.Prerequisite)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}