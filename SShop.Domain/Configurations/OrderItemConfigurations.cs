using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SShop.Domain.Entities;

namespace SShop.Domain.Configurations
{
    public class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItem");

            builder
                .HasKey(x => x.OrderItemId);
            builder
                .Property(x => x.OrderItemId)
                .UseIdentityColumn();
            builder
                .Property(x => x.UnitPrice)
                .HasColumnType("DECIMAL")
                .IsRequired();
            builder
                .Property(x => x.TotalPrice)
                .HasColumnType("DECIMAL")
                .IsRequired();
            builder
                .Property(x => x.ReviewItemId)
                .IsRequired(false);
            builder
                .Property(x => x.Quantity)
                .IsRequired();
            builder
                .Property(x => x.OrderId)
                .IsRequired();
            builder
                .Property(x => x.ProductId)
                .IsRequired();
            builder
                .HasOne(x => x.Order)
                .WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Product)
                .WithMany(x => x.OrderItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasOne(x => x.ReviewItem)
                .WithOne(x => x.OrderItem)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}