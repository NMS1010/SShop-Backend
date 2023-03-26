using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SShop.Domain.Configurations
{
    public class ProvinceConfigurations : IEntityTypeConfiguration<Province>
    {
        public void Configure(EntityTypeBuilder<Province> builder)
        {
            builder.ToTable(nameof(Province));
            builder.HasKey(x => x.ProvinceId);
            builder.Property(x => x.ProvinceId).UseIdentityColumn();
            builder.Property(x => x.ProvinceCode).IsRequired();
            builder.Property(x => x.ProvinceName).IsRequired();
            builder.HasMany(x => x.Addresses)
                .WithOne(x => x.Province)
                .HasForeignKey(x => x.ProvinceId);
        }
    }
}