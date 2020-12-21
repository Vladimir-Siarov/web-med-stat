using Microsoft.EntityFrameworkCore.Migrations;

namespace MedStat.Core.Migrations
{
    public partial class fixed_table_names : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_SystemUser_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_SystemUser_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_SystemUser_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_SystemUser_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUser_Companies_CompanyId",
                table: "CompanyUser");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUser_SystemUser_SystemUserId",
                table: "CompanyUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemUser",
                table: "SystemUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyUser",
                table: "CompanyUser");

            migrationBuilder.RenameTable(
                name: "SystemUser",
                newName: "SystemUsers");

            migrationBuilder.RenameTable(
                name: "CompanyUser",
                newName: "CompanyUsers");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyUser_SystemUserId",
                table: "CompanyUsers",
                newName: "IX_CompanyUsers_SystemUserId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyUser_CompanyId",
                table: "CompanyUsers",
                newName: "IX_CompanyUsers_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemUsers",
                table: "SystemUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyUsers",
                table: "CompanyUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_SystemUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "SystemUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_SystemUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "SystemUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_SystemUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "SystemUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_SystemUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "SystemUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUsers_Companies_CompanyId",
                table: "CompanyUsers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUsers_SystemUsers_SystemUserId",
                table: "CompanyUsers",
                column: "SystemUserId",
                principalTable: "SystemUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            
            // additional changes for "Companies" table
            migrationBuilder.AlterColumn<string>(
                name: "CreatedUtc",
                table: "Companies",
                type: "datetime2",
                nullable: false,
                defaultValue: null);

            migrationBuilder.AlterColumn<string>(
	            name: "UpdatedUtc",
	            table: "Companies",
	            type: "datetime2",
	            nullable: false,
	            defaultValue: null);
    }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_SystemUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_SystemUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_SystemUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_SystemUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUsers_Companies_CompanyId",
                table: "CompanyUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUsers_SystemUsers_SystemUserId",
                table: "CompanyUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemUsers",
                table: "SystemUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyUsers",
                table: "CompanyUsers");

            migrationBuilder.RenameTable(
                name: "SystemUsers",
                newName: "SystemUser");

            migrationBuilder.RenameTable(
                name: "CompanyUsers",
                newName: "CompanyUser");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyUsers_SystemUserId",
                table: "CompanyUser",
                newName: "IX_CompanyUser_SystemUserId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyUsers_CompanyId",
                table: "CompanyUser",
                newName: "IX_CompanyUser_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemUser",
                table: "SystemUser",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyUser",
                table: "CompanyUser",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_SystemUser_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "SystemUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_SystemUser_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "SystemUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_SystemUser_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "SystemUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_SystemUser_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "SystemUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUser_Companies_CompanyId",
                table: "CompanyUser",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUser_SystemUser_SystemUserId",
                table: "CompanyUser",
                column: "SystemUserId",
                principalTable: "SystemUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
