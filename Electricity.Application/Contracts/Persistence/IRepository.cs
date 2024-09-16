using Electricity.Core.Entities;

namespace Electricity.Application.Contracts.Persistence
{
    public interface IRepository
    {
        Task<bool> CreateRangeAsync(List<ElectricityUsage> entity);
        Task<IReadOnlyList<ElectricityUsage>> GetAllAsync();
    }
}
