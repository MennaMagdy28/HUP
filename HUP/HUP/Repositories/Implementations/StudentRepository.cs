using HUP.Core.Entities.Academics;
using HUP.Core.Enums;
using HUP.Data;
using HUP.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HUP.Repositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly HupDbContext _context;
        public StudentRepository(HupDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Student entity)
        {
            await _context.Students.AddAsync(entity);
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetByFacultyAsync(Guid facultyId)
        {
            return await _context.Students
                .Where(s => s.Department.FacultyId == facultyId)
                .AsNoTracking()
                .ToListAsync();
        }

        // ---
        public async Task<Student> GetByIdReadOnly(Guid id)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Department)
                .Include(s=> s.Department.Faculty)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == id);
            return student;
        }

        public async Task<Student> GetByIdTracking(Guid id)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Department)
                .Include(s=> s.Department.Faculty)
                .FirstOrDefaultAsync(s => s.UserId == id);
            return student;
        }

        public async Task<IEnumerable<Student>> GetByDepartmentAsync(Guid departmentId)
        {
            var students = await _context.Students
                .Where(s => s.DepartmentId == departmentId)
                .AsNoTracking()
                .ToListAsync();
            return students;
        }

        public async Task RemoveAsync(Guid id)
        {
            var entity = await _context.Students.FindAsync(id);

            if (entity != null)
                _context.Students.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        // --- 
        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAcademicStatusAsync(Guid studentId, AcademicStatus status)
        {
            var student = await GetByIdReadOnly(studentId);
            if (student != null)
            {
                student.AcademicStatus = status;
                await UpdateAsync(student);
            }
        }
    }
}