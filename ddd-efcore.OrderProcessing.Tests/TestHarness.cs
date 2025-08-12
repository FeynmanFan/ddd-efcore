namespace ddd_efcore.OrderProcessing.Tests
{
    using ddd_efcore.Auditing;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    public abstract class TestHarness
    {
        protected readonly ServiceProvider _serviceProvider;
        protected readonly OrderProcessingDbContext _context;
        protected readonly IMediator _mediator;
        protected readonly StringWriter _consoleOutput;
        protected readonly IAuditLogger _auditLogger;

        public TestHarness()
        {
            // Set up DI container
            var services = new ServiceCollection();

            // Register ILoggerFactory (use NullLoggerFactory for testing)
            services.AddSingleton<ILoggerFactory, NullLoggerFactory>();

            // Configure in-memory database
            services.AddDbContext<OrderProcessingDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString())); // Unique DB per test

            // Add MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CustomerUpdatedEventHandler).Assembly));

            // add logger
            services.AddSingleton<IAuditLogger, InMemoryAuditLogger>();

            // Build service provider
            _serviceProvider = services.BuildServiceProvider();

            // Resolve dependencies
            _context = _serviceProvider.GetRequiredService<OrderProcessingDbContext>();
            _mediator = _serviceProvider.GetRequiredService<IMediator>();
            _auditLogger = _serviceProvider.GetRequiredService<IAuditLogger>();

            // Capture console output
            _consoleOutput = new StringWriter();
            Console.SetOut(_consoleOutput);
        }

        protected OrderProcessingDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<OrderProcessingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
                .Options;
            return new OrderProcessingDbContext(options);
        }

        public void Dispose()
        {
            _context.Dispose();
            _serviceProvider.Dispose();
            _consoleOutput.Dispose();
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }
    }
}
