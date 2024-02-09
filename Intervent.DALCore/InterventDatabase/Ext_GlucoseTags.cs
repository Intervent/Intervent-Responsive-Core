namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class Ext_GlucoseTags
    {
        public int Id { get; set; }

        public int GlucoseId { get; set; }

        [Required]
        [StringLength(64)]
        public string TagName { get; set; }

        [Required]
        [StringLength(128)]
        public string Value { get; set; }

        [StringLength(128)]
        public string? Unit { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public virtual EXT_Glucose EXT_Glucose { get; set; }
    }
}