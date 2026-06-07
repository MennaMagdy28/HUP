using HUP.Core.Entities.Academics;
using HUP.Core.Entities.Identity;
using HUP.Core.Entities.Permissions;
using HUP.Core.Entities.Financial;
using HUP.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using HUP.Core.Enums;
using Gender = HUP.Core.Enums.Gender;
using DayOfWeek = HUP.Core.Enums.DayOfWeek;
using HUP.Core.Enums.Financial;

namespace HUP.Data
{
    public static class DatabaseSeeder
    {
        public static void Initialize(HupDbContext db)
        {
            if (db.Users.Any()) return; // DB has been seeded

            var hasher = new PasswordHasher<User>();

            // ===========================
            // 1. Roles & Permissions
            // ===========================
            var superAdminRole = new Role { Id = Guid.NewGuid(), Name = "SuperAdmin", DisplayName = "SuperAdmin", Description = "Super Administrator" };
            var studentRole = new Role { Id = Guid.NewGuid(), Name = "Student", DisplayName = "Student", Description = "Student Role" };
            var staffRole = new Role { Id = Guid.NewGuid(), Name = "Staff", DisplayName = "Staff", Description = "Staff Role" };

            db.Roles.AddRange(superAdminRole, studentRole, staffRole);

            var permissions = HUP.Core.Constants.AppPermissions.GetAll();
            foreach (var perm in permissions)
            {
                var permission = new Permission { Id = Guid.NewGuid(), Name = perm, DisplayName = perm, Description = perm };
                db.Permissions.Add(permission);
                db.RolePermissions.Add(new RolePermission { RoleId = superAdminRole.Id, PermissionId = permission.Id });
            }

            // ===========================
            // 2. Users
            // ===========================
            var adminUser = CreateUser(hasher, "00000000000000", "Admin@123", "System Admin", superAdminRole, "admin@system.com", "123456789");
            db.Users.Add(adminUser);

            // ===========================
            // 3. Faculties & Departments
            // ===========================
            var facEngineering = new Faculty
            {
                Id = Guid.NewGuid(),
                Name = FacultyTitle.FacultyOfEngineering,
                DisplayName = "Engineering",
                DeanName = "Dean Smith",
                ContactInfo = "contact@eng.com",
                DeanId = adminUser.Id,
                Departments = new List<Department>()
            };
            db.Faculties.Add(facEngineering);

            var deptSWE = new Department
            {
                Id = Guid.NewGuid(),
                DepartmentName = "{\"en\": \"Software Engineering\", \"ar\": \"هندسة البرمجيات\"}",
                DepartmentCode = "SWE",
                DurationInYears = 4,
                CompulsoryHours = 100,
                ElectiveHours = 32,
                FacultyId = facEngineering.Id,
                CourseOfferings = new List<CourseOffering>(),
                StaffMembers = new List<Staff>(),
                Programs = new List<ProgramPlan>()
            };
            db.Departments.Add(deptSWE);

            var deptCS = new Department
            {
                Id = Guid.NewGuid(),
                DepartmentName = "{\"en\": \"Computer Science\", \"ar\": \"علوم الحاسب\"}",
                DepartmentCode = "CS",
                DurationInYears = 4,
                CompulsoryHours = 100,
                ElectiveHours = 32,
                FacultyId = facEngineering.Id,
                CourseOfferings = new List<CourseOffering>(),
                StaffMembers = new List<Staff>(),
                Programs = new List<ProgramPlan>()
            };
            db.Departments.Add(deptCS);

            // ===========================
            // 4. Staff
            // ===========================
            var profUser = CreateUser(hasher, "22222222222222", "Staff@123", "Prof. Alan Turing", staffRole, "alan@univ.edu", "5551234");
            db.Users.Add(profUser);
            var profStaff = new Staff { UserId = profUser.Id, Title = StaffTitle.Professor, Category = StaffCategory.Academic, DepartmentId = deptSWE.Id, FacultyId = facEngineering.Id };
            db.Staff.Add(profStaff);

            var taUser = CreateUser(hasher, "33333333333333", "Staff@123", "TA. John Doe", staffRole, "john@univ.edu", "5555678");
            db.Users.Add(taUser);
            var taStaff = new Staff { UserId = taUser.Id, Title = StaffTitle.TeachingAssistant, Category = StaffCategory.Academic, DepartmentId = deptSWE.Id, FacultyId = facEngineering.Id };
            db.Staff.Add(taStaff);

            // ===========================
            // 5. Semesters
            // ===========================
            // Past Semesters
            var y1s1 = new Semester { Id = Guid.NewGuid(), SemesterName = "Fall 2022", StartDate = new DateTime(2022, 9, 1), EndDate = new DateTime(2023, 1, 15), RegistrationDeadline = new DateTime(2022, 9, 15), DropDeadline = new DateTime(2022, 10, 15), IsActive = false };
            var y1s2 = new Semester { Id = Guid.NewGuid(), SemesterName = "Spring 2023", StartDate = new DateTime(2023, 2, 1), EndDate = new DateTime(2023, 6, 15), RegistrationDeadline = new DateTime(2023, 2, 15), DropDeadline = new DateTime(2023, 3, 15), IsActive = false };
            var y1sum = new Semester { Id = Guid.NewGuid(), SemesterName = "Summer 2023", StartDate = new DateTime(2023, 7, 1), EndDate = new DateTime(2023, 8, 30), RegistrationDeadline = new DateTime(2023, 7, 10), DropDeadline = new DateTime(2023, 7, 25), IsActive = false };
            var y2s1 = new Semester { Id = Guid.NewGuid(), SemesterName = "Fall 2023", StartDate = new DateTime(2023, 9, 1), EndDate = new DateTime(2024, 1, 15), RegistrationDeadline = new DateTime(2023, 9, 15), DropDeadline = new DateTime(2023, 10, 15), IsActive = false };
            // Current Semester
            var currentSem = new Semester { Id = Guid.NewGuid(), SemesterName = "Spring 2024", StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddMonths(4), RegistrationDeadline = DateTime.UtcNow.AddDays(14), DropDeadline = DateTime.UtcNow.AddMonths(1), IsActive = true };

            db.Semesters.AddRange(y1s1, y1s2, y1sum, y2s1, currentSem);

            // ===========================
            // 6. Courses & Programs
            // ===========================
            var courses = new List<Course>();
            // Generate 24 courses to cover the student's history (6+6+2+6 = 20) + some for current semester
            for (int i = 1; i <= 24; i++)
            {
                var c = new Course
                {
                    Id = Guid.NewGuid(),
                    CourseCode = $"SWE10{i}",
                    CourseName = $"{{\"en\": \"Software Eng Course {i}\", \"ar\": \"مقرر هندسة برمجيات {i}\"}}",
                    Credits = 3
                };
                courses.Add(c);
                db.Courses.Add(c);
                db.Set<ProgramPlan>().Add(new ProgramPlan { CourseId = c.Id, DepartmentId = deptSWE.Id, RequirementType = RequirementType.Department, IsCompulsory = true });
            }

            // ===========================
            // 7. Students
            // ===========================

            // Student A (The specific student: Year 2 Semester 2)
            var stuAUser = CreateUser(hasher, "11111111111111", "Student@123", "Target Student", studentRole, "target@1001.com", "111");
            db.Users.Add(stuAUser);
            var studentA = new Student { UserId = stuAUser.Id, UniversityCode = "STU1001", UniversityEmail = "stu@1001.com", AcademicStatus = AcademicStatus.Active, Cgpa = 3.8m, Level = 2, Group = "A", DepartmentId = deptSWE.Id };
            db.Students.Add(studentA);

            // Student B (Freshman)
            var stuBUser = CreateUser(hasher, "11111111111112", "Student@123", "Freshman Student", studentRole, "fresh@1002.com", "222");
            db.Users.Add(stuBUser);
            var studentB = new Student { UserId = stuBUser.Id, UniversityCode = "STU1002", UniversityEmail = "stu@1002.com", AcademicStatus = AcademicStatus.Active, Cgpa = 0m, Level = 1, Group = "B", DepartmentId = deptSWE.Id };
            db.Students.Add(studentB);

            // Student C (Senior)
            var stuCUser = CreateUser(hasher, "11111111111113", "Student@123", "Senior Student", studentRole, "senior@1003.com", "333");
            db.Users.Add(stuCUser);
            var studentC = new Student { UserId = stuCUser.Id, UniversityCode = "STU1003", UniversityEmail = "stu@1003.com", AcademicStatus = AcademicStatus.Active, Cgpa = 3.2m, Level = 4, Group = "C", DepartmentId = deptCS.Id };
            db.Students.Add(studentC);

            // ===========================
            // 8. CourseOfferings, Enrollments & Schedules (Past History for Student A)
            // ===========================
            int courseIndex = 0;

            void SeedPastSemester(Semester sem, int numCourses)
            {
                for (int i = 0; i < numCourses; i++)
                {
                    var c = courses[courseIndex++];
                    var co = new CourseOffering { Id = Guid.NewGuid(), CourseId = c.Id, SemesterId = sem.Id, DepartmentId = deptSWE.Id };
                    db.CourseOfferings.Add(co);

                    var sched = new Schedule { Id = Guid.NewGuid(), CourseOfferingId = co.Id, StaffId = profStaff.UserId, Group = "A", DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(9,0,0), EndTime = new TimeSpan(11,0,0), Hall = "Hall 1", StaffName = "Prof. Alan Turing", TotalSeats = 50, AvailableSeats = 49 };
                    db.Schedules.Add(sched);

                    db.Enrollments.Add(new Enrollment { StudentId = studentA.UserId, CourseOfferingId = co.Id, ScheduleId = sched.Id, EnrollmentDate = sem.StartDate, Status = EnrollmentStatus.Completed, ClassGrade = 20, MidtermGrade = 20, finalGrade = 50 });
                }
            }

            // Y1 S1: 6 subjects
            SeedPastSemester(y1s1, 6);
            // Y1 S2: 6 subjects
            SeedPastSemester(y1s2, 6);
            // Y1 Summer: 2 subjects
            SeedPastSemester(y1sum, 2);
            // Y2 S1: 6 subjects
            SeedPastSemester(y2s1, 6);

            // ===========================
            // 9. Current Semester Offerings & Conflicting Schedules
            // ===========================
            // The remaining 4 courses will be available for Student A in the current semester
            for (int i = courseIndex; i < courses.Count; i++)
            {
                var c = courses[i];
                var co = new CourseOffering { Id = Guid.NewGuid(), CourseId = c.Id, SemesterId = currentSem.Id, DepartmentId = deptSWE.Id };
                db.CourseOfferings.Add(co);

                // Create multiple timeslots for each offering
                var sched1 = new Schedule { Id = Guid.NewGuid(), CourseOfferingId = co.Id, StaffId = profStaff.UserId, Group = "A1", DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(12, 0, 0), Hall = "Hall A", StaffName = "Prof. Alan Turing", TotalSeats = 30, AvailableSeats = 30 };
                var sched2 = new Schedule { Id = Guid.NewGuid(), CourseOfferingId = co.Id, StaffId = taStaff.UserId, Group = "A2", DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(16, 0, 0), Hall = "Hall B", StaffName = "TA. John Doe", TotalSeats = 30, AvailableSeats = 30 };

                db.Schedules.AddRange(sched1, sched2);

                // Introduce conflicts on Monday 10:00-12:00
                if (i == courseIndex + 1) // Force the second available course to conflict with the first one
                {
                    var schedConflict = new Schedule { Id = Guid.NewGuid(), CourseOfferingId = co.Id, StaffId = profStaff.UserId, Group = "A3 (Conflict)", DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(13, 0, 0), Hall = "Hall C", StaffName = "Prof. Alan Turing", TotalSeats = 30, AvailableSeats = 30 };
                    db.Schedules.Add(schedConflict);
                }
            }

            // ===========================
            // 10. Financials
            // ===========================
            var tuitionFee = new Fee { Id = Guid.NewGuid(), Name = "Tuition Spring 2024", Description = "Tuition fee for Spring 2024", Amount = 500, IsPerCredit = true, Type = FeeType.Tuition, SemesterId = currentSem.Id };
            var busFee = new Fee { Id = Guid.NewGuid(), Name = "Bus Spring 2024", Description = "Bus fee for Spring 2024", Amount = 1500, IsPerCredit = false, Type = FeeType.Bus, SemesterId = currentSem.Id };
            db.Fees.AddRange(tuitionFee, busFee);

            var studentFeeA = new StudentFee { Id = Guid.NewGuid(), StudentId = studentA.UserId, FeeId = tuitionFee.Id, Amount = 6000, PaidAmount = 2000, Status = FeeStatus.PartiallyPaid, DueDate = DateTime.UtcNow.AddMonths(1) };
            db.StudentFees.Add(studentFeeA);

            db.Payments.Add(new Payment { Id = Guid.NewGuid(), StudentFeeId = studentFeeA.Id, Amount = 2000, PaymentDate = DateTime.UtcNow.AddDays(-5), Method = PaymentMethod.Online, ReferenceNumber = "TXN123456" });

            db.SaveChanges();
        }

        private static User CreateUser(PasswordHasher<User> hasher, string nationalId, string pwd, string fullName, Role role, string email, string phone)
        {
            var u = new User
            {
                Id = Guid.NewGuid(),
                NationalId = nationalId,
                FullName = fullName,
                IsActive = true,
                RoleId = role.Id,
                PersonalInfo = new UserPersonalInfo { Gender = Gender.Male, BirthDate = new DateTime(2000, 1, 1), Nationality = Nationality.Egyptian },
                ContactInfo = new UserContact { AltEmail = email, PhoneNumber = phone }
            };
            u.PasswordHash = hasher.HashPassword(u, pwd);
            return u;
        }
    }
}
