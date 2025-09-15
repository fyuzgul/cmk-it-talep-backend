using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMKITTalep.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRequestResponseType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestResponses_RequestResponseTypes_RequestResponseTypeId",
                table: "RequestResponses");

            migrationBuilder.DropTable(
                name: "RequestResponseTypes");

            migrationBuilder.DropIndex(
                name: "IX_RequestResponses_RequestResponseTypeId",
                table: "RequestResponses");

            migrationBuilder.DropColumn(
                name: "RequestResponseTypeId",
                table: "RequestResponses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestResponseTypeId",
                table: "RequestResponses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RequestResponseTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestResponseTypes", x => x.Id);
                });

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
    }
}
