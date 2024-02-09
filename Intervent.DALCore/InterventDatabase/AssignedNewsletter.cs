namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AssignedNewsletter")]
    public partial class AssignedNewsletter
    {
        public int Id { get; set; }

        public int NewsletterId { get; set; }

        public int OrganizationId { get; set; }

        public DateTime Date { get; set; }

        public bool? Completed { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual Newsletter Newsletter { get; set; }
    }
}
