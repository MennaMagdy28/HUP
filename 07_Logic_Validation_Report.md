# Logic Validation Report ⚠️

This report identifies critical bugs, risks, and architectural flaws in the codebase.

## 1) Logical Errors

**ID:** L-01
**Location:** `EnrollmentService.Remove(Guid id)` / `EnrollmentService.SoftDelete(Guid id)`
**Problem:** Dropping a course soft-deletes the `Enrollment` record, but it **does not** increment `AvailableSeats` back on the `Schedule`.
**Why it is wrong:** If a student drops a course, their seat remains occupied forever, reducing capacity artificially and preventing other students from joining.
**Suggested fix:** Add raw SQL execution inside the drop logic to increment the seat: `UPDATE Schedules SET AvailableSeats = AvailableSeats + 1 WHERE Id = {enrollment.ScheduleId}`.

**ID:** L-02
**Location:** `EnrollmentValidator.ValidateDropAsync`
**Problem:** The minimum credits check calculates the student's *current* enrolled credits for the semester and subtracts the dropping course. However, it uses `CurrentEnrollments.Sum(e => e.CourseOffering?.Course?.Credits ?? 0)`.
**Why it is wrong:** If `CourseOffering` or `Course` navigation properties are null (due to missing Includes), it sums `0`, causing the subtraction to immediately fail the `< 12` check incorrectly.
**Suggested fix:** Ensure `GetByStudentAndSemesterAsync` explicitly includes `CourseOffering.Course`.

## 2) Null Reference Risks

**ID:** N-01
**Location:** `EnrollmentValidator.ValidateEnrollmentAsync`
**Problem:** `if (targetSchedule.StartTime < existingSlot.EndTime && targetSchedule.EndTime > existingSlot.StartTime)`
**Why it is wrong:** In the fallback logic for older data, it retrieves `enrolledSchedules` from `enrolled.CourseOffering?.Schedules`. If `CourseOffering` is not loaded, this is null. Furthermore, `enrolled.CourseOffering.Course.CourseCode` is accessed inside the exception message without checking if `Course` is null.
**Suggested fix:** Ensure proper eager loading, or use null-conditional operators safely. Use a generic conflict message if `CourseCode` is unavailable.

**ID:** N-02
**Location:** `ScheduleRepository.GetByStudentEnrollmentsAsync`
**Problem:** `StaffName = e.Schedule.Staff.User.FullName`
**Why it is wrong:** If a Schedule does not have a Staff assigned yet, or if `Staff` doesn't include `User`, this throws a Null Reference Exception in LINQ to Objects (if evaluated locally) or might generate bad SQL.
**Suggested fix:** Use null propagation: `StaffName = e.Schedule.Staff != null ? e.Schedule.Staff.User.FullName : null`.

## 3) Data Integrity Issues

**ID:** D-01
**Location:** `EnrollmentService.GetStudentGradesAsync`
**Problem:** Dynamic GPA Calculation vs. `Student.Cgpa` mismatch.
**Why it is wrong:** The system calculates cumulative GPA on the fly by fetching grade models, but `Student.Cgpa` is the property used to determine enrollment priority tier (`CanStudentEnroll`). There is no visible mechanism syncing the dynamically calculated GPA back to `Student.Cgpa`.
**Suggested fix:** Implement a background job or an event hook at the end of the semester to calculate and persist the final `Cgpa` to the `Student` entity.

## 4) Performance Issues

**ID:** P-01
**Location:** `UserService.GetMissingInfo`
**Problem:** Uses Reflection on every login (`GetProfileStatus` is called inside `LoginAsync`).
**Why it is wrong:** Reflection (`GetType().GetProperties()`) is computationally expensive and is executed twice (for PersonalInfo and ContactInfo) during the critical path of logging in.
**Suggested fix:** Cache the property list statically, or hardcode the checks (since the DTO/Entity structure rarely changes).

**ID:** P-02
**Location:** `EnrollmentRepository.GetByStudentAndSemesterAsync`
**Problem:** `ThenInclude(co => co.Schedules)` is called.
**Why it is wrong:** Fetching all schedules for a course offering when verifying a student's enrollment pulls unnecessary data, especially if a course has many slots. The student is only enrolled in ONE schedule per course.
**Suggested fix:** Since `Enrollment` already has `ScheduleId` and `Schedule` navigation property, rely on `Include(e => e.Schedule)` and remove `ThenInclude(co => co.Schedules)`.

## 5) Architectural Problems

**ID:** A-01
**Location:** `ScheduleService.Remove`
**Problem:** Comments indicate confusion between Soft Delete and Hard Delete due to `GenericRepository`'s abstraction.
**Why it is wrong:** `GenericRepository.RemoveAsync` forces a Soft Delete if the entity is `BaseEntity`. The service exposes both `SoftDelete` and `Remove`, but both perform soft deletes under the hood. This creates misleading API contracts.
**Suggested fix:** Clarify the Repository interface. Implement an explicit `HardDeleteAsync` if needed, or remove the duplicate method from the Service.

## 6) Inconsistent Behavior

**ID:** I-01
**Location:** `StudentService.UploadProfilePhotoAsync`
**Problem:** Directly interacts with `wwwroot` and `System.IO.File`.
**Why it is wrong:** Tightly couples the Application layer to the file system. In a cloud environment (e.g., Azure App Service, Docker containers), local file storage is volatile.
**Suggested fix:** Abstract file storage behind an `IFileStorageService` interface to allow plugging in Azure Blob Storage or AWS S3.
