using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimDataAnalytics.Eligibility.CodeDto
{
    public interface ITypeSafeEnum
    {
        string Description { get; }
    }
    public sealed class YesNoDto
    {
        public string Key { get; private set; }

        public string Description { get; private set; }


        private YesNoDto(string key, string description)
        {
            Key = key;
            Description = description;
        }

        public static readonly YesNoDto Yes = new YesNoDto("Y", "Yes");
        public static readonly YesNoDto No = new YesNoDto("N", "No");


        public static IEnumerable<YesNoDto> GetAllTypes()
        {
            return new[] { Yes, No };
        }


        public static YesNoDto GetByKey(string key)
        {
            return GetAllTypes().FirstOrDefault(x => x.Key == key);
        }
    }

    public sealed class GenderDto : ITypeSafeEnum
    {
        public byte Key { get; set; }

        public string Description { get; private set; }

        public string CsvDescription { get; private set; }


        private GenderDto(byte key, string description, string csvDescription)
        {
            Key = key;
            Description = description;
            CsvDescription = csvDescription;
        }

        public GenderDto()
        {

        }

        public static readonly GenderDto Male = new GenderDto(1, "Male", "M");
        public static readonly GenderDto Female = new GenderDto(2, "Female", "F");


        public static IEnumerable<GenderDto> GetAllTypes()
        {
            return new[] { Male, Female };
        }


        public static GenderDto GetByKey(int key)
        {
            return GetAllTypes().FirstOrDefault(x => x.Key == key);
        }

        public static GenderDto GetByCsvDescription(string csvDescription)
        {
            return GetAllTypes().FirstOrDefault(x => x.CsvDescription == csvDescription);
        }
    }

    public sealed class EligibilityUserStatusDto : ITypeSafeEnum
    {
        public string Key { get; private set; }//updating the db records with shorthand and using the new format key

        public string Description { get; private set; }

        public string OldFormatKey { get; set; }

        public string NewFormatKey { get; set; }




        private EligibilityUserStatusDto(string key, string description, string oldFormatKey, string newFormatKey)
        {
            Key = key;
            Description = description;
            OldFormatKey = oldFormatKey;
            NewFormatKey = newFormatKey;
        }

        public static readonly EligibilityUserStatusDto Active = new EligibilityUserStatusDto("A", "Active", "A", "A");
        public static readonly EligibilityUserStatusDto Retired = new EligibilityUserStatusDto("R", "Retired", "R", "R");
        public static readonly EligibilityUserStatusDto Terminated = new EligibilityUserStatusDto("T", "Terminated", "T", "T");
        public static readonly EligibilityUserStatusDto LoA = new EligibilityUserStatusDto("L", "LoA", "L", "L");
        public static readonly EligibilityUserStatusDto Dead = new EligibilityUserStatusDto("D", "Dead", "D", "D");

        public static IEnumerable<EligibilityUserStatusDto> GetAllTypes()
        {
            return new[] { Active, Retired, Terminated, LoA, Dead };
        }

        public EligibilityUserStatusDto() { }

        public static EligibilityUserStatusDto GetByKey(string key)
        {
            return GetAllTypes().FirstOrDefault(x => x.Key == key);
        }

        public static EligibilityUserStatusDto GetByOldFormatKey(string key)
        {
            return GetAllTypes().FirstOrDefault(x => x.OldFormatKey == key);
        }

        public static EligibilityUserStatusDto GetByNewFormatKey(string key)
        {
            return GetAllTypes().FirstOrDefault(x => x.NewFormatKey == key);
        }
    }

    public sealed class EligibilityUserEnrollmentTypeDto : ITypeSafeEnum
    {
        public string UserEnrollmentTypeKey { get; set; }

        public string Description { get; set; }

        public string OldFormatKey { get; set; }


        private EligibilityUserEnrollmentTypeDto(string key, string description, string oldFormatKey)
        {
            UserEnrollmentTypeKey = key;
            Description = description;
            OldFormatKey = oldFormatKey;
        }

        public EligibilityUserEnrollmentTypeDto()
        {

        }

        public static readonly EligibilityUserEnrollmentTypeDto Employee = new EligibilityUserEnrollmentTypeDto("E", "Employee", "S");
        public static readonly EligibilityUserEnrollmentTypeDto Spouse = new EligibilityUserEnrollmentTypeDto("S", "Spouse", "SP");
        public static readonly EligibilityUserEnrollmentTypeDto Dependent = new EligibilityUserEnrollmentTypeDto("AD", "Adult Dependent", "AD");
        public static readonly EligibilityUserEnrollmentTypeDto Child = new EligibilityUserEnrollmentTypeDto("C", "Child", "C");

        public static IEnumerable<EligibilityUserEnrollmentTypeDto> GetAllTypes()
        {
            return new[] { Employee, Spouse, Dependent, Child };
        }


        public static EligibilityUserEnrollmentTypeDto GetByKey(string key)
        {
            return GetAllTypes().FirstOrDefault(x => x.UserEnrollmentTypeKey == key);
        }

        public static EligibilityUserEnrollmentTypeDto GetByOldFormatKey(string key)
        {
            return GetAllTypes().FirstOrDefault(x => x.OldFormatKey == key);
        }
    }

    public sealed class EligibilityPayTypeDto : ITypeSafeEnum
    {
        public string Key { get; private set; }

        public string Description { get; private set; }


        EligibilityPayTypeDto(string key, string description)
        {
            Key = key;
            Description = description;
        }

        public EligibilityPayTypeDto()
        {

        }

        public static readonly EligibilityPayTypeDto Hourly = new EligibilityPayTypeDto("H", "Hourly");
        public static readonly EligibilityPayTypeDto Salaried = new EligibilityPayTypeDto("S", "Salaried");


        public static IEnumerable<EligibilityPayTypeDto> GetAllTypes()
        {
            return new[] { Hourly, Salaried };
        }


        public static EligibilityPayTypeDto GetByKey(string key)
        {
            return GetAllTypes().FirstOrDefault(x => x.Key == key);
        }
    }
}
