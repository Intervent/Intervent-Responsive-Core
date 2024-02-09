namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class KitsinProgram
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProgramId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int KitId { get; set; }

        public short Order { get; set; }

        public bool Active { get; set; }

        public virtual Kit Kit { get; set; }

        public virtual Program Program { get; set; }
    }
}
