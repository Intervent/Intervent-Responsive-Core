namespace Intervent.Web.DTO.DTO.Notification
{
    public sealed class NotificationTemplateRenderDto
    {
        public int Id { get; set; }

        public string Description { get; set; }
        private NotificationTemplateRenderDto(int id, string desc)
        {
            Id = id;
            Description = desc;
        }

        public static NotificationTemplateRenderDto Passthrough = new NotificationTemplateRenderDto(1, "Passthrough");
        public static NotificationTemplateRenderDto Xslt = new NotificationTemplateRenderDto(2, "Xslt");

        static IEnumerable<NotificationTemplateRenderDto> GetAll()
        {
            List<NotificationTemplateRenderDto> lst = new List<NotificationTemplateRenderDto>();
            lst.Add(NotificationTemplateRenderDto.Passthrough);
            lst.Add(NotificationTemplateRenderDto.Xslt);
            return lst;
        }

        public static NotificationTemplateRenderDto GetByKey(int id)
        {
            return GetAll().Where(x => x.Id == id).FirstOrDefault();
        }

    }
}
