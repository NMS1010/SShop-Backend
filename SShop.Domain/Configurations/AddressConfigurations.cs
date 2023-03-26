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
    public class AddressConfigurations : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable(nameof(Address));
            builder.HasKey(x => x.AddressId);
            builder.Property(x => x.AddressId).UseIdentityColumn();
            builder.Property(x => x.FirstName)
                .HasMaxLength(200)
                .IsRequired();
            builder.Property(x => x.LastName)
                .HasMaxLength(200)
                .IsRequired();
            builder.Property(x => x.SpecificAddress)
                .HasMaxLength(200)
                .IsRequired();
            builder.Property(x => x.Phone)
                .HasMaxLength(200)
                .IsRequired();
            builder.Property(x => x.IsDefault)
                .IsRequired();
            builder.HasOne(x => x.Province)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.ProvinceId);

            builder.HasOne(x => x.District)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.DistrictId);

            builder.HasOne(x => x.Ward)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.WardId);
            builder.HasOne(x => x.User)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.UserId);
            builder.HasMany(x => x.Orders)
                .WithOne(x => x.Address)
                .HasForeignKey(x => x.AddressId);
        }
    }
}