using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp.Controllers
{
    public class ChallengeController : BaseController
    {
        private readonly IHostEnvironment environment;

        public ChallengeController(IHostEnvironment environment)
        {
            this.environment = environment;
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult Challenges()
        {
            ChallengesModel model = new ChallengesModel();
            var user = ParticipantUtility.ReadUserParticipation(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            model.Name = user.user.FirstName + " " + user.user.LastName;
            model.MemberSince = CommonUtility.dateFormater(user.user.CreatedOn, false, HttpContext.Session.GetString(SessionContext.DateFormat));
            var incentivePoints = CommonUtility.GetIncentivePoints(GetParticipantPortalId(), HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            model.Points = incentivePoints.Points;
            model.Currency = incentivePoints.Currency;
            var response = ChallengeUtility.GetUserActivitiesDetail(HttpContext.Session.GetInt32(SessionContext.UserId).Value, GetParticipantPortalId());
            model.userRaffles = response.userRaffles;
            model.Keys = response.userKeys.Count();
            model.raffles = ChallengeUtility.GetRaffleTypes(response.rafflesinPortals, HttpContext.Session.GetString(SessionContext.LanguagePreference));
            model.customIncentives = PortalUtility.GetCustomIncentiveTypes(HttpContext.Session.GetInt32(SessionContext.IntegrationWith));
            model.challenges = ChallengeUtility.GetIncentiveforChallenges(true, HttpContext.Session.GetInt32(SessionContext.ProgramType), Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.SelfScheduling) != null ? HttpContext.Session.GetString(SessionContext.SelfScheduling) : false), GetParticipantPortalId(), HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            model.employerIncentiveText = user.Portal.EmployerIncentiveText;
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            model.OrganizationName = HttpContext.Session.GetString(SessionContext.OrganizationName);
            return PartialView("_Challenges", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult AddEditUserIncentive([FromBody] UserIncetiveModel model)
        {
            model.userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.portalId = HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value;
            model.adminId = HttpContext.Session.GetInt32(SessionContext.AdminId).Value;
            var result = ChallengeUtility.AddEditIncentives(model);
            if (result == true && model.type == "Delete" && model.fileName != null)
            {
                deleteTobaccoFile(model.fileName);
            }
            return Json(new { Result = result });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult AddCustomIncentive(int PortalIncentiveId, int CustomIncentiveType, double Points, string comments)
        {
            return Json(new { Result = ChallengeUtility.AddCustomIncentive(PortalIncentiveId, CustomIncentiveType, Points, comments, HttpContext.Session.GetInt32(SessionContext.AdminId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value) });
        }

        [Authorize]
        public JsonResult RemoveIncentiveReference(int userIncentiveId, string fileName)
        {
            deleteTobaccoFile(fileName);
            return Json(new { Result = ChallengeUtility.RemoveIncentiveReference(userIncentiveId, HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue ? HttpContext.Session.GetInt32(SessionContext.AdminId).Value : null).success });
        }

        public void deleteTobaccoFile(string fileName)
        {
            string filePath = Path.Combine(environment.ContentRootPath, "~/IncentiveUploads", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        [Authorize]
        public ActionResult Activities()
        {
            ChallengesModel model = new ChallengesModel();
            var user = ParticipantUtility.ReadUserParticipation(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            model.Name = user.user.FirstName + " " + user.user.LastName;
            model.MemberSince = CommonUtility.dateFormater(user.user.CreatedOn, false, HttpContext.Session.GetString(SessionContext.DateFormat));
            var incentivePoints = CommonUtility.GetIncentivePoints(GetParticipantPortalId(), HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            model.Points = incentivePoints.Points;
            model.Currency = incentivePoints.Currency;
            var response = ChallengeUtility.GetUserActivitiesDetail(HttpContext.Session.GetInt32(SessionContext.UserId).Value, GetParticipantPortalId());
            if (response.rafflesinPortals.Count() > 0)
                model.nextRaffle = ChallengeUtility.GetUpcomingRaffleEligiblity(model.Points, response.rafflesinPortals, User.TimeZone());
            model.userRaffles = response.userRaffles;
            model.Keys = response.userKeys.Count();
            model.raffles = ChallengeUtility.GetRaffleTypes(response.rafflesinPortals, HttpContext.Session.GetString(SessionContext.LanguagePreference));
            model.challenges = ChallengeUtility.GetIncentiveforChallenges(false, HttpContext.Session.GetInt32(SessionContext.ProgramType), Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.SelfScheduling) != null ? HttpContext.Session.GetString(SessionContext.SelfScheduling) : false), GetParticipantPortalId(), HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            model.employerIncentiveText = user.Portal.EmployerIncentiveText;
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            model.OrganizationName = HttpContext.Session.GetString(SessionContext.OrganizationName);
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult CustomIncentives(int PortalIncentiveId)
        {
            PortalIncentiveModel model = new PortalIncentiveModel();
            model.PortalIncentiveId = PortalIncentiveId;
            model.AdminId = HttpContext.Session.GetInt32(SessionContext.AdminId);
            model.IntegrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            model = PortalUtility.ReadIncentive(GetParticipantPortalId(), HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            model.CustomIncentiveTypes = PortalUtility.GetCustomIncentiveTypes(HttpContext.Session.GetInt32(SessionContext.IntegrationWith)).Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() });
            return PartialView("_CustomIncentives", model);
        }
    }
}