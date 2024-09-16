using Electricity.Application.Contracts.Persistence;
using Electricity.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Electricity.Persistence
{
    public class ElectricityRepository(ElectricityDbContext db) : IRepository
    {
        public async Task<ElectricityUsage> CreateAsync(ElectricityUsage entity)
        {
            await db.Set<ElectricityUsage>().AddAsync(entity);
            await db.SaveChangesAsync();
            return entity;
        }

        public async Task<IReadOnlyList<ElectricityUsage>> GetAllAsync()
            => await db.Set<ElectricityUsage>().ToListAsync();
    }
}
