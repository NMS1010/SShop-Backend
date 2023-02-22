using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SShop.Domain.Entities;

namespace SShop.Domain.Configurations
{
    public class BrandConfigurations : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("Brand");

            builder.HasKey(x => x.BrandId);

            builder
                .Property(x => x.BrandId)
                .UseIdentityColumn();
            builder
                .Property(x => x.BrandName)
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(x => x.Origin)
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(x => x.Image)
                .HasMaxLength(255)
                .IsRequired();
            builder
                .HasMany(x => x.Products)
                .WithOne(x => x.Brand)
                .HasForeignKey(x => x.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}