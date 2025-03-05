using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinTrack.API.Migrations
{
    public partial class add_accountId_to_monobankTransaction_entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "MonobankTransactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "22222222-2222-2222-2222-222222222222",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9223f004-3afc-4dd2-994e-76e8791d2736", "AQAAAAIAAYagAAAAEGfJ8x3gt35VBb8gUhIofKm3Cwb8UhBAzkd7FrhYoZbP4FwClDl0WJ39GLGjLmU+0Q==", "d9be0362-51a5-410b-afa9-d72d87e76b4c" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "MonobankTransactions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "22222222-2222-2222-2222-222222222222",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f83d7d8b-b89b-4db1-b080-3b41bb863236", "AQAAAAIAAYagAAAAEEyQEgsYfwfhOBnTpBLOYc98wZzOnee/SVVG0RHkLJwkXI6Md5U9iw8jfHOiN9qIlw==", "9df62eb7-9505-4e58-b615-d197dade7d90" });
        }
    }
}
