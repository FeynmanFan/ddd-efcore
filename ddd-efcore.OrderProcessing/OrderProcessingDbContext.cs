using ddd_efcore.OrderProcessing.config;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ddd_efcore.OrderProcessing
{
    public class OrderProcessingDbContext(DbContextOptions<OrderProcessingDbContext> options) : DbContext(options)
    {
        private readonly IMediator _mediator;

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public OrderProcessingDbContext(DbContextOptions<OrderProcessingDbContext> options, IMediator mediator): this(options)
        {
            _mediator = mediator;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Collect domain events
            var domainEvents = ChangeTracker
                .Entries<DDDObject>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            // Save changes
            var result = await base.SaveChangesAsync(cancellationToken);

            // Dispatch events
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            // Clear events
            foreach (var entry in ChangeTracker.Entries<DDDObject>())
            {
                entry.Entity.ClearDomainEvents();
            }

            return result;
        }
    }
}