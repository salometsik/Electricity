using Electricity.Application.Models;

namespace Electricity.Application.Contracts.Infrastructure
{
    public interface IRetrieveDataService
    {
        Task<List<ElectricityDataModel>> GetElectricityData();
    }
}
