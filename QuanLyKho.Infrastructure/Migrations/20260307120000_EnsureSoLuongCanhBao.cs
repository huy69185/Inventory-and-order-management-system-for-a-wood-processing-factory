using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Data.Sqlite;

#nullable disable

namespace QuanLyKho.Infrastructure.Migrations
{
    /// <summary>
    /// Đảm bảo cột SoLuongCanhBao tồn tại trên bảng Products (an toàn khi chạy nhiều lần).
    /// </summary>
    [Migration("20260307120000_EnsureSoLuongCanhBao")]
    public partial class EnsureSoLuongCanhBao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            try
            {
                migrationBuilder.Sql("ALTER TABLE Products ADD COLUMN SoLuongCanhBao INTEGER NOT NULL DEFAULT 10;");
            }
            catch (SqliteException ex) when (ex.Message.Contains("duplicate column", StringComparison.OrdinalIgnoreCase))
            {
                // Cột đã tồn tại (từ migration trước) -> bỏ qua
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // SQLite không hỗ trợ DROP COLUMN đơn giản trước 3.35; không làm gì để tránh mất dữ liệu
        }
    }
}
