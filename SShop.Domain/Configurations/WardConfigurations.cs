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
    public class WardConfigurations : IEntityTypeConfiguration<Ward>
    {
        public void Configure(EntityTypeBuilder<Ward> builder)
        {
            builder.ToTable(nameof(Ward));
            builder.HasKey(x => x.WardId);
            builder.Property(x => x.WardId).UseIdentityColumn();
            builder.Property(x => x.WardCode).IsRequired();
            builder.Property(x => x.WardName).IsRequired();
            builder.HasMany(x => x.Addresses)
                .WithOne(x => x.Ward)
                .HasForeignKey(x => x.WardId);
        }
    }
}