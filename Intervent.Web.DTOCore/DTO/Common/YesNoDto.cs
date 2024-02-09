namespace Intervent.Web.DTO
{
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

        public static YesNoDto GetByDescription(string Description)
        {
            return GetAllTypes().FirstOrDefault(x => x.Description.ToLower() == Description.ToLower());
        }
    }
}
