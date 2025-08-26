using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FloristAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldInTablePartner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrivateSpreadsheetId",
                table: "Partners",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrivateSpreadsheetId",
                table: "Partners");
        }
    }
}
