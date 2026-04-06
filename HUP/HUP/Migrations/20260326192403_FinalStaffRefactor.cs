using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HUP.Migrations
{
    public partial class FinalStaffRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, add the new columns
            migrationBuilder.AddColumn<Guid>(
                name: "FacultyId",
                table: "Instructors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Instructors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Rename Title column
            migrationBuilder.RenameColumn(
                name: "AcademicTitle",
                table: "Instructors",
                newName: "Title");

            // Drop foreign keys referencing Instructors table FIRST (to prevent constraint violation)
            migrationBuilder.DropForeignKey(name: "FK_Departments_Instructors_HeadOfDepartmentId", table: "Departments");
            migrationBuilder.DropForeignKey(name: "FK_Schedules_Instructors_InstructorId", table: "Schedules");

            // Drop existing outgoing foreign keys on Instructors to prevent duplicates
            migrationBuilder.DropForeignKey(name: "FK_Instructors_Departments_DepartmentId", table: "Instructors");
            migrationBuilder.DropForeignKey(name: "FK_Instructors_Users_UserId", table: "Instructors");

            // Migrate data to update foreign keys in child tables from old Id to new UserId
            migrationBuilder.Sql(@"
                -- Update Schedules to use UserId instead of old Instructor Id
                UPDATE s
                SET s.InstructorId = i.UserId
                FROM Schedules s
                INNER JOIN Instructors i ON s.InstructorId = i.Id;

                -- Update Departments to use UserId instead of old Instructor Id
                UPDATE d
                SET d.HeadOfDepartmentId = i.UserId
                FROM Departments d
                INNER JOIN Instructors i ON d.HeadOfDepartmentId = i.Id;
            ");

            // Rename InstructorId columns to StaffId
            migrationBuilder.RenameColumn(name: "InstructorId", table: "Schedules", newName: "StaffId");

            // Rename InstructorName column to StaffName
            migrationBuilder.RenameColumn(name: "InstructorName", table: "Schedules", newName: "StaffName");

            // Rename HeadOfDepartmentId to HeadOfDepartmentId (unchanged)

            // Drop primary key on Instructors and related index
            migrationBuilder.DropPrimaryKey(name: "PK_Instructors", table: "Instructors");
            migrationBuilder.DropIndex(name: "IX_Instructors_UserId", table: "Instructors");

            // Now safely drop old columns
            migrationBuilder.DropColumn(name: "Id", table: "Instructors");
            migrationBuilder.DropColumn(name: "CreatedAt", table: "Instructors");
            migrationBuilder.DropColumn(name: "UpdatedAt", table: "Instructors");
            migrationBuilder.DropColumn(name: "IsDeleted", table: "Instructors");

            // Alter DepartmentId to be nullable
            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "Instructors",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            // Rename the table
            migrationBuilder.RenameTable(name: "Instructors", newName: "Staff");

            // Set PK to UserId
            migrationBuilder.AddPrimaryKey(name: "PK_Staff", table: "Staff", column: "UserId");

            // Create Indexes
            migrationBuilder.CreateIndex(name: "IX_Staff_FacultyId", table: "Staff", column: "FacultyId");

            // Recreate FKs
            migrationBuilder.AddForeignKey(
                name: "FK_Staff_Departments_DepartmentId",
                table: "Staff",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_Faculties_FacultyId",
                table: "Staff",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_Users_UserId",
                table: "Staff",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Staff_HeadOfDepartmentId",
                table: "Departments",
                column: "HeadOfDepartmentId",
                principalTable: "Staff",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Staff_StaffId",
                table: "Schedules",
                column: "StaffId",
                principalTable: "Staff",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Not adding full down for brevity, but would be reverse operations
        }
    }
}
