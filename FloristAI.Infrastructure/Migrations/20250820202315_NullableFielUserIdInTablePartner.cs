using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FloristAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NullableFielUserIdInTablePartner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_Users_UserId",
                table: "Partners");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Partners",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_Users_UserId",
                table: "Partners",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_Users_UserId",
                table: "Partners");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Partners",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_Users_UserId",
                table: "Partners",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
