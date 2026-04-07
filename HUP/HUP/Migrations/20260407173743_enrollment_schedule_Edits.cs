using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HUP.Migrations
{
    /// <inheritdoc />
    public partial class enrollment_schedule_Edits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScheduleId",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ScheduleId1",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_ScheduleId",
                table: "Enrollments",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_ScheduleId1",
                table: "Enrollments",
                column: "ScheduleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Schedules_ScheduleId",
                table: "Enrollments",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Schedules_ScheduleId1",
                table: "Enrollments",
                column: "ScheduleId1",
                principalTable: "Schedules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Schedules_ScheduleId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Schedules_ScheduleId1",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_ScheduleId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_ScheduleId1",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "ScheduleId1",
                table: "Enrollments");
        }
    }
}
