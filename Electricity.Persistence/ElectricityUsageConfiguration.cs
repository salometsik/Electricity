using Electricity.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electricity.Persistence
{
    internal class ElectricityUsageConfiguration : IEntityTypeConfiguration<ElectricityUsage>
    {
        public void Configure(EntityTypeBuilder<ElectricityUsage> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
