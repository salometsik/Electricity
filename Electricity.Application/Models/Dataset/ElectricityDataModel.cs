using CsvHelper.Configuration.Attributes;

namespace Electricity.Application.Models
{
    public class ElectricityDataModel
    {
        [Name("PL_T")]
        public DateTime Date { get; set; }

        [Name("OBT_PAVADINIMAS")]
        public string Type { get; set; }

        [Name("TINKLAS")]
        public string Tinklas { get; set; }

        [Name("P+")]
        public decimal ElectricityConsumption { get; set; }
    }
}
