using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Al_Muzayyen.Migrations
{
    /// <inheritdoc />
    public partial class addadminpass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Admins");
        }
    }
}
