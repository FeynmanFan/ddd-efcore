namespace ddd_efcore.OrderProcessing.Tests
{
    using ddd_efcore.OrderProcessing;
    using Microsoft.EntityFrameworkCore;

    public class AddressTests: TestHarness
    {
        public ICityValidator validator = new StaticCityValidator();

        [Fact]
        public async Task Address_ValidatesAndPersistsCorrectly()
        {
            // Arrange
            var context = CreateDbContext();

            var customer = Customer.Create(
                name: "Jane Doe",
                email: Email.Create("jane.doe@example.com"),
                customerType: CustomerTypeEnum.Private
            );

            customer.AddAddress("123 Main Street", "Beverly Hills", "CA", "90210", validator);
            customer.AddAddress("5150 Camp Bowie Boulevard", "Fort Worth", "TX", "76103", validator);

            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            // Act
            var loadedCustomer = await context.Customers
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
                Address.Create("123 Main Street", "", "CA", "90210", validator)); // Empty city
            Assert.Throws<AddressValidityException>(() =>
                Address.Create("123 Main Street", "Beverly Hills", "XX", "90210", validator)); // Invalid state
            Assert.Throws<AddressValidityException>(() =>
                Address.Create("123 Main Street", "Beverly Hills", "CA", "1234", validator)); // Invalid ZIP
            Assert.Throws<AddressValidityException>(() =>
                Address.Create("5150 Camp Bowie Boulevard", "Fort Worth", "TX", "90210", validator)); // ZIP mismatch
        }

        [Fact(Skip="Slow and non-idempotent")]
        public async Task SaveToTestDB()
        {
            var ctx = TestDbContextFactory.CreateDbContext();

            var customer = Customer.Create(
                name: "Chris B. Behrens",
                email: Email.Create("cbehrens2@example.com"),
                customerType: CustomerTypeEnum.Private
            );

            customer.AddAddress("123 Main Street", "Beverly Hills", "CA", "90210", validator);
            customer.AddAddress("5150 Camp Bowie Boulevard", "Fort Worth", "TX", "76103", validator);

            ctx.Customers.Add(customer);
            await ctx.SaveChangesAsync();
        }
    }
}