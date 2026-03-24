using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKho.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddThueGTGTToInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ThueGTGT",
                table: "Invoices",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThueGTGT",
                table: "Invoices");
        }
    }
}
