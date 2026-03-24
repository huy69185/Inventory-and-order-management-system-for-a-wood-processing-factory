using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKho.Infrastructure.Migrations
{
    [Migration("20260307100000_AddTaxEntities")]
    public partial class AddTaxEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseInvoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SoHoaDon = table.Column<string>(type: "TEXT", nullable: false),
                    NgayHoaDon = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NhaCungCap = table.Column<string>(type: "TEXT", nullable: false),
                    TienHangChuaThue = table.Column<decimal>(type: "TEXT", nullable: false),
                    ThueGTGT = table.Column<decimal>(type: "TEXT", nullable: false),
                    TienThueGTGT = table.Column<decimal>(type: "TEXT", nullable: false),
                    GhiChu = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_PurchaseInvoices", x => x.Id));

            migrationBuilder.CreateTable(
                name: "WithholdingTaxes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NgayChi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NguoiNhan = table.Column<string>(type: "TEXT", nullable: false),
                    MaSoThue = table.Column<string>(type: "TEXT", nullable: false),
                    ThuNhapChiuThue = table.Column<decimal>(type: "TEXT", nullable: false),
                    ThueKhauTru = table.Column<decimal>(type: "TEXT", nullable: false),
                    GhiChu = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_WithholdingTaxes", x => x.Id));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PurchaseInvoices");
            migrationBuilder.DropTable(name: "WithholdingTaxes");
        }
    }
}
