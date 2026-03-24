using Microsoft.EntityFrameworkCore;
using QuanLyKho.Application.Interfaces;
using QuanLyKho.Domain.Entities;
using QuanLyKho.Infrastructure.Data;

namespace QuanLyKho.Infrastructure.Services;

public class PurchaseInvoiceService : IPurchaseInvoiceService
{
    private readonly AppDbContext _context;

    public PurchaseInvoiceService(AppDbContext context) => _context = context;

    public async Task<List<PurchaseInvoice>> GetAllAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var q = _context.PurchaseInvoices.AsNoTracking();
        if (fromDate.HasValue)
            q = q.Where(p => p.NgayHoaDon >= fromDate.Value);
        if (toDate.HasValue)
            q = q.Where(p => p.NgayHoaDon <= toDate.Value);
        return await q.OrderByDescending(p => p.NgayHoaDon).ToListAsync();
    }

    public async Task<PurchaseInvoice?> GetByIdAsync(int id) =>
        await _context.PurchaseInvoices.FindAsync(id);

    public async Task AddAsync(PurchaseInvoice entity)
    {
        _context.PurchaseInvoices.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PurchaseInvoice entity)
    {
        _context.PurchaseInvoices.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _context.PurchaseInvoices.FindAsync(id);
        if (e != null)
        {
            _context.PurchaseInvoices.Remove(e);
            await _context.SaveChangesAsync();
        }
    }
}
