namespace Intervent.Web.DTO
{
    public class Drug_IngredientsDto
    {
        public int Drug_Code { get; set; }
        public int Active_Ingredient_Code { get; set; }
        public string Ingredient { get; set; }
        public string Strength { get; set; }
        public string Strength_Unit { get; set; }
        public string Strength_Type { get; set; }
        public string Dosage_Value { get; set; }
        public string Base { get; set; }
        public string Dosage_Unit { get; set; }
        public string Notes { get; set; }
        public Drug_ProductDto Drug_Product { get; set; }
    }
}
