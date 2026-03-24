using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKho.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateClean1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoiceId1",
                table: "InvoiceDetails");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetails_InvoiceId1",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "InvoiceId1",
                table: "InvoiceDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvoiceId1",
                table: "InvoiceDetails",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoiceId1",
                table: "InvoiceDetails",
                column: "InvoiceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoiceId1",
                table: "InvoiceDetails",
                column: "InvoiceId1",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
