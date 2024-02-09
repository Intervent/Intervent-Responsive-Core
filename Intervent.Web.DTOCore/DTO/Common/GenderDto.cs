namespace Intervent.Web.DTO
{
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

        public static GenderDto GetByDescription(string Description)
        {
            return GetAllTypes().FirstOrDefault(x => x.Description.ToLower() == Description.ToLower());
        }
    }
}
