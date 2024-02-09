namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;

    public partial class UserClaim
    {
        public int UserId { get; set; }

        [Key]
        public int ClaimId { get; set; }

        public string? ClaimType { get; set; }

        public string? ClaimValue { get; set; }

        public virtual User User { get; set; }
    }
}
