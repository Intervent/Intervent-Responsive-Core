namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Webinar
    {
        public Webinar()
        {
            OrganizationsforWebinars = new HashSet<OrganizationsforWebinar>();
            RegisteredUsersforWebinars = new HashSet<RegisteredUsersforWebinar>();
            WebinarOccurrences = new HashSet<WebinarOccurrence>();
        }

        public int Id { get; set; }

        [StringLength(100)]
        public string? HostEmail { get; set; }

        [StringLength(100)]
        public string? HostId { get; set; }

        [Required]
        [StringLength(100)]
        public string WebinarId { get; set; }

        [StringLength(50)]
        public string? UniqueId { get; set; }

        public int? PresentedBy { get; set; }

        public string? Agenda { get; set; }

        public int? Duration { get; set; }

        [StringLength(10)]
        public string? Password { get; set; }

        [StringLength(200)]
        public string? JoinUrl { get; set; }

        public string? StartUrl { get; set; }

        public DateTime? StartTime { get; set; }

        [StringLength(200)]
        public string? Topic { get; set; }

        public int? Type { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        [StringLength(200)]
        public string? ImageUrl { get; set; }

        [StringLength(200)]
        public string? VideoUrl { get; set; }

        [StringLength(200)]
        public string? Handout { get; set; }

        public int? RecurrenceType { get; set; }

        public int? RecurrenceInterval { get; set; }

        public int? RecurrenceTimes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegisteredUsersforWebinar> RegisteredUsersforWebinars { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrganizationsforWebinar> OrganizationsforWebinars { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WebinarOccurrence> WebinarOccurrences { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }

        public virtual User User2 { get; set; }
    }
}
