# Data Model and Relationships

This document outlines the EF Core data structure, relationships, and constraints.

## Core Entities
All entities inherit from `BaseEntity` (except `ProgramPlan`, `UserContact`, `UserPersonalInfo`, `StudentRequest`):
- `Guid Id`
- `DateTime CreatedAt`
- `DateTime? UpdatedAt`
- `bool IsDeleted` (Soft Delete flag used in Global Query Filters).

## 1. Identity & Permissions
- **User**
  - 1-1 with `UserPersonalInfo` and `UserContact`.
  - M-1 with `Role`.
  - Can be extended logically (but not inherited) by `Staff` and `Student` via 1-1 `UserId`.
- **Role**
  - M-1 with User (CreatedBy).
  - 1-M with `User` (assigned to).
  - 1-M with `RolePermission`.
- **Permission**
  - 1-M with `RolePermission`.

## 2. Academics & Structure
- **Faculty**
  - 1-1 Dean (`User`).
  - 1-M with `Department`.
- **Department**
  - M-1 with `Faculty`.
  - 1-1 HeadOfDepartment (`Staff`).
  - 1-M with `CourseOffering`, `Fee`, `Staff`.
- **Staff**
  - 1-1 with `User`.
  - M-1 with `Department`, `Faculty`.
  - 1-M with `Schedule`.
- **Student**
  - 1-1 with `User`.
  - M-1 with `Department`.
  - 1-M with `Enrollment`.
  - Contains `Cgpa` which dictates enrollment priority.

## 3. Courses, Schedules & Enrollments
- **Course**
  - Self-referencing M-1 `PrerequisiteId`.
  - 1-M with `CourseOffering`.
- **Semester**
  - Holds `StartDate`, `EndDate`, `RegistrationDeadline`, `DropDeadline`, `IsActive`.
- **CourseOffering**
  - Junction mapping `Course`, `Department`, `Semester`.
  - 1-M with `Schedule`, `Enrollment`, `Exam`.
- **Schedule (Slot)**
  - M-1 with `CourseOffering` and `Staff`.
  - Tracks `TotalSeats`, `AvailableSeats`.
  - Holds `DayOfWeek`, `StartTime`, `EndTime`, `Group`, `Hall`.
  - 1-M with `Enrollment`.
- **Enrollment**
  - M-1 with `Student`, `CourseOffering`, `Schedule`.
  - Holds `ClassGrade`, `MidtermGrade`, `finalGrade`, `Status`.
- **ProgramPlan**
  - Composite Key (`DepartmentId`, `CourseId`).
  - Junction between `Department` and `Course`.
  - Holds `RequirementType`, `IsCompulsory`, `FinalGrade` (Max points).

## 4. Financial
- **Fee**
  - Template for charges. M-1 with `Department`, `Semester`.
  - `IsPerCredit` determines if amount scales with course credits.
- **StudentFee**
  - Actual charge for a student. M-1 with `Student`, `Fee`.
  - Tracks `Amount`, `PaidAmount`, `DueDate`, `Status`.
- **Payment**
  - Transaction record. M-1 with `StudentFee`.

## Relationship Rules & Risks
- **Risk / Missing Constraint**: `Student.Cgpa` is updated when? The logic to update `Cgpa` does not appear actively in the codebase after a semester finishes. The system calculates it dynamically in `GetStudentGradesAsync` but does not persist it back to `Student.Cgpa`. This could cause the dynamic calculation and the stored GPA (used for enrollment tiering) to drift apart.
- **Missing Constraint**: `CourseOffering` can have multiple schedules. A student enrolls in a specific `Schedule` (slot). The code does not enforce that the `ScheduleId` belongs to the `CourseOfferingId` at the database schema level (though checked in code).
- **Missing Relationship**: The `ProgramPlan` is treated as a configuration but not heavily linked to enrollment limits, beyond providing the max grade.
