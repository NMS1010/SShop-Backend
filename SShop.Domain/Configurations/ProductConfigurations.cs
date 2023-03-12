using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SShop.Domain.Entities;

namespace SShop.Domain.Configurations
{
    public class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");
            builder.HasKey(p => p.ProductId);
            builder
                .Property(p => p.ProductId)
                .UseIdentityColumn();
            builder
                .Property(p => p.Name)
                .HasMaxLength(255)
                .IsRequired();
            builder
                .Property(p => p.Description)
                .HasMaxLength(int.MaxValue)
                .IsRequired();
            builder
                .Property(p => p.Price)
                .HasColumnType("DECIMAL")
                .IsRequired();
            builder
                .Property(p => p.Quantity)
                .IsRequired();
            builder
                .Property(p => p.BrandId)
                .IsRequired();
            builder
                .Property(p => p.CategoryId)
                .IsRequired();
            builder
                .Property(p => p.DateCreated)
                .IsRequired();
            builder
                .Property(p => p.Origin)
                .HasMaxLength(30)
                .IsRequired();
            builder
                .Property(p => p.Status)
                .IsRequired();

            builder
                .HasOne(x => x.Brand)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasMany(p => p.ReviewItems)
                .WithOne(r => r.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(p => p.CartItems)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(p => p.OrderItems)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(p => p.WishItems)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}