using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sales.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Infrastructure.Configuration
{
    public class ScheduledJobConfiguration : IEntityTypeConfiguration<ScheduledJob>
    {
        public void Configure(EntityTypeBuilder<ScheduledJob> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.JobType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p=>p.Status)
                .IsRequired()
                .HasMaxLength(10);

        }
    }
}
