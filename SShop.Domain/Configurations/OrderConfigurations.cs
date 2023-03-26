using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SShop.Domain.Entities;

namespace SShop.Domain.Configurations
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Order");

            builder.HasKey(x => x.OrderId);
            builder
                .Property(x => x.OrderId)
                .UseIdentityColumn();
            builder
                .Property(x => x.Shipping)
                .HasColumnType("DECIMAL")
                .IsRequired();
            builder
                .Property(x => x.TotalPrice)
                .HasColumnType("DECIMAL")
                .IsRequired();
            builder
                .Property(x => x.TotalItemPrice)
                .HasColumnType("DECIMAL")
                .IsRequired();
            builder.Property(x => x.UserId)
                .IsRequired();
            builder.Property(x => x.DiscountId)
                .IsRequired(false);
            builder
                .Property(x => x.DateCreated)
                .IsRequired();
            builder
                .Property(x => x.DateDone)
                .IsRequired(false);
            builder
                .Property(x => x.Status)
                .IsRequired();
            builder
                .Property(x => x.Payment)
                .IsRequired();
            builder
                .HasOne(x => x.Discount)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.DiscountId)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasOne(x => x.User)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasOne(x => x.Address)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.AddressId)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasMany(x => x.OrderItems)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}