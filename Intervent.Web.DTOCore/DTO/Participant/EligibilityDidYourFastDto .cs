namespace Intervent.Web.DTO
{
    public sealed class DidYouFastDto : ITypeSafeEnum
    {
        public byte UserDidYouFastKey { get; set; }

        public string Description { get; private set; }


        DidYouFastDto(byte key, string description)
        {
            UserDidYouFastKey = key;
            Description = description;
        }

        public DidYouFastDto()
        {

        }

        public static readonly DidYouFastDto Fasting = new DidYouFastDto(1, "Fasting");
        public static readonly DidYouFastDto NonFasting = new DidYouFastDto(2, "Non fasting");


        public static IEnumerable<DidYouFastDto> GetAllTypes()
        {
            return new[] { Fasting, NonFasting };
        }


        public static DidYouFastDto GetByKey(byte key)
        {
            return GetAllTypes().FirstOrDefault(x => x.UserDidYouFastKey == key);
        }

        public static DidYouFastDto GetByDescription(string desc)
        {
            return GetAllTypes().FirstOrDefault(x => x.Description == desc);
        }
    }
}
