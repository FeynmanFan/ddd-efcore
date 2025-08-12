namespace ddd_efcore.OrderProcessing.Tests
{
    using Microsoft.EntityFrameworkCore;

    public class AddressTests : TestHarness, IDisposable
    {
        private readonly ICityValidator _validator;
        private readonly IReadOnlyList<ZipCode> _zipCodes;

        public AddressTests()
        {
            // Create disconnected ZipCode objects
            _zipCodes = new List<ZipCode>
            {
                new("90210", "Beverly Hills", "CA"),
                new("10001", "New York", "NY"),
                new("76103", "Fort Worth", "TX"),
                new("00501", "Holtsville", "NY")
            }.AsReadOnly();

            // Attach ZipCodes to context
            AttachZipCodes();

            _validator = new StaticCityValidator(_zipCodes);
        }

        private void AttachZipCodes()
        {
            foreach (var zipCode in _zipCodes)
            {
                _context.ZipCodes.Attach(zipCode);
                _context.Entry(zipCode).State = EntityState.Added;
            }
            _context.SaveChanges();
        }

        [Fact]
        public async Task AddAddress_ShouldLogAuditEntry()
        {
            // Arrange
            var email = Email.Create("alice@example.com");
            var customer = Customer.Create("Alice Smith", email, CustomerTypeEnum.Private);

            // Act
            customer.AddAddress("456 Elm St", "Holtsville", "NY", "00501", _validator);
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            // Assert
            var auditLogs = _auditLogger.GetLogs();
            Assert.Equal(3, auditLogs.Count); // Customer, Address, and Email

            var customerAudit = auditLogs.First(a => a.EntityType == nameof(Customer) && a.EntityId == customer.Id);
            Assert.Equal("Created", customerAudit.Action);
            Assert.Equal("System", customerAudit.UserId);
            Assert.True(customerAudit.Timestamp <= DateTime.UtcNow);

            var addressAudit = auditLogs.First(a => a.EntityType == nameof(Address));
            Assert.Equal("Created", addressAudit.Action);
            Assert.Equal("System", addressAudit.UserId);
            Assert.True(addressAudit.Timestamp <= DateTime.UtcNow);

            var emailAudit = auditLogs.First(a => a.EntityType == nameof(Email));
            Assert.Equal("Created", emailAudit.Action);
            Assert.Equal("System", emailAudit.UserId);
            Assert.True(emailAudit.Timestamp <= DateTime.UtcNow);
        }

        [Fact]
        public async Task Address_ValidatesAndPersistsCorrectly()
        {
            // Arrange
            var customer = Customer.Create(
                name: "Jane Doe",
                email: Email.Create("jane.doe@example.com"),
                customerType: CustomerTypeEnum.Private
            );

            customer.AddAddress("123 Main Street", "Beverly Hills", "CA", "90210", _validator);
            customer.AddAddress("5150 Camp Bowie Boulevard", "Fort Worth", "TX", "76103", _validator);

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var loadedCustomer = await _context.Customers
                .AsNoTracking()
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Id == customer.Id);

            // Assert
            Assert.NotNull(loadedCustomer);
            Assert.Equal(2, loadedCustomer.Addresses.Count);

            var address1 = loadedCustomer.Addresses.First(a => a.ZipCode == "90210");
            Assert.Equal("123 Main Street", address1.Street);
            Assert.Equal("90210", address1.ZipCode);

            var address2 = loadedCustomer.Addresses.First(a => a.ZipCode == "76103");
            Assert.Equal("5150 Camp Bowie Boulevard", address2.Street);
            Assert.Equal("76103", address2.ZipCode);

            // Verify validation
            Assert.Throws<AddressValidityException>(() =>
                Address.Create("123 Main Street", "", "CA", "90210", _validator)); // Empty city
            Assert.Throws<AddressValidityException>(() =>
                Address.Create("123 Main Street", "Beverly Hills", "XX", "90210", _validator)); // Invalid state
            Assert.Throws<AddressValidityException>(() =>
                Address.Create("123 Main Street", "Beverly Hills", "CA", "1234", _validator)); // Invalid ZIP
            Assert.Throws<AddressValidityException>(() =>
                Address.Create("5150 Camp Bowie Boulevard", "Fort Worth", "TX", "90210", _validator)); // ZIP mismatch
        }

        [Fact]
        public async Task Texas_ZipCode_Only()
        {
            // Arrange
            var texasZipCodes = _context.ZipCodes
                .Where(z => z.State == "TX")
                .ToList();

            var txValidator = new StaticCityValidator(texasZipCodes);

            //Act & Assert
            await Assert.ThrowsAsync<AddressValidityException>(async () =>
                await Task.Run(() => Address.Create("123 Main Street", "Beverly Hills", "CA", "90210", txValidator))); // mostly definitely not in Texas
        }

        [Fact]
        public async Task AddAddress_WithValidZipCode_ShouldSucceed()
        {
            // Arrange
            var email = Email.Create("john@example.com");
            var customer = Customer.Create("John Doe", email, CustomerTypeEnum.Private);

            // Act
            customer.AddAddress("123 Main St", "Beverly Hills", "CA", "90210", _validator);
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            // Assert
            Assert.Single(customer.Addresses);
            var address = customer.Addresses[0];
            Assert.Equal("123 Main St", address.Street);
            Assert.Equal("90210", address.ZipCode);
        }

        [Fact]
        public void AddAddress_WithInvalidZipCode_ShouldThrow()
        {
            // Arrange
            var email = Email.Create("john@example.com");
            var customer = Customer.Create("John Doe", email, CustomerTypeEnum.Private);

            // Act & Assert
            var exception = Assert.Throws<AddressValidityException>(() =>
                customer.AddAddress("123 Main St", "Beverly Hills", "CA", "99999", _validator));
            Assert.Contains("Invalid address: Zip code 99999", exception.Message);
        }

        [Fact(Skip = "Slow and non-idempotent")]
        public async Task SaveToTestDB()
        {
            // Use TestDbContextFactory from TestHarness
            var ctx = TestDbContextFactory.CreateDbContext();

            var customer = Customer.Create(
                name: "Chris B. Behrens",
                email: Email.Create("cbehrens2@example.com"),
                customerType: CustomerTypeEnum.Private
            );

            customer.AddAddress("123 Main Street", "Beverly Hills", "CA", "90210", _validator);
            customer.AddAddress("5150 Camp Bowie Boulevard", "Fort Worth", "TX", "76103", _validator);

            ctx.Customers.Add(customer);
            await ctx.SaveChangesAsync();
        }
    }
}