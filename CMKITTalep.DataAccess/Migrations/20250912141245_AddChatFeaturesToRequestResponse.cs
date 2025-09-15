using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMKITTalep.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddChatFeaturesToRequestResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "RequestResponses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "RequestResponses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SenderId",
                table: "RequestResponses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestResponses_SenderId",
                table: "RequestResponses",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestResponses_Users_SenderId",
                table: "RequestResponses",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestResponses_Users_SenderId",
                table: "RequestResponses");

            migrationBuilder.DropIndex(
                name: "IX_RequestResponses_SenderId",
                table: "RequestResponses");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "RequestResponses");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "RequestResponses");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "RequestResponses");
        }
    }
}
