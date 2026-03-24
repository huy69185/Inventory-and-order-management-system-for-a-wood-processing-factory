using Microsoft.EntityFrameworkCore;
using QuanLyKho.Application.Interfaces;
using QuanLyKho.Domain.Entities;
using QuanLyKho.Infrastructure.Data;

namespace QuanLyKho.Infrastructure.Services;

public class SupplierService : ISupplierService
{
    private readonly AppDbContext _context;

    public SupplierService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Supplier>> GetAllAsync(string? search = null)
    {
        var query = _context.Suppliers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(c =>
                (c.TenNhaCungCap != null && c.TenNhaCungCap.ToLower().Contains(s)) ||
                (c.MaNhaCungCap != null && c.MaNhaCungCap.ToLower().Contains(s)) ||
                (c.DienThoai != null && c.DienThoai.Contains(search)));
        }
        return await query.OrderBy(c => c.TenNhaCungCap).ToListAsync();
    }

    public async Task<Supplier?> GetByIdAsync(int id) => await _context.Suppliers.FindAsync(id);

    public async Task AddAsync(Supplier supplier)
    {
        if (string.IsNullOrWhiteSpace(supplier.MaNhaCungCap))
        {
            var max = await _context.Suppliers.CountAsync();
            supplier.MaNhaCungCap = $"NCC{(max + 1):D4}";
        }
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Supplier supplier)
    {
        var id = supplier.Id;
        if (id <= 0) throw new KeyNotFoundException("Nhà cung cấp không tồn tại");
        var rows = await _context.Suppliers
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.MaNhaCungCap, supplier.MaNhaCungCap)
                .SetProperty(c => c.TenNhaCungCap, supplier.TenNhaCungCap)
                .SetProperty(c => c.DienThoai, supplier.DienThoai ?? "")
                .SetProperty(c => c.DiaChi, supplier.DiaChi ?? "")
                .SetProperty(c => c.Email, supplier.Email ?? "")
                .SetProperty(c => c.GhiChu, supplier.GhiChu ?? ""));
        if (rows == 0) throw new KeyNotFoundException("Nhà cung cấp không tồn tại");
    }

    public async Task DeleteAsync(int id)
    {
        var c = await _context.Suppliers.FindAsync(id);
        if (c == null) throw new KeyNotFoundException("Nhà cung cấp không tồn tại");
        _context.Suppliers.Remove(c);
        await _context.SaveChangesAsync();
    }
}

