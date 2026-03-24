using Microsoft.EntityFrameworkCore;
using QuanLyKho.Infrastructure.Data;

namespace QuanLyKho.Tests.TestUtils;

public static class DbContextFactory
{
    public static AppDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString("N"))
            .EnableSensitiveDataLogging()
            .Options;

        return new AppDbContext(options);
    }
}

