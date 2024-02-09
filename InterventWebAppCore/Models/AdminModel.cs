using Intervent.Web.DTO;
using Intervent.Web.DTO.Diff;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class AdminDashboardModel
    {
        public string firstName { get; set; }

        public string lastName { get; set; }

        public string picture { get; set; }

        public int messageCount { get; set; }

        public int ParticipantId { get; set; }

        public string ParticipantName { get; set; }

        public bool ShowStopTimer { get; set; }

        public DateTime TrackerStartTime { get; set; }

        public IEnumerable<SelectListItem> TimeTrackingDispositionList { get; set; }

        public IList<AdminModuleDto> AdminModules { get; set; }

        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Picture { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsCoach { get; internal set; }

        public bool IsCSR { get; internal set; }

        public IEnumerable<SelectListItem> ContactRequirements { get; set; }

        public AdminDashboardResponse DashBoardDetails { get; set; }
    }

    public class AdminMenuModel
    {
        public int MessageCount { get; set; }

        public IList<AdminModuleDto> AdminModules { get; set; }
    }

    public class UserNamewithId
    {
        public IList<Object> userList { get; set; }
    }

    public class TaskManagementModel
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public IEnumerable<SelectListItem> TaskList { get; set; }

        public int Task { get; set; }

        public IEnumerable<SelectListItem> OwnerList { get; set; }

        public IEnumerable<SelectListItem> OrganizationList { get; set; }

        public int Owner { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }

        public string Status { get; set; }

        public string DateFormat { get; set; }

        public FilterReportTempData tempData { get; set; }
    }

    public class GroupsModel
    {
        public IEnumerable<SelectListItem> UserRoleCodes { get; set; }

        public IEnumerable<SelectListItem> AdminModules { get; set; }

        public IEnumerable<SelectListItem> OrganizationList { get; set; }

        public int AdminModule { get; set; }

        public int Organization { get; set; }

        public string RoleCode { get; set; }
    }
    public class UserRolesModel
    {
        public IEnumerable<SelectListItem> Groups { get; set; }

        public IEnumerable<SelectListItem> Integrations { get; set; }

        public int Group { get; set; }

        public int? Integration { get; set; }
    }
    public class AdminUsersModel
    {
        public IEnumerable<SelectListItem> OrganizationList { get; set; }

        public string Organization { get; set; }

        public IEnumerable<SelectListItem> Units { get; set; }

        public byte? Unit { get; set; }

    }

    public class ParticipantHistorySearchModel
    {
        public string ParticipantName { get; set; }

        public string AbsoluteUri { get; set; }

        public int ParticipantId { get; set; }

        public IEnumerable<SelectListItem> UserHistoryCategories { get; set; }

        public int UserHistoryCategoryId { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }

    public class ParticipantHistoryChangesModel
    {
        public IEnumerable<PropertyCompare> Changes { get; set; }
    }
    public class ReportSelectModel
    {
        public bool IsSuperAdmin { get; set; }

        public string DateFormat { get; set; }

        public IEnumerable<SelectListItem> OrganizationList { get; set; }

        public IEnumerable<SelectListItem> TimeZoneList { get; set; }

        public int TimeZoneType { get; set; }

        public bool? reviewedOrNot { get; set; }

        public int Organization { get; set; }

        public IEnumerable<SelectListItem> AppointmentTypeList { get; set; }

        public int AppointmentType { get; set; }

        public IEnumerable<SelectListItem> PortalList { get; set; }

        public int portal { get; set; }

        public FilterReportTempData tempData { get; set; }

        public IEnumerable<SelectListItem> CoachList { get; set; }

        public int coach { get; set; }

        public IEnumerable<SelectListItem> KitsInPortalList { get; set; }

        public int kit { get; set; }

        public IEnumerable<SelectListItem> AssessmentList { get; set; }

        public int AssessmentType { get; set; }

        public IEnumerable<SelectListItem> UserStautsList { get; set; }

        public int UserStatus { get; set; }
    }
    public class ScreeningDataModel
    {
        public string UniqueId { get; set; }

        public int OrganizationId { get; set; }

        public float? TotalChol { get; set; }

        public float? LDL { get; set; }

        public float? HDL { get; set; }

        public float? Trig { get; set; }

        public float? Glucose { get; set; }

        public float? A1C { get; set; }

        public float? Weight { get; set; }

        public float? Height { get; set; }

        public float? Waist { get; set; }

        public short? SBP { get; set; }

        public short? DBP { get; set; }

        public int hraid { get; set; }
        public int userId { get; set; }
        public int portalId { get; set; }
    }
}