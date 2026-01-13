using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubQuiz.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimateQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CorrectValue",
                table: "Questions",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TolerancePercent",
                table: "Questions",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Questions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeviationPercent",
                table: "Answers",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimateValue",
                table: "Answers",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectValue",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "TolerancePercent",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "DeviationPercent",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "EstimateValue",
                table: "Answers");
        }
    }
}
