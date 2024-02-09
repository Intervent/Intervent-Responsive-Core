namespace Intervent.Web.DTO
{
    public class ProgramsinPortalDto
    {
        public int Id { get; set; }

        public int ProgramId { get; set; }

        public int PortalId { get; set; }


        public decimal Cost { get; set; }

        public string MoreInfo { get; set; }

        public bool Active { get; set; }

        public int? ApptCallTemplateId { get; set; }

        public string NameforUser { get; set; }

        public string DescriptionforUser { get; set; }

        public string NameforAdmin { get; set; }

        public string NameforUserLanguageItem { get; set; }

        public string DescforUserLanguageItem { get; set; }

        public string DescriptionforAdmin { get; set; }
        public AppointmentCallTemplateDto ApptCallTemplate { get; set; }

        public ProgramDto program { get; set; }

        public PortalDto portal { get; set; }

        public int? SortOrder { get; set; }
    }
}