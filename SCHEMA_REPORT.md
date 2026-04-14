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
