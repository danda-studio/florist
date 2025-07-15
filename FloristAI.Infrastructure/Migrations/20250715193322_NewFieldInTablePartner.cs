using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FloristAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewFieldInTablePartner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Partners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Partners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Partners",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Partners");
        }
    }
}
