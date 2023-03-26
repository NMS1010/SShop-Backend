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
    public class DistrictConfigurations : IEntityTypeConfiguration<District>
    {
        public void Configure(EntityTypeBuilder<District> builder)
        {
            builder.ToTable(nameof(District));
            builder.HasKey(x => x.DistrictId);
            builder.Property(x => x.DistrictId).UseIdentityColumn();
            builder.Property(x => x.DistrictCode).IsRequired();
            builder.Property(x => x.DistrictName).IsRequired();
            builder.HasMany(x => x.Addresses)
                .WithOne(x => x.District)
                .HasForeignKey(x => x.DistrictId);
        }
    }
}