namespace Electricity.Core.Entities
{
    public class ElectricityUsage
    {
        public Guid Id { get; set; }
        public string Tinklas { get; set; }
        public decimal TotalConsumption { get; set; }
        public int DataCount { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
