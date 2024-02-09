namespace Intervent.Web.DTO
{
    public sealed class VisitTypeDto : ITypeSafeEnum
    {
        public byte Key { get; private set; }

        public string Description { get; private set; }


        VisitTypeDto(byte key, string description)
        {
            Key = key;
            Description = description;
        }

        public VisitTypeDto()
        {

        }

        public static readonly VisitTypeDto Baseline = new VisitTypeDto(0, "Baseline");
        public static readonly VisitTypeDto First = new VisitTypeDto(1, "1 Month");
        public static readonly VisitTypeDto Fourth = new VisitTypeDto(4, "4 Month");
        public static readonly VisitTypeDto Eighth = new VisitTypeDto(8, "8 Month");
        public static readonly VisitTypeDto Twelfth = new VisitTypeDto(12, "12 Month");


        public static IEnumerable<VisitTypeDto> GetAllTypes()
        {
            return new[] { Baseline, First, Fourth, Eighth, Twelfth };
        }

        public static VisitTypeDto GetByDescription(string desc)
        {
            return GetAllTypes().FirstOrDefault(x => x.Description == desc);
        }

        public static VisitTypeDto GetByKey(byte key)
        {
            return GetAllTypes().FirstOrDefault(x => x.Key == key);
        }
    }
}