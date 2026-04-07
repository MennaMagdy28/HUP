using Bogus;
using HUP.Core.Entities.Academics;
using HUP.Core.Entities.Identity;
using HUP.Core.Enums.AcademicEnums;
using HUP.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HUP.Data.Seeders
{
    public class DataSeeder
    {
        private readonly HupDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public DataSeeder(HupDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            if (await _context.Students.AnyAsync(s => s.UniversityEmail.EndsWith("@fakecs.hup.edu.eg")))
            {
                // To allow testing this specific updated scenario, we drop early return,
                // OR we check for the specific new student email.
                // We'll check for "student1@fakecs.hup.edu.eg"
                if(await _context.Users.AnyAsync(u => u.Email == "student1@fakecs.hup.edu.eg"))
                    return;
            }

            var faker = new Faker();
            var now = DateTime.UtcNow;

            // --- ROLES ---
            var roles = new Dictionary<string, Role>();
            foreach (var rName in new[] { "FacultyDean", "Instructor", "Student" })
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == rName);
                if (role == null)
                {
                    role = new Role { Id = Guid.NewGuid(), Name = rName, DisplayName = rName, Description = $"{rName} Role" };
                    await _context.Roles.AddAsync(role);
                }
                roles[rName] = role;
            }

            // --- FACULTY ---
            var deanUser = new User
            {
                Id = Guid.NewGuid(), FullName = "Dean Smith", Email = "dean@fakecs.hup.edu.eg",
                NationalId = faker.Random.Replace("#############"), PasswordExpiryDate = now.AddYears(1),
                IsActive = true, RoleId = roles["FacultyDean"].Id,
                PersonalInfo = new UserPersonalInfo { BirthDate = faker.Date.Past(50, now.AddYears(-40)), Gender = Gender.Male },
                ContactInfo = new UserContact { PhoneNumber = "01000000000", Address = "Dean Office" }
            };
            deanUser.PasswordHash = _passwordHasher.HashPassword(deanUser, "Dean@123");
            await _context.Users.AddAsync(deanUser);

            var faculty = new Faculty
            {
                Id = Guid.NewGuid(), Name = FacultyTitle.FacultyOfComputingAndAI, DisplayName = "Faculty of Computing and AI",
                DeanId = deanUser.Id, DeanName = deanUser.FullName, ContactInfo = "01000000000"
            };
            await _context.Faculties.AddAsync(faculty);

            // --- DEPARTMENTS ---
            // General Department (Before specializing)
            var generalDept = new Department
            {
                Id = Guid.NewGuid(), DepartmentName = "General", DepartmentCode = "GEN", FacultyId = faculty.Id,
                DurationInYears = 4, CompulsoryHours = 40, ElectiveHours = 0
            };
            await _context.Departments.AddAsync(generalDept);

            var csDept = new Department
            {
                Id = Guid.NewGuid(), DepartmentName = "Computer Science", DepartmentCode = "CS", FacultyId = faculty.Id,
                DurationInYears = 4, CompulsoryHours = 80, ElectiveHours = 24
            };
            await _context.Departments.AddAsync(csDept);

            // --- INSTRUCTOR ---
            var instructorUser = new User
            {
                Id = Guid.NewGuid(), FullName = "Dr. Ahmed", Email = "dr.ahmed@fakecs.hup.edu.eg",
                NationalId = faker.Random.Replace("#############"), PasswordExpiryDate = now.AddYears(1),
                IsActive = true, RoleId = roles["Instructor"].Id,
                PersonalInfo = new UserPersonalInfo { BirthDate = faker.Date.Past(40, now.AddYears(-30)), Gender = Gender.Male },
                ContactInfo = new UserContact { PhoneNumber = "01011111111", Address = "Staff Room" }
            };
            instructorUser.PasswordHash = _passwordHasher.HashPassword(instructorUser, "Instructor@123");
            await _context.Users.AddAsync(instructorUser);

            var instructor = new Instructor
            {
                Id = Guid.NewGuid(), UserId = instructorUser.Id, DepartmentId = csDept.Id, AcademicTitle = AcademicTitle.Professor
            };
            await _context.Instructors.AddAsync(instructor);

            // --- COURSES ---
            var allCourses = new List<Course>();
            string[] subjects = new string[] {
                // Year 1 - Semester 1 (6 subjects)
                "Intro to Programming", "Math I", "Physics I", "English I", "Human Rights", "Discrete Math",
                // Year 1 - Semester 2 (6 subjects)
                "Object Oriented Prog", "Math II", "Physics II", "Electronics", "English II", "Technical Writing",
                // Year 1 - Summer (2 subjects)
                "Logic Design", "Probability & Stat",
                // Year 2 - Semester 1 (6 subjects)
                "Data Structures", "Computer Arch", "Algorithms", "System Analysis", "Linear Algebra", "Operations Research",
                // Year 2 - Semester 2 (Current Available - 10 subjects to choose from)
                "Database Systems", "Software Engineering", "Operating Systems", "Computer Networks", "Artificial Intelligence",
                "Computer Graphics", "Information Security", "Web Technologies", "Machine Learning", "Cloud Computing"
            };

            for (int i = 0; i < subjects.Length; i++)
            {
                var course = new Course
                {
                    Id = Guid.NewGuid(),
                    CourseCode = $"GEN{100 + i}",
                    CourseName = subjects[i],
                    Credits = 3
                };
                allCourses.Add(course);
                await _context.Courses.AddAsync(course);

                await _context.ProgramPlan.AddAsync(new ProgramPlan
                {
                    DepartmentId = generalDept.Id,
                    CourseId = course.Id,
                    RequirementType = RequirementType.Faculty,
                    IsCompulsory = true,
                    FinalGrade = 100
                });
            }

            // --- SEMESTERS ---
            // Year 1 Sem 1, Sem 2, Summer. Year 2 Sem 1. Year 2 Sem 2 (Current)
            var semsData = new[]
            {
                new { Name = "Year 1 - Fall", CoursesIdxStart = 0, CoursesCount = 6, IsActive = false, MonthsAgo = 18 },
                new { Name = "Year 1 - Spring", CoursesIdxStart = 6, CoursesCount = 6, IsActive = false, MonthsAgo = 12 },
                new { Name = "Year 1 - Summer", CoursesIdxStart = 12, CoursesCount = 2, IsActive = false, MonthsAgo = 8 },
                new { Name = "Year 2 - Fall", CoursesIdxStart = 14, CoursesCount = 6, IsActive = false, MonthsAgo = 6 },
                new { Name = "Year 2 - Spring", CoursesIdxStart = 20, CoursesCount = 10, IsActive = true, MonthsAgo = 0 }
            };

            var semesters = new List<Semester>();
            var pastOfferings = new List<CourseOffering>();
            var currentOfferings = new List<CourseOffering>();

            foreach(var sData in semsData)
            {
                var semester = new Semester
                {
                    Id = Guid.NewGuid(),
                    SemesterName = sData.Name,
                    StartDate = now.AddMonths(-sData.MonthsAgo),
                    EndDate = now.AddMonths(-sData.MonthsAgo + 3),
                    RegistrationDeadline = now.AddMonths(-sData.MonthsAgo).AddDays(14),
                    DropDeadline = now.AddMonths(-sData.MonthsAgo).AddDays(28),
                    IsActive = sData.IsActive
                };
                semesters.Add(semester);
                await _context.Semesters.AddAsync(semester);

                // Create Offerings
                for(int i = sData.CoursesIdxStart; i < sData.CoursesIdxStart + sData.CoursesCount; i++)
                {
                    var offering = new CourseOffering
                    {
                        Id = Guid.NewGuid(), CourseId = allCourses[i].Id, SemesterId = semester.Id, DepartmentId = generalDept.Id
                    };
                    await _context.CourseOfferings.AddAsync(offering);

                    if (!sData.IsActive)
                    {
                        pastOfferings.Add(offering);
                    }
                    else
                    {
                        currentOfferings.Add(offering);
                    }
                }
            }

            // --- CURRENT SEMESTER SCHEDULES (WITH OVERLAPS) ---
            var overlappingTimeSlots = new (System.DayOfWeek Day, TimeSpan Start, TimeSpan End)[]
            {
                (System.DayOfWeek.Sunday, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0)),
                (System.DayOfWeek.Sunday, new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0)), // Overlaps 8-10 & 10-12
                (System.DayOfWeek.Sunday, new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0)),

                (System.DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(11, 0, 0)), // 3 hours
                (System.DayOfWeek.Monday, new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0)),// Overlaps 8-11
                (System.DayOfWeek.Monday, new TimeSpan(12, 0, 0), new TimeSpan(14, 0, 0)),

                (System.DayOfWeek.Tuesday, new TimeSpan(12, 0, 0), new TimeSpan(14, 0, 0)),
                (System.DayOfWeek.Tuesday, new TimeSpan(13, 0, 0), new TimeSpan(15, 0, 0)) // Overlaps 12-14
            };

            foreach (var offering in currentOfferings)
            {
                // Each offering gets 3 varying schedules so student must choose
                for(int s=0; s<3; s++)
                {
                    var slot = faker.PickRandom(overlappingTimeSlots);
                    await _context.Schedules.AddAsync(new Schedule
                    {
                        Id = Guid.NewGuid(), CourseOfferingId = offering.Id, InstructorId = instructor.Id,
                        InstructorName = instructorUser.FullName, Group = $"G{s+1}", DayOfWeek = slot.Day,
                        StartTime = slot.Start, EndTime = slot.End, Hall = faker.PickRandom(new[]{"Hall A", "Hall B", "Lab 1"}),
                        TotalSeats = 30, AvailableSeats = 30
                    });
                }
            }

            // --- STUDENTS ---
            // Student 1: Active password
            var student1User = CreateStudentUser(faker, "student1@fakecs.hup.edu.eg", now.AddYears(1), roles["Student"].Id);
            student1User.PasswordHash = _passwordHasher.HashPassword(student1User, "Student@123");
            await _context.Users.AddAsync(student1User);

            var student1 = CreateStudentEntity(student1User, generalDept.Id, 2, "A"); // 2nd year
            await _context.Students.AddAsync(student1);

            // Student 2: Expired password
            var student2User = CreateStudentUser(faker, "student2@fakecs.hup.edu.eg", now.AddMonths(-1), roles["Student"].Id);
            student2User.PasswordHash = _passwordHasher.HashPassword(student2User, "Student@123");
            await _context.Users.AddAsync(student2User);

            var student2 = CreateStudentEntity(student2User, generalDept.Id, 2, "B");
            await _context.Students.AddAsync(student2);

            // --- ENROLLMENTS FOR STUDENTS (History) ---
            foreach (var offering in pastOfferings)
            {
                await _context.Enrollments.AddAsync(new Enrollment
                {
                    Id = Guid.NewGuid(), StudentId = student1.UserId, CourseOfferingId = offering.Id, EnrollmentDate = now.AddMonths(-6),
                    ClassGrade = faker.Random.Decimal(15, 20), MidtermGrade = faker.Random.Decimal(15, 20), finalGrade = faker.Random.Decimal(40, 60),
                    Status = EnrollmentStatus.Completed
                });

                await _context.Enrollments.AddAsync(new Enrollment
                {
                    Id = Guid.NewGuid(), StudentId = student2.UserId, CourseOfferingId = offering.Id, EnrollmentDate = now.AddMonths(-6),
                    ClassGrade = faker.Random.Decimal(10, 18), MidtermGrade = faker.Random.Decimal(10, 18), finalGrade = faker.Random.Decimal(30, 50),
                    Status = EnrollmentStatus.Completed
                });
            }

            await _context.SaveChangesAsync();
        }

        private User CreateStudentUser(Faker faker, string email, DateTime expiry, Guid roleId)
        {
            return new User
            {
                Id = Guid.NewGuid(), FullName = faker.Name.FullName(), Email = email,
                NationalId = faker.Random.Replace("#############"), PasswordExpiryDate = expiry,
                IsActive = true, RoleId = roleId,
                PersonalInfo = new UserPersonalInfo { BirthDate = faker.Date.Past(20, DateTime.UtcNow.AddYears(-19)), Gender = faker.PickRandom<Gender>() },
                ContactInfo = new UserContact { PhoneNumber = faker.Phone.PhoneNumber("010########"), Address = faker.Address.FullAddress() }
            };
        }

        private Student CreateStudentEntity(User user, Guid deptId, int level, string group)
        {
            return new Student
            {
                UserId = user.Id, UniversityCode = new Faker().Random.Replace("202#00###"),
                UniversityEmail = user.Email, AcademicStatus = AcademicStatus.Active,
                DepartmentId = deptId, Level = level, Cgpa = new Faker().Random.Decimal(2.5m, 4.0m), Group = group
            };
        }
    }
}