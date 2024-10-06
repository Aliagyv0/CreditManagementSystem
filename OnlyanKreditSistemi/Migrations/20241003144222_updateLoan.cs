using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyanKreditSistemi.Migrations
{
    /// <inheritdoc />
    public partial class updateLoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanItems_BranchProduct_BranchProductId",
                table: "LoanItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Employees_EmployeeId",
                table: "Loans");

            migrationBuilder.DropTable(
                name: "BranchProduct");

            migrationBuilder.RenameColumn(
                name: "BranchProductId",
                table: "LoanItems",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_LoanItems_BranchProductId",
                table: "LoanItems",
                newName: "IX_LoanItems_ProductId");

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "Loans",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BranchId",
                table: "Products",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanItems_Products_ProductId",
                table: "LoanItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Employees_EmployeeId",
                table: "Loans",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Branches_BranchId",
                table: "Products",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanItems_Products_ProductId",
                table: "LoanItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Employees_EmployeeId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Branches_BranchId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_BranchId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "LoanItems",
                newName: "BranchProductId");

            migrationBuilder.RenameIndex(
                name: "IX_LoanItems_ProductId",
                table: "LoanItems",
                newName: "IX_LoanItems_BranchProductId");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "Loans",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "BranchProduct",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchProduct_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BranchProduct_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchProduct_BranchId",
                table: "BranchProduct",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchProduct_ProductId",
                table: "BranchProduct",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanItems_BranchProduct_BranchProductId",
                table: "LoanItems",
                column: "BranchProductId",
                principalTable: "BranchProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Employees_EmployeeId",
                table: "Loans",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
