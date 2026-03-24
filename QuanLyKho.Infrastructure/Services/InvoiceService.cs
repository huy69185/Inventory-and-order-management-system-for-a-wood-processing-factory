using Microsoft.EntityFrameworkCore;
using QuanLyKho.Application.Interfaces;
using QuanLyKho.Domain.Entities;
using QuanLyKho.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKho.Infrastructure.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly AppDbContext _context;

        public InvoiceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .AsNoTracking()
                .Include(i => i.ChiTiet)
                .Include(i => i.Customer)
                .OrderByDescending(i => i.NgayLap)
                .ToListAsync();
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _context.Invoices
                .AsNoTracking()
                .Include(i => i.ChiTiet)
                .Include(i => i.Customer)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<int> CreateAsync(Invoice invoice, List<InvoiceDetail> details)
        {
            // Sinh số hóa đơn tự động (HD + ngày + số thứ tự)
            var lastInvoice = await _context.Invoices
                .OrderByDescending(i => i.NgayLap)
                .FirstOrDefaultAsync();
            int nextNumber = lastInvoice != null ? int.Parse(lastInvoice.SoHoaDon.Split('-').LastOrDefault() ?? "0") + 1 : 1;
            invoice.SoHoaDon = $"HD{DateTime.Now:yyyyMMdd}-{nextNumber:D3}";

            // Tính tổng tiền
            invoice.TongTienTruocCK = details.Sum(d => d.ThanhTien);
            invoice.TongTienSauCK = invoice.TongTienTruocCK * (1 - invoice.ChietKhau / 100);

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync(); // Lưu để có ID

            // Bước 1: Gán InvoiceId, thêm chi tiết và trừ tồn kho (chỉ INSERT InvoiceDetails + UPDATE Products)
            foreach (var detail in details)
            {
                if (detail.ProductId <= 0)
                    throw new Exception($"Dòng \"{detail.TenSanPham}\" chưa chọn sản phẩm hợp lệ.");
                detail.InvoiceId = invoice.Id;
                _context.InvoiceDetails.Add(detail);

                var product = await _context.Products.FindAsync(detail.ProductId);
                if (product == null)
                    throw new Exception($"Sản phẩm {detail.TenSanPham} không tồn tại!");
                if (product.SoLuongTon < detail.SoLuong)
                    throw new Exception($"Sản phẩm {detail.TenSanPham} không đủ tồn kho! Còn {product.SoLuongTon}");

                product.SoLuongTon -= detail.SoLuong;
                product.NgayCapNhat = DateTime.Now;
            }
            await _context.SaveChangesAsync();

            // Bước 2: Ghi lịch sử xuất kho (INSERT StockTransactions sau khi Products đã cập nhật — tránh lỗi FK SQLite)
            foreach (var detail in details)
            {
                if (detail.ProductId <= 0) continue;
                _context.StockTransactions.Add(new StockTransaction
                {
                    ProductId = detail.ProductId,
                    LoaiGiaoDich = TransactionType.XuatKho,
                    SoLuong = detail.SoLuong,
                    Gia = detail.DonGia,
                    GhiChu = $"Xuất kho hóa đơn {invoice.SoHoaDon}",
                    NgayGiaoDich = DateTime.Now
                });
            }
            await _context.SaveChangesAsync();
            return invoice.Id;
        }

        public async Task UpdatePaymentAsync(int invoiceId, decimal tienDaThu)
        {
            var value = Math.Max(0, tienDaThu);
            await _context.Invoices
                .Where(i => i.Id == invoiceId)
                .ExecuteUpdateAsync(s => s.SetProperty(i => i.TienDaThu, value));
        }

        public async Task DeleteAsync(int id)
        {
            var invoice = await _context.Invoices.AsNoTracking().Include(i => i.ChiTiet).FirstOrDefaultAsync(i => i.Id == id);
            if (invoice == null) return;

            // Bước 1: Hoàn tồn kho và ghi lịch sử nhập kho (INSERT + UPDATE) — lưu trước để tránh lỗi FK khi xóa
            foreach (var detail in invoice.ChiTiet)
            {
                if (detail.ProductId <= 0) continue;
                var product = await _context.Products.FindAsync(detail.ProductId);
                if (product != null)
                {
                    product.SoLuongTon += detail.SoLuong;
                    product.NgayCapNhat = DateTime.Now;
                }
                _context.StockTransactions.Add(new StockTransaction
                {
                    ProductId = detail.ProductId,
                    LoaiGiaoDich = TransactionType.NhapKho,
                    SoLuong = detail.SoLuong,
                    Gia = detail.DonGia,
                    GhiChu = $"Hoàn hàng do hủy hóa đơn {invoice.SoHoaDon}",
                    NgayGiaoDich = DateTime.Now
                });
            }
            await _context.SaveChangesAsync();

            // Bước 2: Xóa chi tiết rồi xóa hóa đơn (tránh lỗi FK trên SQLite)
            await _context.InvoiceDetails.Where(d => d.InvoiceId == id).ExecuteDeleteAsync();
            await _context.Invoices.Where(i => i.Id == id).ExecuteDeleteAsync();
        }
    }
}
