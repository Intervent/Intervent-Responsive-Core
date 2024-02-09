namespace Intervent.Web.DTO
{
    public class LabErrorLogDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string UniqueId { get; set; }
        public int PortalId { get; set; }
        public string Error { get; set; }
        public DateTime LogDate { get; set; }
        public DateTime FirstLogDate { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public UserDto user { get; set; }
    }
}
