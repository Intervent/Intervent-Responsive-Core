using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{

    public class CRMProfileModel
    {
        public CRM_ContactDto CRM_Contact { get; set; }

        public CRM_NoteDto CRM_Note { get; set; }

        public IEnumerable<SelectListItem> ListOrganizations { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<SelectListItem> InsuranceTypes { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }

        public IEnumerable<SelectListItem> AccountTypes { get; set; }

        public IEnumerable<SelectListItem> CallerProfileTypes { get; set; }

        public IEnumerable<SelectListItem> InquiryTypes { get; set; }

        public IEnumerable<SelectListItem> ComplaintClassificationTypes { get; set; }

        public IEnumerable<SelectListItem> Dispositions { get; set; }

        public IEnumerable<SelectListItem> PogoMeterNumbers { get; set; }

        public IEnumerable<SelectListItem> NamePrefixList { get; set; }

        public IEnumerable<SelectListItem> years { get; set; }

        public IEnumerable<SelectListItem> CSRLists { get; set; }

        public int year { get; set; }

        public IEnumerable<SelectListItem> months { get; set; }

        public int month { get; set; }

        public IEnumerable<SelectListItem> days { get; set; }

        public int day { get; set; }

        public IEnumerable<SelectListItem> GenderList { get; set; }

        public IEnumerable<SelectListItem> RaceList { get; set; }

        public IEnumerable<SelectListItem> PreferredContactTimes { get; set; }

        public IEnumerable<SelectListItem> ContactModes { get; set; }

        public IEnumerable<SelectListItem> TimeZones { get; set; }

        public IEnumerable<SelectListItem> LanguagePreferences { get; set; }

        public IEnumerable<SelectListItem> DoctorInfo_States { get; set; }

        public IEnumerable<SelectListItem> Sources { get; set; }

        public IEnumerable<SelectListItem> Units { get; set; }

        public IEnumerable<SelectListItem> InactiveReason { get; set; }

        public bool readOnly { get; set; }

        public IList<string> readOnlyList { get; set; }

        public string module { get; set; }

        public string InsuranceType { get; set; }

        public bool? create { get; set; }

        public string existingEmail { get; set; }

        public bool LockOption { get; set; }

        public bool IsLocked { get; set; }

        public bool basicDetails { get; set; }

        public bool contactDetails { get; set; }

        public bool basicDetailsCompeleted { get; set; }

        public bool contactDetailsCompeleted { get; set; }

        public int? TotalRecords { get; set; }

        public string dob { get; set; }

        public IEnumerable<SelectListItem> States2 { get; set; }

        public bool fromInfoPage { get; set; }

        public int eligibilityId { get; set; }

        public int? userId { get; set; }

        public FilterReportTempData tempData { get; set; }

        public string DateFormat { get; set; }
    }

    public class CRMSearchModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? crmId { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string RMAandQADNumber { get; set; }

        public string MasterControlNo { get; set; }

        public int? CSRId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int? TotalRecords { get; set; }

        public bool ComplaintsSearch { get; set; }

        public bool UnhandledWebforms { get; set; }

    }

}