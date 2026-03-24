using Microsoft.EntityFrameworkCore;
using QuanLyKho.Domain.Entities;
using QuanLyKho.Infrastructure.Services;
using QuanLyKho.Tests.TestUtils;

namespace QuanLyKho.Tests;

public class CompanySettingServiceTests
{
    [Fact]
    public async Task GetAsync_WhenNoSetting_ReturnsDefaultId1()
    {
        await using var context = DbContextFactory.Create();
        var sut = new CompanySettingService(context);

        var setting = await sut.GetAsync();

        Assert.Equal(1, setting.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenNoExisting_InsertsWithId1()
    {
        await using var context = DbContextFactory.Create();
        var sut = new CompanySettingService(context);
        var input = new CompanySetting
        {
            TenCongTy = "Cty",
            DiaChi = "Addr",
            DienThoai = "Phone",
            Email = "Email",
            MaSoThue = "MST",
            LogoPath = "Logo"
        };

        await sut.UpdateAsync(input);

        var saved = await context.CompanySettings.SingleAsync();
        Assert.Equal(1, saved.Id);
        Assert.Equal("Cty", saved.TenCongTy);
        Assert.Equal("Addr", saved.DiaChi);
    }

    [Fact]
    public async Task UpdateAsync_WhenExisting_UpdatesProperties()
    {
        await using var context = DbContextFactory.Create();
        context.CompanySettings.Add(new CompanySetting { Id = 1, TenCongTy = "Old", DiaChi = "OldAddr" });
        await context.SaveChangesAsync();

        var sut = new CompanySettingService(context);
        var input = new CompanySetting { TenCongTy = "New", DiaChi = "NewAddr", DienThoai = "1", Email = "2", MaSoThue = "3", LogoPath = "4" };

        await sut.UpdateAsync(input);

        var saved = await context.CompanySettings.SingleAsync();
        Assert.Equal(1, saved.Id);
        Assert.Equal("New", saved.TenCongTy);
        Assert.Equal("NewAddr", saved.DiaChi);
    }
}

