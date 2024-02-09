namespace Intervent.Web.DTO
{
    public sealed class ValidatorTypeDto
    {
        public int Id { get; set; }

        public string Description { get; set; }
        private ValidatorTypeDto(int id, string desc)
        {
            Id = id;
            Description = desc;
        }

        public static ValidatorTypeDto None = new ValidatorTypeDto(1, "None");
        public static ValidatorTypeDto Regex = new ValidatorTypeDto(2, "Regex");
        public static ValidatorTypeDto Range = new ValidatorTypeDto(3, "Range");


        static IEnumerable<ValidatorTypeDto> GetAll()
        {
            List<ValidatorTypeDto> lst = new List<ValidatorTypeDto>();
            lst.Add(ValidatorTypeDto.None);
            lst.Add(ValidatorTypeDto.Regex);
            lst.Add(ValidatorTypeDto.Range);

            return lst;
        }

        public static ValidatorTypeDto GetByKey(int id)
        {
            return GetAll().Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
