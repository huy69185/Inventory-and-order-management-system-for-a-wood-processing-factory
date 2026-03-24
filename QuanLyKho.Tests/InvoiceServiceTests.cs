using Microsoft.EntityFrameworkCore;
using QuanLyKho.Domain.Entities;
using QuanLyKho.Infrastructure.Services;
using QuanLyKho.Tests.TestUtils;

namespace QuanLyKho.Tests;

public class InvoiceServiceTests
{
    [Fact]
    public async Task GetAllAsync_IncludesChiTiet_AndOrdersByNgayLapDesc()
    {
        await using var context = DbContextFactory.Create();

        var older = new Invoice { NgayLap = DateTime.Now.AddDays(-1), KhachHang = "Old" };
        var newer = new Invoice { NgayLap = DateTime.Now, KhachHang = "New" };
        context.Invoices.AddRange(older, newer);
        await context.SaveChangesAsync();

        context.InvoiceDetails.Add(new InvoiceDetail
        {
            InvoiceId = newer.Id,
            ProductId = 1,
            TenSanPham = "X",
            SoLuong = 1,
            DonGia = 10
        });
        await context.SaveChangesAsync();

        var sut = new InvoiceService(context);

        var result = await sut.GetAllAsync();

        Assert.Equal(new[] { "New", "Old" }, result.Select(i => i.KhachHang).ToArray());
        Assert.True(result[0].ChiTiet.Count > 0);
    }

    [Fact]
    public async Task GetByIdAsync_IncludesChiTiet()
    {
        await using var context = DbContextFactory.Create();
        var invoice = new Invoice { KhachHang = "KH" };
        context.Invoices.Add(invoice);
        await context.SaveChangesAsync();

        context.InvoiceDetails.Add(new InvoiceDetail
        {
            InvoiceId = invoice.Id,
            ProductId = 1,
            TenSanPham = "SP",
            SoLuong = 2,
            DonGia = 5
        });
        await context.SaveChangesAsync();

        var sut = new InvoiceService(context);

        var loaded = await sut.GetByIdAsync(invoice.Id);

        Assert.NotNull(loaded);
        Assert.Single(loaded!.ChiTiet);
    }

    [Fact]
    public async Task CreateAsync_SetsSoHoaDonAndTotals_ReducesStock_AndCreatesStockTransaction()
    {
        await using var context = DbContextFactory.Create();
        var product = new Product { MaSanPham = "P01", TenSanPham = "Prod", SoLuongTon = 10, GiaBan = 100 };
        context.Products.Add(product);

        // Existing invoice to drive next number
        context.Invoices.Add(new Invoice { SoHoaDon = "HD20260227-005", NgayLap = DateTime.Now.AddMinutes(-1) });
        await context.SaveChangesAsync();

        var sut = new InvoiceService(context);
        var invoice = new Invoice { KhachHang = "A", ChietKhau = 10 };
        var details = new List<InvoiceDetail>
        {
            new()
            {
                ProductId = product.Id,
                TenSanPham = product.TenSanPham,
                SoLuong = 2,
                DonGia = 50
            }
        };

        var id = await sut.CreateAsync(invoice, details);

        Assert.True(id > 0);
        Assert.StartsWith("HD", invoice.SoHoaDon);
        Assert.EndsWith("-006", invoice.SoHoaDon);

        Assert.Equal(100, invoice.TongTienTruocCK); // 2 * 50
        Assert.Equal(90, invoice.TongTienSauCK);    // 10% off

        var savedProduct = await context.Products.SingleAsync(p => p.Id == product.Id);
        Assert.Equal(8, savedProduct.SoLuongTon);

        var tx = await context.StockTransactions.SingleAsync(t => t.LoaiGiaoDich == TransactionType.XuatKho);
        Assert.Equal(product.Id, tx.ProductId);
        Assert.Equal(2, tx.SoLuong);
        Assert.Equal(50, tx.Gia);
        Assert.Contains(invoice.SoHoaDon, tx.GhiChu);
    }

    [Fact]
    public async Task CreateAsync_WhenProductMissing_Throws()
    {
        await using var context = DbContextFactory.Create();
        var sut = new InvoiceService(context);

        var invoice = new Invoice { KhachHang = "A" };
        var details = new List<InvoiceDetail>
        {
            new()
            {
                ProductId = 999,
                TenSanPham = "Missing",
                SoLuong = 1,
                DonGia = 1
            }
        };

        var ex = await Assert.ThrowsAsync<Exception>(() => sut.CreateAsync(invoice, details));
        Assert.Contains("không tồn tại", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_WhenInsufficientStock_Throws_AndDoesNotChangeStock()
    {
        await using var context = DbContextFactory.Create();
        var product = new Product { MaSanPham = "P01", TenSanPham = "Prod", SoLuongTon = 1 };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var sut = new InvoiceService(context);
        var invoice = new Invoice { KhachHang = "A" };
        var details = new List<InvoiceDetail>
        {
            new()
            {
                ProductId = product.Id,
                TenSanPham = product.TenSanPham,
                SoLuong = 2,
                DonGia = 10
            }
        };

        var ex = await Assert.ThrowsAsync<Exception>(() => sut.CreateAsync(invoice, details));

        Assert.Contains("không đủ tồn kho", ex.Message);
        Assert.Equal(1, (await context.Products.SingleAsync(p => p.Id == product.Id)).SoLuongTon);
    }

    [Fact]
    public async Task DeleteAsync_WhenInvoiceExists_RemovesIt()
    {
        await using var context = DbContextFactory.Create();
        var invoice = new Invoice { KhachHang = "Del" };
        context.Invoices.Add(invoice);
        await context.SaveChangesAsync();

        var sut = new InvoiceService(context);

        await sut.DeleteAsync(invoice.Id);

        Assert.Empty(context.Invoices);
    }
}

