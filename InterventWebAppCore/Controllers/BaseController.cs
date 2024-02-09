using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using NLog;

namespace InterventWebApp
{
    //[ServiceFilter(typeof(BaseResultFilter))]
    [TypeFilter(typeof(BaseExceptionFilter))]
    public class BaseController : Controller
    {
        protected Logger Log { get; private set; }

        public BaseController()
        {
            Log = LogManager.GetLogger(GetType().FullName);
        }

        public int GetParticipantPortalId()
        {
            return HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : (HttpContext.Session.GetInt32(SessionContext.InActiveParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.InActiveParticipantPortalId).Value : 0);
        }

        public bool HasCoachingConditions()
        {
            //#NOTE : This method is dupilicated in MobileUtility (if there is any change here, copy the same to MobileUtility)
            PortalReader reader = new PortalReader();
            bool All = false;
            bool Diabetes = false;
            bool Postmenopausal = false;
            var portalId = GetParticipantPortalId();
            List<PortalCoachingConditionsDto> CoachingConditions = reader.GetPortalCoachingConditions(portalId);
            if (CoachingConditions != null)
            {
                foreach (PortalCoachingConditionsDto schedulingTool in CoachingConditions)
                {
                    if (schedulingTool.CoachCondId == PortalCoachingConditions.All.GetHashCode())
                        All = true;
                    if (schedulingTool.CoachCondId == PortalCoachingConditions.Diabetes.GetHashCode())
                        Diabetes = true;
                    if (schedulingTool.CoachCondId == PortalCoachingConditions.Postmenopausal.GetHashCode())
                        Postmenopausal = true;
                }
            }
            return All || (Diabetes && HasDiabetes()) || (Postmenopausal && IsPostmenopausal());
        }

        public bool HasDiabetes()
        {
            ParticipantReader reader = new ParticipantReader();
            CheckTobaccoUserRequest request = new CheckTobaccoUserRequest();
            request.participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            request.portalId = GetParticipantPortalId();
            return reader.HasDiabetes(request);
        }

        public bool IsPostmenopausal()
        {
            //#NOTE : This method is dupilicated in MobileUtility (if there is any change here, copy the same to MobileUtility)
            ParticipantReader reader = new ParticipantReader();
            if (HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue)
            {
                int HRAId = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
                return reader.IsPostmenopausal(HRAId, HttpContext.Session.GetString(SessionContext.DOB), HttpContext.Session.GetInt32(SessionContext.Gender));
            }
            else
                return false;
        }

        public bool IsRescheduling()
        {
            //#NOTE : This method is dupilicated in MobileUtility (if there is any change here, copy the same to MobileUtility)
            if (HttpContext.Session.GetInt32(SessionContext.UserinProgramId).HasValue)
            {
                ParticipantReader reader = new ParticipantReader();
                GetCoachingCountRequest request = new GetCoachingCountRequest();
                request.userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                request.portalId = GetParticipantPortalId();
                request.refId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value;
                if (HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue)
                    request.hraId = HttpContext.Session.GetInt32(SessionContext.HRAId);
                var coachingNotes = reader.GetCoachingCount(request);
                if (coachingNotes.count >= 1)
                    return true;
            }
            if (HttpContext.Session.GetInt32(SessionContext.NextApptId).HasValue)
            {
                return true;
            }
            return false;
        }

        public bool ShowSelfScheduling()
        {
            //#NOTE : This method is dupilicated in MobileUtility (if there is any change here, copy the same to MobileUtility)
            if ((!HttpContext.Session.GetInt32(SessionContext.ProgramType).HasValue || HttpContext.Session.GetInt32(SessionContext.ProgramType) == 2)
                && (HttpContext.Session.GetString(SessionContext.NextApptDate) == null)
                && (HttpContext.Session.GetString(SessionContext.CoachingProgram) != null && Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.CoachingProgram).ToString()) == true)
                && (HttpContext.Session.GetString(SessionContext.ShowProgramOption) != null && Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.ShowProgramOption).ToString()) == true
                && HttpContext.Session.GetString(SessionContext.SelfScheduling) != null && Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.SelfScheduling).ToString()) == true)
                && (HttpContext.Session.GetString(SessionContext.UserStatus) == null || HttpContext.Session.GetString(SessionContext.UserStatus).ToString() != "T")
                && HasCoachingConditions())
            {
                return true;
            }
            return false;
        }

        public void ClearParticipantSession()
        {
            //HttpContext.Session.Clear();
            //HttpContext.Session.Remove(SessionContext.UserId);
            //HttpContext.Session.Remove(SessionContext.DateFormat);
            //HttpContext.Session.Remove(SessionContext.AdminId);
            //HttpContext.Session.Remove(SessionContext.LandingPage);

            HttpContext.Session.Remove(SessionContext.ParticipantId);
            HttpContext.Session.Remove(SessionContext.CarePlanType);
            HttpContext.Session.Remove(SessionContext.DOB);
            HttpContext.Session.Remove(SessionContext.Gender);
            HttpContext.Session.Remove(SessionContext.HRAId);
            HttpContext.Session.Remove(SessionContext.HRACompleteDate);
            HttpContext.Session.Remove(SessionContext.HRAPageSeqDone);
            HttpContext.Session.Remove(SessionContext.UserinProgramId);
            HttpContext.Session.Remove(SessionContext.ProgramsInPortalId);
            HttpContext.Session.Remove(SessionContext.EnrolledinCoaching);
            HttpContext.Session.Remove(SessionContext.ProgramType);
            HttpContext.Session.Remove(SessionContext.ParticipantName);
            HttpContext.Session.Remove(SessionContext.ParticipantEmail);
            HttpContext.Session.Remove(SessionContext.OrganizationId);
            HttpContext.Session.Remove(SessionContext.OrganizationName);
            HttpContext.Session.Remove(SessionContext.OrganizationCode);
            HttpContext.Session.Remove(SessionContext.OrgContactNumber);
            HttpContext.Session.Remove(SessionContext.ParticipantTimeZone);
            HttpContext.Session.Remove(SessionContext.ParticipantTimeZoneName);
            HttpContext.Session.Remove(SessionContext.ParticipantLanguagePreference);
            HttpContext.Session.Remove(SessionContext.ParticipantPortalId);
            HttpContext.Session.Remove(SessionContext.HRAPageSeq);
            HttpContext.Session.Remove(SessionContext.NextApptDate);
            HttpContext.Session.Remove(SessionContext.LanguagePreference);
            HttpContext.Session.Remove(SessionContext.SelfHelpProgram);
            HttpContext.Session.Remove(SessionContext.CoachingProgram);
            HttpContext.Session.Remove(SessionContext.FollowUpId);
            HttpContext.Session.Remove(SessionContext.FollowUpCompleteDate);
            HttpContext.Session.Remove(SessionContext.UniqueId);
            HttpContext.Session.Remove(SessionContext.HRAValidity);
            HttpContext.Session.Remove(SessionContext.ShowProgramOption);
            HttpContext.Session.Remove(SessionContext.SelfScheduling);
            HttpContext.Session.Remove(SessionContext.ClientNameInReport);
            HttpContext.Session.Remove(SessionContext.AssignedFollowUp);
            HttpContext.Session.Remove(SessionContext.Unit);
            HttpContext.Session.Remove(SessionContext.MailScoreCard);
            HttpContext.Session.Remove(SessionContext.CarePlan);
            HttpContext.Session.Remove(SessionContext.Zip);
            HttpContext.Session.Remove(SessionContext.ShowPricing);
            HttpContext.Session.Remove(SessionContext.VisitedTab);
            HttpContext.Session.Remove(SessionContext.IntegrationWith);
            HttpContext.Session.Remove(SessionContext.Challenges);
            HttpContext.Session.Remove(SessionContext.HasActivePortal);
            HttpContext.Session.Remove(SessionContext.HRAVer);
            HttpContext.Session.Remove(SessionContext.TermsAccepted);
            HttpContext.Session.Remove(SessionContext.TermsSSO);
            HttpContext.Session.Remove(SessionContext.HasSP);
            HttpContext.Session.Remove(SessionContext.SSOState);
            HttpContext.Session.Remove(SessionContext.UserStatus);
            HttpContext.Session.Remove(SessionContext.AssignPrograms);
            HttpContext.Session.Remove(SessionContext.SessionTimeout);
            HttpContext.Session.Remove(SessionContext.StateId);
            HttpContext.Session.Remove(SessionContext.isPregnant);
            HttpContext.Session.Remove(SessionContext.NextApptId);
            HttpContext.Session.Remove(SessionContext.AllowCardiacQuestion);
            HttpContext.Session.Remove(SessionContext.OrgContactEmail);
            HttpContext.Session.Remove(SessionContext.FollowUpPageSeqDone);
            HttpContext.Session.Remove(SessionContext.IsParticipantView);
            HttpContext.Session.Remove(SessionContext.HasHRA);
            HttpContext.Session.Remove(SessionContext.FollowUpValidity);
            HttpContext.Session.Remove(SessionContext.MessageCount);
            HttpContext.Session.Remove(SessionContext.ShowPostmenopausal);
            HttpContext.Session.Remove(SessionContext.NeedCareplanApproval);
            HttpContext.Session.Remove(SessionContext.CareplanPath);
            HttpContext.Session.Remove(SessionContext.AssessmentName);
            HttpContext.Session.Remove(SessionContext.InActiveParticipantPortalId);
            HttpContext.Session.Remove(SessionContext.GarminOAuthSecretKey);
            HttpContext.Session.Remove(SessionContext.ProgramCode);
            HttpContext.Session.Remove(SessionContext.IsMediOrbisUser);
            HttpContext.Session.Remove(SessionContext.SingleSignOn);
            HttpContext.Session.Remove(SessionContext.MobileSignOn);
            HttpContext.Session.Remove(SessionContext.SSO);
            HttpContext.Session.Remove(SessionContext.KitAlert);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
            {
                if (!string.IsNullOrEmpty(User.UserId()))
                {
                    ParticipantUtility.TrackTime(0, Convert.ToInt32(User.UserId()), null, ParticipantUtility.GetTimeTrackingDispositionList().Where(x => x.Type == "Coaching (SE)").Select(x => x.Id).FirstOrDefault(), true);
                }

                if (!string.IsNullOrEmpty(User.ExpirationUrl()))
                {
                    filterContext.Result = Redirect(User.ExpirationUrl());
                }
                else
                {
                    filterContext.Result = RedirectToAction("Index", "Home");
                }
            }
            else if (!CommonUtility.HasAdminRole(User.RoleCode()) && HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue && Request.ContentType == "" && HttpContext.Request.Query.Count() == 0)
            {
                var acceptTypes = HttpContext.Request.Headers["Accept"];
                var acceptedContentTypes = acceptTypes.ToString().Split(',');

                if (acceptedContentTypes.Count() > 0 && acceptedContentTypes.Contains("text/html"))
                {
                    string url = HttpContext.Request.Path;
                    CommonUtility.UpdateLastVisited(url, Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value));
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class BaseResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers[HeaderNames.Expires] = "-1";
            filterContext.HttpContext.Response.Headers[HeaderNames.Pragma] = "no-cache";
            filterContext.HttpContext.Response.Headers[HeaderNames.CacheControl] = "no-store, no-cache, must-revalidate";
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }

    public class BaseExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext != null && filterContext.Exception != null)
            {
                LogReader reader = new LogReader();
                string controller = filterContext.RouteData.Values["controller"].ToString();
                string action = filterContext.RouteData.Values["action"].ToString();
                string message = string.Format("{0}Controller.{1}, Browser: {2} {3}", controller, action, "Request.Browser.Browser", "Request.Browser.Version");
                string userId = "";
                if (!filterContext.HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue)
                    userId = string.Format("U:{0}", Convert.ToInt32(filterContext.HttpContext.User.UserId()));
                else
                    userId = string.Format("U:{0}, P:{1}", Convert.ToInt32(filterContext.HttpContext.User.UserId()), filterContext.HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, userId, null, message, null, filterContext.Exception);
                reader.WriteLogMessage(logEvent);
            }

            filterContext.ExceptionHandled = true;
        }
    }

    [TypeFilter(typeof(AccountExceptionFilter))]
    public class AccountBaseController : Controller
    {
        public int GetParticipantPortalId()
        {
            return HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : (HttpContext.Session.GetInt32(SessionContext.InActiveParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.InActiveParticipantPortalId).Value : 0);
        }
    }

    public class AccountExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext != null && filterContext.Exception != null)
            {
                LogReader reader = new LogReader();
                string controller = filterContext.RouteData.Values["controller"].ToString();
                string action = filterContext.RouteData.Values["action"].ToString();
                string message = string.Format("{0}Controller.{1}, Browser: {2} {3}", controller, action, "Request.Browser.Browser", "Request.Browser.Version");
                string userId = "";
                if (filterContext.HttpContext.User.UserId() == null || filterContext.HttpContext.User.UserId() == "")
                    userId = "LandingPage";
                else if (!filterContext.HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue)
                    userId = string.Format("U:{0}", Convert.ToInt32(filterContext.HttpContext.User.UserId()));
                else
                    userId = string.Format("U:{0}, P:{1}", Convert.ToInt32(filterContext.HttpContext.User.UserId()), filterContext.HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, userId, null, message, null, filterContext.Exception);
                reader.WriteLogMessage(logEvent);
            }

            filterContext.ExceptionHandled = true;
        }
    }
}