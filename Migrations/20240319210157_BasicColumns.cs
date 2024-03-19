using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class BasicColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "BarcodeBytes",
                table: "Barcodes",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BarcodeNumber",
                table: "Barcodes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Barcodes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarcodeBytes",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "BarcodeNumber",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Barcodes");
        }
    }
}
