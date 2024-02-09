using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intervent.DAL
{
    public class PreviousPassword
    {
        public PreviousPassword()
        {
            CreateDate = DateTime.UtcNow;
        }
        [Key, Column(Order = 0)]
        public string PasswordHash { get; set; }

        public DateTime CreateDate { get; set; }

        [Key, Column(Order = 1)]
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}