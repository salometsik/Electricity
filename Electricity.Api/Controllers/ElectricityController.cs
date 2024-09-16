using Electricity.Application.Features.ElectricityUsage.Commands;
using Electricity.Application.Features.ElectricityUsage.Queries.GetElectricityUsageList;
using Electricity.Application.Models.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Electricity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectricityController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<GetElectricityUsageListQueryResponse> GetElectricityUsageList()
            => await _mediator.Send(new GetElectricityUsageQuery());

        [HttpPost("data")]
        public async Task<BaseResponse> GetData() => await _mediator.Send(new StoreElectricityDataCommand());

    }
}
