# Architectural Review Report

## 1. Generic Repository Audit
**Status**: The `GenericRepository<T>` currently uses `virtual` methods which are overridden in almost every derived repository (e.g., `StudentRepository`, `EnrollmentRepository`) primarily to inject `Include()` statements for Eager Loading.
**Issue**: This violates the **Liskov Substitution Principle (LSP)** implicitly, as the base method returns a "shallow" entity while the derived method returns a "deep" graph. Callers might assume one behavior and get another.
**Recommendation**:
- Revert `GenericRepository` methods to non-virtual to enforce standard behavior.
- Rename overridden methods in derived repositories to be explicit about their data loading (e.g., `GetStudentWithDetailsAsync` instead of `GetByIdReadOnly`).
- This improves clarity and avoids "surprise" data loading or missing data when using the generic interface.

## 2. Interface Segregation
**Status**: **Compliant**. The Service layer correctly depends on interfaces (`IStudentRepository`, etc.) rather than concrete implementations.

## 3. Encapsulation
**Status**: **Compliant**. Controllers are consistently returning DTOs (e.g., `StudentProfileDto`, `EnrollmentResponseDto`) rather than Domain Entities.

## 4. Best Practices: EnrollmentService
**Status**: **Violation**. The `EnrollmentService` suffers from "Fat Service" syndrome.
**Issue**: The `AddAsync` method contains over 100 lines of mixed orchestration and validation logic (GPA checks, Time Conflicts, Prerequisites, Capacity).
**Recommendation**: Extract the validation logic into a dedicated `IEnrollmentValidator` service. This adheres to the **Single Responsibility Principle (SRP)** and makes the enrollment flow testable and readable.

## 5. SOLID Violations Summary
- **SRP**: `EnrollmentService` handles both transaction management and business rule validation.
- **LSP**: `GenericRepository` virtual overrides change data loading semantics.
- **OCP**: Adding new enrollment rules requires modifying the core Service method.

## Proposed Refactoring Plan
1.  **Extract Validator**: Move GPA, Prerequisite, Conflict, and Capacity checks to `EnrollmentValidator`.
2.  **Refactor Repositories**: Remove `virtual` from `GenericRepository`. Rename derived "Get" methods to reflect Eager Loading (e.g., `GetWithSchedulesAsync`). Update Services to call these specific methods.
