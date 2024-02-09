namespace Intervent.Web.DTO
{
    public sealed class QuestionCategoryDto
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }
        private QuestionCategoryDto(int id, string desc)
        {
            Id = id;
            CategoryName = desc;
        }

        public static QuestionCategoryDto HealthHistory = new QuestionCategoryDto(1, "Health History");
        public static QuestionCategoryDto LifestyleHabits = new QuestionCategoryDto(2, "Lifestyle Habits and Risk Factors");
        public static QuestionCategoryDto HealthSafety = new QuestionCategoryDto(3, "Health, Safety and Productivity");
        public static QuestionCategoryDto PreventiveExamsOrImmunizations = new QuestionCategoryDto(4, "Preventive Exams/Immunizations");
        public static QuestionCategoryDto InterestandMotivation = new QuestionCategoryDto(5, "Interest and Motivation");
        public static QuestionCategoryDto HealthMeasurements = new QuestionCategoryDto(6, "Health Measurements");
        public static QuestionCategoryDto UserProfile = new QuestionCategoryDto(7, "User Profile");

        public static IEnumerable<QuestionCategoryDto> GetHRA()
        {
            List<QuestionCategoryDto> lst = new List<QuestionCategoryDto>();
            lst.Add(QuestionCategoryDto.HealthHistory);
            lst.Add(QuestionCategoryDto.LifestyleHabits);
            lst.Add(QuestionCategoryDto.HealthSafety);
            lst.Add(QuestionCategoryDto.PreventiveExamsOrImmunizations);
            lst.Add(QuestionCategoryDto.InterestandMotivation);
            lst.Add(QuestionCategoryDto.HealthMeasurements);
            return lst;
        }
        public static IEnumerable<QuestionCategoryDto> GetProfile()
        {
            List<QuestionCategoryDto> lst = new List<QuestionCategoryDto>();
            lst.Add(QuestionCategoryDto.UserProfile);
            return lst;
        }


        public static QuestionCategoryDto GetByKey(int id)
        {
            return GetHRA().Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
