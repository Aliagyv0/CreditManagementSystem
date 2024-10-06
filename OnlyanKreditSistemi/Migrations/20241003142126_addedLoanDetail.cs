using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyanKreditSistemi.Migrations
{
    /// <inheritdoc />
    public partial class addedLoanDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentAmount",
                table: "Loans");

            migrationBuilder.CreateTable(
                name: "LoanDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentAmount = table.Column<double>(type: "float", nullable: false),
                    LoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanDetails_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanDetails_LoanId",
                table: "LoanDetails",
                column: "LoanId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanDetails");

            migrationBuilder.AddColumn<double>(
                name: "CurrentAmount",
                table: "Loans",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
