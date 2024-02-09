using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace InterventWebApp
{
    public class LabController : BaseController
    {
        private readonly AppSettings _appSettings;

        public LabController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        [Authorize]
        public ActionResult MyLabs()
        {
            TempData.Remove("AdminView");
            return View();
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AdminView()
        {
            TempData["AdminView"] = "True";
            return PartialView("_AdminView");
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddLabSelection(int labSource)
        {
            var response = LabUtility.AddLabSelection(labSource, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue ? HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value : null, HttpContext.Session.GetString(SessionContext.ParticipantEmail), HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value, _appSettings.LabCorpUserName, _appSettings.LabCorpPassword, _appSettings.LabCorpAccountNumber);
            return Json(new { Result = "OK", LabOrder = response, AdminView = TempData["AdminView"] != null ? TempData["AdminView"].ToString() : "False" });
        }

        [Authorize]
        [HttpPost]
        public JsonResult UpdateLabSelection(int id, byte selection, int? labId)
        {
            var response = LabUtility.UpdateLabSelection(id, selection, labId, HttpContext.Session.GetInt32(SessionContext.UserId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetString(SessionContext.ParticipantLanguagePreference), HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value, _appSettings.LabCorpUserName, _appSettings.LabCorpPassword, _appSettings.LabCorpAccountNumber);
            return Json(new { Result = "OK", LabOrder = response, AdminView = TempData["AdminView"] != null ? TempData["AdminView"].ToString() : "False" });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AddLabValues(int? id, bool? update)
        {
            AddLabModel model = new AddLabModel();
            var measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)));
            model.Measurements = measurements.Measurements;
            model.Id = id.HasValue ? id.Value : 0;
            model.Unit = HttpContext.Session.GetInt32(SessionContext.Unit);
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.IsParticipantView = HttpContext.Session.GetString(SessionContext.IsParticipantView) == "true";
            var labwork = LabUtility.ReadLabWork(id, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value).Lab;
            if (update.HasValue && update.Value && labwork != null)
            {
                if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric)
                {
                    model.Lab = CommonUtility.ConvertIntoMetric(labwork, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                }
                else if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Imperial)
                {
                    model.Lab = labwork;
                    if (model.Lab.Height.HasValue)
                    {
                        var height = CommonUtility.ToFeetInches(model.Lab.Height.Value);
                        model.HeightFt = height.Key;
                        model.HeightInch = (float)Math.Round(height.Value, 2);
                    }
                    model.Lab = CommonUtility.Roundoff(labwork, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                }
                model.BloodTestDate = labwork.BloodTestDateString;
                model.updateLab = update.Value;
            }
            return PartialView("_AddLabs", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult AddLabValues(AddLabModel model)
        {
            if (model.Id != 0)
            {
                model.Lab.Id = model.Id;
                if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric)
                {
                    CommonUtility.CheckIfUpdated(model.Lab, HealthNumberType.Labs, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                }
            }
            else
            {
                if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric)
                {
                    model.Lab = CommonUtility.ConvertIntoImperial(model.Lab, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                }

            }
            if (!model.Lab.LabSelection.HasValue)
            {
                model.Lab.LabSelection = 1;
            }
            if (HttpContext.Session.GetInt32(SessionContext.HRAValidity).HasValue)
                model.HRAValidity = HttpContext.Session.GetInt32(SessionContext.HRAValidity);
            if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                model.updatedBy = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            if (HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue)
                model.participantPortalId = HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value;
            if (HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue)
                model.participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            //Value should be updated in the corresponding follow up
            LabUtility.UpdateLabs(_appSettings.SystemAdminId, model);
            return Json(new { Result = "OK", AdminView = TempData["AdminView"] != null ? TempData["AdminView"].ToString() : "False" });
        }

        [Authorize]
        public ActionResult LabResults()
        {
            var timeZone = "";
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)))
                timeZone = HttpContext.Session.GetString(SessionContext.ParticipantTimeZone);
            else
                timeZone = User.TimeZone();
            var custzone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            LabResults model = new LabResults();
            model.LabRejectionReasons = ListOptions.GetLabRejectionReasons().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
            model.LabList = LabUtility.GetDiagnosticLabs().Select(x => new SelectListItem { Text = x.LabName, Value = x.Id.ToString() });
            model.Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
            model.LabReferences = LabUtility.ListLabReferenceRanges().LabReferences;
            var portalId = GetParticipantPortalId();
            var portal = PortalUtility.ReadPortal(portalId).portal;
            var validDate = new DateTime();
            if (HttpContext.Session.GetString(SessionContext.HRACompleteDate) != null)
            {
                validDate = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(HttpContext.Session.GetString(SessionContext.HRACompleteDate)).AddYears(-1), custzone) > Convert.ToDateTime(portal.ValidLabs)
                ? TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(HttpContext.Session.GetString(SessionContext.HRACompleteDate)).AddYears(-1), custzone) : Convert.ToDateTime(portal.ValidLabs);
            }
            else
            {
                validDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddYears(-1), custzone) > Convert.ToDateTime(portal.ValidLabs)
                   ? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddYears(-1), custzone) : Convert.ToDateTime(portal.ValidLabs);
            }
            model.ValidLabs = portal.usePreviousLabs ? validDate.ToShortDateString() : "";
            model.LabIntegration = portal.LabIntegration;
            model.UserName = Convert.ToString(HttpContext.Session.GetString(SessionContext.ParticipantName));
            var labs = LabUtility.ListLabs(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).Labs;
            if (!String.IsNullOrEmpty(model.ValidLabs))
                model.hasRecentLabs = (labs.Where(x => x.BloodTestDate >= Convert.ToDateTime(model.ValidLabs)).Count() > 0);
            var lab = labs.Where(x => x.PortalId == portalId).OrderByDescending(x => x.Id).FirstOrDefault();
            if (lab != null && !lab.DateCompleted.HasValue)
                model.incompleteLab = lab;
            if (model.incompleteLab != null)
            {
                model.incompleteLabId = model.incompleteLab.Id;
                model.DiagnosticLabId = model.incompleteLab.DiagnosticLabId;
                if (labs.Where(x => x.PortalId == portalId).Count() > 1)
                    model.SwitchCount = 1;
                else
                    model.SwitchCount = model.incompleteLab.SwitchCount.HasValue ? model.incompleteLab.SwitchCount.Value : 0;
            }
            else
            {
                model.incompleteLabId = 0;
            }
            model.Labs = labs.Where(x => x.DateCompleted.HasValue).ToList();
            model.LabsForCurrentPortal = labs.Where(x => x.PortalId == portalId).Count();
            if (model.Labs.Count > 0)
            {
                for (int i = 0; i < model.Labs.Count; i++)
                {
                    LabUtility.ProcessLabResults(model.Labs[i], model.LabReferences, HttpContext.Session.GetString(SessionContext.DateFormat));
                    if (model.Labs[i].Height.HasValue)
                    {
                        var height = CommonUtility.ToFeetInches(model.Labs[i].Height.Value);
                        model.Labs[i].HeightFt = height.Key;
                        model.Labs[i].HeightInch = (float)Math.Round(height.Value);
                    }
                    float Height = 0;
                    if (HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue)
                    {
                        var hra = HRAUtility.ReadHRA(HttpContext.Session.GetInt32(SessionContext.HRAId).Value);
                        if (hra.hra != null && hra.hra.Goals != null)
                        {
                            Height = hra.hra.HealthNumbers.Height.Value;
                            model.Labs[i].WeightMax = (int)hra.hra.Goals.LtWt;
                        }
                    }
                    Height = model.Labs[i].Height.HasValue ? model.Labs[i].Height.Value : Height;
                    model.Labs[i].WeightMax = model.Labs[i].WeightMax.HasValue ? model.Labs[i].WeightMax : (int)ReportUtility.GetWeightfromBMI(Height, (float)24.9);
                    model.Labs[i].WeightMin = (int)ReportUtility.GetWeightfromBMI(Height, (float)18.5);
                    if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric)
                    {
                        model.Labs[i] = CommonUtility.ConvertIntoMetric(model.Labs[i], HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                        model.Labs[i].WeightMax = (int)CommonUtility.ToMetric((float)model.Labs[i].WeightMax, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                        model.Labs[i].WeightMin = (int)CommonUtility.ToMetric((float)model.Labs[i].WeightMin, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                    }
                    else if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Imperial)
                    {
                        model.Labs[i] = CommonUtility.Roundoff(model.Labs[i], HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                    }
                    model.Labs[i].hasLabCorpPdf = LabUtility.PullLabReport(model.Labs[i].Id, false, HttpContext.Session.GetInt32(SessionContext.UserId).Value).Data != null ? true : false;
                    if (!model.Labs[i].DiagnosticLabId.HasValue)
                    {
                        model.Labs[i].LabSource = model.Labs[i].LabSelection == LabSelection.DoctorsOffice ? Translate.Message("L3155") : Translate.Message("L3156");
                    }
                    else
                        model.Labs[i].LabSource = model.LabList.FirstOrDefault(x => x.Value == model.Labs[i].DiagnosticLabId.Value.ToString()).Text;
                }
            }
            if ((Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))) == (int)Unit.Metric)
            {
                //model.LabReferences = LabUtility.ListLabReferenceRanges().LabReferences;
                model.LabReferences[LabReference.TotalCholesterol].NormalMax = CommonUtility.ToMetric(model.LabReferences[LabReference.TotalCholesterol].NormalMax.Value + 1, BioLookup.Cholesterol, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                model.LabReferences[LabReference.LDL].NormalMax = CommonUtility.ToMetric(model.LabReferences[LabReference.LDL].NormalMax.Value + 1, BioLookup.LDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                model.LabReferences[LabReference.GlucoseFasting].NormalMin = CommonUtility.ToMetric(model.LabReferences[LabReference.GlucoseFasting].NormalMin.Value, BioLookup.Glucose, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                model.LabReferences[LabReference.GlucoseFasting].NormalMax = CommonUtility.ToMetric(model.LabReferences[LabReference.GlucoseFasting].NormalMax.Value, BioLookup.Glucose, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                model.LabReferences[LabReference.GlucoseNonFasting].NormalMin = CommonUtility.ToMetric(model.LabReferences[LabReference.GlucoseNonFasting].NormalMin.Value, BioLookup.Glucose, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                model.LabReferences[LabReference.GlucoseNonFasting].NormalMax = CommonUtility.ToMetric(model.LabReferences[LabReference.GlucoseNonFasting].NormalMax.Value, BioLookup.Glucose, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                model.LabReferences[LabReference.Triglycerides].NormalMax = CommonUtility.ToMetric(model.LabReferences[LabReference.Triglycerides].NormalMax.Value + 1, BioLookup.Triglycerides, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                model.LabReferences[LabReference.HDL].NormalforMale = CommonUtility.ToMetric(model.LabReferences[LabReference.HDL].NormalforMale.Value, BioLookup.HDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                model.LabReferences[LabReference.HDL].NormalforFemale = CommonUtility.ToMetric(model.LabReferences[LabReference.HDL].NormalforFemale.Value, BioLookup.HDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                model.LabReferences[LabReference.WaistCircumference].NormalforMale = CommonUtility.ToMetric(model.LabReferences[LabReference.WaistCircumference].NormalforMale.Value, BioLookup.Waist, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                model.LabReferences[LabReference.WaistCircumference].NormalforFemale = CommonUtility.ToMetric(model.LabReferences[LabReference.WaistCircumference].NormalforFemale.Value, BioLookup.Waist, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
            }
            else if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Imperial)
            {
                model.LabReferences[LabReference.TotalCholesterol].NormalMax = model.LabReferences[LabReference.TotalCholesterol].NormalMax + 1;
                model.LabReferences[LabReference.LDL].NormalMax = model.LabReferences[LabReference.LDL].NormalMax.Value + 1;
                model.LabReferences[LabReference.Triglycerides].NormalMax = model.LabReferences[LabReference.Triglycerides].NormalMax.Value + 1;
            }
            if (HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue)
                model.OrganizationId = HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value;
            model.IsParticipantView = CommonUtility.IsParticipantView(HttpContext.Session.GetString(SessionContext.IsParticipantView));
            model.HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            model.IntegrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            model.ParticipantPortalId = HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId);
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.Gender = HttpContext.Session.GetInt32(SessionContext.Gender);
            model.Unit = HttpContext.Session.GetInt32(SessionContext.Unit);
            model.southUniversityOrgId = _appSettings.SouthUniversityOrgId;
            return PartialView("_LabResults", model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult TakeAction(int id)
        {
            var response = LabUtility.TakeAction(id, Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.AdminId).Value.ToString())).success;
            return Json(new { Result = "OK" });
        }

        [Authorize]
        public JsonResult ResendLabRequisitionEmail(int id, string labOrderNo)
        {
            var response = LabUtility.ResendLabRequisitionEmail(id, labOrderNo, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value);
            return Json(new { Result = "OK" });
        }

        [Authorize]
        public ActionResult PullLabReport(int id, bool? fromLog)
        {
            var islog = fromLog.HasValue ? fromLog.Value : false;
            var result = LabUtility.PullLabReport(id, islog, HttpContext.Session.GetInt32(SessionContext.UserId).Value).Data;
            if (result != null)
            {
                FileResult fileResult = new FileContentResult(result, "application/pdf");
                return fileResult;
            }

            return RedirectToAction("NotAuthorized", "Account");

        }

        [Authorize]
        public ActionResult PullLabOrderConfirmation(string id)
        {
            var result = LabUtility.PullLabOrder(id, _appSettings.LabCorpUserName, _appSettings.LabCorpPassword, _appSettings.LabCorpAccountNumber);
            if (result != null)
            {
                FileResult fileResult = new FileContentResult(result, "application/pdf");
                return fileResult;
            }

            return RedirectToAction("NotAuthorized", "Account");

        }
    }
}