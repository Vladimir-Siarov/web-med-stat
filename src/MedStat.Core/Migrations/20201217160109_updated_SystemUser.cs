using Microsoft.EntityFrameworkCore.Migrations;

namespace MedStat.Core.Migrations
{
    public partial class updated_SystemUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "SystemUser",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: null);

            migrationBuilder.AddColumn<string>(
                name: "Patronymic",
                table: "SystemUser",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: null);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "SystemUser",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: null);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "SystemUser");

            migrationBuilder.DropColumn(
                name: "Patronymic",
                table: "SystemUser");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "SystemUser");
        }
    }
}
