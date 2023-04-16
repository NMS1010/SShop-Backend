using Microsoft.EntityFrameworkCore.Migrations;

namespace SShop.Domain.Migrations
{
    public partial class removeShipping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Shipping",
                table: "Order");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Shipping",
                table: "Order",
                type: "DECIMAL",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
