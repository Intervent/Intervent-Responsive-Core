using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intervent.DAL
{
    public class ApplicationUser : IdentityUser<int>
    {
        public override int Id { get; set; }

        [Required]
        [StringLength(50)]
        public override string? UserName { get; set; }

        [Required]
        [StringLength(50)]
        public override string? Email { get; set; }

        public override bool EmailConfirmed { get; set; }

        [StringLength(100)]
        public override string? PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        public override string? SecurityStamp { get; set; }

        [StringLength(25)]
        public override string? PhoneNumber { get; set; }

        public override bool PhoneNumberConfirmed { get; set; }

        public override bool TwoFactorEnabled { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public override bool LockoutEnabled { get; set; }

        public override int AccessFailedCount { get; set; }

        [StringLength(50)]
        public string? NamePrefix { get; set; }

        [Required]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DOB { get; set; }

        public byte? Gender { get; set; }

        public int? Race { get; set; }

        [StringLength(50)]
        public string? RaceOther { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(255)]
        public string? Address2 { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        public int? State { get; set; }

        public int? Country { get; set; }

        [StringLength(15)]
        public string? Zip { get; set; }

        [StringLength(50)]
        public string? HomeNumber { get; set; }

        [StringLength(50)]
        public string? WorkNumber { get; set; }

        [StringLength(50)]
        public string? CellNumber { get; set; }

        public int? TimeZoneId { get; set; }

        public byte? PreferredContactTimeId { get; set; }

        public int? ProfessionId { get; set; }

        public int OrganizationId { get; set; }

        [StringLength(50)]
        public string? Occupation { get; set; }

        public byte? Source { get; set; }

        [StringLength(50)]
        public string? SourceOther { get; set; }

        public string? ReferralDetails { get; set; }

        public byte? Text { get; set; }

        [StringLength(100)]
        public string? Picture { get; set; }

        public bool? Complete { get; set; }

        public byte? ContactMode { get; set; }

        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? UniqueId { get; set; }

        public int? CreatedBy { get; set; }

        [StringLength(10)]
        public string? LanguagePreference { get; set; }

        public byte? PrimaryCarePhysician { get; set; }

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [StringLength(50)]
        public string? Suffix { get; set; }

        public byte? Unit { get; set; }

        public bool IsActive { get; set; }

        public int? InactiveReason { get; set; }

        [StringLength(50)]
        public string? CouponCode { get; set; }

        public bool? TermsAccepted { get; set; }

        [StringLength(1024)]
        public string? LastVisited { get; set; }

        public int? DeptId { get; set; }

        [StringLength(50)]
        public string? EmployeeId { get; set; }

        public bool MobileNotification { get; set; }

        public bool BiometricEnabled { get; set; }

        public bool AllowText { get; set; }

        [StringLength(50)]
        public string? UnsubscribedEmail { get; set; }

        public override string? NormalizedUserName { get; set; }

        public override string? NormalizedEmail { get; set; }

        public override string? ConcurrencyStamp { get; set; }

        public Organization Organization { get; set; }

        public TimeZone TimeZone { get; set; }

        public virtual AdminProperty AdminProperty { get; set; }

        public ICollection<UserDoctorInfo> UserDoctorInfoes { get; set; }

        public ICollection<State> CoachStates { get; set; }
    }
}
