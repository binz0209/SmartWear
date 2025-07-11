using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategoryData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedOn", "DeletedOn", "Description", "IsDeleted", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("375cec24-34b1-4b40-a90c-0db470fb95ff"), new DateTime(2025, 7, 10, 0, 37, 52, 636, DateTimeKind.Local).AddTicks(6046), null, "Quần áo nam", false, new DateTime(2025, 7, 10, 0, 37, 52, 636, DateTimeKind.Local).AddTicks(6048), "Jeans" },
                    { new Guid("dcad3cee-cd18-4ebf-888d-229b1885ac02"), new DateTime(2025, 7, 10, 0, 37, 52, 636, DateTimeKind.Local).AddTicks(6059), null, "Quần áo nữ", false, new DateTime(2025, 7, 10, 0, 37, 52, 636, DateTimeKind.Local).AddTicks(6060), "Skirt" },
                    { new Guid("f39ec276-28b6-4541-8cc5-2e6b9c0afcdd"), new DateTime(2025, 7, 10, 0, 37, 52, 635, DateTimeKind.Local).AddTicks(6380), null, "Quần áo trẻ em", false, new DateTime(2025, 7, 10, 0, 37, 52, 636, DateTimeKind.Local).AddTicks(5473), "Shirt" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("375cec24-34b1-4b40-a90c-0db470fb95ff"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("dcad3cee-cd18-4ebf-888d-229b1885ac02"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("f39ec276-28b6-4541-8cc5-2e6b9c0afcdd"));

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
