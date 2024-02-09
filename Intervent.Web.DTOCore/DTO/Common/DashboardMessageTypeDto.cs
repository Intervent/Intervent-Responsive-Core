namespace Intervent.Web.DTO
{
    public class DashboardMessageTypeDto
    {
        public int Id { get; set; }

        public string MessageTemplate { get; set; }

        public string Type { get; set; }

        public string Image { get; set; }

        public string Alt { get; set; }

        public string Url { get; set; }

        public string languageCode { get; set; }

        public string LanguageItem { get; set; }

        public byte NotificationType { get; set; }

        public string Subject { get; set; }

        public virtual ICollection<UserDashboardMessageDto> UserDashboardMessages { get; set; }
    }
}
