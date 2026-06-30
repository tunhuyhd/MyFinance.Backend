using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFinance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminSeedAndIsAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_admin",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "avatar_url", "created_by", "created_on", "email", "full_name", "is_admin", "last_modified_by", "last_modified_on", "password_hash", "refresh_token", "refresh_token_expiry_time", "username" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), null, new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@myfinance.local", "System Administrator", true, null, null, "$2a$11$KvCWuCbUfdprIh5NNvPOYuNRDBiLiBRbwJ3BfFemiNhF7aFXQWbmC", null, null, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.DropColumn(
                name: "is_admin",
                table: "users");
        }
    }
}
