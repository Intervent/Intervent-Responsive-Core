namespace Intervent.Web.DTO
{
    public class FoodDiaryDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public System.DateTime Date { get; set; }

        public string MealPlan { get; set; }

        public int MealTypeId { get; set; }

        public int FoodGroupId { get; set; }

        public double? ServingSize { get; set; }

        public double? FatGrams { get; set; }

        public double? CarbGrams { get; set; }

        public double? CarbChoices { get; set; }

        public FoodGroupDto FoodGroup { get; set; }

        public MealTypeDto MealType { get; set; }

        public UserDto User { get; set; }
    }
}
