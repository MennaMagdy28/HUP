# Business Rules and Constraints

This document lists all strictly enforced rules from the code.

## 1. Enrollment Eligibility & Tiering
- **Rule**: Students register based on GPA tiers against the Semester StartDate.
  - GPA >= 3.8: Immediately at `StartDate`.
  - GPA >= 3.5: `StartDate` + 2 hours.
  - GPA < 3.5: `StartDate` + 4 hours.
- **Enforced**: `EnrollmentService.CanStudentEnroll` and `EnrollmentValidator`.

## 2. Seat Capacity
- **Rule**: A schedule slot cannot be over-enrolled. `AvailableSeats` must be > 0.
- **Enforced**: Checked in `EnrollmentValidator`. Updated atomically in `ScheduleRepository.TryBookSeatAsync` via raw SQL.

## 3. Pre-requisites
- **Rule**: If a course has a `PrerequisiteId`, the student MUST have a prior `Enrollment` for that prerequisite course with `Status == Completed` AND `finalGrade >= 50`.
- **Enforced**: `EnrollmentValidator` and `CourseOfferingRepository.GetAvailableToRegisterAsync`.

## 4. Time Conflicts
- **Rule**: A student cannot enroll in two schedules that overlap in time on the same `DayOfWeek`.
- **Enforced**: `EnrollmentValidator`. Checks against existing DB enrollments and against other items in the batch being submitted.

## 5. Duplicate Enrollment
- **Rule**: A student cannot register for the same `CourseOffering` multiple times in a single batch, nor if they already have an active/completed enrollment for it.
- **Enforced**: `EnrollmentValidator` (batch distinct check) and `EnrollmentRepository.GetExistingAsync` (DB check).

## 6. Course Dropping (Withdrawal)
- **Rule 1**: A course cannot be dropped after the active semester's `DropDeadline`.
- **Rule 2**: A drop cannot result in the student's total enrolled credits for that semester falling below 12.
- **Enforced**: `EnrollmentValidator.ValidateDropAsync`.

## 7. Profile Photo Restrictions
- **Rule**: Photos must be JPEG or PNG and <= 5MB.
- **Enforced**: `StudentService.UploadProfilePhotoAsync`.

## 8. Soft Deletion
- **Rule**: Entities inheriting from `BaseEntity` are never hard-deleted via standard repository methods. `IsDeleted` is set to `true`.
- **Enforced**: `GenericRepository.RemoveAsync`. Global query filters (assumed based on `!e.IsDeleted` checks everywhere).

## 9. Missing Profile Information
- **Rule**: A user is considered to have an "Incomplete Profile" if any property in `UserPersonalInfo` or `UserContact` is null or empty.
- **Enforced**: `UserService.GetMissingInfo` via reflection.
