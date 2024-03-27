using Intervent.Business.SFax;
using Intervent.Web.DTO;
using InterventWebApp.Helpers;
using IronPdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace InterventWebApp
{
	public class ReportsController : BaseController
	{
		private readonly AppSettings _appSettings;
		private readonly IModelMetadataProvider _metadataProvider;
		private readonly ICompositeViewEngine _viewEngine;
		private readonly ITempDataProvider _tempDataProvider;

		public ReportsController(IOptions<AppSettings> appSettings, IModelMetadataProvider metadataProvider, ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider)
		{
			_viewEngine = viewEngine;
			_tempDataProvider = tempDataProvider;
			_metadataProvider = metadataProvider;
			_appSettings = appSettings.Value;
		}

		#region HRAReport

		[Authorize]
		public ActionResult BriefReport()
		{
			ReportsModel model = new ReportsModel();
			if (HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue)
			{
				var hra = ReportUtility.ReadHRAReport(HttpContext.Session.GetInt32(SessionContext.HRAId).Value).hra;
				if (hra != null)
				{
					HttpContext.Session.SetInt32(SessionContext.HRAId, hra.Id);
					if (hra.Goals != null)
					{
						WellnessScoreInfo wellnessScoreInfo = new WellnessScoreInfo();
						model.wellnessScoreInfo = ReportUtility.GetWellnessScoreInfo(hra.WellnessScore.Value, (byte)HttpContext.Session.GetInt32(SessionContext.HRAVer), HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : "", User.TimeZone());
					}
				}
			}
			return PartialView("_BriefReport", model);
		}

		[Authorize]
		public ActionResult ReportsDashboard()
		{
			ReportsDashboardModel model = new ReportsDashboardModel();
			var response = ReportUtility.ReadHRAReport(HttpContext.Session.GetInt32(SessionContext.HRAId).Value);
			model.Name = response.hra.User.FirstName + " " + response.hra.User.LastName;
			WellnessScoreInfo wellnessScoreInfo = new WellnessScoreInfo();
			model.wellnessScoreInfo = ReportUtility.GetWellnessScoreInfo(response.hra.WellnessScore.Value, (byte)HttpContext.Session.GetInt32(SessionContext.HRAVer), HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : "", User.TimeZone());
			model.picture = response.hra.User.Picture;
			model.ageInfo = ReportUtility.GetAgeInformation(response.hra, response.hra.User);
			IList<MeasurementsDto> Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
			var prevStatus = ParticipantUtility.GetPrevYearStatus(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : "", User.TimeZone());
			//check if pregnant
			if (response.hra.MedicalCondition.Pregnant.HasValue)
				model.pregnant = response.hra.MedicalCondition.Pregnant;
			//Peer Average Score
			var peerAverage = ReportUtility.GetAvgWellnessScore(HttpContext.Session.GetInt32(SessionContext.HRAId), null);
			if (peerAverage > 0)
				model.PeerAverage = CommonUtility.GetNewWellnessScore((float)peerAverage, (byte)HttpContext.Session.GetInt32(SessionContext.HRAVer));
			RisksModel risks = new RisksModel();
			//Health Measurement Risks
			//hds risk
			risks.hdsRisk = ReportUtility.GetHDSRisk(response.hra.User, response.hra, prevStatus.hra);
			if (risks.hdsRisk.risk == 1)
				model.healthMeasurementRisks = 1;
			//bp risk
			risks.bpRisk = ReportUtility.GetBPRisk(response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer), null, true, prevStatus.hra, false);
			if (risks.bpRisk.risk == 1)
				model.healthMeasurementRisks = model.healthMeasurementRisks + 1;
			//ct risk
			risks.ctRisk = ReportUtility.GetCTRisk(response.hra.User, response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer), null, Measurements, prevStatus.hra, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
			if (risks.ctRisk.risk == 1)
				model.healthMeasurementRisks = model.healthMeasurementRisks + 1;
			//diabetes risk
			risks.diabetesRisk = ReportUtility.GetDiabetesRisk(response.hra.User, response.hra, true, prevStatus.hra, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
			if (risks.diabetesRisk.risk == 1)
				model.healthMeasurementRisks = model.healthMeasurementRisks + 1;
			//overweight risk
			risks.overweightRisk = ReportUtility.GetOverweightRisk(response.hra, null, Measurements, prevStatus.hra, HttpContext.Session.GetInt32(SessionContext.Unit).Value, false);
			if (risks.overweightRisk.risk > 0)
				model.healthMeasurementRisks = model.healthMeasurementRisks + 1;
			//Lifestyle Risks
			//physical activity risk
			risks.paRisk = ReportUtility.GetPARisk(response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer), null);
			if (risks.paRisk.risk == 1)
				model.lifetimeRisks = 1;
			risks.adaRisk = ReportUtility.GetADAScore(response.hra);
			if (risks.adaRisk.risk == 1)
				model.lifetimeRisks = model.lifetimeRisks + 1;
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
			model.goals = response.hra.Goals;
			TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
			model.hraCompleteDate = response.hra.UpdatedOn == null ? CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(response.hra.CompleteDate.Value, custTZone), false, HttpContext.Session.GetString(SessionContext.DateFormat)) :
									CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(response.hra.UpdatedOn.Value, custTZone), false, HttpContext.Session.GetString(SessionContext.DateFormat));
			model.CAC = response.hra.HealthNumbers.CAC;
			if (!response.hra.HealthNumbers.BloodTestDate.HasValue)
			{
				model.showNote = response.hra.CompleteDate.Value > DateTime.UtcNow.AddDays(-(response.hra.Portal.HRAValidity ?? 45));
			}

			//key action steps
			bool showSelfScheduling = ShowSelfScheduling();
			var keyActionSteps = ReportUtility.KeyActionSteps(HttpContext.Session.GetInt32(SessionContext.HRAId).Value, showSelfScheduling);
			model.keyActionSteps = keyActionSteps.gapActionSteps.Where(x => x.noneIdentified != true).Count() + keyActionSteps.lifetimeActionSteps.Where(x => x.noneIdentified != true).Count();
			model.gender = HttpContext.Session.GetInt32(SessionContext.Gender);
			model.dob = HttpContext.Session.GetString(SessionContext.DOB);
			model.hraId = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
			model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
			bool isMediOrbisUser = !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.IsMediOrbisUser)) && Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.IsMediOrbisUser));
			model.showSelfScheduling = showSelfScheduling && (!isMediOrbisUser || (isMediOrbisUser && HttpContext.Session.GetString(SessionContext.ProgramCode) != null && HttpContext.Session.GetString(SessionContext.ProgramCode) == "COACH"));
			return View(model);
		}

		[Authorize]
		public ActionResult DrSummary()
		{
			return PartialView("DrSummary", GetDrSummaryModel());
		}

		private DrSummaryModel GetDrSummaryModel()
		{
			var response = ReportUtility.ReadHRAReport(HttpContext.Session.GetInt32(SessionContext.HRAId).Value);
			DrSummaryModel model = new DrSummaryModel();
			model.BaseUrl = _appSettings.EmailUrl;
			model.ageInfo = ReportUtility.GetAgeInformation(response.hra, response.hra.User);
			if (response.hra.HealthNumbers.CAC.HasValue)
			{
				model.CAC = response.hra.HealthNumbers.CAC;
				model.arterialAgeText = string.Format(Translate.Message("L2008"), ((model.ageInfo.Age >= 45 && model.ageInfo.Age < 85) ? "is " + model.ageInfo.arterialAge.Value + " years." : Translate.Message("L2011")));
				model.calciumRiskText = ReportUtility.GetHDSRisk(response.hra.User, response.hra, null).calciumRiskText.ToString();
				model.wellnessheaderText = model.CAC.HasValue ? Translate.Message("L2006") : Translate.Message("L2007");
			}
			//check if pregnant
			if (response.hra.MedicalCondition.Pregnant.HasValue)
				model.pregnant = response.hra.MedicalCondition.Pregnant;
			if (!response.hra.HealthNumbers.BloodTestDate.HasValue)
			{
				model.showNote = response.hra.CompleteDate.Value > DateTime.UtcNow.AddDays(-(response.hra.Portal.HRAValidity ?? 45));
			}
			var peerAverage = ReportUtility.GetAvgWellnessScore(HttpContext.Session.GetInt32(SessionContext.HRAId), null);
			model.wellnessScoreInfo = ReportUtility.GetWellnessScoreInfo(response.hra.WellnessScore.Value, (byte)HttpContext.Session.GetInt32(SessionContext.HRAVer), HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : "", User.TimeZone());
			if (peerAverage > 0)
				model.PeerAverage = CommonUtility.GetNewWellnessScore((float)peerAverage, (byte)HttpContext.Session.GetInt32(SessionContext.HRAVer));
			model.keyActionSteps = ReportUtility.KeyActionSteps(HttpContext.Session.GetInt32(SessionContext.HRAId).Value, ShowSelfScheduling());
			model.user = response.hra.User;
			TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
			model.HraCompleteDate = response.hra.UpdatedOn == null ? CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(response.hra.CompleteDate.Value, custTZone), false, HttpContext.Session.GetString(SessionContext.DateFormat)) :
									CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(response.hra.UpdatedOn.Value, custTZone), false, HttpContext.Session.GetString(SessionContext.DateFormat));
			if (response.hra.HealthNumbers.Lab != null && response.hra.HealthNumbers.Lab.LabSelection.HasValue)
				model.HraLabSource = response.hra.HealthNumbers.Lab.LabSelection.Value == LabSelection.DoctorsOffice ? Translate.Message("L3155") : response.hra.HealthNumbers.Lab.LabSelection.Value == LabSelection.LabCorp ? (CommonUtility.IsIntegratedWithLMC(HttpContext.Session.GetInt32(SessionContext.IntegrationWith)) ? Translate.Message("L3580") : Translate.Message("L3156")) : "";
			model.BMI = CommonUtility.GetBMI(response.hra.HealthNumbers.Height.Value, response.hra.HealthNumbers.Weight.Value);
			model.medicalConditionsandRiskFactors = ReportUtility.GetMedicalConditionsandRiskFactors(response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer), response.hra.User, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
			model.measurementsandGoals = ReportUtility.GetMeasurementsandGoals(response.hra, response.hra.Goals, response.hra.User, model.ageInfo.Age, HttpContext.Session.GetInt32(SessionContext.Unit).Value, HttpContext.Session.GetInt32(SessionContext.IntegrationWith));
			model.checkupsandGoals = ReportUtility.GetCheckupsandGoals(response.hra.Exams, response.hra.HealthNumbers, response.hra.User, model.ageInfo.Age);
			model.immunizationandGoals = ReportUtility.GetImmunizationandGoals(response.hra.Exams, response.hra.HealthNumbers, response.hra.User, model.ageInfo.Age);
			model.DrReferralModel = ReportUtility.DrReferral(response.hra.Goals, HttpContext.Session.GetInt32(SessionContext.HRAVer), response.hra.HealthNumbers.CAC);
			var adaRisk = ReportUtility.GetADAScore(response.hra);
			model.GINAQuestion = adaRisk.GINAQuestion;
			model.isDiaborPreDiab = adaRisk.isDiaborPreDiab;
			var prevStatus = ParticipantUtility.GetPrevYearStatus(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : "", User.TimeZone(), true, true);
			if (prevStatus.hra != null && prevStatus.hra.CompleteDate.HasValue)
			{
				model.PrevHraCompleteDate = CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(prevStatus.hra.CompleteDate.Value, custTZone), false, HttpContext.Session.GetString(SessionContext.DateFormat));
				model.initialMeasurementsandGoals = ReportUtility.GetMeasurementsandGoals(prevStatus.hra, prevStatus.hra.Goals, prevStatus.hra.User, model.ageInfo.Age, HttpContext.Session.GetInt32(SessionContext.Unit).Value, HttpContext.Session.GetInt32(SessionContext.IntegrationWith));
			}
			//prev follow-up detai;s
			if (prevStatus.followupResponse != null)
			{
				model.hadFollowUp = true;
				model.PreviousYearScheduledCoachingSession = prevStatus.followupResponse.PreviousYearScheduledCoachingSession;
				model.PreviousYearCompletedCoachingSession = prevStatus.followupResponse.PreviousYearCompletedCoachingSession;

				model.PreviousYearAssignedKitsmodel = new List<AssignedKit>();
				if (prevStatus.followupResponse.PreviousYearKits != null && prevStatus.followupResponse.PreviousYearKits.Count > 0)
				{
					model.PreviousYearAssignedKitsmodel.AddRange(prevStatus.followupResponse.PreviousYearKits);
				}
			}
			model.clientNameInReport = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.ClientNameInReport) != null ? HttpContext.Session.GetString(SessionContext.ClientNameInReport) : false);
			model.organizationName = HttpContext.Session.GetString(SessionContext.OrganizationName);
			model.orgContactEmail = HttpContext.Session.GetString(SessionContext.OrgContactEmail);
			model.dob = HttpContext.Session.GetString(SessionContext.DOB);
			model.dateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
			model.hraId = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
			model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
			model.coachingProgram = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.CoachingProgram) != null ? HttpContext.Session.GetString(SessionContext.CoachingProgram) : false);
			model.selfHelpProgram = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.SelfHelpProgram) != null ? HttpContext.Session.GetString(SessionContext.SelfHelpProgram) : false);
			return model;
		}

		[Authorize]
		public ActionResult ScoreCard(int hraid)
		{
			var response = ReportUtility.ReadHRAReport(hraid);
			ScorecardModel model = new ScorecardModel();
			model = ReportUtility.GetScorecardItems(response.hra.User, response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer), HttpContext.Session.GetInt32(SessionContext.Unit).Value);
			WellnessScoreInfo wellnessScoreInfo = new WellnessScoreInfo();
			model.wellnessScoreInfo = ReportUtility.GetWellnessScoreInfo(response.hra.WellnessScore.Value, (byte)HttpContext.Session.GetInt32(SessionContext.HRAVer), HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : "", User.TimeZone());
			model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
			return PartialView("_ScorecardDetail", model);
		}

		[Authorize]
		public ActionResult KeyActionSteps(int hraid)
		{
			var response = ReportUtility.KeyActionSteps(hraid, ShowSelfScheduling());
			return PartialView("_KeyActionSteps", response);
		}

		[Authorize]
		public PrintableDetailedPlanModel GetPrintableDetailedPlan()
		{
			var response = ReportUtility.ReadHRAReport(HttpContext.Session.GetInt32(SessionContext.HRAId).Value);
			PrintableDetailedPlanModel model = new PrintableDetailedPlanModel();
			model.User = response.hra.User;
			model.HRA = response.hra;
			if (CommonUtility.HasAdminRole(User.RoleCode()) && HttpContext.Session.GetString(SessionContext.NeedCareplanApproval) != null && Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.NeedCareplanApproval)) == true)
			{
				var carePlanReportResponse = ReportUtility.GetCarePlanReport((int)ReportTypes.HRA, HttpContext.Session.GetInt32(SessionContext.HRAId).Value.ToString()).carePlanReport;
				if (carePlanReportResponse != null)
				{
					TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
					model.reportGenerated = carePlanReportResponse.ReportGenerated.Value;
					model.reportGeneratedOn = TimeZoneInfo.ConvertTimeFromUtc(carePlanReportResponse.CreatedOn.Value, custTZone).ToShortDateString();
					model.reportGeneratedBy = carePlanReportResponse.User.FirstName + " " + carePlanReportResponse.User.LastName;
				}
			}
			if (response.hra.ReportFeedbacks != null && response.hra.ReportFeedbacks.Count() > 0)
			{
				var comments = response.hra.ReportFeedbacks.Where(x => x.Type == 1).FirstOrDefault();
				model.CoachComments = comments != null ? comments.Comments : "";
				comments = response.hra.ReportFeedbacks.Where(x => x.Type == 2).FirstOrDefault();
				model.ProviderComments = comments != null ? comments.Comments : "";
			}
			var prevStatus = ParticipantUtility.GetPrevYearStatus(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : "", User.TimeZone());
			UsersinProgramDto program = ProgramUtility.GetUserProgramForHRA(HttpContext.Session.GetInt32(SessionContext.HRAId).Value);
			if (program == null)
				program = new UsersinProgramDto();
			model.DrReferralGoal = ReportUtility.DrReferral(response.hra.Goals, HttpContext.Session.GetInt32(SessionContext.HRAVer), response.hra.HealthNumbers.CAC);
			model.NutritionGoal = ReportUtility.NutritionGoal(response.hra.Goals, program, HttpContext.Session.GetInt32(SessionContext.ProgramType), HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.Gender), ShowSelfScheduling());
			model.PhysicalActivityGoal = ReportUtility.PhysicalActivityGoal(response.hra, program, HttpContext.Session.GetInt32(SessionContext.ProgramType));
			TobaccoGoalModel tobaccoGoal = new TobaccoGoalModel();
			tobaccoGoal.tobaccoGoalContent = ReportUtility.GetTobaccoGoalContent(response);
			model.TobaccoGoal = tobaccoGoal;
			StressGoalModel stressGoal = new StressGoalModel();
			stressGoal.stressGoalContent = ReportUtility.GetStressGoalContent(program, HttpContext.Session.GetInt32(SessionContext.ProgramType));
			model.StressGoal = stressGoal;
			MedicationGoal medicationGoal = new MedicationGoal();
			medicationGoal.medicationGoalContent = ReportUtility.GetMedicationGoalContent(program, HttpContext.Session.GetInt32(SessionContext.ProgramType));
			model.MedicationGoal = medicationGoal;
			SupplementGoalModel supplementGoal = new SupplementGoalModel();
			supplementGoal.supplementGoalContent = ReportUtility.GetSupplementGoalContent();
			model.SupplementGoal = supplementGoal;
			IList<MeasurementsDto> Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
			RisksModel risks = new RisksModel();
			//Health Measurement Risks
			//hds risk
			risks.hdsRisk = ReportUtility.GetHDSRisk(response.hra.User, response.hra, prevStatus.hra);
			if (risks.hdsRisk.risk == 1)
				model.HealthMeasurementRisks = 1;
			//bp risk
			risks.bpRisk = ReportUtility.GetBPRisk(response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer), null, true, prevStatus.hra, false);
			if (risks.bpRisk.risk == 1)
				model.HealthMeasurementRisks = model.HealthMeasurementRisks + 1;
			//ct risk
			risks.ctRisk = ReportUtility.GetCTRisk(response.hra.User, response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer), null, Measurements, prevStatus.hra, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
			if (risks.ctRisk.risk == 1)
				model.HealthMeasurementRisks = model.HealthMeasurementRisks + 1;
			//diabetes risk
			risks.diabetesRisk = ReportUtility.GetDiabetesRisk(response.hra.User, response.hra, true, prevStatus.hra, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
			if (risks.diabetesRisk.risk == 1)
				model.HealthMeasurementRisks = model.HealthMeasurementRisks + 1;
			//overweight risk
			risks.overweightRisk = ReportUtility.GetOverweightRisk(response.hra, null, Measurements, prevStatus.hra, HttpContext.Session.GetInt32(SessionContext.Unit).Value, false);
			if (risks.overweightRisk.risk > 0)
				model.HealthMeasurementRisks = model.HealthMeasurementRisks + 1;
			//Lifestyle Risks
			//physical activity risk
			risks.paRisk = ReportUtility.GetPARisk(response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer), null);
			if (risks.paRisk.risk == 1)
				model.LifetimeRisks = 1;
			risks.adaRisk = ReportUtility.GetADAScore(response.hra);
			if (risks.adaRisk.risk == 1)
				model.LifetimeRisks = model.LifetimeRisks + 1;
			//nutrition risk
			risks.nutRisk = ReportUtility.GetNutritionRisk(response.hra);
			if (risks.nutRisk.risk == 1)
				model.LifetimeRisks = model.LifetimeRisks + 1;
			//stress risk
			risks.stressRisk = ReportUtility.GetStressRisk(response.hra);
			if (risks.stressRisk.risk == 1)
				model.LifetimeRisks = model.LifetimeRisks + 1;
			//tobacco risk
			risks.tobaccoRisk = ReportUtility.GetTobaccoRisk(response.hra, null, prevStatus.hra);
			if (risks.tobaccoRisk.risk == 1)
				model.LifetimeRisks = model.LifetimeRisks + 1;
			//Other Modifiable Risks
			//met risk
			risks.metRisk = ReportUtility.GetMetRisk(response.hra);
			if (risks.metRisk.risk == 1)
				model.OtherModifiableRisks = model.OtherModifiableRisks + 1;
			//cancer risk
			risks.cancerRisk = ReportUtility.GetCancerRisk(response.hra);
			if (risks.cancerRisk.risk == 1)
				model.OtherModifiableRisks = model.OtherModifiableRisks + 1;
			//safety risk
			risks.safetyRisk = ReportUtility.GetSafetyRisk(response.hra);
			if (risks.safetyRisk.risk == 1)
				model.OtherModifiableRisks = model.OtherModifiableRisks + 1;
			model.Risks = risks;
			var summaryCarePlan = GetSummaryCarePlan();
			if (summaryCarePlan.stroke != null)
			{
				model.Stroke = summaryCarePlan.stroke;
				model.Stroke.isPrintableReport = true;
			}
			else if (summaryCarePlan.bloodPressure != null)
			{
				model.BloodPressure = summaryCarePlan.bloodPressure;
				model.BloodPressure.isPrintableReport = true;
			}
			else if (summaryCarePlan.cvdRisk != null)
			{
				model.CVDRisk = summaryCarePlan.cvdRisk;
				model.CVDRisk.isPrintableReport = true;
			}
			else if (summaryCarePlan.weightManagement != null)
			{
				model.WeightManagement = summaryCarePlan.weightManagement;
				model.WeightManagement.isPrintableReport = true;
			}
			model.needCareplanApproval = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.NeedCareplanApproval) != null ? HttpContext.Session.GetString(SessionContext.NeedCareplanApproval) : false);
			model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
			model.hraVer = HttpContext.Session.GetInt32(SessionContext.HRAVer).HasValue ? HttpContext.Session.GetInt32(SessionContext.HRAVer).Value : null;
			return model;
		}

		[Authorize]
		public ActionResult PrintableDetailedPlan()
		{
			return View("PrintableDetailedPlan", GetPrintableDetailedPlan());
		}

		[Authorize]
		public ActionResult HeartDiseaseInfo()
		{
			return PartialView("_HeartDiseaseInfo");
		}

		[Authorize]
		public ActionResult StrokeInfo()
		{
			return PartialView("_StrokeInfo");
		}

		[Authorize]
		public ActionResult BPInfo()
		{
			return PartialView("_BPInfo");
		}

		[Authorize]
		public ActionResult CTInfo()
		{
			return PartialView("_CTInfo");
		}

		[Authorize]
		public ActionResult OverweightInfo(int hraid)
		{
			var hra = ReportUtility.ReadHRAReport(hraid).hra;
			OverweightInfoModel model = new OverweightInfoModel();
			model.BMI = CommonUtility.GetBMI(hra.HealthNumbers.Height.Value, hra.HealthNumbers.Weight.Value);
			model.Waist = hra.HealthNumbers.Waist.HasValue ? (float)Math.Round(CommonUtility.ToMetric(hra.HealthNumbers.Waist.Value, BioLookup.Waist, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) : hra.HealthNumbers.Waist;
			return PartialView("_OverweightInfo", model);
		}

		[Authorize]
		public ActionResult MSInfo()
		{
			return PartialView("_MSInfo");
		}

		[Authorize]
		public ActionResult DiabetesInfo()
		{
			var hra = ReportUtility.ReadHRAReport(HttpContext.Session.GetInt32(SessionContext.HRAId).Value).hra;
			DiabetesInfoModel model = new DiabetesInfoModel();
			model.Diabetes = hra.Goals.Diabetes;
			return PartialView("_DiabetesInfo", model);
		}

		[Authorize]
		public ActionResult CancerInfo()
		{
			return PartialView("_CancerInfo");
		}

		[Authorize]
		public ActionResult PAInfo()
		{
			return PartialView("_PAInfo");
		}

		[Authorize]
		public ActionResult NutritionInfo()
		{
			return PartialView("_NutritionInfo");
		}

		[Authorize]
		public ActionResult StressInfo()
		{
			return PartialView("_StressInfo");
		}

		[Authorize]
		public ActionResult SafetyInfo()
		{
			return PartialView("_SafetyInfo");
		}

		[Authorize]
		public ActionResult TobaccoInfo()
		{
			var hra = ReportUtility.ReadHRAReport(HttpContext.Session.GetInt32(SessionContext.HRAId).Value).hra;
			TobaccoInfoModel model = new TobaccoInfoModel();
			model.SmokeCig = hra.OtherRiskFactors.SmokeCig;
			model.OtherTobacco = hra.OtherRiskFactors.OtherTobacco;
			model.Cigar = hra.OtherRiskFactors.Cigar;
			model.Pipe = hra.OtherRiskFactors.Pipe;
			model.SmokelessTob = hra.OtherRiskFactors.SmokelessTob;
			return PartialView("_TobaccoInfo", model);
		}

		[Authorize]
		public ActionResult RecommendationsDashboard()
		{
			return View(GetRecommendationsDashboardModel());
		}
		public RecommendationsModel GetRecommendationsDashboardModel()
		{
			RecommendationsModel model = new RecommendationsModel();
			var response = ReportUtility.ReadHRAReport(HttpContext.Session.GetInt32(SessionContext.HRAId).Value);
			if (response.hra.MedicalCondition.Pregnant.HasValue)
				model.pregnant = response.hra.MedicalCondition.Pregnant;
			var userInProgram = ProgramUtility.GetUserProgramForHRA(HttpContext.Session.GetInt32(SessionContext.HRAId).Value);
			if (userInProgram != null)
			{
				model.hasProgram = true;
				model.programType = userInProgram.ProgramsinPortal.program.ProgramType;
			}
			else
				model.hasProgram = false;
			model.dob = HttpContext.Session.GetString(SessionContext.DOB);
			model.coachingProgram = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.CoachingProgram) != null ? HttpContext.Session.GetString(SessionContext.CoachingProgram) : false);
			model.nextApptDate = HttpContext.Session.GetString(SessionContext.NextApptDate) != null ? HttpContext.Session.GetString(SessionContext.NextApptDate) : "";
			model.hraId = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
			model.carePlan = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.CarePlan) != null ? HttpContext.Session.GetString(SessionContext.CarePlan) : false);
			model.userStatus = HttpContext.Session.GetString(SessionContext.UserStatus) != null ? HttpContext.Session.GetString(SessionContext.UserStatus) : "";
			return model;
		}

		[Authorize]
		public ActionResult MyActionPlans()
		{
			return PartialView("_MyActionPlans", GetRecommendationsDashboardModel());
		}

		[Authorize]
		public ActionResult MyActionPlan()
		{
			return View("MyActionPlan", GetRecommendationsDashboardModel());
		}

		[Authorize]
		public ActionResult NutritionRecommendation(bool onlymeal = false, bool onlydiet = false)
		{
			NutritionGoalModel model = new NutritionGoalModel();
			var Goals = ReportUtility.ReadHRAGoals(HttpContext.Session.GetInt32(SessionContext.HRAId).Value).hraGoals;
			model = ReportUtility.NutritionGoal(Goals, null, HttpContext.Session.GetInt32(SessionContext.ProgramType), HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.Gender), ShowSelfScheduling());
			model.onlydiet = onlydiet;
			model.onlymeal = onlymeal;
			return PartialView("_NutritionGoal", model);
		}

		[Authorize]
		public ActionResult SummaryCarePlan()
		{
			return PartialView("_SummaryCarePlan", GetSummaryCarePlan());
		}

		public SummaryCarePlansModel GetSummaryCarePlan()
		{
			SummaryCarePlansModel model = new SummaryCarePlansModel();
			var Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
			if (HttpContext.Session.GetInt32(SessionContext.CarePlanType).HasValue && HttpContext.Session.GetInt32(SessionContext.CarePlanType) == (byte)CarePlanTypes.Stroke)
			{
				//stroke
				SummaryCarePlanModel strokeModel = new SummaryCarePlanModel();
				strokeModel.numbersandGoals = new List<string>
				{
				Translate.Message("L4303"),
				string.Format(Translate.Message("L4304"), "140"),
				string.Format(Translate.Message("L4306"), ((float)Math.Round(CommonUtility.ToMetric(70, BioLookup.LDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) + " " + Measurements[BioLookup.LDL].MeasurementUnit), ""),
				string.Format(Translate.Message("L4310"), (float)Math.Round(CommonUtility.ToMetric(100, BioLookup.HDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) + " " + Measurements[BioLookup.HDL].MeasurementUnit),
				string.Format(Translate.Message("L4311"), HasDiabetes() ? 7 : 5.7)
				};
				strokeModel.healthyLifestyle = new List<string>
				{
				string.Format(Translate.Message("L4312"), 90),
				Translate.Message("L4313"),
				Translate.Message("L4314"),
				string.Format(Translate.Message("L4315"), HttpContext.Session.GetInt32(SessionContext.Gender) == 1 ? Translate.Message("L4317") : Translate.Message("L4316")),
				Translate.Message("L4318"),
				Translate.Message("L4319")
				};
				strokeModel.selfManagement = new List<string>
				{
				Translate.Message("L4320"),
				Translate.Message("L4321"),
				Translate.Message("L4322"),
				Translate.Message("L4323")
				};
				strokeModel.regularMedicalCare = new List<string>
				{
				Translate.Message("L4324"),
				Translate.Message("L4325"),
				Translate.Message("L4326"),
				Translate.Message("L4327")
				};
				model.stroke = strokeModel;
			}
			else if (HttpContext.Session.GetInt32(SessionContext.CarePlanType).HasValue && HttpContext.Session.GetInt32(SessionContext.CarePlanType) == (byte)CarePlanTypes.BloodPressure)
			{
				//bloodPressure
				SummaryCarePlanModel bloodPressure = new SummaryCarePlanModel();
				bloodPressure.numbersandGoals = new List<string>
				{
					Translate.Message("L4369"),
					Translate.Message("L4370"),
					Translate.Message("L4371"),
					Translate.Message("L4372")
				};
				bloodPressure.healthyLifestyle = new List<string>
				{
					Translate.Message("L4373"),
					Translate.Message("L4313"),
					Translate.Message("L4314"),
					string.Format(Translate.Message("L4315"), Translate.Message("L4317")),
					Translate.Message("L4318"),
					Translate.Message("L4319")
				};
				bloodPressure.selfManagement = new List<string>
				{
					Translate.Message("L4374"),
					Translate.Message("L4321"),
					Translate.Message("L4322"),
					Translate.Message("L4375")
				};
				bloodPressure.regularMedicalCare = new List<string>
				{
					Translate.Message("L4324"),
					Translate.Message("L4325"),
					Translate.Message("L4326"),
					Translate.Message("L4327")
				};
				model.bloodPressure = bloodPressure;

			}
			else if (HttpContext.Session.GetInt32(SessionContext.CarePlanType).HasValue && HttpContext.Session.GetInt32(SessionContext.CarePlanType) == (byte)CarePlanTypes.CVD)
			{
				//cvdRisk
				var hra = ReportUtility.ReadHRAReport(HttpContext.Session.GetInt32(SessionContext.HRAId).Value).hra;
				SummaryCarePlanModel cvdRisk = new SummaryCarePlanModel();
				cvdRisk.numbersandGoals = new List<string>
				{
					Translate.Message("L4377"),
					string.Format(Translate.Message("L4304"), "130"),
					Translate.Message("L4305"),
					string.Format(Translate.Message("L4306"), (float)Math.Round(CommonUtility.ToMetric(hra.Goals.LtLdl.Value, BioLookup.LDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) + " " + Measurements[BioLookup.LDL].MeasurementUnit, string.Format(Translate.Message("L4307"), (float)Math.Round(CommonUtility.ToMetric(hra.Goals.OptimalLdl.Value, BioLookup.LDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) + " " + Measurements[BioLookup.LDL].MeasurementUnit)),
					string.Format(Translate.Message("L4308"), (float)Math.Round(CommonUtility.ToMetric(hra.Goals.LtHdl.Value, BioLookup.HDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) + " " + Measurements[BioLookup.HDL].MeasurementUnit, ((float)Math.Round(CommonUtility.ToMetric(60, BioLookup.HDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) + " " + Measurements[BioLookup.HDL].MeasurementUnit)),
					string.Format(Translate.Message("L4309"), (float)Math.Round(CommonUtility.ToMetric(150, BioLookup.Triglycerides, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) + " " + Measurements[BioLookup.Triglycerides].MeasurementUnit),
					string.Format(Translate.Message("L4311"), HasDiabetes() ? 7 : 5.7)
				};
				cvdRisk.healthyLifestyle = new List<string>
				{
					string.Format(Translate.Message("L4312"), 150),
					Translate.Message("L4424"),
					Translate.Message("L4314"),
					string.Format(Translate.Message("L4315"), HttpContext.Session.GetInt32(SessionContext.Gender) == 1 ? Translate.Message("L4317") : Translate.Message("L4316")),
					Translate.Message("L4318"),
					Translate.Message("L4319")
				};
				cvdRisk.selfManagement = new List<string>
				{
					Translate.Message("L4379"),
					Translate.Message("L4321"),
					Translate.Message("L4322"),
					Translate.Message("L4376")
				};
				cvdRisk.regularMedicalCare = new List<string>
				{
					Translate.Message("L4324"),
					Translate.Message("L4325"),
					Translate.Message("L4326"),
					Translate.Message("L4327")
				};
				model.cvdRisk = cvdRisk;
			}
			else if (HttpContext.Session.GetInt32(SessionContext.CarePlanType).HasValue && HttpContext.Session.GetInt32(SessionContext.CarePlanType) == (byte)CarePlanTypes.WeightManagement)
			{
				//cvdRisk
				var hra = ReportUtility.ReadHRAReport(HttpContext.Session.GetInt32(SessionContext.HRAId).Value).hra;
				SummaryCarePlanModel weightManagement = new SummaryCarePlanModel();
				weightManagement.numbersandGoals = new List<string>
				{
					Translate.Message("L4591"),
					Translate.Message("L4592"),
					string.Format(Translate.Message("L4304"), "130"),
					Translate.Message("L4305"),
					string.Format(Translate.Message("L4306"), (float)Math.Round(CommonUtility.ToMetric(hra.Goals.LtLdl.Value, BioLookup.LDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) + " " + Measurements[BioLookup.LDL].MeasurementUnit, string.Format(Translate.Message("L4307"), (float)Math.Round(CommonUtility.ToMetric(hra.Goals.OptimalLdl.Value, BioLookup.LDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) + " " + Measurements[BioLookup.LDL].MeasurementUnit)),
					string.Format(Translate.Message("L4308"), (float)Math.Round(CommonUtility.ToMetric(hra.Goals.LtHdl.Value, BioLookup.HDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) + " " + Measurements[BioLookup.HDL].MeasurementUnit, ((float)Math.Round(CommonUtility.ToMetric(60, BioLookup.HDL, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) + " " + Measurements[BioLookup.HDL].MeasurementUnit)),
					string.Format(Translate.Message("L4309"), (float)Math.Round(CommonUtility.ToMetric(150, BioLookup.Triglycerides, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) + " " + Measurements[BioLookup.Triglycerides].MeasurementUnit),
					string.Format(Translate.Message("L4311"), HasDiabetes() ? 7 : 5.7)
				};
				weightManagement.healthyLifestyle = new List<string>
				{
					string.Format(Translate.Message("L4312"), 150),
					Translate.Message("L4424"),
					Translate.Message("L4314"),
					string.Format(Translate.Message("L4315"), HttpContext.Session.GetInt32(SessionContext.Gender) == 1 ? Translate.Message("L4317") : Translate.Message("L4316")),
					Translate.Message("L4318"),
					Translate.Message("L4319")
				};
				weightManagement.selfManagement = new List<string>
				{
					Translate.Message("L4379"),
					Translate.Message("L4321"),
					Translate.Message("L4322"),
					Translate.Message("L4376")
				};
				weightManagement.regularMedicalCare = new List<string>
				{
					Translate.Message("L4324"),
					Translate.Message("L4325"),
					Translate.Message("L4326"),
					Translate.Message("L4327")
				};
				model.weightManagement = weightManagement;
			}
			return model;
		}

		[Authorize]
		public ActionResult ExerciseRecommendation(bool fromKit)
		{
			PhysicalActivityGoalModel model = new PhysicalActivityGoalModel();
			var hra = HRAUtility.ReadHRA(HttpContext.Session.GetInt32(SessionContext.HRAId).Value).hra;
			model = ReportUtility.PhysicalActivityGoal(hra, null, HttpContext.Session.GetInt32(SessionContext.ProgramType));
			if (fromKit)
			{
				TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
				model.HRADate = TimeZoneInfo.ConvertTimeFromUtc(hra.CompleteDate.Value, custTZone).ToShortDateString();
			}
			model.hasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
			return PartialView("_PhysicalActivityGoal", model);
		}

		[Authorize]
		public ActionResult StressRecommendation()
		{
			StressGoalModel model = new StressGoalModel();
			model.stressGoalContent = ReportUtility.GetStressGoalContent(null, HttpContext.Session.GetInt32(SessionContext.ProgramType));
			return PartialView("_StressGoal", model);
		}

		[Authorize]
		public ActionResult MedicationRecommendation()
		{
			MedicationGoal model = new MedicationGoal();
			model.medicationGoalContent = ReportUtility.GetMedicationGoalContent(null, HttpContext.Session.GetInt32(SessionContext.ProgramType));
			return PartialView("_MedicationGoal", model);
		}

		[Authorize]
		public ActionResult TobaccoRecommendation()
		{
			TobaccoGoalModel model = new TobaccoGoalModel();
			var response = ReportUtility.ReadHRAReport(HttpContext.Session.GetInt32(SessionContext.HRAId).Value);
			model.tobaccoGoalContent = ReportUtility.GetTobaccoGoalContent(response);
			return PartialView("_TobaccoGoal", model);
		}

		[Authorize]
		public ActionResult ExamsRecommendation()
		{
			var response = ReportUtility.ReadHRAReport(HttpContext.Session.GetInt32(SessionContext.HRAId).Value);
			ExamsGoalModel model = new ExamsGoalModel();
			AgeInfo ageInfo = new AgeInfo();
			model.checkupsandGoals = ReportUtility.GetCheckupsandGoals(response.hra.Exams, response.hra.HealthNumbers, response.hra.User, response.hra.Age.Value);
			model.immunizationandGoals = ReportUtility.GetImmunizationandGoals(response.hra.Exams, response.hra.HealthNumbers, response.hra.User, response.hra.Age.Value);
			return PartialView("_ExamsGoal", model);
		}

		[Authorize]
		public ActionResult SupplementRecommendation()
		{
			SupplementGoalModel model = new SupplementGoalModel();
			model.supplementGoalContent = ReportUtility.GetSupplementGoalContent();
			return PartialView("_SupplementGoal", model);
		}

		[Authorize]
		public ActionResult Referrals()
		{
			DrReferralModel model = new DrReferralModel();
			var Goals = ReportUtility.ReadHRAGoals(HttpContext.Session.GetInt32(SessionContext.HRAId).Value).hraGoals;
			model = ReportUtility.DrReferral(Goals, HttpContext.Session.GetInt32(SessionContext.HRAVer), null);
			return PartialView("_Referrals", model);
		}

		[Authorize]
		public ActionResult ShowVideo(string url)
		{
			return PartialView("_Video", url);
		}

		#endregion

		#region FollowUp Report

		[Authorize]
		public ActionResult FollowupReportDashboard()
		{
			if (!HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue)
				return RedirectToAction("FollowUpDashboard", "FollowUp");
			return View(GetFollowupReportModel());
		}
		public FollowupReport GetFollowupReportModel()
		{
			FollowupReport model = new FollowupReport();
			ViewBag.ProgramType = HttpContext.Session.GetInt32(SessionContext.ProgramType).Value;
			int followupId = HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value;
			string timeZone = !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) ? HttpContext.Session.GetString(SessionContext.ParticipantTimeZone) : User.TimeZone();
			var response = ReportUtility.GetFollowupReportDashboard(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, followupId, HttpContext.Session.GetInt32(SessionContext.HRAVer).Value, HttpContext.Session.GetInt32(SessionContext.Unit).Value, timeZone);
			response.SurveyQuestions = ParticipantUtility.GetSurveyQuestions();
			response.IsSurveyCompleted = ParticipantUtility.GetSurveyCompletedStatus(HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value).PartiallyCompleted;
			model = response;
			if (CommonUtility.HasAdminRole(User.RoleCode()) && HttpContext.Session.GetString(SessionContext.NeedCareplanApproval) != null && Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.NeedCareplanApproval)) == true)
			{
				var carePlanReportResponse = ReportUtility.GetCarePlanReport((int)ReportTypes.FollowUp, HttpContext.Session.GetInt32(SessionContext.FollowUpId).ToString()).carePlanReport;
				if (carePlanReportResponse != null)
				{
					TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
					model.reportGenerated = carePlanReportResponse.ReportGenerated.Value;
					model.reportGeneratedBy = carePlanReportResponse.User.FirstName + " " + carePlanReportResponse.User.LastName;
					model.reportGeneratedOn = TimeZoneInfo.ConvertTimeFromUtc(carePlanReportResponse.CreatedOn.Value, custTZone).ToShortDateString();
				}
			}
			model.hasAdminRole = CommonUtility.HasAdminRole(User.RoleCode());
			model.needCareplanApproval = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.NeedCareplanApproval) != null ? HttpContext.Session.GetString(SessionContext.NeedCareplanApproval) : false);
			model.gender = HttpContext.Session.GetInt32(SessionContext.Gender);
			if (model.ProgramStartDate.HasValue)
				model.programStartDateFormat = CommonUtility.dateFormater(model.ProgramStartDate.Value, false, HttpContext.Session.GetString(SessionContext.DateFormat));
			model.dateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
			model.orgContactEmail = HttpContext.Session.GetString(SessionContext.OrgContactEmail);
			model.dob = HttpContext.Session.GetString(SessionContext.DOB);
			return model;
		}
		#endregion

		[ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
		public ActionResult SendReports()
		{
			SendReportsModel model = new SendReportsModel();
			model.reportTypes = CommonUtility.ListReportTypes(HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue ? HttpContext.Session.GetInt32(SessionContext.HRAId).Value : null, HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue ? HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value : null, Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.CarePlan) != null ? HttpContext.Session.GetString(SessionContext.CarePlan) : false)).Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() });
			model.faxedReports = ReportUtility.GetFaxedReports(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).faxedReports;
			model.hasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
			var userDoctorInfo = AccountUtility.GetUserDoctorInfo(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
			if (!string.IsNullOrEmpty(userDoctorInfo.FaxNumber))
			{
				model.FaxNumber = Regex.Replace(userDoctorInfo.FaxNumber, "[^0-9.]", "");
				if (model.FaxNumber.Length == 10)
					model.FaxNumber = "1" + model.FaxNumber;
			}
			return PartialView("_SendReports", model);
		}

		[Authorize]
		public async Task<ActionResult> PrintableReportsAsync(string view, bool generatePDF, bool? sendFax, string faxNumber, int? awvReportId, int? kitId, string pageIdentifier, int? kitsInUserProgramsId, string languageCode, string kitColor)
		{
			ViewDataDictionary viewData;
			var modelState = new ModelStateDictionary();
			int type = 0;
			ReportTypeDto reportType = null;
			string refId = "";
			if (sendFax.HasValue && sendFax.Value)
			{
				faxNumber = Regex.Replace(faxNumber, "[^0-9.]", "");
				if (faxNumber.Length == 10)
					faxNumber = "1" + faxNumber;
				reportType = CommonUtility.ListReportTypes(HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue ? HttpContext.Session.GetInt32(SessionContext.HRAId).Value : null, HttpContext.Session.GetInt32(SessionContext.FollowUpId).HasValue ? HttpContext.Session.GetInt32(SessionContext.FollowUpId).Value : null, Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.CarePlan) != null ? HttpContext.Session.GetString(SessionContext.CarePlan) : false)).Where(x => x.Id == Convert.ToInt16(view)).FirstOrDefault();
				view = reportType.ReportName;
			}
			switch (view)
			{
				case "DrSummary":
					viewData = new ViewDataDictionary(_metadataProvider, modelState) { Model = GetDrSummaryModel() };
					type = (int)ReportTypes.HRA;
					refId = HttpContext.Session.GetInt32(SessionContext.HRAId).ToString();
					break;
				case "FollowupReportDashboard":
					viewData = new ViewDataDictionary(_metadataProvider, modelState) { Model = GetFollowupReportModel() };
					type = (int)ReportTypes.FollowUp;
					refId = HttpContext.Session.GetInt32(SessionContext.FollowUpId).ToString();
					break;
				case "PrintableDetailedPlan":
					viewData = new ViewDataDictionary(_metadataProvider, modelState) { Model = GetPrintableDetailedPlan() };
					type = (int)ReportTypes.HRA;
					refId = HttpContext.Session.GetInt32(SessionContext.HRAId).ToString();
					break;
				case "MyActionPlan":
					viewData = new ViewDataDictionary(_metadataProvider, modelState) { Model = GetRecommendationsDashboardModel() };
					break;
				case "AWVReport":
					viewData = new ViewDataDictionary(_metadataProvider, modelState) { Model = GetAWVReportDetails(awvReportId.Value) };
					break;
				case "KitStepDetails":
					viewData = new ViewDataDictionary(_metadataProvider, modelState) { Model = KitUtility.GetKitByIdentifier(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, kitId.Value, pageIdentifier, kitsInUserProgramsId.Value, languageCode) };
					break;

				default:
					viewData = new ViewDataDictionary(_metadataProvider, modelState);
					break;
			}
			// The string writer where to render the HTML code of the view
			StringWriter stringWriter = new StringWriter();

			viewData.Add("isPdf", true);
			ViewEngineResult viewResult;
			if (view == "KitStepDetails")
			{
				viewData.Add("kitColor", kitColor);
				viewResult = _viewEngine.FindView(ControllerContext, "../Kit/_KitStepDetails", false);
			}
			else
				viewResult = _viewEngine.FindView(ControllerContext, view, true);

			ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, viewData, new TempDataDictionary(new DefaultHttpContext(), _tempDataProvider), stringWriter, new HtmlHelperOptions());
			viewResult.View.RenderAsync(viewContext);

			string baseUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host.Value, Url.Content("~"));

			License.LicenseKey = _appSettings.IronPdfLicenseKey;
			var MyTempPath = _appSettings.PDFReportPath;
			Environment.SetEnvironmentVariable("TEMP", MyTempPath);
			Environment.SetEnvironmentVariable("TMP", MyTempPath);
			Installation.TempFolderPath = Path.Combine(MyTempPath, "IronPdfTemp");

			var Renderer = new ChromePdfRenderer();
			Renderer.RenderingOptions.MarginTop = 6;
			Renderer.RenderingOptions.MarginBottom = 18;
			Renderer.RenderingOptions.MarginLeft = 5;
			Renderer.RenderingOptions.MarginRight = 5;
			Renderer.RenderingOptions.CssMediaType = IronPdf.Rendering.PdfCssMediaType.Print;
			Renderer.RenderingOptions.PrintHtmlBackgrounds = true;
			Renderer.RenderingOptions.EnableJavaScript = true;
			Renderer.RenderingOptions.Timeout = 120; // seconds 
			Renderer.RenderingOptions.RenderDelay = 6000; //milliseconds=30seconds
			Renderer.RenderingOptions.TextFooter.DrawDividerLine = true;
			Renderer.RenderingOptions.TextFooter.RightText = "© " + @DateTime.Now.Year + " INTERVENT. All rights reserved.";

			var result = Renderer.RenderHtmlAsPdfAsync(stringWriter.ToString(), new Uri(baseUrl));
			if (generatePDF)
			{
				var Id = HttpContext.Session.GetString(SessionContext.UniqueId) != null ? HttpContext.Session.GetString(SessionContext.UniqueId) : HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value.ToString();
				Renderer.RenderHtmlAsPdf(stringWriter.ToString(), baseUrl).SaveAs(HttpContext.Session.GetString(SessionContext.CareplanPath) + "" + Id + "_" + DateTime.UtcNow.ToString("yyyyMMdd") + ".pdf");
				var response = ReportUtility.AddEditCarePlanReport(type, refId, true, Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value));
				TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
				var confMessage = response.userName + " " + string.Format(Translate.Message("L4366"), TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone).ToShortDateString());
				return Json(new { Result = response.status, confMessage = confMessage });
			}
			else if (sendFax.HasValue && sendFax.Value)
			{
				var response = new SFaxManager().SendFax(result.Result.Stream, HttpContext.Session.GetString(SessionContext.ParticipantName), faxNumber, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, reportType.Type, _appSettings.SFaxAPIUrl, _appSettings.SFaxAPIKey, _appSettings.SFaxUsername);
				if (response.isSuccess)
				{
					FaxedReportsDto faxedReport = new FaxedReportsDto();
					faxedReport.UserId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
					faxedReport.SentBy = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
					faxedReport.SentOn = DateTime.UtcNow;
					faxedReport.FaxNumber = faxNumber;
					faxedReport.RefId = refId;
					faxedReport.ReportType = reportType.Id;
					faxedReport.IsSent = response.isSuccess;
					ReportUtility.AddFaxedReport(faxedReport);
				}
				return Json(new { response });
			}
			else
				return new FileContentResult(result.Result.Stream.ToArray(), "application/pdf");
		}

		public JsonResult SaveReportFeedback(byte type, string comments)
		{
			bool status = ReportUtility.SaveReportFeedback(type, comments, HttpContext.Session.GetInt32(SessionContext.HRAId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
			return Json(status);
		}

		[Authorize]
		public ActionResult ViewReport(int id)
		{
			return View("AWVReport", GetAWVReportDetails(id));
		}

		public AWVReportModel GetAWVReportDetails(int id)
		{
			var goal = AWVUtility.GetAWVReportDetails(id);
			AWVReportModel model = new AWVReportModel(goal);
			model.Token = goal.PrintToken;
			HttpContext.Session.SetInt32("AWVId", model.AWVId);
			HttpContext.Session.SetString("UserName", model.User.UserName);
			return model;
		}

		[Authorize]
		public ActionResult KitStepDetails(int kitId, string pageIdentifier, int kitsInUserProgramsId, string languageCode)
		{
			var response = GetKitStepDetails(kitId, pageIdentifier, kitsInUserProgramsId, languageCode);
			response.kitId = kitId;
			response.pageIdentifier = pageIdentifier;
			response.languageCode = languageCode;
			return PartialView("../Kit/_KitStepDetails", response);
		}

		public ReadPageResponse GetKitStepDetails(int kitId, string pageIdentifier, int kitsInUserProgramsId, string languageCode)
		{
			return KitUtility.GetKitByIdentifier(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, kitId, pageIdentifier, kitsInUserProgramsId, languageCode);
		}
	}
}