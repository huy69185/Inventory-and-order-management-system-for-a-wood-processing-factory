using QuanLyKho.Domain.Entities;

namespace QuanLyKho.Application.Interfaces;

public interface IWithholdingTaxService
{
    Task<List<WithholdingTax>> GetAllAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<WithholdingTax?> GetByIdAsync(int id);
    Task AddAsync(WithholdingTax entity);
    Task UpdateAsync(WithholdingTax entity);
    Task DeleteAsync(int id);
}
