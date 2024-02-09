namespace Intervent.Web.DTO
{
    public sealed class EligibilityUserDiabetesTypeDto : ITypeSafeEnum
    {
        public byte UserDiabetesTypeKey { get; set; }

        public string Description { get; set; }


        private EligibilityUserDiabetesTypeDto(byte key, string description)
        {
            UserDiabetesTypeKey = key;
            Description = description;
        }

        public EligibilityUserDiabetesTypeDto()
        {

        }

        public static readonly EligibilityUserDiabetesTypeDto Type1 = new EligibilityUserDiabetesTypeDto(1, "Type 1 diabetes");
        public static readonly EligibilityUserDiabetesTypeDto Type2 = new EligibilityUserDiabetesTypeDto(2, "Type 2 diabetes");
        public static readonly EligibilityUserDiabetesTypeDto Prediabetes = new EligibilityUserDiabetesTypeDto(3, "Prediabetes");
        public static readonly EligibilityUserDiabetesTypeDto Gestdiabetes = new EligibilityUserDiabetesTypeDto(4, "Gestational diabetes");
        public static readonly EligibilityUserDiabetesTypeDto None = new EligibilityUserDiabetesTypeDto(5, "");

        public static IEnumerable<EligibilityUserDiabetesTypeDto> GetAllTypes()
        {
            return new[] { Type1, Type2, Prediabetes, Gestdiabetes, None };
        }


        public static EligibilityUserDiabetesTypeDto GetByDescription(string desc)
        {
            return GetAllTypes().FirstOrDefault(x => x.Description == desc);
        }

        public static EligibilityUserDiabetesTypeDto GetByKey(byte key)
        {
            return GetAllTypes().FirstOrDefault(x => x.UserDiabetesTypeKey == key);
        }
    }
}
