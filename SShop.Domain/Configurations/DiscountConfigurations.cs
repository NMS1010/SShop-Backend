using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SShop.Domain.Entities;

namespace SShop.Domain.Configurations
{
    public class DiscountConfigurations : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.ToTable("Discount");

            builder
                .HasKey(x => x.DiscountId);
            builder
                .Property(x => x.DiscountId)
                .UseIdentityColumn();
            builder
                .HasIndex(x => x.DiscountCode)
                .IsUnique();

            builder
                .Property(x => x.DiscountCode)
                .HasMaxLength(20)
                .IsRequired();
            builder
                .Property(x => x.DiscountValue)
                .HasColumnType("DECIMAL(10,5)")
                .IsRequired();
            builder
                .Property(x => x.Quantity)
                .IsRequired();
            builder
                .Property(x => x.StartDate)
                .IsRequired();
            builder
                .Property(x => x.EndDate)
                .IsRequired();
            builder
                .Property(x => x.Status)
                .IsRequired();
            builder
                .HasMany(x => x.Orders)
                .WithOne(x => x.Discount)
                .HasForeignKey(x => x.DiscountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}