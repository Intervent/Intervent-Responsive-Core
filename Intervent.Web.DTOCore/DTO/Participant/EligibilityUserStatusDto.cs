namespace Intervent.Web.DTO
{
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
        public static readonly EligibilityUserStatusDto Cobra = new EligibilityUserStatusDto("C", "Cobra", "C", "C");

        public static IEnumerable<EligibilityUserStatusDto> GetAllTypes()
        {
            return new[] { Active, Retired, Terminated, LoA, Cobra };
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
}
