using QuanLyKho.Domain.Entities;

namespace QuanLyKho.Application.Interfaces;

public interface IStockTransactionService
{
    Task AddTransactionAsync(StockTransaction transaction);
    Task<List<StockTransaction>> GetAllAsync(DateTime? fromDate = null, DateTime? toDate = null, int? productId = null, TransactionType? loai = null);
}
