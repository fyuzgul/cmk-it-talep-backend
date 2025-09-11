using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMKITTalep.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ReverseRequestTypeSupportTypeRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportTypes_RequestTypes_RequestTypeId",
                table: "SupportTypes");

            migrationBuilder.DropIndex(
                name: "IX_SupportTypes_RequestTypeId",
                table: "SupportTypes");

            migrationBuilder.DropColumn(
                name: "RequestTypeId",
                table: "SupportTypes");

            migrationBuilder.AddColumn<int>(
                name: "SupportTypeId",
                table: "RequestTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RequestTypes_SupportTypeId",
                table: "RequestTypes",
                column: "SupportTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTypes_SupportTypes_SupportTypeId",
                table: "RequestTypes",
                column: "SupportTypeId",
                principalTable: "SupportTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestTypes_SupportTypes_SupportTypeId",
                table: "RequestTypes");

            migrationBuilder.DropIndex(
                name: "IX_RequestTypes_SupportTypeId",
                table: "RequestTypes");

            migrationBuilder.DropColumn(
                name: "SupportTypeId",
                table: "RequestTypes");

            migrationBuilder.AddColumn<int>(
                name: "RequestTypeId",
                table: "SupportTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SupportTypes_RequestTypeId",
                table: "SupportTypes",
                column: "RequestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportTypes_RequestTypes_RequestTypeId",
                table: "SupportTypes",
                column: "RequestTypeId",
                principalTable: "RequestTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
