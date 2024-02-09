namespace Intervent.Web.DTO
{
    public class GetAllHRAsforUserResponse
    {
        public IList<int> HRAIds { get; set; }

        public IList<HRADto> HRAs { get; set; }
    }
}
