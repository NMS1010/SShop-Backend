using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.Domain.Configurations
{
    public class PaymentMethodConfigurations : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable(nameof(PaymentMethod));

            builder.HasKey(x => x.PaymentMethodId);
            builder
                .Property(x => x.PaymentMethodId)
                .UseIdentityColumn();

            builder
                .Property(x => x.PaymentMethodName)
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(x => x.Image)
                .IsRequired();
            builder
                .HasMany(x => x.Orders)
                .WithOne(x => x.PaymentMethod)
                .HasForeignKey(x => x.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}