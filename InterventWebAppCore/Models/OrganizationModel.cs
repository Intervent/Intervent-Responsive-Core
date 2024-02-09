using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class OrganizationModel
    {
        public GetOrganizationDetailsResponse OrganizationDetails { get; set; }

        public IEnumerable<SelectListItem> Groups { get; set; }

        public IEnumerable<SelectListItem> Integrations { get; set; }

        public IEnumerable<SelectListItem> ParentOrganizations { get; set; }

        public int Group { get; set; }

        public int? Integration { get; set; }

        public int ParentOrganization { get; set; }

        public string BaseUrl { get; set; }
    }

    public class ListOrganizationModel
    {

        public int Id { get; set; }

        public int RecentPortalId { get; set; }

        public string Name { get; set; }

        public string ParentOrganizationName { get; set; }

        public string Description { get; set; }

        public string RecentPortalName { get; set; }

        public string Language { get; set; }

        public string HRAVersion { get; set; }

        public string ProgramTypes { get; set; }

        public string EndDate { get; set; }

        public bool IsActivePortal { get; set; }

        public string ActiveDetails { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Groups { get; set; }

        public string EmailValidationRequired { get; set; }

        public string SSO { get; set; }

        public string TermsForSSO { get; set; }

        public string OwnCoach { get; set; }
    }

    public class OrganizationDetailsModel
    {
        public OrganizationDto Organization { get; set; }

        public int Group { get; set; }

        public int ParentOrganization { get; set; }

        public IEnumerable<SelectListItem> Groups { get; set; }

        public IEnumerable<SelectListItem> Integrations { get; set; }

        public IEnumerable<SelectListItem> ParentOrganizations { get; set; }

        public IEnumerable<SelectListItem> Portals { get; set; }

        public IEnumerable<SelectListItem> languages { get; set; }

        public IEnumerable<SelectListItem> specializations { get; set; }

        public IEnumerable<SelectListItem> kits { get; set; }

        public IEnumerable<SelectListItem> emails { get; set; }

        public int emailType { get; set; }

        public int kitType { get; set; }

        public int portalLanguages { get; set; }

        public int portalSpecializations { get; set; }

        public int PortalId { get; set; }

        public PortalDto portal { get; set; }

        public string PatientReleaseForm { get; set; }

        public string MedicalClearanceForm { get; set; }

        public string KnowYourNumbersForm { get; set; }

        public string TestimonialForm { get; set; }

        public string dateFormat { get; set; }

        public string BaseUrl { get; set; }
    }

    public class ProgramDetailsModel
    {

        public IEnumerable<SelectListItem> programLists { get; set; }

        public IEnumerable<SelectListItem> callTemplateList { get; set; }

        public ProgramsinPortalDto programinPortal { get; set; }


    }

    public class IncentiveDetailsModel
    {

        public IEnumerable<SelectListItem> incentiveTypes { get; set; }

        public IEnumerable<SelectListItem> programTypes { get; set; }

        public PortalIncentiveDto portalIncentive { get; set; }

        public bool isUpload { get; set; }

    }

    public class RaffleDetailsModel
    {

        public IEnumerable<SelectListItem> raffleTypes { get; set; }

        public IEnumerable<SelectListItem> programTypes { get; set; }

        public RafflesinPortalsDto rafflesinPortals { get; set; }

        public bool isUpload { get; set; }

    }
}