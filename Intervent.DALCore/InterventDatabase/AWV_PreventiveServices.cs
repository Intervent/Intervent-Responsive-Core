namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AWV_PreventiveServices
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime? LastReceived { get; set; }

        public byte? Recommend { get; set; }

        public byte? Testing { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Type { get; set; }

        public virtual AWV AWV { get; set; }

        public virtual AWV_PreventiveServicesType AWV_PreventiveServicesType { get; set; }
    }
}
