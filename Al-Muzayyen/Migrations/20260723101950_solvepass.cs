using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Al_Muzayyen.Migrations
{
    /// <inheritdoc />
    public partial class solvepass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StdPhone",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "endExamTime",
                table: "Exams",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "startExamTime",
                table: "Exams",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "Index_Unique_StdPhone",
                table: "Students",
                column: "StdPhone",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Index_Unique_StdPhone",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "endExamTime",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "startExamTime",
                table: "Exams");

            migrationBuilder.AlterColumn<string>(
                name: "StdPhone",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
