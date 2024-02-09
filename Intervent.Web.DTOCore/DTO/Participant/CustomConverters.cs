using CsvHelper;
using CsvHelper.TypeConversion;
using System.Text.RegularExpressions;

namespace Intervent.Web.DTO.CustomCsvConverter
{
    public class UserEnrollmentTypeConverter : DefaultTypeConverter
    {
        bool _isOldFormat = false;

        public UserEnrollmentTypeConverter()
        {

        }

        public UserEnrollmentTypeConverter(bool isOldFormat)
        {
            _isOldFormat = isOldFormat;
        }
        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            EligibilityUserEnrollmentTypeDto userEnrollmentType = null;
            if (_isOldFormat)
            {
                userEnrollmentType = EligibilityUserEnrollmentTypeDto.GetByOldFormatKey(text);
            }
            else
            {
                userEnrollmentType = EligibilityUserEnrollmentTypeDto.GetByKey(text);
            }
            return userEnrollmentType;
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

    public class EligibilityUserDiabetesTypeConverter : DefaultTypeConverter
    {
        public EligibilityUserDiabetesTypeConverter()
        {
        }

        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            EligibilityUserDiabetesTypeDto diabetesType = null;
            diabetesType = EligibilityUserDiabetesTypeDto.GetByDescription(text);
            if (diabetesType == null)
            {
                throw new CsvBadDataException("Bad data for User Diabetes Type");
            }
            else
                return diabetesType;
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

    public class GenderConverter : DefaultTypeConverter
    {
        bool byDesc = false;
        public GenderConverter()
        {

        }
        public GenderConverter(bool isIntuity = false)
        {
            byDesc = isIntuity;
        }

        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            if (byDesc)
                return GenderDto.GetByDescription(text);
            else
                return GenderDto.GetByCsvDescription(text);
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

        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            if (value == null)
            {
                return "";
            }
            var gender = (GenderDto)value;
            return gender.CsvDescription;
        }
    }

    public class YesNoConverter : DefaultTypeConverter
    {
        bool byDesc = false;
        public YesNoConverter()
        {

        }
        public YesNoConverter(bool isCaptiva = false)
        {
            byDesc = isCaptiva;
        }
        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            if (byDesc)
                return YesNoDto.GetByDescription(text);
            else
                return YesNoDto.GetByKey(text);
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

    public class TrueFalseConverter : DefaultTypeConverter
    {
        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            return TrueFalseDto.GetByDescription(text);

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

    public class PayTypeConverter : DefaultTypeConverter
    {
        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            return EligibilityPayTypeDto.GetByKey(text);

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

    public class EligibilityStatusConverter : DefaultTypeConverter
    {
        bool _isOldFormat = false;

        public EligibilityStatusConverter()
        {

        }

        public EligibilityStatusConverter(bool isOldFormat)
        {
            _isOldFormat = isOldFormat;
        }
        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            EligibilityUserStatusDto status = null;
            if (_isOldFormat)
            {
                status = EligibilityUserStatusDto.GetByOldFormatKey(text);
            }
            else
            {
                if (String.IsNullOrEmpty(text))
                    return EligibilityUserStatusDto.Active;
                status = EligibilityUserStatusDto.GetByNewFormatKey(text);
            }

            if (status == null)
            {
                throw new CsvBadDataException("Bad data for User Status");
            }
            return status;

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

    public class EligibilityDidYouFastConverter : DefaultTypeConverter
    {

        public EligibilityDidYouFastConverter()
        {

        }

        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            return DidYouFastDto.GetByDescription(text);
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

    public class EligibilityLMCUniqueIdConverter : DefaultTypeConverter
    {

        public EligibilityLMCUniqueIdConverter()
        {

        }

        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            return Regex.Replace(text, @"[A-Za-z]+", "");
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

    public class VisitTypeConverter : DefaultTypeConverter
    {
        public VisitTypeConverter()
        {
        }
        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            VisitTypeDto visitType = null;
            visitType = VisitTypeDto.GetByDescription(text);
            if (visitType == null)
            {
                throw new CsvBadDataException("Bad data for Visit Type");
            }
            else
                return visitType;
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
