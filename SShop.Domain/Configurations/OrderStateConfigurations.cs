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
    public class OrderStateConfigurations : IEntityTypeConfiguration<OrderState>
    {
        public void Configure(EntityTypeBuilder<OrderState> builder)
        {
            builder.ToTable(nameof(OrderState));

            builder.HasKey(x => x.OrderStateId);
            builder
                .Property(x => x.OrderStateId)
                .UseIdentityColumn();

            builder
                .Property(x => x.OrderStateName)
                .HasMaxLength(100)
                .IsRequired();

            builder
                .HasMany(x => x.Orders)
                .WithOne(x => x.OrderState)
                .HasForeignKey(x => x.OrderStateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}