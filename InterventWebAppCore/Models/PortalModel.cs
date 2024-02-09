using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class PortalModel
    {
        public PortalDto portal { get; set; }

        public IEnumerable<SelectListItem> emails { get; set; }

        public IEnumerable<SelectListItem> kits { get; set; }

        public IEnumerable<SelectListItem> languages { get; set; }

        public IEnumerable<SelectListItem> specializations { get; set; }

        public IEnumerable<SelectListItem> CarePlanTypes { get; set; }

        public IEnumerable<SelectListItem> FollowUpTypes { get; set; }

        public IEnumerable<SelectListItem> HRAStatus { get; set; }

        public IEnumerable<SelectListItem> HRAVersions { get; set; }

        public IEnumerable<SelectListItem> AssessmentNames { get; set; }

        public IEnumerable<SelectListItem> LabProcedures { get; set; }

        public IEnumerable<SelectListItem> CoachingConditions { get; set; }

        public IEnumerable<SelectListItem> EligibilitytoIntuity { get; set; }

        public IEnumerable<SelectListItem> EligibilityFormats { get; set; }

        public IEnumerable<SelectListItem> ProviderDetails { get; set; }

        public int emailType { get; set; }

        public int kitType { get; set; }

        public int portalLanguages { get; set; }

        public byte? LabProcedure { get; set; }

        public int portalSpecializations { get; set; }

        public int portalcoachingfollowups { get; set; }

        public int portalselfhelpfollowups { get; set; }

        public int followuplabs { get; set; }

        public int selfhelpfollowuplabs { get; set; }

        public int CoachingCondition { get; set; }

        public int incentiveCount { get; set; }

        public int kitsCount { get; set; }

        public int emailCount { get; set; }

        public int rafflesCount { get; set; }

        public int formsCount { get; set; }

        public int couponsCount { get; set; }

        public int kitsinFollowUpCount { get; set; }

        public int updateFor { get; set; }

        public IEnumerable<SelectListItem> yesorNoQuestion { get; set; }

        public IEnumerable<SelectListItem> hraOption { get; set; }

        public string dateFormat { get; set; }
    }

    public class OrganizationListModel
    {
        public int page { get; set; }

        public int pageSize { get; set; }

        public int? totalRecords { get; set; }

        public int? parentOrganizationId { get; set; }

        public int? filterBy { get; set; }

        public bool? removechildOrganizations { get; set; }

        public bool includeParentOrganization { get; set; }

        public string search { get; set; }
    }
}