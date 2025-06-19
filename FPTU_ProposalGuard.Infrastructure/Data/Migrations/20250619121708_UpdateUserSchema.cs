using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPTU_ProposalGuard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "two_factor_backup_codes",
                table: "User",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "User",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "User",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email_verification_code",
                table: "User",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "System_Role",
                type: "nvarchar(155)",
                maxLength: 155,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "User");

            migrationBuilder.DropColumn(
                name: "email_verification_code",
                table: "User");

            migrationBuilder.DropColumn(
                name: "description",
                table: "System_Role");

            migrationBuilder.AlterColumn<string>(
                name: "two_factor_backup_codes",
                table: "User",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "User",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
