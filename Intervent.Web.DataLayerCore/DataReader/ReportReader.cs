using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class ReportReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public ReadHRAGoalsResponse ReadHRAGoals(ReadHRAGoalsRequest request)
        {
            ReadHRAGoalsResponse response = new ReadHRAGoalsResponse();

            var hra = context.HRA_Goals.FirstOrDefault(x => x.Id == request.hraId);

            response.hraGoals = Utility.mapper.Map<DAL.HRA_Goals, HRAGoalsDto>(hra); ;

            return response;
        }

        public CarePlanReportResponse GetCarePlanReport(CarePlanReportRequest request)
        {
            CarePlanReportResponse response = new CarePlanReportResponse();
            var carePlanReport = context.CarePlanReports.Include("User").Where(x => x.Type == request.type && x.RefId == request.refId).FirstOrDefault();
            response.carePlanReport = Utility.mapper.Map<DAL.CarePlanReport, CarePlanReportDto>(carePlanReport);
            return response;
        }

        public AddEditCarePlanReportsResponse AddEditCarePlanReport(AddEditCarePlanReportsRequest request)
        {
            AddEditCarePlanReportsResponse response = new AddEditCarePlanReportsResponse();
            var carePlanReport = Utility.mapper.Map<CarePlanReportDto, CarePlanReport>(request.carePlanReport);
            context.CarePlanReports.Add(carePlanReport);
            context.SaveChanges();
            response.status = true;
            return response;
        }

        public ListKeyActionStepsResponse KeyActionSteps(ListKeyActionStepsRequest request)
        {
            ListKeyActionStepsResponse response = new ListKeyActionStepsResponse();
            var actionSteps = context.HRA_ActionSteps.Include("ActionStepType").Where(x => x.HRAId == request.hraId && x.Score != 0).OrderBy(x => x.ActionStepType.Type)
                .ThenBy(x => x.Score).ToList();
            if (actionSteps != null)
            {
                List<ActionStep> lifetimeActionSteps = new List<ActionStep>();
                List<ActionStep> gapActionSteps = new List<ActionStep>();
                for (int i = 0; i < actionSteps.Count; i++)
                {
                    if (actionSteps[i].ActionStepType.Type == "Lifestyle")
                    {
                        ActionStep lifetimeActionStep = new ActionStep();
                        if (actionSteps[i].IsNull == false)
                            lifetimeActionStep.name = actionSteps[i].ActionStepType.Statement;
                        else
                            lifetimeActionStep.name = actionSteps[i].ActionStepType.StatementIfNull;
                        lifetimeActionSteps.Add(lifetimeActionStep);
                    }
                    else
                    {
                        ActionStep gapActionStep = new ActionStep();
                        if (actionSteps[i].IsNull == false)
                            gapActionStep.name = actionSteps[i].ActionStepType.Statement;
                        else
                            gapActionStep.name = actionSteps[i].ActionStepType.StatementIfNull;
                        gapActionSteps.Add(gapActionStep);
                    }
                }

                response.lifetimeActionSteps = lifetimeActionSteps;
                response.gapActionSteps = gapActionSteps;
            }
            return response;
        }

        public ConditionsNeedHelpResponse GetConditionsNeedHelp(ConditionsNeedHelpRequest request)
        {
            //select top 5 records
            ConditionsNeedHelpResponse response = new ConditionsNeedHelpResponse();
            var conditions = context.HRA_ActionSteps.Include("ActionStepType").Where(x => x.HRAId == request.hraId
            && ((x.ActionStepType.RiskFactor == "Diabetes") || !x.IsNull) && !string.IsNullOrEmpty(x.ActionStepType.HelpStatement)).ToList();
            var filteredConditions = conditions.Where(x => x.ActionStepType.RiskFactor == "Diabetes").ToList().Union(conditions.Where(x => x.ActionStepType.RiskFactor != "Diabetes").OrderBy(x => x.Score).Take(4).ToList());
            List<HRA_ActionStepsDto> actionSteps = new List<HRA_ActionStepsDto>();
            foreach (var cond in filteredConditions)
            {
                HRA_ActionStepsDto actionStep = new HRA_ActionStepsDto();
                ActionStepTypeDto actionStepType = new ActionStepTypeDto();
                actionStep.HRAId = cond.HRAId;
                actionStepType.HelpStatement = cond.ActionStepType.HelpStatement;
                actionStep.ActionStepType = actionStepType;
                actionSteps.Add(actionStep);
            }
            response.actionSteps = actionSteps;
            return response;
        }

        public CompareWellnessScoreResponse GetAvgWellnessScore(CompareWellnessScoreRequest request)
        {
            CompareWellnessScoreResponse response = new CompareWellnessScoreResponse();
            StoredProcedures sp = new StoredProcedures();
            var score = sp.GetAvgWellnessScore(request.hraId);
            if (score.HasValue)
                response.score = score.Value;
            else
                response.score = 0;
            return response;
        }

        public ReadFollowupReportResponse GetFollowupReportDashboard(ReadFollowupReportRequest request)
        {
            ReadFollowupReportResponse response = new ReadFollowupReportResponse();

            var followup = context.FollowUps.Include("UsersinProgram").Include("UsersinProgram.User").Include("UsersinProgram.User.Organization").Include("UsersinProgram.User.State1").Include("UsersinProgram.ProgramsinPortal").Include("UsersinProgram.ProgramsinPortal.Portal").Include("UsersinProgram.ProgramsinPortal.ApptCallTemplate")
                .Include("UsersinProgram.HRA").Include("UsersinProgram.HRA.HRA_HealthNumbers").Include("UsersinProgram.HRA.HRA_Goals").Include("UsersinProgram.HRA.HRA_OtherRiskFactors")
                .Include("UsersinProgram.KitsinUserPrograms").Include("UsersinProgram.KitsinUserPrograms.Kit").Include("UsersinProgram.KitsinUserPrograms.Kit.KitTranslations").Include("UsersinProgram.User.Notes").Include("FollowUp_Goals")
                .Include("FollowUp_HealthNumbers").Include("FollowUp_HealthNumbers.Lab").Include("FollowUp_OtherRiskFactors").Include("UsersinProgram.FollowUps.FollowUp_Goals")
                .Include("UsersinProgram.FollowUps.FollowUp_HealthNumbers").Include("UsersinProgram.FollowUps.FollowUp_HealthNumbers.Lab").Include("UsersinProgram.FollowUps.FollowUp_OtherRiskFactors")
                .Where(x => x.Id == request.followupId).FirstOrDefault();

            if (followup != null)
            {
                var user = followup.UsersinProgram.User;
                var userinProgram = followup.UsersinProgram;
                var lab = followup.FollowUp_HealthNumbers.Lab;
                var programinPortal = followup.UsersinProgram.ProgramsinPortal;
                var completeDate = followup.UsersinProgram.HRA.CompleteDate;
                var TenYrProbStart = followup.UsersinProgram.HRA.HRA_Goals.TenYrProb;
                var TenYearASCVD = followup.UsersinProgram.HRA.HRA_Goals.TenYearASCVD;
                var LifetimeASCVD = followup.UsersinProgram.HRA.HRA_Goals.LifetimeASCVD;
                var DidYouFast = followup.UsersinProgram.HRA.HRA_HealthNumbers.DidYouFast;
                var TotalChol = followup.UsersinProgram.HRA.HRA_HealthNumbers.TotalChol;
                var HDL = followup.UsersinProgram.HRA.HRA_HealthNumbers.HDL;
                var LDL = followup.UsersinProgram.HRA.HRA_HealthNumbers.LDL;
                var Trig = followup.UsersinProgram.HRA.HRA_HealthNumbers.Trig;
                var Weight = followup.UsersinProgram.HRA.HRA_HealthNumbers.Weight;
                var SBP = followup.UsersinProgram.HRA.HRA_HealthNumbers.SBP;
                var DBP = followup.UsersinProgram.HRA.HRA_HealthNumbers.DBP;
                var Glucose = followup.UsersinProgram.HRA.HRA_HealthNumbers.Glucose;
                var A1C = followup.UsersinProgram.HRA.HRA_HealthNumbers.A1C;
                var NoOfCig = followup.UsersinProgram.HRA.HRA_OtherRiskFactors.NoOfCig;


                response.UserName = followup.UsersinProgram.User.FirstName + " " + followup.UsersinProgram.User.LastName;
                response.Address = followup.UsersinProgram.User.Address;
                response.Address2 = followup.UsersinProgram.User.Address2;
                response.City = followup.UsersinProgram.User.City;
                response.State = followup.UsersinProgram.User.State1.Name;
                response.Zip = followup.UsersinProgram.User.Zip;
                response.Picture = followup.UsersinProgram.User.Picture;
                response.DOB = followup.UsersinProgram.User.DOB;
                response.ProgramStartDate = followup.UsersinProgram.StartDate;
                response.AssessmentDate = followup.CompleteDate;
                response.BloodTestDate = followup.FollowUp_HealthNumbers.BloodTestDate;
                response.FollowUpValidity = programinPortal.Portal.FollowUpValidity;
                if (followup.FollowUp_HealthNumbers.Lab != null && followup.FollowUp_HealthNumbers.Lab.LabSelection.HasValue)
                {
                    response.FollowupLabSource = followup.FollowUp_HealthNumbers.Lab.LabSelection.Value == (int)LabChoices.DoctorsOffice ? "L3155" : followup.FollowUp_HealthNumbers.Lab.LabSelection.Value == (int)LabChoices.LabCorp ? (user.Organization.IntegrationWith == 3 ? "L3580" : "L3156") : "";
                }
                if (followup.UsersinProgram.ProgramsinPortal.ApptCallTemplate != null)
                    response.ScheduledCoachingSession = followup.UsersinProgram.ProgramsinPortal.ApptCallTemplate.NoOfCalls;
                response.TenYrProbStart = TenYrProbStart;
                response.TenYrProbCurrent = followup.FollowUp_Goals.TenYrProb;
                response.PreviousDate = DateTime.Parse(completeDate.ToString()).Month.ToString() + "/" + DateTime.Parse(completeDate.ToString()).Year.ToString();
                response.CurrentDate = DateTime.Parse(followup.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(followup.CompleteDate.ToString()).Year.ToString();
                response.TenYrLowGoal = followup.FollowUp_Goals.TenYrLow;
                response.TenYearASCVDStart = TenYearASCVD;
                response.TenYearASCVDCurrent = followup.FollowUp_Goals.TenYearASCVD;
                response.TenYearASCVDGoal = followup.FollowUp_Goals.TenYearASCVDGoal;
                response.LifetimeASCVDStart = LifetimeASCVD;
                response.LifetimeASCVDCurrent = followup.FollowUp_Goals.LifetimeASCVD;
                response.LifetimeASCVDGoal = followup.FollowUp_Goals.LifetimeASCVDGoal;
                response.DidYouFastStart = DidYouFast;
                response.DidYouFastCurrent = followup.FollowUp_HealthNumbers.DidYouFast;
                response.TotalCholStart = TotalChol;
                response.TotalCholCurrent = followup.FollowUp_HealthNumbers.TotalChol;
                response.HDLStart = HDL;
                response.HDLCurrent = followup.FollowUp_HealthNumbers.HDL;
                response.HDLGoal = followup.FollowUp_Goals.LtHdl;
                response.LDLStart = LDL;
                response.LDLCurrent = followup.FollowUp_HealthNumbers.LDL;
                response.LDLGoal = followup.FollowUp_Goals.LtLdl;
                response.TrigStart = Trig;
                response.TrigCurrent = followup.FollowUp_HealthNumbers.Trig;
                response.TrigGoal = followup.FollowUp_Goals.LtTrig;
                response.WeightStart = Weight;
                response.WeightCurrent = followup.FollowUp_HealthNumbers.Weight;
                response.WeightSTGoal = followup.FollowUp_Goals.StWt;
                response.WeightLTGoal = followup.FollowUp_Goals.LtWt;
                response.Height = followup.FollowUp_HealthNumbers.Height;
                response.SBPStart = SBP;
                response.SBPCurrent = followup.FollowUp_HealthNumbers.SBP;
                response.SBPGoal = followup.FollowUp_Goals.LtSbp;
                response.DBPStart = DBP;
                response.DBPCurrent = followup.FollowUp_HealthNumbers.DBP;
                response.DBPGoal = followup.FollowUp_Goals.LtDbp;
                response.GlucoseStart = Glucose;
                response.GlucoseCurrent = followup.FollowUp_HealthNumbers.Glucose;
                response.GlucoseGoal1 = followup.FollowUp_Goals.LtGluc1;
                response.GlucoseGoal2 = followup.FollowUp_Goals.LtGluc2;
                response.A1CStart = A1C;
                response.A1CCurrent = followup.FollowUp_HealthNumbers.A1C;
                response.A1CGoal = followup.FollowUp_Goals.LtA1c;
                response.tobaccoStart = NoOfCig;
                response.tobaccoCurrent = followup.FollowUp_OtherRiskFactors.NoOfCig;
                response.Diabetes = followup.FollowUp_Goals.Diabetes;
                response.CholRef = followup.FollowUp_Goals.CholRef;
                response.BPRef = followup.FollowUp_Goals.BPRef;
                response.ElevatedBPRef = followup.FollowUp_Goals.ElevatedBPRef;
                response.HypertensiveBPRef = followup.FollowUp_Goals.HypertensiveBPRef;
                response.LdlRef1 = followup.FollowUp_Goals.LdlRef1;
                response.LdlRef2 = followup.FollowUp_Goals.LdlRef2;
                response.HdlRef = followup.FollowUp_Goals.HdlRef;
                response.ASCVDRef = followup.FollowUp_Goals.ASCVDRef;
                response.TrigRef1 = followup.FollowUp_Goals.TrigRef1;
                response.TrigRef2 = followup.FollowUp_Goals.TrigRef2;
                response.DiabRef = followup.FollowUp_Goals.DiabRef;
                response.NicRef = followup.FollowUp_Goals.NicRef;
                response.AspRef = followup.FollowUp_Goals.AspRef;
                //Assigned kits
                response.AssignedKits = new List<AssignedKit>();
                var kits = (followup.UsersinProgram.KitsinUserPrograms.Where(c => c.IsActive == true && c.StartDate <= followup.CompleteDate && c.Kit.Topic != (int)KitTopic.Questionnaires)).ToList();
                if (kits != null && kits.Count > 0)
                {
                    TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
                    for (int i = 0; i < kits.Count; i++)
                    {
                        AssignedKit readfollowup = new AssignedKit();
                        readfollowup.EducationalTopic = kits[i].Kit.Name;
                        readfollowup.DateAssigned = TimeZoneInfo.ConvertTimeFromUtc(kits[i].StartDate, timeZone);
                        readfollowup.PercentCompleted = kits[i].PercentCompleted;
                        if (userinProgram.Language != ListOptions.DefaultLanguage)
                        {
                            var kitTranslation = kits[i].Kit.KitTranslations.Where(x => x.LanguageCode == followup.UsersinProgram.Language).FirstOrDefault();
                            if (kitTranslation != null)
                            {
                                readfollowup.EducationalTopic = kitTranslation.Name;
                            }
                        }
                        response.AssignedKits.Add(readfollowup);
                    }
                }
                //completed sessions
                response.CompletedCoachingSession = followup.UsersinProgram.User.Notes.Where(x => (x.RefId == followup.UsersinProgramsId || (x.Type == (byte)NoteTypes.BiometricReview && x.PortalId == followup.UsersinProgram.ProgramsinPortal.PortalId)) && x.NotesDate <= followup.CompleteDate).Count();
            }
            response.IsSurveyCompleted = false;
            var surveyCount = context.SurveyResponses.Where(x => x.UsersinProgramsId == followup.UsersinProgramsId).FirstOrDefault();
            if (surveyCount != null && surveyCount.UsersinProgramsId > 0)
                response.IsSurveyCompleted = true;

            if (followup.UsersinProgram != null && followup.UsersinProgram.StartDate != null && followup.CompleteDate != null && followup.CompleteDate != null)
            {
                var weeks = Convert.ToInt32((Convert.ToDateTime(followup.CompleteDate) - Convert.ToDateTime(followup.UsersinProgram.StartDate)).TotalDays / 7);
                response.NoOfWeeks = weeks;
            }
            return response;
        }

        public List<SurveyQuestionDto> GetSurveyQuestions()
        {
            var surveyList = context.SurveyQuestions.Where(x => x.IsActive).ToList();
            return Utility.mapper.Map<List<DAL.SurveyQuestion>, List<SurveyQuestionDto>>(surveyList);
        }

        public GetSurveyCompletedResponse GetSurveyCompletedStatus(GetSurveyCompletedRequest request)
        {
            GetSurveyCompletedResponse response = new GetSurveyCompletedResponse();
            var survey = context.SurveyResponses.Include("UsersinProgram").Where(x => x.UsersinProgramsId == request.usersinProgramId).ToList();
            if (survey != null && survey.Count > 0)
            {
                response.PartiallyCompleted = true;
                response.surveyResponse = Utility.mapper.Map<List<DAL.SurveyResponse>, List<SurveyResponseDto>>(survey);
                if (survey.Count == 8)
                    response.SurveyCompleted = true;
                response.Comments = survey.FirstOrDefault().UsersinProgram.UserComments;
            }
            return response;
        }

        public bool SaveReportFeedback(AddEditReportFeedbackRequest request)
        {
            bool status = false;
            var ReportFeedback = context.ReportFeedbacks.Where(x => x.HRAId == request.HRAId && x.Type == request.Type).FirstOrDefault();
            if (ReportFeedback != null)
            {
                ReportFeedback.UpdatedBy = request.CreatedBy;
                ReportFeedback.UpdatedOn = DateTime.UtcNow;
                ReportFeedback.Comments = request.Comments;
                context.ReportFeedbacks.Attach(ReportFeedback);
                context.Entry(ReportFeedback).State = EntityState.Modified;
                context.SaveChanges();
                status = true;
            }
            else
            {
                DAL.ReportFeedback reportFeedback = new DAL.ReportFeedback
                {
                    HRAId = request.HRAId,
                    Type = request.Type,
                    Comments = request.Comments,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy
                };
                context.ReportFeedbacks.Add(reportFeedback);
                context.SaveChanges();
                status = true;
            }
            return status;
        }

        public List<BillingServiceTypeDto> GetBillingServiceTypes()
        {
            var ServiceTypes = context.BillingServiceTypes.ToList();
            return Utility.mapper.Map<List<DAL.BillingServiceType>, List<BillingServiceTypeDto>>(ServiceTypes);
        }

        public bool AddInvoiceDetails(InvoiceDetailsRequest request)
        {
            InvoiceDetail invoice = context.InvoiceDetails.Where(x => x.UserId == request.UserId && x.Type == request.Type && x.CreatedOn.Month == DateTime.UtcNow.Month && x.CreatedOn.Year == DateTime.UtcNow.Year).FirstOrDefault();
            if (invoice == null)
            {
                var serviceType = GetBillingServiceTypes();
                InvoiceBilledDetail invoiceBilling = context.InvoiceBilledDetails.Where(x => x.UserId == request.UserId && x.CreatedOn.Month == DateTime.UtcNow.Month && x.CreatedOn.Year == DateTime.UtcNow.Year && x.Billed == false).FirstOrDefault();
                if (invoiceBilling == null)
                {
                    invoiceBilling = new InvoiceBilledDetail
                    {
                        UserId = request.UserId,
                        Billed = false,
                        Total = serviceType.Where(x => x.Id == request.Type).Select(x => x.Price).FirstOrDefault(),
                        CreatedOn = DateTime.UtcNow,
                    };
                    invoiceBilling = context.InvoiceBilledDetails.Add(invoiceBilling).Entity;
                    context.SaveChanges();
                }
                else
                {
                    invoiceBilling.Total = invoiceBilling.Total + serviceType.Where(x => x.Id == request.Type).Select(x => x.Price).FirstOrDefault();
                    context.InvoiceBilledDetails.Attach(invoiceBilling);
                    context.Entry(invoiceBilling).State = EntityState.Modified;
                    context.SaveChanges();
                }

                invoice = new InvoiceDetail
                {
                    UserId = request.UserId,
                    InvoiceId = invoiceBilling.Id,
                    Type = request.Type,
                    CreatedOn = DateTime.UtcNow,
                };
                context.InvoiceDetails.Add(invoice);
                context.SaveChanges();
            }
            return true;
        }

        public bool AddMonitorChargeToInvoiceDetails()
        {
            AccountReader accountReader = new AccountReader();
            var service = GetBillingServiceTypes();
            var monitorCharge = service.Where(x => x.Type == CommonReader.BillingServiceTypes.MonitorAndSupplies).FirstOrDefault();
            var healthCoaching = service.Where(x => x.Type == CommonReader.BillingServiceTypes.HealthCoaching).FirstOrDefault();
            DateTime date = DateTime.UtcNow.AddMonths(-monitorCharge.Frequency.Value);
            var invoiceStartDate = new DateTime(date.Year, date.Month, 1);
            var invoiceEndDate = invoiceStartDate.AddMonths(monitorCharge.Frequency.Value).AddDays(-1);
            var result = context.IntuityFulfillments.Include("IntuityEligibility").Where(x => x.SendMeter.HasValue && x.SendMeter.Value && x.DateCreated >= invoiceStartDate && x.DateCreated <= invoiceEndDate).ToList();
            foreach (IntuityFulfillments fulfillment in result)
            {
                var userResponse = accountReader.GetUserByUniqueId(new GetUserRequestByUniqueId { UniqueId = fulfillment.IntuityEligibility.UniqueId, OrganizationId = fulfillment.IntuityEligibility.OrganizationId });
                if (userResponse != null && userResponse.User != null)
                {
                    AddInvoiceDetails(new InvoiceDetailsRequest { UserId = userResponse.User.Id, CreatedOn = DateTime.UtcNow, Type = monitorCharge.Id });
                    AddInvoiceDetails(new InvoiceDetailsRequest { UserId = userResponse.User.Id, CreatedOn = DateTime.UtcNow, Type = healthCoaching.Id });
                }
            }
            return true;
        }

        public bool AddIntuityEventToInvoiceDetails()
        {
            AccountReader accountReader = new AccountReader();
            var service = GetBillingServiceTypes();
            var monitorCharge = service.Where(x => x.Type == CommonReader.BillingServiceTypes.MonitorAndSupplies).FirstOrDefault();
            var healthCoaching = service.Where(x => x.Type == CommonReader.BillingServiceTypes.HealthCoaching).FirstOrDefault();
            DateTime date = DateTime.UtcNow.AddMonths(-1);
            var invoiceStartDate = new DateTime(date.Year, date.Month, 1);
            var invoiceEndDate = invoiceStartDate.AddMonths(1).AddDays(-1);
            var result = context.IntuityEvents.Include("User").Where(x => x.EventDate.Value.Year == invoiceStartDate.Year && x.EventDate.Value.Month == invoiceStartDate.Month && x.EventDate.Value.Year == invoiceEndDate.Year && x.EventDate.Value.Month == invoiceEndDate.Month).ToList();
            foreach (IntuityEvent ievent in result)
            {
                AddInvoiceDetails(new InvoiceDetailsRequest { UserId = ievent.User.Id, CreatedOn = DateTime.UtcNow, Type = monitorCharge.Id });
                AddInvoiceDetails(new InvoiceDetailsRequest { UserId = ievent.User.Id, CreatedOn = DateTime.UtcNow, Type = healthCoaching.Id });
            }
            return true;
        }

        public bool EditInvoiceDetails(InvoiceDetailsRequest request)
        {
            InvoiceBilledDetail invoice = context.InvoiceBilledDetails.Where(x => x.Id == request.Id).FirstOrDefault();
            if (invoice != null)
            {
                invoice.UpdatedOn = DateTime.UtcNow;
                invoice.Billed = true;
                context.InvoiceBilledDetails.Attach(invoice);
                context.Entry(invoice).State = EntityState.Modified;
                context.SaveChanges();
            }
            return true;
        }

        public GetInvoiceDetailsResponse GetAllPendingInvoiceBill()
        {
            GetInvoiceDetailsResponse response = new GetInvoiceDetailsResponse();
            response.InvoiceDetails = new List<InvoiceBilledDetailsDto>();
            var invoiceBillingList = context.InvoiceBilledDetails.Where(x => !x.Billed).ToList();

            foreach (InvoiceBilledDetail bill in invoiceBillingList)
            {
                InvoiceBilledDetailsDto InvoiceBilling = Utility.mapper.Map<DAL.InvoiceBilledDetail, InvoiceBilledDetailsDto>(bill);
                var invoices = context.InvoiceDetails.Include("User").Include("BillingServiceType").Where(x => x.UserId == bill.UserId && x.InvoiceId == bill.Id).OrderByDescending(x => x.CreatedOn).ToList();
                InvoiceBilling.InvoiceDetails = Utility.mapper.Map<List<DAL.InvoiceDetail>, List<InvoiceDetailsDto>>(invoices);
                InvoiceBilling.User = Utility.mapper.Map<DAL.User, UserDto>(invoices.Select(x => x.User).FirstOrDefault());
                response.InvoiceDetails.Add(InvoiceBilling);
            }
            return response;
        }

        public AddFaxedReportResponse AddFaxedReport(AddFaxedReportRequest request)
        {
            AddFaxedReportResponse response = new AddFaxedReportResponse();
            var faxedReport = Utility.mapper.Map<FaxedReportsDto, FaxedReport>(request.faxedReport);
            context.FaxedReports.Add(faxedReport);
            context.SaveChanges();
            response.status = true;
            return response;
        }

        public GetFaxedReportsResponse GetFaxedReports(int userId)
        {
            GetFaxedReportsResponse response = new GetFaxedReportsResponse();
            var faxedReports = context.FaxedReports.Include("User1").Include("ReportType1").Where(x => x.UserId == userId).OrderByDescending(x => x.SentOn).ToList();
            response.faxedReports = Utility.mapper.Map<List<DAL.FaxedReport>, List<FaxedReportsDto>>(faxedReports);
            return response;
        }
    }
}