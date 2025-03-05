using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FinTrack.API.Migrations
{
    public partial class add_monobank_transaction_entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.CreateTable(
                name: "MonobankTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Time = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Mcc = table.Column<int>(type: "integer", nullable: false),
                    OriginalMcc = table.Column<int>(type: "integer", nullable: false),
                    Hold = table.Column<bool>(type: "boolean", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    OperationAmount = table.Column<long>(type: "bigint", nullable: false),
                    CurrencyCode = table.Column<int>(type: "integer", nullable: false),
                    CommissionRate = table.Column<long>(type: "bigint", nullable: false),
                    CashbackAmount = table.Column<long>(type: "bigint", nullable: false),
                    Balance = table.Column<long>(type: "bigint", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    ReceiptId = table.Column<string>(type: "text", nullable: false),
                    InvoiceId = table.Column<string>(type: "text", nullable: false),
                    CounterEdrpou = table.Column<string>(type: "text", nullable: false),
                    CounterIban = table.Column<string>(type: "text", nullable: false),
                    CounterName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonobankTransactions", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "22222222-2222-2222-2222-222222222222",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f83d7d8b-b89b-4db1-b080-3b41bb863236", "AQAAAAIAAYagAAAAEEyQEgsYfwfhOBnTpBLOYc98wZzOnee/SVVG0RHkLJwkXI6Md5U9iw8jfHOiN9qIlw==", "9df62eb7-9505-4e58-b615-d197dade7d90" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonobankTransactions");

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "22222222-2222-2222-2222-222222222222",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f6dcba0e-d416-4c20-8ecc-9d27c4c60076", "AQAAAAIAAYagAAAAENzhdtMLubDDUqmKK9ibPTXF/ICmBGOCsAILDH5nrdxjV4SQXrRMTrDRpCI7Q1LFPw==", "2524695c-fe2f-45f3-b72d-0b4077052f01" });
        }
    }
}
