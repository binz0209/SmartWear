using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Business.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedOn", "DeletedOn", "Description", "IsDeleted", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("4ca7f0fb-29f7-4b90-b582-6cd224210a07"), new DateTime(2025, 7, 10, 0, 41, 25, 494, DateTimeKind.Local).AddTicks(5058), null, "Quần áo nam", false, new DateTime(2025, 7, 10, 0, 41, 25, 494, DateTimeKind.Local).AddTicks(5060), "Jeans" },
                    { new Guid("b8feaa13-4956-4aea-a688-921ca3ec880f"), new DateTime(2025, 7, 10, 0, 41, 25, 494, DateTimeKind.Local).AddTicks(5062), null, "Quần áo nữ", false, new DateTime(2025, 7, 10, 0, 41, 25, 494, DateTimeKind.Local).AddTicks(5063), "Skirt" },
                    { new Guid("c48eadb3-7cc2-4988-b5b9-9e937d6c9461"), new DateTime(2025, 7, 10, 0, 41, 25, 493, DateTimeKind.Local).AddTicks(6195), null, "Quần áo trẻ em", false, new DateTime(2025, 7, 10, 0, 41, 25, 494, DateTimeKind.Local).AddTicks(4574), "Shirt" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedOn", "DeletedOn", "Description", "IsDeleted", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("375cec24-34b1-4b40-a90c-0db470fb95ff"), new DateTime(2025, 7, 10, 0, 37, 52, 636, DateTimeKind.Local).AddTicks(6046), null, "Quần áo nam", false, new DateTime(2025, 7, 10, 0, 37, 52, 636, DateTimeKind.Local).AddTicks(6048), "Jeans" },
                    { new Guid("dcad3cee-cd18-4ebf-888d-229b1885ac02"), new DateTime(2025, 7, 10, 0, 37, 52, 636, DateTimeKind.Local).AddTicks(6059), null, "Quần áo nữ", false, new DateTime(2025, 7, 10, 0, 37, 52, 636, DateTimeKind.Local).AddTicks(6060), "Skirt" },
                    { new Guid("f39ec276-28b6-4541-8cc5-2e6b9c0afcdd"), new DateTime(2025, 7, 10, 0, 37, 52, 635, DateTimeKind.Local).AddTicks(6380), null, "Quần áo trẻ em", false, new DateTime(2025, 7, 10, 0, 37, 52, 636, DateTimeKind.Local).AddTicks(5473), "Shirt" }
                });
        }
    }
}
