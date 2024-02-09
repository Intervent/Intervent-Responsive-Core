using CsvHelper.TypeConversion;

namespace Intervent.Web.DTO.DTO.Claims.CustomConverter.BCBS
{
    public class RelationshipToSubscriberCodeConverter : DefaultTypeConverter
    {
        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;
            if (text == "SPS")
                return ClaimsRelationshipToSubscriberCode.SPOUSE;
            else if (text == "SUB")
                return ClaimsRelationshipToSubscriberCode.EMPLOYEE;
            else if (text == "CHD")
                return ClaimsRelationshipToSubscriberCode.CHILDREN;
            else
                return null;
            // return !String.IsNullOrEmpty(text) && text == "SPS";

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
