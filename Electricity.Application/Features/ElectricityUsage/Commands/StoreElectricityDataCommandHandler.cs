using Electricity.Application.Contracts.Infrastructure;
using Electricity.Application.Contracts.Persistence;
using Electricity.Application.Helpers;
using Electricity.Application.Models.Base;
using MediatR;

namespace Electricity.Application.Features.ElectricityUsage.Commands
{
    public class StoreElectricityDataCommandHandler(IRetrieveDataService _service, IRepository _repository)
        : IRequestHandler<StoreElectricityDataCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(StoreElectricityDataCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            var csvFiles = await _service.GetElectricityData();

            if (csvFiles == null || (csvFiles != null && csvFiles.Count == 0))
                return response.HandleNoContent();

            var groupedData = csvFiles
                            .GroupBy(d => d.Tinklas)
                            .Select(g => new Core.Entities.ElectricityUsage
                            {
                                Tinklas = g.Key,
                                TotalConsumption = g.Sum(d => d.ElectricityConsumption),
                                DataCount = g.Count()
                            })
                            .ToList();

            var success = await _repository.CreateRangeAsync(groupedData);
            if (!success)
                return response.HandleException();
            return response;
        }
    }
}
