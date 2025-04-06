using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "22222222-2222-2222-2222-222222222222",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "04d5422c-d62f-4d41-8d4c-d70ac547e496", "AQAAAAIAAYagAAAAEJeySrostD/e3GSZc0ID0OaNXMTLh9RJTKTMbZal2Erq0K2r8xmn/TQ6JBR55apbew==", "a4221cf0-bb7f-4636-b7df-093f0bf3b967" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "22222222-2222-2222-2222-222222222222",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "75df8232-5255-4082-bef9-0f8bd9901309", "AQAAAAIAAYagAAAAEONaAxq6CuVl3QJJPJ0r6RZa5oDDIVEzlDP1K/M8BVJ56csFKxiSx6QsIqcSBpj7vA==", "ceed9484-0cc6-4e55-9c7e-f0d7e899f42c" });
        }
    }
}
