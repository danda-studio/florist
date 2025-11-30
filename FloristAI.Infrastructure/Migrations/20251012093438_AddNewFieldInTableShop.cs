using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FloristAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldInTableShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlGoogleMap",
                table: "Shops",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlGoogleMap",
                table: "Shops");
        }
    }
}
