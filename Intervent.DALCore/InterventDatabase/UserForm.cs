namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class UserForm
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int PortalId { get; set; }

        public int FormTypeId { get; set; }

        [StringLength(256)]
        public string? Form { get; set; }

        public bool Approved { get; set; }

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedOn { get; set; }
    }
}
