namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Drug_Products
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Drug_Products()
        {
            Drug_Ingredients = new HashSet<Drug_Ingredients>();
            Drug_ProductsForm = new HashSet<Drug_ProductsForm>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DRUG_CODE { get; set; }

        [StringLength(80)]
        public string? PRODUCT_CATEGORIZATION { get; set; }

        [StringLength(40)]
        public string? CLASS { get; set; }

        [StringLength(50)]
        public string? DRUG_IDENTIFICATION_NUMBER { get; set; }

        [StringLength(200)]
        public string? BRAND_NAME { get; set; }

        [StringLength(150)]
        public string? DESCRIPTOR { get; set; }

        [StringLength(1)]
        public string? PEDIATRIC_FLAG { get; set; }

        [StringLength(5)]
        public string? ACCESSION_NUMBER { get; set; }

        [StringLength(10)]
        public string? NUMBER_OF_AIS { get; set; }

        public DateTime? LAST_UPDATE_DATE { get; set; }

        [StringLength(10)]
        public string? AI_GROUP_NO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Drug_Ingredients> Drug_Ingredients { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Drug_ProductsForm> Drug_ProductsForm { get; set; }
    }
}
