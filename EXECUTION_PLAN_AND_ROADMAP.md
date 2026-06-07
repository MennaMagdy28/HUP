# Execution Plan & Documentation Roadmap

## 1. Execution Plan

The execution plan is divided into three phases: **Stabilization**, **Feature Completion**, and **Integration & Advanced Features**. Each phase has clear milestones and deliverables.

### Phase 1: Stabilization & Core Fixes (Weeks 1-2)
**Goal:** Address critical security vulnerabilities, architectural violations, and performance bottlenecks identified in the code analysis.

**Tasks:**
1.  **Security Hardening (IDOR Fixes):**
    *   Implement resource-based authorization in `StudentsController`, `EnrollmentController`, and `FinancialController`.
    *   Ensure that users can only access/modify their own data unless they have an Admin role.
    *   *Deliverable:* Secured endpoints with integration tests verifying unauthorized access is blocked.

2.  **Architectural Refactoring:**
    *   **Repository Pattern:** Refactor `GenericRepository` to remove `virtual` methods and enforce explicit loading strategies (e.g., `GetWithDetailsAsync`). This fixes the Liskov Substitution Principle violation.
    *   **Service Layer:** Extract validation logic from `EnrollmentService` into a dedicated `IEnrollmentValidator`. This adheres to the Single Responsibility Principle.
    *   *Deliverable:* Clean, testable code with reduced cyclomatic complexity in services.

3.  **Performance Optimization:**
    *   Implement pagination for all "List" endpoints (e.g., `GetAllStudents`, `GetAllCourses`).
    *   Add database indexing for frequently queried fields (e.g., `NationalId`, `StudentId`).
    *   *Deliverable:* API endpoints that handle large datasets efficiently.

### Phase 2: Feature Completion - Financials & Academics (Weeks 3-4)
**Goal:** Implement missing core business logic to meet the functional requirements for Academic and Financial services.

**Tasks:**
1.  **Admin & Staff Management:**
    *   Create `InstructorController` to handle the lifecycle of academic staff (CRUD).
    *   Implement Role Management logic to securely assign "Instructor" or "Staff" roles to users.
    *   *Deliverable:* Endpoints for managing university staff and their permissions.

2.  **Financial Module:**
    *   Implement `FeeGenerationService` to calculate tuition fees based on credit hours and cost per credit.
    *   Create a `PaymentService` stub to handle mock transactions and update `StudentFee` status.
    *   *Deliverable:* Functional fee generation and payment tracking.

3.  **Academic Enhancements:**
    *   Enhance `ExamService` to return exam schedules linked to specific student enrollments.
    *   Implement "Waitlist" functionality for full course offerings (optional but high value).
    *   *Deliverable:* Comprehensive academic schedule and exam views.

### Phase 3: Integration & Advanced Features (Week 5)
**Goal:** Prepare the system for deployment and integration with external systems, and finalize documentation.

**Tasks:**
1.  **Notification System:**
    *   Implement a notification service (email/SMS stub) for key events (e.g., Grade Posting, Fee Payment).
    *   *Deliverable:* Notification triggers in relevant services.

2.  **LMS Integration (Mock):**
    *   Create a mock integration service for the Learning Management System (LMS) to demonstrate data exchange.
    *   *Deliverable:* API endpoints that simulate fetching assignments/grades from an LMS.

3.  **Reporting & Dashboard:**
    *   Develop an Admin Dashboard API to provide summary statistics (e.g., Total Students, Fees Collected).
    *   *Deliverable:* Insightful data for administrators.

---

## 2. Documentation Roadmap

To meet academic and engineering standards, the following documents will be created and maintained.

### 2.1 Software Requirements Specification (SRS) - *Revised*
*   **Purpose:** The single source of truth for requirements.
*   **Content:**
    *   Updated Technology Stack (.NET, SQL Server).
    *   Detailed Use Cases for core modules (Enrollment, Financials).
    *   Non-functional requirements (Performance, Security).
*   **Structure:** IEEE 830 Standard.

### 2.2 Software Design Document (SDD)
*   **Purpose:** Describes the system architecture and design decisions.
*   **Content:**
    *   **High-Level Architecture:** Layered Architecture Diagram (API, Application, Core, Infrastructure).
    *   **Database Design:** Entity Relationship Diagram (ERD).
    *   **Sequence Diagrams:** Visualizing complex flows like "Student Enrollment" and "Fee Payment".
    *   **Class Diagrams:** Core domain entities and relationships.
*   **Structure:** IEEE 1016 Standard.

### 2.3 API Documentation (Contract)
*   **Purpose:** Guide for frontend developers and external integrators.
*   **Content:**
    *   Swagger/OpenAPI specification (auto-generated from code).
    *   Detailed descriptions of request/response schemas.
    *   Error code reference.
*   **Tool:** Swagger UI / ReDoc.

### 2.4 Test Plan & Report
*   **Purpose:** Strategy for verifying system correctness.
*   **Content:**
    *   **Unit Testing:** Strategy for testing Services and Validators.
    *   **Integration Testing:** End-to-end scenarios (e.g., "Register for Course -> Check Schedule -> Pay Fees").
    *   **Security Testing:** Checklist for IDOR, SQL Injection, and XSS.
*   **Deliverable:** A comprehensive test report summarizing coverage and results.

### 2.5 User Manual
*   **Purpose:** Guide for end-users (Students, Admins).
*   **Content:**
    *   Step-by-step instructions for common tasks (e.g., "How to Drop a Course").
    *   Screenshots and troubleshooting tips.

---

## 3. Logical Order of Completion

1.  **Revised SRS:** Immediate (Baseline).
2.  **SDD:** During Phase 1 (As-Built + Planned).
3.  **API Contract:** Continuous (Auto-generated).
4.  **Test Plan:** During Phase 2 (Parallel with feature dev).
5.  **User Manual:** End of Phase 3.

This roadmap ensures that documentation evolves with the software, providing a complete and professional package at the end of the project.
