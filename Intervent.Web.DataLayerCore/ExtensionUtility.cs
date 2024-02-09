namespace Intervent.Web.DataLayer
{
    public static class ExtensionUtility
    {
        public static bool IsTrue(this byte? value)
        {
            if (value.HasValue && value.Value == 1)
                return true;

            return false;
        }

        public static bool IsNullOrTrue(this byte? value)
        {
            if (!value.HasValue || value.Value == 1)
                return true;

            return false;
        }

        public static bool IsFalse(this byte? value)
        {
            if (value.HasValue && value.Value == 2)
                return true;

            return false;
        }

        public static bool IsNullOrFalse(this byte? value)
        {
            if (!value.HasValue || value.Value == 2)
                return true;

            return false;
        }

        public static bool ContainsValue(this byte? value, List<byte> options)
        {
            if (value.HasValue && options.Contains(value.Value))
                return true;

            return false;
        }

        public static bool HasValue(this byte? value, byte option)
        {
            if (value.HasValue && value.Value == option)
                return true;

            return false;
        }

        public static bool GreaterThanOrEqualToYears(this DateTime? value, DateTime compareTo, int year)
        {
            if (value.HasValue)
            {
                int diff = compareTo.Year - value.Value.Year;
                if (compareTo < value.Value.AddYears(diff)) diff--;

                if (diff >= year)
                    return true;
            }
            return false;
        }

        public static bool GreaterThanYears(this DateTime? value, DateTime compareTo, int year)
        {
            if (value.HasValue)
            {
                int diff = compareTo.Year - value.Value.Year;
                if (compareTo < value.Value.AddYears(diff)) diff--;

                if (diff > year)
                    return true;
            }
            return false;
        }

        public static bool LessThanOrEqualToYears(this DateTime? value, DateTime compareTo, int year)
        {
            if (value.HasValue)
            {
                int diff = compareTo.Year - value.Value.Year;
                if (compareTo > value.Value.AddYears(diff)) diff++;

                if (diff <= year)
                    return true;
            }

            return false;
        }

        public static bool LessThanOrEqualToMonths(this DateTime? value, DateTime compareTo, int month)
        {
            if (value.HasValue)
            {

                var diff = (compareTo.Month - value.Value.Month) + 12 * (compareTo.Year - value.Value.Year);
                if (compareTo > value.Value.AddMonths(diff)) diff++;

                if (diff <= month)
                    return true;
            }

            return false;
        }

        public static string AppendFormatedString(this string value, string stringToAppend)
        {
            if (string.IsNullOrEmpty(value))
                return stringToAppend;
            else
                return value + "~" + stringToAppend;
        }

        public static bool GraterThanOrEqualToMonths(this DateTime? value, DateTime compareTo, int month)
        {
            if (value.HasValue)
            {
                var diff = (compareTo.Month - value.Value.Month) + 12 * (compareTo.Year - value.Value.Year);
                if (compareTo < value.Value.AddMonths(diff)) diff--;

                if (diff >= month)
                    return true;
            }
            return false;
        }

        public static int GetAge(DateTime birthday, DateTime now)
        {
            int age = now.Year - birthday.Year;
            if (now < birthday.AddYears(age)) age--;

            return age;
        }

        public static int DiffInYears(DateTime endDate, DateTime startDate)
        {

            return (endDate.Year - startDate.Year - 1) +
        (((endDate.Month > startDate.Month) ||
        ((endDate.Month == startDate.Month) && (endDate.Day >= startDate.Day))) ? 1 : 0);

        }
    }
}
