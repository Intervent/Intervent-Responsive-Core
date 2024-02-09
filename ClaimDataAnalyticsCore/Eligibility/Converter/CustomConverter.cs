using ClaimDataAnalytics.Eligibility.CodeDto;
using CsvHelper;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimDataAnalytics.Eligibility.Converter
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
            if (userEnrollmentType == null)
            {
                throw new CsvBadDataException("Bad data for User Enrollment Type");
            }
            else
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

    public class GenderConverter : DefaultTypeConverter
    {
        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
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
    }

    public class YesNoConverter : DefaultTypeConverter
    {
        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
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
}
