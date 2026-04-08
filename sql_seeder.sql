USE [HUP];
GO

-- =====================================================================
-- STEP 1: DISABLE CONSTRAINTS & CLEAR DATA
-- =====================================================================
PRINT 'Disabling constraints...';
EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all";
GO

PRINT 'Clearing tables...';
DELETE FROM [dbo].[Enrollments];
DELETE FROM [dbo].[Exams];
DELETE FROM [dbo].[Schedules];
DELETE FROM [dbo].[CourseOfferings];
DELETE FROM [dbo].[Semesters];
DELETE FROM [dbo].[Staff];
DELETE FROM [dbo].[Students];
DELETE FROM [dbo].[ProgramPlan];
DELETE FROM [dbo].[Courses];
DELETE FROM [dbo].[Departments];
DELETE FROM [dbo].[Faculties];
DELETE FROM [dbo].[RolePermissions];
DELETE FROM [dbo].[Permissions];
DELETE FROM [dbo].[Users];
DELETE FROM [dbo].[Roles];
GO

-- =====================================================================
-- STEP 2: DECLARE VARIABLES (Using NEWID() for valid GUIDs)
-- =====================================================================
PRINT 'Initializing Variables...';

-- Roles
DECLARE @RoleId_Admin UNIQUEIDENTIFIER = NEWID();
DECLARE @RoleId_Student UNIQUEIDENTIFIER = NEWID();
DECLARE @RoleId_Instructor UNIQUEIDENTIFIER = NEWID();
DECLARE @RoleId_HOD UNIQUEIDENTIFIER = NEWID();

-- Users
DECLARE @UserId_Admin UNIQUEIDENTIFIER = NEWID();
DECLARE @UserId_Instr1 UNIQUEIDENTIFIER = NEWID();
DECLARE @UserId_Instr2 UNIQUEIDENTIFIER = NEWID();
DECLARE @UserId_HOD UNIQUEIDENTIFIER = NEWID();
DECLARE @UserId_Student1 UNIQUEIDENTIFIER = NEWID();
DECLARE @UserId_Student2 UNIQUEIDENTIFIER = NEWID();

-- Faculties
DECLARE @FacId_Eng UNIQUEIDENTIFIER = NEWID();
DECLARE @FacId_CS UNIQUEIDENTIFIER = NEWID();

-- Departments
DECLARE @DeptId_CS UNIQUEIDENTIFIER = NEWID();
DECLARE @DeptId_SE UNIQUEIDENTIFIER = NEWID();
DECLARE @DeptId_EE UNIQUEIDENTIFIER = NEWID();
DECLARE @DeptId_ME UNIQUEIDENTIFIER = NEWID();

-- Courses
DECLARE @CourseId_CS101 UNIQUEIDENTIFIER = NEWID();
DECLARE @CourseId_CS102 UNIQUEIDENTIFIER = NEWID();
DECLARE @CourseId_CS201 UNIQUEIDENTIFIER = NEWID();
DECLARE @CourseId_SE101 UNIQUEIDENTIFIER = NEWID();
DECLARE @CourseId_EE101 UNIQUEIDENTIFIER = NEWID();
DECLARE @CourseId_ME101 UNIQUEIDENTIFIER = NEWID();

-- Semesters
DECLARE @SemId_Spring25 UNIQUEIDENTIFIER = NEWID();

-- Course Offerings
DECLARE @OfferId_CS101 UNIQUEIDENTIFIER = NEWID();
DECLARE @OfferId_CS102 UNIQUEIDENTIFIER = NEWID();
DECLARE @OfferId_SE101 UNIQUEIDENTIFIER = NEWID();

-- Schedules
DECLARE @SchedId_CS101 UNIQUEIDENTIFIER = NEWID();
DECLARE @SchedId_CS102 UNIQUEIDENTIFIER = NEWID();
DECLARE @SchedId_SE101 UNIQUEIDENTIFIER = NEWID();
DECLARE @SchedId_CS101_1 UNIQUEIDENTIFIER = NEWID();
DECLARE @SchedId_CS102_1 UNIQUEIDENTIFIER = NEWID();
DECLARE @SchedId_SE101_1 UNIQUEIDENTIFIER = NEWID();

-- Permissions
DECLARE @PermId_UserRead UNIQUEIDENTIFIER = NEWID();

-- =====================================================================
-- STEP 3: INSERT DATA (Matching your Migration Columns Exactly)
-- =====================================================================

PRINT 'Seeding Roles...';
-- CreatedBy matches FK_Roles_Users_CreatedBy
INSERT INTO [dbo].[Roles] (Id, Name, DisplayName, Description, CreatedBy, CreatedAt, IsDeleted)
VALUES
    (@RoleId_Admin, 'Admin', 'Administrator', 'System administrator', @UserId_Admin, GETDATE(), 0),
    (@RoleId_Student, 'Student', 'Student', 'Student role', @UserId_Admin, GETDATE(), 0),
    (@RoleId_Instructor, 'Instructor', 'Instructor', 'Teaching staff', @UserId_Admin, GETDATE(), 0),
    (@RoleId_HOD, 'HOD', 'Head of Department', 'Department head', @UserId_Admin, GETDATE(), 0);

PRINT 'Seeding Permissions...';
INSERT INTO [dbo].[Permissions] (Id, Name, DisplayName, Description, CreatedAt, IsDeleted)
VALUES
    (@PermId_UserRead, 'perm_users_read','Users:Read','Read user info',GETDATE(),0),
    (NEWID(), 'perm_users_update','Users:Update','Update user info',GETDATE(),0),
    (NEWID(), 'perm_roles_manage','Roles:Manage','Manage roles',GETDATE(),0),
    (NEWID(), 'perm_courses_manage','Courses:Manage','Manage courses',GETDATE(),0),
    (NEWID(), 'perm_enrollments_manage','Enrollments:Manage','Manage enrollments',GETDATE(),0);

PRINT 'Seeding RolePermissions...';
INSERT INTO [dbo].[RolePermissions] (RoleId, PermissionId)
VALUES
    (@RoleId_Admin, @PermId_UserRead);

PRINT 'Seeding Users...';
-- RoleId matches FK_Users_Roles_RoleId
INSERT INTO [dbo].[Users] (Id, NationalId, Email, PasswordHash, FullName, ContactInfo_PhoneNumber, IsActive, RoleId, CreatedAt, IsDeleted, ContactInfo_City, PersonalInfo_BirthDate, PersonalInfo_BirthPlace, PersonalInfo_Gender, PersonalInfo_Nationality, PersonalInfo_Religion, PasswordExpiryDate)
VALUES
    -- Admin
    (@UserId_Admin, '00000000000000', 'admin@uni.example', 'AQAAAAIAAYagAAAAEO7eDg3Gr2Im3Kys9LIXP94AlypVD9bgJFCDB/P9xBBM5i3YzmOBfkMYNse6f5Ya0A==', N'{"en":"System Adminstrator","ar":"مسؤول النظام"}', '+201000', 1, @RoleId_Admin, GETDATE(), 0, 0, '1980-01-01', 0, 0, 0, 0, GETDATE()),
    -- Instructors
    (@UserId_Instr1, '10010010010010', 'ahmed@uni.example', 'AQAAAAIAAYagAAAAEO7eDg3Gr2Im3Kys9LIXP94AlypVD9bgJFCDB/P9xBBM5i3YzmOBfkMYNse6f5Ya0A==', N'{"en":"Ahmed Saeed","ar":"احمد سعيد"}', '+201111', 1, @RoleId_Instructor, GETDATE(), 0, 0, '1985-03-15', 2, 0, 0, 0, GETDATE()),
    (@UserId_Instr2, '10020020020020', 'mona@uni.example', 'AQAAAAIAAYagAAAAEO7eDg3Gr2Im3Kys9LIXP94AlypVD9bgJFCDB/P9xBBM5i3YzmOBfkMYNse6f5Ya0A==', N'{"en":"Mona elhadad","ar":"مني الحداد"}', '+201222', 1, @RoleId_Instructor, GETDATE(), 0, 0, '1978-11-20', 1, 1, 0, 1, GETDATE()),
    -- HOD
    (@UserId_HOD, '20020020020020', 'hod@uni.example', 'AQAAAAIAAYagAAAAEO7eDg3Gr2Im3Kys9LIXP94AlypVD9bgJFCDB/P9xBBM5i3YzmOBfkMYNse6f5Ya0A==', N'{"en":"Dr.Hoda","ar":"د.هدي"}', '+201333', 1, @RoleId_HOD, GETDATE(), 0, 0, '1975-05-05', 0, 1, 0, 0, GETDATE()),
    -- Students
    (@UserId_Student1, '30030030030030', 'sara@uni.example', 'AQAAAAIAAYagAAAAEO7eDg3Gr2Im3Kys9LIXP94AlypVD9bgJFCDB/P9xBBM5i3YzmOBfkMYNse6f5Ya0A==', N'{"en":"Sara Mohamed","ar":"سارة محمد"}', '+201444', 1, @RoleId_Student, GETDATE(), 0, 0, '2002-08-12', 0, 1, 0, 0, GETDATE()),
    (@UserId_Student2, '30040040040040', 'omar@uni.example', 'AQAAAAIAAYagAAAAEO7eDg3Gr2Im3Kys9LIXP94AlypVD9bgJFCDB/P9xBBM5i3YzmOBfkMYNse6f5Ya0A==', N'{"en":"Omar Hassan","ar":"عمر حسن"}', '+201555', 1, @RoleId_Student, GETDATE(), 0, 1, '2001-02-25', 1, 0, 0, 0, GETDATE());

PRINT 'Seeding Faculties...';
INSERT INTO [dbo].[Faculties] (Id, DisplayName, DeanId, DeanName, ContactInfo, CreatedAt, IsDeleted, Name)
VALUES
    (@FacId_Eng, N'{"en":"Faculty of Engineering","ar":"كلية الهندسة"}', @UserId_Instr1, 'Prof. Eng. Dean', 'eng@uni', GETDATE(), 0, 13),
    (@FacId_CS, N'{"en":"Faculty of Computers and Artificial Engineering","ar":"كلية الحاسبات والذكاء الاصطناعي"}', @UserId_Instr2, 'Prof. CS Dean', 'cs@uni', GETDATE(), 0, 4);

PRINT 'Seeding Departments...';
-- NOTE: HeadOfDepartmentId is a GUID pointing to INSTRUCTORS, not Users
-- We use @UserId_HOD as a placeholder for now, but strictly it links to the Instructor Table ID which we create next.
-- Because constraints are disabled, we can insert this now, but logically it should be @InstrId_3.
INSERT INTO [dbo].[Departments] (Id, DepartmentName, DepartmentCode, FacultyId, CompulsoryHours, CreatedAt, DurationInYears, ElectiveHours, IsDeleted, HeadOfDepartmentId)
VALUES
    (@DeptId_CS, N'{"en":"Computer Science","ar":"علوم الحاسب"}', 'CS', @FacId_CS, 120, GETDATE(), 4, 30, 0, @UserId_Instr1), -- Points to HOD Instructor ID
    (@DeptId_SE, N'{"en":"Software Engineering","ar":"الهندسة البرمجيات"}', 'SE', @FacId_CS, 120, GETDATE(), 4, 30, 0, NULL),
    (@DeptId_EE, N'{"en":"Electrical Engineering","ar":"الهندسة الكهربائية"}', 'EE', @FacId_Eng, 130, GETDATE(), 4, 25, 0, NULL),
    (@DeptId_ME, N'{"en":"Mechanical Engineering","ar":"الهندسة الميكانيكية"}', 'ME', @FacId_Eng, 130, GETDATE(), 4, 25, 0, NULL);

PRINT 'Seeding Courses...';
INSERT INTO [dbo].[Courses] (Id, CourseCode, CourseName, Credits, PrerequisiteId, CreatedAt, IsDeleted)
VALUES
    (@CourseId_CS101, N'{"en":"CS101","ar":"CS101"}', N'{"en":"Intro to Programming","ar":"مقدمة في البرمجة"}', 3, NULL, GETDATE(), 0),
    (@CourseId_CS102, N'{"en":"CS102","ar":"CS102"}', N'{"en":"Data Structures","ar":"هياكل البيانات"}',       3, @CourseId_CS101, GETDATE(), 0),
    (@CourseId_CS201, N'{"en":"CS201","ar":"CS201"}', N'{"en":"Algorithms","ar":"الخوارزميات"}',              3, @CourseId_CS102, GETDATE(), 0),
    (@CourseId_SE101, N'{"en":"SE101","ar":"SE101"}', N'{"en":"Software Design","ar":"تصميم البرمجيات"}',     3, NULL, GETDATE(), 0),
    (@CourseId_EE101, N'{"en":"EE101","ar":"EE101"}', N'{"en":"Circuits I","ar":"الدوائر 1"}',                3, NULL, GETDATE(), 0),
    (@CourseId_ME101, N'{"en":"ME101","ar":"ME101"}', N'{"en":"Statics","ar":"الاستاتيكا"}',                 3, NULL, GETDATE(), 0);

PRINT 'Seeding ProgramPlan...';
-- NOTE: Only FinalGrade exists in your migration
INSERT INTO [dbo].[ProgramPlan] (DepartmentId, CourseId, RequirementType, IsCompulsory, FinalGrade)
VALUES
    (@DeptId_CS, @CourseId_CS101, 0, 1, 100),
    (@DeptId_CS, @CourseId_CS102, 0, 1, 100),
    (@DeptId_SE, @CourseId_SE101, 0, 1, 100);

PRINT 'Seeding Staff...';
INSERT INTO [dbo].Staff (UserId, DepartmentId, FacultyId, Title, Category)
VALUES
    (@UserId_Instr1, @DeptId_SE, @FacId_CS, 0, 1),
    (@UserId_Instr2, @DeptId_CS, @FacId_CS, 1, 1),
    (@UserId_HOD,    @DeptId_CS, @FacId_Eng, 5, 2);
PRINT 'Seeding Students...';
INSERT INTO [dbo].[Students] (UserId, UniversityCode, UniversityEmail, AcademicStatus, DepartmentId, Level, Cgpa, [Group])
VALUES
    (@UserId_Student1, '20250001', 'sara@uni.example', 0, @DeptId_CS, 2, 3.45, 'G1'),
    (@UserId_Student2, '20250002', 'omar@uni.example', 0, @DeptId_SE, 3, 3.10, 'G2');

PRINT 'Seeding Semesters...';
INSERT INTO [dbo].[Semesters] (Id, SemesterName, StartDate, EndDate, IsActive, DropDeadline, CreatedAt, IsDeleted, RegistrationDeadline)
VALUES
    (@SemId_Spring25, '2025 Spring', '2025-02-01', '2025-05-31', 1, '2026-01-01', GETDATE(), 0, '2025-01-25');

PRINT 'Seeding CourseOfferings...';
INSERT INTO [dbo].[CourseOfferings] (Id, CourseId, SemesterId, CreatedAt, IsDeleted, DepartmentId)
VALUES
    (@OfferId_CS101, @CourseId_CS101, @SemId_Spring25, GETDATE(), 0, @DeptId_CS),
    (@OfferId_CS102, @CourseId_CS102, @SemId_Spring25, GETDATE(), 0, @DeptId_CS),
    (@OfferId_SE101, @CourseId_SE101, @SemId_Spring25, GETDATE(), 0, @DeptId_SE);

PRINT 'Seeding Schedules...';
INSERT INTO [dbo].[Schedules] (Id ,CourseOfferingId ,StaffId ,[Group] ,DayOfWeek ,StartTime ,EndTime
								,Hall, StaffName,TotalSeats ,AvailableSeats ,CreatedAt ,UpdatedAt ,IsDeleted)
VALUES
    (@SchedId_CS101, @OfferId_CS101, @UserId_Instr1, 'G1', 0,'09:00:00', '10:30:00', N'{"en":"Hall1","ar":"قاعة 1"}', N'{"en":"Dr.soha","ar":"د.سهي"}', 100, 100, GETDATE(), GETDATE(), 0),
    (@SchedId_CS102, @OfferId_CS102, @UserId_Instr1, 'G1', 1,'11:00:00', '12:30:00', N'{"en":"Hall2","ar":"قاعة 2"}', N'{"en":"Dr.ahmed","ar":"د.احمد"}', 100, 100,  GETDATE(), GETDATE(), 0),
    (@SchedId_SE101, @OfferId_SE101, @UserId_Instr1, 'G2', 2,'13:00:00', '14:30:00', N'{"en":"Hall3","ar":"قاعة 3"}', N'{"en":"Dr.mamdouh","ar":"د.ممدوح"}', 100, 100, GETDATE(), GETDATE(), 0),
    (@SchedId_CS101_1, @OfferId_CS101, @UserId_Instr1, 'G2', 0,'10:30:00', '12:00:00', N'{"en":"Hall1","ar":"قاعة 1"}', N'{"en":"Dr.soha","ar":"د.سهي"}', 100, 100, GETDATE(), GETDATE(), 0),
    (@SchedId_CS102_1, @OfferId_CS102, @UserId_Instr1, 'G2', 1,'12:30:00', '14:30:00', N'{"en":"Hall2","ar":"قاعة 2"}', N'{"en":"Dr.ahmed","ar":"د.احمد"}', 100, 100,  GETDATE(), GETDATE(), 0),
    (@SchedId_SE101_1, @OfferId_SE101, @UserId_Instr1, 'G1', 2,'13:00:00', '14:00:00', N'{"en":"Hall3","ar":"قاعة 3"}', N'{"en":"Dr.mamdouh","ar":"د.ممدوح"}', 100, 100, GETDATE(), GETDATE(), 0);

PRINT 'Seeding Exams...';
INSERT INTO [dbo].[Exams] (Id, CourseOfferingId, ExamType, ExamDate, ExamTime, Location, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @OfferId_CS101, 0, '2025-05-20', '09:00:00', 'Hall A', GETDATE(), 0),
    (NEWID(), @OfferId_CS102, 0, '2025-05-22', '11:00:00', 'Hall B', GETDATE(), 0),
    (NEWID(), @OfferId_SE101, 0, '2025-05-24', '13:00:00', 'Lab 1',  GETDATE(), 0);

PRINT 'Seeding Enrollments...';
-- NOTE: Correct columns ClassGrade, MidtermGrade, finalGrade
INSERT INTO [dbo].[Enrollments] (Id, StudentId, CourseOfferingId, EnrollmentDate, Status, CreatedAt, IsDeleted, ClassGrade, MidtermGrade, finalGrade, ScheduleId)
VALUES
    (NEWID(), @UserId_Student1, @OfferId_CS101, GETDATE(), 0, GETDATE(), 0, 0, 0, 0, @SchedId_CS101),
    (NEWID(), @UserId_Student2, @OfferId_CS102, GETDATE(), 0, GETDATE(), 0, 0, 0, 0, @SchedId_CS102_1);

PRINT 'Re-enabling constraints...';
EXEC sp_MSforeachtable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all";
GO

PRINT 'Seed complete.';
GO
