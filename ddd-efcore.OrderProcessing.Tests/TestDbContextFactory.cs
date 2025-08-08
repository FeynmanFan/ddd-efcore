namespace ddd_efcore.OrderProcessing.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public static class TestDbContextFactory
    {
        public static OrderProcessingDbContext CreateDbContext()
        {
            // Load configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string connectionString = configuration.GetConnectionString("TestDatabase");

            var options = new DbContextOptionsBuilder<OrderProcessingDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new OrderProcessingDbContext(options);
        }
    }
}