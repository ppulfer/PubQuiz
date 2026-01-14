using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubQuiz.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddRealOrFakeQuestionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "Questions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.Sql(
                "ALTER TABLE \"Questions\" ALTER COLUMN \"AcceptedAnswers\" TYPE jsonb USING to_jsonb(\"AcceptedAnswers\")");

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

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Games",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6);

            migrationBuilder.AddColumn<bool>(
                name: "GuessedReal",
                table: "Answers",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "IsReal",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "GuessedReal",
                table: "Answers");

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "Questions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.Sql(
                "ALTER TABLE \"Questions\" ALTER COLUMN \"AcceptedAnswers\" TYPE text[] USING ARRAY(SELECT jsonb_array_elements_text(\"AcceptedAnswers\"))");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Games",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);
        }
    }
}
