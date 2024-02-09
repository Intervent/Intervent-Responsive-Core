namespace Intervent.Web.DTO
{
    public class Drug_ProductDto
    {
        public int DRUG_CODE { get; set; }
        public string PRODUCT_CATEGORIZATION { get; set; }
        public string CLASS { get; set; }
        public string DRUG_IDENTIFICATION_NUMBER { get; set; }
        public string BRAND_NAME { get; set; }
        public string DESCRIPTOR { get; set; }
        public string PEDIATRIC_FLAG { get; set; }
        public string ACCESSION_NUMBER { get; set; }
        public string NUMBER_OF_AIS { get; set; }
        public DateTime? LAST_UPDATE_DATE { get; set; }
        public string AI_GROUP_NO { get; set; }


        public IList<Drug_ProductsFormDto> Drug_ProductsForm { get; set; }

        public IList<Drug_IngredientsDto> Drug_Ingredients { get; set; }

    }
}