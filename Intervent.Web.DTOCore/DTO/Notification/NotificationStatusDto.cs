namespace Intervent.Web.DTO
{
    public class NotificationStatusDto
    {
        public int Id { get; set; }

        public string Description { get; set; }
        private NotificationStatusDto(int id, string desc)
        {
            Id = id;
            Description = desc;
        }

        public static NotificationStatusDto Queued = new NotificationStatusDto(1, "Queued");
        public static NotificationStatusDto Sent = new NotificationStatusDto(2, "Sent");
        public static NotificationStatusDto UnknownFailure = new NotificationStatusDto(3, "Unknown Failure");
        public static NotificationStatusDto Ignored = new NotificationStatusDto(4, "Ignored");

        static IEnumerable<NotificationStatusDto> GetAll()
        {
            List<NotificationStatusDto> lst = new List<NotificationStatusDto>();
            lst.Add(NotificationStatusDto.Queued);
            lst.Add(NotificationStatusDto.Sent);
            lst.Add(NotificationStatusDto.UnknownFailure);
            lst.Add(NotificationStatusDto.Ignored);
            return lst;
        }

        public static NotificationStatusDto GetByKey(int id)
        {
            return GetAll().Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
