namespace Intervent.Web.DTO
{
    public sealed class NotificationContactMethodDto
    {
        public int Id { get; set; }

        public string Description { get; set; }
        private NotificationContactMethodDto(int id, string desc)
        {
            Id = id;
            Description = desc;
        }

        public static NotificationContactMethodDto Email = new NotificationContactMethodDto(1, "Email");

        static IEnumerable<NotificationContactMethodDto> GetAll()
        {
            List<NotificationContactMethodDto> lst = new List<NotificationContactMethodDto>();
            lst.Add(NotificationContactMethodDto.Email);
            return lst;
        }

        public static NotificationContactMethodDto GetByKey(int id)
        {
            return GetAll().Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
