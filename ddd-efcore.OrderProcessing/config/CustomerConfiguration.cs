using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ddd_efcore.OrderProcessing.config
{
    // EF Core configuration for the Customer entity
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // Set Id as the primary key
            builder.HasKey(c => c.Id);

            builder.OwnsOne(c => c.Email, email =>
            {
                email.Property(e => e.Value)
                     .IsRequired()
                     .HasColumnName("Email");
            });

            builder.HasMany(c => c.Orders)
               .WithOne()
               .HasForeignKey("CustomerId")
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}