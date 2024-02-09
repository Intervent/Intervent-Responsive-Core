using CsvHelper.Configuration;

namespace Intervent.Web.DTO
{
    public sealed class GlucometerCsvClassMap : CsvClassMap<GlucometerCsvModel>
    {
        readonly string dateFormat = "yyyyMMdd";
        public GlucometerCsvClassMap()
        {
            Map(m => m.client_uid);
            Map(m => m.registeredDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.activationDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
        }
    }
}
