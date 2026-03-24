using QuanLyKho.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKho.Application.Interfaces
{
    public interface ICompanySettingService
    {
        Task<CompanySetting> GetAsync();
        Task UpdateAsync(CompanySetting setting);
    }
}
