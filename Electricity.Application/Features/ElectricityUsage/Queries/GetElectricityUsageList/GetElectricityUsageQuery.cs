using MediatR;

namespace Electricity.Application.Features.ElectricityUsage.Queries.GetElectricityUsageList
{
    public class GetElectricityUsageQuery : IRequest<GetElectricityUsageListQueryResponse>
    {
    }
}
