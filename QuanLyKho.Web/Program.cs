using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using MudBlazor.Services;
using QuanLyKho.Application.Interfaces;
using QuanLyKho.Infrastructure.Data;
using QuanLyKho.Infrastructure.Services;
using QuanLyKho.Web.Components;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

// Database - SQLite file trong thư mục chạy app
var dbPath = Path.Combine(AppContext.BaseDirectory, "quanlykho.db");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Đăng ký các service
builder.Services.AddScoped<ICompanySettingService, CompanySettingService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IProductionService, ProductionService>();
builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IStockTransactionService, StockTransactionService>();
builder.Services.AddScoped<IPurchaseInvoiceService, PurchaseInvoiceService>();
builder.Services.AddScoped<IWithholdingTaxService, WithholdingTaxService>();

var app = builder.Build();

// Tùy chọn: xóa database cũ để chạy lại từ đầu (đặt RESET_DB=1 khi chạy app)
if (string.Equals(Environment.GetEnvironmentVariable("RESET_DB"), "1", StringComparison.OrdinalIgnoreCase) && File.Exists(dbPath))
{
    try { File.Delete(dbPath); } catch { /* bỏ qua nếu file đang bị lock */ }
}

// Tự động migrate database khi khởi động
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    // Đảm bảo toàn bộ bảng/cột cần thiết tồn tại (tránh lỗi DB cũ thiếu schema)
    await EnsureSchemaAsync(db);
}

static async Task EnsureSchemaAsync(AppDbContext db)
{
    // 0. Bảng Suppliers (nhà cung cấp)
    await db.Database.ExecuteSqlRawAsync(@"
        CREATE TABLE IF NOT EXISTS Suppliers (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            MaNhaCungCap TEXT NOT NULL,
            TenNhaCungCap TEXT NOT NULL,
            DienThoai TEXT NOT NULL,
            DiaChi TEXT NOT NULL,
            Email TEXT NOT NULL,
            GhiChu TEXT NOT NULL
        );");

    // 1. Bảng Customers
    await db.Database.ExecuteSqlRawAsync(@"
        CREATE TABLE IF NOT EXISTS Customers (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            MaKhachHang TEXT NOT NULL,
            TenKhachHang TEXT NOT NULL,
            DienThoai TEXT NOT NULL,
            DiaChi TEXT NOT NULL,
            Email TEXT NOT NULL,
            GhiChu TEXT NOT NULL
        );");

    // 2. Cột trên Invoices
    await TryAddColumnAsync(db, "Invoices", "CustomerId", "INTEGER", null);
    await TryAddColumnAsync(db, "Invoices", "TienDaThu", "TEXT", "0");
    await TryAddColumnAsync(db, "Invoices", "ThueGTGT", "TEXT", "0");

    // 3. Cột trên Products
    await TryAddColumnAsync(db, "Products", "SoLuongCanhBao", "INTEGER", "10");
    await TryAddColumnAsync(db, "Products", "LoaiSanPham", "INTEGER", "0");
    await TryAddColumnAsync(db, "Products", "SupplierId", "INTEGER", null);

    // 4. Bảng PurchaseInvoices
    await db.Database.ExecuteSqlRawAsync(@"
        CREATE TABLE IF NOT EXISTS PurchaseInvoices (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            SoHoaDon TEXT NOT NULL,
            NgayHoaDon TEXT NOT NULL,
            NhaCungCap TEXT NOT NULL,
            TienHangChuaThue TEXT NOT NULL,
            ThueGTGT TEXT NOT NULL,
            TienThueGTGT TEXT NOT NULL,
            GhiChu TEXT NOT NULL
        );");

    // 5. Bảng WithholdingTaxes
    await db.Database.ExecuteSqlRawAsync(@"
        CREATE TABLE IF NOT EXISTS WithholdingTaxes (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            NgayChi TEXT NOT NULL,
            NguoiNhan TEXT NOT NULL,
            MaSoThue TEXT NOT NULL,
            ThuNhapChiuThue TEXT NOT NULL,
            ThueKhauTru TEXT NOT NULL,
            GhiChu TEXT NOT NULL
        );");

    // 6. Cột tỷ lệ thuế trên CompanySettings
    await TryAddColumnAsync(db, "CompanySettings", "TyLeThueGTGTTrucTiep", "TEXT", "1");
    await TryAddColumnAsync(db, "CompanySettings", "ThueGTGTMacDinh", "TEXT", "0");
    await TryAddColumnAsync(db, "CompanySettings", "TyLeThuNhapUocTinh", "TEXT", "30");

    // 7. Bảng SalesOrders & SalesOrderItems
    await db.Database.ExecuteSqlRawAsync(@"
        CREATE TABLE IF NOT EXISTS SalesOrders (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            SoDonHang TEXT NOT NULL,
            NgayDat TEXT NOT NULL,
            CustomerId INTEGER NULL,
            TenKhachHang TEXT NOT NULL,
            DienThoai TEXT NOT NULL,
            DiaChiGiaoHang TEXT NOT NULL,
            TrangThai INTEGER NOT NULL,
            TienDaTraTruoc TEXT NOT NULL,
            GhiChu TEXT NOT NULL
        );");

    await db.Database.ExecuteSqlRawAsync(@"
        CREATE TABLE IF NOT EXISTS SalesOrderItems (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            SalesOrderId INTEGER NOT NULL,
            MoTaSanPham TEXT NOT NULL,
            SoLuong INTEGER NOT NULL,
            DonVi TEXT NOT NULL,
            DonGia TEXT NOT NULL,
            SoLuongDaSanXuat INTEGER NOT NULL DEFAULT 0,
            CONSTRAINT FK_SalesOrderItems_SalesOrders_SalesOrderId FOREIGN KEY (SalesOrderId) REFERENCES SalesOrders (Id) ON DELETE CASCADE
        );");

    // Đảm bảo các cột mới tồn tại đối với DB cũ
    await TryAddColumnAsync(db, "SalesOrderItems", "SoLuongDaSanXuat", "INTEGER", "0");
    await TryAddColumnAsync(db, "SalesOrders", "TienDaTraTruoc", "TEXT", "0");
}

static async Task TryAddColumnAsync(AppDbContext db, string table, string column, string type, string? defaultValue)
{
    try
    {
        var defaultClause = defaultValue != null ? $" DEFAULT {defaultValue}" : "";
        var notNull = defaultValue != null ? " NOT NULL" : "";
        await db.Database.ExecuteSqlRawAsync(
            $"ALTER TABLE {table} ADD COLUMN {column} {type}{notNull}{defaultClause};");
    }
    catch (SqliteException)
    {
        // Cột đã tồn tại hoặc lỗi khác khi thêm cột -> bỏ qua để app vẫn chạy
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();