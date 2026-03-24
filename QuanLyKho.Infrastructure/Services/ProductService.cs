using Microsoft.EntityFrameworkCore;
using QuanLyKho.Application.Interfaces;
using QuanLyKho.Domain.Entities;
using QuanLyKho.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyKho.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Product>> GetAllAsync(string? search = null)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(p =>
                    p.TenSanPham.ToLower().Contains(search) ||
                    p.MaSanPham.ToLower().Contains(search));
            }

            return await query
                .OrderBy(p => p.TenSanPham)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task AddAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            product.NgayCapNhat = DateTime.Now;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            var id = product.Id;
            if (id <= 0) throw new KeyNotFoundException("Sản phẩm không tồn tại");

            var rows = await _context.Products
                .Where(p => p.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.MaSanPham, product.MaSanPham)
                    .SetProperty(p => p.TenSanPham, product.TenSanPham)
                    .SetProperty(p => p.DonVi, product.DonVi)
                    .SetProperty(p => p.GiaNhap, product.GiaNhap)
                    .SetProperty(p => p.GiaBan, product.GiaBan)
                    .SetProperty(p => p.SoLuongCanhBao, product.SoLuongCanhBao)
                    .SetProperty(p => p.NgayCapNhat, DateTime.Now));
            if (rows == 0) throw new KeyNotFoundException("Sản phẩm không tồn tại");
        }

        public async Task DeleteAsync(int id)
        {
            var usedInInvoice = await _context.InvoiceDetails.AnyAsync(d => d.ProductId == id);
            if (usedInInvoice)
                throw new InvalidOperationException("Không thể xóa sản phẩm đã có trong hóa đơn. Hãy xóa hoặc sửa các hóa đơn liên quan trước.");

            var rows = await _context.Products.Where(p => p.Id == id).ExecuteDeleteAsync();
            if (rows == 0) throw new KeyNotFoundException("Sản phẩm không tồn tại");
        }

        public async Task<List<Product>> GetLowStockAsync(int threshold = 10)
        {
            return await _context.Products
                .Where(p => p.SoLuongTon >= 0 && p.SoLuongTon < (p.SoLuongCanhBao > 0 ? p.SoLuongCanhBao : threshold))
                .OrderBy(p => p.SoLuongTon)
                .ToListAsync();
        }

        public async Task NhapKhoAsync(int productId, int soLuongNhap, decimal giaNhapMoi, string? ghiChu = null)
        {
            if (soLuongNhap <= 0) throw new ArgumentException("Số lượng nhập phải lớn hơn 0");
            if (giaNhapMoi < 0) throw new ArgumentException("Giá nhập không được âm");

            var product = await GetByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException("Sản phẩm không tồn tại");

            // Cập nhật tồn kho và giá nhập mới nhất
            product.SoLuongTon += soLuongNhap;
            product.GiaNhap = giaNhapMoi;
            product.NgayCapNhat = DateTime.Now;

            // Lưu lịch sử giao dịch nhập kho
            var transaction = new StockTransaction
            {
                ProductId = productId,
                LoaiGiaoDich = TransactionType.NhapKho,
                SoLuong = soLuongNhap,
                Gia = giaNhapMoi,
                GhiChu = ghiChu ?? "Nhập kho thủ công",
                NgayGiaoDich = DateTime.Now
            };

            _context.StockTransactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task DieuChinhTonKhoAsync(int productId, int tonThucTe, string? lyDo = null)
        {
            var product = await GetByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException("Sản phẩm không tồn tại");
            if (tonThucTe < 0) throw new ArgumentException("Tồn thực tế không được âm");

            var chenhlech = tonThucTe - product.SoLuongTon;
            if (chenhlech == 0) return;

            var now = DateTime.Now;
            if (chenhlech > 0)
            {
                product.SoLuongTon += chenhlech;
                product.NgayCapNhat = now;
                _context.StockTransactions.Add(new StockTransaction
                {
                    ProductId = productId,
                    LoaiGiaoDich = TransactionType.NhapKho,
                    SoLuong = chenhlech,
                    Gia = product.GiaNhap,
                    GhiChu = lyDo ?? "Điều chỉnh tăng tồn kho theo kiểm kê",
                    NgayGiaoDich = now
                });
            }
            else
            {
                var giam = -chenhlech;
                if (giam > product.SoLuongTon)
                    throw new InvalidOperationException("Không thể điều chỉnh âm vượt quá tồn kho hiện tại.");
                product.SoLuongTon -= giam;
                product.NgayCapNhat = now;
                _context.StockTransactions.Add(new StockTransaction
                {
                    ProductId = productId,
                    LoaiGiaoDich = TransactionType.XuatKho,
                    SoLuong = giam,
                    Gia = product.GiaNhap,
                    GhiChu = lyDo ?? "Điều chỉnh giảm tồn kho theo kiểm kê",
                    NgayGiaoDich = now
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task XuatHuyAsync(int productId, int soLuong, string? lyDo = null)
        {
            if (soLuong <= 0) throw new ArgumentException("Số lượng phải lớn hơn 0");
            var product = await GetByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException("Sản phẩm không tồn tại");
            if (product.SoLuongTon < soLuong)
                throw new InvalidOperationException("Không đủ tồn kho để xuất hủy.");

            var now = DateTime.Now;
            product.SoLuongTon -= soLuong;
            product.NgayCapNhat = now;

            _context.StockTransactions.Add(new StockTransaction
            {
                ProductId = productId,
                LoaiGiaoDich = TransactionType.XuatKho,
                SoLuong = soLuong,
                Gia = product.GiaNhap,
                GhiChu = lyDo ?? "Xuất hủy / hàng hư hỏng",
                NgayGiaoDich = now
            });

            await _context.SaveChangesAsync();
        }
    }
}