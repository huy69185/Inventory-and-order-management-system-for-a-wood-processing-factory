using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKho.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanySettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenCongTy = table.Column<string>(type: "TEXT", nullable: false),
                    DiaChi = table.Column<string>(type: "TEXT", nullable: false),
                    DienThoai = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    MaSoThue = table.Column<string>(type: "TEXT", nullable: false),
                    LogoPath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SoHoaDon = table.Column<string>(type: "TEXT", nullable: false),
                    NgayLap = table.Column<DateTime>(type: "TEXT", nullable: false),
                    KhachHang = table.Column<string>(type: "TEXT", nullable: false),
                    DienThoaiKH = table.Column<string>(type: "TEXT", nullable: false),
                    ChietKhau = table.Column<decimal>(type: "TEXT", nullable: false),
                    TongTienTruocCK = table.Column<decimal>(type: "TEXT", nullable: false),
                    TongTienSauCK = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaSanPham = table.Column<string>(type: "TEXT", nullable: false),
                    TenSanPham = table.Column<string>(type: "TEXT", nullable: false),
                    DonVi = table.Column<string>(type: "TEXT", nullable: false),
                    SoLuongTon = table.Column<int>(type: "INTEGER", nullable: false),
                    GiaNhap = table.Column<decimal>(type: "TEXT", nullable: false),
                    GiaBan = table.Column<decimal>(type: "TEXT", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    InvoiceId1 = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    TenSanPham = table.Column<string>(type: "TEXT", nullable: false),
                    SoLuong = table.Column<int>(type: "INTEGER", nullable: false),
                    DonGia = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_Invoices_InvoiceId1",
                        column: x => x.InvoiceId1,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    LoaiGiaoDich = table.Column<int>(type: "INTEGER", nullable: false),
                    SoLuong = table.Column<int>(type: "INTEGER", nullable: false),
                    Gia = table.Column<decimal>(type: "TEXT", nullable: false),
                    GhiChu = table.Column<string>(type: "TEXT", nullable: false),
                    NgayGiaoDich = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTransactions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CompanySettings",
                columns: new[] { "Id", "DiaChi", "DienThoai", "Email", "LogoPath", "MaSoThue", "TenCongTy" },
                values: new object[] { 1, "", "", "", "", "", "" });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoiceId",
                table: "InvoiceDetails",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoiceId1",
                table: "InvoiceDetails",
                column: "InvoiceId1");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_ProductId",
                table: "StockTransactions",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanySettings");

            migrationBuilder.DropTable(
                name: "InvoiceDetails");

            migrationBuilder.DropTable(
                name: "StockTransactions");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
