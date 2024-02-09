namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UserDoctorInfo")]
    public partial class UserDoctorInfo
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [StringLength(50)]
        public string? Name { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        public int? State { get; set; }

        public int? Country { get; set; }

        [StringLength(15)]
        public string? Zip { get; set; }

        public bool? Active { get; set; }

        public byte? ContactPermission { get; set; }

        [StringLength(50)]
        public string? FaxNumber { get; set; }

        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        public int? ProviderId { get; set; }

        public virtual Country Country1 { get; set; }

        public virtual State State1 { get; set; }

        public virtual User User { get; set; }

        public virtual ApplicationUser appUser { get; set; }

        public virtual Provider Provider { get; set; }
    }
}
