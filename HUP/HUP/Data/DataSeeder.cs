using HUP.Core.Entities.Academics;
using HUP.Core.Entities.Identity;
using HUP.Core.Entities.Permissions;
using HUP.Core.Enums.AcademicEnums;
using HUP.Core.Enums.IdentityEnums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DayOfWeek = HUP.Core.Enums.AcademicEnums.DayOfWeek;
using Gender = HUP.Core.Enums.AcademicEnums.Gender;

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
            // التأكد من أن قاعدة البيانات تم إنشاؤها
            await _context.Database.EnsureCreatedAsync();

            try
            {
                // اذا كان فيه مستخدم موجود معناها الداتا موجودة
                if (await _context.Users.AnyAsync(u => u.Email == "admin@university.edu"))
                    return;

                Console.WriteLine("بدء عملية البذر بترتيب منطقي...");
                await SeedPermissionsAsync(_context);
                await SeedRolesAsync(_context);
                await SeedUsersAsync(_context, _passwordHasher);
                await SeedFacultiesAsync(_context);
                await SeedDepartmentsAsync(_context);
                await SeedInstructorsAsync(_context);
                await UpdateDepartmentHeadsAsync(_context);
                await SeedCoursesAsync(_context);
                await SeedProgramPlansAsync(_context);
                await SeedSemestersAsync(_context);
                await SeedStudentsAsync(_context);
                await SeedCourseOfferingsAsync(_context);
                await SeedSchedulesAsync(_context);
                await SeedExamsAsync(_context);
                await SeedEnrollmentsAsync(_context);

                Console.WriteLine("✅ جميع البيانات تم بذرها بنجاح!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطأ في بذر البيانات: {ex.Message}");
                throw;
            }
        }

        private async Task SeedPermissionsAsync(HupDbContext context)
        {
            if (!await context.Permissions.AnyAsync())
            {
                var permissions = new List<Permission>
                {
                    new Permission { Id = Guid.NewGuid(), Name = "User.View", DisplayName = "عرض المستخدمين", Description = "عرض قائمة المستخدمين", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "User.Create", DisplayName = "إنشاء مستخدم", Description = "إضافة مستخدم جديد", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "User.Edit", DisplayName = "تعديل مستخدم", Description = "تعديل بيانات المستخدم", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "User.Delete", DisplayName = "حذف مستخدم", Description = "حذف مستخدم", CreatedAt = DateTime.UtcNow },

                    new Permission { Id = Guid.NewGuid(), Name = "Student.View", DisplayName = "عرض الطلاب", Description = "عرض قائمة الطلاب", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "Student.Create", DisplayName = "إنشاء طالب", Description = "إضافة طالب جديد", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "Student.Edit", DisplayName = "تعديل طالب", Description = "تعديل بيانات الطالب", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "Student.Delete", DisplayName = "حذف طالب", Description = "حذف طالب", CreatedAt = DateTime.UtcNow },

                    new Permission { Id = Guid.NewGuid(), Name = "Instructor.View", DisplayName = "عرض الأساتذة", Description = "عرض قائمة الأساتذة", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "Instructor.Create", DisplayName = "إنشاء أستاذ", Description = "إضافة أستاذ جديد", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "Instructor.Edit", DisplayName = "تعديل أستاذ", Description = "تعديل بيانات الأستاذ", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "Instructor.Delete", DisplayName = "حذف أستاذ", Description = "حذف أستاذ", CreatedAt = DateTime.UtcNow },

                    new Permission { Id = Guid.NewGuid(), Name = "Course.View", DisplayName = "عرض المواد", Description = "عرض قائمة المواد", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "Course.Create", DisplayName = "إنشاء مادة", Description = "إضافة مادة جديدة", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "Course.Edit", DisplayName = "تعديل مادة", Description = "تعديل بيانات المادة", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "Course.Delete", DisplayName = "حذف مادة", Description = "حذف مادة", CreatedAt = DateTime.UtcNow },

                    new Permission { Id = Guid.NewGuid(), Name = "Enrollment.View", DisplayName = "عرض التسجيلات", Description = "عرض قائمة التسجيلات", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "Enrollment.Create", DisplayName = "إنشاء تسجيل", Description = "تسجيل طالب في مادة", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "Enrollment.Edit", DisplayName = "تعديل تسجيل", Description = "تعديل بيانات التسجيل", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "Enrollment.Delete", DisplayName = "حذف تسجيل", Description = "حذف تسجيل", CreatedAt = DateTime.UtcNow },

                    new Permission { Id = Guid.NewGuid(), Name = "Grade.Manage", DisplayName = "إدارة الدرجات", Description = "إدخال وتعديل درجات الطلاب", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "Schedule.View", DisplayName = "عرض الجداول", Description = "عرض الجداول الدراسية", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "Schedule.Manage", DisplayName = "إدارة الجداول", Description = "إنشاء وتعديل الجداول", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "Report.View", DisplayName = "عرض التقارير", Description = "عرض التقارير الإحصائية", CreatedAt = DateTime.UtcNow },

                    new Permission { Id = Guid.NewGuid(), Name = "Role.Manage", DisplayName = "إدارة الصلاحيات", Description = "إدارة الأدوار والصلاحيات", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "System.Config", DisplayName = "إعدادات النظام", Description = "تعديل إعدادات النظام", CreatedAt = DateTime.UtcNow }
                };

                await context.Permissions.AddRangeAsync(permissions);
                await context.SaveChangesAsync();
            }
        }

        private async Task SeedRolesAsync(HupDbContext context)
        {
            if (!await context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { Id = Guid.NewGuid(), Name = "Admin", DisplayName = "{\"en\":\"System Admin\",\"ar\":\"مدير النظام\"}", Description = "مدير النظام الكامل", CreatedAt = DateTime.UtcNow },
                    new Role { Id = Guid.NewGuid(), Name = "Student", DisplayName = "{\"en\":\"Student\",\"ar\":\"طالب\"}", Description = "طالب في الجامعة", CreatedAt = DateTime.UtcNow },
                    new Role { Id = Guid.NewGuid(), Name = "Instructor", DisplayName = "{\"en\":\"Instructor\",\"ar\":\"أستاذ\"}", Description = "أستاذ جامعي", CreatedAt = DateTime.UtcNow },
                    new Role { Id = Guid.NewGuid(), Name = "DepartmentHead", DisplayName = "{\"en\":\"Department Head\",\"ar\":\"رئيس قسم\"}", Description = "رئيس قسم أكاديمي", CreatedAt = DateTime.UtcNow },
                    new Role { Id = Guid.NewGuid(), Name = "FacultyDean", DisplayName = "{\"en\":\"Faculty Dean\",\"ar\":\"عميد كلية\"}", Description = "عميد كلية", CreatedAt = DateTime.UtcNow }
                };

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();

                await SeedRolePermissionsAsync(context);
            }
        }

        private async Task SeedRolePermissionsAsync(HupDbContext context)
        {
            if (!await context.RolePermissions.AnyAsync())
            {
                var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
                var studentRole = await context.Roles.FirstAsync(r => r.Name == "Student");
                var instructorRole = await context.Roles.FirstAsync(r => r.Name == "Instructor");
                var deptHeadRole = await context.Roles.FirstAsync(r => r.Name == "DepartmentHead");

                var allPermissions = await context.Permissions.ToListAsync();
                var rolePermissions = new List<RolePermission>();

                // مدير النظام: جميع الصلاحيات
                foreach (var permission in allPermissions)
                {
                    rolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = permission.Id });
                }

                // الطالب: صلاحيات محدودة
                var pSchedView = allPermissions.FirstOrDefault(p => p.Name == "Schedule.View");
                var pCourseView = allPermissions.FirstOrDefault(p => p.Name == "Course.View");
                if (pSchedView != null) rolePermissions.Add(new RolePermission { RoleId = studentRole.Id, PermissionId = pSchedView.Id });
                if (pCourseView != null) rolePermissions.Add(new RolePermission { RoleId = studentRole.Id, PermissionId = pCourseView.Id });

                await context.RolePermissions.AddRangeAsync(rolePermissions);
                await context.SaveChangesAsync();
            }
        }

        private async Task SeedUsersAsync(HupDbContext context, IPasswordHasher<User> passwordHasher)
        {
            if (!await context.Users.AnyAsync())
            {
                var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
                var studentRole = await context.Roles.FirstAsync(r => r.Name == "Student");
                var instructorRole = await context.Roles.FirstAsync(r => r.Name == "Instructor");
                var deptHeadRole = await context.Roles.FirstAsync(r => r.Name == "DepartmentHead");
                var deanRole = await context.Roles.FirstAsync(r => r.Name == "FacultyDean");

                var users = new List<User>();

                // مدير النظام
                var adminUser = new User
                {
                    Id = Guid.NewGuid(), NationalId = "1234567890", Email = "admin@university.edu",
                    PasswordExpiryDate = DateTime.Now.AddMonths(6), FullName = "{\"en\":\"Ahmed Mohamed Ali\",\"ar\":\"أحمد محمد علي\"}",
                    RoleId = adminRole.Id, IsActive = true, CreatedAt = DateTime.Now.AddDays(-365),
                    PersonalInfo = new UserPersonalInfo { Gender = Gender.Male, BirthDate = new DateTime(1980, 5, 15), Religion = Religion.Muslim, Nationality = Nationality.Egyptian },
                    ContactInfo = new UserContact { Address = "شارع الجامعة، القاهرة", PhoneNumber = "01012345678", AltEmail = "ahmed.ali@email.com" }
                };
                adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin@123");
                users.Add(adminUser);

                // طلاب
                var student1User = new User
                {
                    Id = Guid.NewGuid(), NationalId = "2233445566", Email = "student1@university.edu",
                    PasswordExpiryDate = DateTime.Now.AddMonths(6), FullName = "{\"en\":\"Mohamed Khaled Saeed\",\"ar\":\"محمد خالد سعيد\"}",
                    RoleId = studentRole.Id, IsActive = true, CreatedAt = DateTime.Now.AddDays(-200),
                    PersonalInfo = new UserPersonalInfo { Gender = Gender.Male, BirthDate = new DateTime(2002, 8, 20), Religion = Religion.Muslim, Nationality = Nationality.Egyptian },
                    ContactInfo = new UserContact { Address = "حي سموحة، الإسكندرية", PhoneNumber = "01098765432", AltEmail = "mohamed.khaled@email.com" }
                };
                student1User.PasswordHash = passwordHasher.HashPassword(student1User, "Student@123");
                users.Add(student1User);

                var student2User = new User
                {
                    Id = Guid.NewGuid(), NationalId = "3344556677", Email = "student2@university.edu",
                    PasswordExpiryDate = DateTime.Now.AddMonths(-1), // ⭐ Expired Password
                    FullName = "{\"en\":\"Fatma Ali Hassan\",\"ar\":\"فاطمة علي حسن\"}", RoleId = studentRole.Id, IsActive = true, CreatedAt = DateTime.Now.AddDays(-190),
                    PersonalInfo = new UserPersonalInfo { Gender = Gender.Female, BirthDate = new DateTime(2003, 3, 12), Religion = Religion.Muslim, Nationality = Nationality.Egyptian },
                    ContactInfo = new UserContact { Address = "الدقي، الجيزة", PhoneNumber = "01122334455", AltEmail = "fatma.ali@email.com" }
                };
                student2User.PasswordHash = passwordHasher.HashPassword(student2User, "Student@123"); // Same password
                users.Add(student2User);

                // أساتذة
                var profUser = new User
                {
                    Id = Guid.NewGuid(), NationalId = "4455667788", Email = "prof.ahmed@university.edu",
                    PasswordExpiryDate = DateTime.Now.AddMonths(6), FullName = "{\"en\":\"Dr. Ahmed Mahmoud Abdullah\",\"ar\":\"د. أحمد محمود عبد الله\"}",
                    RoleId = instructorRole.Id, IsActive = true, CreatedAt = DateTime.Now.AddDays(-400),
                    PersonalInfo = new UserPersonalInfo { Gender = Gender.Male, BirthDate = new DateTime(1975, 11, 5), Religion = Religion.Muslim, Nationality = Nationality.Egyptian },
                    ContactInfo = new UserContact { Address = "المهندسين، الجيزة", PhoneNumber = "01233445566", AltEmail = "ahmed.mahmoud@email.com" }
                };
                profUser.PasswordHash = passwordHasher.HashPassword(profUser, "Professor@123");
                users.Add(profUser);

                var drSaraUser = new User
                {
                    Id = Guid.NewGuid(), NationalId = "5566778899", Email = "dr.sara@university.edu",
                    PasswordExpiryDate = DateTime.Now.AddMonths(6), FullName = "{\"en\":\"Dr. Sara Mohamed Fouad\",\"ar\":\"د. سارة محمد فؤاد\"}",
                    RoleId = instructorRole.Id, IsActive = true, CreatedAt = DateTime.Now.AddDays(-350),
                    PersonalInfo = new UserPersonalInfo { Gender = Gender.Female, BirthDate = new DateTime(1985, 7, 25), Religion = Religion.Muslim, Nationality = Nationality.Egyptian },
                    ContactInfo = new UserContact { Address = "المنصورة الجديدة", PhoneNumber = "01055667788", AltEmail = "sara.mohamed@email.com" }
                };
                drSaraUser.PasswordHash = passwordHasher.HashPassword(drSaraUser, "Professor@123");
                users.Add(drSaraUser);

                // رئيس قسم
                var hodUser = new User
                {
                    Id = Guid.NewGuid(), NationalId = "6677889900", Email = "head.cs@university.edu",
                    PasswordExpiryDate = DateTime.Now.AddMonths(6), FullName = "{\"en\":\"Dr. Mohamed Ibrahim Hussein\",\"ar\":\"د. محمد إبراهيم حسين\"}",
                    RoleId = deptHeadRole.Id, IsActive = true, CreatedAt = DateTime.Now.AddDays(-500),
                    PersonalInfo = new UserPersonalInfo { Gender = Gender.Male, BirthDate = new DateTime(1970, 2, 18), Religion = Religion.Muslim, Nationality = Nationality.Egyptian },
                    ContactInfo = new UserContact { Address = "مدينة نصر، القاهرة", PhoneNumber = "01066778899", AltEmail = "mohamed.ibrahim@email.com" }
                };
                hodUser.PasswordHash = passwordHasher.HashPassword(hodUser, "Head@123");
                users.Add(hodUser);

                // عميد كلية
                var deanUser = new User
                {
                    Id = Guid.NewGuid(), NationalId = "7788990011", Email = "dean.cs@university.edu",
                    PasswordExpiryDate = DateTime.Now.AddMonths(6), FullName = "{\"en\":\"Dr. Ali Suleiman Mohamed\",\"ar\":\"د. علي سليمان محمد\"}",
                    RoleId = deanRole.Id, IsActive = true, CreatedAt = DateTime.Now.AddDays(-600),
                    PersonalInfo = new UserPersonalInfo { Gender = Gender.Male, BirthDate = new DateTime(1965, 12, 8), Religion = Religion.Muslim, Nationality = Nationality.Egyptian },
                    ContactInfo = new UserContact { Address = "الزمالك، القاهرة", PhoneNumber = "01077889900", AltEmail = "ali.suleiman@email.com" }
                };
                deanUser.PasswordHash = passwordHasher.HashPassword(deanUser, "Dean@123");
                users.Add(deanUser);

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();

                foreach (var u in users) { u.PersonalInfo.UserId = u.Id; u.ContactInfo.UserId = u.Id; }
                await context.SaveChangesAsync();
            }
        }

        private async Task SeedFacultiesAsync(HupDbContext context)
        {
            if (!await context.Faculties.AnyAsync())
            {
                var dean = await context.Users.FirstAsync(u => u.Email == "dean.cs@university.edu");

                var faculties = new List<Faculty>
                {
                    new Faculty
                    {
                        Id = Guid.NewGuid(), Name = FacultyTitle.FacultyOfComputingAndAI, DisplayName = "{\"en\":\"Faculty of Computing and AI\",\"ar\":\"كلية الحاسبات والذكاء الاصطناعي\"}",
                        DeanId = dean.Id, DeanName = "{\"en\":\"Dr. Ali Suleiman Mohamed\",\"ar\":\"د. علي سليمان محمد\"}", ContactInfo = "هاتف: 02-12345678", CreatedAt = DateTime.Now.AddDays(-700)
                    },
                    new Faculty
                    {
                        Id = Guid.NewGuid(), Name = FacultyTitle.FacultyOfEngineering, DisplayName = "{\"en\":\"Faculty of Engineering\",\"ar\":\"كلية الهندسة\"}",
                        DeanId = dean.Id, DeanName = "{\"en\":\"Dr. Ali Suleiman Mohamed\",\"ar\":\"د. علي سليمان محمد\"}", ContactInfo = "هاتف: 02-23456789", CreatedAt = DateTime.Now.AddDays(-680)
                    },
                    new Faculty
                    {
                        Id = Guid.NewGuid(), Name = FacultyTitle.FacultyOfCommerceAndBusinessAdministration, DisplayName = "{\"en\":\"Faculty of Commerce and Business Admin\",\"ar\":\"كلية التجارة وإدارة الأعمال\"}",
                        DeanId = dean.Id, DeanName = "{\"en\":\"Dr. Ali Suleiman Mohamed\",\"ar\":\"د. علي سليمان محمد\"}", ContactInfo = "هاتف: 02-34567890", CreatedAt = DateTime.Now.AddDays(-660)
                    }
                };

                await context.Faculties.AddRangeAsync(faculties);
                await context.SaveChangesAsync();
            }
        }

        private async Task SeedDepartmentsAsync(HupDbContext context)
        {
            if (!await context.Departments.AnyAsync())
            {
                var csFaculty = await context.Faculties.FirstAsync(f => f.Name == FacultyTitle.FacultyOfComputingAndAI);
                var engFaculty = await context.Faculties.FirstAsync(f => f.Name == FacultyTitle.FacultyOfEngineering);

                var departments = new List<Department>
                {
                    // General Department for unassigned students (requirement: "doesn't have a department")
                    new Department { Id = Guid.NewGuid(), FacultyId = csFaculty.Id, DepartmentName = "{\"en\":\"General\",\"ar\":\"عام\"}", DepartmentCode = "GEN", DurationInYears = 4, CompulsoryHours = 120, ElectiveHours = 20, CreatedAt = DateTime.Now.AddDays(-650) },
                    new Department { Id = Guid.NewGuid(), FacultyId = csFaculty.Id, DepartmentName = "{\"en\":\"Computer Science\",\"ar\":\"علوم الحاسب\"}", DepartmentCode = "CS", DurationInYears = 4, CompulsoryHours = 120, ElectiveHours = 20, CreatedAt = DateTime.Now.AddDays(-650) },
                    new Department { Id = Guid.NewGuid(), FacultyId = csFaculty.Id, DepartmentName = "{\"en\":\"Information Systems\",\"ar\":\"نظم المعلومات\"}", DepartmentCode = "IS", DurationInYears = 4, CompulsoryHours = 115, ElectiveHours = 25, CreatedAt = DateTime.Now.AddDays(-640) }
                };

                await context.Departments.AddRangeAsync(departments);
                await context.SaveChangesAsync();
            }
        }

        private async Task SeedInstructorsAsync(HupDbContext context)
        {
            if (!await context.Instructors.AnyAsync())
            {
                var csDept = await context.Departments.FirstAsync(d => d.DepartmentCode == "CS");
                var isDept = await context.Departments.FirstAsync(d => d.DepartmentCode == "IS");

                var profAhmed = await context.Users.FirstAsync(u => u.Email == "prof.ahmed@university.edu");
                var drSara = await context.Users.FirstAsync(u => u.Email == "dr.sara@university.edu");
                var headOfDept = await context.Users.FirstAsync(u => u.Email == "head.cs@university.edu");

                var instructors = new List<Instructor>
                {
                    new Instructor { Id = Guid.NewGuid(), UserId = profAhmed.Id, DepartmentId = csDept.Id, AcademicTitle = AcademicTitle.Professor, CreatedAt = DateTime.Now.AddDays(-400) },
                    new Instructor { Id = Guid.NewGuid(), UserId = drSara.Id, DepartmentId = isDept.Id, AcademicTitle = AcademicTitle.AssistantProfessor, CreatedAt = DateTime.Now.AddDays(-350) },
                    new Instructor { Id = Guid.NewGuid(), UserId = headOfDept.Id, DepartmentId = csDept.Id, AcademicTitle = AcademicTitle.Professor, CreatedAt = DateTime.Now.AddDays(-500) }
                };

                await context.Instructors.AddRangeAsync(instructors);
                await context.SaveChangesAsync();
            }
        }

        private async Task UpdateDepartmentHeadsAsync(HupDbContext context)
        {
            var csDept = await context.Departments.FirstAsync(d => d.DepartmentCode == "CS");
            var headInstructor = await context.Instructors.Include(i => i.User).FirstAsync(i => i.User.Email == "head.cs@university.edu");
            csDept.HeadOfDepartmentId = headInstructor.Id;
            await context.SaveChangesAsync();
        }


        private async Task SeedCoursesAsync(HupDbContext context)
        {
            if (!await context.Courses.AnyAsync())
            {
                var courses = new List<Course>();
                // We need 14 history subjects (6 Y1S1 + 6 Y1S2 + 2 Y1Sum) and 6 Y2S1 subjects and ~10 Y2S2 subjects. Total = 30 courses minimum.
                string[] englishNames = {
                    "Prog 1", "Math 1", "Physics 1", "English 1", "Human Rights", "Discrete Math", // Y1S1
                    "OOP", "Math 2", "Physics 2", "Electronics", "English 2", "Tech Writing", // Y1S2
                    "Logic Design", "Probability", // Y1Summer
                    "Data Structures", "Comp Arch", "Algorithms", "System Analysis", "Linear Algebra", "Operations Research", // Y2S1
                    "DB Systems", "Software Eng", "OS", "Networks", "AI", "Graphics", "Security", "Web Dev", "Machine Learning", "Cloud" // Y2S2
                };

                string[] arabicNames = {
                    "برمجة 1", "رياضيات 1", "فيزياء 1", "انجليزي 1", "حقوق انسان", "رياضيات متقطعة", // Y1S1
                    "برمجة كينونية", "رياضيات 2", "فيزياء 2", "الكترونيات", "انجليزي 2", "كتابة تقنية", // Y1S2
                    "تصميم منطقي", "احتمالات", // Y1Summer
                    "هياكل بيانات", "معمارية حاسب", "خوارزميات", "تحليل نظم", "جبر خطي", "بحوث عمليات", // Y2S1
                    "نظم قواعد بيانات", "هندسة برمجيات", "نظم تشغيل", "شبكات", "ذكاء اصطناعي", "رسوميات حاسب", "أمن معلومات", "تطوير ويب", "تعلم آلة", "حوسبة سحابية" // Y2S2
                };

                for (int i = 0; i < englishNames.Length; i++)
                {
                    courses.Add(new Course {
                        Id = Guid.NewGuid(),
                        CourseCode = $"CS{100 + i}",
                        CourseName = $"{{\"en\":\"{englishNames[i]}\",\"ar\":\"{arabicNames[i]}\"}}",
                        Credits = 3,
                        CreatedAt = DateTime.Now.AddDays(-300)
                    });
                }

                await context.Courses.AddRangeAsync(courses);
                await context.SaveChangesAsync();
            }
        }

        private async Task SeedProgramPlansAsync(HupDbContext context)
        {
            if (!await context.ProgramPlan.AnyAsync())
            {
                var generalDept = await context.Departments.FirstAsync(d => d.DepartmentCode == "GEN");
                var courses = await context.Courses.ToListAsync();

                var plans = courses.Select(c => new ProgramPlan { DepartmentId = generalDept.Id, CourseId = c.Id, RequirementType = RequirementType.Department, IsCompulsory = true, FinalGrade = 100 });
                await context.ProgramPlan.AddRangeAsync(plans);
                await context.SaveChangesAsync();
            }
        }

        private async Task SeedSemestersAsync(HupDbContext context)
        {
            if (!await context.Semesters.AnyAsync())
            {
                var now = DateTime.UtcNow;
                var semesters = new List<Semester>
                {
                    new Semester { Id = Guid.NewGuid(), SemesterName = "{\"en\":\"Y1-Fall\",\"ar\":\"سنة 1 - خريف\"}", StartDate = now.AddMonths(-18), EndDate = now.AddMonths(-15), RegistrationDeadline = now.AddMonths(-18).AddDays(14), DropDeadline = now.AddMonths(-18).AddDays(30), IsActive = false, CreatedAt = now },
                    new Semester { Id = Guid.NewGuid(), SemesterName = "{\"en\":\"Y1-Spring\",\"ar\":\"سنة 1 - ربيع\"}", StartDate = now.AddMonths(-12), EndDate = now.AddMonths(-9), RegistrationDeadline = now.AddMonths(-12).AddDays(14), DropDeadline = now.AddMonths(-12).AddDays(30), IsActive = false, CreatedAt = now },
                    new Semester { Id = Guid.NewGuid(), SemesterName = "{\"en\":\"Y1-Summer\",\"ar\":\"سنة 1 - صيف\"}", StartDate = now.AddMonths(-8), EndDate = now.AddMonths(-6), RegistrationDeadline = now.AddMonths(-8).AddDays(14), DropDeadline = now.AddMonths(-8).AddDays(30), IsActive = false, CreatedAt = now },
                    new Semester { Id = Guid.NewGuid(), SemesterName = "{\"en\":\"Y2-Fall\",\"ar\":\"سنة 2 - خريف\"}", StartDate = now.AddMonths(-6), EndDate = now.AddMonths(-3), RegistrationDeadline = now.AddMonths(-6).AddDays(14), DropDeadline = now.AddMonths(-6).AddDays(30), IsActive = false, CreatedAt = now },
                    // Active semester! Ensure StartDate is well in the past so GPA check passes!
                    new Semester { Id = Guid.NewGuid(), SemesterName = "{\"en\":\"Y2-Spring (Current)\",\"ar\":\"سنة 2 - ربيع (الحالي)\"}", StartDate = now.AddDays(-2), EndDate = now.AddMonths(3), RegistrationDeadline = now.AddDays(14), DropDeadline = now.AddDays(30), IsActive = true, CreatedAt = now }
                };
                await context.Semesters.AddRangeAsync(semesters);
                await context.SaveChangesAsync();
            }
        }

        private async Task SeedStudentsAsync(HupDbContext context)
        {
            if (!await context.Students.AnyAsync())
            {
                var generalDept = await context.Departments.FirstAsync(d => d.DepartmentCode == "GEN");
                var student1 = await context.Users.FirstAsync(u => u.Email == "student1@university.edu");
                var student2 = await context.Users.FirstAsync(u => u.Email == "student2@university.edu");

                var students = new List<Student>
                {
                    // 2nd year student (Level 2), unassigned department (General)
                    new Student { UserId = student1.Id, UniversityCode = "20230001", UniversityEmail = "student1@university.edu", ProfileImage = "/images/student1.jpg", AcademicStatus = AcademicStatus.Active, DepartmentId = generalDept.Id, Level = 2, Cgpa = 3.9m, Group = "A" },
                    new Student { UserId = student2.Id, UniversityCode = "20230002", UniversityEmail = "student2@university.edu", ProfileImage = null, AcademicStatus = AcademicStatus.Active, DepartmentId = generalDept.Id, Level = 2, Cgpa = 3.7m, Group = "B" }
                };

                await context.Students.AddRangeAsync(students);
                await context.SaveChangesAsync();
            }
        }

        private async Task SeedCourseOfferingsAsync(HupDbContext context)
        {
            if (!await context.CourseOfferings.AnyAsync())
            {
                var generalDept = await context.Departments.FirstAsync(d => d.DepartmentCode == "GEN");
                var sems = await context.Semesters.OrderBy(s => s.StartDate).ToListAsync();
                var courses = await context.Courses.OrderBy(c => c.CourseCode).ToListAsync();

                var offerings = new List<CourseOffering>();

                // Map courses to semesters: 6 to Y1S1, 6 to Y1S2, 2 to Y1Sum, 6 to Y2S1, 10 to Y2S2
                int[] counts = { 6, 6, 2, 6, 10 };
                int cIdx = 0;

                for (int s = 0; s < 5; s++)
                {
                    var semId = sems[s].Id;
                    for (int i = 0; i < counts[s]; i++)
                    {
                        offerings.Add(new CourseOffering { Id = Guid.NewGuid(), CourseId = courses[cIdx++].Id, SemesterId = semId, DepartmentId = generalDept.Id });
                    }
                }

                await context.CourseOfferings.AddRangeAsync(offerings);
                await context.SaveChangesAsync();
            }
        }

        private async Task SeedSchedulesAsync(HupDbContext context)
        {
            if (!await context.Schedules.AnyAsync())
            {
                var activeSem = await context.Semesters.FirstAsync(s => s.IsActive);
                var activeOfferings = await context.CourseOfferings.Where(co => co.SemesterId == activeSem.Id).ToListAsync();
                var profAhmed = await context.Instructors.Include(i => i.User).FirstAsync(i => i.User.Email == "prof.ahmed@university.edu");

                var schedules = new List<Schedule>();

                var overlappingTimeSlots = new (DayOfWeek Day, TimeSpan Start, TimeSpan End)[]
                {
                    (DayOfWeek.Sunday, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0)),
                    (DayOfWeek.Sunday, new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0)), // Overlaps 8-10 & 10-12
                    (DayOfWeek.Sunday, new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0)),
                    (DayOfWeek.Monday, new TimeSpan(8, 0, 0), new TimeSpan(11, 0, 0)), // 3 hours
                    (DayOfWeek.Monday, new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0)),// Overlaps 8-11
                };

                int slotIdx = 0;
                foreach (var offering in activeOfferings)
                {
                    // 3 overlapping timeslots per course
                    for (int g = 1; g <= 3; g++)
                    {
                        var slot = overlappingTimeSlots[slotIdx % overlappingTimeSlots.Length];
                        schedules.Add(new Schedule
                        {
                            Id = Guid.NewGuid(), CourseOfferingId = offering.Id, InstructorId = profAhmed.Id,
                            Group = $"G{g}", DayOfWeek = (System.DayOfWeek)(int)slot.Day, StartTime = slot.Start, EndTime = slot.End,
                            Hall = $"{{\"en\":\"Hall {g}\",\"ar\":\"قاعة {g}\"}}", InstructorName = profAhmed.User.FullName, TotalSeats = 50, AvailableSeats = 50
                        });
                        slotIdx++;
                    }
                }

                await context.Schedules.AddRangeAsync(schedules);
                await context.SaveChangesAsync();
            }
        }

        private async Task SeedExamsAsync(HupDbContext context)
        {
            if (!await context.Exams.AnyAsync())
            {
                var activeSem = await context.Semesters.FirstAsync(s => s.IsActive);
                var activeOfferings = await context.CourseOfferings.Where(co => co.SemesterId == activeSem.Id).ToListAsync();
                var exams = new List<Exam>();

                foreach (var offering in activeOfferings.Take(3)) // Just seed a few
                {
                    exams.Add(new Exam { Id = Guid.NewGuid(), CourseOfferingId = offering.Id, ExamType = ExamType.Midterm, ExamDate = new DateOnly(2025, 4, 15), ExamTime = new TimeOnly(9, 0), Location = "{\"en\":\"Exam Hall 1\",\"ar\":\"قاعة امتحانات 1\"}" });
                }

                await context.Exams.AddRangeAsync(exams);
                await context.SaveChangesAsync();
            }
        }

        private async Task SeedEnrollmentsAsync(HupDbContext context)
        {
            if (!await context.Enrollments.AnyAsync())
            {
                var student1 = await context.Students.FirstAsync(s => s.UniversityEmail == "student1@university.edu");
                var student2 = await context.Students.FirstAsync(s => s.UniversityEmail == "student2@university.edu");

                var pastSemesters = await context.Semesters.Where(s => !s.IsActive).ToListAsync();
                var pastSemIds = pastSemesters.Select(s => s.Id).ToList();

                var pastOfferings = await context.CourseOfferings.Where(co => pastSemIds.Contains(co.SemesterId)).ToListAsync();
                var enrollments = new List<Enrollment>();

                var random = new Random();

                foreach (var offering in pastOfferings)
                {
                    // History enrollments for both students
                    enrollments.Add(new Enrollment
                    {
                        Id = Guid.NewGuid(), StudentId = student1.UserId, CourseOfferingId = offering.Id, EnrollmentDate = DateTime.Now.AddDays(-100),
                        ClassGrade = random.Next(15, 20), MidtermGrade = random.Next(15, 20), finalGrade = random.Next(40, 60), Status = EnrollmentStatus.Completed
                    });
                    enrollments.Add(new Enrollment
                    {
                        Id = Guid.NewGuid(), StudentId = student2.UserId, CourseOfferingId = offering.Id, EnrollmentDate = DateTime.Now.AddDays(-100),
                        ClassGrade = random.Next(10, 18), MidtermGrade = random.Next(10, 18), finalGrade = random.Next(30, 50), Status = EnrollmentStatus.Completed
                    });
                }

                await context.Enrollments.AddRangeAsync(enrollments);
                await context.SaveChangesAsync();
            }
        }
    }
}