# System Flow and Dependencies

This document maps how features interact and the execution flow across layers.

## General Execution Flow
1. **API Layer**: Receives HTTP requests. Applies implicit `FluentValidation` (via `ValidationFilter`).
2. **Security**: Applies JWT Authentication and Policy-based Authorization (e.g., `PermissionAuthorizationHandler`).
3. **Application Layer (Services)**: Contains the core business logic. Often delegates complex validation to dedicated Validator classes (e.g., `IEnrollmentValidator`).
4. **Data Layer (Repositories)**: Uses EF Core or Dapper (Raw SQL) to query and persist data. Abstracted via interfaces. Uses `CacheService` for reference data.

## Feature Dependencies
- **Auth → Users/Permissions**: Login depends on retrieving User details and Permissions, which are then cached in Redis.
- **Enrollment → Schedule → CourseOffering**: Registration requires checking Course availability, resolving to a specific Schedule, and atomically decrementing seats on that Schedule.
- **Enrollment → Semester**: Registration and Drops are bound by Semester deadlines (`StartDate`, `DropDeadline`).
- **Enrollment → ProgramPlan**: Calculating a transcript/GPA depends on `ProgramPlan` to know the maximum `FinalGrade` possible for a course in a specific department to compute percentages.
- **Financial → Enrollment** (Conceptual): While currently just reading `StudentFee`, logically, enrolling in courses should trigger fee generation (noted as missing/TODO in memory/code).

## State Transitions
### Enrollment Status
- Flow: `Registered` -> `InProgress` -> `Completed`
- Checked during prerequisite evaluation.

### File State (Profile Images)
- Uploading a new photo checks DB state for an existing path. It deletes the physical file from `wwwroot` before writing the new one, ensuring disk space is not leaked.

### Transaction State
- Operations spanning multiple tables or requiring atomic updates (like Registration modifying both `Enrollment` and `Schedule` tables) use `ITransactionService.ExecuteInTransactionAsync`.
