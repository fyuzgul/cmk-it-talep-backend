using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMKITTalep.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPriorityLevelToRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First create the PriorityLevels table
            migrationBuilder.CreateTable(
                name: "PriorityLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriorityLevels", x => x.Id);
                });

            // Insert default priority levels
            migrationBuilder.InsertData(
                table: "PriorityLevels",
                columns: new[] { "Name", "IsDeleted" },
                values: new object[,]
                {
                    { "Acil", false },
                    { "Öncelikli", false },
                    { "Normal", false },
                    { "Düşük", false }
                });

            // Add the PriorityLevelId column to Requests table with a default value
            migrationBuilder.AddColumn<int>(
                name: "PriorityLevelId",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 3); // Default to "Normal" priority (Id = 3)

            // Update existing requests to have Normal priority (Id = 3)
            migrationBuilder.Sql("UPDATE Requests SET PriorityLevelId = 3 WHERE PriorityLevelId = 0");

            // Create index
            migrationBuilder.CreateIndex(
                name: "IX_Requests_PriorityLevelId",
                table: "Requests",
                column: "PriorityLevelId");

            // Add foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Requests_PriorityLevels_PriorityLevelId",
                table: "Requests",
                column: "PriorityLevelId",
                principalTable: "PriorityLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_PriorityLevels_PriorityLevelId",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "PriorityLevels");

            migrationBuilder.DropIndex(
                name: "IX_Requests_PriorityLevelId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "PriorityLevelId",
                table: "Requests");
        }
    }
}
