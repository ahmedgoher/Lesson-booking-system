using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Al_Muzayyen.Migrations
{
    /// <inheritdoc />
    public partial class AddPaperExamToExamModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExamDate",
                table: "Exams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaperExam",
                table: "Exams",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamDate",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "IsPaperExam",
                table: "Exams");
        }
    }
}
