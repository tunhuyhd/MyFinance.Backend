using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWithdrawalCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "id", "color", "created_by", "created_on", "icon", "is_system", "last_modified_by", "last_modified_on", "name", "type", "user_id" },
                values: new object[] { new Guid("10000001-0000-0000-0000-000000000009"), "#14B8A6", new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "banknote", true, null, null, "Rút tiền", 2, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("10000001-0000-0000-0000-000000000009"));
        }
    }
}
