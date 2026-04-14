# Database Schema Report

## Summary
The system consists of interconnected modules dealing with Identity, Academics, Financials, and Permissions.
- **Identity & Permissions:** Users exist with various attributes. Roles and Permissions implement standard RBAC, assigned to Users. Staff and Students have an optional one-to-one relationship with Users. Users also embed `UserContact` and `UserPersonalInfo` as owned entities.
- **Academics:** Comprises the core structure: `Faculties` -> `Departments` -> `CourseOfferings` & `ProgramPlans`. It tracks `Semesters` and maps students' `Enrollments` into `Schedules` linked to a `CourseOffering` and `Staff`. `Exams` are also tied to `CourseOfferings`. `Courses` can have self-referencing prerequisites.
- **Financial:** Keeps track of `Fees` which are assigned to a `Department` and `Semester`. `StudentFees` map these onto individual `Students`. Finally, `Payments` represent the individual transactions towards a `StudentFee`.

## Potential Issues / Inconsistencies
- `Staff` entity contains a `DepartmentHeadedId` but its definition in code (`DepartmentHeadedId`) maps to `Department.Id`. Also, `Department` has a `HeadOfDepartmentId` pointing to `Staff`. This creates a bi-directional reference that might complicate updates and referential integrity (e.g. creating both simultaneously without nullability challenges).
- `Staff.UserId` and `Student.UserId` act as both Primary Key and Foreign Key to `User`. This correctly establishes 1-to-1 relationships, however `BaseEntity` is not inherited by `Staff` and `Student`. This is fine conceptually but inconsistent with the rest of the entities that use `BaseEntity.Id`.
- `Faculty` has a `DeanId` to `User` instead of `Staff`, which is logically odd since deans are typically staff members. It may have been a design decision to bypass staff specifics for top level admins, but normally Dean is a specific Staff assignment.
- `Enrollment.finalGrade` (camelCase) has non-standard casing compared to `MidtermGrade` and `ClassGrade`.
- The EF Core setup configures `User.PersonalInfo` and `User.ContactInfo` as Owned Entities (`OwnsOne`), meaning these typically end up as columns in the `Users` table itself rather than separate tables, though they are modeled as separate C# classes. The DBML treats them as columns on `Users`.

## Update: Student Requests & Invoicing System Extensions
### Student Requests System
A robust scoped request system was added. `RequestType` defines available requests, while `RequestTypeScope` bounds these types to either Global (both `FacultyId` and `DepartmentId` are null), Faculty-wide, or Department-specific. Department scope is the most specific. `StudentRequest` tracks the active workflow steps, while `RequestMessage` and `RequestDocument` track comments and requirements related to the request.

### Fees / Finance Module
The previous simple `Fee`/`StudentFee` model has been expanded to a robust `Invoice` system. `Invoice` bridges `Student` and `Semester`, aggregating totals. `InvoiceItem` lists individual charges (which may still tie back to `Fees` conceptually, but decouple the exact lines). `Payment` was updated to fulfill an `InvoiceId` instead of `StudentFeeId`, and `PaymentHistory` allows tracking life cycles (Creation, Updates, Refunds) per payment, ensuring traceability.

### Scoping Conflict/Consideration
`StudentFee` is still present. To maintain backward compatibility while fully satisfying the new `Invoice` logic, `Payment` was repointed. In a true migration, `StudentFee` might either be deprecated in favor of `InvoiceItem`, or serve as an intermediate ledger that generates `Invoices`. For now, `Invoice` correctly handles all semester billing grouping per the new requirement.
