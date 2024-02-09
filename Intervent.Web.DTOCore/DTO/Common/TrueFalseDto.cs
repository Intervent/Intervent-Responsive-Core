namespace Intervent.Web.DTO
{
    public sealed class TrueFalseDto
    {
        public byte Key { get; private set; }

        public string Description { get; private set; }


        private TrueFalseDto(byte key, string description)
        {
            Key = key;
            Description = description;
        }

        public static readonly TrueFalseDto True = new TrueFalseDto(1, "TRUE");
        public static readonly TrueFalseDto False = new TrueFalseDto(2, "FALSE");


        public static IEnumerable<TrueFalseDto> GetAllTypes()
        {
            return new[] { True, False };
        }


        public static TrueFalseDto GetByDescription(string desc)
        {
            return GetAllTypes().FirstOrDefault(x => x.Description.ToLower() == desc.ToLower());
        }
    }
}
