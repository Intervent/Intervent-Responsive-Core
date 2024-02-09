using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public partial class RegisteredUsersforWebinar
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int WebinarId { get; set; }

        public DateTime? RegistrationDate { get; set; }

        [StringLength(800)]
        public string? UserJoinUrl { get; set; }

        public virtual User User { get; set; }

        public virtual Webinar Webinar { get; set; }
    }
}
