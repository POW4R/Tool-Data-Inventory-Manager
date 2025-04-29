using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tool_Data_Inventory_Manager.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetAttemptsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PasswordResetAttempts",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordResetAttempts",
                table: "Users");
        }
    }
}
