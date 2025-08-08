using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ddd_efcore.OrderProcessing.config
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.PaidOn)
                .IsRequired();

            builder.HasOne<Order>()
                .WithOne(o => o.Payment)
                .IsRequired()
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
