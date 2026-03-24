using QuanLyKho.Domain.Entities;

namespace QuanLyKho.Application.Interfaces;

public interface IPurchaseInvoiceService
{
    Task<List<PurchaseInvoice>> GetAllAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<PurchaseInvoice?> GetByIdAsync(int id);
    Task AddAsync(PurchaseInvoice entity);
    Task UpdateAsync(PurchaseInvoice entity);
    Task DeleteAsync(int id);
}
