using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace InterventWebApp
{
    public class PortalController : BaseController
    {
        private readonly AppSettings _appSettings;

        public PortalController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        #region Organizations and Portals

        [ModuleControl(Modules.Organizations)]
        public ActionResult Organizations()
        {
            OrganizationModel model = new OrganizationModel();
            model.BaseUrl = _appSettings.EmailUrl;
            //old code
            model.Groups = AccountUtility.ListRoles().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            model.Integrations = CommonUtility.GetIntegrationsList();
            model.ParentOrganizations = PortalUtility.GetParentOrganizationsDetails().Organizations.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(y => y.Text).ToList();
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListOrganizationsforSetup([FromBody] OrganizationListModel model)
        {
            var response = PortalUtility.ListOrganizationsForSetup(model);
            var organizations = response.Organizations.ToList();

            List<ListOrganizationModel> OrgLists = new List<ListOrganizationModel>();
            foreach (var org in organizations)
            {
                var portal = org.Portals.Any(x => x.Active == true) ? org.Portals.Where(y => y.Active == true).FirstOrDefault() : org.Portals.OrderByDescending(z => z.Id).FirstOrDefault();
                ListOrganizationModel orgList = new ListOrganizationModel();
                orgList.Id = org.Id.Value;
                orgList.Name = org.Name;
                orgList.Description = org.Description;
                orgList.PhoneNumber = org.ContactNumber;
                orgList.Email = org.ContactEmail;
                orgList.Groups = string.Join(", ", org.UserRoles.Select(x => x.Name));
                orgList.EmailValidationRequired = org.EmailValidationRequired ? "Required" : "Not Required";
                orgList.OwnCoach = org.OwnCoach ? "Yes" : "No";
                orgList.SSO = org.SSO.HasValue && org.SSO.Value ? "Yes" : "No";
                orgList.TermsForSSO = org.TermsForSSO.HasValue && org.TermsForSSO.Value ? "Yes" : "No";
                if (org.Organization1 != null)
                    orgList.ParentOrganizationName = org.Organization1.Name;
                if (portal != null)
                {
                    orgList.RecentPortalId = portal.Id;
                    orgList.RecentPortalName = portal.Name;
                    orgList.Language = string.Join("/", portal.Languages.Select(x => x.LanguageCode));
                    if (portal.HRAVer.HasValue)
                    {
                        orgList.HRAVersion = CommonUtility.GetHRAVersions().ToList().Where(x => x.Value == portal.HRAVer.Value.ToString()).FirstOrDefault().Text;
                    }
                    if (portal.HasCoachingProgram)
                    {
                        orgList.ProgramTypes = "Coaching";
                    }
                    if (portal.HasSelfHelpProgram)
                    {
                        orgList.ProgramTypes = string.IsNullOrEmpty(orgList.ProgramTypes) ? "Self Help" : orgList.ProgramTypes + "/Self Help";
                    }
                    orgList.EndDate = Convert.ToDateTime(portal.EndDate).ToString(HttpContext.Session.GetString(SessionContext.DateFormat));
                    orgList.IsActivePortal = portal.Active;
                    if (portal.Active)
                        orgList.ActiveDetails = "Active for " + Convert.ToDateTime(portal.EndDate).Subtract(DateTime.Now).Days + " days";
                    else
                        orgList.ActiveDetails = "Inactive for " + DateTime.Now.Subtract(Convert.ToDateTime(portal.EndDate)).Days + " days";
                }
                OrgLists.Add(orgList);
            }
            return Json(new { Result = "OK", Records = OrgLists, TotalRecords = response.totalRecords, ActiveOrgCount = response.ActiveOrganizationsCount, CoachingOrgCount = response.CoachingOrganizationsCount, SelfHelpOrgCount = response.SelfHelpOrganizationsCount, InActiveOrgCount = response.InActiveOrganizationsCount });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListOrganizations(OrganizationListModel model)
        {
            var response = PortalUtility.ListOrganizations(model);
            var organizations = response.Organizations.Where(x => x.Portals.Any(y => y.Active == true)).ToList(); ;
            return Json(new
            {
                Result = "OK",
                Records = response.Organizations.Select(x => new
                {
                    Name = x.Name,
                    Description = x.Description,
                    Id = x.Id,
                    ParentOrganization = x.Organization1 != null ? x.Organization1.Name : "",
                    ParentOrganizationId = x.Organization1 != null ? x.Organization1.Id : null,
                    Url = x.Url,
                    EmailValidationRequired = x.EmailValidationRequired
                }),
                OrganizationDropDownData = PortalUtility.ListOrganizations(null).Organizations.Select(x => new
                {
                    Name = x.Name,
                    Id = x.Id,
                    ParentOrganization = x.Organization1 != null ? x.Organization1.Name : ""
                }),
                TotalRecords = response.totalRecords
            });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult RemoveCustomIncentive(int UserIncentiveId)
        {
            return Json(new { Result = PortalUtility.RemoveCustomIncentive(UserIncentiveId, HttpContext.Session.GetInt32(SessionContext.AdminId).Value).success });
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult ListCoupons(int OrganizationId)
        {
            return Json(new { Result = PortalUtility.ListCoupons(OrganizationId, false) });
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult ReadCoupon(int couponid)
        {
            var coupon = PortalUtility.ReadCoupon(couponid);
            return Json(new
            {
                id = coupon.Id,
                Name = coupon.Name,
                CouponCode = coupon.Code,
                Active = coupon.Active
            });
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult AddEditCoupons(int? id, int orgid, string name, string code, bool active)
        {
            var response = PortalUtility.AddEditCoupons(id, orgid, name, code, active);
            if (response.success)
            {
                return Json(new { Result = "OK" });
            }
            else
            {
                return Json(new { Result = "Not ok" });
            }
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult SendEmail(int typeId)
        {
            NotificationUtility.CreateDirectMail(NotificationEventTypeDto.GetByKey(typeId), HttpContext.Session.GetInt32(SessionContext.UserId).Value, _appSettings.InfoEmail, _appSettings.SecureEmail, _appSettings.SMPTAddress, _appSettings.PortNumber, _appSettings.SecureEmailPassword, _appSettings.MailAttachmentPath);
            return Json(new { Result = "OK" });
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult ReadOrganization(int OrganizationId)
        {
            var organization = PortalUtility.ReadOrganization(null, OrganizationId).organization;
            var parentOrg = organization.Organization1 != null ? organization.Organization1.Id : null;
            return Json(new
            {
                Result = "OK",
                Records = organization.Id,
                organization.Name,
                organization.Description,
                parentOrg,
                organization.Url,
                organization.EmailValidationRequired,
                organization.UserRoles,
                organization.ContactNumber,
                organization.ContactEmail,
                organization.Active,
                organization.SSO,
                organization.TermsForSSO,
                organization.OwnCoach,
                organization.IntegrationWith
            });
        }
        [ModuleControl(Modules.Organizations)]
        public ActionResult OrganizationDetails(int Id)
        {
            OrganizationDetailsModel model = new OrganizationDetailsModel();
            model.Groups = AccountUtility.ListRoles().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            model.Integrations = CommonUtility.GetIntegrationsList();
            model.ParentOrganizations = PortalUtility.GetParentOrganizationsDetails().Organizations.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(y => y.Text).ToList();
            model.Organization = PortalUtility.GetOrganizationById(Id);
            model.Portals = PortalUtility.ListPortals(Id).portals.OrderByDescending(x => x.Id).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            model.languages = CommonUtility.GetLanguages().Languages.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.Id.ToString() }).ToList();
            model.specializations = AccountUtility.ListSpecialization(false, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId)).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageId), Value = x.Id.ToString() });
            //model.yesorNoQuestion = CommonUtility.GetBoolQuestion().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).ToList();
            //model.hraOption = CommonUtility.GetHRAOption().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).ToList();
            ViewData["languageList"] = CommonUtility.GetLanguages().Languages.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.LanguageCode });
            model.kits = KitUtility.ListEduKits(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId), null, null, null, false).EduKits.Select(x => new SelectListItem { Text = x.InvId + " : " + x.Description, Value = x.Id.ToString() }).ToList();
            model.emails = NotificationUtility.ListNotificationEventType().emails.Select(x => new SelectListItem { Text = x.Description, Value = x.Id.ToString() }).ToList();
            model.dateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.BaseUrl = _appSettings.EmailUrl;
            return View(model);
        }
        [ModuleControl(Modules.Organizations)]
        public JsonResult CreateOrganization(string Name, string Description, int? Id, int? parentOrg, string groups, string url, string contactnumber, string contactemail, bool IsAutoActivateEmail, bool IsActive, bool SSO, bool TermsForSSO, bool OwnCoach, byte? IntegrationWith)
        {
            var response = PortalUtility.AddEditOrganization(Name, Description, Id, parentOrg, groups, url, contactnumber, contactemail, IsAutoActivateEmail, IsActive, SSO, TermsForSSO, OwnCoach, IntegrationWith);
            return Json(new { Result = "OK", Portal = response.Organization });
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult DeleteOrganization(int Id)
        {
            if (PortalUtility.DeleteOrganization(Id))
                return Json(new { Result = "OK" });
            else
                return Json(new { Result = "FAIL" });
        }

        [ModuleControl(Modules.Organizations)]
        public ActionResult Portal(int id)
        {
            PortalModel model = new PortalModel();
            PortalDto portal = new PortalDto();
            portal.Id = id;
            model.portal = portal;
            model.languages = CommonUtility.GetLanguages().Languages.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.Id.ToString() }).ToList();
            model.specializations = AccountUtility.ListSpecialization(false, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : 0).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageId), Value = x.Id.ToString() });
            model.FollowUpTypes = CommonUtility.GetFollowUpTypes().Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() }).ToList();
            model.emails = NotificationUtility.ListNotificationEventType().emails.Select(x => new SelectListItem { Text = x.Description, Value = x.Id.ToString() }).ToList();
            model.kits = KitUtility.ListEduKits(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId), null, null, null, false).EduKits.Select(x => new SelectListItem { Text = x.InvId + " : " + x.Description, Value = x.Id.ToString() }).ToList();
            model.HRAStatus = CommonUtility.GetHRAStatus();
            model.CarePlanTypes = CommonUtility.GetCarePlanTypes();
            model.HRAVersions = CommonUtility.GetHRAVersions();
            model.EligibilityFormats = CommonUtility.GetEligibilityFormats();
            model.ProviderDetails = CommonUtility.GetProviderDetails();
            model.EligibilitytoIntuity = CommonUtility.GetEligibilitytoIntuity();
            ViewData["languageList"] = CommonUtility.GetLanguages().Languages.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.LanguageCode });
            model.yesorNoQuestion = CommonUtility.GetBoolQuestion().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).ToList();
            model.portal = PortalUtility.ReadPortal(id).portal;
            model.LabProcedures = PortalUtility.GetLabProcedures().Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() }).ToList();
            model.CoachingConditions = PortalUtility.GetCoachingConditions().Select(x => new SelectListItem { Text = x.Condition, Value = x.Id.ToString() }).ToList();
            model.incentiveCount = PortalUtility.GetPortalIncentives(id).portalIncentives.Count();
            model.kitsCount = PortalUtility.ListKitsforPortal(id).Count();
            var email = PortalUtility.GetPortalEmails(id).NotificationEventTypePortals;
            model.emailCount = email != null ? email.Count() : 0;
            int formsCount = 0;
            if (!string.IsNullOrEmpty(model.portal.PatientReleaseForm))
                formsCount++;
            if (!string.IsNullOrEmpty(model.portal.MedicalClearanceForm))
                formsCount++;
            if (!string.IsNullOrEmpty(model.portal.KnowYourNumbersForm))
                formsCount++;
            if (!string.IsNullOrEmpty(model.portal.TestimonialForm))
                formsCount++;
            if (!string.IsNullOrEmpty(model.portal.TobaccoReleaseForm))
                formsCount++;
            model.formsCount = formsCount;
            model.couponsCount = PortalUtility.ListCoupons(model.portal.OrganizationId, false).Count();
            model.rafflesCount = PortalUtility.GetPortalRaffles(id).rafflesinPortals.Count();
            model.kitsinFollowUpCount = PortalUtility.GetPortalFollowUps(id, null, true).portalFollowUps.Count();
            model.dateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return PartialView("_PortalDetails", model);
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult ListPortals(int OrganizationId)
        {
            var response = PortalUtility.ListPortals(OrganizationId);
            return Json(new
            {
                Result = "OK",
                Records = response.portals.Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.StartDate,
                    x.EndDate
                }),
                response.OrgName
            });
        }

        [Authorize]
        public JsonResult ReadPortal(int portalId)
        {
            var response = PortalUtility.ReadPortal(portalId);

            var programsinPortal = response.portal.ProgramsinPortal.Select(x => new
            {
                Id = x.Id,
                PortalId = x.PortalId,
                ProgramId = x.ProgramId,
                Name = x.program.Name,
                Type = x.program.ProgramType,
                Cost = x.Cost,
                NoOfCalls = x.ApptCallTemplateId.HasValue ? x.ApptCallTemplate.NoOfCalls.ToString() : "N/A",
                MoreInfo = x.MoreInfo,
                Active = x.Active
            });
            return Json(new
            {
                Result = "OK",
                Records = response.portal.Id,
                response.portal.Name,
                response.portal.StartDate,
                response.portal.EndDate,
                response.portal.HRAValidity,
                response.portal.HasHRA,
                response.portal.HasCoachingProgram,
                response.portal.HRAforCoachingProgram,
                response.portal.FollowUpforCoaching,
                response.portal.HasSelfHelpProgram,
                response.portal.HRAforSelfHelpProgram,
                response.portal.FollowUpforSelfHelp,
                response.portal.Active,
                response.portal.AWV,
                response.portal.LabIntegration,
                response.portal.AppointmentCalls,
                response.portal.CampaignCalls,
                response.portal.CACScanQuestion,
                response.portal.GINAQuestion,
                response.portal.ShowProgramOption,
                response.portal.SelfScheduling,
                response.portal.ShowPricing,
                response.portal.ClientNameInReport,
                response.portal.ProcessLivongoCodes,
                response.portal.TrackingCalls,
                response.portal.NoOfTrackingCalls,
                response.portal.IncompleteHRA,
                response.portal.HRAOnly,
                response.portal.HRAandBiometrics,
                response.portal.FirstCoaching,
                response.portal.CompletedCoaching,
                response.portal.FirstTimeHRA,
                response.portal.ReportText,
                response.portal.ProgramText,
                response.portal.ContactText,
                response.portal.LabCorpAttachment,
                response.portal.DoctorOfficeAttachment,
                NoProgDays = response.portal.NoProgDays,
                response.portal.AutoImmune,
                MailScoreCard = response.portal.MailScoreCard,
                CarePlan = response.portal.CarePlan,
                CarePlanType = response.portal.CarePlanType,
                response.portal.HRAVer,
                response.portal.EligibilityFormat,
                response.portal.EligibilityImportLoadFlag,
                response.portal.EligibilityFolderPath,
                response.portal.EligibilityTerminateFlag,
                response.portal.ProcessInterventCodes,
                response.portal.ClaimsDirectoryPath,
                response.portal.AssignPrograms,
                response.portal.usePreviousLabs,
                response.portal.ValidLabs,
                response.portal.PatientReleaseForm,
                response.portal.MedicalClearanceForm,
                response.portal.KnowYourNumbersForm,
                response.portal.TestimonialForm,
                response.portal.TobaccoReleaseForm,
                response.portal.Languages,
                response.portal.Specializations,
                response.portal.PortalFollowUps,
                response.portal.FollowUpTypes,
                response.portal.AllowCardiacQuestion,
                response.portal.FollowUpValidity,
                response.portal.CampaignStartDate,
                response.portal.CampaignEndDate,
                response.portal.NeedCareplanApproval,
                response.portal.CareplanPath,
                response.portal.EligtoIntuity,
                response.portal.EmployerIncentiveText,
                programsinPortal
                //response.portal.UserRoles
            });
        }
        [Authorize]
        public JsonResult GetProgramsinPortal(int portalId)
        {
            var response = PortalUtility.ReadPortal(portalId);
            return Json(new
            {
                Result = "OK",
                Records = response.portal.ProgramsinPortal.Select(x => new
                {
                    Id = x.Id,
                    PortalId = x.PortalId,
                    ProgramId = x.ProgramId,
                    Name = x.program.Name,
                    Type = x.program.ProgramType,
                    Cost = x.Cost,
                    NoOfCalls = x.ApptCallTemplateId.HasValue ? x.ApptCallTemplate.NoOfCalls.ToString() : "N/A",
                    MoreInfo = x.MoreInfo,
                    Active = x.Active
                })
            });
        }

        [ModuleControl(Modules.Organizations)]
        [HttpPost]
        public ActionResult AddEditKits()
        {
            PortalModel model = new PortalModel();
            model.kits = KitUtility.ListEduKits(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId), null, null, null, false).EduKits.Select(x => new SelectListItem { Text = x.InvId + " : " + x.Description, Value = x.Id.ToString() }).ToList();
            return PartialView("_AddEditKits", model);
        }

        [ModuleControl(Modules.Organizations)]
        [HttpPost]
        public ActionResult AddEditEmails()
        {
            PortalModel model = new PortalModel();
            model.emails = NotificationUtility.ListNotificationEventType().emails.Select(x => new SelectListItem { Text = x.Description, Value = x.Id.ToString() }).ToList();
            return PartialView("_AddEditEmails", model);
        }

        [ModuleControl(Modules.Organizations)]
        [HttpPost]
        public JsonResult AddPortal([FromBody] PortalModel model)
        {
            var response = PortalUtility.AddPortal(model);
            return Json(new { Result = "OK", Portal = response.Portal });
        }

        [ModuleControl(Modules.Organizations)]
        [HttpPost]
        public JsonResult EditPortal(PortalModel model)
        {
            var response = PortalUtility.EditPortal(model);
            return Json(new { Result = "OK", Portal = response.Portal });
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult DeletePortal(int Id)
        {
            if (PortalUtility.DeletePortal(Id))
                return Json(new { Result = "OK" });
            else
                return Json(new { Result = "FAIL" });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult ReadPortalFollowUps(int portalId)
        {
            var response = PortalUtility.GetPortalFollowUps(portalId, null, true);
            var portalFollowUps = response.portalFollowUps.Select(x => new
            {
                Id = x.Id,
                FollowUpType = x.FollowUpType.Type,
                ProgramType = x.ProgramType == (int)ProgramTypes.SelfHelp ? "Self-Help" : "Coaching"
            });
            return Json(new { Result = "OK", Records = portalFollowUps });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult ReadFollowUpKits(int portalFollowUpId)
        {
            var response = PortalUtility.ReadFollowUpKits(portalFollowUpId);
            var kitsinPortalFollowUps = response.kitsinPortalFollowUps.Where(x => x.Active == true).OrderBy(x => x.Order).Select(x => new
            {
                Id = x.eduKit.Id,
                Name = x.eduKit.Name,
                Order = x.Order
            });
            return Json(new { Result = "OK", Records = kitsinPortalFollowUps });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult ListEduKits(int PortalFollowUpId)
        {
            var response = PortalUtility.GetPortalFollowUps(10, HttpContext.Session.GetInt32(SessionContext.ProgramType).Value);
            var eduKits = response.portalFollowUps.FirstOrDefault().KitsinPortalFollowUps.Where(x => x.Active == true).OrderBy(x => x.Order).Select(x => new
            {
                Id = x.eduKit.Id,
                Name = x.eduKit.Name,
                Order = x.Order
            });
            return Json(new { Result = "OK", Records = response });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult AddKittoFollowUp(int kitId, int portalFollowUpId, short order)
        {
            var response = PortalUtility.AddKittoFollowUp(kitId, portalFollowUpId, order);
            return Json(new { Result = "OK", Record = response.kitsinPortalFollowUp });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult DeleteKitfromFollowUp(int portalFollowUpId, int KitId)
        {
            var response = PortalUtility.DeleteKitfromFollowUp(portalFollowUpId, KitId).success;

            return Json(new { Result = "OK" });
        }

        #endregion

        #region Program in Portals

        [ModuleControl(Modules.Organizations)]
        public JsonResult ReadPrograminPortal(int PrograminPortalId, string language = null)
        {
            var response = PortalUtility.ReadPrograminPortal(PrograminPortalId, language);
            var NoOfCalls = response.programinPortal.ApptCallTemplate != null ? response.programinPortal.ApptCallTemplate.NoOfCalls : 0;
            return Json(new
            {
                response.programinPortal.Id,
                response.programinPortal.ProgramId,
                NoOfCalls,
                response.programinPortal.Cost,
                response.programinPortal.MoreInfo,
                response.programinPortal.Active,
                response.programinPortal.ApptCallTemplateId,
                response.programinPortal.NameforUser,
                response.programinPortal.DescriptionforUser,
                response.programinPortal.NameforAdmin,
                response.programinPortal.DescriptionforAdmin
            });
        }

        [Authorize]
        public ActionResult GetProgramDetails(int PrograminPortalId)
        {
            ProgramDetailsModel model = new ProgramDetailsModel();
            model.programLists = ProgramUtility.ListPrograms(null, null, null, true).Programs.Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() }).OrderBy(s => s.Text);
            model.programinPortal = PortalUtility.ReadPrograminPortal(PrograminPortalId, null).programinPortal;
            model.callTemplateList = ProgramUtility.ListAppointmentCallTemplates().CallTemplates.Select(c => new SelectListItem { Text = c.TemplateName, Value = c.Id.ToString() }).OrderBy(s => s.Text);
            ViewData["languageList"] = CommonUtility.GetLanguages().Languages.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.LanguageCode });

            return PartialView("_PrograminPortal", model);
        }

        [ModuleControl(Modules.Organizations)]
        [HttpPost]
        public JsonResult AddProgramsinPortal(int? id, int portalId, short progId, decimal progCost, string moreInfo, bool isactive, string nameforUser, string descriptionforUser, string nameforAdmin, string descriptionforAdmin, int? apptCallTemplateId, string language, int? sortOrder)
        {
            var response = PortalUtility.AddEditProgramsinPortal(id, portalId, progId, progCost, moreInfo, isactive, nameforUser, descriptionforUser, nameforAdmin, descriptionforAdmin, apptCallTemplateId, language, sortOrder);
            return Json(new { Result = "OK", Record = response.ProgramsinPortal });
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult DeleteProgramsinPortal(int PortalId, int ProgramId)
        {
            if (PortalUtility.DeleteProgramsinPortal(PortalId, ProgramId).success)
                return Json(new { Result = "OK" });
            else
                return Json(new { Result = "FAIL" });
        }

        #endregion

        #region Portal Incentives

        [ModuleControl(Modules.Organizations)]
        public JsonResult ReadIncentive(int portalId)
        {
            return Json(PortalUtility.GetPortalIncentives(portalId));
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult AddEditPortalIncentive(int? id, int portalId, string name, int incentiveType, float points, string pointsText, bool isPoint, bool isCompanyIncentive, string imageUrl, bool isActive, int? programPortalId, int? refValue, int? refValue2, int? refValue3, string attachment, string moreInfo, string langname, string langMoreInfo, bool isGiftCard, bool removeSurcharge)
        {
            var response = PortalUtility.AddPortalIncentive(id, portalId, name, incentiveType, points, pointsText, isPoint, isCompanyIncentive, imageUrl, isActive, programPortalId, refValue, refValue2, refValue3, attachment, moreInfo, langname, langMoreInfo, isGiftCard, removeSurcharge);
            return Json(new { IsSuccess = response, Incentives = PortalUtility.GetPortalIncentives(portalId) });
        }

        [Authorize]
        public ActionResult GetIncentiveDetails(int portalId, int portalIncentiveId)
        {
            IncentiveDetailsModel model = new IncentiveDetailsModel();
            model.incentiveTypes = PortalUtility.GetIncentiveTypes().Select(c => new SelectListItem { Text = c.Text, Value = c.Value.ToString() });
            model.programTypes = PortalUtility.GetProgramsByPortal(portalId, (int)ProgramTypes.Coaching, true).ProgramsinPortal.Select(x => new SelectListItem { Text = x.program.Name, Value = x.Id.ToString() });
            model.portalIncentive = PortalUtility.GetPortalIncentives(portalId).portalIncentives.Where(x => x.Id == portalIncentiveId).FirstOrDefault();
            if (model.portalIncentive == null)
                model.portalIncentive = new PortalIncentiveDto();
            return PartialView("_IncentiveinPortal", model);
        }

        [Authorize]
        public ActionResult GetRaffleDetails(int portalId, int raffleId)
        {
            RaffleDetailsModel model = new RaffleDetailsModel();
            model.raffleTypes = CommonUtility.GetRaffleTypes().Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            model.rafflesinPortals = PortalUtility.GetPortalRaffles(portalId).rafflesinPortals.Where(x => x.Id == raffleId).FirstOrDefault();
            if (model.rafflesinPortals == null)
                model.rafflesinPortals = new RafflesinPortalsDto();
            return PartialView("_RafflesinPortal", model);
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult GetListOptions(string type, int PortalId)
        {
            if (type.ToLower().Equals("programtype"))
            {
                return Json(new { Result = "OK", Programs = PortalUtility.GetProgramsByPortal(PortalId, null, true) });
            }
            return Json("true");
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult GetIncentiveTypes(int portalId)
        {
            return Json(new { IncentiveTypes = PortalUtility.GetIncentiveTypes(), PortalPrograms = PortalUtility.GetProgramsByPortal(portalId, (int)ProgramTypes.Coaching, true).ProgramsinPortal.Select(x => new SelectListItem { Text = x.program.Name, Value = x.Id.ToString() }) });
        }

        #endregion

        #region Portal Emails
        [ModuleControl(Modules.Organizations)]
        public JsonResult AddEditPortalEmails(int portalId, string types)
        {
            var response = PortalUtility.AddEditPortalEmails(portalId, types);
            return Json(new { IsSuccess = response });
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult GetPortalEmails(int portalId)
        {
            var emails = PortalUtility.GetPortalEmails(portalId);
            return Json(new { Emails = emails.NotificationEventTypePortals });
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult ListKitsforPortals(int portalId)
        {
            var kits = PortalUtility.ListKitsforPortal(portalId);
            return Json(new { Kits = kits });
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult AddEditPortalKits(int portalId, string kits)
        {
            var response = PortalUtility.AddEditPortalKits(portalId, kits);
            return Json(new { IsSuccess = response });
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult CopyKitsFromPreviousPortal(int portalId)
        {
            var response = PortalUtility.CopyKitsFromPreviousPortal(portalId);
            return Json(new { Result = true });
        }
        #endregion

        #region Portal Raffles
        [ModuleControl(Modules.Organizations)]
        public JsonResult AddEditPortalRaffles(int? id, int portalId, int raffleType, Dictionary<int, DateTime?> date, string reward, string name, string description, bool isActive, int pointType)
        {
            var response = PortalUtility.AddEditPortalRafflles(id, portalId, raffleType, date, reward, name, description, isActive, pointType);
            return Json(new { IsSuccess = response });
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult GetPortalRaffles(int portalId)
        {
            var raffles = PortalUtility.GetPortalRaffles(portalId);
            return Json(new
            {
                Raffles = raffles.rafflesinPortals.Select(x => new
                {
                    x.Id,
                    x.RaffleTypes,
                    x.Reward,
                    x.isActive,
                    x.Points
                })
            });
        }

        [ModuleControl(Modules.Organizations)]
        public JsonResult GetPortalRafflesById(int Id)
        {
            var raffles = PortalUtility.GetPortalRafflesById(Id);
            return Json(new RafflesinPortalsDto
            {
                Id = raffles.Id,
                RaffleTypeId = raffles.RaffleTypeId,
                Reward = raffles.Reward,
                Name = raffles.Name,
                Description = raffles.Description,
                RaffleDates = raffles.RaffleDates,
                isActive = raffles.isActive,
                Points = raffles.Points
            });
        }
        #endregion

        #region Forms

        [ModuleControl(Modules.Organizations)]
        public JsonResult AddForms(int portalId, string patientReleaseForm, string medicalClearanceForm, string knowYourNumbersForm, string testimonialForm, string tobaccoReleaseForm)
        {
            var response = PortalUtility.AddForms(portalId, patientReleaseForm, medicalClearanceForm, knowYourNumbersForm, testimonialForm, tobaccoReleaseForm);
            return Json(new { IsSuccess = response });
        }


        #endregion

        public JsonResult ClonePortal(int portalId)
        {
            var response = PortalUtility.ClonePortal(portalId);
            return Json(new { Result = "OK", PortalId = response });
        }
    }
}