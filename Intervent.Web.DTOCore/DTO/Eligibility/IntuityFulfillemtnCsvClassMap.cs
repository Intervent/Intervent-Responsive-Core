using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Intervent.Web.DTO
{
    public class IntuityFulfillemtnCsvClassMap : CsvClassMap<IntuityFulfillmentCsvModel>
    {
        const string dateFormat = "MM/dd/yyyy";
        public IntuityFulfillemtnCsvClassMap()
        {
            Map(m => m.UniqueId);
            Map(m => m.FirstName);
            Map(m => m.MiddleName);
            Map(m => m.LastName);
            Map(m => m.Email);
            Map(m => m.PhoneNumber);
            Map(m => m.Address1);
            Map(m => m.Address2);
            Map(m => m.City);
            Map(m => m.ZipOrPostalCode);
            Map(m => m.Country);
            Map(m => m.StateOrProvince);
            Map(m => m.CartridgeQty);
            Map(m => m.IntuityEligibilityId).Ignore();
            Map(m => m.SerialNo).Ignore();
            Map(m => m.RefillRequestDate).Ignore();
            Map(m => m.SendMeter).TypeConverter(new BoolCharConverter());

        }
    }

    public sealed class BoolCharConverter : DefaultTypeConverter
    {

        public BoolCharConverter()
        { }

        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            if (value == null)
            {
                return "";
            }
            var b = (bool)value;
            if (b)
            {
                return "Y";
            }
            else
            {
                return "N";
            }
        }
    }
}
