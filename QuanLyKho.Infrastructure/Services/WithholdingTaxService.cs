using Microsoft.EntityFrameworkCore;
using QuanLyKho.Application.Interfaces;
using QuanLyKho.Domain.Entities;
using QuanLyKho.Infrastructure.Data;

namespace QuanLyKho.Infrastructure.Services;

public class WithholdingTaxService : IWithholdingTaxService
{
    private readonly AppDbContext _context;

    public WithholdingTaxService(AppDbContext context) => _context = context;

    public async Task<List<WithholdingTax>> GetAllAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var q = _context.WithholdingTaxes.AsNoTracking();
        if (fromDate.HasValue)
            q = q.Where(w => w.NgayChi >= fromDate.Value);
        if (toDate.HasValue)
            q = q.Where(w => w.NgayChi <= toDate.Value);
        return await q.OrderByDescending(w => w.NgayChi).ToListAsync();
    }

    public async Task<WithholdingTax?> GetByIdAsync(int id) =>
        await _context.WithholdingTaxes.FindAsync(id);

    public async Task AddAsync(WithholdingTax entity)
    {
        _context.WithholdingTaxes.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(WithholdingTax entity)
    {
        _context.WithholdingTaxes.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _context.WithholdingTaxes.FindAsync(id);
        if (e != null)
        {
            _context.WithholdingTaxes.Remove(e);
            await _context.SaveChangesAsync();
        }
    }
}
