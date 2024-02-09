using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class PortalIncentiveModel
    {
        public IList<PortalIncentiveDto> PortalIncentives { get; set; }

        // public bool IsPregnant { get; set; }

        public int? AdminId { get; set; }

        public int? IntegrationWith { get; set; }

        public IEnumerable<SelectListItem> CustomIncentiveTypes { get; set; }

        public int CustomIncentiveType { get; set; }

        public int PortalIncentiveId { get; set; }

        public bool IsSmoker { get; set; }

        public int programsinportalId { get; set; }
    }

    public class UserIncetiveModel
    {
        public int portalIncentiveId { get; set; }

        public int userIncentiveId { get; set; }

        public bool isTobacco { get; set; }

        public string type { get; set; }

        public string fileName { get; set; }

        public int userId { get; set; }

        public int portalId { get; set; }

        public int adminId { get; set; }
    }

    public class ChallengesViewModel
    {
        public int PortalIncentiveId { get; set; }

        public int IncentiveTypeId { get; set; }

        public bool IsActive { get; set; }

        public bool IsCompanyIncentive { get; set; }

        public bool IsPoint { get; set; }

        public string PointsText { get; set; }

        public string CompanyPointsText { get; set; }

        public string Attachment { get; set; }

        public string Name { get; set; }

        public string MoreInfo { get; set; }

        public string ImageUrl { get; set; }

        public bool supportTobaccoUpload { get; set; }

        public int userIncentiveId { get; set; }

        public bool completed { get; set; }

        public bool employerComplete { get; set; }

        public string Reference { get; set; }

        public string Url { get; set; }

        public bool isGiftCard { get; set; }

        public List<UserIncentiveDto> userIncentives { get; set; }
    }
}


