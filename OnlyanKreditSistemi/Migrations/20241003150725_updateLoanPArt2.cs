using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyanKreditSistemi.Migrations
{
    /// <inheritdoc />
    public partial class updateLoanPArt2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Mounths",
                table: "LoanDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mounths",
                table: "LoanDetails");
        }
    }
}
