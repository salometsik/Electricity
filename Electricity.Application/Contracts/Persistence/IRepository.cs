using Electricity.Core.Entities;

namespace Electricity.Application.Contracts.Persistence
{
    public interface IRepository
    {
        Task<ElectricityUsage> CreateAsync(ElectricityUsage entity);
        Task<IReadOnlyList<ElectricityUsage>> GetAllAsync();
    }
}
