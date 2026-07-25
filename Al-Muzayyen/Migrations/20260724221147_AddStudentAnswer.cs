using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Al_Muzayyen.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "SubmittedAt",
                table: "StudentExams",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "AttemptNumber",
                table: "StudentExams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "StudentExams",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsSubmitted",
                table: "StudentExams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "StudentExams",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "StudentAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentExamId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    QuestionOptionId = table.Column<int>(type: "int", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    EarnedMarks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_QuestionOptions_QuestionOptionId",
                        column: x => x.QuestionOptionId,
                        principalTable: "QuestionOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_StudentExams_StudentExamId",
                        column: x => x.StudentExamId,
                        principalTable: "StudentExams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_QuestionId",
                table: "StudentAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_QuestionOptionId",
                table: "StudentAnswers",
                column: "QuestionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_StudentExamId",
                table: "StudentAnswers",
                column: "StudentExamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "AttemptNumber",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "IsSubmitted",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "StudentExams");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SubmittedAt",
                table: "StudentExams",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
