using Microsoft.EntityFrameworkCore;
using QuanLyKho.Application.Interfaces;
using QuanLyKho.Domain.Entities;
using QuanLyKho.Infrastructure.Data;

namespace QuanLyKho.Infrastructure.Services;

public class ProductionService : IProductionService
{
    private readonly AppDbContext _context;

    public ProductionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task ProduceForOrderAsync(int salesOrderItemId, int nguyenLieuId, int soLuongNguyenLieu, int soLuongHoanThanh, decimal chiPhiKhac, string? ghiChu = null)
    {
        if (soLuongNguyenLieu <= 0) throw new ArgumentException("Số lượng nguyên liệu phải > 0");
        if (soLuongHoanThanh <= 0) throw new ArgumentException("Số lượng hoàn thành phải > 0");

        var item = await _context.SalesOrderItems
            .Include(i => i.SalesOrder)
            .FirstOrDefaultAsync(i => i.Id == salesOrderItemId)
            ?? throw new KeyNotFoundException("Dòng đơn hàng không tồn tại");

        var nguyenLieu = await _context.Products.FindAsync(nguyenLieuId)
            ?? throw new KeyNotFoundException("Nguyên liệu không tồn tại");

        if (nguyenLieu.SoLuongTon < soLuongNguyenLieu)
            throw new InvalidOperationException("Không đủ tồn kho nguyên liệu để sản xuất.");

        var now = DateTime.Now;

        // Giảm tồn kho nguyên liệu
        nguyenLieu.SoLuongTon -= soLuongNguyenLieu;
        nguyenLieu.NgayCapNhat = now;

        // Cập nhật số lượng đã sản xuất cho dòng đơn
        item.SoLuongDaSanXuat += soLuongHoanThanh;
        if (item.SoLuongDaSanXuat > item.SoLuong)
            item.SoLuongDaSanXuat = item.SoLuong;

        var note = ghiChu ?? $"Sản xuất cho đơn {item.SalesOrder.SoDonHang}";

        // Ghi lịch sử xuất nguyên liệu
        _context.StockTransactions.Add(new StockTransaction
        {
            ProductId = nguyenLieuId,
            LoaiGiaoDich = TransactionType.XuatKho,
            SoLuong = soLuongNguyenLieu,
            Gia = nguyenLieu.GiaNhap,
            GhiChu = note,
            NgayGiaoDich = now
        });

        // Cập nhật trạng thái đơn nếu cần: chỉ chuyển sang Đang sản xuất,
        // điều kiện Hoàn thành sẽ kiểm tra thêm tiền thanh toán ở Service khác.
        var order = item.SalesOrder;
        if (order.TrangThai == SalesOrderStatus.Moi)
        {
            order.TrangThai = SalesOrderStatus.DangSanXuat;
        }

        await _context.SaveChangesAsync();
    }
}

