using Electricity.Application.Contracts.Persistence;
using Electricity.Application.Features.ElectricityUsage.DTOs;
using Electricity.Application.Helpers;
using MediatR;

namespace Electricity.Application.Features.ElectricityUsage.Queries.GetElectricityUsageList
{
    public class GetElectricityUsageQueryHandler(IRepository _repository) : IRequestHandler<GetElectricityUsageQuery, GetElectricityUsageListQueryResponse>
    {
        public async Task<GetElectricityUsageListQueryResponse> Handle(GetElectricityUsageQuery request, CancellationToken cancellationToken)
        {
            var response = new GetElectricityUsageListQueryResponse();
            var usages = await _repository.GetAllAsync();
            if (usages.Count == 0)
                return response.HandleNoContent();
            response.ElectricityUsages = usages.Select(i => new ElectricityUsageListDto
            {
                Tinklas = i.Tinklas,
                TotalConsumption = i.TotalConsumption,
                DataCount = i.DataCount,
                //Date = i.Date
            }).ToList();
            return response;
        }
    }
}
