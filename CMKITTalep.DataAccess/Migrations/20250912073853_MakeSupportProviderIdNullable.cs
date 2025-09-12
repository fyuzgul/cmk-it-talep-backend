using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMKITTalep.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MakeSupportProviderIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int?>(
                name: "SupportProviderId",
                table: "Requests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SupportProviderId",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int?),
                oldType: "int",
                oldNullable: true);
        }
    }
}
