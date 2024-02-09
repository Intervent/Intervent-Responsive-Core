namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;

    public partial class Drug_Ingredients
    {
        public int Id { get; set; }

        public int Drug_code { get; set; }

        public int? Active_Ingredient_Code { get; set; }

        [StringLength(500)]
        public string? Ingredient { get; set; }

        [StringLength(500)]
        public string? Ingredient_Supplied_IND { get; set; }

        [StringLength(500)]
        public string? Strength { get; set; }

        [StringLength(500)]
        public string? Strength_Unit { get; set; }

        [StringLength(500)]
        public string? Strength_Type { get; set; }

        [StringLength(500)]
        public string? Dosage_Value { get; set; }

        [StringLength(500)]
        public string? Base { get; set; }

        [StringLength(500)]
        public string? Dosage_Unit { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public virtual Drug_Products Drug_Products { get; set; }
    }
}
