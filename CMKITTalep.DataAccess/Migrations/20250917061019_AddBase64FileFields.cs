using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMKITTalep.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddBase64FileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ScreenshotBase64",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScreenshotFileName",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScreenshotMimeType",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileBase64",
                table: "RequestResponses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileMimeType",
                table: "RequestResponses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "RequestResponses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScreenshotBase64",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ScreenshotFileName",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ScreenshotMimeType",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "FileBase64",
                table: "RequestResponses");

            migrationBuilder.DropColumn(
                name: "FileMimeType",
                table: "RequestResponses");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "RequestResponses");
        }
    }
}
