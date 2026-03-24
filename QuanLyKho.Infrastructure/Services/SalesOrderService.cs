using Microsoft.EntityFrameworkCore;
using QuanLyKho.Application.Interfaces;
using QuanLyKho.Domain.Entities;
using QuanLyKho.Infrastructure.Data;

namespace QuanLyKho.Infrastructure.Services;

public class SalesOrderService : ISalesOrderService
{
    private readonly AppDbContext _context;

    public SalesOrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SalesOrder>> GetAllAsync()
    {
        return await _context.SalesOrders
            .Include(o => o.Customer)
            .Include(o => o.ChiTiet)
            .OrderByDescending(o => o.NgayDat)
            .ToListAsync();
    }

    public async Task<SalesOrder?> GetByIdAsync(int id)
    {
        return await _context.SalesOrders
            .Include(o => o.Customer)
            .Include(o => o.ChiTiet)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<int> CreateAsync(SalesOrder order, List<SalesOrderItem> items)
    {
        // Sinh số đơn hàng đơn giản: DDyyyyMMdd-###
        var last = await _context.SalesOrders
            .OrderByDescending(o => o.NgayDat)
            .FirstOrDefaultAsync();
        int nextNumber = 1;
        if (last != null && !string.IsNullOrWhiteSpace(last.SoDonHang))
        {
            var tail = last.SoDonHang.Split('-').LastOrDefault();
            if (int.TryParse(tail, out var n)) nextNumber = n + 1;
        }
        order.SoDonHang = $"DD{DateTime.Now:yyyyMMdd}-{nextNumber:D3}";

        // Tính lại tổng và thông tin KH hiển thị
        if (order.CustomerId.HasValue)
        {
            var c = await _context.Customers.FindAsync(order.CustomerId.Value);
            if (c != null)
            {
                order.TenKhachHang = c.TenKhachHang ?? "";
                order.DienThoai = c.DienThoai ?? "";
                if (string.IsNullOrWhiteSpace(order.DiaChiGiaoHang))
                    order.DiaChiGiaoHang = c.DiaChi ?? "";
            }
        }

        _context.SalesOrders.Add(order);
        await _context.SaveChangesAsync();

        foreach (var item in items)
        {
            item.SalesOrderId = order.Id;
            _context.SalesOrderItems.Add(item);
        }
        await _context.SaveChangesAsync();
        return order.Id;
    }

    public async Task UpdateAsync(SalesOrder order, List<SalesOrderItem> items)
    {
        var existing = await _context.SalesOrders
            .Include(o => o.ChiTiet)
            .FirstOrDefaultAsync(o => o.Id == order.Id);
        if (existing == null) throw new KeyNotFoundException("Đơn đặt hàng không tồn tại");

        existing.NgayDat = order.NgayDat;
        existing.CustomerId = order.CustomerId;
        existing.TenKhachHang = order.TenKhachHang;
        existing.DienThoai = order.DienThoai;
        existing.DiaChiGiaoHang = order.DiaChiGiaoHang;
        existing.GhiChu = order.GhiChu;

        // Đồng bộ lại chi tiết: xóa cũ, thêm mới (vì số lượng thường ít)
        _context.SalesOrderItems.RemoveRange(existing.ChiTiet);
        foreach (var item in items)
        {
            item.Id = 0;
            item.SalesOrderId = existing.Id;
            _context.SalesOrderItems.Add(item);
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int id, SalesOrderStatus status)
    {
        if (status == SalesOrderStatus.HoanThanh)
        {
            var order = await _context.SalesOrders
                .Include(o => o.ChiTiet)
                .FirstOrDefaultAsync(o => o.Id == id)
                ?? throw new KeyNotFoundException("Đơn đặt hàng không tồn tại");

            var tong = order.ChiTiet.Sum(i => i.ThanhTien);
            var conLai = tong - order.TienDaTraTruoc;
            if (conLai > 0)
                throw new InvalidOperationException("Không thể hoàn thành đơn hàng khi chưa thanh toán đủ.");

            if (order.ChiTiet.Any(i => i.SoLuongDaSanXuat < i.SoLuong))
                throw new InvalidOperationException("Không thể hoàn thành đơn hàng khi còn dòng chưa sản xuất đủ.");

            order.TrangThai = SalesOrderStatus.HoanThanh;
            await _context.SaveChangesAsync();
        }
        else
        {
            var rows = await _context.SalesOrders
                .Where(o => o.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(o => o.TrangThai, status));
            if (rows == 0) throw new KeyNotFoundException("Đơn đặt hàng không tồn tại");
        }
    }

    public async Task UpdatePaymentAsync(int id, decimal tienDaTraTruoc)
    {
        var value = Math.Max(0, tienDaTraTruoc);
        var rows = await _context.SalesOrders
            .Where(o => o.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.TienDaTraTruoc, value));
        if (rows == 0) throw new KeyNotFoundException("Đơn đặt hàng không tồn tại");
    }

    public async Task DeleteAsync(int id)
    {
        var order = await _context.SalesOrders.FindAsync(id);
        if (order == null) return;
        _context.SalesOrders.Remove(order);
        await _context.SaveChangesAsync();
    }
}

