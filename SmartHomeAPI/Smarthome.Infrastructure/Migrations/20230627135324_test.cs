using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Temperatures",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Humidities",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Temperatures",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Humidities",
                newName: "ID");
        }
    }
}
