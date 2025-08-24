using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FloristAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditPartnerField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InviteCode",
                table: "Partners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Partners",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InviteCode",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Partners");
        }
    }
}
