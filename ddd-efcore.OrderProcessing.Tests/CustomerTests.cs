namespace ddd_efcore.OrderProcessing.Tests
{
    using Microsoft.EntityFrameworkCore;

    public class CustomerTests
    {
        private OrderProcessingDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<OrderProcessingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
                .Options;
            return new OrderProcessingDbContext(options);
        }

        [Fact]
        public void CreateCustomer_WithValidData_Succeeds()
        {
            // Arrange
            var email = new Email("john@example.com");
            var name = "John Doe";

            // Act
            var customer = new Customer(name, email);

            // Assert
            Assert.NotNull(customer);
            Assert.NotEqual(Guid.Empty, customer.Id);
            Assert.Equal(name, customer.Name);
            Assert.Equal(email.Value, customer.Email.Value);
            Assert.True(customer.CreatedAt <= DateTime.UtcNow);
        }

        [Fact(Skip="Not ready for validity yet")]
        public void CreateCustomer_WithInvalidEmail_ThrowsException()
        {
            // Arrange
            var name = "John Doe";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Email("")); // Empty email
            Assert.Throws<ArgumentException>(() => new Email("invalid-email")); // No @
        }

        [Fact(Skip = "Not ready for validity yet")]
        public void CreateCustomer_WithInvalidName_ThrowsException()
        {
            // Arrange
            var email = new Email("john@example.com");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Customer("", email)); // Empty name
            Assert.Throws<ArgumentException>(() => new Customer(null, email)); // Null name
        }

        [Fact]
        public void PersistCustomer_WithValidData_SavesAndRetrievesCorrectly()
        {
            // Arrange
            using var context = CreateDbContext();
            var email = new Email("john@example.com");
            var customer = new Customer("John Doe", email);

            // Act
            context.Customers.Add(customer);
            context.SaveChanges();

            // Assert
            var savedCustomer = context.Customers.FirstOrDefault();
            Assert.NotNull(savedCustomer);
            Assert.Equal(customer.Id, savedCustomer.Id);
            Assert.Equal(customer.Name, savedCustomer.Name);
            Assert.Equal(customer.Email.Value, savedCustomer.Email.Value);
            Assert.Equal(customer.CreatedAt, savedCustomer.CreatedAt);
        }

        [Fact]
        public void UpdateCustomerEmail_UpdatesSuccessfully()
        {
            // Arrange
            using var context = CreateDbContext();
            var originalEmail = new Email("john@example.com");
            var customer = new Customer("John Doe", originalEmail);
            context.Customers.Add(customer);
            context.SaveChanges();

            // Act
            var newEmail = new Email("jane@example.com");
            customer.UpdateEmail(newEmail);
            context.SaveChanges();

            // Assert
            var updatedCustomer = context.Customers.FirstOrDefault();
            Assert.NotNull(updatedCustomer);
            Assert.Equal(newEmail.Value, updatedCustomer.Email.Value);
            Assert.Equal(customer.Name, updatedCustomer.Name);
            Assert.Equal(customer.Id, updatedCustomer.Id);
        }
    }
}