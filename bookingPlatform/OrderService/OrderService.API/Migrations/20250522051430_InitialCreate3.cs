using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    orderId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<long>(type: "bigint", nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    totalOrderprice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.orderId);
                });

            migrationBuilder.CreateTable(
                name: "orderItems",
                columns: table => new
                {
                    orderItemId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<long>(type: "bigint", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    itemTotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderItems", x => x.orderItemId);
                    table.ForeignKey(
                        name: "FK_orderItems_Order_orderId",
                        column: x => x.orderId,
                        principalTable: "Order",
                        principalColumn: "orderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orderItems_orderId",
                table: "orderItems",
                column: "orderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orderItems");

            migrationBuilder.DropTable(
                name: "Order");
        }
    }
}
