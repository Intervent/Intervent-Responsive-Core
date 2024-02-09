using CsvHelper.TypeConversion;

namespace Intervent.Web.DTO.DTO.Claims.CustomConverter
{
    public class EbenCustomConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            if (text == "SPOUSE")
                return ClaimsRelationshipToSubscriberCode.SPOUSE;
            else if (text == "INSURED")
                return ClaimsRelationshipToSubscriberCode.EMPLOYEE;
            else if (text == "CHILD")
                return ClaimsRelationshipToSubscriberCode.CHILDREN;
            else
                return null;
        }

        public override bool CanConvertFrom(System.Type type)
        {
            return type == typeof(string);
        }
    }
}
