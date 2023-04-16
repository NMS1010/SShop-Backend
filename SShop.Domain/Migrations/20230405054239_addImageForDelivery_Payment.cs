using Microsoft.EntityFrameworkCore.Migrations;

namespace SShop.Domain.Migrations
{
    public partial class addImageForDelivery_Payment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "PaymentMethod",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "DeliveryMethod",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "PaymentMethod");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "DeliveryMethod");
        }
    }
}
