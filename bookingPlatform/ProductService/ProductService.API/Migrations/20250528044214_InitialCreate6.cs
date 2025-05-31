using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductService.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductCategoryEntity_ProductCategoryId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCategoryEntity",
                table: "ProductCategoryEntity");

            migrationBuilder.RenameTable(
                name: "ProductCategoryEntity",
                newName: "productCategory");

            migrationBuilder.AddColumn<long>(
                name: "createdBy",
                table: "Products",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_productCategory",
                table: "productCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_productCategory_ProductCategoryId",
                table: "Products",
                column: "ProductCategoryId",
                principalTable: "productCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_productCategory_ProductCategoryId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_productCategory",
                table: "productCategory");

            migrationBuilder.DropColumn(
                name: "createdBy",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "productCategory",
                newName: "ProductCategoryEntity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCategoryEntity",
                table: "ProductCategoryEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductCategoryEntity_ProductCategoryId",
                table: "Products",
                column: "ProductCategoryId",
                principalTable: "ProductCategoryEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
