using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public class UserKeys
    {
        [Key]
        public int Id { get; set; }

        public int userId { get; set; }

        public int portalId { get; set; }

        public string? Reason { get; set; }

        public DateTime AcheivedDate { get; set; }

        public virtual User user { get; set; }

        public virtual Portal portal { get; set; }
    }
}
