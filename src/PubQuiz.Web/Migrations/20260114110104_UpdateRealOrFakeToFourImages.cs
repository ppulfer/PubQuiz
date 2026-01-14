using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubQuiz.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRealOrFakeToFourImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "IsReal",
                table: "Questions");

            migrationBuilder.AddColumn<int>(
                name: "CorrectImageIndex",
                table: "Questions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "Questions",
                type: "jsonb",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectImageIndex",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "Questions");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Questions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReal",
                table: "Questions",
                type: "boolean",
                nullable: true);
        }
    }
}
