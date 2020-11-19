using Microsoft.EntityFrameworkCore.Migrations;

namespace MedStat.Core.Migrations
{
	public partial class added_Company__and__Requisites : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
					name: "Companies",
					columns: table => new
					{
						Id = table.Column<int>(type: "int", nullable: false)
									.Annotation("SqlServer:Identity", "1, 1"),
						
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
						BankRequisites_Bank = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_Companies", x => x.Id);
					});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
					name: "Companies");
		}
	}
}
