namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AssignedMotivationMessages")]
    public partial class AssignedMotivationMessage
    {
        public int Id { get; set; }

        public int MessagesID { get; set; }

        public int OrganizationID { get; set; }

        public DateTime Date { get; set; }

        public bool Completed { get; set; }

        [StringLength(50)]
        public string? MessageType { get; set; }

        public bool Active { get; set; }

        public virtual MotivationMessage MotivationMessage { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
