using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace InterventWebApp
{
    public class HRAController : BaseController
    {
        private readonly AppSettings _appSettings;

        public HRAController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        [Authorize]
        public ActionResult HRADashboard()
        {
            if (HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value == null)
                return RedirectToAction("NotAuthorized", "Account");
            HRAModel model = new HRAModel();

            if (HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue)
            {
                var response = HRAUtility.ReadHRA(HttpContext.Session.GetInt32(SessionContext.HRAId).Value);
                HttpContext.Session.SetString(SessionContext.HRAPageSeqDone, model.HAPageSeqDone = response.hra.HAPageSeqDone);
                HttpContext.Session.SetString(SessionContext.HRACompleteDate, (model.CompleteDate = response.hra.CompleteDate).ToString());
                model.HRAId = response.hra.Id;
            }
            else
            {
                var partResponse = ParticipantUtility.ReadUserParticipation(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
                if (!(partResponse.user.Complete.HasValue && partResponse.user.Complete.Value))
                    return RedirectToAction("InitialDashboard", "Participant");
            }
            model.AdminId = HttpContext.Session.GetInt32(SessionContext.AdminId);
            model.HRAValidity = HttpContext.Session.GetInt32(SessionContext.HRAValidity).ToString();
            model.OrgContactEmail = HttpContext.Session.GetString(SessionContext.OrgContactEmail);
            model.HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            model.HRAPageSeq = HttpContext.Session.GetString(SessionContext.HRAPageSeq);
            model.hraPercent = HRAUtility.GetHRACompletionPercent(model.HRAPageSeq, model.HAPageSeqDone);
            model.readOnly = ReadOnly(null);
            model.isSouthUniversity = HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue && HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value == _appSettings.SouthUniversityOrgId;
            return View(model);
        }

        public bool ReadOnly(int? hraId)
        {
            var readOnly = false;
            if (!Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal)))
                return true;
            if ((hraId.HasValue && HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue && (hraId > 0 && hraId.Value != HttpContext.Session.GetInt32(SessionContext.HRAId).Value)) || (!HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue && hraId > 0))
                return true;
            if (HttpContext.Session.GetInt32(SessionContext.HRAValidity).HasValue && !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.HRACompleteDate))
                && DateTime.UtcNow > Convert.ToDateTime(HttpContext.Session.GetString(SessionContext.HRACompleteDate)).AddDays(HttpContext.Session.GetInt32(SessionContext.HRAValidity).Value) && !CommonUtility.IsSuperAdmin(User.RoleCode()))
                readOnly = true;
            if (HttpContext.Session.GetInt32(SessionContext.HasHRA) != (int)HRAStatus.Optional && !CommonUtility.IsSuperAdmin(User.RoleCode()) && HttpContext.Session.GetInt32(SessionContext.UserinProgramId).HasValue)
            {
                var userinProgram = ProgramUtility.GetUserinProgramDetails(HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value);
                if ((userinProgram.StartDate.HasValue && DateTime.UtcNow > userinProgram.StartDate.Value.AddDays(30)))
                    readOnly = true;
                else
                    readOnly = false;
            }
            return readOnly;
        }

        public bool updateHRASessions(HRADto hra)
        {
            if (hra.CompleteDate.HasValue)
                HttpContext.Session.SetString(SessionContext.HRACompleteDate, hra.CompleteDate.Value.ToString());
            if (hra != null && !string.IsNullOrEmpty(hra.HAPageSeqDone))
                HttpContext.Session.SetString(SessionContext.HRAPageSeqDone, hra.HAPageSeqDone.ToString());
            return true;
        }

        [Authorize]
        public ActionResult MedicalConditions(bool adminview, int hraid)
        {
            MedicalConditionModel model = new MedicalConditionModel();
            if (!(hraid > 0) && HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue)
                hraid = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            var HApageSeq = HttpContext.Session.GetString(SessionContext.HRAPageSeq);
            model.readOnly = ReadOnly(hraid);
            if (hraid > 0)
            {
                model.AutoImmune = HRAUtility.ReadHRA(hraid).hra.Portal.AutoImmune;
                var response = ReportUtility.ReadHRAReport(hraid);
                if (response.hra != null && response.hra.MedicalCondition != null)
                {
                    MedicalConditionsDto medicalConditions = new MedicalConditionsDto();
                    medicalConditions = response.hra.MedicalCondition;
                    model.medicalConditions = medicalConditions;
                    if (model.IsLocked == true)
                        model.Age = response.hra.Age;
                    else
                        model.Age = CommonUtility.GetAge(response.hra.User.DOB.Value);
                    model.Gender = response.hra.User.Gender;
                    model.hracompletion = HRAUtility.GetHRACompletionPercent(HApageSeq, response.hra.HAPageSeqDone);
                    if (response.hra.HAPageSeqDone.Contains("IN"))
                        model.hasInterest = true;
                }
            }
            else
            {
                model.AutoImmune = PortalUtility.ReadPortal(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value)).portal.AutoImmune;
                model.Age = CommonUtility.GetAge(Convert.ToDateTime(HttpContext.Session.GetString(SessionContext.DOB)));
                model.Gender = (byte)HttpContext.Session.GetInt32(SessionContext.Gender);
                model.hracompletion = 0;
            }
            if (adminview)
            {
                model.IsAdminView = true;
            }
            else
            {
                model.IsAdminView = false;
            }
            if (!HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue && model.medicalConditions == null)
            {
                //Check values in last year
                var recentHRA = ParticipantUtility.GetPrevYearStatus(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : "", User.TimeZone());
                if (recentHRA.hra != null && recentHRA.hra.MedicalCondition != null)
                {
                    MedicalConditionsDto medicalConditions = new MedicalConditionsDto();
                    medicalConditions.Stroke = (recentHRA.hra.MedicalCondition.Stroke.HasValue && recentHRA.hra.MedicalCondition.Stroke == 1) ? recentHRA.hra.MedicalCondition.Stroke : null;
                    medicalConditions.HeartAttack = (recentHRA.hra.MedicalCondition.HeartAttack.HasValue && recentHRA.hra.MedicalCondition.HeartAttack == 1) ? recentHRA.hra.MedicalCondition.HeartAttack : null;
                    medicalConditions.Angina = (recentHRA.hra.MedicalCondition.Angina.HasValue && recentHRA.hra.MedicalCondition.Angina == 1) ? recentHRA.hra.MedicalCondition.Angina : null;
                    medicalConditions.ToldBlock = (recentHRA.hra.MedicalCondition.ToldBlock.HasValue && recentHRA.hra.MedicalCondition.ToldBlock == 1) ? recentHRA.hra.MedicalCondition.ToldBlock : null;
                    medicalConditions.ToldHeartBlock = (recentHRA.hra.MedicalCondition.ToldHeartBlock.HasValue && recentHRA.hra.MedicalCondition.ToldHeartBlock == 1) ? recentHRA.hra.MedicalCondition.ToldHeartBlock : null;
                    medicalConditions.ToldHighBP = (recentHRA.hra.MedicalCondition.ToldHighBP.HasValue && recentHRA.hra.MedicalCondition.ToldHighBP == 1) ? recentHRA.hra.MedicalCondition.ToldHighBP : null;
                    medicalConditions.ToldHighChol = (recentHRA.hra.MedicalCondition.ToldHighChol.HasValue && recentHRA.hra.MedicalCondition.ToldHighChol == 1) ? recentHRA.hra.MedicalCondition.ToldHighChol : null;
                    medicalConditions.ToldDiabetes = (recentHRA.hra.MedicalCondition.ToldDiabetes.HasValue && recentHRA.hra.MedicalCondition.ToldDiabetes == 1) ? recentHRA.hra.MedicalCondition.ToldDiabetes : null;
                    medicalConditions.ToldAsthma = (recentHRA.hra.MedicalCondition.ToldAsthma.HasValue && recentHRA.hra.MedicalCondition.ToldAsthma == 1) ? recentHRA.hra.MedicalCondition.ToldAsthma : null;
                    medicalConditions.ToldBronchitis = (recentHRA.hra.MedicalCondition.ToldBronchitis.HasValue && recentHRA.hra.MedicalCondition.ToldBronchitis == 1) ? recentHRA.hra.MedicalCondition.ToldBronchitis : null;
                    medicalConditions.ToldCancer = (recentHRA.hra.MedicalCondition.ToldCancer.HasValue && recentHRA.hra.MedicalCondition.ToldCancer == 1) ? recentHRA.hra.MedicalCondition.ToldCancer : null;
                    medicalConditions.ToldKidneyDisease = (recentHRA.hra.MedicalCondition.ToldKidneyDisease.HasValue && recentHRA.hra.MedicalCondition.ToldKidneyDisease == 1) ? recentHRA.hra.MedicalCondition.ToldKidneyDisease : null;
                    medicalConditions.ToldPolycyst = (recentHRA.hra.MedicalCondition.ToldPolycyst.HasValue && recentHRA.hra.MedicalCondition.ToldPolycyst == 1) ? recentHRA.hra.MedicalCondition.ToldPolycyst : null;
                    medicalConditions.ToldGestDiab = (recentHRA.hra.MedicalCondition.ToldGestDiab.HasValue && recentHRA.hra.MedicalCondition.ToldGestDiab == 1) ? recentHRA.hra.MedicalCondition.ToldGestDiab : null;
                    model.medicalConditions = medicalConditions;
                }
                if (model.medicalConditions == null || model.medicalConditions.ToldDiabetes != 1)
                {
                    if (model.medicalConditions == null)
                        model.medicalConditions = new MedicalConditionsDto();
                    var isDiabetic = HasDiabetes();
                    if (isDiabetic)
                        model.medicalConditions.ToldDiabetes = 1;
                }
            }
            model.userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            model.participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.AdminId = HttpContext.Session.GetInt32(SessionContext.AdminId);
            model.ParticipantEmail = HttpContext.Session.GetString(SessionContext.ParticipantEmail);
            model.MailScoreCard = HttpContext.Session.GetString(SessionContext.MailScoreCard) == "true";
            model.ShowPostmenopausal = HttpContext.Session.GetString(SessionContext.ShowPostmenopausal) == "true";

            return PartialView("_MedicalConditions", model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult MedicalConditions(MedicalConditionModel model)
        {
            model.userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            model.participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.hraId = HttpContext.Session.GetInt32(SessionContext.HRAId);
            model.participantPortalId = HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId);
            model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            model.userinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId);
            model.hraCompleteDate = !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.HRACompleteDate)) ? Convert.ToDateTime(HttpContext.Session.GetString(SessionContext.HRACompleteDate)) : null;
            model.hraPageSeq = HttpContext.Session.GetString(SessionContext.HRAPageSeq);
            model.hraPageSeqDone = HttpContext.Session.GetString(SessionContext.HRAPageSeqDone);
            model.systemAdminId = _appSettings.SystemAdminId;
			model.DTCOrgCode = _appSettings.DTCOrgCode;
			var response = HRAUtility.AddEditMedicalCondition(model);
            updateHRASessions(response);
            return Json("success");
        }

        [Authorize]
        public ActionResult OtherRiskFactors(int hraid)
        {
            OtherRisksModel model = new OtherRisksModel();
            if (!(hraid > 0) && HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue)
                hraid = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            model.readOnly = ReadOnly(hraid);
            model.SmokeHistList = ListOptions.SmokingDetails().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            if (HttpContext.Session.GetInt32(SessionContext.HRAVer).HasValue && HttpContext.Session.GetInt32(SessionContext.HRAVer).Value == (int)HRAVersions.ActivateVersion)
            {
                model.ExerciseFrequencyList = ListOptions.GetExercisefrequency().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
                model.ExerciseIntensityList = ListOptions.GetExerciseIntensity().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
                model.ExerciseDurationList = ListOptions.GetExerciseMinutes().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            }
            //var id = GetHRAId(hraid);
            if (hraid > 0)
            {
                var response = ReportUtility.ReadHRAReport(hraid);
                model.GINAQuestion = response.hra.Portal.GINAQuestion;
                OtherRiskFactorsDto otherRisks = new OtherRiskFactorsDto();
                if (response.hra != null && response.hra.OtherRiskFactors != null)
                {
                    otherRisks = response.hra.OtherRiskFactors;
                    model.otherRisks = otherRisks;
                    if (response.hra.Interest != null && response.hra.Interest.QuitSmokeProg == null)
                        model.smokingProgram = true;
                    else
                        model.smokingProgram = false;
                }
                //Check with most recent hra
                var recentHRA = ParticipantUtility.GetPrevYearStatus(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : "", User.TimeZone());
                if (recentHRA.hra != null && recentHRA.hra.OtherRiskFactors != null && response.hra != null && response.hra.OtherRiskFactors == null)
                {
                    otherRisks.HeartHist = (recentHRA.hra.OtherRiskFactors.HeartHist.HasValue && recentHRA.hra.OtherRiskFactors.HeartHist == 1) ? recentHRA.hra.OtherRiskFactors.HeartHist : null;
                    otherRisks.CancerHist = (recentHRA.hra.OtherRiskFactors.CancerHist.HasValue && recentHRA.hra.OtherRiskFactors.CancerHist == 1) ? recentHRA.hra.OtherRiskFactors.CancerHist : null;
                    otherRisks.DiabetesHist = (recentHRA.hra.OtherRiskFactors.DiabetesHist.HasValue && recentHRA.hra.OtherRiskFactors.DiabetesHist == 1) ? recentHRA.hra.OtherRiskFactors.DiabetesHist : null;
                    otherRisks.SleepApnea = (recentHRA.hra.OtherRiskFactors.SleepApnea.HasValue && recentHRA.hra.OtherRiskFactors.SleepApnea == 1) ? recentHRA.hra.OtherRiskFactors.SleepApnea : null;
                }
                if (recentHRA.hra != null && recentHRA.hra.OtherRiskFactors != null)
                {
                    model.pastHRA = true;
                }
                model.otherRisks = otherRisks;
            }
            model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            model.HRAVer = HttpContext.Session.GetInt32(SessionContext.HRAVer);
            return PartialView("_OtherRiskFactors", model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult OtherRiskFactors(OtherRisksModel model)
        {
            model.userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            model.participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.hraId = HttpContext.Session.GetInt32(SessionContext.HRAId);
            model.participantPortalId = HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId);
            model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            model.userinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId);
            model.hraCompleteDate = !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.HRACompleteDate)) ? Convert.ToDateTime(HttpContext.Session.GetString(SessionContext.HRACompleteDate)) : null;
            model.hraPageSeq = HttpContext.Session.GetString(SessionContext.HRAPageSeq);
            model.hraPageSeqDone = HttpContext.Session.GetString(SessionContext.HRAPageSeqDone);
            model.systemAdminId = _appSettings.SystemAdminId;
            model.DTCOrgCode = _appSettings.DTCOrgCode;

            var response = HRAUtility.AddEditOtherRisks(model);
            updateHRASessions(response);

            return Json("success");
        }

        [Authorize]
        public ActionResult HSP(int hraid)
        {
            HSPModel model = new HSPModel();
            if (!(hraid > 0) && HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue)
                hraid = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            model.readOnly = ReadOnly(hraid);
            model.StateOfHealthList = ListOptions.GetStateOfHealthLists().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.LifeSatisfactionList = ListOptions.GetLifeSatisfactionList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.JobSatisfactionList = ListOptions.GetJobSatisfactionList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.RelaxMedList = ListOptions.GetRelaxMedList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.WorkMissPersList = ListOptions.GetMissDays().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.WorkMissFamList = ListOptions.GetMissDays().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.EmergRoomVisitList = ListOptions.GetTimes().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.AdmitHospList = ListOptions.GetTimes().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.DrVisitPersList = ListOptions.GetTimes().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.ProductivityLossList = ListOptions.GetHealthProblems().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.HRAVer = HttpContext.Session.GetInt32(SessionContext.HRAVer);
            if (hraid > 0)
            {
                var response = ReportUtility.ReadHRAReport(hraid);
                if (response.hra != null && response.hra.HSP != null)
                {
                    HSPDto hsp = new HSPDto();
                    hsp = response.hra.HSP;
                    model.hsp = hsp;
                }
            }
            return PartialView("_HSP", model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult HSP(HSPModel model)
        {
            model.userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            model.participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.hraId = HttpContext.Session.GetInt32(SessionContext.HRAId);
            model.participantPortalId = HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId);
            model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            model.userinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId);
            model.hraCompleteDate = !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.HRACompleteDate)) ? Convert.ToDateTime(HttpContext.Session.GetString(SessionContext.HRACompleteDate)) : null;
            model.hraPageSeq = HttpContext.Session.GetString(SessionContext.HRAPageSeq);
            model.hraPageSeqDone = HttpContext.Session.GetString(SessionContext.HRAPageSeqDone);
            model.systemAdminId = _appSettings.SystemAdminId;
			model.DTCOrgCode = _appSettings.DTCOrgCode;

			var response = HRAUtility.AddEditHSP(model);
            updateHRASessions(response);
            return Json("success");
        }

        [Authorize]
        public ActionResult ExamsandShots(int hraid)
        {
            ExamsModel model = new ExamsModel();
            if (!(hraid > 0) && HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue)
                hraid = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            model.readOnly = ReadOnly(hraid);
            if (hraid > 0)
            {
                var response = ReportUtility.ReadHRAReport(hraid);
                if (response.hra != null && response.hra.Exams != null)
                {
                    model.PhysicalExamBool = response.hra.Exams.PhysicalExam == 1;
                    model.StoolTestBool = response.hra.Exams.StoolTest == 1;
                    model.SigTestBool = response.hra.Exams.SigTest == 1;
                    model.ColStoolTestBool = response.hra.Exams.ColStoolTest == 1;
                    model.ColTestBool = response.hra.Exams.ColTest == 1;
                    model.PSATestBool = response.hra.Exams.PSATest == 1;
                    model.PapTestBool = response.hra.Exams.PapTest == 1;
                    model.BoneTestBool = response.hra.Exams.BoneTest == 1;
                    model.MammogramBool = response.hra.Exams.Mammogram == 1;
                    model.DentalExamBool = response.hra.Exams.DentalExam == 1;
                    model.BPCheckBool = response.hra.Exams.BPCheck == 1;
                    model.CholTestBool = response.hra.Exams.CholTest == 1;
                    model.GlucoseTestBool = response.hra.Exams.GlucoseTest == 1;
                    model.EyeExamBool = response.hra.Exams.EyeExam == 1;
                    model.NoTestBool = response.hra.Exams.NoTest == 1;
                    model.TetanusShotBool = response.hra.Exams.TetanusShot == 1;
                    model.FluShotBool = response.hra.Exams.FluShot == 1;
                    model.MMRBool = response.hra.Exams.MMR == 1;
                    model.VaricellaBool = response.hra.Exams.Varicella == 1;
                    model.HepBShotBool = response.hra.Exams.HepBShot == 1;
                    model.ShinglesShotBool = response.hra.Exams.ShinglesShot == 1;
                    model.HPVShotBool = response.hra.Exams.HPVShot == 1;
                    model.PneumoniaShotBool = response.hra.Exams.PneumoniaShot == 1;
                    model.NoShotsBool = response.hra.Exams.NoShots == 1;
                    model.Gender = response.hra.User.Gender;
                }
                else
                {
                    model.Gender = (byte)HttpContext.Session.GetInt32(SessionContext.Gender);
                }
                if (response.hra != null && response.hra.Exams == null)
                {
                    //Check values in last year
                    var recentHRA = ParticipantUtility.GetPrevYearStatus(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : "", User.TimeZone());
                    if (recentHRA.hra != null && recentHRA.hra.Exams != null)
                    {
                        model.MMRBool = recentHRA.hra.Exams.MMR.HasValue ? recentHRA.hra.Exams.MMR == 1 : false;
                        model.VaricellaBool = recentHRA.hra.Exams.Varicella.HasValue ? recentHRA.hra.Exams.Varicella == 1 : false;
                        model.HepBShotBool = recentHRA.hra.Exams.HepBShot.HasValue ? recentHRA.hra.Exams.HepBShot == 1 : false;
                        model.ShinglesShotBool = recentHRA.hra.Exams.ShinglesShot.HasValue ? recentHRA.hra.Exams.ShinglesShot == 1 : false;
                        model.HPVShotBool = recentHRA.hra.Exams.HPVShot.HasValue ? recentHRA.hra.Exams.HPVShot == 1 : false;
                        model.PneumoniaShotBool = recentHRA.hra.Exams.PneumoniaShot.HasValue ? recentHRA.hra.Exams.PneumoniaShot == 1 : false;

                    }
                }
            }
            return PartialView("_ExamsandShots", model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult ExamsandShots(ExamsModel model)
        {
            model.userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            model.participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.hraId = HttpContext.Session.GetInt32(SessionContext.HRAId);
            model.participantPortalId = HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId);
            model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            model.userinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId);
            model.hraCompleteDate = !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.HRACompleteDate)) ? Convert.ToDateTime(HttpContext.Session.GetString(SessionContext.HRACompleteDate)) : null;
            model.hraPageSeq = HttpContext.Session.GetString(SessionContext.HRAPageSeq);
            model.hraPageSeqDone = HttpContext.Session.GetString(SessionContext.HRAPageSeqDone);
            model.systemAdminId = _appSettings.SystemAdminId;
			model.DTCOrgCode = _appSettings.DTCOrgCode;

			var response = HRAUtility.AddEditExams(model);
            updateHRASessions(response);
            return Json("success");
        }

        [Authorize]
        public ActionResult Interests(int hraid)
        {
            InterestModel model = new InterestModel();
            if (!(hraid > 0) && HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue)
                hraid = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            model.readOnly = ReadOnly(hraid);
            if (hraid > 0)
            {
                var response = ReportUtility.ReadHRAReport(hraid);
                if (response.hra != null && response.hra.Interest != null)
                {
                    InterestsDto interests = new InterestsDto();
                    interests = response.hra.Interest;
                    model.interests = interests;
                }
                if (response.hra != null && response.hra.OtherRiskFactors != null)
                {
                    var hraOtherRisk = response.hra.OtherRiskFactors;
                    if (hraOtherRisk.SmokeCig == 1 || hraOtherRisk.OtherTobacco == 1 || hraOtherRisk.ECig == 1)
                        model.isSmoker = 1;
                }
                if (response.hra != null && response.hra.MedicalCondition != null)
                {
                    var medicalConditions = response.hra.MedicalCondition;
                    if (medicalConditions.Pregnant == 1)
                        model.isPregnant = 1;
                }
            }
            return PartialView("_Interests", model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult Interests(InterestModel model)
        {
            model.userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            model.participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.hraId = HttpContext.Session.GetInt32(SessionContext.HRAId);
            model.participantPortalId = HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId);
            model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            model.userinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId);
            model.hraCompleteDate = !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.HRACompleteDate)) ? Convert.ToDateTime(HttpContext.Session.GetString(SessionContext.HRACompleteDate)) : null;
            model.hraPageSeq = HttpContext.Session.GetString(SessionContext.HRAPageSeq);
            model.hraPageSeqDone = HttpContext.Session.GetString(SessionContext.HRAPageSeqDone);
            model.systemAdminId = _appSettings.SystemAdminId;
			model.DTCOrgCode = _appSettings.DTCOrgCode;
			var response = HRAUtility.AddEditInterest(model);
            updateHRASessions(response);

            return Json("success");
        }

        [Authorize]
        public ActionResult HealthNumbers(int hraid)
        {
            HealthNumbersModel model = new HealthNumbersModel();
            model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            if (HttpContext.Session.GetString(SessionContext.IsParticipantView) != null)
                model.IsParticipantView = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.IsParticipantView));
            model.OrganizationId = HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value;
            model.southUniversityOrgId = _appSettings.SouthUniversityOrgId;
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.HRAVer = HttpContext.Session.GetInt32(SessionContext.HRAVer);

            if (!(hraid > 0) && HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue)
                hraid = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;

            var portalId = GetParticipantPortalId();
            model.readOnly = ReadOnly(hraid);
            var response = ReportUtility.ReadHRAReport(hraid);
            model.Measurements = CommonUtility.ListMeasurements((int)Unit.Imperial).Measurements;
            model.MetricMeasurements = CommonUtility.ListMeasurements((int)Unit.Metric).Measurements;
            model.LabList = LabUtility.GetDiagnosticLabs().Select(x => new SelectListItem { Text = x.LabName, Value = x.Id.ToString() });
            model.LabOrder = "";
            model.LabIntegration = response.hra.Portal.LabIntegration;
            model.ShowLabDetails = response.hra.Portal.SelfEnterLabs;
            model.UserName = Convert.ToString(HttpContext.Session.GetString(SessionContext.ParticipantName));
            var labs = LabUtility.ListLabs(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).Labs;
            GetEligibilityResponse eligResponse = null;
            CanriskQuestionnaireDto canriskQuestionnaire = null;
            if (HttpContext.Session.GetString(SessionContext.UniqueId) != null)
            {
                eligResponse = ParticipantUtility.GetEligibility(null, HttpContext.Session.GetString(SessionContext.UniqueId), portalId);
                canriskQuestionnaire = ParticipantUtility.GetCanriskResponse(HttpContext.Session.GetString(SessionContext.UniqueId), null);
            }
            if (model.LabIntegration)
            {
                if (CommonUtility.IsIntegratedWithLMC(HttpContext.Session.GetInt32(SessionContext.IntegrationWith)))
                {
                    if (labs.Count() == 0 || (labs.Count() > 0 && !labs.OrderBy(x => x.Id).FirstOrDefault().BloodTestDate.HasValue))
                    {
                        if (eligResponse != null)
                        {
                            if (eligResponse.Eligibility.Lab_Date.HasValue && (eligResponse.Eligibility.Lab_Date.Value > DateTime.UtcNow.Date.AddDays(-60)))
                            {
                                var firstLab = labs.Where(x => x.PortalId == portalId).OrderBy(x => x.Id).FirstOrDefault();
                                AddLabModel labmodel = new AddLabModel();
                                LabDto lab = new LabDto();
                                if (canriskQuestionnaire != null)
                                {
                                    lab.Height = canriskQuestionnaire.Height;
                                    lab.Weight = canriskQuestionnaire.Weight;
                                    lab.Waist = canriskQuestionnaire.Waist;
                                }
                                //update labs table
                                lab.BloodTestDate = eligResponse.Eligibility.Lab_Date;
                                lab.DidYouFast = eligResponse.Eligibility.Lab_DidYouFast;
                                lab.A1C = eligResponse.Eligibility.Lab_A1C;
                                lab.Glucose = eligResponse.Eligibility.Lab_Glucose;
                                lab.LabSelection = (int)LabChoices.DoctorsOffice;
                                if (firstLab != null)
                                    lab.Id = firstLab.Id;
                                else
                                    labmodel.SaveNew = true;
                                labmodel.BloodTestDate = lab.BloodTestDateString;
                                labmodel.Lab = lab;
                                labmodel.fromEligibility = true;
                                if (HttpContext.Session.GetInt32(SessionContext.HRAValidity).HasValue)
                                    labmodel.HRAValidity = HttpContext.Session.GetInt32(SessionContext.HRAValidity);
                                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                                    labmodel.updatedBy = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
                                if (HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue)
                                    labmodel.participantPortalId = HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value;
                                if (HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue)
                                    labmodel.participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                                labmodel.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
                                LabUtility.UpdateLabs(_appSettings.SystemAdminId, labmodel);
                                //get updated labs
                                labs = LabUtility.ListLabs(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).Labs;
                            }
                        }
                    }
                }
                if (labs != null && labs.Count > 0)
                {
                    var labwork = labs.Where(x => x.PortalId == portalId).OrderBy(x => x.Id).FirstOrDefault();
                    if (labwork != null && labwork.LabSelection.HasValue)
                    {
                        model.LabSelection = labwork.LabSelection.Value;
                        model.LabId = labwork.Id;
                        model.LabCompleteDate = labwork.DateCompleted;
                        model.LabBloodTestDate = labwork.BloodTestDate;
                        model.DiagnosticLabId = labwork.DiagnosticLabId;
                        model.LabOrder = labwork.LabOrder;
                        model.OrderNo = labwork.OrderNo;
                    }
                    else
                        model.LabId = 0;
                }
                else
                    model.LabId = 0;
            }
            if (hraid > 0)
            {
                var timeZone = "";
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)))
                    timeZone = HttpContext.Session.GetString(SessionContext.ParticipantTimeZone);
                else
                    timeZone = User.TimeZone();
                var custzone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                model.CACScanQuestion = response.hra.Portal.CACScanQuestion;
                model.usePreviousLabs = response.hra.Portal.usePreviousLabs;
                var validDate = new DateTime();
                if (response.hra.CompleteDate.HasValue)
                {
                    validDate = TimeZoneInfo.ConvertTimeFromUtc(response.hra.CompleteDate.Value.AddYears(-1), custzone) > Convert.ToDateTime(response.hra.Portal.ValidLabs)
                    ? TimeZoneInfo.ConvertTimeFromUtc(response.hra.CompleteDate.Value.AddYears(-1), custzone) : Convert.ToDateTime(response.hra.Portal.ValidLabs);
                }
                else
                {
                    validDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddYears(-1), custzone) > Convert.ToDateTime(response.hra.Portal.ValidLabs)
                    ? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddYears(-1), custzone) : Convert.ToDateTime(response.hra.Portal.ValidLabs);
                }
                if (model.usePreviousLabs)
                {
                    model.ValidLabs = validDate.ToShortDateString();
                }
                var portalIncentives = PortalUtility.GetPortalIncentives(response.hra.PortalId).portalIncentives.Where(x => (x.IncentiveTypeId == (int)IncentiveTypes.HRA_HealthNumbers) && x.IsCompanyIncentive && x.IsActive).ToList();
                if (portalIncentives.Count() > 0)
                {
                    model.incentive = portalIncentives.FirstOrDefault().PointsText;
                }
                if (response.hra != null && response.hra.HealthNumbers != null)
                {
                    HealthNumbersDto healthNumbers = new HealthNumbersDto();
                    healthNumbers = response.hra.HealthNumbers;
                    model.BloodTestDate = response.hra.HealthNumbers.BloodTestDate.HasValue ? response.hra.HealthNumbers.BloodTestDate.Value.ToShortDateString() : null;
                    var Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(Unit.Metric)).Measurements;
                    if (response.hra.HealthNumbers.Height.HasValue)
                    {
                        var height = CommonUtility.ToFeetInches(response.hra.HealthNumbers.Height.Value);
                        model.HeightFt = height.Key;
                        model.HeightInch = (float)Math.Round(height.Value, 2);
                        healthNumbers = CommonUtility.Roundoff(healthNumbers, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                    }
                    model.HealthNumbers = healthNumbers;
                    model.HealthNumbersInMetric = new HealthNumbersDto();
                    model.HealthNumbersInMetric.HeightCM = response.hra.HealthNumbers.Height.HasValue ? ListOptions.ToMetric(response.hra.HealthNumbers.Height.Value, BioLookup.Height, Measurements) : response.hra.HealthNumbers.Height;
                    model.HealthNumbersInMetric.Glucose = response.hra.HealthNumbers.Glucose.HasValue ? ListOptions.ToMetric(response.hra.HealthNumbers.Glucose.Value, BioLookup.Glucose, Measurements) : model.HealthNumbersInMetric.Glucose;
                    model.HealthNumbersInMetric.Weight = response.hra.HealthNumbers.Weight.HasValue ? ListOptions.ToMetric(response.hra.HealthNumbers.Weight.Value, BioLookup.Weight, Measurements) : model.HealthNumbersInMetric.Weight;
                    model.HealthNumbersInMetric.Waist = response.hra.HealthNumbers.Waist.HasValue ? ListOptions.ToMetric(response.hra.HealthNumbers.Waist.Value, BioLookup.Waist, Measurements) : model.HealthNumbersInMetric.Waist;
                    model.HealthNumbersInMetric.TotalChol = response.hra.HealthNumbers.TotalChol.HasValue ? ListOptions.ToMetric(response.hra.HealthNumbers.TotalChol.Value, BioLookup.Cholesterol, Measurements) : model.HealthNumbersInMetric.TotalChol;
                    model.HealthNumbersInMetric.Trig = response.hra.HealthNumbers.Trig.HasValue ? ListOptions.ToMetric(response.hra.HealthNumbers.Trig.Value, BioLookup.Triglycerides, Measurements) : model.HealthNumbersInMetric.Trig;
                    model.HealthNumbersInMetric.HDL = response.hra.HealthNumbers.HDL.HasValue ? ListOptions.ToMetric(response.hra.HealthNumbers.HDL.Value, BioLookup.HDL, Measurements) : model.HealthNumbersInMetric.HDL;
                    model.HealthNumbersInMetric.LDL = response.hra.HealthNumbers.LDL.HasValue ? ListOptions.ToMetric(response.hra.HealthNumbers.LDL.Value, BioLookup.LDL, Measurements) : model.HealthNumbersInMetric.LDL;
                    model.HealthNumbersInMetric.DesiredWeight = response.hra.HealthNumbers.DesiredWeight.HasValue ? ListOptions.ToMetric(response.hra.HealthNumbers.DesiredWeight.Value, BioLookup.Weight, Measurements) : model.HealthNumbersInMetric.DesiredWeight;
                    if (!CommonUtility.HasAdminRole(User.RoleCode()) && (((model.LabSelection == 1 || model.LabSelection == 2) && model.LabCompleteDate.HasValue && model.LabBloodTestDate.HasValue && healthNumbers.BloodTestDate.HasValue && model.LabBloodTestDate == healthNumbers.BloodTestDate) || healthNumbers.LabId.HasValue))
                        model.readOnlyLab = true;
                    if (HRAUtility.hasCompletedHealthNumbers(model.HealthNumbers) || (CommonUtility.IsIntegratedWithLMC(HttpContext.Session.GetInt32(SessionContext.IntegrationWith)) && model.HealthNumbers.BloodTestDate.HasValue))
                    {
                        if (model.ShowLabDetails == false && !CommonUtility.HasAdminRole(User.RoleCode()))
                            model.readOnlyLab = model.ShowLabDetails = true;
                        return PartialView("_HealthNumbers", model);
                    }
                }
                if (response.hra.HealthNumbers == null)
                {
                    model.IncompleteHRA = true;
                    var recentHRA = ParticipantUtility.GetPrevYearStatus(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : "", User.TimeZone());
                    if (recentHRA.hra != null && recentHRA.hra.HealthNumbers != null && recentHRA.hra.HealthNumbers.Height.HasValue)
                    {
                        if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Imperial)
                        {
                            var height = CommonUtility.ToFeetInches(recentHRA.hra.HealthNumbers.Height.Value);
                            model.HeightFt = height.Key;
                            model.HeightInch = (float)height.Value;
                        }
                        else if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric)
                        {
                            model.HealthNumbersInMetric = new HealthNumbersDto();
                            model.HealthNumbersInMetric.HeightCM = CommonUtility.ToMetric(recentHRA.hra.HealthNumbers.Height.Value, BioLookup.Height, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                        }
                    }
                    else if (CommonUtility.IsIntegratedWithLMC(HttpContext.Session.GetInt32(SessionContext.IntegrationWith)))
                    {
                        if (canriskQuestionnaire != null)
                        {
                            if (model.HealthNumbers == null)
                                model.HealthNumbers = new HealthNumbersDto();
                            if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Imperial)
                            {
                                var height = CommonUtility.ToFeetInches(canriskQuestionnaire.Height.Value);
                                model.HeightFt = height.Key;
                                model.HeightInch = (float)height.Value;
                                model.HealthNumbers.Weight = canriskQuestionnaire.Weight;
                                model.HealthNumbers.Waist = canriskQuestionnaire.Waist;

                            }
                            else if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric)
                            {
                                model.HealthNumbersInMetric = new HealthNumbersDto();
                                model.HealthNumbersInMetric.HeightCM = CommonUtility.ToMetric(canriskQuestionnaire.Height.Value, BioLookup.Height, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                                model.HealthNumbersInMetric.Weight = CommonUtility.ToMetric(canriskQuestionnaire.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                                model.HealthNumbersInMetric.Waist = CommonUtility.ToMetric(canriskQuestionnaire.Waist.Value, BioLookup.Waist, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                            }
                        }
                        if (eligResponse != null && eligResponse.Eligibility.Lab_Date.HasValue && (eligResponse.Eligibility.Lab_Date.Value > DateTime.UtcNow.Date.AddDays(-60)))
                        {
                            var eligibilityLab = labs.Where(x => x.Id == model.LabId).FirstOrDefault();
                            if (model.HealthNumbers == null)
                                model.HealthNumbers = new HealthNumbersDto();
                            model.BloodTestDate = eligibilityLab.BloodTestDateString;
                            model.HealthNumbers.DidYouFast = eligibilityLab.DidYouFast;
                            model.HealthNumbers.Glucose = eligibilityLab.Glucose;
                            model.HealthNumbers.A1C = eligibilityLab.A1C;
                            model.readOnlyLab = true;
                        }
                    }
                }
                if (response.hra.Portal.usePreviousLabs && (model.LabId == 0 || model.HealthNumbers == null || !HRAUtility.hasCompletedHealthNumbers(model.HealthNumbers)))
                {
                    var latestLabs = labs.Where(x => x.BloodTestDate >= Convert.ToDateTime(model.ValidLabs) && (x.LabOrder != null || x.HL7 != null)).OrderByDescending(x => x.BloodTestDate).ToList();
                    if (latestLabs.Count() > 0)
                    {
                        var recentLab = latestLabs.FirstOrDefault();
                        if (HRAUtility.hasCompletedHealthNumbers(null, recentLab))
                        {
                            var assessmentDate = response.hra.CompleteDate;
                            var validDays = response.hra.Portal.HRAValidity;
                            if (recentLab.BloodTestDate >= assessmentDate && HttpContext.Session.GetInt32(SessionContext.UserinProgramId).HasValue)
                            {
                                var userinProgram = ProgramUtility.GetUserinProgramDetails(HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value);
                                if (userinProgram != null && userinProgram.UserId == recentLab.UserId && userinProgram.IsActive == true && userinProgram.StartDate.HasValue && userinProgram.HRAId == response.hra.Id)
                                {
                                    assessmentDate = userinProgram.StartDate.Value;
                                    validDays = (byte)HRAs.HRAProgramValidity;
                                }
                            }
                            if (!response.hra.CompleteDate.HasValue || (response.hra.CompleteDate.HasValue && assessmentDate.HasValue && HRAUtility.IsValidStratificationWindow(new ValidateStratificationRequest { AssessmentCompletionDate = assessmentDate.Value, labCompletionDate = recentLab.BloodTestDate.Value, HRAValidity = validDays.Value }).success))
                            {
                                model.recentLabs = true;
                                if (model.HealthNumbers == null)
                                    model.HealthNumbers = new HealthNumbersDto();
                                model.BloodTestDate = recentLab.BloodTestDateString;
                                model.HealthNumbers.DidYouFast = recentLab.DidYouFast;
                                model.HealthNumbers.Glucose = recentLab.Glucose;
                                model.HealthNumbers.HDL = recentLab.HDL;
                                model.HealthNumbers.LDL = recentLab.LDL;
                                model.HealthNumbers.Trig = recentLab.Trig;
                                model.HealthNumbers.TotalChol = recentLab.TotalChol;
                                model.HealthNumbers.A1C = recentLab.A1C;
                                model.hasRecentLab = DateTime.UtcNow.AddDays(-30) <= recentLab.BloodTestDate ? true : false;
                                var Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(Unit.Metric)).Measurements;
                                model.HealthNumbersInMetric.TotalChol = recentLab.TotalChol.HasValue ? ListOptions.ToMetric(recentLab.TotalChol.Value, BioLookup.Cholesterol, Measurements) : model.HealthNumbersInMetric.TotalChol;
                                model.HealthNumbersInMetric.Trig = recentLab.Trig.HasValue ? ListOptions.ToMetric(recentLab.Trig.Value, BioLookup.Triglycerides, Measurements) : model.HealthNumbersInMetric.Trig;
                                model.HealthNumbersInMetric.HDL = recentLab.HDL.HasValue ? ListOptions.ToMetric(recentLab.HDL.Value, BioLookup.HDL, Measurements) : model.HealthNumbersInMetric.HDL;
                                model.HealthNumbersInMetric.LDL = recentLab.LDL.HasValue ? ListOptions.ToMetric(recentLab.LDL.Value, BioLookup.LDL, Measurements) : model.HealthNumbersInMetric.LDL;
                                model.HealthNumbersInMetric.Glucose = recentLab.Glucose.HasValue ? ListOptions.ToMetric(recentLab.Glucose.Value, BioLookup.Glucose, Measurements) : model.HealthNumbersInMetric.Glucose;
                                if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value) == recentLab.PortalId)
                                    model.hasRecentLab = true;
                                model.readOnlyLab = true;
                            }
                        }
                    }
                    if (model.usePreviousLabs)
                        model.ValidLabs = CommonUtility.dateFormater(validDate, false, HttpContext.Session.GetString(SessionContext.DateFormat));
                }
            }

            return PartialView("_HealthNumbers", model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult HealthNumbers(HealthNumbersModel model, bool bloodwork)
        {
            model.userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            model.participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.hraId = HttpContext.Session.GetInt32(SessionContext.HRAId);
            model.participantPortalId = HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId);
            model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            model.userinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId);
            model.hraCompleteDate = !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.HRACompleteDate)) ? Convert.ToDateTime(HttpContext.Session.GetString(SessionContext.HRACompleteDate)) : null;
            model.hraPageSeq = HttpContext.Session.GetString(SessionContext.HRAPageSeq);
            model.hraPageSeqDone = HttpContext.Session.GetString(SessionContext.HRAPageSeqDone);
            model.systemAdminId = _appSettings.SystemAdminId;
            model.OrganizationId = HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value;
			model.DTCOrgCode = _appSettings.DTCOrgCode;
			var response = HRAUtility.AddEditHealthNumbers(model, bloodwork);
            updateHRASessions(response);

            return Json(new { Result = "success" });
        }

        [Authorize]
        [HttpPost]
        public JsonResult GetHRACompletionPercent(string HRAPageSeq, string HRAPageSeqDone)
        {
            int percent;
            if (!string.IsNullOrEmpty(HRAPageSeq) && !string.IsNullOrEmpty(HRAPageSeqDone))
                percent = HRAUtility.GetHRACompletionPercent(HRAPageSeq, HRAPageSeqDone);
            else
                percent = HRAUtility.GetHRACompletionPercent(HttpContext.Session.GetString(SessionContext.HRAPageSeq), HttpContext.Session.GetString(SessionContext.HRAPageSeqDone));
            return Json(percent);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult HRAModule(int hraid)
        {
            HRAModuleModel model = new HRAModuleModel();
            model.HRASummary = GetHRAReport(hraid);
            model.hraid = hraid;
            return PartialView("_HRAModule", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult HRA(int hraid)
        {
            HRAModel model = new HRAModel();
            if (!(hraid > 0))
                hraid = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            if (hraid > 0)
            {
                if (HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue && hraid == HttpContext.Session.GetInt32(SessionContext.HRAId).Value)
                {
                    var response = HRAUtility.ReadHRA(hraid);
                    HttpContext.Session.SetString(SessionContext.HRAPageSeqDone, model.HAPageSeqDone = response.hra.HAPageSeqDone);
                    HttpContext.Session.SetString(SessionContext.HRACompleteDate, (model.CompleteDate = response.hra.CompleteDate).ToString());
                    model.UserId = response.hra.UserId;
                    model.CreatedBy = response.hra.CreatedBy == null ? 0 : response.hra.CreatedBy ?? default(int);
                    model.HRAId = response.hra.Id;
                }
                else
                {
                    var response = HRAUtility.ReadHRA(hraid);
                    model.HAPageSeqDone = response.hra.HAPageSeqDone;
                    model.CompleteDate = response.hra.CompleteDate;
                    model.HRAId = response.hra.Id;
                    model.UserId = response.hra.UserId;
                    model.CreatedBy = response.hra.CreatedBy == null ? 0 : response.hra.CreatedBy ?? default(int);
                    model.isPastHRA = true;
                }
                model.hraPercent = HRAUtility.GetHRACompletionPercent(HttpContext.Session.GetString(SessionContext.HRAPageSeq), model.HAPageSeqDone);
            }
            return PartialView("_HRA", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult HRAReport(int hraid)
        {
            if (!(hraid > 0))
                hraid = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            return PartialView("_HRAReport", GetHRAReport(hraid));
        }

        public HRASummaryModel GetHRAReport(int hraid)
        {
            HRASummaryModel model = new HRASummaryModel();
            if (hraid > 0)
            {
                model.hraid = hraid;
                model.HRAVer = HttpContext.Session.GetInt32(SessionContext.HRAVer);
                model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
                var response = ReportUtility.ReadHRAReport(hraid);
                if (response.hra != null && response.hra.CompleteDate.HasValue)
                {
                    UsersinProgramDto program = null;
                    model.hasSummary = true;
                    model.Gender = response.hra.User.Gender;
                    model.ageInfo = ReportUtility.GetAgeInformation(response.hra, response.hra.User);
                    model.wellnessScoreInfo = ReportUtility.GetWellnessScoreInfo(response.hra.WellnessScore.Value, (byte)HttpContext.Session.GetInt32(SessionContext.HRAVer), HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : "", User.TimeZone());
                    var Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
                    var prevStatus = ParticipantUtility.GetPrevYearStatus(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : "", User.TimeZone());
                    if ((HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue && hraid != HttpContext.Session.GetInt32(SessionContext.HRAId).Value) || !HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue)
                    {
                        prevStatus.hra = null;
                        program = ProgramUtility.GetUserProgramForHRA(hraid);
                        if (program == null)
                            program = new UsersinProgramDto();
                    }
                    //Peer Average Score
                    var peerAverage = ReportUtility.GetAvgWellnessScore(HttpContext.Session.GetInt32(SessionContext.HRAId), hraid);
                    if (peerAverage > 0)
                        model.PeerAverage = CommonUtility.GetNewWellnessScore((float)peerAverage, (byte)HttpContext.Session.GetInt32(SessionContext.HRAVer));
                    RisksModel risks = new RisksModel();
                    //Health Measurement Risks
                    //hds risk
                    risks.hdsRisk = ReportUtility.GetHDSRisk(response.hra.User, response.hra, prevStatus.hra, true);
                    if (risks.hdsRisk.risk == 1)
                        model.healthMeasurementRisks = 1;
                    //bp risk
                    risks.bpRisk = ReportUtility.GetBPRisk(response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer).Value, null, true, prevStatus.hra, true);
                    if (risks.bpRisk.risk == 1)
                        model.healthMeasurementRisks = model.healthMeasurementRisks + 1;
                    //ct risk
                    risks.ctRisk = ReportUtility.GetCTRisk(response.hra.User, response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer).Value, null, Measurements, prevStatus.hra, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                    if (risks.ctRisk.risk == 1)
                        model.healthMeasurementRisks = model.healthMeasurementRisks + 1;
                    //diabetes risk
                    risks.diabetesRisk = ReportUtility.GetDiabetesRisk(response.hra.User, response.hra, true, prevStatus.hra, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                    if (risks.diabetesRisk.risk == 1)
                        model.healthMeasurementRisks = model.healthMeasurementRisks + 1;
                    //overweight risk
                    risks.overweightRisk = ReportUtility.GetOverweightRisk(response.hra, null, Measurements, prevStatus.hra, HttpContext.Session.GetInt32(SessionContext.Unit).Value, true);
                    if (risks.overweightRisk.risk > 0)
                        model.healthMeasurementRisks = model.healthMeasurementRisks + 1;
                    //Lifestyle Risks
                    //physical activity risk
                    risks.paRisk = ReportUtility.GetPARisk(response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer), null);
                    if (risks.paRisk.risk == 1)
                        model.lifetimeRisks = 1;
                    risks.adaRisk = ReportUtility.GetADAScore(response.hra);
                    if (risks.adaRisk.risk == 1)
                        model.lifetimeRisks = 1;
                    //nutrition risk
                    risks.nutRisk = ReportUtility.GetNutritionRisk(response.hra);
                    if (risks.nutRisk.risk == 1)
                        model.lifetimeRisks = model.lifetimeRisks + 1;
                    //stress risk
                    risks.stressRisk = ReportUtility.GetStressRisk(response.hra);
                    if (risks.stressRisk.risk == 1)
                        model.lifetimeRisks = model.lifetimeRisks + 1;
                    //tobacco risk
                    risks.tobaccoRisk = ReportUtility.GetTobaccoRisk(response.hra, null, prevStatus.hra);
                    if (risks.tobaccoRisk.risk == 1)
                        model.lifetimeRisks = model.lifetimeRisks + 1;
                    //Other Modifiable Risks
                    //met risk
                    risks.metRisk = ReportUtility.GetMetRisk(response.hra);
                    if (risks.metRisk.risk == 1)
                        model.otherModifiableRisks = model.otherModifiableRisks + 1;
                    //cancer risk
                    risks.cancerRisk = ReportUtility.GetCancerRisk(response.hra);
                    if (risks.cancerRisk.risk == 1)
                        model.otherModifiableRisks = model.otherModifiableRisks + 1;
                    //safety risk
                    risks.safetyRisk = ReportUtility.GetSafetyRisk(response.hra);
                    if (risks.safetyRisk.risk == 1)
                        model.otherModifiableRisks = model.otherModifiableRisks + 1;
                    model.risks = risks;
                    //key action steps
                    var keyActionSteps = ReportUtility.KeyActionSteps(hraid, ShowSelfScheduling());
                    model.keyActionSteps = keyActionSteps.gapActionSteps.Where(x => x.noneIdentified != true).Count() + keyActionSteps.lifetimeActionSteps.Where(x => x.noneIdentified != true).Count();
                    model.medicalConditionsandRiskFactors = ReportUtility.GetMedicalConditionsandRiskFactors(response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer), response.hra.User, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                    model.measurementsandGoals = ReportUtility.GetMeasurementsandGoals(response.hra, response.hra.Goals, response.hra.User, response.hra.Age.Value, HttpContext.Session.GetInt32(SessionContext.Unit).Value, HttpContext.Session.GetInt32(SessionContext.IntegrationWith).Value);
                    model.checkupsandGoals = ReportUtility.GetCheckupsandGoals(response.hra.Exams, response.hra.HealthNumbers, response.hra.User, response.hra.Age.Value);
                    model.immunizationandGoals = ReportUtility.GetImmunizationandGoals(response.hra.Exams, response.hra.HealthNumbers, response.hra.User, response.hra.Age.Value);
                    model.nutritionGoal = ReportUtility.NutritionGoal(response.hra.Goals, program, HttpContext.Session.GetInt32(SessionContext.ProgramType), HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.Gender), ShowSelfScheduling());
                    model.physicalActivityGoal = ReportUtility.PhysicalActivityGoal(response.hra, program, HttpContext.Session.GetInt32(SessionContext.ProgramType));
                    model.drReferralGoal = ReportUtility.DrReferral(response.hra.Goals, HttpContext.Session.GetInt32(SessionContext.HRAVer), response.hra.HealthNumbers.CAC);
                    if (HttpContext.Session.GetInt32(SessionContext.HRAVer).HasValue && HttpContext.Session.GetInt32(SessionContext.HRAVer).Value == (int)HRAVersions.ActivateVersion)
                    {
                        var BMI = CommonUtility.GetBMI(response.hra.HealthNumbers.Height.Value, response.hra.HealthNumbers.Weight.Value);
                        var age = model.ageInfo.Age;
                        var RHR = response.hra.HealthNumbers.RHR ?? 0;
                        model.PAISummary = response.hra.PAISummary;
                        if (HttpContext.Session.GetInt32(SessionContext.Gender) == 1)
                        {
                            model.CRF = Math.Round((float)(92.5 - (0.327 * age) - (0.933 * BMI) - (0.167 * RHR) + (0.257 * model.PAISummary)), 2);
                        }
                        else
                        {
                            model.CRF = Math.Round((float)(70.77 - (0.244 * age) - (0.749 * BMI) - (0.107 * RHR) + (0.213 * model.PAISummary)), 2);
                        }
                        if (response.hra.HealthNumbers.CRF.HasValue)
                            model.MeasuredCRF = Math.Round((float)response.hra.HealthNumbers.CRF, 2);
                    }
                    //Tobacco goal
                    TobaccoGoalModel tobaccoGoal = new TobaccoGoalModel();
                    tobaccoGoal.tobaccoGoalContent = ReportUtility.GetTobaccoGoalContent(response);
                    model.tobaccoGoal = tobaccoGoal;
                    //Stress goal
                    StressGoalModel stressGoal = new StressGoalModel();
                    stressGoal.stressGoalContent = ReportUtility.GetStressGoalContent(program, HttpContext.Session.GetInt32(SessionContext.ProgramType));
                    model.stressGoal = stressGoal;
                    //Medication Goal
                    MedicationGoal medicationGoal = new MedicationGoal();
                    medicationGoal.medicationGoalContent = ReportUtility.GetMedicationGoalContent(program, HttpContext.Session.GetInt32(SessionContext.ProgramType));
                    model.medicationGoal = medicationGoal;
                    //Supplement Goal
                    SupplementGoalModel supplementGoal = new SupplementGoalModel();
                    supplementGoal.supplementGoalContent = ReportUtility.GetSupplementGoalContent();
                    model.supplementGoal = supplementGoal;
                    model.CAC = response.hra.HealthNumbers.CAC;
                }
                else
                    model.hasSummary = false;
            }
            else
                model.hasSummary = false;
            return model;
        }
        [Authorize]
        [HttpPost]
        public JsonResult ChangeTelHRA([FromBody] ChangeTelHRARequest request)
        {
            HRAUtility.ChangeTelHRARequest(request);
            return Json(true);
        }
    }
}