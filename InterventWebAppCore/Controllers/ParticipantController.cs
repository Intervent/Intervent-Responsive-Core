using Intervent.Business.Account;
using Intervent.Business.Claims;
using Intervent.Business.Eligibility;
using Intervent.Business.Organization;
using Intervent.DAL;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using InterventWebApp.Helpers;
using InterventWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace InterventWebApp
{
    public class ParticipantController : BaseController
    {
        private readonly AppSettings _appSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public ParticipantController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        #region Dashboard
        [Authorize]
        public ActionResult Stream(bool? ForceParticipant, bool? myself, bool? byClick, bool? openProfile)
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.SSOState)))
            {
                int userId;
                if (HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue)
                    userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                else
                    userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
                var samlPartResponse = ParticipantUtility.ReadUserParticipation(userId);
                HttpContext.Session.SetString(SessionContext.ParticipantName, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(samlPartResponse.user.FirstName.ToLower()) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(samlPartResponse.user.LastName.ToLower()));
                HttpContext.Session.SetString(SessionContext.ParticipantEmail, samlPartResponse.user.Email.ToString());
                HttpContext.Session.SetString(SessionContext.DOB, samlPartResponse.user.DOB.ToString());
                if (samlPartResponse.user.Gender.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.Gender, samlPartResponse.user.Gender.Value);
                if (!string.IsNullOrEmpty(samlPartResponse.user.Zip))
                    HttpContext.Session.SetString(SessionContext.Zip, samlPartResponse.user.Zip);
                return RedirectToAction("Login", "Saml");
            }
            if (ForceParticipant.HasValue && ForceParticipant.Value)
            {
                if (myself.HasValue && myself.Value)
                {
                    ClearParticipantSession();
                    HttpContext.Session.SetInt32(SessionContext.ParticipantId, Convert.ToInt32(User.UserId()));
                }
                HttpContext.Session.SetString(SessionContext.IsParticipantView, "true");
            }
            else if (ForceParticipant.HasValue && !ForceParticipant.Value)
            {
                var participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                ClearParticipantSession();
                return RedirectToAction("ParticipantProfile", "Participant", new { Id = participantId });
            }
            else if (CommonUtility.HasAdminRole(User.RoleCode()))
            {
                HttpContext.Session.SetInt32(SessionContext.AdminId, Convert.ToInt32(User.UserId()));
                return RedirectToAction("AdminDashboard", "Admin");
            }
            else
            {
                HttpContext.Session.SetInt32(SessionContext.ParticipantId, Convert.ToInt32(User.UserId()));
            }

            HttpContext.Session.SetString(SessionContext.SingleSignOn, User.SingleSignOn());
            HttpContext.Session.SetString(SessionContext.MobileSignOn, User.MobileSignOn());

            int UserId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            ParticipantDashboardModel model = new ParticipantDashboardModel();
            model.visitedTab = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.VisitedTab) != null ? HttpContext.Session.GetString(SessionContext.VisitedTab) : false);
            model.openProfile = openProfile.HasValue ? openProfile.Value : false;
            var partResponse = ParticipantUtility.ReadUserParticipation(UserId);
            if (partResponse.user != null)
            {
                if (partResponse.user.Gender.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.Gender, partResponse.user.Gender.Value);
                HttpContext.Session.SetString(SessionContext.DOB, partResponse.user.DOB.ToString());
                partResponse.user.FirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(partResponse.user.FirstName.ToLower());
                partResponse.user.LastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(partResponse.user.LastName.ToLower());
                HttpContext.Session.SetString(SessionContext.ParticipantName, partResponse.user.FirstName + " " + partResponse.user.LastName);
                HttpContext.Session.SetString(SessionContext.ParticipantEmail, partResponse.user.Email.ToString());
                if (!string.IsNullOrEmpty(partResponse.user.UniqueId))
                    HttpContext.Session.SetString(SessionContext.UniqueId, partResponse.user.UniqueId);
                if (partResponse.user.State.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.StateId, partResponse.user.State.Value);
                if (partResponse.user.OrganizationId > 0)
                    HttpContext.Session.SetInt32(SessionContext.OrganizationId, partResponse.user.OrganizationId);
                HttpContext.Session.SetInt32(SessionContext.SouthUniversityOrgId, _appSettings.SouthUniversityOrgId);
                HttpContext.Session.SetString(SessionContext.OrganizationName, partResponse.user.Organization.Name.ToString());
                if (!string.IsNullOrEmpty(partResponse.user.Organization.Code))
                    HttpContext.Session.SetString(SessionContext.OrganizationCode, partResponse.user.Organization.Code.ToString());
                HttpContext.Session.SetString(SessionContext.OrgContactNumber, partResponse.user.Organization.ContactNumber.ToString());
                HttpContext.Session.SetString(SessionContext.OrgContactEmail, partResponse.user.Organization.ContactEmail.ToString());
                if (partResponse.user.Organization.SSO.HasValue)
                    HttpContext.Session.SetString(SessionContext.SSO, partResponse.user.Organization.SSO.ToString());
                HttpContext.Session.SetString(SessionContext.TermsSSO, partResponse.user.Organization.TermsForSSO.ToString());
                if (partResponse.user.Organization.IntegrationWith.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.IntegrationWith, partResponse.user.Organization.IntegrationWith.Value);
                HttpContext.Session.SetString(SessionContext.IsMediOrbisUser, ParticipantUtility.IsMediOrbisUser(partResponse.user.OrganizationId).ToString());
                if (!string.IsNullOrEmpty(partResponse.user.LanguagePreference))
                {
                    HttpContext.Session.SetString(SessionContext.LanguagePreference, partResponse.user.LanguagePreference.ToString());
                    HttpContext.Session.SetString(SessionContext.ParticipantLanguagePreference, partResponse.user.LanguagePreference.ToString());
                }
                if (!string.IsNullOrEmpty(partResponse.user.TimeZone))
                {
                    HttpContext.Session.SetString(SessionContext.ParticipantTimeZone, partResponse.user.TimeZone.ToString());
                    HttpContext.Session.SetString(SessionContext.ParticipantTimeZoneName, Translate.Message(partResponse.user.TimeZoneName));
                }
                HttpContext.Session.SetInt32(SessionContext.Unit, partResponse.user.Unit.HasValue ? (int)partResponse.user.Unit.Value : (int)Unit.Imperial);
                HttpContext.Session.SetString(SessionContext.UserStatus, string.IsNullOrEmpty(partResponse.UserStatus) ? "" : partResponse.UserStatus);
                HttpContext.Session.SetString(SessionContext.TermsAccepted, partResponse.user.TermsAccepted.ToString());
                if (HttpContext.Session.GetString(SessionContext.DateFormat) == null)
                    HttpContext.Session.SetString(SessionContext.DateFormat, partResponse.user.Country1 != null ? partResponse.user.Country1.DateFormat : "MM/dd/yyyy");
                HttpContext.Session.SetInt32(SessionContext.SessionTimeout, _appSettings.SessionTimeOut);
                int hraId = partResponse.HRA != null ? partResponse.HRA.Id : 0;
                HttpContext.Session.SetString(SessionContext.isPregnant, CommonUtility.GetPregancyStatus(hraId, UserId, DateTime.Now).isPregnant.ToString());
                if (partResponse.user.OrganizationId == _appSettings.TeamsBPOrgId)
                {
                    var externalReport = ParticipantUtility.ListExternalReports(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).reportLists.Where(x => !x.ReportName.Contains("provider")).OrderByDescending(y => y.Id).FirstOrDefault();
                    if (externalReport != null)
                        model.externalReportId = externalReport.Id;
                }
            }
            TempData["TermsPage"] = false;
            if (HttpContext.Session.GetString(SessionContext.TermsSSO) != null && HttpContext.Session.GetString(SessionContext.TermsSSO) == "True")
            {
                if (HttpContext.Session.GetString(SessionContext.TermsAccepted) == null || HttpContext.Session.GetString(SessionContext.TermsAccepted) == "False")
                {
                    TempData["TermsPage"] = true;
                    return RedirectToAction("Terms", "Participant");
                }
            }
            if (partResponse.hasActivePortal)
            {
                HttpContext.Session.SetString(SessionContext.HasActivePortal, true.ToString());
                if (partResponse.HRA != null)
                    HttpContext.Session.SetInt32(SessionContext.HRAId, partResponse.HRA.Id);
                HttpContext.Session.SetInt32(SessionContext.ParticipantPortalId, partResponse.Portal.Id);
                HttpContext.Session.SetString(SessionContext.HRAPageSeq, partResponse.Portal.HAPageSeq.ToString());
                HttpContext.Session.SetString(SessionContext.CoachingProgram, (partResponse.Portal.HasCoachingProgram && HasCoachingConditions()).ToString());
                HttpContext.Session.SetString(SessionContext.SelfHelpProgram, partResponse.Portal.HasSelfHelpProgram.ToString());
                if (partResponse.Portal.HRAValidity.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.HRAValidity, partResponse.Portal.HRAValidity.Value);
                HttpContext.Session.SetString(SessionContext.ShowProgramOption, partResponse.Portal.ShowProgramOption.ToString());
                HttpContext.Session.SetString(SessionContext.SelfScheduling, partResponse.Portal.SelfScheduling.ToString());
                if (partResponse.Portal.HRAVer.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.HRAVer, partResponse.Portal.HRAVer.Value);
                HttpContext.Session.SetString(SessionContext.ShowPricing, partResponse.Portal.ShowPricing.ToString());
                HttpContext.Session.SetString(SessionContext.ClientNameInReport, partResponse.Portal.ClientNameInReport.ToString());
                HttpContext.Session.SetString(SessionContext.MailScoreCard, partResponse.Portal.MailScoreCard.ToString());
                HttpContext.Session.SetString(SessionContext.CarePlan, partResponse.Portal.CarePlan.ToString());
                HttpContext.Session.SetString(SessionContext.Challenges, partResponse.Portal.PortalIncentives.Any(x => x.IsActive).ToString());
                HttpContext.Session.SetString(SessionContext.AssignPrograms, partResponse.Portal.AssignPrograms.ToString());
                if (partResponse.Portal.FollowUpValidity.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.FollowUpValidity, partResponse.Portal.FollowUpValidity.Value);
                HttpContext.Session.SetString(SessionContext.ShowPostmenopausal, partResponse.Portal.ShowPostmenopausal.ToString());
                if (partResponse.Portal.HasHRA.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.HasHRA, (int)partResponse.Portal.HasHRA.Value);
                HttpContext.Session.SetString(SessionContext.NeedCareplanApproval, partResponse.Portal.NeedCareplanApproval.ToString());
                HttpContext.Session.SetString(SessionContext.CareplanPath, CommonUtility.NullCheck(partResponse.Portal.CareplanPath));
                if (partResponse.Portal.CarePlanType.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.CarePlanType, partResponse.Portal.CarePlanType.Value);
                model.portalStartDate = partResponse.Portal.StartDate;
                model.ShowSelfScheduling = ShowSelfScheduling();
                model.IsRescheduling = IsRescheduling();

                HttpContext.Session.SetInt32(SessionContext.MessageCount, MessageUtility.GetMessageCountForDashboard(UserId, false, null, false, _appSettings.SystemAdminId).MessageBoardCount);
                if (partResponse.appointment != null)
                {
                    HttpContext.Session.SetInt32(SessionContext.NextApptId, partResponse.appointment.Id);
                    var timeZone = "";
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)))
                        timeZone = HttpContext.Session.GetString(SessionContext.ParticipantTimeZone);
                    else
                        timeZone = User.TimeZone();
                    if (!string.IsNullOrEmpty(timeZone))
                        HttpContext.Session.SetString(SessionContext.NextApptDate, (TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(partResponse.appointment.Date), TimeZoneInfo.FindSystemTimeZoneById(timeZone)).ToString()));
                    else
                        HttpContext.Session.SetString(SessionContext.NextApptDate, partResponse.appointment.Date.ToString());
                }
                bool isMediOrbisUser = !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.IsMediOrbisUser)) && Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.IsMediOrbisUser));
                if (isMediOrbisUser && HttpContext.Session.GetString(SessionContext.ProgramCode) == null && partResponse.usersinProgram != null && partResponse.usersinProgram.IsActive)
                {
                    HttpContext.Session.SetString(SessionContext.ProgramCode, partResponse.usersinProgram.ProgramsinPortal.program.Code.ToString());
                }

                if ((partResponse.HRA != null && partResponse.HRA.CompleteDate.HasValue)
                    || (partResponse.usersinProgram != null && !isMediOrbisUser)
                    || (partResponse.usersinProgram != null && isMediOrbisUser && partResponse.usersinProgram.IsActive))
                {
                    if (partResponse.HRA != null && partResponse.HRA.CompleteDate.HasValue)
                    {
                        HttpContext.Session.SetString(SessionContext.HRACompleteDate, partResponse.HRA.CompleteDate.Value.ToString());
                    }
                    if (partResponse.usersinProgram != null)
                    {
                        HttpContext.Session.SetInt32(SessionContext.UserinProgramId, partResponse.usersinProgram.Id);
                        HttpContext.Session.SetInt32(SessionContext.ProgramsInPortalId, partResponse.usersinProgram.ProgramsinPortalId);
                        HttpContext.Session.SetInt32(SessionContext.ProgramType, partResponse.usersinProgram.ProgramsinPortal.program.ProgramType);
                        HttpContext.Session.SetInt32(SessionContext.AssignedFollowUp, partResponse.usersinProgram.AssignedFollowUp);
                        if (partResponse.usersinProgram.FollowUps != null && partResponse.usersinProgram.FollowUps.Count() > 0)
                        {
                            if (partResponse.usersinProgram.FollowUps.Count() >= partResponse.usersinProgram.AssignedFollowUp)
                                HttpContext.Session.SetInt32(SessionContext.FollowUpId, partResponse.usersinProgram.FollowUps.OrderByDescending(x => x.Id).FirstOrDefault().Id);
                            if (partResponse.usersinProgram.AssignedFollowUp == partResponse.usersinProgram.FollowUps.Count())
                                HttpContext.Session.SetString(SessionContext.FollowUpCompleteDate, partResponse.usersinProgram.FollowUps.OrderByDescending(x => x.Id).FirstOrDefault().CompleteDate.ToString());
                        }
                    }

                    if (!(partResponse.Portal.HasCoachingProgram && HasCoachingConditions()) && !partResponse.Portal.HasSelfHelpProgram)
                        return RedirectToAction("ReportsDashboard", "Reports");
                    else if (!CommonUtility.HasAdminRole(User.RoleCode()) && (!byClick.HasValue || (byClick.HasValue && byClick.Value == false))
                            && partResponse.user.LastVisited != null && !partResponse.user.LastVisited.Contains("Participant/Stream") && partResponse.user.UserLogs.Where(x => x.IsSuccess == null || x.IsSuccess.HasValue && x.IsSuccess.Value).OrderByDescending(x => x.Id).Skip(1).FirstOrDefault() != null
                            && partResponse.user.UserLogs.Where(x => x.IsSuccess == null || x.IsSuccess.HasValue && x.IsSuccess.Value).OrderByDescending(x => x.Id).Skip(1).FirstOrDefault().LastAccessedOn.AddHours(1) >= DateTime.UtcNow)
                    {
                        CommonUtility.UpdateLastVisited(null, Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value));
                        var host = HttpContext.Request.Host;
                        return Redirect("http://" + host + "" + partResponse.user.LastVisited + "");
                    }
                    else
                    {
                        return View(model);
                    }
                }
                else if (HttpContext.Session.GetInt32(SessionContext.HasHRA) == (int)HRAStatus.Optional || HttpContext.Session.GetInt32(SessionContext.HasHRA) == (int)HRAStatus.No)
                {
                    if (AccountUtility.CheckProfileCompleted(partResponse.user).profileCompleted)
                    {
                        if (ShowSelfScheduling() && (!isMediOrbisUser || (isMediOrbisUser && HttpContext.Session.GetString(SessionContext.ProgramCode) != null && HttpContext.Session.GetString(SessionContext.ProgramCode) == "COACH")))
                            return RedirectToAction("MyCoach", "Participant", new { initialStage = true });
                        else if (!HttpContext.Session.GetInt32(SessionContext.UserinProgramId).HasValue && HttpContext.Session.GetString(SessionContext.SelfHelpProgram) != null && Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.SelfHelpProgram)) == true && (!HttpContext.Session.GetInt32(SessionContext.ProgramType).HasValue || HttpContext.Session.GetInt32(SessionContext.ProgramType) != 1))
                            return RedirectToAction("CurrentPrograms", "Program", new { initialStage = true });
                        return View(model);
                    }
                    else
                        return RedirectToAction("InitialDashboardNew");
                }
                else
                {
                    return RedirectToAction("InitialDashboard");
                }
            }
            else
            {
                var userDetails = PortalUtility.ReadParticipantInactivePortal(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
                var portalDetails = userDetails.portal;
                if (userDetails.hra != null)
                {
                    HttpContext.Session.SetInt32(SessionContext.HRAId, userDetails.hra.Id);
                    if (userDetails.hra.CompleteDate.HasValue)
                        HttpContext.Session.SetString(SessionContext.HRACompleteDate, userDetails.hra.CompleteDate.Value.ToString());
                }
                if (userDetails.usersinProgram != null)
                {
                    HttpContext.Session.SetInt32(SessionContext.UserinProgramId, userDetails.usersinProgram.Id);
                    HttpContext.Session.SetInt32(SessionContext.ProgramsInPortalId, userDetails.usersinProgram.ProgramsinPortalId);
                    HttpContext.Session.SetInt32(SessionContext.ProgramType, userDetails.usersinProgram.ProgramsinPortal.program.ProgramType);
                    HttpContext.Session.SetInt32(SessionContext.AssignedFollowUp, userDetails.usersinProgram.AssignedFollowUp);
                    if (userDetails.usersinProgram.FollowUps != null && userDetails.usersinProgram.FollowUps.Count() > 0)
                    {
                        if (userDetails.usersinProgram.FollowUps.Count() >= userDetails.usersinProgram.AssignedFollowUp)
                            HttpContext.Session.SetInt32(SessionContext.FollowUpId, userDetails.usersinProgram.FollowUps.OrderByDescending(x => x.Id).FirstOrDefault().Id);
                        if (userDetails.usersinProgram.AssignedFollowUp == userDetails.usersinProgram.FollowUps.Count())
                            HttpContext.Session.SetString(SessionContext.FollowUpCompleteDate, userDetails.usersinProgram.FollowUps.OrderByDescending(x => x.Id).FirstOrDefault().CompleteDate.ToString());
                    }
                }
                HttpContext.Session.SetString(SessionContext.HasActivePortal, false.ToString());
                HttpContext.Session.SetInt32(SessionContext.InActiveParticipantPortalId, portalDetails.Id);
                HttpContext.Session.SetInt32(SessionContext.ParticipantPortalId, portalDetails.Id);
                HttpContext.Session.SetString(SessionContext.HRAPageSeq, portalDetails.HAPageSeq.ToString());
                HttpContext.Session.SetString(SessionContext.CoachingProgram, (portalDetails.HasCoachingProgram && HasCoachingConditions()).ToString());
                HttpContext.Session.SetString(SessionContext.SelfHelpProgram, portalDetails.HasSelfHelpProgram.ToString());
                if (portalDetails.HRAValidity.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.HRAValidity, portalDetails.HRAValidity.Value);
                HttpContext.Session.SetString(SessionContext.ShowProgramOption, portalDetails.ShowProgramOption.ToString());
                HttpContext.Session.SetString(SessionContext.SelfScheduling, portalDetails.SelfScheduling.ToString());
                if (portalDetails.HRAVer.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.HRAVer, (int)portalDetails.HRAVer.Value);
                HttpContext.Session.SetString(SessionContext.ShowPricing, portalDetails.ShowPricing.ToString());
                HttpContext.Session.SetString(SessionContext.ClientNameInReport, portalDetails.ClientNameInReport.ToString());
                HttpContext.Session.SetString(SessionContext.MailScoreCard, portalDetails.MailScoreCard.ToString());
                HttpContext.Session.SetString(SessionContext.CarePlan, portalDetails.CarePlan.ToString());
                HttpContext.Session.SetString(SessionContext.Challenges, (portalDetails.PortalIncentives.Any(x => x.IsActive)).ToString());
                HttpContext.Session.SetString(SessionContext.AssignPrograms, portalDetails.AssignPrograms.ToString());
                if (portalDetails.FollowUpValidity.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.FollowUpValidity, portalDetails.FollowUpValidity.Value);
                HttpContext.Session.SetString(SessionContext.ShowPostmenopausal, portalDetails.ShowPostmenopausal.ToString());
                if (portalDetails.HasHRA.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.HasHRA, (int)portalDetails.HasHRA.Value);
                HttpContext.Session.SetString(SessionContext.NeedCareplanApproval, portalDetails.NeedCareplanApproval.ToString());
                HttpContext.Session.SetString(SessionContext.CareplanPath, portalDetails.CareplanPath.ToString());
                if (portalDetails.CarePlanType.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.CarePlanType, portalDetails.CarePlanType.Value);
                model.portalStartDate = portalDetails.StartDate;
                HttpContext.Session.SetInt32(SessionContext.MessageCount, MessageUtility.GetMessageCountForDashboard(UserId, false, null, false, _appSettings.SystemAdminId).MessageBoardCount);

                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult KeepSession()
        {
            return Json(new { Result = "OK" });
        }

        [Authorize]
        public ActionResult InitialDashboard(bool? openProfile)
        {
            if (!HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue)
                return RedirectToAction("NotAuthorized", "Account");

            ParticipantDashboardModel model = new ParticipantDashboardModel();
            int participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            var partResponse = ParticipantUtility.ReadUserParticipation(participantId);
            //if(partResponse.user != null)
            //    Session[SessionContext.ParticipantEmail] = partResponse.user.Email;
            if (partResponse.HRA != null)
            {
                HttpContext.Session.SetInt32(SessionContext.HRAId, partResponse.HRA.Id);
                model.hraPercent = HRAUtility.GetHRACompletionPercent(HttpContext.Session.GetString(SessionContext.HRAPageSeq), partResponse.HRA.HAPageSeqDone);
            }
            if (partResponse.user.Complete.HasValue)
                model.profileComplete = partResponse.user.Complete.Value;

            if (partResponse.user != null)
                model.ParticipantFirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(partResponse.user.FirstName.ToLower());
            model.PreviousPortalAvailable = false;
            var result = ParticipantUtility.GetPrevYearStatus(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : "", User.TimeZone());
            if (result.prevPortal == true)
                model.PreviousPortalAvailable = true;
            model.openProfile = openProfile.HasValue ? openProfile.Value : false;
            model.MailScoreCard = HttpContext.Session.GetString(SessionContext.MailScoreCard) == "true";
            model.UserinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId);
            model.AdminId = HttpContext.Session.GetInt32(SessionContext.AdminId);
            model.ParticipantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.ParticipantEmail = HttpContext.Session.GetString(SessionContext.ParticipantEmail);
            model.UserId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.isSouthUniversity = HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue && HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value == _appSettings.SouthUniversityOrgId;
            return View(model);
        }

        [Authorize]
        public ActionResult UserSidebar(ParticipantDashboardModel model)
        {
            return PartialView("_UserSidebar", model);
        }

        [Authorize]
        public ActionResult InitialDashboardNew()
        {
            InitialDashboardModel model = new InitialDashboardModel();
            var portal = PortalUtility.ReadPortal(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value).portal;
            model.dashboardContent = translate(portal.WelcomeText);
            return View(model);
        }

        [Authorize]
        public async Task<ActionResult> ProgramIntro()
        {
            UserProfileModel model = new UserProfileModel();
            model.Countries = CommonUtility.ListCountries().OrderBy(t => t.Code == "US" ? 1 : 2).ThenBy(t => t.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            model.TimeZones = CommonUtility.GetTimeZones(null).TimeZones.Select(x => new SelectListItem { Text = Translate.Message(x.TimeZoneDisplay), Value = x.Id.ToString() })
                .OrderBy(t => t.Text);
            model.GenderList = ListOptions.GetGenderList(null).Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.LanguagePreferences = CommonUtility.GetPortalLanguages(null, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.LanguageCode });
            model.Units = ListOptions.GetUnits().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.States = Enumerable.Empty<SelectListItem>();
            model.ProviderDropDown = PortalUtility.ReadPortal(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value)).portal.ProviderDetails == (byte)ProviderDetails.DropDown;
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.ShowSelfScheduling = ShowSelfScheduling();
            model.ShowProgram = !HttpContext.Session.GetInt32(SessionContext.UserinProgramId).HasValue && HttpContext.Session.GetString(SessionContext.SelfHelpProgram) != null && HttpContext.Session.GetString(SessionContext.SelfHelpProgram) == "true" && (!HttpContext.Session.GetInt32(SessionContext.ProgramType).HasValue || HttpContext.Session.GetInt32(SessionContext.ProgramType) != 1);
            UserDto user = new UserDto();
            int userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            var response = await AccountUtility.GetUser(_userManager, null, null, userId, null, null);
            var completedStatus = AccountUtility.CheckProfileCompleted(response.User);
            model.basicDetailsCompeleted = completedStatus.basicDetails;
            model.contactDetailsCompeleted = completedStatus.contactDetails;
            user.Id = response.User.Id;
            user.FirstName = response.User.FirstName;
            user.LastName = response.User.LastName;
            if (response.User.DOB.HasValue)
            {
                user.DOB = response.User.DOB;
            }
            user.Gender = response.User.Gender;
            user.Address = response.User.Address;
            user.Address2 = response.User.Address2;
            user.City = response.User.City;
            user.State = response.User.State;
            user.Country = response.User.Country;
            user.Zip = response.User.Zip;
            if (!response.User.Email.EndsWith("@samlnoemail.com"))
                user.Email = response.User.Email;
            user.HomeNumber = response.User.HomeNumber;
            user.TimeZoneId = response.User.TimeZoneId;
            user.LanguagePreference = response.User.LanguagePreference;
            user.Text = response.User.Text;
            user.Unit = response.User.Unit;
            if (response.User.UserDoctorInfoes != null && response.User.UserDoctorInfoes.Where(x => x.Active == true).Count() > 0)
            {
                var doctorInfo = response.User.UserDoctorInfoes.Where(x => x.Active == true).FirstOrDefault();
                user.UserDoctorInfoes = new List<UserDoctorInfoDto>();
                UserDoctorInfoDto userDoctorInfo = new UserDoctorInfoDto();
                userDoctorInfo.ProviderId = doctorInfo.ProviderId;
                user.UserDoctorInfoes.Add(userDoctorInfo);
            }
            model.ProvidersList = CommonUtility.GetProvidersList(HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value).Where(x => x.Active || (response.User.UserDoctorInfoes.Count > 0 && x.Id == response.User.UserDoctorInfoes[0].ProviderId)).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(y => y.Text);
            if (user.Country.HasValue)
            {
                model.States = CommonUtility.ListStates(user.Country.Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            }
            model.user = user;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public JsonResult UpdateDashboardMessage(int id)
        {
            var response = ParticipantUtility.UpdateDashboardMessage(id, null, null, false, null);
            return Json(new
            {
                Result = "OK",
                Records = response.success
            });
        }

        #endregion
        [Authorize]
        public ActionResult MyCoach(bool? initialStage)
        {

            ViewData["InitialStage"] = initialStage;
            if (HttpContext.Session.GetString(SessionContext.CoachingProgram) == null || Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.CoachingProgram)) == false)
                return RedirectToAction("Stream", "Participant");
            if (IsRescheduling())
                return RedirectToAction("Reschedule", "Scheduler");
            MyCoachModel model = new MyCoachModel();
            if (HttpContext.Session.GetString(SessionContext.isPregnant) != null && Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.isPregnant)) == true)
            {
                model.Specializations = AccountUtility.ListSpecialization(true, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value).Where(x => x.Id == Convert.ToInt32(Constants.MaternityManagement)).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageId), Value = x.Id.ToString(), Selected = true });
            }
            else
            {
                model.Specializations = AccountUtility.ListSpecialization(true, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageId), Value = x.Id.ToString() });
            }
            model.Languages = CommonUtility.GetPortalLanguages(null, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.Id.ToString() });
            model.Timezones = CommonUtility.GetTimeZones(null).TimeZones.Select(x => new SelectListItem { Text = Translate.Message(x.TimeZone1), Value = x.TimeZoneId });
            model.participantTimeZone = HttpContext.Session.GetString(SessionContext.ParticipantTimeZone);
            model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            model.BaseUrl = _appSettings.EmailUrl;
            return View(model);
        }

        [Authorize]
        public JsonResult GetCoachInfo(int coachId)
        {
            var coach = AdminUtility.ReadAdmin(coachId).admin;
            return Json(new
            {
                Result = "OK",
                picture = coach.user.Picture,
                profile = coach.user.AdminProperty == null ? "" : coach.user.AdminProperty.Profile
            });
        }

        [Authorize]
        public JsonResult GetFilteredCoachList(string coachName, int? speciality, string language, string date, string time, bool? byCoach, string timezone)
        {
            var response = SchedulerUtility.GetFilteredCoachList(coachName, speciality, language, date, time, byCoach, string.IsNullOrEmpty(timezone) ? User.TimeZone() : timezone, HttpContext.Session.GetString(SessionContext.isPregnant) == null ? Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.isPregnant)) : false, HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue ? HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value : null, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.StateId).Value);
            for (int i = 0; i < response.coachList.Count; i++)
            {
                if (!string.IsNullOrEmpty(response.coachList[i].Speciality))
                {
                    string specializations = "";
                    string[] specialityList = null;
                    specialityList = response.coachList[i].Speciality.Replace(" ", String.Empty).Split(',');
                    foreach (var Speciality in specialityList)
                    {

                        specializations = specializations + (specializations == "" ? "" : ", ") + Translate.Message(Speciality);
                    }
                    response.coachList[i].Speciality = specializations;
                }
            }
            return Json(new
            {
                Result = "OK",
                coaches = response
            });
        }

        [Authorize]
        public ActionResult SelectProgram()
        {
            SelectProgramModel model = new SelectProgramModel();
            var response = HRAUtility.ReadHRA(HttpContext.Session.GetInt32(SessionContext.HRAId).Value);
            if (response != null && response.hra.HealthNumbers != null && response.hra.HealthNumbers.CAC.HasValue)
                model.CACScore = (float)response.hra.HealthNumbers.CAC;
            model.conditions = ReportUtility.GetConditionsNeedHelp(HttpContext.Session.GetInt32(SessionContext.HRAId)).conditions;
            var portalResponse = PortalUtility.ReadPortal(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value));
            model.ReportText = string.IsNullOrEmpty(portalResponse.portal.ReportText) ? "" : Translate.Message(portalResponse.portal.ReportText);
            model.ProgramText = string.IsNullOrEmpty(portalResponse.portal.ProgramText) ? "" : Translate.Message(portalResponse.portal.ProgramText);
            model.ContactText = string.IsNullOrEmpty(portalResponse.portal.ContactText) ? "" : string.Format(Translate.Message(portalResponse.portal.ContactText), HttpContext.Session.GetString(SessionContext.OrgContactNumber));
            string timeZone;
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)))
                timeZone = HttpContext.Session.GetString(SessionContext.ParticipantTimeZone);
            else
                timeZone = User.TimeZone();
            var appointments = SchedulerUtility.GetAppointments(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone())), null, null, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, timeZone, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null);
            if (appointments != null && appointments.Appointments != null && appointments.Appointments.Count > 0)
                model.FutureAppointmentDate = CommonUtility.dateFormater(Convert.ToDateTime(appointments.Appointments[0].Date + " " + appointments.Appointments[0].StartTime), true, HttpContext.Session.GetString(SessionContext.DateFormat)) + " (" + HttpContext.Session.GetString(SessionContext.ParticipantTimeZoneName) + ")";
            model.hasCoachingConditions = HasCoachingConditions();
            model.participantName = HttpContext.Session.GetString(SessionContext.ParticipantName);
            model.userStatus = HttpContext.Session.GetString(SessionContext.UserStatus) != null ? HttpContext.Session.GetString(SessionContext.UserStatus) : "";
            model.userinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId).HasValue ? HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value : null;
            model.nextApptDate = HttpContext.Session.GetString(SessionContext.NextApptDate) != null ? HttpContext.Session.GetString(SessionContext.NextApptDate) : "";
            model.coachingProgram = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.CoachingProgram) != null ? HttpContext.Session.GetString(SessionContext.CoachingProgram) : false);
            model.selfHelpProgram = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.SelfHelpProgram) != null ? HttpContext.Session.GetString(SessionContext.SelfHelpProgram) : false);
            model.showProgramOption = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.ShowProgramOption) != null ? HttpContext.Session.GetString(SessionContext.ShowProgramOption) : false);
            model.participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.organizationId = HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value;
            model.programType = HttpContext.Session.GetInt32(SessionContext.ProgramType);
            model.orgContactEmail = HttpContext.Session.GetString(SessionContext.OrgContactEmail);
            model.isSouthUniversity = HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue && HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value == _appSettings.SouthUniversityOrgId;
            return View(model);
        }

        [Authorize]
        public ActionResult NextProgramOption()
        {
            return View();
        }

        [Authorize]
        public ActionResult ProgramOptions(bool Coaching, bool? hasCoachingConditions)
        {
            ProgramOptionsModel model = new ProgramOptionsModel();
            var PortalId = GetParticipantPortalId();
            var portal = PortalUtility.ReadPortal(PortalId).portal;
            if (portal != null)
            {
                model.hasSelfHelp = portal.HasSelfHelpProgram == true;
                model.hasCoaching = portal.HasCoachingProgram == true;
            }
            if (Coaching)
            {
                model.showCoaching = true;
                model.showSelfHelp = false;
            }
            else
            {
                model.showCoaching = false;
                model.showSelfHelp = true;
            }
            if (hasCoachingConditions.HasValue)
                model.hasCoachingConditions = hasCoachingConditions.Value;
            else if (Coaching)
                model.hasCoachingConditions = HasCoachingConditions();
            var prevStatus = ParticipantUtility.GetPrevYearStatus(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : "", User.TimeZone(), true);
            if (prevStatus.prevPortal == true)
            {
                int participantLevel = 0;
                if (prevStatus.hra == null || !prevStatus.hra.CompleteDate.HasValue)
                    participantLevel = Coachinglevel.IncompleteHRA;
                else if (prevStatus.hra != null)
                {
                    if (prevStatus.prevCompCoaching)
                        participantLevel = Coachinglevel.CompletedCoaching;
                    else if (prevStatus.prevCompFirstCoaching)
                        participantLevel = Coachinglevel.FirstCoaching;
                    else if (prevStatus.prevCompBiometrics)
                        participantLevel = Coachinglevel.HRAandBiometrics;
                    else if (prevStatus.hra.CompleteDate.HasValue)
                        participantLevel = Coachinglevel.HRAOnly;
                }
                switch (participantLevel)
                {
                    case Coachinglevel.IncompleteHRA:
                        {
                            model.coachText = portal.IncompleteHRA;
                        }
                        break;

                    case Coachinglevel.HRAOnly:
                        {
                            model.coachText = portal.HRAOnly;
                        }
                        break;
                    case Coachinglevel.HRAandBiometrics:
                        {
                            model.coachText = portal.HRAandBiometrics;
                        }
                        break;
                    case Coachinglevel.FirstCoaching:
                        {
                            model.coachText = portal.FirstCoaching;
                        }
                        break;
                    case Coachinglevel.CompletedCoaching:
                        {
                            model.coachText = portal.CompletedCoaching;
                        }
                        break;
                    case Coachinglevel.FirstTimeHRA:
                        {
                            model.coachText = portal.FirstTimeHRA;
                        }
                        break;
                }
            }
            else
                model.coachText = portal.FirstTimeHRA;
            model.coachText = translate(model.coachText);
            model.selfScheduling = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.SelfScheduling) != null ? HttpContext.Session.GetString(SessionContext.SelfScheduling) : false);
            model.programType = HttpContext.Session.GetInt32(SessionContext.ProgramType);
            return PartialView("_ProgramOptions", model);
        }

        private string translate(string text)
        {
            var sText = text.Split('+');
            var translatedText = "";
            foreach (var t in sText)
            {
                if (t.ToLower().Trim().StartsWith("translate"))
                {
                    var code = t.Split('"')[1];
                    translatedText += Translate.Message(code);
                }
                else
                {
                    translatedText += t;
                }
            }
            return translatedText;
        }

        [Authorize]
        public ActionResult CheckRisk()
        {
            return View("CheckRisk", GetWhatIf(HttpContext.Session.GetInt32(SessionContext.HRAId).Value));
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AdminCheckRisk(int hraid)
        {
            return PartialView("AdminCheckRisk", GetWhatIf(hraid));
        }

        [Authorize]
        public CheckRiskModel GetWhatIf(int hraid)
        {
            CheckRiskModel model = new CheckRiskModel();
            var report = ReportUtility.ReadHRAReport(hraid);
            var hra = report.hra;
            var MeasurementList = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)));
            var Measurements = MeasurementList.Measurements;
            model.Gender = report.hra.User.Gender;
            if (report.hra.User.Race.HasValue)
                model.Race = report.hra.User.Race.Value;
            model.ASCVD = hra.Goals.ASCVD;
            model.SmokeCig = hra.OtherRiskFactors.SmokeCig;
            model.SBP = hra.HealthNumbers.SBP;
            model.DBP = hra.HealthNumbers.DBP;
            model.HDLText = string.Format(Translate.Message("L327"), Measurements[BioLookup.HDL].MeasurementUnit);
            model.HDL = hra.HealthNumbers.HDL.HasValue ? (float)Math.Round(CommonUtility.ToMetric(hra.HealthNumbers.HDL.Value, BioLookup.HDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) : hra.HealthNumbers.HDL;
            model.HDLMin = CommonUtility.ToMetric(5, BioLookup.HDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
            model.HDLMax = CommonUtility.ToMetric(160, BioLookup.HDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
            model.ASCVDHDLMin = CommonUtility.ToMetric(20, BioLookup.HDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
            model.ASCVDHDLMax = CommonUtility.ToMetric(100, BioLookup.HDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
            model.ASCVDSBPMin = CommonUtility.ToMetric(90, 0, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
            model.ASCVDSBPMax = CommonUtility.ToMetric(200, 0, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
            model.CholesterolText = string.Format(Translate.Message("L324"), Measurements[BioLookup.Cholesterol].MeasurementUnit);
            model.TotalChol = hra.HealthNumbers.TotalChol.HasValue ? (float)Math.Round(CommonUtility.ToMetric(hra.HealthNumbers.TotalChol.Value, BioLookup.Cholesterol, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) : hra.HealthNumbers.TotalChol;
            model.TotalCholMin = CommonUtility.ToMetric(50, BioLookup.Cholesterol, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
            model.TotalCholMax = CommonUtility.ToMetric(999, BioLookup.Cholesterol, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
            model.ASCVDCholMin = CommonUtility.ToMetric(130, BioLookup.Cholesterol, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
            model.ASCVDCholMax = CommonUtility.ToMetric(320, BioLookup.Cholesterol, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
            model.ConversionValue = Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric ? Measurements[BioLookup.Cholesterol].ConversionValue : 1;
            model.Diabetes = Convert.ToInt32(hra.Goals.Diabetes);
            model.HighBPMed = hra.MedicalCondition.HighBPMed;
            model.Age = hra.Age.Value;
            return model;
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult Assessment()
        {
            int followUpId = 0, hraId = 0;
            AssessmentModel model = new AssessmentModel();
            var portalId = GetParticipantPortalId();
            if (portalId != 0)
            {
                TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
                var response = PortalUtility.ReadPortal(portalId).portal;
                if (response.Active && (response.HasHRA.Value == (int)HRAStatus.Yes || response.HasHRA.Value == (int)HRAStatus.Optional))
                    model.IsEligibilForHRA = true;
                var followUps = FollowUpUtility.GetAllFollowUps(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).FollowUps;
                var userPrograms = ProgramUtility.GetUserProgramsByPortal(portalId, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).Where(x => x.IsActive || x.InactiveReason == 6);
                var currentCampaignFollowUps = followUps.Where(x => x.UsersinProgram.ProgramsinPortal.PortalId == portalId).OrderByDescending(x => x.StartDate).ToList();
                var portalFollowupTypes = PortalUtility.GetPortalFollowUps(portalId, HttpContext.Session.GetInt32(SessionContext.ProgramType)).portalFollowUps.ToArray();

                var portalFollowups = PortalUtility.GetPortalFollowUps(portalId, HttpContext.Session.GetInt32(SessionContext.ProgramType)).portalFollowUps.ToArray();
                var followupTypeIds = portalFollowups.Select(x => x.FollowupTypeId).ToList();

                List<FollowUpTypeDto> followupTypes = new List<FollowUpTypeDto>();
                followupTypes = CommonUtility.GetFollowUpTypes().Where(x => followupTypeIds.Contains(x.Id)).ToList();
                if (followUps.Count > 0 || (HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp).HasValue && Convert.ToByte(HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp)) > 0))
                {
                    if (HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue)
                    {
                        followUpId = HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value;
                        var currentFollowUp = followUps.Where(x => x.Id == followUpId).FirstOrDefault();
                        model.currentFUStatus = "- " + (HttpContext.Session.GetInt32(SessionContext.ProgramType).Value == (byte)ProgramTypes.SelfHelp ? ProgramTypes.SelfHelp.ToString() : ProgramTypes.Coaching.ToString()) + " (" + followupTypes[Convert.ToByte(HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp)) - 1].Type + " Started: " + TimeZoneInfo.ConvertTimeFromUtc(currentFollowUp.StartDate, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) + (currentFollowUp.CompleteDate.HasValue ? "; Completed: " + TimeZoneInfo.ConvertTimeFromUtc(currentFollowUp.CompleteDate.Value, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) : "") + ")";
                        model.IsFUAssigned = true;
                        currentCampaignFollowUps.Remove(currentFollowUp);
                    }
                    if (HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp).HasValue && Convert.ToByte(HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp)) > currentCampaignFollowUps.Where(x => x.UsersinProgramsId == HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value).Count())
                    {
                        model.IsFUAssigned = true;
                    }
                    if (currentCampaignFollowUps.Count() > 0)
                    {
                        model.FUAssessments = new Dictionary<string, string>();
                        var followUpsProgramWise = currentCampaignFollowUps.GroupBy(x => x.UsersinProgramsId).ToList();

                        foreach (var currentCampaignFollowUp in followUpsProgramWise)
                        {
                            int index = currentCampaignFollowUp.Count() - 1;
                            var userinProgram = ProgramUtility.GetUserinProgramDetails(currentCampaignFollowUp.FirstOrDefault().UsersinProgramsId);
                            var currentCampaignPortalFollowups = PortalUtility.GetPortalFollowUps(portalId, userinProgram.ProgramsinPortal.program.ProgramType).portalFollowUps.ToArray();
                            var currentCampaignFollowupTypeIds = currentCampaignPortalFollowups.Select(x => x.FollowupTypeId).ToList();
                            var currentCampaignFollowupTypes = CommonUtility.GetFollowUpTypes().Where(x => currentCampaignFollowupTypeIds.Contains(x.Id)).ToList();
                            if (followUpsProgramWise[followUpsProgramWise.Count - 1].FirstOrDefault().UsersinProgramsId == currentCampaignFollowUp.FirstOrDefault().UsersinProgramsId)
                            {
                                currentCampaignFollowupTypes = CommonUtility.GetFollowUpTypes().Where(x => currentCampaignFollowupTypeIds.Contains(x.Id)).ToList();
                            }
                            foreach (var followup in currentCampaignFollowUp)
                            {
                                model.FUAssessments.Add(followup.Id.ToString() + "|followup", " - " + (userinProgram.ProgramsinPortal.program.ProgramType.ToString() == ((byte)ProgramTypes.SelfHelp).ToString() ? ProgramTypes.SelfHelp.ToString() : ProgramTypes.Coaching.ToString()) + " (" + currentCampaignFollowupTypes[index].Type + " Started: " + TimeZoneInfo.ConvertTimeFromUtc(followup.StartDate, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) + (followup.CompleteDate.HasValue ? "; Completed: " + TimeZoneInfo.ConvertTimeFromUtc(followup.CompleteDate.Value, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) : "") + ")");
                                --index;
                            }
                        }
                    }
                }
                var pastHRAs = HRAUtility.GetAllHRAs(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).HRAs.OrderByDescending(x => x.PortalId).ToList();
                model.pastAssessments = new Dictionary<string, string>();
                if (pastHRAs.Count > 0)
                {
                    foreach (var hra in pastHRAs)
                    {
                        if (HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue && hra.Id == HttpContext.Session.GetInt32(SessionContext.HRAId).Value)
                        {
                            if (model.FUAssessments == null)
                                model.FUAssessments = new Dictionary<string, string>();
                            if (hra.CompleteDate.HasValue)
                            {
                                model.FUAssessments.Add(hraId.ToString() + "|hra", "(Started: " + TimeZoneInfo.ConvertTimeFromUtc(hra.StartDate, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) + "; Completed: " + TimeZoneInfo.ConvertTimeFromUtc(hra.CompleteDate.Value, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) + ")");
                            }
                            else
                            {
                                model.currentHRAStatus = "(Started: " + TimeZoneInfo.ConvertTimeFromUtc(hra.StartDate, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) + ")";
                            }
                        }
                        var pastFollowUps = followUps.Where(x => (followUpId == 0 || x.Id != followUpId) && x.UsersinProgram.ProgramsinPortal.PortalId == hra.PortalId && !currentCampaignFollowUps.Select(y => y.Id).Contains(x.Id)).OrderByDescending(x => x.StartDate).ToList();
                        if (pastFollowUps != null)
                        {

                            var followUpsProgramWise = pastFollowUps.GroupBy(x => x.UsersinProgramsId).ToList();
                            foreach (var pastCampaignFollowUp in followUpsProgramWise)
                            {
                                int pastFollowupIndex = pastCampaignFollowUp.Count() - 1;
                                var userinProgram = ProgramUtility.GetUserinProgramDetails(pastCampaignFollowUp.FirstOrDefault().UsersinProgramsId);
                                var pastCampaignPortalFollowups = PortalUtility.GetPortalFollowUps(portalId, userinProgram.ProgramsinPortal.program.ProgramType).portalFollowUps.ToArray();
                                var pastCampaignFollowupTypeIds = pastCampaignPortalFollowups.Select(x => x.FollowupTypeId).ToList();
                                var pastCampaignFollowupTypes = CommonUtility.GetFollowUpTypes().Where(x => pastCampaignFollowupTypeIds.Contains(x.Id)).ToList();

                                if (followUpsProgramWise[followUpsProgramWise.Count - 1].FirstOrDefault().UsersinProgramsId == pastCampaignFollowUp.FirstOrDefault().UsersinProgramsId)
                                    pastCampaignFollowupTypes = CommonUtility.GetFollowUpTypes().Where(x => pastCampaignFollowupTypeIds.Contains(x.Id)).ToList();
                                foreach (var followup in pastCampaignFollowUp)
                                {
                                    model.pastAssessments.Add(followup.Id.ToString() + "|followup", " - " + (userinProgram.ProgramsinPortal.program.ProgramType.ToString() == ((byte)ProgramTypes.SelfHelp).ToString() ? ProgramTypes.SelfHelp.ToString() : ProgramTypes.Coaching.ToString()) + " (" + pastCampaignFollowupTypes[pastFollowupIndex].Type + " Started: " + TimeZoneInfo.ConvertTimeFromUtc(followup.StartDate, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) + (followup.CompleteDate.HasValue ? "; Completed: " + TimeZoneInfo.ConvertTimeFromUtc(followup.CompleteDate.Value, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) : "") + ")");
                                    --pastFollowupIndex;
                                }
                            }
                        }
                        if (!HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue || (HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue && hra.Id != HttpContext.Session.GetInt32(SessionContext.HRAId).Value))
                        {
                            model.pastAssessments.Add(hra.Id.ToString() + "|hra", "(Started: " + TimeZoneInfo.ConvertTimeFromUtc(hra.StartDate, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) + (hra.CompleteDate.HasValue ? "; Completed: " + TimeZoneInfo.ConvertTimeFromUtc(hra.CompleteDate.Value, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) : "") + ")");
                        }
                    }
                }


            }
            if (HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue)
                model.followUpId = HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value;
            model.hraId = HttpContext.Session.GetInt32(SessionContext.HRAId);
            return PartialView("_Assessment", model);
        }

        #region Eligibility

        [ModuleControl(Modules.Eligibility, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult Eligibility()
        {
            EligibilitySearchModel model = new EligibilitySearchModel();
            model.OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            var trackingModes = CommonUtility.GetEnrollmentStatusList().ToList();
            trackingModes.Insert(0, new SelectListItem() { Text = "--Select Tracking Mode--", Value = "" });
            model.TrackingModes = trackingModes;
            var diagnosisCodes = CommonUtility.GetDiagnosisCodeList().ToList();
            diagnosisCodes.Insert(0, new SelectListItem() { Text = "--Select Diagnosis Code--", Value = "" });
            model.DiagnosisCodes = diagnosisCodes;
            ViewData["languageList"] = CommonUtility.GetLanguages().Languages.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.LanguageCode });
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return View(model);
        }

        [ModuleControl(Modules.Eligibility, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListEligibility(int? orgId, string uniqueId, string firstName, string lastName, string email, string telephone, string language, DateTime? eligibilityStartDate, DateTime? eligibilityEndDate, byte? enrollmentStatus, DateTime? claimStartDate, DateTime? claimEndDate, string claimDiagnosisMode, bool? canrisk, bool? coachingEnabled, int page, int pageSize, int totalRecords)
        {
            int? portalId = null;
            if (orgId.HasValue)
            {
                portalId = new AccountManager().CurrentPortalId(orgId.Value).PortalId;
                if (!portalId.HasValue)
                    return Json(new { Success = false, ErrorMessage = "No active portal for this organization." });
            }
            var eligibilityList = ParticipantUtility.ListEligibility(portalId, uniqueId, firstName, lastName, email, telephone, language, eligibilityStartDate, eligibilityEndDate, claimStartDate, claimEndDate, enrollmentStatus, claimDiagnosisMode, canrisk, coachingEnabled, page, pageSize, totalRecords, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            return Json(new
            {
                Success = true,
                TotalRecords = eligibilityList.totalRecords,
                Records = eligibilityList.Eligibilities.Select(x => new
                {
                    Id = x.Id,
                    PortalId = x.PortalId,
                    PortalName = x.PortalName,
                    UniqueId = x.UniqueId,
                    Name = x.FirstName + " " + x.LastName,
                    DOB = x.DOB == DateTime.MinValue ? null : x.DOB.ToShortDateString(),
                    ReferredOn = x.ReferredOn == DateTime.MinValue ? null : x.ReferredOn.ToShortDateString(),
                    LastCalledDate = x.LastUserNotesDate > x.LastEligNotesDate ? (x.LastUserNotesDate == DateTime.MinValue ? null : x.LastUserNotesDate.ToShortDateString()) : (x.LastEligNotesDate == DateTime.MinValue ? null : x.LastEligNotesDate.ToShortDateString())
                })
            });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListEligibilityByUserName(string name)
        {
            var response = ParticipantUtility.ListEligibilityByUserName(name);
            if (response.eligibilityList != null)
            {
                UserNamewithId users = new UserNamewithId();
                users.userList = response.eligibilityList;
                return Json(users.userList);
            }
            return Json(response);
        }

        [HttpGet]
        [ModuleControl(Modules.Eligibility, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public PartialViewResult GetEligibilityDetails(string eligibilityId, bool? fromElgPage)
        {
            var model = new EligibilityUserModel();
            if (fromElgPage.HasValue && fromElgPage.Value)
            {
                ClearParticipantSession();
                model.fromElgPage = fromElgPage.Value;
            }
            var eligibilityResponse = ParticipantUtility.GetEligibility(Convert.ToInt32(eligibilityId), null, null);
            var eligibility = eligibilityResponse.Eligibility;
            if (eligibility != null)
            {
                CanriskModel canriskmodel = new CanriskModel();
                canriskmodel.GenderList = ListOptions.GetGenderList(null).Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
                canriskmodel.RaceMaleList = ListOptions.GetCanriskMaleRaceList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
                canriskmodel.RaceFemaleList = ListOptions.GetCanriskFemaleRaceList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
                canriskmodel.years = CommonUtility.GetYears(false).Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                canriskmodel.months = CommonUtility.GetMonths().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                canriskmodel.days = CommonUtility.GetDays().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                model.Address = eligibility.Address;
                model.Address2 = eligibility.Address2;
                model.canriskModel = canriskmodel;
                model.BusinessUnit = eligibility.BusinessUnit;
                model.City = eligibility.City;
                model.Country = eligibility.Country;
                model.DeathDate = eligibility.DeathDate;
                model.DOB = eligibility.DOB;
                model.EligibilityStatus = eligibility.UserStatus != null ? eligibility.UserStatus.Description : "";
                model.Email = eligibility.Email;
                model.EmployeeUniqueId = eligibility.EmployeeUniqueId;
                if (!string.IsNullOrEmpty(eligibility.EmployeeUniqueId))
                {
                    if (eligibility.UniqueId != eligibility.EmployeeUniqueId)//dependent record
                    {
                        var primaryUserElgiibilityResponse = ParticipantUtility.GetEligibility(null, eligibility.EmployeeUniqueId, eligibility.PortalId);
                        if (primaryUserElgiibilityResponse != null && primaryUserElgiibilityResponse.Eligibility != null)
                        {
                            var primaryUserEligibility = primaryUserElgiibilityResponse.Eligibility;
                            model.PrimaryUserId = primaryUserEligibility.Id;
                            model.PrimaryUserDOB = primaryUserEligibility.DOB;
                            model.PrimaryUserFirstName = primaryUserEligibility.FirstName;
                            model.PrimaryUserLastName = primaryUserEligibility.LastName;
                            model.PrimaryUserEmail = primaryUserEligibility.Email;
                            if (primaryUserEligibility.Gender != null)
                                model.PrimaryUserGender = primaryUserEligibility.Gender.Description;
                        }
                    }
                    else
                    {
                        var spouse = ParticipantUtility.GetSpouseEligibility(eligibility.UniqueId, eligibility.PortalId);
                        if (spouse.Eligibility == null)
                        {
                            model.CanAddSpouseEligibility = true;
                        }
                        else
                        {
                            model.SpouseId = spouse.Eligibility.Id.Value;
                            model.SpouseFirstName = spouse.Eligibility.FirstName;
                            model.SpouseLastName = spouse.Eligibility.LastName;
                            model.SpouseDOB = spouse.Eligibility.DOB;
                            if (spouse.Eligibility.Gender != null)
                                model.SpouseGender = spouse.Eligibility.Gender.Description;
                        }
                    }
                }
                model.FirstName = eligibility.FirstName;
                if (eligibility.Gender != null)
                    model.Gender = eligibility.Gender.Description;
                model.HireDate = eligibility.HireDate;
                model.HomeNumber = eligibility.HomeNumber;
                model.CellNumber = eligibility.CellNumber;
                model.Id = eligibility.Id.Value;
                model.LastName = eligibility.LastName;
                model.MiddleName = eligibility.MiddleName;
                model.MedicalPlanCode = eligibility.MedicalPlanCode;
                model.MedicalPlanEndDate = eligibility.MedicalPlanEndDate;
                model.medicalCodeComment = ParticipantUtility.MedicalEligibility(model.MedicalPlanCode, model.MedicalPlanEndDate);
                model.Ref_FirstName = eligibility.Ref_FirstName;
                model.Ref_LastName = eligibility.Ref_LastName;
                model.Ref_City = eligibility.Ref_City;
                model.Ref_StateOrProvince = eligibility.Ref_StateOrProvince;
                model.Ref_PractNum = eligibility.Ref_PractNum;
                model.Ref_OfficeName = eligibility.Ref_OfficeName;
                model.Ref_Phone = eligibility.Ref_Phone;
                model.Ref_Fax = eligibility.Ref_Fax;
                model.Lab_Glucose = eligibility.Lab_Glucose;
                model.Lab_A1C = eligibility.Lab_A1C;
                model.Lab_Date = eligibility.Lab_Date;
                if (eligibility.Lab_DidYouFast.HasValue)
                    model.Lab_DidYouFast = eligibility.Lab_DidYouFast == 1 ? "Yes" : "No";
                model.MedicalPlanStartDate = eligibility.MedicalPlanStartDate;
                if (eligibility.PayType != null)
                    model.PayType = eligibility.PayType.Description;
                model.PortalId = Convert.ToString(eligibility.PortalId);
                model.OrganizationId = eligibility.Portal.OrganizationId;
                var crmContact = CRMUtility.GetCRMContactByUniqueId(eligibility.UniqueId, model.OrganizationId);
                if (crmContact != null && fromElgPage.HasValue && fromElgPage.Value)
                    model.CRMId = crmContact.Id;
                model.IntegrationWith = eligibility.Portal.Organization.IntegrationWith;
                model.IsIntuityDTC = eligibility.Portal.Organization.Code == _appSettings.IntuityDTCOrgCode ? true : false;
                model.RegionCode = eligibility.RegionCode;
                model.SSN = eligibility.SSN;
                model.State = eligibility.State;
                model.TerminatedDate = eligibility.TerminatedDate;
                if (eligibility.TobaccoFlag.HasValue)
                    model.TobaccoFlag = eligibility.TobaccoFlag.Value ? "Yes" : "No";
                if (eligibility.UnionFlag.HasValue)
                    model.UnionFlag = eligibility.UnionFlag.Value ? "Yes" : "No";
                if (eligibility.EducationalAssociates.HasValue)
                    model.EducationalAssociates = eligibility.EducationalAssociates.Value ? "Yes" : "No";
                model.PayrollArea = eligibility.PayrollArea;
                model.UniqueId = eligibility.UniqueId;
                model.UserEnrollmentType = eligibility.UserEnrollmentType.Description;
                model.WorkNumber = eligibility.WorkNumber;
                model.Zip = eligibility.Zip;
                if (model.IntegrationWith == (byte)Integrations.Intuity && !String.IsNullOrEmpty(model.Email) && model.IsIntuityDTC)
                {
                    var notificationEvents = NotificationUtility.GetNotificationEventByUniqueId(eligibility.UniqueId, eligibility.PortalId);
                    if (notificationEvents != null && notificationEvents.NotificationEvent.Where(x => x.NotificationStatusId == 2).Count() > 0)
                        model.MissedYouEmail = notificationEvents.NotificationEvent.Where(x => x.NotificationStatusId == 2).Count();
                }
                var userResponse = AccountUtility.GetUserByUniqueId(eligibility.Portal.OrganizationId, eligibility.UniqueId);
                if (userResponse != null && userResponse.User != null)
                {
                    model.PortalUserId = userResponse.User.Id;
                    model.Hracompletion = 0;
                    HttpContext.Session.SetInt32(SessionContext.ParticipantId, userResponse.User.Id);
                    if (userResponse.User.HRAs.Count > 0)
                    {
                        var HRA = userResponse.User.HRAs.Where(x => x.PortalId == eligibility.PortalId).FirstOrDefault();
                        if (HRA != null)
                            model.Hracompletion = HRAUtility.GetHRACompletionPercent(eligibility.Portal.HAPageSeq, HRA.HAPageSeqDone);
                    }
                    if (userResponse.User.UsersinPrograms.Count > 0)
                    {
                        var UserinProgram = userResponse.User.UsersinPrograms.Where(x => x.IsActive == true && x.ProgramsinPortal.PortalId == eligibility.PortalId).FirstOrDefault();
                        if (UserinProgram != null)
                            model.ProgramStatus = UserinProgram.ProgramsinPortal.program.Name + " (" + (UserinProgram.ProgramsinPortal.program.ProgramType == (int)ProgramTypes.SelfHelp ? "SelfHelp" : "Coaching") + ").";
                    }
                }
                model.EnrollmentStatus = eligibility.EnrollmentStatus;
                model.DeclinedEnrollmentReason = eligibility.DeclinedEnrollmentReason;
                model.IsFalseReferral = eligibility.IsFalseReferral;
                model.VisionPlanCode = eligibility.VisionPlanCode;
                model.VisionPlanEndDate = eligibility.VisionPlanEndDate;
                model.VisionPlanStartDate = eligibility.VisionPlanStartDate;
                model.DentalPlanCode = eligibility.DentalPlanCode;
                model.DentalPlanEndDate = eligibility.DentalPlanEndDate;
                model.DentalPlanStartDate = eligibility.DentalPlanStartDate;
                if (eligibility.DiabetesType != null)
                    model.DiabetesType = eligibility.DiabetesType.Description;
                model.CoachingEnabled = eligibility.CoachingEnabled;
                model.CoachingExpirationDate = eligibility.CoachingExpirationDate;
                model.Email2 = eligibility.Email2;
                model.EligibilityFormat = eligibility.Portal.EligibilityFormat ?? 0;
                model.FirstEligibleDate = eligibility.FirstEligibleDate;
                model.Race = eligibility.Race;
                model.Ref_Address1 = eligibility.Ref_Address1;
                model.Ref_Address2 = eligibility.Ref_Address2;
                model.Ref_Zip = eligibility.Ref_Zip;
                model.Ref_Country = eligibility.Ref_Country;
                model.Ref_Phone2 = eligibility.Ref_Phone2;
                model.Ref_Email = eligibility.Ref_Email;
                model.TerminationReason = eligibility.TerminationReason;
                if (!string.IsNullOrEmpty(eligibility.RegionCode))
                    model.Location = ParticipantUtility.GetLocation(eligibility.RegionCode);

            }
            ViewData["moduleTypeList"] = CommonUtility.GetEligibilityModuleList();
            ViewData["notesTypeList"] = CommonUtility.GetEligibilityNoteTypeList();
            ViewData["EnrollmentStatusList"] = CommonUtility.GetEnrollmentStatusList();
            ViewData["DeclinedEnrollmentReasons"] = CommonUtility.GetDeclinedEnrollmentReasons();
            ViewData["languageList"] = CommonUtility.GetLanguages().Languages.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.LanguageCode });
            model.dateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return PartialView("_EligibilityUser", model);
        }

        [HttpGet]
        [ModuleControl(Modules.Eligibility, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public PartialViewResult AddSpouseEligibilityDetails(int id)
        {
            ClearParticipantSession();
            var eligibilityResponse = ParticipantUtility.GetEligibility(id, null, null);
            var eligibility = eligibilityResponse.Eligibility;
            var model = new SpouseEligibilityModel();
            model.Country = eligibility.Country;
            if (string.IsNullOrEmpty(model.Country) || model.Country.Length > 2)
                model.Countries = CommonUtility.ListCountries().Select(x => new SelectListItem { Text = x.Name, Value = x.UNCode });
            else
                model.Countries = CommonUtility.ListCountries().Select(x => new SelectListItem { Text = x.Name, Value = x.Code });
            //Get states for the country
            if (!string.IsNullOrEmpty(model.Country))
            {
                var response = CommonUtility.GetCountry(model.Country);
                if (response != null && response.country != null)
                    model.States = CommonUtility.ListStates(response.country.Id).Select(x => new SelectListItem { Text = x.Name, Value = x.Code }).OrderBy(x => x.Text);
            }
            if (model.States == null || model.States.Count() == 0)
                model.States = Enumerable.Empty<SelectListItem>();
            model.State = eligibility.State;
            if (eligibility != null)
            {
                model.Address = eligibility.Address;
                model.Address2 = eligibility.Address2;
                model.City = eligibility.City;
                model.State = eligibility.State;
                model.Zip = eligibility.Zip;
                model.Country = eligibility.Country;
                model.EmployeeUniqueId = eligibility.UniqueId;
                var companyPersonnelType = PersonnelTypeBase.GetOrganizationPersonnelType(eligibility.Portal.OrganizationId);
                var empId = eligibility.UniqueId.Replace(companyPersonnelType.Code(EligibilityUserEnrollmentTypeDto.Employee), "");
                model.UniqueId = empId + companyPersonnelType.Code(EligibilityUserEnrollmentTypeDto.Spouse);
                model.PrimaryEligibilityId = eligibility.Id.Value;
                model.LastName = eligibility.LastName;
            }
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return PartialView("_AddSpouseEligibility", model);
        }

        [HttpPost]
        [ModuleControl(Modules.Eligibility, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult AddSpouseEligibilityDetails(SpouseEligibilityModel model)
        {
            try
            {
                var eligibilityResponse = ParticipantUtility.GetEligibility(model.PrimaryEligibilityId, null, null);
                var primaryParticipant = eligibilityResponse.Eligibility;
                //create an entry in the eligibility table
                EligibilityDto newParticipant = new EligibilityDto();
                newParticipant.PortalId = primaryParticipant.PortalId;
                newParticipant.Gender = GenderDto.GetByKey(model.Gender);
                newParticipant.DOB = model.DOB;
                newParticipant.Email = model.Email;
                newParticipant.FirstName = model.FirstName;
                newParticipant.LastName = model.LastName;
                newParticipant.UniqueId = model.UniqueId;
                newParticipant.UserEnrollmentType = EligibilityUserEnrollmentTypeDto.Spouse;
                //primary user's information
                newParticipant.EmployeeUniqueId = model.EmployeeUniqueId;
                newParticipant.Address = model.Address;
                newParticipant.Address2 = model.Address2;
                newParticipant.City = model.City;
                newParticipant.Zip = model.Zip;
                newParticipant.State = model.State;
                newParticipant.Country = model.Country;
                newParticipant.CreateDate = DateTime.UtcNow;
                ParticipantUtility.CopyPrimaryEligibilityDetails(primaryParticipant, newParticipant);
                AddEditEligibilityRequest eligibilityRequest = new AddEditEligibilityRequest();
                eligibilityRequest.Eligibility = new EligibilityDto();
                IEligibilityManager eligibilityManager = new EligibilityManager();
                var newParticipantResponse = eligibilityManager.AddEditEligibilityRecord(new AddEditEligibilityRequest() { Eligibility = newParticipant });
                eligibilityManager.AddEligibilityImportLog("N", null, primaryParticipant.PortalId, newParticipant.UniqueId, false, newParticipantResponse.Eligibility.Id, firstName: newParticipant.FirstName, lastName: newParticipant.LastName, createdByUser: "Manual Spouse Eligibility Entry");

                return Json(new { id = newParticipantResponse.Eligibility.Id });
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { id = "" });
            }
        }

        [ModuleControl(Modules.Eligibility, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult UpdateEligiblity(UpdateEligiblityModel model)
        {
            ParticipantUtility.UpdateEligiblity(model);
            return Json(new { Result = "OK" });
        }

        [HttpPost]
        [ModuleControl(Modules.Eligibility, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult AddEditEligibilityNotes([FromBody] EligibilityNotesModel model)
        {
            var result = ParticipantUtility.AddEditEligibilityNotes(model, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            return Json(new { Result = "OK", Record = result });
        }

        [HttpPost]
        [ModuleControl(Modules.Eligibility, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult RemoveEligibilityNote(int id)
        {
            var result = ParticipantUtility.RemoveEligibilityNote(id, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            return Json(new { Result = "OK", Record = result });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult GetEligibilityNotes(string UniqueId, int EligibilityId, int? moduleType, int? noteType)
        {
            var response = ParticipantUtility.GetEligibilityNotes(UniqueId);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            //user elgibility setting
            var eligibilityResponse = ParticipantUtility.GetEligibility(EligibilityId, null, null);
            var primaryParticipant = eligibilityResponse.Eligibility;
            GetUserEligibilitySettingRequest req = new GetUserEligibilitySettingRequest() { UniqueID = UniqueId, OrgID = primaryParticipant.Portal.OrganizationId };
            var userSettingResponse = new EligibilityManager().GetUserEligibilitySetting(req);
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone);
            // int? moduleTy
            return Json(new
            {
                Result = "OK",
                EnrollmentStatus = response.EnrollmentStatus,
                DeclinedEnrollmentReason = response.DeclinedEnrollmentReason,
                Language = userSettingResponse != null && userSettingResponse.UserEligibilitySetting != null ? userSettingResponse.UserEligibilitySetting.Language : String.Empty,
                Records = response.participantEligibilityNotes.Where(x => (!moduleType.HasValue || moduleType.Value == x.ModuleType) && (!noteType.HasValue || noteType.Value == x.NoteType)).Select(x => new
                {
                    Id = x.Id,
                    Notes = x.Notes,
                    Coach = x.User.FirstName + " " + x.User.LastName,
                    CreatedOn = String.Format("{0:G}", TimeZoneInfo.ConvertTimeFromUtc(x.CreatedOn, custTZone)),
                    NoteTypeId = x.NoteType,
                    NoteType = CommonUtility.GetEligibilityNoteTypeList().Where(y => y.Value == x.NoteType.ToString()).FirstOrDefault().Text,
                    ModuleTypeId = x.ModuleType,
                    ModuleType = CommonUtility.GetEligibilityModuleList().Where(y => y.Value == x.ModuleType.ToString()).FirstOrDefault().Text,
                    Edit = (TimeZoneInfo.ConvertTimeFromUtc(x.CreatedOn, custTZone) > currentTime.AddHours(-24) && (CommonUtility.IsSuperAdmin(User.RoleCode()) || x.CreatedBy == Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value))) ? true : false,
                })
            });
        }
        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult ReadEligibilityNote(int noteId)
        {
            var response = ParticipantUtility.ReadEligibilityNote(noteId).EligibilityNote;
            return Json(new
            {
                NoteId = response.Id,
                ModuleType = response.ModuleType,
                NoteType = response.NoteType,
                Notes = response.Notes
            });
        }

        #endregion

        #region Claims

        static ClaimsConditionCostModel MapCandidateCondition(CandidateConditionsDto src)
        {
            if (src == null)
                return null;
            ClaimsConditionCostModel target = new ClaimsConditionCostModel();
            target.ConditionDate = !src.ConditionDate.HasValue ? string.Empty : src.ConditionDate.Value.ToString("MM/dd/yyyy");
            target.ConditionName = src.ConditionName;
            target.ConditionType = src.ConditionType;
            target.BilledAmount = !src.BilledAmount.HasValue ? string.Empty : Math.Round(src.BilledAmount.Value, 2).ToString();
            return target;
        }

        ClaimsMedicationModel MapCandidateMedication(CandidateMedicationsDto src)
        {
            if (src == null)
                return null;
            ClaimsMedicationModel target = new ClaimsMedicationModel();
            target.MedicationDate = !src.MedicationDate.HasValue ? string.Empty : CommonUtility.dateFormater(src.MedicationDate.Value, false, HttpContext.Session.GetString(SessionContext.DateFormat));
            target.MedicationName = src.MedicationName;
            target.MedicationType = src.MedicationType;
            target.TotalAmountPaid = !src.Total_Amount_Paid_by_All_Source.HasValue ? string.Empty : src.Total_Amount_Paid_by_All_Source.Value.ToString();
            return target;
        }

        ClaimsConditionModel MapCandidateCondition(CandidateReasonForLastChangeDto src)
        {
            if (src == null)
                return null;
            ClaimsConditionModel target = new ClaimsConditionModel();
            target.ConditionDate = !src.ConditionDate.HasValue ? string.Empty : CommonUtility.dateFormater(src.ConditionDate.Value, false, HttpContext.Session.GetString(SessionContext.DateFormat));
            target.ConditionType = src.ConditionType;
            target.ConditionCodes = src.ConditionCode;
            target.RecentConditionDate = !src.RecentConditionDate.HasValue ? string.Empty : CommonUtility.dateFormater(src.RecentConditionDate.Value, false, HttpContext.Session.GetString(SessionContext.DateFormat));
            return target;
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpGet]
        public PartialViewResult GetEligibilityClaims(string uniqueId, string organizationid)
        {
            EligibilityClaimsModel model = new EligibilityClaimsModel();
            IClaimManager claimManager = new ClaimManager();
            var orgid = Convert.ToInt32(organizationid);
            var conditionsResponse = claimManager.GetConditionsList(new GetCandidateConditionListRequest() { UniqueId = uniqueId, Organizationid = orgid });
            model.ClaimsConditionCosts = conditionsResponse.CandidateConditionsCost.Select(MapCandidateCondition);
            model.ClaimsConditions = conditionsResponse.InsuranceSummary.Select(MapCandidateCondition);
            model.ClaimsMedications = claimManager.GetMedicationsList(new GetCandidateMedicationListRequest() { UniqueId = uniqueId, Organizationid = orgid }).CandidateMedications.Select(MapCandidateMedication);

            return PartialView("_EligibilityClaimDetail", model);
        }

        #endregion

        #region Participant Search and profile

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult ParticipantSearch()
        {
            ParticipantSearchModel model = new ParticipantSearchModel();
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            return View("ParticipantSearch", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult ParticipantAdvancedSearch()
        {
            ParticipantAdvancedSearchModel model = new ParticipantAdvancedSearchModel();
            model.OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            var selfhelp = new SelectListItem { Text = "Self-Help", Value = ((int)ProgramTypes.SelfHelp).ToString() };
            var coaching = new SelectListItem { Text = "Coaching", Value = ((int)ProgramTypes.Coaching).ToString() };
            model.ProgramList = new List<SelectListItem>() { selfhelp, coaching };
            model.ContactRequirements = ParticipantUtility.GetAllContactRequirements().Select(x => new SelectListItem { Text = x.AlertType, Value = x.Id.ToString() });
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            var coachResponse = SchedulerUtility.GetCoachList(null, true, HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue ? HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value : null, HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue ? HttpContext.Session.GetInt32(SessionContext.AdminId).Value : null, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.StateId));
            var coaches = coachResponse.users.ToList();
            var list = (from coach in coaches
                        select new
                        {
                            Id = coach.Id,
                            Text = coach.FirstName + " " + coach.LastName,
                            Category = coach.RoleCode
                        }).ToArray();
            model.Coaches = new SelectList(list, "Id", "Text", 0, "Category");
            return View("ParticipantAdvancedSearch", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AdvancedSearch([FromBody] AdvancedSearchRequestModel request)
        {
            AdvancedSearchModel model = new AdvancedSearchModel();
            AdvancedSearchUsersResponse response = ParticipantUtility.AdvancedSearchUsers(request, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            if (response != null && response.Result != null && response.Result.Count > 0)
            {
                foreach (var user in response.Result)
                {
                    if (!string.IsNullOrEmpty(user.MedicalPlanCode))
                        user.MedicalPlanCode = ParticipantUtility.MedicalEligibility(user.MedicalPlanCode, user.MedicalPlanEndDate);
                }
            }
            model.Result = response.Result;
            model.TotalRecords = response.TotalRecords;
            return PartialView("_ParticipantResult", model);
        }

        [Authorize]
        public JsonResult GetDashboadMessages(int page, int pageSize, int? totalRecords, string portalStartDate, bool newMessage)
        {
            HttpContext.Session.SetString(SessionContext.VisitedTab, !newMessage ? "true" : "false");
            string timeZone, timeZoneName = "";
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZoneName)))
                timeZoneName = HttpContext.Session.GetString(SessionContext.ParticipantTimeZoneName);
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)))
                timeZone = HttpContext.Session.GetString(SessionContext.ParticipantTimeZone);
            else
                timeZone = User.TimeZone();
            var response = ParticipantUtility.GetDashboadMessages(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, page, pageSize, totalRecords, portalStartDate, newMessage, timeZone, timeZoneName, HttpContext.Session.GetString(SessionContext.DateFormat), (int)NotificationTypes.Feed, true);
            return Json(new
            {
                Records = response
            });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult SearchUsers(SearchUserModel model)
        {
            var response = ParticipantUtility.SearchUsers(model, HttpContext.Session.GetInt32(SessionContext.UserId).Value, User.TimeZone());
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            return Json(new
            {
                TotalRecords = response.totalRecords,
                Records = response.result.Select(x => new
                {
                    Id = x.Id,
                    Name = x.FirstName + " " + x.LastName,
                    DOB = x.DOB.HasValue ? x.DOB.Value.ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) : "",
                    Email = x.Email,
                    DeclinedEnrollment = x.DeclinedEnrollment,
                    DoNotTrack = x.DoNotTrack,
                    OrgName = x.Organization.IndexOf(" ") > 0 ? x.Organization.Substring(0, x.Organization.IndexOf(" ")) : x.Organization,
                    HAPageSeq = x.HAPageSeq,
                    LastAccessedOn = x.LastAccessedOn.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(x.LastAccessedOn.Value, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) : "None",
                    RiskCode = x.RiskCode,
                    HAPageSeqDone = x.HAPageSeqDone,
                    UOMRisk = x.UOMRisk,
                    IVRisk = x.IVRisk,
                    PortalId = x.PortalId,
                    HRAId = x.HRAId,
                    StartDate = x.StartDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(x.StartDate.Value, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) : "",
                    CompleteDate = x.CompleteDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(x.CompleteDate.Value, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) : "",
                    Date = x.Date != null ? String.Format("{0:G}", TimeZoneInfo.ConvertTimeFromUtc(x.Date.Value, custTZone)) : "None",
                    programType = x.ProgramType,
                    MedicalPlanCode = x.MedicalPlanCode,
                    MedicalPlanEndDate = x.MedicalPlanEndDate
                    //Eligibility = (!string.IsNullOrEmpty(x.UniqueId)) ? ParticipantUtility.GetEligibility(null, x.UniqueId, x.PortalId).Eligibility : null
                })
            });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult GetMedicalEligibilityText(string code, DateTime? endDate)
        {
            var eligibilityText = ParticipantUtility.MedicalEligibility(code, endDate);
            return Json(new { Result = "OK", Option = eligibilityText });

        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult ParticipantProfile(int Id, bool? fromLabReport, bool? fromNoShowReport, bool? fromCoachReport, bool? fromAdminTasks, bool? openAppointment, bool? fromCRMProfile, bool? unapprovedCarePlan)
        {
            //Need to check if the coach has access to this user
            ClearParticipantSession();
            ParticipantInfoModel model = new ParticipantInfoModel();
            HttpContext.Session.SetInt32(SessionContext.ParticipantId, Id);
            model = GetParticipantInfo();
            model.fromLabReport = fromLabReport;
            model.fromNoShowReport = fromNoShowReport;
            model.fromCoachReport = fromCoachReport;
            model.fromAdminTasks = fromAdminTasks;
            model.openAppointment = openAppointment;
            model.fromCRMProfile = fromCRMProfile;
            model.fromUnapprovedCarePlan = unapprovedCarePlan;
            model.TimeTrackingDispositionList = ParticipantUtility.GetTimeTrackingDispositionList().Where(x => x.ShowInUI).Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            model.Gender = HttpContext.Session.GetInt32(SessionContext.Gender);
            model.ParticipantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.AdminId = HttpContext.Session.GetInt32(SessionContext.AdminId);
            model.HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            model.UserinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId);
            model.ProgramType = HttpContext.Session.GetInt32(SessionContext.ProgramType);
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.BaseUrl = _appSettings.EmailUrl;
            if (model.User == null)
            {
                HttpContext.Session.Remove(SessionContext.ParticipantId);
                return RedirectToAction("NotAuthorized", "Account");
            }
            else
            {
                return View("ParticipantProfile", model);
            }
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult ParticipantInfo()
        {
            return PartialView("_ParticipantInfo", GetParticipantInfo());
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult TimeTracker(bool InfoPage)
        {
            TimeTrackerModel model = new TimeTrackerModel();
            var response = ParticipantUtility.ReadParticipantInfo(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value));
            if (response.success && Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal)))
            {
                model.IsParticipantInfoPage = InfoPage;
                model.UserName = response.user.FirstName + " " + response.user.LastName;
                model.ShowTimeTracker = response.portal.ShowTimeTracker.HasValue && response.portal.ShowTimeTracker.Value;
                model.TrackerStartTime = response.user.UserTimeTracker.Where(x => !x.EndTime.HasValue && x.CoachId == Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value)).Select(x => x.StartTime).FirstOrDefault();
                model.TimeTrackingDispositionList = ParticipantUtility.GetTimeTrackingDispositionList().Where(x => x.ShowInUI).Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            }
            model.reminderTime = _appSettings.ReminderTime;
            return PartialView("_TimeTracker", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult TimerReport()
        {
            return PartialView("_TimerReport");
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult GetParticipantTimeTracker(int page, int pageSize)
        {
            List<TimerReports> timerList = new List<TimerReports>();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            var response = ParticipantUtility.GetParticipantTrackTime(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, page, pageSize);
            foreach (var timer in response.timeList)
            {
                timerList.Add(new TimerReports
                {
                    StartTime = TimeZoneInfo.ConvertTimeFromUtc(timer.StartTime, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) + " " + TimeZoneInfo.ConvertTimeFromUtc(timer.StartTime, custTZone).ToLongTimeString(),
                    EndTime = TimeZoneInfo.ConvertTimeFromUtc(timer.EndTime.Value, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) + " " + TimeZoneInfo.ConvertTimeFromUtc(timer.EndTime.Value, custTZone).ToLongTimeString(),
                    Disposition = timer.TimeTrackerDisposition.Type,
                    Minutes = (int)(timer.EndTime - timer.StartTime).Value.TotalMinutes,
                    Billed = timer.Billed
                });
            }
            return Json(new { Result = true, Records = timerList, TotalRecords = response.totalRecords });
        }

        public ParticipantInfoModel GetParticipantInfo()
        {
            ParticipantInfoModel model = new ParticipantInfoModel();
            var response = ParticipantUtility.ReadParticipantInfo(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value));
            if (response.success == false)
                return model;
            model.User = response.user;
            model.BaseUrl = _appSettings.EmailUrl;
            model.URL = response.user.Organization.Url;
            model.IsProfileCompleted = response.user.Complete.HasValue && response.user.Complete.Value;
            HttpContext.Session.SetString(SessionContext.HasActivePortal, (response.portal != null).ToString());
            if (response.portal == null)
            {
                var inActivepPortal = PortalUtility.ReadParticipantInactivePortal(Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value.ToString())).portal;
                model.challengesLink = inActivepPortal.PortalIncentives.Any(x => x.IsActive);
                model.sendReportsLink = inActivepPortal.FaxReports;
                model.ShowTimeTracker = inActivepPortal.ShowTimeTracker.HasValue && inActivepPortal.ShowTimeTracker.Value;
                HttpContext.Session.SetInt32(SessionContext.InActiveParticipantPortalId, inActivepPortal.Id);
                HttpContext.Session.SetString(SessionContext.HRAPageSeq, inActivepPortal.HAPageSeq);
                if (inActivepPortal.HRAValidity.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.HRAValidity, inActivepPortal.HRAValidity.Value);
                HttpContext.Session.SetString(SessionContext.MailScoreCard, inActivepPortal.MailScoreCard.ToString());
                if (inActivepPortal.HRAVer.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.HRAVer, inActivepPortal.HRAVer.Value);
                if (inActivepPortal.HasHRA.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.HasHRA, (int)inActivepPortal.HasHRA.Value);
                HttpContext.Session.SetString(SessionContext.CoachingProgram, (inActivepPortal.HasCoachingProgram && HasCoachingConditions()).ToString());
                HttpContext.Session.SetString(SessionContext.SelfHelpProgram, inActivepPortal.HasSelfHelpProgram.ToString());
                HttpContext.Session.SetString(SessionContext.AllowCardiacQuestion, inActivepPortal.AllowCardiacQuestion.ToString());
                if (inActivepPortal.FollowUpValidity.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.FollowUpValidity, inActivepPortal.FollowUpValidity.Value);
                HttpContext.Session.SetString(SessionContext.ShowPostmenopausal, inActivepPortal.ShowPostmenopausal.ToString());
                HttpContext.Session.SetString(SessionContext.CarePlan, inActivepPortal.CarePlan.ToString());
            }
            if (response.portal != null)
                model.ShowTimeTracker = response.portal.ShowTimeTracker.HasValue && response.portal.ShowTimeTracker.Value;
            model.participant = response.participant;
            if (response.participant.OrgId > 0)
                HttpContext.Session.SetInt32(SessionContext.OrganizationId, response.participant.OrgId.Value);
            HttpContext.Session.SetString(SessionContext.OrgContactNumber, response.participant.ContactNum);
            HttpContext.Session.SetString(SessionContext.OrgContactEmail, response.participant.ContactEmail);
            HttpContext.Session.SetString(SessionContext.OrganizationName, response.participant.OrgName);
            if (!string.IsNullOrEmpty(response.participant.OrgCode))
                HttpContext.Session.SetString(SessionContext.OrganizationCode, response.participant.OrgCode);
            HttpContext.Session.SetString(SessionContext.ParticipantEmail, response.participant.Email);
            if (!string.IsNullOrEmpty(response.participant.UniqueId))
                HttpContext.Session.SetString(SessionContext.UniqueId, response.participant.UniqueId);
            HttpContext.Session.SetString(SessionContext.ParticipantName, response.user.FirstName + " " + response.user.LastName);
            if (!string.IsNullOrEmpty(response.participant.LanguagePreference)) 
                HttpContext.Session.SetString(SessionContext.ParticipantLanguagePreference, response.participant.LanguagePreference);
            HttpContext.Session.SetInt32(SessionContext.Unit, response.participant.Unit.HasValue ? (int)response.participant.Unit.Value : (int)Unit.Imperial);
            if (!string.IsNullOrEmpty(response.participant.UserStatus))
                HttpContext.Session.SetString(SessionContext.UserStatus, response.participant.UserStatus);
            if (response.participant.IntegrationWith.HasValue)
                HttpContext.Session.SetInt32(SessionContext.IntegrationWith, response.participant.IntegrationWith.Value);
            HttpContext.Session.SetString(SessionContext.IsMediOrbisUser, ParticipantUtility.IsMediOrbisUser(response.participant.OrgId.Value).ToString());
            if (response.participant.StateId.HasValue)
                HttpContext.Session.SetInt32(SessionContext.StateId, response.participant.StateId.Value);
            if (!string.IsNullOrEmpty(response.participant.MedicalPlanCode))
            {
                model.MedicalEligibility = ParticipantUtility.MedicalEligibility(response.participant.MedicalPlanCode, response.participant.MedicalPlanEndDate);
            }
            model.BusinessUnit = response.participant.BusinessUnit;
            model.TobaccoFlag = response.participant.TobaccoFlag;
            if (response.participant.DOB.HasValue)
            {
                model.age = CommonUtility.GetAge(response.participant.DOB.Value);
                HttpContext.Session.SetString(SessionContext.DOB, response.participant.DOB.Value.ToString());
            }
            if (response.participant.Gender.HasValue)
                HttpContext.Session.SetInt32(SessionContext.Gender, response.participant.Gender.Value);
            model.hraLink = true;
            if (response.hra != null)
            {
                HttpContext.Session.SetInt32(SessionContext.HRAId, response.hra.Id);
                HttpContext.Session.SetString(SessionContext.HRACompleteDate, response.hra.CompleteDate.ToString());
            }
            else if (response.portal != null && response.portal.HasHRA == (int)HRAStatus.No)
            {
                var pastHRAs = HRAUtility.GetAllHRAs(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).HRAs.OrderByDescending(x => x.PortalId).ToList();
                if (pastHRAs.Count() <= 0)
                    model.hraLink = false;
            }
            else if (response.portal == null && response.hra == null)
            {
                var pastHRAs = HRAUtility.GetAllHRAs(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).HRAs.OrderByDescending(x => x.Portal.EndDate).FirstOrDefault();
                if (pastHRAs != null)
                {
                    HttpContext.Session.SetInt32(SessionContext.HRAId, pastHRAs.Id);
                    HttpContext.Session.SetString(SessionContext.HRACompleteDate, pastHRAs.CompleteDate.ToString());
                }
            }
            if (response.portal != null)
            {
                HttpContext.Session.SetInt32(SessionContext.ParticipantPortalId, response.portal.Id);
                HttpContext.Session.SetString(SessionContext.HRAPageSeq, response.portal.HAPageSeq.ToString());
                if (response.portal.HRAValidity.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.HRAValidity, response.portal.HRAValidity.Value);
                HttpContext.Session.SetString(SessionContext.MailScoreCard, response.portal.MailScoreCard.ToString());
                if (response.portal.HRAVer.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.HRAVer, response.portal.HRAVer.Value);
                if (response.portal.HasHRA.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.HasHRA, (int)response.portal.HasHRA.Value);
                HttpContext.Session.SetString(SessionContext.CoachingProgram, (response.portal.HasCoachingProgram && HasCoachingConditions()).ToString());
                HttpContext.Session.SetString(SessionContext.SelfHelpProgram, response.portal.HasSelfHelpProgram.ToString());
                HttpContext.Session.SetString(SessionContext.AllowCardiacQuestion, response.portal.AllowCardiacQuestion.ToString());
                if (response.portal.FollowUpValidity.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.FollowUpValidity, response.portal.FollowUpValidity.Value);
                HttpContext.Session.SetString(SessionContext.ShowPostmenopausal, response.portal.ShowPostmenopausal.ToString());
                HttpContext.Session.SetString(SessionContext.CarePlan, response.portal.CarePlan.ToString());
                HttpContext.Session.SetString(SessionContext.KitAlert, response.portal.KitAlert.ToString());
            }
            if (response.participant.UPId != null)
            {
                if (response.UsersinProgram.Where(x => x.IsActive).Count() > 0 && !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.IsMediOrbisUser)) && Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.IsMediOrbisUser)))
                    HttpContext.Session.SetString(SessionContext.ProgramCode, response.UsersinProgram.Where(x => x.IsActive).FirstOrDefault().ProgramsinPortal.program.Code.ToString());
                HttpContext.Session.SetInt32(SessionContext.UserinProgramId, response.participant.UPId.Value);
                HttpContext.Session.SetInt32(SessionContext.ProgramsInPortalId, response.participant.PPId.Value);
                HttpContext.Session.SetInt32(SessionContext.ProgramType, response.participant.ProgramType.Value);
                HttpContext.Session.SetInt32(SessionContext.AssignedFollowUp, (response.participant.FollowUp.HasValue ? (int)response.participant.FollowUp.Value : 0));
                if (response.participant.ProgramActiveStatus.HasValue && response.participant.ProgramActiveStatus.Value)
                    model.ProgramName = response.participant.ProgramName;
                if (response.participant.CoachFirst != null && response.participant.CoachLast != null)
                    model.CoachName = response.participant.CoachFirst + " " + response.participant.CoachLast;

                if (response.UsersinProgram.FirstOrDefault() != null && response.UsersinProgram.FirstOrDefault().IsActive && response.UsersinProgram.FirstOrDefault().AssignedFollowUp != 0 && response.UsersinProgram.FirstOrDefault().AssignedFollowUp > response.UsersinProgram.FirstOrDefault().FollowUps.Count() && response.UsersinProgram.FirstOrDefault().IsActive)
                    model.followUpDue = true;
                if (response.UsersinProgram.FirstOrDefault()?.FollowUps != null && response.UsersinProgram.FirstOrDefault().FollowUps.Count() > 0)
                {
                    if (response.UsersinProgram.FirstOrDefault().FollowUps.Count() >= response.UsersinProgram.FirstOrDefault().AssignedFollowUp)
                        HttpContext.Session.SetInt32(SessionContext.FollowUpId, response.UsersinProgram.FirstOrDefault().FollowUps.OrderByDescending(x => x.Id).FirstOrDefault().Id);
                    if (response.UsersinProgram.FirstOrDefault().AssignedFollowUp == response.UsersinProgram.FirstOrDefault().FollowUps.Count())
                        HttpContext.Session.SetString(SessionContext.FollowUpCompleteDate, response.UsersinProgram.FirstOrDefault().FollowUps.OrderByDescending(x => x.Id).FirstOrDefault().CompleteDate.ToString());
                    if (!HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue && HttpContext.Session.GetString(SessionContext.FollowUpCompleteDate) != null && response.UsersinProgram.FirstOrDefault().IsActive)
                        model.followUpDue = true;
                }
                if (HttpContext.Session.GetInt32(SessionContext.OrganizationId) == _appSettings.RetailOrgId && response.UsersinProgram.Where(x => x.IsActive).Count() > 0 && ((DateTime.UtcNow - response.UsersinProgram.Where(x => x.IsActive).LastOrDefault().EnrolledOn).Days / 365) + 1 > response.UsersinProgram.Sum(x => x.YearsPaid))
                {
                    model.dueForRenewal = true;
                }
            }
            model.CompIntroKitsOnTime = response.participant.CompIntroKitsOnTime;
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            var appointments = SchedulerUtility.GetAppointments(null, null, null, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone()).StandardName, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null).Appointments;
            if (appointments != null)
            {
                var latestAppt = appointments.Where(x => x.Active == true && (TimeZoneInfo.ConvertTimeFromUtc(x.UTCDate, custTZone).Date == TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone).Date || (TimeZoneInfo.ConvertTimeFromUtc(x.UTCDate, custTZone).Date == TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone).AddDays(-1).Date))).OrderByDescending(x => x.Id).FirstOrDefault();
                if (latestAppt != null)
                {
                    model.AppId = latestAppt.Id;
                }
                else
                {
                    appointments.ForEach(x => x.Date = CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(x.Date.ToString()), custTZone), false, HttpContext.Session.GetString(SessionContext.DateFormat)));
                    model.Appointments = appointments.Where(x => x.Active == true).ToList();
                }
            }
            if (model.User.UserLogs.Count() > 0 && (model.User.UserLogs.Where(x => x.IsSuccess == null || x.IsSuccess.HasValue && x.IsSuccess.Value).Count() > 0))
                model.LastAccessedOn = TimeZoneInfo.ConvertTimeFromUtc(model.User.UserLogs.Where(x => x.IsSuccess == null || x.IsSuccess.HasValue && x.IsSuccess.Value).OrderByDescending(x => x.Id).FirstOrDefault().LastAccessedOn, custTZone).ToString();

            model.claimsReason = response.ConditionType;
            model.claimsDate = response.ConditionDate.HasValue ? response.ConditionDate.Value.ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) : null;
            var Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
            model.GlucoseMeasurement = Measurements[BioLookup.Glucose].MeasurementUnit;
            if (response.lab != null)
            {
                if (response.participant.IntegrationWith == (byte)Integrations.LMC)
                {
                    model.CanriskLabType = " (IE)";
                    if (response.labNo > 1)
                    {
                        model.CanriskLabType = " (FE)";
                    }
                    model.CanriskA1c = response.lab.A1C;
                    if (response.lab.Glucose.HasValue)
                        model.CanriskGlucose = Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric ? (float)Math.Round(CommonUtility.ToMetric(response.lab.Glucose.Value, BioLookup.Glucose, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) : response.lab.Glucose;
                }
                if (!string.IsNullOrEmpty(response.lab.CoachAlert) || !string.IsNullOrEmpty(response.lab.CriticalAlert))
                {
                    model.labAlert = true;
                }
            }
            if (response.participant.ProgramType != null && response.participant.ProgramType == 2)
            {
                HttpContext.Session.SetString(SessionContext.EnrolledinCoaching, true.ToString());
            }

            if (response.portal != null && response.portal.PortalIncentives.Any(x => x.IsActive))
            {
                model.challengesLink = true;
            }
            if (response.portal != null && response.portal.FaxReports)
            {
                model.sendReportsLink = true;
            }
            if (!string.IsNullOrEmpty(response.participant.TimeZoneId))
            {
                HttpContext.Session.SetString(SessionContext.ParticipantTimeZone, response.participant.TimeZoneId);
                HttpContext.Session.SetString(SessionContext.ParticipantTimeZoneName, Translate.Message(response.participant.TimeZone));
            }
            if (response.participant.Weight != null)
            {
                model.Weight = (float)Math.Round(CommonUtility.ToMetric(response.participant.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1);
                model.WeightText = string.Format(Translate.Message("L318"), Translate.Message(Measurements[BioLookup.Weight].MeasurementUnit));
                model.WaistText = string.Format(Translate.Message("L320"), Translate.Message(Measurements[BioLookup.Waist].MeasurementUnit));
                model.WellnessDate = Convert.ToDateTime(response.participant.CollectedOn).ToString(HttpContext.Session.GetString(SessionContext.DateFormat));
            }
            if (response.participant.WellnessVision != null)
                model.participant.WellnessVision = response.participant.WellnessVision;
            var AWVReports = ParticipantUtility.ListAWVReports(model.User.Id);
            if (AWVReports.AWV != null && AWVReports.AWV.Count > 0)
                model.hasEMR = true;
            if (response.UserForms != null)
            {
                model.patientReleaseForm = response.UserForms.Where(x => x.FormTypeId == (int)FormType.PatientRelease).FirstOrDefault();
                model.medicalClearanceForm = response.UserForms.Where(x => x.FormTypeId == (int)FormType.MedicalClearance).FirstOrDefault();
            }

            model.hasExternalReport = ParticipantUtility.ListExternalReports(model.User.Id).reportLists.Count > 0;
            if (response.hra == null && HttpContext.Session.GetInt32(SessionContext.InActiveParticipantPortalId).HasValue)
                response.hra = HRAUtility.ReadHRAByPortal(model.User.Id, HttpContext.Session.GetInt32(SessionContext.InActiveParticipantPortalId).Value).hra;
            if (response.hra != null)
            {
                if (response.hra.CompleteDate.HasValue)
                {
                    model.hraId = response.hra.Id;
                    var riskLevel = response.hra.RiskCode.Substring(0, 1);
                    if (riskLevel == "L")
                        model.hraRisk = "Low";
                    else if (riskLevel == "M")
                        model.hraRisk = "Medium";
                    else if (riskLevel == "H")
                        model.hraRisk = "High";
                    RisksModel risks = new RisksModel();
                    if (response.hra.Goals.ASCVD == true)
                        model.ASCVDRisk = 1;
                    if (response.hra.Goals.Diabetes == true)
                    {
                        model.DiabetesRisk = 1;
                        model.DiabetesRiskText = "Diabetes";
                        if (response.hra.MedicalCondition.Insulin.HasValue && response.hra.MedicalCondition.Insulin == 1)
                            model.DiabetesRiskText = model.DiabetesRiskText + " (Med - I)";
                        else if (response.hra.MedicalCondition.DiabetesMed.HasValue && response.hra.MedicalCondition.DiabetesMed == 1)
                            model.DiabetesRiskText = model.DiabetesRiskText + " (Med)";
                    }
                    //Health Measurement Risks
                    risks.hdsRisk = ReportUtility.GetHDSRisk(response.user, response.hra, null);
                    risks.bpRisk = ReportUtility.GetBPRisk(response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer), null, true, null);
                    risks.ctRisk = ReportUtility.GetCTRisk(response.user, response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer), null, Measurements, null, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                    risks.diabetesRisk = ReportUtility.GetDiabetesRisk(response.user, response.hra, true, null, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                    risks.overweightRisk = ReportUtility.GetOverweightRisk(response.hra, null, Measurements, null, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                    //Lifestyle Risks
                    risks.paRisk = ReportUtility.GetPARisk(response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer), null);
                    risks.nutRisk = ReportUtility.GetNutritionRisk(response.hra);
                    risks.stressRisk = ReportUtility.GetStressRisk(response.hra);
                    risks.tobaccoRisk = ReportUtility.GetTobaccoRisk(response.hra, null, null);
                    //Other Modifiable Risks
                    risks.metRisk = ReportUtility.GetMetRisk(response.hra);
                    risks.cancerRisk = ReportUtility.GetCancerRisk(response.hra);
                    risks.safetyRisk = ReportUtility.GetSafetyRisk(response.hra);
                    model.risks = risks;
                }
                if (response.hra.HealthNumbers != null && response.participant.Weight != null)
                    model.BMI = CommonUtility.GetBMI(response.hra.HealthNumbers.Height.Value, response.participant.Weight.Value);
            }
            if (response.user.Gender == 2)
            {
                model.PregnancyStatus = CommonUtility.GetPregancyStatus(model.hraId, model.User.Id, DateTime.Now).Trimester;
            }
            if (response.participant.IntegrationWith == (byte)Integrations.LMC)
            {
                if (response.participant.CanriskScore != null)
                {
                    model.CanriskScore = response.participant.CanriskScore;
                    model.CanriskCompletedOn = TimeZoneInfo.ConvertTimeFromUtc(response.participant.CanriskCompletedOn.Value, custTZone).Date;
                    model.CanriskText = response.participant.CanriskScore > 32 ? "(High)" : (response.participant.CanriskScore > 20 ? "(Moderate)" : "(Low)");
                }
            }
            if (response.portal != null && response.portal.Organization.Code == _appSettings.IntuityDTCOrgCode)
            {
                if (!response.participant.UniqueId.EndsWith("COU") && (int.TryParse(response.participant.UniqueId, out _) || long.TryParse(response.participant.UniqueId, out _)))
                {
                    if (response.participant.CoachingEnabled.HasValue && response.participant.CoachingExpirationDate.HasValue && response.participant.CoachingEnabled == true)
                    {
                        if (response.participant.CoachingExpirationDate.Value.Date > DateTime.UtcNow.Date)
                            model.SubscriptionPlan = "Subscription: YES <br/>Coaching will expire on: " + response.participant.CoachingExpirationDate.Value.ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) + "";
                        else
                            model.SubscriptionPlan = "Subscription: NO <br/>Coaching ended on: " + response.participant.CoachingExpirationDate.Value.ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) + "";
                    }
                    else
                        model.SubscriptionPlan = "Subscription: NO";
                }
            }
            if (HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value != HttpContext.Session.GetInt32(SessionContext.AdminId).Value)
                model.MessageCount = MessageUtility.GetMessageCountForDashboard(Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value), false, model.User.Id, true, _appSettings.SystemAdminId).MessageBoardCount;
            model.ParticipantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            model.UserinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId);
            model.ProgramType = HttpContext.Session.GetInt32(SessionContext.ProgramType);
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.enrolledinCoaching = HttpContext.Session.GetString(SessionContext.EnrolledinCoaching) != null ? HttpContext.Session.GetString(SessionContext.EnrolledinCoaching) : "";
            model.participantPortalId = GetParticipantPortalId();
            model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            return model;
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult UpdateUserTrackingStatus(bool? DoNotTrack, bool? DeclinedEnrollment, byte? DeclinedEnrollmentReason)
        {
            var UserId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            var PortalId = Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value);
            ParticipantUtility.UpdateUserTrackingStatus(UserId, PortalId, DoNotTrack, DeclinedEnrollment, DeclinedEnrollmentReason);
            return Json(new { Result = "OK" });
        }

        [HttpPost]
        public JsonResult ReadUserTrackingStatus(int userid)
        {
            var response = ParticipantUtility.ReadUserTrackingStatus(userid, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value);
            return Json(new
            {
                Result = "OK",
                Records = response.UserTrackingStatus
            });
        }
        #endregion

        #region Notes for participant

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult Notes()
        {
            TempData["notesPage"] = "True";
            NotesModel model = new NotesModel();
            bool isMcAllenPortal = HttpContext.Session.GetInt32(SessionContext.OrganizationId).ToString() == Constants.McAllenOrgId ? true : false;
            model.NoteTypes = CommonUtility.GetNotesTypeList(false, false, false, isMcAllenPortal).Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() });
            //Get user tracking status
            if (HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue)
            {
                var response = ParticipantUtility.ReadUserTrackingStatus(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value);
                model.UserTrackingStatus = response.UserTrackingStatus;
            }

            ViewData["DeclinedEnrollmentReasons"] = CommonUtility.GetDeclinedEnrollmentReasons();
            if (model.UserTrackingStatus != null && model.UserTrackingStatus.DeclinedEnrollmentReason.HasValue)
                ViewData["DeclinedEnrollmentReason"] = CommonUtility.GetDeclinedEnrollmentReasons().Where(x => x.Value == model.UserTrackingStatus.DeclinedEnrollmentReason.ToString()).FirstOrDefault().Text;
            model.participantPortalId = GetParticipantPortalId();
            model.userinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId);
            model.programType = HttpContext.Session.GetInt32(SessionContext.ProgramType);
            model.enrolledinCoaching = HttpContext.Session.GetString(SessionContext.EnrolledinCoaching);
            model.hasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            return PartialView("_Notes", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult GetNotes()
        {
            var response = ParticipantUtility.GetNotes(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone);
            return Json(new
            {
                Result = "OK",
                Records = response.participantNotes.Select(x => new
                {
                    Id = x.Id,
                    Text = x.Text,
                    NotesDate = String.Format("{0:G}", TimeZoneInfo.ConvertTimeFromUtc(x.NotesDate, custTZone)),
                    Type = x.Type,
                    Pinned = x.Pinned,
                    Name = x.User1.FirstName + " " + x.User1.LastName,
                    CanEdit = (TimeZoneInfo.ConvertTimeFromUtc(x.NotesDate, custTZone) > currentTime.AddHours(-24)) && (x.Admin == Convert.ToInt32(User.UserId())) ? true : false,
                    PortalId = x.PortalId
                })
            });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult GetNotesTypes()
        {
            var isNoteExist = CheckNoteExists();
            bool isMcAllenPortal = HttpContext.Session.GetInt32(SessionContext.OrganizationId).ToString() == Constants.McAllenOrgId ? true : false;
            var noteTypes = CommonUtility.GetNotesTypeList(isNoteExist.noteCheck, isNoteExist.coachCheck, isNoteExist.bioCheck, isMcAllenPortal).Select(c => new { DisplayText = c.Type, Value = c.Id.ToString() }).OrderBy(s => s.DisplayText);
            return Json(new { Result = "OK", Options = noteTypes });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult GetNoteReferralTypes()
        {
            var noteTypes = CommonUtility.GetReferralTypes().Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() });
            return Json(new { Result = "OK", Options = noteTypes });
        }


        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AddEditNotes(int? Id)
        {
            NotesModel model = new NotesModel();
            CoachModel excludeNote = new CoachModel();
            GetNoteResponse response = null;
            if (Id.HasValue)
            {
                response = ParticipantUtility.GetNote(Id);
                NotesDto note = new NotesDto();
                model.Note = response.note;
                model.DisableFields = true;
            }
            else
            {
                excludeNote = CheckNoteExists();
                NotesDto note = new NotesDto();
                TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
                note.NotesDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone);
                model.Note = note;
                model.DisableFields = false;
                var appointments = SchedulerUtility.GetAppointments(null, null, null, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone()).StandardName, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null).Appointments;
                if (appointments != null)
                {
                    var latestAppt = appointments.Where(x => x.Active == true && (TimeZoneInfo.ConvertTimeFromUtc(x.UTCDate, custTZone).Date == TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone).Date || (TimeZoneInfo.ConvertTimeFromUtc(x.UTCDate, custTZone).Date == TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone).AddDays(-1).Date))).OrderByDescending(x => x.Id).FirstOrDefault();
                    if (latestAppt != null)
                    {
                        model.AppId = latestAppt.Id;
                    }
                    else
                    {
                        appointments.ForEach(x => x.Date = CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(x.UTCDate, custTZone), false, HttpContext.Session.GetString(SessionContext.DateFormat)));
                        model.Appointments = appointments.Where(x => x.Active == true).ToList();
                    }
                }
            }
            bool isMcAllenPortal = HttpContext.Session.GetInt32(SessionContext.OrganizationId).ToString() == Constants.McAllenOrgId ? true : false;
            model.NoteTypes = CommonUtility.GetNotesTypeList(excludeNote.noteCheck, excludeNote.coachCheck, excludeNote.bioCheck, isMcAllenPortal).Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() }).OrderBy(s => s.Text);
            model.ReferralTypes = CommonUtility.GetReferralTypes().Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() });
            model.userinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId);
            model.programType = HttpContext.Session.GetInt32(SessionContext.ProgramType);
            model.dateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return PartialView("_AddNotes", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult AddEditNotes(NotesDto note)
        {
            var response = ParticipantUtility.AddEditNotes(note, HttpContext.Session.GetInt32(SessionContext.AdminId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null, HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, User.TimeZone(), HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetString(SessionContext.OrganizationCode), HttpContext.Session.GetString(SessionContext.UniqueId), HttpContext.Session.GetInt32(SessionContext.ProgramType).HasValue ? HttpContext.Session.GetInt32(SessionContext.ProgramType).Value : null, HttpContext.Session.GetInt32(SessionContext.UserinProgramId).HasValue ? HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value : null, HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue ? HttpContext.Session.GetInt32(SessionContext.HRAId).Value : null, _appSettings.SouthUniversityOrgId, _appSettings.DTCOrgCode);
            if (TempData["notesPage"] != null && TempData["notesPage"].ToString() == "True")
                response.notesPage = true;
            return Json(response);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public CoachModel CheckNoteExists()
        {
            var result = ParticipantUtility.GetNotes(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            CoachModel temp = new CoachModel();
            if (result != null && result.participantNotes.Count > 0 && result.participantNotes.Where(x => x.Pinned).Count() > 0)
                temp.noteCheck = true;
            if (result != null && result.participantNotes.Count > 0 && result.participantNotes.Where(x => x.Type == (int)NoteTypes.Coaching && x.NotesDate.Date == DateTime.UtcNow.Date && x.Admin == Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value)).Count() > 0)
                temp.coachCheck = true;
            if (result != null && result.participantNotes.Count > 0 && result.participantNotes.Where(x => x.Type == (int)NoteTypes.BiometricReview && x.NotesDate.Date == DateTime.UtcNow.Date && x.Admin == Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value)).Count() > 0)
                temp.bioCheck = true;
            return temp;
        }

        #endregion

        #region WellnessData

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult WellnessData()
        {
            WellnessDataViewModel model = new WellnessDataViewModel();
            model.organizationId = HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value;
            model.dateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.teamsBPOrgId = _appSettings.TeamsBPOrgId;
            return PartialView("_WellnessData", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult WellnessChart()
        {
            var Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
            ViewData["Weight"] = string.Format(Translate.Message("L318"), Translate.Message(Measurements[BioLookup.Weight].MeasurementUnit));
            ViewData["Waist"] = "Waist (" + Translate.Message(Measurements[BioLookup.Waist].MeasurementUnit) + ")";
            WellnessChartModel model = new WellnessChartModel();
            model.hasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            model.organizationId = HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value;
            model.teamsBPOrgId = _appSettings.TeamsBPOrgId;
            return PartialView("_WellnessChart", model);
        }

        [Authorize]
        public JsonResult ListWellnessData(ListWellnessDataModel model)
        {
            var response = ParticipantUtility.ListWellnessDataByPage(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            return Json(new
            {
                Result = "OK",
                isDiabetic = HasDiabetes(),
                response.TotalRecords,
                AllRecords = response.WellnessData.Select(x => new
                {
                    Id = x.Id,
                    CollectedOn = TimeZoneInfo.ConvertTimeFromUtc(x.CollectedOn, custTZone).ToShortDateString(),
                    Weight = x.Weight.HasValue ? (float)Math.Round(CommonUtility.ToMetric(x.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) : x.Weight,
                    WaistCircumference = x.waist.HasValue ? (float)Math.Round(CommonUtility.ToMetric(x.waist.Value, BioLookup.Waist, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) : x.waist,
                    SBP = x.SBP,
                    DBP = x.DBP,
                    Pulse = x.Pulse,
                    A1C = x.A1C,
                    TakeVitamins = x.TakeVitamins,
                    TakeMeds = x.TakeMeds,
                    MissMeds = x.MissMeds,
                    ChangeMeds = x.ChangeMeds,
                    Allergyreaction = x.Allergyreaction,
                    ExerMin = x.ExerMin,
                    ExerInt = x.ExerInt,
                    HealthyEating = x.HealthyEating,
                    PhysicallyActive = x.PhysicallyActive,
                    ManageStress = x.ManageStress,
                    Motivation = x.Motivation,
                    CurrentSmoker = x.CurrentSmoker,
                    SmokedPerDay = x.SmokedPerDay,
                    YQuitDate = x.YQuitDate,
                    YContractSign = x.YContractSign,
                    NQuitDate = x.NQuitDate,
                    ManWithdrawSymp = x.ManWithdrawSymp,
                    UsingNRT = x.UsingNRT,
                    Gum = x.Gum,
                    Patch = x.Patch,
                    Lozenge = x.Lozenge,
                    Inhaler = x.Inhaler,
                    Spray = x.Spray,
                    UsingBupropion = x.UsingBupropion,
                    UsingVarenicline = x.UsingVarenicline,
                    UsingOtherDrug = x.UsingOtherDrug,
                    SourceHRA = x.SourceHRA,
                    SourceFollowUp = x.SourceFollowUp,
                    isPregnant = x.isPregnant,
                    DueDate = x.DueDate,
                    x.UpdatedBy,
                    x.UserId
                }),
                Records = response.TableWellnessData.Select(x => new
                {
                    Id = x.Id,
                    CollectedOn = TimeZoneInfo.ConvertTimeFromUtc(x.CollectedOn, custTZone).ToShortDateString(),
                    Weight = x.Weight.HasValue ? (float)Math.Round(CommonUtility.ToMetric(x.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) : x.Weight,
                    WaistCircumference = x.waist.HasValue ? (float)Math.Round(CommonUtility.ToMetric(x.waist.Value, BioLookup.Waist, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) : x.waist,
                    SBP = x.SBP,
                    DBP = x.DBP,
                    A1C = x.A1C,
                    Pulse = x.Pulse,
                    TakeVitamins = x.TakeVitamins,
                    TakeMeds = x.TakeMeds,
                    MissMeds = x.MissMeds,
                    ChangeMeds = x.ChangeMeds,
                    Allergyreaction = x.Allergyreaction,
                    ExerMin = x.ExerMin,
                    ExerInt = x.ExerInt,
                    HealthyEating = x.HealthyEating,
                    PhysicallyActive = x.PhysicallyActive,
                    ManageStress = x.ManageStress,
                    Motivation = x.Motivation,
                    CurrentSmoker = x.CurrentSmoker,
                    SmokedPerDay = x.SmokedPerDay,
                    YQuitDate = x.YQuitDate,
                    YContractSign = x.YContractSign,
                    NQuitDate = x.NQuitDate,
                    ManWithdrawSymp = x.ManWithdrawSymp,
                    UsingNRT = x.UsingNRT,
                    Gum = x.Gum,
                    Patch = x.Patch,
                    Lozenge = x.Lozenge,
                    Inhaler = x.Inhaler,
                    Spray = x.Spray,
                    UsingBupropion = x.UsingBupropion,
                    UsingVarenicline = x.UsingVarenicline,
                    UsingOtherDrug = x.UsingOtherDrug,
                    SourceHRA = x.SourceHRA,
                    SourceFollowUp = x.SourceFollowUp,
                    isPregnant = x.isPregnant,
                    DueDate = x.DueDate,
                    x.UpdatedBy,
                    x.UserId
                })
            });
        }

        public string FindGlucoseCodeName(string code)
        {
            switch (code)
            {
                case "2345-7": return ("BG");
                case "1558-6": return ("Fasting BG");
                case "53049-3": return ("Pre-meal BG");
                case "1521-4": return ("Post-meal BG");
                default: return code;
            }
        }

        /// <summary>
        /// Add and Edit wellnessdata
        /// </summary>
        /// <returns></returns>
        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AddWellnessData(int? id)
        {
            WellnessDataModel model = new WellnessDataModel();
            model.Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
            model.WeightText = Translate.Message(model.Measurements[BioLookup.Weight].MeasurementUnit);
            model.WaistText = Translate.Message(model.Measurements[BioLookup.Waist].MeasurementUnit);
            model.ExerIntList = CommonUtility.GetExerciseIntensity();
            model.ContractStatusList = CommonUtility.GetContractStatus();
            model.Ratings = CommonUtility.GetRatings();
            model.tobaccoUser = ParticipantUtility.CheckIfTobaccoUser(false, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, GetParticipantPortalId()).smoker;
            model.DiabetesTypeList = CommonUtility.GetDiabetesTypeList();
            model.GlucoseCheckCountList = CommonUtility.GetGlucoseCheckCountList();
            model.ClaudicationScale = CommonUtility.GetClaudicationScale();
            int hraId = HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue ? HttpContext.Session.GetInt32(SessionContext.HRAId).Value : 0;
            var pregnencyStatus = CommonUtility.GetPregancyStatus(hraId, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, null);
            model.wellnessData = new WellnessDataDto();
            if (id.HasValue)
            {
                var response = ParticipantUtility.ReadWellnessData(id, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
                model.showDiabetes = response.WellnessData.isDiabetic.HasValue ? true : model.showDiabetes;
                model.wellnessData = response.WellnessData;
                model.wellnessData.Weight = model.wellnessData.Weight.HasValue ? (float)CommonUtility.ToMetric(model.wellnessData.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : model.wellnessData.Weight;
                model.wellnessData.sameDevice = response.WellnessData.sameDevice;
                model.wellnessData.waist = model.wellnessData.waist.HasValue ? (float)CommonUtility.ToMetric(model.wellnessData.waist.Value, BioLookup.Waist, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : model.wellnessData.waist;
                model.wellnessData.GlucoseFrom = model.wellnessData.GlucoseFrom.HasValue ? (float)CommonUtility.ToMetric(model.wellnessData.GlucoseFrom.Value, BioLookup.Glucose, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : model.wellnessData.GlucoseFrom;
                model.wellnessData.GlucoseTo = model.wellnessData.GlucoseTo.HasValue ? (float)CommonUtility.ToMetric(model.wellnessData.GlucoseTo.Value, BioLookup.Glucose, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : model.wellnessData.GlucoseTo;
            }
            else
            {
                model.showDiabetes = !HasDiabetes();
                var response = ParticipantUtility.ListWellnessData(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
                var wellnessData = response.WellnessData.Where(x => !String.IsNullOrEmpty(x.WellnessVision)).OrderByDescending(x => x.CollectedOn).FirstOrDefault();
                if (wellnessData != null)
                {
                    model.wellnessData.WellnessVision = wellnessData.WellnessVision;
                }
                var diabeticData = response.WellnessData.Where(x => x.DidDocReqGlucoseTest.HasValue).OrderByDescending(x => x.CollectedOn).FirstOrDefault();
                if (diabeticData != null)
                {
                    model.wellnessData.DidDocReqGlucoseTest = diabeticData.DidDocReqGlucoseTest;
                    model.wellnessData.NoOfGlucTestRequested = diabeticData.NoOfGlucTestRequested;
                    model.wellnessData.NoOfGlucTestDone = diabeticData.NoOfGlucTestDone;
                }
                var diabeticType = response.WellnessData.Where(x => x.DiabeticType.HasValue).FirstOrDefault();
                if (diabeticType != null)
                    model.wellnessData.DiabeticType = diabeticType.DiabeticType;
            }
            model.Age = CommonUtility.GetAge(Convert.ToDateTime(HttpContext.Session.GetString(SessionContext.DOB)));
            model.wellnessData.isPregnant = pregnencyStatus.isPregnant;
            model.wellnessData.DueDate = pregnencyStatus.pregDueDate;
            model.dateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.allowCardiacQuestion = HttpContext.Session.GetString(SessionContext.AllowCardiacQuestion) == "true";
            model.hasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            model.gender = HttpContext.Session.GetInt32(SessionContext.Gender);

            return PartialView("_AddWellnessData", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AddEditTeamsBP_PPR(int? id)
        {
            var TeamsBP_PPRList = ParticipantUtility.ReadTeamsBP_PPRData(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).TeamsBP_PPR;
            if (!id.HasValue && TeamsBP_PPRList.Count > 0)
            {
                var teamsBP = TeamsBP_PPRList.Where(x => x.WellnessData.CollectedOn.Date == DateTime.UtcNow.Date).FirstOrDefault();
                if (teamsBP != null)
                    id = teamsBP.WellnessData.Id;
            }
            TeamsBP_PPRModel model = new TeamsBP_PPRModel();
            model.Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
            model.WeightText = Translate.Message(model.Measurements[BioLookup.Weight].MeasurementUnit);
            model.AchievePAGoalList = CommonUtility.GetReasonsAbleToAchievePhysicalActivityGoals();
            model.FactorsInMonitoringBPList = CommonUtility.GetFactorsHelpfulInMonitoringBP();
            model.InjuryList = CommonUtility.GetHowSeriousWasInjury();
            model.ExerIntList = CommonUtility.GetExerciseIntensity();
            model.NotTakingMedicationsList = CommonUtility.GetReasonsNotTakingMedications();
            model.Ratings = CommonUtility.GetRatings();
            model.IsAlcoholicUser = true;
            model.IsTobaccoUser = true;
            model.IsReadOnly = false;
            model.UnableToAttendTherapyList = CommonUtility.GetReasonsUnableToAttendRehabilitationTherapy();
            model.UnableToAchievePAGoalList = CommonUtility.GetReasonsUnableToAchievePhysicalActivityGoals();
            model.UnableToFollowHDGoalList = CommonUtility.GetReasonsUnableToFollowHealthyDietGoals();
            model.UnableToMonitorBPList = CommonUtility.GetReasonUnableToMonitorBP();
            model.TeamsBP_PPR = new TeamsBP_PPRDto();
            model.TeamsBP_PPR.WellnessData = new WellnessDataDto();

            if (!id.HasValue && TeamsBP_PPRList.Count > 0)
            {
                model.IsAlcoholicUser = TeamsBP_PPRList.Any(x => x.WellnessData.BillingNotes != null && x.WellnessData.BillingNotes.Any(w => w.Submitted) && x.LimitingAlcohol.HasValue && x.LimitingAlcohol.Value != 0) || TeamsBP_PPRList.All(x => !x.LimitingAlcohol.HasValue);
                if (TeamsBP_PPRList.Any(x => x.WellnessData.BillingNotes != null && x.WellnessData.BillingNotes.Any(w => w.Submitted) && x.LimitingAlcohol.HasValue && x.LimitingAlcohol.Value == 0))
                    model.IsAlcoholicUser = false;

                model.IsTobaccoUser = TeamsBP_PPRList.Any(x => x.WellnessData.BillingNotes != null && x.WellnessData.BillingNotes.Any(w => w.Submitted) && x.QuitSmoking.HasValue && x.QuitSmoking.Value != 0) || TeamsBP_PPRList.All(x => !x.QuitSmoking.HasValue);
                if (TeamsBP_PPRList.Any(x => x.WellnessData.BillingNotes != null && x.WellnessData.BillingNotes.Any(w => w.Submitted) && x.QuitSmoking.HasValue && x.QuitSmoking.Value == 0))
                    model.IsTobaccoUser = false;
            }
            if (id.HasValue)
            {
                model.TeamsBP_PPR = TeamsBP_PPRList.Where(x => x.WellnessData.Id == id.Value).FirstOrDefault();
                model.TeamsBP_PPR.WellnessData.Weight = model.TeamsBP_PPR.WellnessData.Weight.HasValue ? (float)CommonUtility.ToMetric(model.TeamsBP_PPR.WellnessData.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : model.TeamsBP_PPR.WellnessData.Weight;
                model.IsReadOnly = model.TeamsBP_PPR.WellnessData.BillingNotes.Any(x => x.Submitted) || !Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
                model.IsAlcoholicUser = model.TeamsBP_PPR.LimitingAlcohol.HasValue || !TeamsBP_PPRList.Any(x => x.WellnessData.CollectedOn < model.TeamsBP_PPR.WellnessData.CollectedOn) || TeamsBP_PPRList.Any(x => x.WellnessData.CollectedOn < model.TeamsBP_PPR.WellnessData.CollectedOn && x.WellnessData.BillingNotes != null && x.WellnessData.BillingNotes.Any(w => w.Submitted) && x.LimitingAlcohol.HasValue && x.LimitingAlcohol.Value != 0) || TeamsBP_PPRList.Where(x => x.WellnessData.CollectedOn < model.TeamsBP_PPR.WellnessData.CollectedOn && (!x.LimitingAlcohol.HasValue || x.LimitingAlcohol.HasValue && x.LimitingAlcohol.Value == 0)).Count() == TeamsBP_PPRList.Where(x => x.WellnessData.CollectedOn < model.TeamsBP_PPR.WellnessData.CollectedOn).Count();
                if (TeamsBP_PPRList.Any(x => x.WellnessData.CollectedOn < model.TeamsBP_PPR.WellnessData.CollectedOn && x.WellnessData.BillingNotes != null && x.WellnessData.BillingNotes.Any(w => w.Submitted) && x.LimitingAlcohol.HasValue && x.LimitingAlcohol.Value == 0))
                    model.IsAlcoholicUser = false;
                model.IsTobaccoUser = model.TeamsBP_PPR.QuitSmoking.HasValue || !TeamsBP_PPRList.Any(x => x.WellnessData.CollectedOn < model.TeamsBP_PPR.WellnessData.CollectedOn) || TeamsBP_PPRList.Any(x => x.WellnessData.CollectedOn < model.TeamsBP_PPR.WellnessData.CollectedOn && x.WellnessData.BillingNotes != null && x.WellnessData.BillingNotes.Any(w => w.Submitted) && x.QuitSmoking.HasValue && x.QuitSmoking.Value != 0) || TeamsBP_PPRList.Where(x => x.WellnessData.CollectedOn < model.TeamsBP_PPR.WellnessData.CollectedOn && (!x.QuitSmoking.HasValue || x.QuitSmoking.HasValue && x.QuitSmoking.Value == 0)).Count() == TeamsBP_PPRList.Where(x => x.WellnessData.CollectedOn < model.TeamsBP_PPR.WellnessData.CollectedOn).Count();
                if (TeamsBP_PPRList.Any(x => x.WellnessData.CollectedOn < model.TeamsBP_PPR.WellnessData.CollectedOn && x.WellnessData.BillingNotes != null && x.WellnessData.BillingNotes.Any(w => w.Submitted) && x.QuitSmoking.HasValue && x.QuitSmoking.Value == 0))
                    model.IsTobaccoUser = false;
            }
            model.dataFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return PartialView("_AddWellnessData_TeamsBP", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult AddEditWellnessData(WellnessDataModel model)
        {
            model.wellnessData.UserId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.wellnessData.Weight = model.wellnessData.Weight.HasValue && Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric ? (float)CommonUtility.ToImperial(model.wellnessData.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : model.wellnessData.Weight;
            model.wellnessData.waist = model.wellnessData.waist.HasValue && Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric ? (float)CommonUtility.ToImperial(model.wellnessData.waist.Value, BioLookup.Waist, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : model.wellnessData.waist;
            model.wellnessData.GlucoseFrom = model.wellnessData.GlucoseFrom.HasValue && Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric ? (float)CommonUtility.ToImperial(model.wellnessData.GlucoseFrom.Value, BioLookup.Glucose, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : model.wellnessData.GlucoseFrom;
            model.wellnessData.GlucoseTo = model.wellnessData.GlucoseTo.HasValue && Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric ? (float)CommonUtility.ToImperial(model.wellnessData.GlucoseTo.Value, BioLookup.Glucose, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : model.wellnessData.GlucoseTo;
            model.updatedbyUser = false;
            model.userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            ParticipantUtility.AddEditWellnessData(model);
            return Json("success");
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult AddEditTeamsBP_PPR(TeamsBP_PPRModel model)
        {
            model.TeamsBP_PPR.WellnessData.UserId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.TeamsBP_PPR.WellnessData.Weight = model.TeamsBP_PPR.WellnessData.Weight.HasValue && Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric ? (float)CommonUtility.ToImperial(model.TeamsBP_PPR.WellnessData.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : model.TeamsBP_PPR.WellnessData.Weight;
            model.userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            return Json(ParticipantUtility.AddEditTeamsBP_PPR(model));
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult DeleteWellnessDat(int Id)
        {
            if (ParticipantUtility.DeleteWellnessDataRecord(Id))
                return Json(new { Result = "OK" });
            else
                return Json(new { Result = "FAIL" });
        }

        #endregion

        [HttpPost]
        [Authorize]
        public JsonResult AddEditSurvey([FromBody] SurveyResponseModel model)
        {
            var result = ParticipantUtility.AddEditSurvey(model, HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value).success;
            return Json(new { Result = "OK", Record = result });
        }

        [Authorize]
        public ActionResult SatisfactionSurvey()
        {
            ReadSurveyResponse model = new ReadSurveyResponse();
            model.SurveyQuestions = ParticipantUtility.GetSurveyQuestions();
            var survey = ParticipantUtility.GetSurveyCompletedStatus(HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value);
            model.SurveyResponse = survey.surveyResponse;
            model.Comments = survey.Comments;
            return PartialView("_SatisfactionSurvey", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult ChatMessage()
        {
            ViewData["BaseUrl"] = _appSettings.EmailUrl;
            ViewData["SystemAdminId"] = _appSettings.SystemAdminId;
            return PartialView("_ChatMessage");
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AutomatedEmails()
        {
            NotificationMessageModel model = new NotificationMessageModel();
            var response = NotificationUtility.ListNotificationMessage(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            var custzone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            model.NotificationMessage = response.NotificationMessage;
            for (var i = 0; i < model.NotificationMessage.Count; i++)
            {
                model.NotificationMessage[i].Date = CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(model.NotificationMessage[i].Date), custzone), true, HttpContext.Session.GetString(SessionContext.DateFormat));
            }
            return PartialView("_AutomatedEmails", model);
        }

        public JsonResult AddTestimonial(string feedback, string SignedName, string Date)
        {
            var response = ParticipantUtility.AddTestimonial(feedback, SignedName, Date, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value).success;
            return Json(new { Result = "OK" });
        }

        public ActionResult Terms()
        {
            if (HttpContext.Session.GetString(SessionContext.TermsSSO) != null && HttpContext.Session.GetString(SessionContext.TermsSSO) == "True")
            {
                if (HttpContext.Session.GetString(SessionContext.TermsAccepted) == null || HttpContext.Session.GetString(SessionContext.TermsAccepted) == "False")
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Stream", "Participant");
                }
            }
            else
            {
                return RedirectToAction("Stream", "Participant");
            }

        }

        public JsonResult AcceptTerms()
        {
            var response = ParticipantUtility.AcceptTerms(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            return Json(new { Result = response });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult Forms()
        {
            FormModel model = new FormModel();
            model.userForms = ParticipantUtility.GetUserForms(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value).userForms;
            return PartialView("_Forms", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult ApproveForm(int id)
        {
            var response = ParticipantUtility.ApproveForm(id, HttpContext.Session.GetInt32(SessionContext.AdminId).Value).success;
            return Json(new { Result = "OK" });
        }

        [Authorize]
        public JsonResult SendMissedOutreachEmail(string uniqueId, int portalId, string email)
        {
            NotificationUtility.MissedOutreach(NotificationEventTypeDto.MissedOutreach, uniqueId, portalId, email);
            return Json(new { Result = "OK" });
        }

        [Authorize]
        public JsonResult TrackTime(int? timeSpent, int? disposition, int? participantId)
        {
            int UserId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value : participantId.Value;
            int CoachId = HttpContext.Session.GetInt32(SessionContext.AdminId).Value;
            ParticipantUtility.TrackTime(UserId, CoachId, timeSpent, disposition, false);
            return Json(new { Result = "OK" });
        }

        [Authorize]
        public JsonResult AddTrackTime(DateTime startTime, DateTime endTime, int disposition)
        {
            int UserId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            int CoachId = HttpContext.Session.GetInt32(SessionContext.AdminId).Value;
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            var result = ParticipantUtility.TrackTime(UserId, CoachId, null, disposition, false, TimeZoneInfo.ConvertTimeToUtc(startTime, custTZone), TimeZoneInfo.ConvertTimeToUtc(endTime, custTZone));
            return Json(new { Result = result });
        }

        public ActionResult ExternalReports()
        {
            ExternalReportsModel model = new ExternalReportsModel();
            model.reportsList = ParticipantUtility.ListExternalReports(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).reportLists.Select(x => new SelectListItem { Text = x.ReportName, Value = x.Id.ToString() });

            return PartialView("_ExternalReports", model);
        }

        [Authorize]
        public ActionResult PullExternalReport(int reportId)
        {
            var result = ParticipantUtility.PullExternalReport(reportId, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            if (result != null)
            {
                FileResult fileResult = new FileContentResult(result, "application/pdf");
                return fileResult;
            }
            return RedirectToAction("NotAuthorized", "Account");
        }

        [Authorize]
        public ActionResult RPMSummary()
        {
            ViewData["DateFormat"] = HttpContext.Session.GetString(SessionContext.DateFormat);
            return PartialView("_RPMSummary");
        }

        public ActionResult GetRPMSummary(int days, string startDate, string endDate)
        {
            DateTime? startDateFilter = null, endDateFilter = null;
            if (!string.IsNullOrEmpty(startDate) && startDate != "undefined" && !string.IsNullOrEmpty(endDate) && endDate != "undefined")
            {
                startDateFilter = Convert.ToDateTime(startDate);
                endDateFilter = Convert.ToDateTime(endDate).AddDays(1);
                var diffDays = (endDateFilter - startDateFilter).Value.TotalDays;
                if (diffDays > 31)
                    endDateFilter = startDateFilter.Value.AddDays(31);
            }
            else if (days > 0)
            {
                endDateFilter = DateTime.UtcNow;
                startDateFilter = endDateFilter.Value.AddDays(-(days));
            }
            var result = ParticipantUtility.GetRPMSummaryGraph(HttpContext.Session.GetString(SessionContext.UniqueId), startDateFilter.Value, endDateFilter.Value, _appSettings.TeamsBPApiKey, _appSettings.TeamsBPURL);
            return result != null ? new FileContentResult(result, "image/jpeg") : null;
        }
    }
}