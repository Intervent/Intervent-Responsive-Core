using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intervent.DAL
{
    public partial class IntuityUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        public int ExternalUserId { get; set; }

        [Required]
        [StringLength(64)]
        public string AuthToken { get; set; }

        public DateTime ExpiryOn { get; set; }

        public bool IsEligible { get; set; }

        public bool IsCoachingActive { get; set; }

        public DateTime DateUpdated { get; set; }

        public DateTime? DateCreated { get; set; }

        public virtual User User { get; set; }
    }
}