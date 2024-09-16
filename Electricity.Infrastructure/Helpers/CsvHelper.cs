using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Electricity.Application.Models;
using System.Globalization;

namespace Electricity.Infrastructure.Helpers
{
    public class SafeDecimalConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text) || !decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return 0m;

            return result;
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            => value.ToString();

    }

    public class ElectricityDataMap : ClassMap<ElectricityDataModel>
    {
        public ElectricityDataMap()
        {
            Map(m => m.Date).Name("PL_T");
            Map(m => m.Type).Name("OBT_PAVADINIMAS");
            Map(m => m.Tinklas).Name("TINKLAS");
            Map(m => m.ElectricityConsumption).Name("P+").TypeConverter<SafeDecimalConverter>();
        }
    }
}
