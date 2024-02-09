using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Intervent.Web.DTO
{
    public class IntuityShipmentCsvClassMap : CsvClassMap<IntuityShipmentCsvModel>
    {
        readonly string dateFormat = "yyyy-MM-dd";
        public IntuityShipmentCsvClassMap()
        {
            Map(m => m.UniqueId).TypeConverter(new UniqueIdConverter());
            Map(m => m.SerialNo);
            Map(m => m.CartridgeQty);
            Map(m => m.DateSent).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.Tracking);
            Map(m => m.EmployeeOrderType);
            Map(m => m.SoNbr);
        }
    }

    public class UniqueIdConverter : DefaultTypeConverter
    {
        public char SplChar = '-';

        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    var valList = text.Split(SplChar);
                    if (valList.Length > 1)
                    {
                        return valList[1];
                    }
                }
            }
            catch
            {
            }
            return text;
        }

        /// <summary>
        /// Determines whether this instance [can convert from] the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can convert from] the specified type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvertFrom(System.Type type)
        {
            // We only care about strings.
            return type == typeof(string);
        }
    }
}
