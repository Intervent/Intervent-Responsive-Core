using ClaimDataAnalytics.Claims.Model;
using CsvHelper;
using CsvHelper.TypeConversion;
using System;

namespace ClaimDataAnalytics.Claims.Converters.CVSCaremark
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
            if (text == "01")
                return ClaimsRelationshipToSubscriberCode.SPOUSE;
            else if (text == "18")
                return ClaimsRelationshipToSubscriberCode.EMPLOYEE;
            else
                return null;
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

    public class UniqueIDConverter : DefaultTypeConverter
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
            if (text == "01")
                return ClaimsRelationshipToSubscriberCode.SPOUSE;
            else if (text == "18")
                return ClaimsRelationshipToSubscriberCode.EMPLOYEE;
            else
                return null;
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

    public class AmountConverter : DefaultTypeConverter
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
                return default(decimal?);
            int lastCharacterASCIIValue = (int)text.ToUpper()[text.Length - 1];
            if (text.EndsWith("}"))
            {
                var amt = (Convert.ToDecimal(text.Substring(0, text.Length - 1)) * 10) / 100;
                return -amt;
            }
            else if (text.EndsWith("{"))
            {
                var amt = (Convert.ToDecimal(text.Substring(0, text.Length - 1)) * 10) / 100;
                return amt;
            }
            else if (lastCharacterASCIIValue >= 74 && lastCharacterASCIIValue <= 82)
            {
                var amt = Convert.ToDecimal(text.Substring(0, text.Length - 1) + ((char)(lastCharacterASCIIValue - 25))) / 100;
                return -amt;
            }
            else if (lastCharacterASCIIValue >= 65 && lastCharacterASCIIValue <= 73)
            {
                var amt = Convert.ToDecimal(text.Substring(0, text.Length - 1) + ((char)(lastCharacterASCIIValue - 16))) / 100;
                return amt;
            }
            else if (lastCharacterASCIIValue >= 48 && lastCharacterASCIIValue <= 57)
            {
                var amt = Convert.ToDecimal(text) / 100;
                return amt;
            }
            else
            {
                throw new CsvHelperException($"Cannot convert {text} to decimal");
            }
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