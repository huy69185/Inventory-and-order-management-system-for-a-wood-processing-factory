using Microsoft.EntityFrameworkCore;
using QuanLyKho.Application.Interfaces;
using QuanLyKho.Domain.Entities;
using QuanLyKho.Infrastructure.Data;

namespace QuanLyKho.Infrastructure.Services;

public class CompanySettingService : ICompanySettingService
{
    private readonly AppDbContext _context;

    public CompanySettingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CompanySetting> GetAsync()
    {
        var setting = await _context.CompanySettings
            .AsNoTracking() // Tránh tracking conflict
            .FirstOrDefaultAsync();

        return setting ?? new CompanySetting { Id = 1 };
    }

    public async Task UpdateAsync(CompanySetting setting)
    {
        var existing = await _context.CompanySettings.FirstOrDefaultAsync();

        if (existing == null)
        {
            // Đảm bảo Id = 1 khi insert lần đầu
            setting.Id = 1;
            _context.CompanySettings.Add(setting);
        }
        else
        {
            // Cách 1: Update từng property (AN TOÀN nhất)
            existing.TenCongTy = setting.TenCongTy;
            existing.DiaChi = setting.DiaChi;
            existing.DienThoai = setting.DienThoai;
            existing.Email = setting.Email;
            existing.MaSoThue = setting.MaSoThue;
            existing.LogoPath = setting.LogoPath;
            existing.TyLeThueGTGTTrucTiep = setting.TyLeThueGTGTTrucTiep;
            existing.ThueGTGTMacDinh = setting.ThueGTGTMacDinh;
            existing.TyLeThuNhapUocTinh = setting.TyLeThuNhapUocTinh;

            _context.CompanySettings.Update(existing);
        }

        await _context.SaveChangesAsync();
    }
}