using Microsoft.EntityFrameworkCore;
using QuanLyKho.Application.Interfaces;
using QuanLyKho.Domain.Entities;
using QuanLyKho.Infrastructure.Data;

namespace QuanLyKho.Infrastructure.Services;

public class StockTransactionService : IStockTransactionService
{
    private readonly AppDbContext _context;

    public StockTransactionService(AppDbContext context)
    {
        _context = context;
    }

    public Task AddTransactionAsync(StockTransaction transaction)
    {
        _context.StockTransactions.Add(transaction);
        return _context.SaveChangesAsync();
    }

    public async Task<List<StockTransaction>> GetAllAsync(DateTime? fromDate = null, DateTime? toDate = null, int? productId = null, TransactionType? loai = null)
    {
        var query = _context.StockTransactions.Include(s => s.Product).AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(s => s.NgayGiaoDich >= fromDate.Value);
        if (toDate.HasValue)
        {
            var end = toDate.Value.Date.AddDays(1);
            query = query.Where(s => s.NgayGiaoDich < end);
        }
        if (productId.HasValue)
            query = query.Where(s => s.ProductId == productId.Value);
        if (loai.HasValue)
            query = query.Where(s => s.LoaiGiaoDich == loai.Value);

        return await query.OrderByDescending(s => s.NgayGiaoDich).ToListAsync();
    }
}
