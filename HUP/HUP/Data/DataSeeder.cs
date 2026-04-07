using Bogus;
using HUP.Core.Entities.Academics;
using HUP.Core.Entities.Identity;
using HUP.Core.Enums.AcademicEnums;
using HUP.Data;
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

        public DataSeeder(HupDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.Students.AnyAsync(s => s.UniversityEmail.EndsWith("@fakecs.hup.edu.eg")))
            {
                return; // Already seeded
            }

            var faker = new Faker();

            // Create Dean Role
            var deanRoleId = Guid.NewGuid();
            var deanRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "FacultyDean");
            if (deanRole == null)
            {
                deanRole = new Role { Id = deanRoleId, Name = "FacultyDean", DisplayName = "Faculty Dean", Description = "Faculty Dean Role" };
                await _context.Roles.AddAsync(deanRole);
            }
            else
            {
                deanRoleId = deanRole.Id;
            }

            // 1. Create a Faculty Dean User
            var deanUser = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Dean Smith",
                Email = "dean@fakecs.hup.edu.eg",
                NationalId = faker.Random.Replace("#############"),
                PasswordHash = "fakehash",
                PasswordExpiryDate = DateTime.UtcNow.AddYears(1),
                IsActive = true,
                RoleId = deanRoleId,
                PersonalInfo = new UserPersonalInfo
                {
                    BirthDate = faker.Date.Past(50, DateTime.UtcNow.AddYears(-40)),
                    Gender = HUP.Core.Enums.AcademicEnums.Gender.Male
                },
                ContactInfo = new UserContact
                {
                    PhoneNumber = "01000000000",
                    Address = "Dean Office"
                }
            };
            await _context.Users.AddAsync(deanUser);

            // 1b. Create a Faculty
            var faculty = new Faculty
            {
                Id = Guid.NewGuid(),
                Name = FacultyTitle.FacultyOfComputingAndAI,
                DisplayName = "Faculty of Computing and Artificial Intelligence",
                DeanId = deanUser.Id,
                DeanName = deanUser.FullName,
                ContactInfo = "01000000000"
            };
            await _context.Faculties.AddAsync(faculty);

            // 2. Create a Department
            var department = new Department
            {
                Id = Guid.NewGuid(),
                DepartmentName = "Computer Science",
                DepartmentCode = "CS",
                FacultyId = faculty.Id,
                DurationInYears = 4,
                CompulsoryHours = 120,
                ElectiveHours = 24
            };
            await _context.Departments.AddAsync(department);

            // 3. Create Courses (12 per year * 3 years = 36 courses)
            var allCourses = new List<Course>();
            var programPlans = new List<ProgramPlan>();

            string[] subjects = new string[] {
                "Introduction to Programming", "Mathematics I", "Physics I", "Digital Logic Design", "English Language I", "Human Rights",
                "Object Oriented Programming", "Mathematics II", "Physics II", "Electronics", "English Language II", "Computer Architecture",
                "Data Structures", "Discrete Mathematics", "Probability and Statistics", "Microprocessors", "System Analysis and Design", "Technical Writing",
                "Algorithms", "Linear Algebra", "Operations Research", "Database Systems", "Software Engineering", "Operating Systems",
                "Computer Networks", "Artificial Intelligence", "Computer Graphics", "Compiler Design", "Information Security", "Web Technologies",
                "Machine Learning", "Cloud Computing", "Data Mining", "Mobile Application Development", "Distributed Systems", "Software Testing"
            };

            for (int i = 0; i < 36; i++)
            {
                var course = new Course
                {
                    Id = Guid.NewGuid(),
                    CourseCode = $"CS{(i/12 + 1) * 100 + (i % 12 + 1)}",
                    CourseName = subjects[i],
                    Credits = 3
                };
                allCourses.Add(course);
                await _context.Courses.AddAsync(course);

                var plan = new ProgramPlan
                {
                    DepartmentId = department.Id,
                    CourseId = course.Id,
                    RequirementType = RequirementType.Department,
                    IsCompulsory = true,
                    FinalGrade = 100
                };
                programPlans.Add(plan);
                await _context.ProgramPlan.AddAsync(plan);
            }

            // 4. Create Semesters
            var semesters = new List<Semester>();
            var now = DateTime.UtcNow;

            // Past semesters
            for (int i = 0; i < 4; i++)
            {
                var pastSem = new Semester
                {
                    Id = Guid.NewGuid(),
                    SemesterName = $"Year {i / 2 + 1} - {(i % 2 == 0 ? "Fall" : "Spring")}",
                    StartDate = now.AddMonths(-((4 - i) * 6)),
                    EndDate = now.AddMonths(-((4 - i) * 6 - 4)),
                    RegistrationDeadline = now.AddMonths(-((4 - i) * 6)).AddDays(14),
                    DropDeadline = now.AddMonths(-((4 - i) * 6)).AddDays(28),
                    IsActive = false
                };
                semesters.Add(pastSem);
                await _context.Semesters.AddAsync(pastSem);
            }

            // Current semester (Year 3, Fall)
            var currentSemester = new Semester
            {
                Id = Guid.NewGuid(),
                SemesterName = "Year 3 - Fall",
                StartDate = now.AddMonths(-1),
                EndDate = now.AddMonths(3),
                RegistrationDeadline = now.AddDays(14),
                DropDeadline = now.AddDays(28),
                IsActive = true
            };
            await _context.Semesters.AddAsync(currentSemester);

            // 5. Create Course Offerings
            var courseOfferings = new List<CourseOffering>();

            // Offerings for past 4 semesters (6 courses each)
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    var courseIndex = i * 6 + j;
                    var offering = new CourseOffering
                    {
                        Id = Guid.NewGuid(),
                        CourseId = allCourses[courseIndex].Id,
                        SemesterId = semesters[i].Id,
                        DepartmentId = department.Id
                    };
                    courseOfferings.Add(offering);
                    await _context.CourseOfferings.AddAsync(offering);
                }
            }

            // Offerings for current semester (12 courses available for 3rd year)
            // They belong to Year 3, which are index 24 to 35
            for (int i = 24; i < 36; i++)
            {
                var offering = new CourseOffering
                {
                    Id = Guid.NewGuid(),
                    CourseId = allCourses[i].Id,
                    SemesterId = currentSemester.Id,
                    DepartmentId = department.Id
                };
                await _context.CourseOfferings.AddAsync(offering);
            }

            // 6. Create Student User Role if not exists
            var roleId = Guid.NewGuid();
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Student");
            if (role == null)
            {
                role = new Role { Id = roleId, Name = "Student", DisplayName = "Student", Description = "Student Role" };
                await _context.Roles.AddAsync(role);
            }
            else
            {
                roleId = role.Id;
            }

            // 7. Create 3rd Year Student
            var studentUser = new User
            {
                Id = Guid.NewGuid(),
                FullName = faker.Name.FullName(),
                Email = faker.Internet.Email(provider: "fakecs.hup.edu.eg"),
                NationalId = faker.Random.Replace("#############"), // 14 digits typical for Egypt
                PasswordHash = "fakehash", // Just dummy hash since it's seeder
                PasswordExpiryDate = now.AddYears(1),
                IsActive = true,
                RoleId = roleId,
                PersonalInfo = new UserPersonalInfo
                {
                    BirthDate = faker.Date.Past(20, now.AddYears(-20)),
                    Gender = faker.PickRandom<Gender>()
                },
                ContactInfo = new UserContact
                {
                    PhoneNumber = faker.Phone.PhoneNumber("010########"),
                    Address = faker.Address.FullAddress()
                }
            };
            await _context.Users.AddAsync(studentUser);

            var student = new Student
            {
                UserId = studentUser.Id,
                UniversityCode = faker.Random.Replace("202#00###"),
                UniversityEmail = studentUser.Email,
                AcademicStatus = AcademicStatus.Active,
                DepartmentId = department.Id,
                Level = 3,
                Cgpa = faker.Random.Decimal(2.0m, 4.0m),
                Group = "A"
            };
            await _context.Students.AddAsync(student);

            // 8. Enroll Student in past courses and assign grades
            var enrollments = new List<Enrollment>();
            foreach (var offering in courseOfferings) // These are the 24 past offerings
            {
                var enrollment = new Enrollment
                {
                    Id = Guid.NewGuid(),
                    StudentId = student.UserId,
                    CourseOfferingId = offering.Id,
                    EnrollmentDate = now.AddMonths(-12),
                    ClassGrade = faker.Random.Decimal(10, 20), // out of 20
                    MidtermGrade = faker.Random.Decimal(10, 20), // out of 20
                    finalGrade = faker.Random.Decimal(30, 60), // out of 60
                    Status = EnrollmentStatus.Completed
                };
                enrollments.Add(enrollment);
                await _context.Enrollments.AddAsync(enrollment);
            }

            await _context.SaveChangesAsync();
        }
    }
}