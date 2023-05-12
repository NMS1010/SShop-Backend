using Microsoft.EntityFrameworkCore.Migrations;

namespace SShop.Domain.Migrations
{
    public partial class updateReviewRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReviewItemId",
                table: "OrderItem",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_ReviewItemId",
                table: "OrderItem",
                column: "ReviewItemId",
                unique: true,
                filter: "[ReviewItemId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Review_ReviewItemId",
                table: "OrderItem",
                column: "ReviewItemId",
                principalTable: "Review",
                principalColumn: "ReviewItemId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Review_ReviewItemId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_ReviewItemId",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "ReviewItemId",
                table: "OrderItem");
        }
    }
}
