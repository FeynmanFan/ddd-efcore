namespace ddd_efcore.OrderProcessing.DataLayer.config
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AddressConfiguration: IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.ZipCode);
            builder.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(a => a.ZipCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.ToTable("Addresses");
        }
    }
}
