using QuanLyKho.Domain.Entities;

namespace QuanLyKho.Application.Interfaces;

public interface ICustomerService
{
    Task<List<Customer>> GetAllAsync(string? search = null);
    Task<Customer?> GetByIdAsync(int id);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
}
