using Identity.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Configuration
{
    public class IdentityUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Configure the table name and primary key for IdentityUser
            builder.ToTable("AspNetUsers");
            builder.HasKey(x => x.Id);

            // You can add more custom configurations for ApplicationUser here
            builder.Property(x => x.FullName).HasMaxLength(250);
            builder.Property(x => x.Address).HasMaxLength(500);
        }
    }
}
