namespace Intervent.Web.DTO
{
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
