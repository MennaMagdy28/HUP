# Feature Catalog

This document lists all features implemented in the University Portal system, based on the backend codebase.

## 1. Authentication and Authorization
**Description:** Manages user login, token generation (JWT), password management, and permission retrieval.
**Entry Points:**
- `POST /api/Auth/login`
- `POST /api/Auth/update-password`
**Input DTOs:**
- `LoginDto` (NationalId, Password)
- `UpdatePassword` (CurrentPassword, NewPassword, UserId)
**Output DTOs:**
- `AuthResponseDto` (Token, UserDto, ProfileStatus)
**Business Logic:**
- Validates credentials using `IPasswordHasher<User>`.
- Generates JWT with roles and claims.
- Caches user role for 2 minutes via `CacheService`.
- Retrieves and caches user permissions via `PermissionService`.
- Determines `ProfileStatus` (missing fields, password expiry) and returns it with the token.
- Password updates verify the current password, hash the new one, and reset the expiry date (+60 days).
**Dependencies:**
- `AuthService`, `UserService`, `PermissionService`, `UserRepository`
- `CacheService`
- Identity (`IPasswordHasher`)

## 2. Enrollment Management
**Description:** Handles student course registration, enforcing business rules like prerequisites, capacity, GPA-based registration windows, and schedule conflicts.
**Entry Points:**
- `POST /api/Enrollment`
- `GET /api/Enrollment/student/{studentId}/registered`
- `GET /api/Enrollment/student/{studentId}/grades`
- `DELETE /api/Enrollment/{id}`
**Input DTOs:**
- `CreateEnrollmentDto` (StudentId, CourseOfferingId, ScheduleId)
- `EnrollmentFilterDto` (Status)
**Output DTOs:**
- `EnrollmentResponseDto`
- `SemesterTranscriptDto`
- `SemesterGradesDto`
**Business Logic:**
- **Registration**: Runs within a transaction. Validates batch for duplicates. Enforces GPA-based time windows (tier >=3.8, >=3.5, <3.5) against the active semester start date. Checks for prerequisites passed. Verifies atomic seat booking via `TryBookSeatAsync`. Checks for time conflicts between current enrollments and within the requested batch itself.
- **Grades**: Calculates cumulative and semester GPA based on fetched grade models. Grade mapping (e.g., A+ = 4.0, A = 3.75, etc.).
- **Drop**: Checks drop deadline against active semester. Validates student minimum credit load (e.g., 12 credits) before allowing a drop.
**Dependencies:**
- `EnrollmentService`, `EnrollmentValidator`
- `EnrollmentRepository`, `StudentRepository`, `CourseOfferingRepository`, `ScheduleRepository`, `SemesterRepository`, `ProgramPlanRepository`
- `TransactionService`

## 3. User & Student Management
**Description:** Manages user creation, profile fetching, updates, and student-specific operations (e.g., uploading profile photos).
**Entry Points:**
- `GET /api/User`
- `GET /api/User/{userId}/profile`
- `POST /api/User`
- `PUT /api/User/{userId}/missing-info`
- `GET /api/Students/{userId}/profile`
- `POST /api/Students`
- `PUT /api/Students/status`
- `POST /api/Students/{id}/photo`
**Input DTOs:**
- `CreateUserDto`, `UpdateInfoDto`
- `CreateStudentDto`, `StudentStatusDto`
**Output DTOs:**
- `UsersListResponse`, `ProfileInfoDto`, `StudentProfileDto`
**Business Logic:**
- Creates a `User` entity, hashes the password, and assigns an initial expiry date.
- Creates `Student` records tied to `User`.
- Validates missing information across `UserPersonalInfo` and `UserContact` using reflection.
- Handles profile photo uploads: validates size (<5MB) and type (.jpg, .jpeg, .png), stores locally in `wwwroot/uploads/students`, and deletes old photos to save space.
**Dependencies:**
- `UserService`, `StudentService`
- `UserRepository`, `StudentRepository`

## 4. Academic Structure (Departments & Faculties)
**Description:** Manages academic faculties and departments. Uses heavy caching for performance.
**Entry Points:**
- `GET /api/Departments`
- `GET /api/Faculties`
**Input DTOs:** None for fetch.
**Output DTOs:**
- `DepartmentDto`, `FacultyDto`
**Business Logic:**
- Fetches all faculties/departments and caches the results (`faculties:list`, `departments:list`) for 60 minutes.
- Invalidates cache automatically upon add or remove operations.
- DTO mapping utilizes localized names (via `LocalizationHelper`).
**Dependencies:**
- `DepartmentService`, `FacultyService`
- `DepartmentRepository`, `FacultyRepository`
- `CacheService`

## 5. Course Offerings & Scheduling
**Description:** Manages course offerings per semester and their associated schedule slots.
**Entry Points:**
- `GET /api/CourseOffering/available/{studentId}`
- `GET /api/CourseOffering/active/{departmentId}/{semesterId}`
- `GET /api/Schedule/student/{studentId}`
- `GET /api/Schedule/available/{studentId}`
**Input DTOs:**
- `CreateCourseOfferingDto`
- `ScheduleSlotCreateDto`
**Output DTOs:**
- `CourseOfferingDto`
- `ScheduleSlotDto`
**Business Logic:**
- **Course Offerings**: Filters available courses for a student by checking active semester, excluding already passed/in-progress courses, and ensuring prerequisites are met.
- **Scheduling**: Creates slots with specific staff and capacities. Available schedules are filtered to match the available course offerings for a student.
**Dependencies:**
- `CourseOfferingService`, `ScheduleService`
- `CourseOfferingRepository`, `ScheduleRepository`

## 6. Financial Management
**Description:** Provides financial summaries for students.
**Entry Points:**
- `GET /api/Financial/student/{studentId}/summary`
**Input DTOs:** None.
**Output DTOs:**
- `FinancialSummaryDto` (TotalBilled, TotalPaid, TotalOutstanding, Fees list)
- `StudentFeeDto`
**Business Logic:**
- Aggregates all `StudentFee` records for a student.
- Calculates `TotalBilled` (sum of Amount), `TotalPaid` (sum of PaidAmount), and `TotalOutstanding`.
**Dependencies:**
- `FinancialService`
- `FinancialRepository`

## 7. Exam Management
**Description:** Basic CRUD operations for exams.
**Entry Points:**
- `GET /api/Exam`
**Dependencies:**
- `ExamService`, `ExamRepository`
