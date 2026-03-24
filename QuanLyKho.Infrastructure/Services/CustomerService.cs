using Microsoft.EntityFrameworkCore;
using QuanLyKho.Application.Interfaces;
using QuanLyKho.Domain.Entities;
using QuanLyKho.Infrastructure.Data;

namespace QuanLyKho.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetAllAsync(string? search = null)
    {
        var query = _context.Customers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(c =>
                (c.TenKhachHang != null && c.TenKhachHang.ToLower().Contains(s)) ||
                (c.MaKhachHang != null && c.MaKhachHang.ToLower().Contains(s)) ||
                (c.DienThoai != null && c.DienThoai.Contains(search)));
        }
        return await query.OrderBy(c => c.TenKhachHang).ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id) => await _context.Customers.FindAsync(id);

    public async Task AddAsync(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.MaKhachHang))
        {
            var max = await _context.Customers.CountAsync();
            customer.MaKhachHang = $"KH{(max + 1):D4}";
        }
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        var id = customer.Id;
        if (id <= 0) throw new KeyNotFoundException("Khách hàng không tồn tại");
        var rows = await _context.Customers
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.MaKhachHang, customer.MaKhachHang)
                .SetProperty(c => c.TenKhachHang, customer.TenKhachHang)
                .SetProperty(c => c.DienThoai, customer.DienThoai ?? "")
                .SetProperty(c => c.DiaChi, customer.DiaChi ?? "")
                .SetProperty(c => c.Email, customer.Email ?? "")
                .SetProperty(c => c.GhiChu, customer.GhiChu ?? ""));
        if (rows == 0) throw new KeyNotFoundException("Khách hàng không tồn tại");
    }

    public async Task DeleteAsync(int id)
    {
        var c = await _context.Customers.FindAsync(id);
        if (c == null) throw new KeyNotFoundException("Khách hàng không tồn tại");
        _context.Customers.Remove(c);
        await _context.SaveChangesAsync();
    }
}
