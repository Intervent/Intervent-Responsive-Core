namespace Intervent.Web.DTO
{
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
        public static readonly EligibilityUserEnrollmentTypeDto Patient = new EligibilityUserEnrollmentTypeDto("P", "Patient", "P");

        public static IEnumerable<EligibilityUserEnrollmentTypeDto> GetAllTypes()
        {
            return new[] { Employee, Spouse, Dependent, Child, Patient };
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
}
