using Electricity.Application.Features.ElectricityUsage.Queries.GetElectricityUsageList;
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
    }
}
