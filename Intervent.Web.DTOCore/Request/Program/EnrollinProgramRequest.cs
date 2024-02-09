namespace Intervent.Web.DTO
{
    public class EnrollinProgramRequest
    {
        public int UserId { get; set; }

        public int ProgramsinPortalsId { get; set; }

        public int PortalId { get; set; }

        public int? hraId { get; set; }

        public int? CoachId { get; set; }

        public int? LoginId { get; set; }

        public string Language { get; set; }
    }
}
