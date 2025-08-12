using ddd_efcore.Auditing;
using ddd_efcore.OrderProcessing.config;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace ddd_efcore.OrderProcessing
{
    public class OrderProcessingDbContext(DbContextOptions<OrderProcessingDbContext> options) : DbContext(options)
    {
        private readonly IMediator _mediator;
        private readonly IAuditLogger _auditLogger;
        private readonly string _currentUserId = "System";

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ZipCode> ZipCodes { get; set; }

        public OrderProcessingDbContext(DbContextOptions<OrderProcessingDbContext> options, IMediator mediator, IAuditLogger auditLogger) : this(options)
        {
            _mediator = mediator;
            _auditLogger = auditLogger;
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
            modelBuilder.ApplyConfiguration(new ZipCodeConfiguration());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Collect domain events
            var domainEvents = ChangeTracker
                .Entries<DDDObject>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            if (_auditLogger != null)
            {
                var auditEntries = ChangeTracker.Entries()
                    .Where(e => e.Entity is DDDEntity or DDDObject)
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                    .Select(e => new
                    {
                        e.Entity,
                        e.State,
                        EntityId = e.Entity is DDDEntity de ? de.Id : Guid.Empty,
                        Changes = e.State == EntityState.Modified
                            ? JsonSerializer.Serialize(e.Properties
                                .Where(p => p.IsModified)
                                .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue))
                            : null
                    })
                    .ToList();

                foreach (var entry in auditEntries)
                {
                    _auditLogger.Log(new AuditLogEntry(
                        entityType: entry.Entity.GetType().Name,
                        entityId: entry.EntityId,
                        action: entry.State == EntityState.Added ? "Created" : "Updated",
                        userId: _currentUserId,
                        timestamp: DateTime.UtcNow,
                        changes: entry.Changes
                    ));
                }
            }

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