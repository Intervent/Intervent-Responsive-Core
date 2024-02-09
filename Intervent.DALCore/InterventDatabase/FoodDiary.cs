namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("FoodDiary")]
    public partial class FoodDiary
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        [StringLength(1)]
        public string? MealPlan { get; set; }

        public int MealTypeId { get; set; }

        public int FoodGroupId { get; set; }

        [StringLength(255)]
        public string? Name { get; set; }

        public double? ServingSize { get; set; }

        public double? FatGrams { get; set; }

        public double? CarbGrams { get; set; }

        public double? CarbChoices { get; set; }

        public virtual FoodGroup FoodGroup { get; set; }

        public virtual MealType MealType { get; set; }

        public virtual User User { get; set; }
    }
}
