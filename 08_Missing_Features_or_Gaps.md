# Missing Features and Gaps

Based on the codebase analysis, the following features or flows are incomplete, missing, or require attention.

## 1. Fee Generation Logic
- **Gap**: `FinancialService` exists and can summarize `StudentFee`s, but there is no code that actually *generates* or inserts `StudentFee` records.
- **Impact**: When a student enrolls in a course, they are not being charged. Financial module is essentially read-only mock data right now.
- **Need**: Implement a `FeeGenerationService` triggered upon successful registration (e.g., via domain events or a direct service call in `EnrollmentService`).

## 2. Schedule Seat Replenishment
- **Gap**: As noted in the Logic Validation Report, dropping a course does not refund the seat capacity (`AvailableSeats + 1`).
- **Impact**: Classes will appear full even if students drop them.
- **Need**: Implement capacity restoration in the `Remove` and `SoftDelete` paths of `EnrollmentService`.

## 3. Semester Transition & GPA Persistence
- **Gap**: `Semester.IsActive` dictates current operations. There is no automated or administrative endpoint to "Close Semester".
- **Impact**: `Student.Cgpa` is never updated persistently.
- **Need**: A job or endpoint that finalizes grades, recalculates the official CGPA for all students, persists it to `Student.Cgpa`, and rolls the active semester over.

## 4. Hard Deletion Support
- **Gap**: `GenericRepository.RemoveAsync` intercepts any request for a `BaseEntity` and turns it into a soft delete.
- **Impact**: If an admin makes a mistake and legitimately needs to purge a record (e.g., a duplicated entry violating a unique constraint), they cannot do so through the API.
- **Need**: Implement a `HardDeleteAsync` method in the repository for administrative data corrections.

## 5. Course Offering Updates
- **Gap**: `CourseOfferingService.Update` is commented out as `// Need to be implemented`.
- **Impact**: If a course offering is created with a mistake (wrong semester, wrong department), it cannot be updated; it must be deleted and recreated.
- **Need**: Implement the update logic.

## 6. Exam Service Incompleteness
- **Gap**: `ExamService` only has a stub implementation. It doesn't seem to enforce exam scheduling conflicts (e.g., a student having two exams at the same time).
- **Impact**: Exam scheduling might result in impossible situations for students.
- **Need**: Implement exam conflict validation similar to schedule conflict validation.
