using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MedStat.Core.Migrations
{
    public partial class added_CompanyRequisites__updated_Company : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankRequisites_AccountNumber",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "BankRequisites_BIC",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "BankRequisites_Bank",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "BankRequisites_CorrespondentAccount",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "MainRequisites_FullName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "MainRequisites_INN",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "MainRequisites_KPP",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "MainRequisites_LegalAddress",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "MainRequisites_Name",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "MainRequisites_OGRN",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "MainRequisites_OKATO",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "MainRequisites_OKPO",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "MainRequisites_PostalAddress",
                table: "Companies");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                table: "Companies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedUtc",
                table: "Companies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "CompanyRequisites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MainRequisites_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MainRequisites_FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    MainRequisites_LegalAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    MainRequisites_PostalAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    MainRequisites_OGRN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MainRequisites_OKPO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MainRequisites_OKATO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MainRequisites_INN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MainRequisites_KPP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankRequisites_AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankRequisites_BIC = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankRequisites_CorrespondentAccount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankRequisites_Bank = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyRequisites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyRequisites_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRequisites_CompanyId",
                table: "CompanyRequisites",
                column: "CompanyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyRequisites");

            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "UpdatedUtc",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "BankRequisites_AccountNumber",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankRequisites_BIC",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankRequisites_Bank",
                table: "Companies",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankRequisites_CorrespondentAccount",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainRequisites_FullName",
                table: "Companies",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainRequisites_INN",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainRequisites_KPP",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainRequisites_LegalAddress",
                table: "Companies",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainRequisites_Name",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainRequisites_OGRN",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainRequisites_OKATO",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainRequisites_OKPO",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainRequisites_PostalAddress",
                table: "Companies",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }
    }
}
