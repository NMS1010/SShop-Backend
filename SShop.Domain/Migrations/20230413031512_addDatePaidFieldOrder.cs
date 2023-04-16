using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SShop.Domain.Migrations
{
    public partial class addDatePaidFieldOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DatePaid",
                table: "Order",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatePaid",
                table: "Order");
        }
    }
}
