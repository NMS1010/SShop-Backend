using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SShop.Domain.Entities;

namespace SShop.Domain.Configurations
{
    public class ReviewItemConfigurations : IEntityTypeConfiguration<ReviewItem>
    {
        public void Configure(EntityTypeBuilder<ReviewItem> builder)
        {
            builder.ToTable("Review");

            builder
                .HasKey(x => x.ReviewItemId);
            builder
                .Property(x => x.ReviewItemId)
                .UseIdentityColumn();
            builder
                .Property(x => x.DateCreated)
                .IsRequired();
            builder
                .Property(x => x.DateUpdated)
                .IsRequired();
            builder
                .Property(x => x.UserId)
                .IsRequired();
            builder
                .Property(x => x.ProductId)
                .IsRequired();
            builder
                .Property(x => x.Status)
                .IsRequired();
            builder
                .Property(x => x.Rating)
                .IsRequired();
            builder
                .Property(x => x.Content)
                .HasMaxLength(255)
                .IsRequired();
            builder
                .HasOne(x => x.Product)
                .WithMany(x => x.ReviewItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.ReviewItems)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}