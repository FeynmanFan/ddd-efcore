using Microsoft.EntityFrameworkCore;

namespace ddd_efcore.OrderProcessing
{
    public class OrderProcessingDbContext(DbContextOptions<OrderProcessingDbContext> options) : DbContext(options)
    {
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        }
    }
}