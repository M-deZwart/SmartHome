using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Check : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Temperatures",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<double>(
                name: "Celsius",
                table: "Temperatures",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Temperatures",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "SensorId",
                table: "Temperatures",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<double>(
                name: "Percentage",
                table: "Humidities",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Humidities",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Humidities",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "SensorId",
                table: "Humidities",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Sensors",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { new Guid("4b1afb39-0e95-4c6c-95cb-3551cdc599be"), "LivingRoom" },
                    { new Guid("98690b30-1b87-4870-9939-a79f9c198625"), "Bedroom" },
                    { new Guid("d2595ce4-ecb5-441e-a59f-f7628b349281"), "WorkSpace" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Temperatures_SensorId",
                table: "Temperatures",
                column: "SensorId");

            migrationBuilder.CreateIndex(
                name: "IX_Humidities_SensorId",
                table: "Humidities",
                column: "SensorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Humidities_Sensors_SensorId",
                table: "Humidities",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Temperatures_Sensors_SensorId",
                table: "Temperatures",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Humidities_Sensors_SensorId",
                table: "Humidities");

            migrationBuilder.DropForeignKey(
                name: "FK_Temperatures_Sensors_SensorId",
                table: "Temperatures");

            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropIndex(
                name: "IX_Temperatures_SensorId",
                table: "Temperatures");

            migrationBuilder.DropIndex(
                name: "IX_Humidities_SensorId",
                table: "Humidities");

            migrationBuilder.DropColumn(
                name: "SensorId",
                table: "Temperatures");

            migrationBuilder.DropColumn(
                name: "SensorId",
                table: "Humidities");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Temperatures",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<double>(
                name: "Celsius",
                table: "Temperatures",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Temperatures",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<double>(
                name: "Percentage",
                table: "Humidities",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Humidities",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Humidities",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");
        }
    }
}
