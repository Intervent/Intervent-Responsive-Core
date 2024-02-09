namespace Intervent.Web.DTO
{
    public class WearableDeviceDto
    {
        public WearableDeviceDto()
        {
            WearableDevices = new HashSet<WearableDeviceDto>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string AuthUrl { get; set; }

        public bool IsActive { get; set; }

        public byte? Type { get; set; }

        public virtual ICollection<WearableDeviceDto> WearableDevices { get; set; }
    }
}
