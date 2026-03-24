using QuanLyKho.Domain.Entities;

namespace QuanLyKho.Application.Interfaces;

public interface ISalesOrderService
{
    Task<List<SalesOrder>> GetAllAsync();
    Task<SalesOrder?> GetByIdAsync(int id);
    Task<int> CreateAsync(SalesOrder order, List<SalesOrderItem> items);
    Task UpdateAsync(SalesOrder order, List<SalesOrderItem> items);
    Task UpdateStatusAsync(int id, SalesOrderStatus status);
    Task UpdatePaymentAsync(int id, decimal tienDaTraTruoc);
    Task DeleteAsync(int id);
}

