namespace ddd_efcore.OrderProcessing.Tests
{
    using ddd_efcore.OrderProcessing;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using Xunit;

    public class OrderTests: TestHarness
    {
        [Fact(Skip = "Slow and non-idempotent")]
        public async Task CreateOrder_SavesToDatabase()
        {
            var ctx = TestDbContextFactory.CreateDbContext();

            // Arrange
            var email = Email.Create("john.doe@example.com");
            var customer = Customer.Create("John Doe", email);

            customer.PlaceOrder(21.12m, DateTime.UtcNow.AddDays(-1));

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
        }

        [Fact]
        public void Customer_PlaceOrder_AddsOrderToCollection()
        {
            // Arrange
            var customer = Customer.Create("John Doe", Email.Create("john.doe@example.com"));
            var amount = 100.50m;
            var orderDate = DateTime.UtcNow.AddDays(-1);

            // Act
            customer.PlaceOrder(amount, orderDate);
            var orders = customer.Orders;

            // Assert
            Assert.Single(orders); // Ensure one order was added
            var order = orders.First();
            Assert.Equal(amount, order.Amount);
            Assert.Equal(orderDate, order.OrderDate);
            Assert.Equal(OrderStatus.Pending, order.Status);
            Assert.Equal(customer.Id, order.CustomerId); // Verify relationship
        }

        [Fact]
        public void Order_Constructor_ThrowsOrderValidityException_ForFutureDate()
        {
            // Arrange
            var customer = Customer.Create("Jane Doe", Email.Create("jane.doe@example.com"));
            var amount = 200.75m;
            var futureDate = DateTime.UtcNow.AddDays(1); // Invalid: future date

            // Act & Assert
            var exception = Assert.Throws<OrderValidityException>(() =>
                customer.PlaceOrder(amount, futureDate));
            Assert.Equal("Order date cannot be in the future.", exception.Message);
        }

        [Fact]
        public void Customer_ConfirmOrder_UpdatesOrderStatus()
        {
            // Arrange
            var customer = Customer.Create("John Doe", Email.Create("john.doe@example.com"));
            var amount = 150.25m;
            var orderDate = DateTime.UtcNow.AddDays(-1);
            customer.PlaceOrder(amount, orderDate);
            var orderId = customer.Orders.First().Id;

            // Act
            customer.ConfirmOrder(orderId);

            // Assert
            var order = customer.Orders.First();
            Assert.Equal(OrderStatus.Confirmed, order.Status);
        }

        [Fact]
        public void Customer_ConfirmOrder_ThrowsOrderValidityException_ForNonExistentOrder()
        {
            // Arrange
            var customer = Customer.Create("Jane Doe", Email.Create("jane.doe@example.com"));
            var nonExistentOrderId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<OrderValidityException>(() =>
                customer.ConfirmOrder(nonExistentOrderId));
            Assert.Equal("Order not found.", exception.Message);
        }
    }
}