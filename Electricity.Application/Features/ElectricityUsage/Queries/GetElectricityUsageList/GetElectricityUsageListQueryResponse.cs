using Electricity.Application.Features.ElectricityUsage.DTOs;
using Electricity.Application.Models.Base;

namespace Electricity.Application.Features.ElectricityUsage.Queries.GetElectricityUsageList
{
    public class GetElectricityUsageListQueryResponse : BaseResponse
    {
        public GetElectricityUsageListQueryResponse() : base() { }

        public List<ElectricityUsageListDto> ElectricityUsages { get; set; }
    }
}
