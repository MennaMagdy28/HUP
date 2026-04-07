using HUP.Core.Entities.Academics;
using HUP.Core.Entities.Identity;
using HUP.Core.Entities.Permissions;
using HUP.Core.Enums.AcademicEnums;
using HUP.Core.Enums.IdentityEnums;
using HUP.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HUP.Tests
{
    public static class DatabaseSeeder
    {
        public static void Initialize(HupDbContext db)
        {
            if (db.Users.Any()) return;

            var hasher = new PasswordHasher<User>();
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                NationalId = "00000000000000",
                PasswordHash = hasher.HashPassword(null!, "Admin@123"),
                FullName = "System Admin",
                IsActive = true
            };
            var studentUser = new User
            {
                Id = Guid.NewGuid(),
                NationalId = "11111111111111",
                PasswordHash = hasher.HashPassword(null!, "Student@123"),
                FullName = "Test Student",
                IsActive = true
            };

            var superAdminRole = new Role { Id = Guid.NewGuid(), Name = "SuperAdmin", DisplayName = "SuperAdmin", Description = "Super Administrator" };
            var studentRole = new Role { Id = Guid.NewGuid(), Name = "Student", DisplayName = "Student", Description = "Student" };
            db.Roles.AddRange(superAdminRole, studentRole);

            var permissions = HUP.Core.Constants.AppPermissions.GetAll();
            foreach (var perm in permissions)
            {
                var permission = new Permission { Id = Guid.NewGuid(), Name = perm, DisplayName = perm, Description = perm };
                db.Permissions.Add(permission);
                db.RolePermissions.Add(new RolePermission { RoleId = superAdminRole.Id, PermissionId = permission.Id });
            }

            adminUser.RoleId = superAdminRole.Id;
            adminUser.PersonalInfo = new UserPersonalInfo { Gender = HUP.Core.Enums.AcademicEnums.Gender.Male, BirthDate = new DateTime(1980, 1, 1), Nationality = Nationality.Egyptian };
            adminUser.ContactInfo = new UserContact { AltEmail = "admin@system.com", PhoneNumber = "123456789" };
            adminUser.UserRole = superAdminRole;

            studentUser.RoleId = studentRole.Id;
            studentUser.PersonalInfo = new UserPersonalInfo { Gender = HUP.Core.Enums.AcademicEnums.Gender.Female, BirthDate = new DateTime(2000, 1, 1), Nationality = Nationality.Egyptian };
            studentUser.ContactInfo = new UserContact { AltEmail = "student@system.com", PhoneNumber = "987654321" };
            studentUser.UserRole = studentRole;

            db.Users.Add(adminUser);
            db.Users.Add(studentUser);

            var faculty = new Faculty { Id = Guid.NewGuid(), Name = FacultyTitle.FacultyOfScience, DisplayName = "Engineering", DeanName = "Dean Smith", ContactInfo = "contact@eng.com", Dean = adminUser, DeanId = adminUser.Id, Departments = new List<Department>() };
            db.Faculties.Add(faculty);

            var department = new Department { Id = Guid.NewGuid(), DepartmentName = "{\"en\": \"Computer Science\"}", DepartmentCode = "CS", FacultyId = faculty.Id, Faculty = faculty, CourseOfferings = new List<CourseOffering>(), StaffMembers = new List<Staff>(), Programs = new List<ProgramPlan>() };
            db.Departments.Add(department);

            var program = new ProgramPlan { CourseId = Guid.NewGuid(), DepartmentId = department.Id, Department = department, Course = new Course{ CourseCode="1", CourseName="1", Enrollments=new List<Enrollment>(), CourseOfferings=new List<CourseOffering>(), Programs=new List<ProgramPlan>()} };
            db.Set<ProgramPlan>().Add(program);

            var student = new Student
            {
                UserId = studentUser.Id,
                UniversityCode = "STU1001",
                UniversityEmail = "stu@1001.com",
                AcademicStatus = AcademicStatus.Active,
                Cgpa = 3.5m,
                Level = 1,
                Group = "A",
                DepartmentId = department.Id,
                Department = department,
                User = studentUser,
                Enrollments = new List<Enrollment>()
            };
            db.Students.Add(student);

            var course = new Course
            {
                Id = Guid.NewGuid(),
                CourseCode = "CS101",
                CourseName = "{\"en\": \"Intro to Programming\"}",
                Credits = 3,
                Prerequisite = null,
                Enrollments = new List<Enrollment>(), CourseOfferings=new List<CourseOffering>(), Programs=new List<ProgramPlan>()
            };
            db.Courses.Add(course);

            var semester = new Semester { Id = Guid.NewGuid(), SemesterName = "Fall 2024", StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddMonths(4), IsActive = true };
            db.Semesters.Add(semester);

            var courseOffering = new CourseOffering
            {
                Id = Guid.NewGuid(),
                CourseId = course.Id,
                SemesterId = semester.Id,
                DepartmentId = department.Id,
                Department = department,
                Course = course,
                Semester = semester,
                Schedules = new List<Schedule>(),
                Exams = new List<Exam>(),
                Enrollments = new List<Enrollment>()
            };
            db.CourseOfferings.Add(courseOffering);

            db.SaveChanges();
        }
    }
}
