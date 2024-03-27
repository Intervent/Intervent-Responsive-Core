using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace InterventWebApp
{
	public class FollowUpController : BaseController
    {
		private readonly AppSettings _appSettings;

		public FollowUpController(IOptions<AppSettings> appSettings)
		{
			_appSettings = appSettings.Value;
		}

		[Authorize]
        public ActionResult FollowUpDashboard()
        {
            if (!(HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp).HasValue && Convert.ToByte(HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp)) > 0))
                return RedirectToAction("Stream", "Participant");
            FollowUpDto dto = new FollowUpDto();
            if (HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue)
            {
                dto = FollowUpUtility.ReadFollowUp(HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value).FollowUpDto;
                if (dto != null)
                {
                    HttpContext.Session.SetString(SessionContext.FollowUpPageSeqDone, dto.PageSeqDone);
                    HttpContext.Session.SetString(SessionContext.FollowUpCompleteDate, dto.CompleteDate.ToString());
                    dto.PercentComplete = FollowUpUtility.GetFolowUpCompletionPercent(HttpContext.Session.GetString(SessionContext.FollowUpPageSeqDone) != null ? HttpContext.Session.GetString(SessionContext.FollowUpPageSeqDone) : "");
                }
            }
            return View(dto);
        }

        [Authorize]
        public ActionResult FollowUp(int followupid)
        {
            FollowUpDto model = new FollowUpDto();
            if (!(followupid > 0))
                followupid = HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue ? HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value : 0;
            var response = FollowUpUtility.ReadFollowUp(followupid);
            if (response.FollowUpDto != null)
                model = response.FollowUpDto;
            return PartialView("_FollowUp", model);
        }

        public bool FollowUpReadOnly(int followupid)
        {
            if (!Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal)))
                return true;
            if ((HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue && (followupid > 0 && followupid != HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value)) || (!HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue && followupid > 0))
                return true;
            if (!HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue && (HttpContext.Session.GetString(SessionContext.FollowUpCompleteDate) != null
                && Convert.ToDateTime(HttpContext.Session.GetString(SessionContext.FollowUpCompleteDate)).AddDays(HttpContext.Session.GetInt32(SessionContext.FollowUpValidity).Value) <= DateTime.UtcNow))
                return true;
            else
                return false;
        }

        [Authorize]
        public ActionResult MedicalConditions(string viewType, int followupid)
        {
            FU_MedicalConditionsModel model = new FU_MedicalConditionsModel();
            int userinProgramId;
            if (followupid > 0)
            {
                var response = FollowUpUtility.ReadFollowUp(followupid);
                userinProgramId = response.FollowUpDto.UsersinProgramsId;
            }
            else
            {
                if (HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue)
                    followupid = HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value;
                userinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value;
            }
            model.MedicalConditions = FollowUpUtility.ReadMedicalCondition(followupid, userinProgramId).FollowUp_MedicalConditionsDto;
            model.readOnly = FollowUpReadOnly(followupid);
            ViewBag.viewType = viewType;
            return PartialView("_MedicalConditions", model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult MedicalConditions(FollowUp_MedicalConditionsDto model)
        {
            if (!HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue)
            {
                var followUp = FollowUpUtility.CreateFollowUp(model.Id, HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
                HttpContext.Session.SetInt32(SessionContext.FollowUpId, followUp.FollowUpDto.Id);
            }
            var response = FollowUpUtility.AddEditMedicalCondition(model, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetString(SessionContext.OrganizationCode), HttpContext.Session.GetString(SessionContext.UniqueId), HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, _appSettings.DTCOrgCode);
            if (response.FollowUpDto != null)
                HttpContext.Session.SetString(SessionContext.FollowUpPageSeqDone, response.FollowUpDto.PageSeqDone);
            return Json(response.FollowUpDto);
        }

        [Authorize]
        public ActionResult OtherRiskFactors(string viewType, int followupid)
        {
            FU_OtherRisksModel model = new FU_OtherRisksModel();
            int HRAId = 0;
            if (followupid > 0)
            {
                var followup = FollowUpUtility.ReadFollowUp(followupid);
                HRAId = followup.FollowUpDto.UsersinProgram.HRAId.Value;
            }
            else
            {
                followupid = HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value;
                HRAId = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            }
            var response = FollowUpUtility.ReadOtherRisk(followupid, HRAId);
            model.otherRisks = response.otherRisks;
            model.HRA_otherRisks = response.HRA_otherRisks;
            model.GINAQuestion = model.HRA_otherRisks.HRA.Portal.GINAQuestion;
            model.SmokeHistList = ListOptions.SmokingDetails().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.readOnly = FollowUpReadOnly(followupid);
            model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            model.HRAVer = HttpContext.Session.GetInt32(SessionContext.HRAVer);
            ViewBag.viewType = viewType;
            return PartialView("_OtherRiskFactors", model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult OtherRiskFactors(FU_OtherRisksModel model)
        {
            if (!HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue)
            {
                var followUp = FollowUpUtility.CreateFollowUp(model.otherRisks.Id, HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
                HttpContext.Session.SetInt32(SessionContext.FollowUpId, followUp.FollowUpDto.Id);
            }
            var response = FollowUpUtility.AddEditOtherRisks(model.otherRisks, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetString(SessionContext.OrganizationCode), HttpContext.Session.GetString(SessionContext.UniqueId), HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, _appSettings.DTCOrgCode);
            if (response.FollowUpDto != null)
                HttpContext.Session.SetString(SessionContext.FollowUpPageSeqDone, response.FollowUpDto.PageSeqDone);
            return Json(response.FollowUpDto);
        }

        [Authorize]
        public ActionResult HSP(string viewType, int followupid)
        {
            FU_HSAModel model = new FU_HSAModel();
            model.StateOfHealthList = ListOptions.GetStateOfHealthLists().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.ProductivityLossList = ListOptions.GetHealthProblems().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            if (!(followupid > 0))
                followupid = HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value;
            if (followupid > 0)
                model.HealthCondition = FollowUpUtility.ReadHealthConditions(followupid).FollowUp_HealthConditionsDto;
            model.readOnly = FollowUpReadOnly(followupid);
            ViewBag.viewType = viewType;
            model.HRAVer = HttpContext.Session.GetInt32(SessionContext.HRAVer);
            return PartialView("_HSP", model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult HSP(FU_HSAModel model)
        {
            if (!HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue)
            {
                var followUp = FollowUpUtility.CreateFollowUp(model.HealthCondition.Id, HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
                HttpContext.Session.SetInt32(SessionContext.FollowUpId, followUp.FollowUpDto.Id);
            }
            var response = FollowUpUtility.AddEditHealthConditions(model.HealthCondition, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetString(SessionContext.OrganizationCode), HttpContext.Session.GetString(SessionContext.UniqueId), HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, _appSettings.DTCOrgCode);
            if (response.FollowUpDto != null)
                HttpContext.Session.SetString(SessionContext.FollowUpPageSeqDone, response.FollowUpDto.PageSeqDone);
            return Json(response.FollowUpDto);
        }

        [Authorize]
        public ActionResult HealthNumbers(string viewType, int followupid)
        {
            FU_HealthNumbersModel model = new FU_HealthNumbersModel();
            bool hasLabIntegration = false;
            DateTime followUpEligDate = DateTime.MinValue;
            DateTime followUpDate = DateTime.MinValue;
            model.LabOrder = "";
            IList<LabDto> labs = null;
            model.IsAdmin = CommonUtility.HasAdminRole(User.RoleCode());
            model.Measurements = CommonUtility.ListMeasurements((int)Unit.Imperial).Measurements;
            model.MetricMeasurements = CommonUtility.ListMeasurements((int)Unit.Metric).Measurements;
            model.LabList = LabUtility.GetDiagnosticLabs().Select(x => new SelectListItem { Text = x.LabName, Value = x.Id.ToString() });
            model.LabId = 0;
            if (followupid == 0)
                followupid = HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value;

            model.readOnly = FollowUpReadOnly(followupid);

            var followUpResponse = FollowUpUtility.ReadFollowUp(followupid).FollowUpDto;
            var followUps = FollowUpUtility.GetAllFollowUps(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).FollowUps;
            var currentCampaignFollowUps = followUps.Where(x => x.UsersinProgram.ProgramsinPortal.PortalId == Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value) && x.CompleteDate.HasValue).OrderByDescending(x => x.StartDate).ToList();
            if (currentCampaignFollowUps.Count() > 1)
            {
                var prevFollowUp = currentCampaignFollowUps.Where(x => followupid != x.Id && x.Id < followupid).OrderByDescending(x => x.CompleteDate).FirstOrDefault();
                if (prevFollowUp != null)
                {
                    var healthNumber = FollowUpUtility.ReadFUHealthNumber(prevFollowUp.Id).FollowUp_HealthNumbersDto;
                    model.PreviousHealthNumbers = new HealthNumbersDto();
                    model.PreviousHealthNumbers.Weight = healthNumber.Weight;
                    model.PreviousHealthNumbers.Height = healthNumber.Height;
                    model.PreviousHealthNumbers.Waist = healthNumber.Waist;
                    model.PreviousHealthNumbers.DBP = healthNumber.DBP;
                    model.PreviousHealthNumbers.SBP = healthNumber.SBP;
                    model.PreviousHealthNumbers.HeightCM = ListOptions.ToMetric(model.PreviousHealthNumbers.Height.Value, BioLookup.Height, model.MetricMeasurements);
                    model.PreviousHealthNumbers.TotalChol = healthNumber.TotalChol;
                    model.PreviousHealthNumbers.LDL = healthNumber.LDL;
                    model.PreviousHealthNumbers.HDL = healthNumber.HDL;
                    model.PreviousHealthNumbers.Trig = healthNumber.Trig;
                    model.PreviousHealthNumbers.A1C = healthNumber.A1C;
                    model.PreviousHealthNumbers.Glucose = healthNumber.Glucose;
                }
            }
            else
            {
                int HRAId;
                if (followUpResponse.UsersinProgram != null)
                    HRAId = followUpResponse.UsersinProgram.HRAId.Value;
                else
                    HRAId = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;

                if (HRAId != 0)
                    model.PreviousHealthNumbers = HRAUtility.ReadHRA(HRAId).hra.HealthNumbers;

                if (model.PreviousHealthNumbers != null && model.PreviousHealthNumbers.Height.HasValue)
                    model.PreviousHealthNumbers.HeightCM = ListOptions.ToMetric(model.PreviousHealthNumbers.Height.Value, BioLookup.Height, model.MetricMeasurements);
            }

            if (HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue && followupid == HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value)
            {
                var portalFollowUps = PortalUtility.GetPortalFollowUps(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, HttpContext.Session.GetInt32(SessionContext.ProgramType)).portalFollowUps;
                if (portalFollowUps != null && portalFollowUps.Count > 0)
                {
                    var portalFollowUp = portalFollowUps.Skip(followUpResponse.UsersinProgram.AssignedFollowUp - 1).Take(1).FirstOrDefault();
                    if (portalFollowUp != null)
                    {
                        hasLabIntegration = portalFollowUp.LabIntegration;
                        if (hasLabIntegration || CommonUtility.IsIntegratedWithCaptiva(HttpContext.Session.GetInt32(SessionContext.IntegrationWith)))
                        {
                            if (followUpResponse.CompleteDate.HasValue)
                            {
                                followUpDate = followUpResponse.CompleteDate.Value;
                                followUpEligDate = followUpResponse.CompleteDate.Value.AddDays(-HttpContext.Session.GetInt32(SessionContext.FollowUpValidity).Value);
                            }
                            else
                            {
                                followUpDate = DateTime.UtcNow;
                                followUpEligDate = DateTime.UtcNow.AddDays(-HttpContext.Session.GetInt32(SessionContext.FollowUpValidity).Value);
                            }

                        }
                    }
                }
                if (hasLabIntegration || CommonUtility.IsIntegratedWithCaptiva(HttpContext.Session.GetInt32(SessionContext.IntegrationWith)))
                {
                    model.LabIntegration = hasLabIntegration;
                    labs = LabUtility.ListLabs(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).Labs;
                    bool needA1C = false;
                    bool lateLab = false;
                    if (CommonUtility.IsIntegratedWithLMC(HttpContext.Session.GetInt32(SessionContext.IntegrationWith)))
                        needA1C = true;
                    var labwork = labs.Where(x => x.BloodTestDate > followUpEligDate && (!CommonUtility.IsIntegratedWithCaptiva(HttpContext.Session.GetInt32(SessionContext.IntegrationWith)) || (HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp).HasValue
                                    && ((Convert.ToByte(HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp)) == 1 && model.PreviousHealthNumbers != null && model.PreviousHealthNumbers.HRA.CompleteDate.Value.AddDays(28) < x.BloodTestDate.Value)
                                        || (Convert.ToByte(HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp)) == 2 && model.PreviousHealthNumbers != null && followUpDate != DateTime.MinValue && followUpDate.AddDays(-168) < x.BloodTestDate.Value))))
                                    && (!needA1C || x.A1C.HasValue)).OrderBy(x => x.Id).FirstOrDefault();
                    if (labwork != null)
                    {
                        if (followUpResponse.CompleteDate.HasValue && labwork.BloodTestDate > followUpResponse.CompleteDate.Value.AddDays(HttpContext.Session.GetInt32(SessionContext.FollowUpValidity).Value))
                        {
                            lateLab = true;
                            labwork = null;
                        }
                    }
                    if (labwork == null)
                        labwork = labs.Where(x => x.CreatedOn >= followUpEligDate && !x.BloodTestDate.HasValue).OrderBy(x => x.Id).FirstOrDefault();
                    if (labwork != null)
                    {
                        if (labwork.LabSelection.HasValue)
                        {
                            model.LabSelection = labwork.LabSelection.Value;
                            model.LabId = labwork.Id;
                            model.DiagnosticLabId = labwork.DiagnosticLabId;
                            model.LabCompleteDate = labwork.DateCompleted;
                            model.LabBloodTestDate = labwork.BloodTestDate;
                            model.LabOrder = labwork.LabOrder;
                            model.OrderNo = labwork.OrderNo;
                        }
                    }
                    else
                    {
                        if (lateLab)
                            model.LabIntegration = hasLabIntegration = false;
                    }
                }
            }

            model.HealthNumbers = FollowUpUtility.ReadFUHealthNumber(followupid).FollowUp_HealthNumbersDto;
            if (model.HealthNumbers != null)
            {
                model.HealthNumbersInMetric = new FollowUp_HealthNumbersDto();
                if (model.HealthNumbers.Height.HasValue)
                {
                    var height = CommonUtility.ToFeetInches(model.HealthNumbers.Height.Value);
                    model.HeightFt = height.Key;
                    model.HeightInch = (float)Math.Round(height.Value, 2);
                    model.HealthNumbers = CommonUtility.Roundoff(model.HealthNumbers, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                }
                model.HealthNumbersInMetric.HeightCM = model.HealthNumbers.Height.HasValue ? ListOptions.ToMetric(model.HealthNumbers.Height.Value, BioLookup.Height, model.MetricMeasurements) : model.HealthNumbers.Height;
                model.HealthNumbersInMetric.Glucose = model.HealthNumbers.Glucose.HasValue ? ListOptions.ToMetric(model.HealthNumbers.Glucose.Value, BioLookup.Glucose, model.MetricMeasurements) : model.HealthNumbersInMetric.Glucose;
                model.HealthNumbersInMetric.Weight = model.HealthNumbers.Weight.HasValue ? ListOptions.ToMetric(model.HealthNumbers.Weight.Value, BioLookup.Weight, model.MetricMeasurements) : model.HealthNumbersInMetric.Weight;
                model.HealthNumbersInMetric.Waist = model.HealthNumbers.Waist.HasValue ? ListOptions.ToMetric(model.HealthNumbers.Waist.Value, BioLookup.Waist, model.MetricMeasurements) : model.HealthNumbersInMetric.Waist;
                model.HealthNumbersInMetric.TotalChol = model.HealthNumbers.TotalChol.HasValue ? ListOptions.ToMetric(model.HealthNumbers.TotalChol.Value, BioLookup.Cholesterol, model.MetricMeasurements) : model.HealthNumbersInMetric.TotalChol;
                model.HealthNumbersInMetric.Trig = model.HealthNumbers.Trig.HasValue ? ListOptions.ToMetric(model.HealthNumbers.Trig.Value, BioLookup.Triglycerides, model.MetricMeasurements) : model.HealthNumbersInMetric.Trig;
                model.HealthNumbersInMetric.HDL = model.HealthNumbers.HDL.HasValue ? ListOptions.ToMetric(model.HealthNumbers.HDL.Value, BioLookup.HDL, model.MetricMeasurements) : model.HealthNumbersInMetric.HDL;
                model.HealthNumbersInMetric.LDL = model.HealthNumbers.LDL.HasValue ? ListOptions.ToMetric(model.HealthNumbers.LDL.Value, BioLookup.LDL, model.MetricMeasurements) : model.HealthNumbersInMetric.LDL;

                if (!CommonUtility.HasAdminRole(User.RoleCode()) && model.HealthNumbers.LabId.HasValue)
                    model.readOnlyLab = true;

                if (model.PreviousHealthNumbers == null)
                    model.PreviousHealthNumbers = new HealthNumbersDto();

                if (HRAUtility.hasCompletedHealthNumbers(null, null, model.HealthNumbers) || ((CommonUtility.IsIntegratedWithLMC(HttpContext.Session.GetInt32(SessionContext.IntegrationWith)) || CommonUtility.IsIntegratedWithCaptiva(HttpContext.Session.GetInt32(SessionContext.IntegrationWith))) && model.HealthNumbers.BloodTestDate.HasValue))
                    return PartialView("_HealthNumbers", model);
            }

            if ((hasLabIntegration || CommonUtility.IsIntegratedWithCaptiva(HttpContext.Session.GetInt32(SessionContext.IntegrationWith))) && !model.readOnly && HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue && followupid == HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value)
            {
                var lab = labs.Where(x => x.Id == model.LabId).FirstOrDefault();
                if (lab != null && lab.BloodTestDate.HasValue && (HRAUtility.hasCompletedHealthNumbers(null, lab) || CommonUtility.IsIntegratedWithLMC(HttpContext.Session.GetInt32(SessionContext.IntegrationWith)) || CommonUtility.IsIntegratedWithCaptiva(HttpContext.Session.GetInt32(SessionContext.IntegrationWith))))
                {
                    model.HealthNumbers = new FollowUp_HealthNumbersDto();
                    model.HealthNumbers.BPArm = lab.BPArm;
                    model.HealthNumbers.SBP = lab.SBP;
                    model.HealthNumbers.DBP = lab.DBP;
                    model.HealthNumbers.DidYouFast = lab.DidYouFast;
                    model.HealthNumbers.BloodTestDateShort = lab.BloodTestDate.HasValue ? lab.BloodTestDate.Value.ToShortDateString() : "";
                    model.HealthNumbers.TotalChol = lab.TotalChol;
                    model.HealthNumbers.Trig = lab.Trig;
                    model.HealthNumbers.HDL = lab.HDL;
                    model.HealthNumbers.LDL = lab.LDL;
                    model.HealthNumbers.Glucose = lab.Glucose;
                    model.HealthNumbers.A1C = lab.A1C;
                    model.HealthNumbers.Weight = lab.Weight;
                    model.HealthNumbers.Height = lab.Height;
                    model.HealthNumbers.Waist = lab.Waist;
                    if (model.HealthNumbers.Height.HasValue)
                    {
                        var height = CommonUtility.ToFeetInches(model.HealthNumbers.Height.Value);
                        model.HeightFt = height.Key;
                        model.HeightInch = (float)Math.Round(height.Value, 2);
                        model.HealthNumbers = CommonUtility.Roundoff(model.HealthNumbers, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                    }
                    if (model.HealthNumbersInMetric == null)
                        model.HealthNumbersInMetric = new FollowUp_HealthNumbersDto();
                    model.HealthNumbersInMetric.HeightCM = model.HealthNumbers.Height.HasValue ? ListOptions.ToMetric(model.HealthNumbers.Height.Value, BioLookup.Height, model.MetricMeasurements) : model.HealthNumbers.Height;
                    model.HealthNumbersInMetric.Glucose = model.HealthNumbers.Glucose.HasValue ? ListOptions.ToMetric(model.HealthNumbers.Glucose.Value, BioLookup.Glucose, model.MetricMeasurements) : model.HealthNumbersInMetric.Glucose;
                    model.HealthNumbersInMetric.Weight = model.HealthNumbers.Weight.HasValue ? ListOptions.ToMetric(model.HealthNumbers.Weight.Value, BioLookup.Weight, model.MetricMeasurements) : model.HealthNumbersInMetric.Weight;
                    model.HealthNumbersInMetric.Waist = model.HealthNumbers.Waist.HasValue ? ListOptions.ToMetric(model.HealthNumbers.Waist.Value, BioLookup.Waist, model.MetricMeasurements) : model.HealthNumbersInMetric.Waist;
                    model.HealthNumbersInMetric.TotalChol = model.HealthNumbers.TotalChol.HasValue ? ListOptions.ToMetric(model.HealthNumbers.TotalChol.Value, BioLookup.Cholesterol, model.MetricMeasurements) : model.HealthNumbersInMetric.TotalChol;
                    model.HealthNumbersInMetric.Trig = model.HealthNumbers.Trig.HasValue ? ListOptions.ToMetric(model.HealthNumbers.Trig.Value, BioLookup.Triglycerides, model.MetricMeasurements) : model.HealthNumbersInMetric.Trig;
                    model.HealthNumbersInMetric.HDL = model.HealthNumbers.HDL.HasValue ? ListOptions.ToMetric(model.HealthNumbers.HDL.Value, BioLookup.HDL, model.MetricMeasurements) : model.HealthNumbersInMetric.HDL;
                    model.HealthNumbersInMetric.LDL = model.HealthNumbers.LDL.HasValue ? ListOptions.ToMetric(model.HealthNumbers.LDL.Value, BioLookup.LDL, model.MetricMeasurements) : model.HealthNumbersInMetric.LDL;
                    model.readOnlyLab = true;
                }
            }

            if (model.PreviousHealthNumbers != null && (model.HealthNumbers == null || !model.HealthNumbers.Height.HasValue))
            {
                if (model.HealthNumbers == null)
                    model.HealthNumbers = new FollowUp_HealthNumbersDto();
                model.HealthNumbers.Height = model.PreviousHealthNumbers.Height;
                if (model.HealthNumbersInMetric == null)
                    model.HealthNumbersInMetric = new FollowUp_HealthNumbersDto();
                model.HealthNumbersInMetric.HeightCM = ListOptions.ToMetric(model.HealthNumbers.Height.Value, BioLookup.Height, model.MetricMeasurements);
                model.InitialFollowUp = true;
                model.IncompleteFollowUp = true;
            }

            if (model.PreviousHealthNumbers == null)
                model.PreviousHealthNumbers = new HealthNumbersDto();
            ViewBag.viewType = viewType;
            return PartialView("_HealthNumbers", model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult HealthNumbers(FU_HealthNumbersModel model, bool bloodwork)
        {
            bool isValid = true;
            if (bloodwork && model.HealthNumbers.BloodTestDate.HasValue)
            {
                var validityResponse = FollowUpUtility.CheckFollowupValidity(HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value, model.HealthNumbers.BloodTestDate.Value);
                if (validityResponse.isValid && validityResponse.followUpId == HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value)
                    isValid = true;
                else
                    isValid = false;
            }
            if (!bloodwork)
            {
                model.HealthNumbers.Height = (model.HeightFt * 12);
                if (model.HeightInch.HasValue && model.HeightInch.Value > 0)
                    model.HealthNumbers.Height = model.HeightInch + model.HealthNumbers.Height;
            }
            if (isValid)
            {
                if (!HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue)
                {
                    var followUp = FollowUpUtility.CreateFollowUp(model.HealthNumbers.Id, HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
                    HttpContext.Session.SetInt32(SessionContext.FollowUpId, followUp.FollowUpDto.Id);
                }
                var response = FollowUpUtility.AddEditHealthNumbers(model.HealthNumbers, bloodwork, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetString(SessionContext.OrganizationCode), HttpContext.Session.GetString(SessionContext.UniqueId), HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, _appSettings.DTCOrgCode);
                if (response.FollowUpDto != null)
                    HttpContext.Session.SetString(SessionContext.FollowUpPageSeqDone, response.FollowUpDto.PageSeqDone);
                return Json(response.FollowUpDto);
            }
            return Json("");
        }

        [Authorize]
        public ActionResult FollowUpReport(int followupid)
        {
            FollowUpModuleModel model = new FollowUpModuleModel();
            if (!(followupid > 0))
                followupid = HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value;
            model.FollowUpSummary = GetFollowUpReport(followupid);
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return PartialView("_FollowUpReport", model.FollowUpSummary);
        }

        [Authorize]
        public ActionResult FollowUpModule(int followupid)
        {
            FollowUpModuleModel model = new FollowUpModuleModel();
            model.FollowUpSummary = GetFollowUpReport(followupid);
            model.FollowUpId = followupid;
            return PartialView("_FollowUpModule", model);
        }

        public FollowupReport GetFollowUpReport(int followupid)
        {
            FollowupReport model = new FollowupReport();
            if (followupid > 0)
            {
                var response = FollowUpUtility.ReadFollowUp(followupid);
                if (response.FollowUpDto.CompleteDate.HasValue)
                {
                    string timeZone = !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : User.TimeZone();
                    model = ReportUtility.GetFollowupReportDashboard(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, followupid, HttpContext.Session.GetInt32(SessionContext.HRAVer).Value, HttpContext.Session.GetInt32(SessionContext.Unit).Value, timeZone);
                    if ((HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue && followupid != HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value) || !HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue)
                        model.IsSurveyCompleted = true;
                    else
                        model.IsSurveyCompleted = ParticipantUtility.GetSurveyCompletedStatus(response.FollowUpDto.UsersinProgramsId).PartiallyCompleted;
                    model.hasSummary = true;
                }
            }
            else
                model.hasSummary = false;
            return model;
        }

        public JsonResult GetFollowUpCompletionPercent()
        {
            int percent = FollowUpUtility.GetFolowUpCompletionPercent(HttpContext.Session.GetString(SessionContext.FollowUpPageSeqDone) != null ? HttpContext.Session.GetString(SessionContext.FollowUpPageSeqDone) : "");
            return Json(percent);
        }

    }
}