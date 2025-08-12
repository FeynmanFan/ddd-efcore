namespace ddd_efcore.OrderProcessing.DataLayer.config
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    // EF Core configuration for the Customer entity
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // Set Id as the primary key
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Name");

            builder.OwnsOne(c => c.Email, email =>
            {
                email.Property(e => e.Value)
                     .IsRequired()
                     .HasColumnName("Email");
            });

            builder.Metadata.FindNavigation(nameof(Customer.Addresses))
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany<Address>(c => c.Addresses)
                .WithOne()
                .HasForeignKey("CustomerId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.CustomerType)
                .IsRequired()
                .HasConversion(
                    customerType => (int)customerType.Value,
                    value => CustomerType.Create(value))
                .HasColumnName("CustomerType")
                .HasColumnType("int");

            builder.HasMany(c => c.Orders)
               .WithOne()
               .HasForeignKey("CustomerId")
               .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(c => c.DomainEvents);
        }
    }
}