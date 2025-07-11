using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("4ca7f0fb-29f7-4b90-b582-6cd224210a07"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("b8feaa13-4956-4aea-a688-921ca3ec880f"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c48eadb3-7cc2-4988-b5b9-9e937d6c9461"));

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedOn", "DeletedOn", "Description", "IsDeleted", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("4ca7f0fb-29f7-4b90-b582-6cd224210a07"), new DateTime(2025, 7, 10, 0, 41, 25, 494, DateTimeKind.Local).AddTicks(5058), null, "Quần áo nam", false, new DateTime(2025, 7, 10, 0, 41, 25, 494, DateTimeKind.Local).AddTicks(5060), "Jeans" },
                    { new Guid("b8feaa13-4956-4aea-a688-921ca3ec880f"), new DateTime(2025, 7, 10, 0, 41, 25, 494, DateTimeKind.Local).AddTicks(5062), null, "Quần áo nữ", false, new DateTime(2025, 7, 10, 0, 41, 25, 494, DateTimeKind.Local).AddTicks(5063), "Skirt" },
                    { new Guid("c48eadb3-7cc2-4988-b5b9-9e937d6c9461"), new DateTime(2025, 7, 10, 0, 41, 25, 493, DateTimeKind.Local).AddTicks(6195), null, "Quần áo trẻ em", false, new DateTime(2025, 7, 10, 0, 41, 25, 494, DateTimeKind.Local).AddTicks(4574), "Shirt" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
