# API Map

This document details all API endpoints exposed by the controllers, including their expected inputs, outputs, and behaviors.

## AuthController
- **`POST /api/Auth/login`**
  - **Method:** `Login`
  - **Request:** `LoginDto` (NationalId, Password)
  - **Response:** `AuthResponseDto` (200 OK) | `Unauthorized` (401)
  - **Validation:** Global `FluentValidation` (implicitly via `ValidationFilter`). Validates credentials against DB.

- **`POST /api/Auth/update-password`**
  - **Method:** `UpdatePassword`
  - **Request:** `UpdatePassword` DTO (CurrentPassword, NewPassword, UserId)
  - **Response:** `Ok("Password updated successfully")` (200) | `BadRequest` (400)

## CourseOfferingController
- **`GET /api/CourseOffering/{id}`**
  - **Method:** `GetById`
  - **Response:** `CourseOfferingDto` (200 OK) | `NotFound` (404)

- **`GET /api/CourseOffering`**
  - **Method:** `GetAll`
  - **Response:** `IEnumerable<CourseOfferingDto>` (200 OK)

- **`GET /api/CourseOffering/active/{departmentId}/{semesterId}`**
  - **Method:** `GetActiveCourseOffering`
  - **Response:** `IEnumerable<CourseOfferingDto>` (200 OK)

- **`GET /api/CourseOffering/available/{studentId}`**
  - **Method:** `GetAvailableToRegister`
  - **Response:** `IEnumerable<CourseOfferingDto>` (200 OK)

- **`POST /api/CourseOffering`**
  - **Method:** `Create`
  - **Request:** `CreateCourseOfferingDto`
  - **Response:** `Ok("Course Offering created successfully.")` (200) | `BadRequest("Course Offering already exists.")` (400)

- **`DELETE /api/CourseOffering/{id}`**
  - **Method:** `Delete`
  - **Response:** `Ok("Course Offering deleted successfully.")` (200)

## DepartmentsController
- **`GET /api/Departments`**
  - **Method:** `GetAll`
  - **Response:** `IEnumerable<DepartmentDto>` (200 OK)

## EnrollmentController
- **`POST /api/Enrollment`**
  - **Method:** `AddEnrollment`
  - **Request:** `List<CreateEnrollmentDto>`
  - **Response:** `Ok("Enrollment successful")` (200) | `BadRequest` with message (on validation/business logic failure)

- **`GET /api/Enrollment`**
  - **Method:** `GetAllEnrollment`
  - **Response:** `IEnumerable<EnrollmentResponseDto>` (200 OK)

- **`GET /api/Enrollment/{id}`**
  - **Method:** `GetEnrollmentById`
  - **Response:** `EnrollmentResponseDto` (200 OK)

- **`GET /api/Enrollment/student/{studentId}/registered`**
  - **Method:** `GetRegisteredCourses`
  - **Request:** Query params `EnrollmentFilterDto`
  - **Response:** `IEnumerable<EnrollmentResponseDto>` (200 OK)

- **`GET /api/Enrollment/student/{studentId}/grades`**
  - **Method:** `GetStudentGrades`
  - **Response:** `List<SemesterTranscriptDto>` (200 OK)

- **`DELETE /api/Enrollment/{id}`**
  - **Method:** `RemoveEnrollment`
  - **Response:** `Ok("Enrollment Removed successful")` (200)

## ExamController
- **`GET /api/Exam`**
  - **Method:** `GetAll`
  - **Response:** `IEnumerable<Exam>` (200 OK)

## FacultiesController
- **`GET /api/Faculties`**
  - **Method:** `GetAll`
  - **Response:** `IEnumerable<FacultyDto>` (200 OK)

## FinancialController
- **`GET /api/Financial/student/{studentId}/summary`**
  - **Method:** `GetStudentFinancialSummary`
  - **Response:** `FinancialSummaryDto` (200 OK)

## ProgramPlanController
- **`GET /api/ProgramPlan/department/{departmentId}`**
  - **Method:** `GetByDepartment`
  - **Response:** `IEnumerable<ProgramPlan>` (200 OK)

## ScheduleController
- **`GET /api/Schedule/student/{studentId}`**
  - **Method:** `GetStudentSchedule`
  - **Response:** `IEnumerable<ScheduleSlotDto>` (200 OK)

- **`GET /api/Schedule/available/{studentId}`**
  - **Method:** `GetAvailableSchedule`
  - **Response:** `IEnumerable<ScheduleSlotDto>` (200 OK)

- **`POST /api/Schedule`**
  - **Method:** `CreateScheduleSlot`
  - **Request:** `ScheduleSlotCreateDto`
  - **Response:** `Ok("Schedule slot created successfully.")` (200)

- **`DELETE /api/Schedule/{id}`**
  - **Method:** `DeleteScheduleSlot`
  - **Response:** `Ok("Schedule slot deleted successfully.")` (200)

## StudentsController
- **`GET /api/Students/{userId}/profile`**
  - **Method:** `GetStudentProfile`
  - **Response:** `StudentProfileDto` (200 OK) | `NotFound` (404)

- **`POST /api/Students`**
  - **Method:** `AddStudent`
  - **Request:** `CreateStudentDto`
  - **Response:** `Ok("Student added successfully.")` (200)

- **`PUT /api/Students/status`**
  - **Method:** `UpdateStatus`
  - **Request:** `StudentStatusDto`
  - **Response:** `Ok("Status updated successfully.")` (200) | `BadRequest` (400)

- **`POST /api/Students/{id}/photo`**
  - **Method:** `UploadPhoto`
  - **Request:** `IFormFile file` (multipart/form-data)
  - **Response:** `Ok(new { Path = "..." })` (200) | `BadRequest` (400)

## UserController
- **`GET /api/User`**
  - **Method:** `GetAllUsers`
  - **Response:** `IEnumerable<UsersListResponse>` (200 OK)

- **`GET /api/User/{userId}/profile`**
  - **Method:** `GetUserProfile`
  - **Response:** `ProfileInfoDto` (200 OK)

- **`POST /api/User`**
  - **Method:** `AddUser`
  - **Request:** `CreateUserDto`
  - **Response:** `Ok("User created successfully.")` (200) | `BadRequest` (400)

- **`PUT /api/User/{userId}/missing-info`**
  - **Method:** `InsertMissingInfo`
  - **Request:** `UpdateInfoDto`
  - **Response:** `Ok("Info inserted successfully")` (200) | `BadRequest` (400)

- **`DELETE /api/User/{userId}`**
  - **Method:** `SoftDeleteUser`
  - **Response:** `Ok("Removed Successfully.")` (200) | `NotFound` (404)
