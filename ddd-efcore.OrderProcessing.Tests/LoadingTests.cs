namespace ddd_efcore.OrderProcessing.Tests
{
    using Microsoft.EntityFrameworkCore;

    public class LoadingTests
    {
        [Fact]
        public async Task EagerLoading_CustomerWithOrders_LoadsOrdersInSingleQuery()
        {
            // Arrange
            var context = TestDbContextFactory.CreateDbContext();
            var customer = Customer.Create("John Doe", Email.Create("john.doe@example.com"));
            customer.PlaceOrder(100.50m, DateTime.UtcNow.AddDays(-1));
            customer.PlaceOrder(200.75m, DateTime.UtcNow.AddDays(-2));
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            // Clear context to ensure fresh query
            context.ChangeTracker.Clear();

            // Act
            var loadedCustomer = await context.Customers
                .Include(c => c.Orders) // Eagerly load Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == customer.Id);

            // Assert
            Assert.NotNull(loadedCustomer);
            Assert.Equal(2, loadedCustomer.Orders.Count); // Orders loaded eagerly
            Assert.All(loadedCustomer.Orders, o => Assert.Equal(customer.Id, o.CustomerId));
            Assert.All(loadedCustomer.Orders, o => Assert.Null(o.Payment)); // no payments
        }

        [Fact]
        public async Task LazyLoading_OrdersWithPayments_LoadsPaymentsOnDemand()
        {
            // Arrange
            var context = TestDbContextFactory.CreateDbContext();
            var customer = Customer.Create("Jane Doe", Email.Create("jane.doe@example.com"));
            customer.PlaceOrder(100.50m, DateTime.UtcNow.AddDays(-1));
            customer.Orders[0].Pay();
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
            var orderId = customer.Orders[0].Id;

            // new db context
            var actContext = TestDbContextFactory.CreateDbContext();

            // Act
            var loadedOrder = await actContext.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            // Assert
            Assert.NotNull(loadedOrder);
            var payment = loadedOrder.Payment; // Triggers lazy loading
            Assert.NotNull(payment); // Payment loaded on demand
            Assert.Equal(loadedOrder.Id, payment.OrderId);
            Assert.Equal(100.50m, payment.Amount);
        }
    }
}