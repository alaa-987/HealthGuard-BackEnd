using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthGurad.Repository.Migrations
{
    public partial class AddPicUrlToCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "ProductCategories");
        }
    }
}
