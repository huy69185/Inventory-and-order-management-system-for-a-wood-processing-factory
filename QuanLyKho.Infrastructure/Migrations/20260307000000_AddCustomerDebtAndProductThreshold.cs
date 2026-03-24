using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKho.Infrastructure.Migrations
{
    [Migration("20260307000000_AddCustomerDebtAndProductThreshold")]
    public partial class AddCustomerDebtAndProductThreshold : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaKhachHang = table.Column<string>(type: "TEXT", nullable: false),
                    TenKhachHang = table.Column<string>(type: "TEXT", nullable: false),
                    DienThoai = table.Column<string>(type: "TEXT", nullable: false),
                    DiaChi = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    GhiChu = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Customers", x => x.Id));

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Invoices",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TienDaThu",
                table: "Invoices",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SoLuongCanhBao",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 10);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerId",
                table: "Invoices",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Customers_CustomerId",
                table: "Invoices",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Invoices_Customers_CustomerId", table: "Invoices");
            migrationBuilder.DropIndex(name: "IX_Invoices_CustomerId", table: "Invoices");
            migrationBuilder.DropColumn(name: "CustomerId", table: "Invoices");
            migrationBuilder.DropColumn(name: "TienDaThu", table: "Invoices");
            migrationBuilder.DropColumn(name: "SoLuongCanhBao", table: "Products");
            migrationBuilder.DropTable(name: "Customers");
        }
    }
}
