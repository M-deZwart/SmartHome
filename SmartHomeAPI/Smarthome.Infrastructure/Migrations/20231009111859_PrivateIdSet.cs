using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PrivateIdSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sensors",
                keyColumn: "Id",
                keyValue: new Guid("4b1afb39-0e95-4c6c-95cb-3551cdc599be"));

            migrationBuilder.DeleteData(
                table: "Sensors",
                keyColumn: "Id",
                keyValue: new Guid("98690b30-1b87-4870-9939-a79f9c198625"));

            migrationBuilder.DeleteData(
                table: "Sensors",
                keyColumn: "Id",
                keyValue: new Guid("d2595ce4-ecb5-441e-a59f-f7628b349281"));

            migrationBuilder.InsertData(
                table: "Sensors",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { new Guid("66e576ee-bc79-47a1-87fc-68bd33f2882b"), "Bedroom" },
                    { new Guid("6dcc0fc2-bfb2-4432-a4c7-90b198b654dd"), "WorkSpace" },
                    { new Guid("7ad7c0bf-f832-4c58-b4c7-aee45c50ee32"), "LivingRoom" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sensors",
                keyColumn: "Id",
                keyValue: new Guid("66e576ee-bc79-47a1-87fc-68bd33f2882b"));

            migrationBuilder.DeleteData(
                table: "Sensors",
                keyColumn: "Id",
                keyValue: new Guid("6dcc0fc2-bfb2-4432-a4c7-90b198b654dd"));

            migrationBuilder.DeleteData(
                table: "Sensors",
                keyColumn: "Id",
                keyValue: new Guid("7ad7c0bf-f832-4c58-b4c7-aee45c50ee32"));

            migrationBuilder.InsertData(
                table: "Sensors",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { new Guid("4b1afb39-0e95-4c6c-95cb-3551cdc599be"), "LivingRoom" },
                    { new Guid("98690b30-1b87-4870-9939-a79f9c198625"), "Bedroom" },
                    { new Guid("d2595ce4-ecb5-441e-a59f-f7628b349281"), "WorkSpace" }
                });
        }
    }
}
