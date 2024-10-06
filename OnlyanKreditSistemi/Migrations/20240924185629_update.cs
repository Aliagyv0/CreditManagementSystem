using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyanKreditSistemi.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "BranchProduct");

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "BranchProduct",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
