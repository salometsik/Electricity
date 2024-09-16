using Electricity.Application.Contracts.Persistence;
using Electricity.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Electricity.Persistence
{
    public class ElectricityRepository(ElectricityDbContext db) : IRepository
    {
        public async Task<bool> CreateRangeAsync(List<ElectricityUsage> entity)
        {
            await db.Set<ElectricityUsage>().AddRangeAsync(entity);
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<IReadOnlyList<ElectricityUsage>> GetAllAsync()
            => await db.Set<ElectricityUsage>().ToListAsync();
    }
}
