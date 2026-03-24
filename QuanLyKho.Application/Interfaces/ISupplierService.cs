using QuanLyKho.Domain.Entities;

namespace QuanLyKho.Application.Interfaces;

public interface ISupplierService
{
    Task<List<Supplier>> GetAllAsync(string? search = null);
    Task<Supplier?> GetByIdAsync(int id);
    Task AddAsync(Supplier supplier);
    Task UpdateAsync(Supplier supplier);
    Task DeleteAsync(int id);
}

