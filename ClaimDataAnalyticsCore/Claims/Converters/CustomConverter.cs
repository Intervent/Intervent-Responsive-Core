using CsvHelper;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimDataAnalytics.Claims.Converters
{
    public sealed class DateFormatConverter : DefaultTypeConverter
    {
        public string SourceDateFormat { get; set; }

        public DateFormatConverter(string sourceDateFormat)
        {
            SourceDateFormat = sourceDateFormat;
        }

        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            if (String.IsNullOrEmpty(text))
                return default(DateTime?);
            else
            {
                if (String.IsNullOrEmpty(SourceDateFormat))
                {
                    throw new CsvHelperException("date format converter requires source format");
                }

                return DateTime.ParseExact(text, SourceDateFormat, CultureInfo.InvariantCulture);
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

    public sealed class DateToStringFormatConverter : DefaultTypeConverter
    {
        public string TargetDateFormat { get; set; }

        public DateToStringFormatConverter(string targetDateFormat)
        {
            TargetDateFormat = targetDateFormat;
        }

        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            if (value == null)
                return "";
            else
                return ((DateTime)value).ToString(TargetDateFormat);
        }
    }

    //public sealed class ClaimConditionConverter : DefaultTypeConverter
    //{
    //    public ClaimConditionConverter()
    //    {
    //    }

    //    public override string ConvertToString(TypeConverterOptions options, object value)
    //    {
    //        if (value == null)
    //            return "";
    //        else
    //        {
    //            IEnumerable<ClaimsProcessClaimCodeCondition> conditions = (IEnumerable<ClaimsProcessClaimCodeCondition>)value;
    //            if (conditions.Count() > 0)
    //            {
    //                string cond = "";
    //                foreach (var c in conditions)
    //                {
    //                    cond += c.ToString() + ";";
    //                }
    //                return cond;
    //            }
    //            else
    //                return "";
    //        }
    //    }
    //    /// <summary>
    //    /// Converts the string to an object.
    //    /// </summary>
    //    /// <param name="options">The options to use when converting.</param>
    //    /// <param name="text">The string to convert to an object.</param>
    //    /// <returns>The object created from the string.</returns>
    //    public override object ConvertFromString(TypeConverterOptions options, string text)
    //    {
    //        List<ClaimsProcessClaimCodeCondition> conditions = new List<ClaimsProcessClaimCodeCondition>();
    //        if (!String.IsNullOrEmpty(text))
    //        {
    //            foreach (var cond in text.Split(';'))
    //            {
    //                if (!String.IsNullOrEmpty(cond))
    //                {
    //                    conditions.Add((ClaimsProcessClaimCodeCondition)Enum.Parse(typeof(ClaimsProcessClaimCodeCondition), cond));
    //                }
    //            }
    //        }

    //        return conditions;
    //    }

    //    public override bool CanConvertFrom(System.Type type)
    //    {
    //        // We only care about strings.
    //        return type == typeof(string);
    //    }
    //}

    public sealed class ClaimCodeConverter : DefaultTypeConverter
    {
        public ClaimCodeConverter()
        {
        }

        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            if (value == null)
                return "";
            else
            {
                var conditions = (IEnumerable<string>)value;
                if (conditions.Count() > 0)
                {
                    string cond = "";
                    foreach (var c in conditions)
                    {
                        cond += c.ToString().Replace(",", " ") + ";";
                    }
                    return cond;
                }
                else
                    return "";
            }
        }
        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            List<string> conditions = new List<string>();
            if (!String.IsNullOrEmpty(text))
            {
                foreach (var cond in text.Split(';'))
                {
                    if (!String.IsNullOrEmpty(cond))
                    {
                        conditions.Add(cond);
                    }
                }
            }

            return conditions;
        }

        public override bool CanConvertFrom(System.Type type)
        {
            // We only care about strings.
            return type == typeof(string);
        }
    }

}