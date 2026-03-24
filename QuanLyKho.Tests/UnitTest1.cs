using Microsoft.EntityFrameworkCore;
using QuanLyKho.Domain.Entities;
using QuanLyKho.Infrastructure.Services;
using QuanLyKho.Tests.TestUtils;

namespace QuanLyKho.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task GetAllAsync_WhenSearchIsNull_ReturnsOrderedByTenSanPham()
    {
        await using var context = DbContextFactory.Create();
        context.Products.AddRange(
            new Product { MaSanPham = "B02", TenSanPham = "B" },
            new Product { MaSanPham = "A01", TenSanPham = "A" });
        await context.SaveChangesAsync();

        var sut = new ProductService(context);

        var result = await sut.GetAllAsync();

        Assert.Equal(new[] { "A", "B" }, result.Select(p => p.TenSanPham).ToArray());
    }

    [Theory]
    [InlineData("a", 3)]
    [InlineData(" A01 ", 1)]
    [InlineData("zzz", 0)]
    public async Task GetAllAsync_WhenSearchProvided_FiltersByTenSanPhamOrMaSanPham_CaseInsensitive(string search, int expectedCount)
    {
        await using var context = DbContextFactory.Create();
        context.Products.AddRange(
            new Product { MaSanPham = "A01", TenSanPham = "Apple" },
            new Product { MaSanPham = "A02", TenSanPham = "Avocado" },
            new Product { MaSanPham = "B01", TenSanPham = "Banana" });
        await context.SaveChangesAsync();

        var sut = new ProductService(context);

        var result = await sut.GetAllAsync(search);

        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    public async Task AddAsync_SetsNgayCapNhat_AndPersistsProduct()
    {
        await using var context = DbContextFactory.Create();
        var sut = new ProductService(context);
        var product = new Product { MaSanPham = "P01", TenSanPham = "Test", SoLuongTon = 5 };

        await sut.AddAsync(product);

        var saved = await context.Products.SingleAsync();
        Assert.Equal("P01", saved.MaSanPham);
        Assert.True(saved.NgayCapNhat > DateTime.MinValue);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductNotFound_ThrowsKeyNotFoundException()
    {
        await using var context = DbContextFactory.Create();
        var sut = new ProductService(context);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            sut.UpdateAsync(new Product { Id = 123, MaSanPham = "X", TenSanPham = "X" }));

        Assert.Contains("Sản phẩm không tồn tại", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductExists_UpdatesValues_AndNgayCapNhat()
    {
        await using var context = DbContextFactory.Create();
        var existing = new Product { MaSanPham = "P01", TenSanPham = "Old", SoLuongTon = 1, GiaNhap = 10 };
        context.Products.Add(existing);
        await context.SaveChangesAsync();

        var sut = new ProductService(context);
        var updated = new Product
        {
            Id = existing.Id,
            MaSanPham = "P01",
            TenSanPham = "New",
            SoLuongTon = 99,
            GiaNhap = 20
        };

        await sut.UpdateAsync(updated);

        var saved = await context.Products.SingleAsync(p => p.Id == existing.Id);
        Assert.Equal("New", saved.TenSanPham);
        Assert.Equal(99, saved.SoLuongTon);
        Assert.Equal(20, saved.GiaNhap);
        Assert.True(saved.NgayCapNhat > DateTime.MinValue);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_RemovesIt()
    {
        await using var context = DbContextFactory.Create();
        var p = new Product { MaSanPham = "P01", TenSanPham = "ToDelete" };
        context.Products.Add(p);
        await context.SaveChangesAsync();
        var sut = new ProductService(context);

        await sut.DeleteAsync(p.Id);

        Assert.Empty(context.Products);
    }

    [Theory]
    [InlineData(10, new[] { 0, 9 })]
    [InlineData(1, new[] { 0 })]
    public async Task GetLowStockAsync_ReturnsProductsBelowThreshold_AndNonNegativeOnly(int threshold, int[] expectedStock)
    {
        await using var context = DbContextFactory.Create();
        context.Products.AddRange(
            new Product { MaSanPham = "P0", TenSanPham = "P0", SoLuongTon = 0 },
            new Product { MaSanPham = "P1", TenSanPham = "P1", SoLuongTon = 9 },
            new Product { MaSanPham = "P2", TenSanPham = "P2", SoLuongTon = 10 },
            new Product { MaSanPham = "P3", TenSanPham = "P3", SoLuongTon = -1 });
        await context.SaveChangesAsync();
        var sut = new ProductService(context);

        var result = await sut.GetLowStockAsync(threshold);

        Assert.Equal(expectedStock, result.Select(p => p.SoLuongTon).ToArray());
        Assert.True(result.SequenceEqual(result.OrderBy(p => p.SoLuongTon)));
    }

    [Fact]
    public async Task NhapKhoAsync_WhenSoLuongNhapInvalid_Throws()
    {
        await using var context = DbContextFactory.Create();
        var sut = new ProductService(context);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => sut.NhapKhoAsync(1, 0, 1));

        Assert.Contains("Số lượng nhập phải lớn hơn 0", ex.Message);
    }

    [Fact]
    public async Task NhapKhoAsync_WhenGiaNhapMoiNegative_Throws()
    {
        await using var context = DbContextFactory.Create();
        var sut = new ProductService(context);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => sut.NhapKhoAsync(1, 1, -1));

        Assert.Contains("Giá nhập không được âm", ex.Message);
    }

    [Fact]
    public async Task NhapKhoAsync_WhenProductNotFound_ThrowsKeyNotFoundException()
    {
        await using var context = DbContextFactory.Create();
        var sut = new ProductService(context);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.NhapKhoAsync(123, 1, 10));

        Assert.Contains("Sản phẩm không tồn tại", ex.Message);
    }

    [Fact]
    public async Task NhapKhoAsync_UpdatesStockAndGiaNhap_AndAddsStockTransaction()
    {
        await using var context = DbContextFactory.Create();
        var p = new Product { MaSanPham = "P01", TenSanPham = "Nhap", SoLuongTon = 5, GiaNhap = 10 };
        context.Products.Add(p);
        await context.SaveChangesAsync();
        var sut = new ProductService(context);

        await sut.NhapKhoAsync(p.Id, 3, 12, "note");

        var saved = await context.Products.SingleAsync(x => x.Id == p.Id);
        Assert.Equal(8, saved.SoLuongTon);
        Assert.Equal(12, saved.GiaNhap);

        var tx = await context.StockTransactions.SingleAsync();
        Assert.Equal(p.Id, tx.ProductId);
        Assert.Equal(TransactionType.NhapKho, tx.LoaiGiaoDich);
        Assert.Equal(3, tx.SoLuong);
        Assert.Equal(12, tx.Gia);
        Assert.Equal("note", tx.GhiChu);
    }
}

