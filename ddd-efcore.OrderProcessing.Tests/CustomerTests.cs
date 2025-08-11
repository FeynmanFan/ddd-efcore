namespace ddd_efcore.OrderProcessing.Tests
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;

    public class CustomerTests: TestHarness
    {
        [Fact]
        public async Task UpdateName_ShouldRaiseCustomerUpdatedEvent()
        {
            var email = Email.Create("john@example.com");
            var customer = Customer.Create("John Doe", email, CustomerTypeEnum.Private);
            var newName = "Jane Doe";

            // Act
            customer.UpdateName(newName);

            var domainEvent = customer.DomainEvents
                .OfType<CustomerUpdatedEvent>()
                .SingleOrDefault();
            Assert.NotNull(domainEvent);

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            // Assert
            
            Assert.Equal(customer.Id, domainEvent.CustomerId);
            Assert.Equal(newName, domainEvent.Name);
            Assert.Equal(email.Value, domainEvent.Email);
            Assert.True(domainEvent.OccurredOn <= DateTime.UtcNow);

            // Verify console output from event handler
            var output = _consoleOutput.ToString();
            Assert.Contains($"Customer Updated Event: CustomerId={customer.Id}, Name={newName}, Email={email.Value}", output);
        }

        [Fact]
        public async Task UpdateEmail_ShouldRaiseCustomerUpdatedEvent()
        {
            var email = Email.Create("john@example.com");
            var newEmail = Email.Create("jane@example.com");
            var customer = Customer.Create("John Doe", email, CustomerTypeEnum.Private);

            // Act
            customer.UpdateEmail(newEmail);
            
            var domainEvent = customer.DomainEvents
            .OfType<CustomerUpdatedEvent>()
            .SingleOrDefault();

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            // Assert

            Assert.NotNull(domainEvent);
            Assert.Equal(customer.Id, domainEvent.CustomerId);
            Assert.Equal(customer.Name, domainEvent.Name);
            Assert.Equal(newEmail.Value, domainEvent.Email);
            Assert.True(domainEvent.OccurredOn <= DateTime.UtcNow);

            // Verify console output from event handler
            var output = _consoleOutput.ToString();
            Assert.Contains($"Customer Updated Event: CustomerId={customer.Id}, Name={customer.Name}, Email={newEmail.Value}", output);
        }

        [Fact(Skip="Slow and non-idempotent")]
        public async Task CreateCustomer_ValidInput_SavesToDatabase()
        {
            var ctx = TestDbContextFactory.CreateDbContext();

            // Arrange
            var customer = Customer.Create("Chris Behrens", Email.Create("cbehrens@example.com"), CustomerTypeEnum.Business);

            // Act
            ctx.Customers.Add(customer);
            await ctx.SaveChangesAsync();

            // Assert
            var savedCustomer = await ctx.Customers
                .SingleOrDefaultAsync(c => c.Id == customer.Id);
            Assert.NotNull(savedCustomer);
            Assert.Equal("Chris Behrens", savedCustomer.Name);
            Assert.Equal("cbehrens@example.com", savedCustomer.Email.Value);
            Assert.Equal(CustomerTypeEnum.Business, savedCustomer.CustomerType.Value);
        }

        [Fact]
        public void ComparisonStrategy_ValidatesCorrectly()
        {
            // Arrange
            var customer1 = Customer.Create("John Doe", Email.Create("test@test.com"));
            var customer2 = Customer.Create("John Doe", Email.Create("test@test.com"));

            Assert.NotEqual(customer1, customer2);

            var customer3 = customer1;

            Assert.Equal(customer1, customer3);

            var email1 = Email.Create("test@test.com");
            var email2 = Email.Create("test@test.com");

            Assert.Equal(email1, email2);

            var email3 = Email.Create("xyz@test.com");
                
            Assert.NotEqual(email1, email3);

            Assert.True(customer1 != customer2);
            Assert.True(customer1 == customer3);
            Assert.True(email1 == email2);
            Assert.True(email1 != email3);
        }

        [Fact]
        public void CreateCustomer_WithValidData_Succeeds()
        {
            // Arrange
            var email = Email.Create("john@example.com");
            var name = "John Doe";

            // Act
            var customer = Customer.Create(name, email);

            // Assert
            Assert.NotNull(customer);
            Assert.NotEqual(Guid.Empty, customer.Id);
            Assert.Equal(name, customer.Name);
            Assert.Equal(email.Value, customer.Email.Value);
            Assert.True(customer.CreatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void CreateCustomer_WithInvalidEmail_ThrowsException()
        {
            // Arrange
            var name = "John Doe";

            // Act & Assert
            Assert.Throws<ValidationException>(() => Email.Create("")); // Empty email
            Assert.Throws<ValidationException>(() => Email.Create("invalid-email")); // No @
        }

        [Fact]
        public void CreateCustomer_WithInvalidName_ThrowsException()
        {
            // Arrange
            var email = Email.Create("john@example.com");

            // Act & Assert
            Assert.Throws<ValidationException>(() => Customer.Create("", email)); // Empty name
            Assert.Throws<ValidationException>(() => Customer.Create(null, email)); // Null name
        }

        [Fact]
        public void PersistCustomer_WithValidData_SavesAndRetrievesCorrectly()
        {
            // Arrange
            using var context = CreateDbContext();
            var email = Email.Create("john@example.com");
            var customer = Customer.Create("John Doe", email);

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
            var originalEmail = Email.Create("john@example.com");
            var customer = Customer.Create("John Doe", originalEmail);
            context.Customers.Add(customer);
            context.SaveChanges();

            // Act
            var newEmail = Email.Create("jane@example.com");
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