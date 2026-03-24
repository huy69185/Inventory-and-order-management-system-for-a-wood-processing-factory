using QuanLyKho.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKho.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<List<Invoice>> GetAllAsync();
        Task<Invoice?> GetByIdAsync(int id);
        Task<int> CreateAsync(Invoice invoice, List<InvoiceDetail> details);
        Task UpdatePaymentAsync(int invoiceId, decimal tienDaThu);
        Task DeleteAsync(int id);
    }
}
