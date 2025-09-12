using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMKITTalep.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MoveRequestResponseTypeToRequestResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_RequestResponseTypes_RequestResponseTypeId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_RequestResponseTypeId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RequestResponseTypeId",
                table: "Requests");

            migrationBuilder.AlterColumn<int>(
                name: "SupportProviderId",
                table: "Requests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RequestResponseTypeId",
                table: "RequestResponses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestResponses_RequestResponseTypeId",
                table: "RequestResponses",
                column: "RequestResponseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestResponses_RequestResponseTypes_RequestResponseTypeId",
                table: "RequestResponses",
                column: "RequestResponseTypeId",
                principalTable: "RequestResponseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestResponses_RequestResponseTypes_RequestResponseTypeId",
                table: "RequestResponses");

            migrationBuilder.DropIndex(
                name: "IX_RequestResponses_RequestResponseTypeId",
                table: "RequestResponses");

            migrationBuilder.DropColumn(
                name: "RequestResponseTypeId",
                table: "RequestResponses");

            migrationBuilder.AlterColumn<int>(
                name: "SupportProviderId",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestResponseTypeId",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestResponseTypeId",
                table: "Requests",
                column: "RequestResponseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_RequestResponseTypes_RequestResponseTypeId",
                table: "Requests",
                column: "RequestResponseTypeId",
                principalTable: "RequestResponseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
