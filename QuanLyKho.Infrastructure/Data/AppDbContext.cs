using Microsoft.EntityFrameworkCore;
using QuanLyKho.Domain.Entities;

namespace QuanLyKho.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<CompanySetting> CompanySettings => Set<CompanySetting>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceDetail> InvoiceDetails => Set<InvoiceDetail>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<PurchaseInvoice> PurchaseInvoices => Set<PurchaseInvoice>();
    public DbSet<WithholdingTax> WithholdingTaxes => Set<WithholdingTax>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderItem> SalesOrderItems => Set<SalesOrderItem>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<CompanySetting>()
            .HasData(new CompanySetting { Id = 1 });

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Customer)
            .WithMany()
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<InvoiceDetail>()
          .HasOne(d => d.Invoice)
          .WithMany(i => i.ChiTiet)
          .HasForeignKey(d => d.InvoiceId)
          .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier)
            .WithMany()
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SalesOrderItem>()
            .HasOne(i => i.SalesOrder)
            .WithMany(o => o.ChiTiet)
            .HasForeignKey(i => i.SalesOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<StockTransaction>()
        //    .HasOne<Product>()
        //    .WithMany()
        //    .HasForeignKey(t => t.ProductId);
    }
}