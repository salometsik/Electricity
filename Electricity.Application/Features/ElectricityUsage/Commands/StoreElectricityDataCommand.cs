using Electricity.Application.Models.Base;
using MediatR;

namespace Electricity.Application.Features.ElectricityUsage.Commands
{
    public class StoreElectricityDataCommand : IRequest<BaseResponse>
    {
    }
}
