namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;

    public partial class Drug_ProductsForm
    {
        public int Id { get; set; }

        public int DRUG_CODE { get; set; }

        public int? PHARM_FORM_CODE { get; set; }

        [StringLength(50)]
        public string? PHARMACEUTICAL_FORM { get; set; }

        public virtual Drug_Products Drug_Products { get; set; }
    }
}
