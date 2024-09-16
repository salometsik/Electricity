namespace Electricity.Application.Features.ElectricityUsage.DTOs
{
    public class ElectricityUsageListDto
    {
        public string Tinklas { get; set; }
        public decimal TotalConsumption { get; set; }
        public int DataCount { get; set; }
        public DateTime Date { get; set; }
    }
}
