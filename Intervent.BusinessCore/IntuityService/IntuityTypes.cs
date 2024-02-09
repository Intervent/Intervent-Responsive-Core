namespace Intervent.Business
{
    //this maps intuity's enums to our internal value
    public class IntuityEnum
    {

        public static readonly string Primary_Specialty = "PrimaryCare";

        //Refer to EligibilityUserEnrollmentTypeDto for list of possible types
        //Not user anymore
        public static readonly Dictionary<string, string> EligibilityList1 = new Dictionary<string, string>{
            {"self", "E" },
            { "spouse", "S" },
            { "dependent", "AD" },
            { "child", "C" },
        };

        public enum ReminderType
        {
            coaching = 1,
            missed_coaching = 2,
            webinar = 3
        }

        public static readonly List<string> EligibilityList = new List<string>{
            { "E" },
            { "S" },
            { "AD" },
            { "C" },
        };

        public static readonly Dictionary<string, string> StateList = new Dictionary<string, string>{
            {"Newyork", "NY" },
            { "spouse", "S" },
            { "dependent", "AD" },
            { "child", "C" },
        };

        public static readonly Dictionary<string, byte> GenderList = new Dictionary<string, byte>
        {
            {"m", 1},
            {"f", 2 },
        };

        public static readonly Dictionary<string, int> TimeZoneList = new Dictionary<string, int>
        {
            {"America/New_York", 15},
        };

        public enum PhoneType
        {
            Home,
            Cell,
            Work,
        }
        public static readonly Dictionary<string, PhoneType> PhoneList = new Dictionary<string, PhoneType>
        {
            {"cell", PhoneType.Cell},
            {"home", PhoneType.Home },
            {"work", PhoneType.Work },
        };

    }
}
