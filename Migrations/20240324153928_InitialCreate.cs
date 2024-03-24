using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "character varying", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("categories_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "character varying", nullable: true),
                    name = table.Column<string>(type: "character varying", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Products_pkey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "barcodes",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying", nullable: false),
                    product_id = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Barcode_pkey", x => x.key);
                    table.ForeignKey(
                        name: "Barcode_ProductId_fkey",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "products_categories",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "INTEGER", nullable: false),
                    category_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("products_categories_pkey", x => new { x.product_id, x.category_id });
                    table.ForeignKey(
                        name: "products_categories_category_id_fkey",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "products_categories_product_id_fkey",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_barcodes_product_id",
                table: "barcodes",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_categories_category_id",
                table: "products_categories",
                column: "category_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "barcodes");

            migrationBuilder.DropTable(
                name: "products_categories");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "products");
        }
    }
}
