using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.Domain.Configurations
{
    public class DeliveryMethodConfigurations : IEntityTypeConfiguration<DeliveryMethod>
    {
        public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
        {
            builder.ToTable(nameof(DeliveryMethod));

            builder.HasKey(x => x.DeliveryMethodId);
            builder
                .Property(x => x.DeliveryMethodId)
                .UseIdentityColumn();

            builder
                .Property(x => x.DeliveryMethodName)
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(x => x.Image)
                .IsRequired();
            builder
                .Property(x => x.Price)
                .HasColumnType("DECIMAL")
                .IsRequired();

            builder
                .HasMany(x => x.Orders)
                .WithOne(x => x.DeliveryMethod)
                .HasForeignKey(x => x.DeliveryMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}