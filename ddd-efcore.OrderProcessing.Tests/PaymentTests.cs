using Microsoft.EntityFrameworkCore;

namespace ddd_efcore.OrderProcessing.Tests
{
    public class PaymentTests
    {
        [Fact]
        public void Payment_Creation_ShouldSucceed()
        {
            var customer = Customer.Create("John Doe", Email.Create("test@test.com"));

            customer.PlaceOrder(100.00m, DateTime.UtcNow);

            var order = customer.Orders.Single<Order>();

            order.Pay();

            var payment = order.Payment;

            Assert.Equal(100.00m, payment.Amount);
        }

        [Fact(Skip="Slow and non-idempotent")]
        public async Task CreateOrder_SavesToDatabase()
        {
            var ctx = TestDbContextFactory.CreateDbContext();

            // Arrange
            var email = Email.Create("john.doe@example.com");
            var customer = Customer.Create("John Doe", email);

            customer.PlaceOrder(21.12m, DateTime.UtcNow.AddDays(-1));

            customer.Orders.Single().Pay();

            // Act
            ctx.Customers.Add(customer);
            await ctx.SaveChangesAsync();

            // Assert
            var savedCustomer = await ctx.Customers
                .SingleOrDefaultAsync(c => c.Id == customer.Id);
            Assert.NotNull(savedCustomer);
            Assert.Single(savedCustomer.Orders);

            var order = savedCustomer.Orders.Single();

            Assert.Equal(21.12m, order.Amount);

            Assert.NotNull(order.Payment);
        }
    }
}
