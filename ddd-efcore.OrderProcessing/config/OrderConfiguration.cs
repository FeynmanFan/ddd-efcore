using ddd_efcore.OrderProcessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>(); // Store enum as string

        builder.HasOne(builder => builder.Payment)
            .WithOne()
            .HasForeignKey<Payment>(p => p.OrderId);
    }
}