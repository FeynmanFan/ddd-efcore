using ddd_efcore.OrderProcessing.config;
using Microsoft.EntityFrameworkCore;

namespace ddd_efcore.OrderProcessing
{
    public class OrderProcessingDbContext(DbContextOptions<OrderProcessingDbContext> options) : DbContext(options)
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        }
    }
}