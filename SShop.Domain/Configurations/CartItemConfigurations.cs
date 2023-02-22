using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SShop.Domain.Entities;

namespace SShop.Domain.Configurations
{
    public class CartItemConfigurations : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItem");

            builder.HasKey(x => x.CartItemId);

            builder
                .Property(x => x.CartItemId)
                .UseIdentityColumn();

            builder
                .Property(x => x.UserId)
                .IsRequired();
            builder
                .Property(x => x.ProductId)
                .IsRequired();
            builder
                .Property(x => x.Quantity)
                .IsRequired();

            builder
                .Property(x => x.DateAdded)
                .IsRequired();
            builder
                .Property(x => x.Status)
                .IsRequired();
            builder
                .HasOne(x => x.Product)
                .WithMany(x => x.CartItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.CartItems)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}