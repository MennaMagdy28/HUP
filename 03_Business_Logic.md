# Business Logic

This document breaks down the step-by-step logic for key features, from request to DB, including branching and side effects.

## 1. Enrollment Registration (`POST /api/Enrollment`)
**Step-by-step flow:**
1. Request hits `EnrollmentController.AddEnrollment` with `List<CreateEnrollmentDto>`.
2. Calls `EnrollmentService.AddAsync(dtos)`.
3. Opens a DB transaction via `ITransactionService.ExecuteInTransactionAsync`.
4. **Validation Phase (`EnrollmentValidator.ValidateEnrollmentAsync`)**:
   - Checks if batch has duplicate `CourseOfferingId`s.
   - Determines GPA enrollment window (`CanStudentEnroll`):
     - GPA >= 3.8: Can enroll exactly on/after semester start time.
     - GPA >= 3.5: Can enroll 2 hours after semester start time.
     - GPA < 3.5: Can enroll 4 hours after semester start time.
   - For each requested enrollment:
     - Checks if student already enrolled in DB.
     - Validates `CourseOffering` and `Schedule` existence.
     - Validates `Prerequisite`: If a prerequisite exists, student must have an `Enrollment` with `Status == Completed` and `FinalGrade >= 50`.
     - Validates capacity: `Schedule.AvailableSeats > 0`.
     - Checks time conflicts against existing DB enrollments.
   - Validates time conflicts within the requested batch itself.
5. **Execution Phase**:
   - Iterates through the batch.
   - For each item, atomically attempts to decrement `AvailableSeats` on the `Schedule` using raw SQL (`UPDATE Schedules SET AvailableSeats = AvailableSeats - 1 ... WHERE Id = ... AND AvailableSeats > 0`).
   - If SQL returns 0 rows affected, throws an exception (rollback).
   - If successful, creates an `Enrollment` entity with `Status = Registered`.
   - Adds to repository.
6. Commits transaction (`SaveChangesAsync`).
**Side Effects**:
- `Schedules` table updated (AvailableSeats decreased).
- New `Enrollments` inserted.

## 2. Course Dropping (`DELETE /api/Enrollment/{id}`)
**Step-by-step flow:**
1. Request hits `EnrollmentController.RemoveEnrollment`.
2. Calls `EnrollmentService.Remove(id)`.
3. Fetches the `Enrollment` tracking entity.
4. **Validation Phase (`EnrollmentValidator.ValidateDropAsync`)**:
   - Checks ownership (requesting student must match enrollment's student).
   - Checks against Active Semester's `DropDeadline`.
   - **Minimum Credits Check**: Sums up credits of currently enrolled courses for the semester. Subtracts the dropped course's credits. If the result is `< 12`, throws an exception.
5. **Execution Phase**:
   - (Current Logic Note): It currently calls `_repository.RemoveAsync(id)` which does a soft delete (because `Enrollment` inherits from `BaseEntity`).
   - **Side Effect Note**: Wait, the seat is NOT replenished in `Schedule`! The logic for `Drop` does not increment `AvailableSeats`. This is a missing feature / bug.

## 3. Profile Photo Upload (`POST /api/Students/{id}/photo`)
**Step-by-step flow:**
1. Request hits `StudentsController.UploadPhoto` with `IFormFile`.
2. Calls `StudentService.UploadProfilePhotoAsync`.
3. **Validation**:
   - Empty file check.
   - Size limit: `5 * 1024 * 1024` (5MB).
   - Allowed extensions: `.jpg`, `.jpeg`, `.png`.
4. **Execution**:
   - Constructs path: `wwwroot/uploads/students/{studentId}{extension}`.
   - Retrieves `Student` entity.
   - Deletes old file if `Student.ProfileImage` is set.
   - Saves new file to disk using `FileStream`.
   - Updates `Student.ProfileImage` with relative path `/uploads/students/...`.
5. DB save.
**Side Effects**:
- File written to disk.
- DB record updated.

## 4. Financial Summary (`GET /api/Financial/student/{studentId}/summary`)
**Step-by-step flow:**
1. Calls `FinancialService.GetStudentFinancialSummaryAsync`.
2. Fetches all `StudentFee` records for the student.
3. Maps to DTOs.
4. Sums `Amount` for Total Billed.
5. Sums `PaidAmount` for Total Paid.
6. Calculates Outstanding as `TotalBilled - TotalPaid`.
**Side Effects**: Read-only.

## 5. Course Offering Availability (`GET /api/CourseOffering/available/{studentId}`)
**Step-by-step flow:**
1. Calls `CourseOfferingService.GetAvailableToRegisterAsync`.
2. Repository `GetAvailableToRegisterAsync`:
   - Filters offerings in active semester.
   - **Condition 1**: Excludes courses student has already Completed, Registered, or InProgress.
   - **Condition 2**: Prerequisite Check - If course has prerequisite, student must have a Completed enrollment for it.
   - Includes Schedules.
**Side Effects**: Read-only.
