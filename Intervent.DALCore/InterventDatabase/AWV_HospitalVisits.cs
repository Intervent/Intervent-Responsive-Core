namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AWV_HospitalVisits
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Facility { get; set; }

        [StringLength(100)]
        public string? AttendingPhysician { get; set; }

        public DateTime? Date { get; set; }

        [StringLength(255)]
        public string? Surgeries { get; set; }

        public virtual AWV AWV { get; set; }
    }
}
