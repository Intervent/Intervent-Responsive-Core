using CsvHelper.TypeConversion;
using System.ComponentModel;
using System.Reflection;

namespace Intervent.Web.DTO
{

    public class EnumConverter : DefaultTypeConverter
    {
        public System.Type enumtype;

        public string notSupported = "NotSupported";
        public EnumConverter(System.Type t)
        {
            enumtype = t;
        }

        public static string GetDescription(Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description
                ?? value.ToString();
        }

        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            if (value == null)
            {
                return "";
            }
            return GetDescription((Enum)value);

        }

        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            return default(T);
        }

        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="options">The options to use when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            try
            {
                if (enumtype.ToString().ToLower().Contains("eligibilitystatus"))
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        return GetValueFromDescription<EligibilityStatus>(text);
                    }
                    else
                    {
                        return default(EligibilityStatus);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        return GetValueFromDescription<EligibilityReason>(text);
                    }
                    else
                    {
                        return default(EligibilityReason);
                    }
                }
            }
            catch
            {
            }
            return Enum.Parse(enumtype, notSupported, true);
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
