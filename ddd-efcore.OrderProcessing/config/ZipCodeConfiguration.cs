using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ddd_efcore.OrderProcessing.config
{
    public class ZipCodeConfiguration : IEntityTypeConfiguration<ZipCode>
    {
        public void Configure(EntityTypeBuilder<ZipCode> builder)
        {
            builder.HasKey(z => z.ZipCodeValue);
            builder.Property(z => z.ZipCodeValue)
                .IsRequired()
                .HasMaxLength(20); // Supports ZIP+4 or international formats
            builder.Property(z => z.City)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(z => z.State)
                .IsRequired()
                .HasMaxLength(50);
            builder.HasIndex(z => z.ZipCodeValue)
                .IsUnique(); // Ensure unique ZIP codes
            builder.ToTable("ZipCodes");
        }
    }
}