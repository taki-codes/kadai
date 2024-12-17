using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kadai_games.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomPropertiesToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Delete_Flg",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Delete_Flg",
                table: "AspNetUsers");
        }
    }
}
