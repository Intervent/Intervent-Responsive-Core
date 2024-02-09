using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

namespace Intervent.Web.DataLayer
{
    public class AdminReportsReader
    {
        InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        const string KitReportDelimiter = "|";

        public ClaimsErrorLogResponse ListEligibilityErrorReport(LogReportRequest request)
        {
            ClaimsErrorLogResponse response = new ClaimsErrorLogResponse();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.timezone);
            PortalReader reader = new PortalReader();
            bool download = false;
            int[] portallist = { };
            var portal = 0;
            if (request.organization.HasValue)
            {
                portallist = reader.ListPortals(new ListPortalsRequest { organizationId = request.organization.Value }).portals.Select(x => x.Id).ToArray();
            }
            if (portallist.Length > 0)
                portal = portallist[0];
            var totalRecords = request.totalRecords.HasValue ? request.totalRecords.Value : 0;
            var startdate = request.startDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.startDate.Value, custTZone) : System.DateTime.MinValue;
            var enddate = request.endDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.endDate.Value, custTZone).AddDays(1) : System.DateTime.MaxValue;
            if (totalRecords == 0)
            {
                totalRecords = context.EligibilityImportLogs.Where(x => (portal == 0 || portallist.Contains(x.PortalId)) && x.LogDate > startdate && x.LogDate < enddate && x.IsLoadError == 1).OrderByDescending(x => x.ID).Count();
                if (request.pageSize == 0)
                {
                    request.pageSize = totalRecords;
                    download = true;
                }
            }
            var logDAL = context.EligibilityImportLogs.Where(x => (portal == 0 || portallist.Contains(x.PortalId)) && x.LogDate > startdate && x.LogDate < enddate && x.IsLoadError == 1).OrderByDescending(x => x.ID).Skip((download ? 0 : request.page * request.pageSize)).Take(request.pageSize).ToList();
            response.report = Utility.mapper.Map<IList<DAL.EligibilityImportLog>, IList<EligibilityImportLogDto>>(logDAL);
            response.totalRecords = totalRecords;
            return response;
        }

        public LabErrorLogResponse ListLabErrorLogReport(LabErrorLogRequest request)
        {
            LabErrorLogResponse response = new LabErrorLogResponse();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.timezone);
            PortalReader reader = new PortalReader();
            var download = false;
            var organizationsList = reader.GetFilteredOrganizationsList(request.AdminId).Organizations.Select(x => x.Id).ToArray();
            int[] requestOrganizationPortals = { };
            if (request.Organization.HasValue)
            {
                int?[] organization = { request.Organization.Value };
                requestOrganizationPortals = reader.GetPortalList(organization).portals.Select(x => x.Id).ToArray();
            }
            var portalsList = reader.GetPortalList(organizationsList).portals.Select(x => x.Id).ToArray();
            var totalRecords = request.totalRecords.HasValue ? request.totalRecords.Value : 0;
            var startdate = request.startDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.startDate.Value, custTZone) : System.DateTime.MinValue;
            var enddate = request.endDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.endDate.Value, custTZone).AddDays(1) : System.DateTime.MaxValue;
            if (totalRecords == 0)
            {
                totalRecords = context.LabErrorLogs.Where(x => (!request.Organization.HasValue || (x.PortalId != null && requestOrganizationPortals.Contains(x.PortalId.Value))) && x.LogDate > startdate && x.LogDate < enddate
                                && (request.IsSuperAdmin || !request.IsSuperAdmin && (x.PortalId != null && portalsList.Contains(x.PortalId.Value)))).OrderByDescending(x => x.Id).GroupBy(l => new { l.UserId, l.Error }).Count();
                if (request.pageSize == 0)
                {
                    request.pageSize = totalRecords;
                    download = true;
                }
            }
            response.report = context.LabErrorLogs.Include("User").Where(x => (!request.Organization.HasValue || (x.PortalId != null && requestOrganizationPortals.Contains(x.PortalId.Value))) && x.LogDate > startdate && x.LogDate < enddate
                            && (request.IsSuperAdmin || !request.IsSuperAdmin && (x.PortalId != null && portalsList.Contains(x.PortalId.Value)))).GroupBy(l => new { l.UserId, l.Error, l.User.FirstName, l.User.LastName }).Select(g => new LabErrorLogDto
                            {
                                Id = g.Max(row => row.Id),
                                UserId = g.Key.UserId,
                                UniqueId = g.Max(row => row.User.UniqueId),
                                Error = g.Key.Error,
                                FirstLogDate = g.Min(row => row.LogDate).Value,
                                LogDate = g.Max(row => row.LogDate).Value,
                                Data = g.Max(row => row.Data),
                                Name = !string.IsNullOrEmpty(g.Key.FirstName + " " + g.Key.LastName) ? (g.Key.FirstName + " " + g.Key.LastName) : "N/A"
                            }).OrderByDescending(x => x.Id).Skip((download ? 0 : request.page * request.pageSize)).Take(request.pageSize).ToList();
            response.totalRecords = totalRecords;
            return response;
        }

        public IncentiveReportResponse ListIncentiveReport(IncentiveReportRequest request)
        {
            IncentiveReportResponse response = new IncentiveReportResponse();
            StoredProcedures sp = new StoredProcedures();
            response.report = sp.IncentiveReport(request.PortalId);
            return response;
        }

        public NoShowApptReportResponse ListNoShowReport(NoShowApptReportRequest request)
        {
            NoShowApptReportResponse response = new NoShowApptReportResponse();
            PortalReader reader = new PortalReader();
            List<String> FutureApptDict = new List<String>();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.timeZoneDefault);
            var download = false;
            var organizationsList = reader.GetFilteredOrganizationsList(request.AdminId).Organizations.Select(x => x.Id).ToArray();
            var totalRecords = request.totalRecords.HasValue ? request.totalRecords.Value : 0;
            var startdate = request.startDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.startDate.Value, custTZone) : System.DateTime.MinValue;
            var enddate = request.endDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.endDate.Value, custTZone).AddDays(1) : System.DateTime.MaxValue;
            if (totalRecords == 0)
            {
                totalRecords = context.Appointments.Include("User").Include("User.TimeZone").Include("User1").Include("User4").Include("AppointmentType").Include("User.UserTrackingStatuses").Where(x => x.InActiveReason == 5 && (x.User.UserTrackingStatuses.Where(y => y.PortalId == y.User.Organization.Portals.Where(z => z.Active == true).OrderByDescending(z => z.Id).FirstOrDefault().Id).FirstOrDefault().DoNotTrack == null || x.User.UserTrackingStatuses.Where(y => y.PortalId == y.User.Organization.Portals.Where(z => z.Active == true).OrderByDescending(z => z.Id).FirstOrDefault().Id).FirstOrDefault().DoNotTrack == false) && ((!request.isReviewed.HasValue) || (x.NSHandledBy != null && request.isReviewed == true) || (x.NSHandledBy == null && request.isReviewed == false)) && (String.IsNullOrEmpty(request.timezone) || x.User.TimeZone.TimeZoneId == request.timezone) && (!request.orgId.HasValue || x.User.OrganizationId == request.orgId) && (!request.ApptType.HasValue || x.Type == request.ApptType) && (String.IsNullOrEmpty(request.language) || x.User.LanguagePreference == request.language) && x.Date > startdate && x.Date < enddate && organizationsList.Contains(x.User.OrganizationId)).OrderByDescending(x => x.Id).Count();
                if (request.pageSize == 0)
                {
                    request.pageSize = totalRecords;
                    download = true;
                }
            }
            var report = context.Appointments.Include("User").Include("User.Organization").Include("User.TimeZone").Include("User1").Include("User4").Include("AppointmentType").Include("User.UserTrackingStatuses").Where(x => x.InActiveReason == 5 && (x.User.UserTrackingStatuses.Where(y => y.PortalId == y.User.Organization.Portals.Where(z => z.Active == true).OrderByDescending(z => z.Id).FirstOrDefault().Id).FirstOrDefault().DoNotTrack == null || x.User.UserTrackingStatuses.Where(y => y.PortalId == y.User.Organization.Portals.Where(z => z.Active == true).OrderByDescending(z => z.Id).FirstOrDefault().Id).FirstOrDefault().DoNotTrack == false) && ((!request.isReviewed.HasValue) || (x.NSHandledBy != null && request.isReviewed == true) || (x.NSHandledBy == null && request.isReviewed == false)) && (String.IsNullOrEmpty(request.timezone) || x.User.TimeZone.TimeZoneId == request.timezone) && (!request.orgId.HasValue || x.User.OrganizationId == request.orgId) && (!request.ApptType.HasValue || x.Type == request.ApptType) && (String.IsNullOrEmpty(request.language) || x.User.LanguagePreference == request.language) && x.Date > startdate && x.Date < enddate && organizationsList.Contains(x.User.OrganizationId)).OrderBy(x => x.User.FirstName).Skip((download ? 0 : request.page * request.pageSize)).Take(request.pageSize).ToList();
            response.report = Utility.mapper.Map<IList<DAL.Appointment>, IList<AppointmentDTO>>(report);
            var userids = response.report.Select(x => x.UserId).ToList();
            foreach (var userid in userids)
            {
                var apt = context.Appointments.Include("User").Where(x => x.UserId == userid && x.Date > DateTime.UtcNow && x.Active == true).FirstOrDefault();
                if (apt != null)
                {
                    response.report.Where(x => x.UserId == apt.UserId).FirstOrDefault().FutureAppointmentDate = apt.Date;
                }
            }
            response.totalRecords = totalRecords;
            return response;
        }

        public SmokingCessationIncentiveResponse ListSmokingCessationIncentive(SmokingCessationIncentiveRequest request)
        {
            SmokingCessationIncentiveResponse response = new SmokingCessationIncentiveResponse();
            List<GetTobaccoIncentive_Result> result = null;
            StoredProcedures sp = new StoredProcedures();
            result = sp.GetTobaccoIncentive(request.PortalId);
            var final = result.ToList();
            response.report = Utility.mapper.Map<IList<DAL.GetTobaccoIncentive_Result>, IList<SmokingCessationIncentiveDto>>(final);
            return response;
        }

        public ListPaymentTransactionResponse ListPaymentTransactionReport(ListPaymentTransactionRequest request)
        {
            ListPaymentTransactionResponse response = new ListPaymentTransactionResponse();
            PortalReader reader = new PortalReader();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.timezone);
            var download = false;
            var organizationsList = reader.GetFilteredOrganizationsList(request.AdminId).Organizations.Select(x => x.Id).ToArray();
            var totalRecords = request.totalRecords.HasValue ? request.totalRecords.Value : 0;
            var startdate = request.startDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.startDate.Value, custTZone) : System.DateTime.MinValue;
            var enddate = request.endDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.endDate.Value, custTZone).AddDays(1) : System.DateTime.MaxValue;
            if (totalRecords == 0)
            {
                totalRecords = context.PaymentTransactions.Include("User").Where(x => (!request.orgId.HasValue || x.User.OrganizationId == request.orgId) && (String.IsNullOrEmpty(request.type) || x.Type == request.type) && (x.Date >= startdate && x.Date <= enddate) && organizationsList.Contains(x.User.OrganizationId)).OrderByDescending(x => x.Id).Count();
                if (request.pageSize == 0)
                {
                    request.pageSize = totalRecords;
                    download = true;
                }
            }
            var report = context.PaymentTransactions.Include("User").Where(x => (!request.orgId.HasValue || x.User.OrganizationId == request.orgId) && (String.IsNullOrEmpty(request.type) || x.Type.Equals(request.type)) && (x.Date >= startdate && x.Date <= enddate) && organizationsList.Contains(x.User.OrganizationId)).OrderBy(x => x.User.FirstName).Skip((download ? 0 : request.page * request.pageSize)).Take(request.pageSize).ToList();
            response.report = Utility.mapper.Map<IList<DAL.PaymentTransaction>, IList<PaymentTransactionDto>>(report);
            response.totalRecords = totalRecords;
            return response;
        }

        public CoachTrackingResponse ListCoachTrackingReport(ListCoachTrackingRequest request)
        {
            CoachTrackingResponse response = new CoachTrackingResponse();
            List<CoachTrackingReport_Result> report = null;
            StoredProcedures sp = new StoredProcedures();
            PortalReader reader = new PortalReader();
            var organizationsList = reader.GetFilteredOrganizationsList(request.AdminId).Organizations.Select(x => x.Id).ToArray();
            var totalRecords = request.totalRecords.HasValue ? request.totalRecords.Value : 0;
            if (totalRecords == 0 && !request.forDownload)
            {
                report = sp.GetCoachTrackingReport(request.orgId, request.coach, request.AdminId, request.page, request.pageSize, true, request.forDownload);
                totalRecords = report.ToList().FirstOrDefault().Records.Value;
            }
            report = sp.GetCoachTrackingReport(request.orgId, request.coach, request.AdminId, request.page, request.pageSize, false, request.forDownload);
            response.coachTrackingRecords = Utility.mapper.Map<List<DAL.CoachTrackingReport_Result>, List<CoachTrackingReport_ResultDto>>(report);
            response.totalRecords = totalRecords;
            return response;
        }

        public KitUserReportResponse ListKitUserReport(KitUserReportRequest request)
        {
            KitUserReportResponse response = new KitUserReportResponse();
            StoredProcedures sp = new StoredProcedures();
            //var kitsPortalList = sp.KitsPortalReport(request.portalId, request.kitId);
            //response.KitsPortalReport = ConvertToCSVFormat(kitsPortalList, request.kitId);  //Utility.mapper.Map<List<DAL.KitsPortalReport_Result>, List<KitsPortalReport_ResultDto>>(kitsPortalList);
            response.KitsPortalReport = GetKitReport(request.kitId, request.portalId, request.timezone);  //Utility.mapper.Map<List<DAL.KitsPortalReport_Result>, List<KitsPortalReport_ResultDto>>(kitsPortalList);
            return response;
        }

        public StringBuilder GetKitReport(int kitId, int portalId, string timezone)
        {
            var steps = context.StepsinKits.Include("ActivitiesinSteps").Where(x => x.KitId == kitId).ToList();
            var csv = new StringBuilder();
            csv.AppendLine("sep=" + KitReportDelimiter);
            csv.Append(String.Join(KitReportDelimiter, new string[] { "UserId", "FirstName", "LastName", "CompleteDate" }));
            csv.Append(KitReportDelimiter);
            KitReader reader = new KitReader();
            ProgramReader programReader = new ProgramReader();
            List<ActivitiesinStepsDto> activitiesinSteps = new List<ActivitiesinStepsDto>();
            List<int> questionsList = new List<int>();
            foreach (var step in steps)
            {
                GetStepWithActivityRequest request = new GetStepWithActivityRequest();
                request.stepId = step.Id;
                request.activityIds = step.ActivitiesinSteps.Select(x => x.Id).ToList();
                request.languageCode = "";
                var response = reader.GetStepWithActivity(request).stepsinKits;
                if (response.ActivitiesinSteps != null && response.ActivitiesinSteps.Count() > 0)
                    activitiesinSteps.AddRange(response.ActivitiesinSteps);
            }
            foreach (var activity in activitiesinSteps)
            {
                foreach (var question in activity.QuestionsinActivities.Where(x => x.ParentId == null && x.IsActive))
                {
                    if ((QuestionType)question.QuestionType == QuestionType.table)
                    {
                        var text = StripHTML(question.QuestionText);
                        if (text != "&nbsp;")
                        {
                            csv.Append(EscapeCsvText(text) + KitReportDelimiter);
                            questionsList.Add(question.Id);
                        }
                        var options = question.OptionsforActivityQuestions.Where(o => (o.IsActive.HasValue && o.IsActive.Value)).OrderBy(x => x.SequenceNo).ThenBy(x => x.Id).ToList();
                        foreach (var option in options)
                        {
                            foreach (var subQuestion in activity.QuestionsinActivities.Where(x => x.ParentId == option.Id).ToList())
                            {
                                if (!subQuestion.QuestionText.StartsWith(Intervent.Web.DataLayer.CommonConstants.TableOptionText))
                                {
                                    var subtext = StripHTML(subQuestion.QuestionText);
                                    csv.Append(EscapeCsvText(subtext) + KitReportDelimiter);
                                    questionsList.Add(subQuestion.Id);
                                }
                            }
                        }
                    }
                    else if ((QuestionType)question.QuestionType == QuestionType.tablecheckbox)
                    {
                        var text = StripHTML(question.QuestionText);
                        if (text != "&nbsp;")
                        {
                            csv.Append(EscapeCsvText(text) + KitReportDelimiter);
                            questionsList.Add(question.Id);
                        }
                        var options = question.OptionsforActivityQuestions.Where(o => (o.IsActive.HasValue && o.IsActive.Value)).OrderBy(x => x.SequenceNo).ThenBy(x => x.Id).ToList();
                        var nonOption = options.Where(x => x.OptionText != Intervent.Web.DataLayer.CommonConstants.TableOptionText).OrderBy(o => o.SequenceNo).ToList();
                        foreach (var option in nonOption)
                        {
                            foreach (var subQuestion in activity.QuestionsinActivities.Where(x => x.ParentId == option.Id).ToList())
                            {
                                var subtext = StripHTML(option.OptionText) + StripHTML(subQuestion.QuestionText);
                                csv.Append(EscapeCsvText(subtext) + KitReportDelimiter);
                                questionsList.Add(subQuestion.Id);
                            }
                        }
                    }
                    else if ((QuestionType)question.QuestionType == QuestionType.checkbox)
                    {
                        var text = StripHTML(question.QuestionText);
                        if (text != "&nbsp;")
                        {
                            csv.Append(EscapeCsvText(text) + KitReportDelimiter);
                            questionsList.Add(question.Id);
                        }
                        var options = question.OptionsforActivityQuestions.Where(o => (o.IsActive.HasValue && o.IsActive.Value)).OrderBy(x => x.SequenceNo).ThenBy(x => x.Id).ToList();
                        foreach (var option in options)
                        {
                            foreach (var subQuestion in activity.QuestionsinActivities.Where(x => x.ParentId == option.Id).ToList())
                            {
                                var subtext = StripHTML(subQuestion.QuestionText);
                                subtext = option.OptionText + ":" + subtext.Replace("\n", "");
                                csv.Append(EscapeCsvText(subtext) + KitReportDelimiter);
                                questionsList.Add(subQuestion.Id);
                            }
                        }
                    }
                    else if ((QuestionType)question.QuestionType == QuestionType.radiobutton)
                    {
                        var text = StripHTML(question.QuestionText);
                        if (text != "&nbsp;")
                        {
                            csv.Append(EscapeCsvText(text) + KitReportDelimiter);
                            questionsList.Add(question.Id);
                        }
                        var options = question.OptionsforActivityQuestions.Where(o => (o.IsActive.HasValue && o.IsActive.Value)).OrderBy(x => x.SequenceNo).ThenBy(x => x.Id).ToList();
                        foreach (var option in options)
                        {
                            foreach (var subQuestion in activity.QuestionsinActivities.Where(x => x.ParentId == option.Id).ToList())
                            {
                                var subtext = StripHTML(subQuestion.QuestionText);
                                csv.Append(EscapeCsvText(subtext) + KitReportDelimiter);
                                questionsList.Add(subQuestion.Id);
                            }
                        }
                    }
                    else if ((QuestionType)question.QuestionType == QuestionType.dropdown)
                    {
                        var text = StripHTML(question.QuestionText);
                        if (text != "&nbsp;")
                        {
                            csv.Append(EscapeCsvText(text) + KitReportDelimiter);
                            questionsList.Add(question.Id);
                        }
                        var options = question.OptionsforActivityQuestions.Where(o => (o.IsActive.HasValue && o.IsActive.Value)).OrderBy(x => x.SequenceNo).ThenBy(x => x.Id).ToList();
                        foreach (var option in options)
                        {
                            foreach (var subQuestion in activity.QuestionsinActivities.Where(x => x.ParentId.HasValue && question.OptionsforActivityQuestions.Where(o => (o.IsActive.HasValue && o.IsActive.Value)).Select(y => y.Id).ToList().Contains(x.ParentId.Value)).ToList())
                            {
                                var subtext = StripHTML(subQuestion.QuestionText);
                                csv.Append(EscapeCsvText(subtext) + KitReportDelimiter);
                                questionsList.Add(subQuestion.Id);
                            }
                        }
                    }
                    else
                    {
                        var text = StripHTML(question.QuestionText);
                        if (text != "&nbsp;")
                        {
                            csv.Append(EscapeCsvText(text) + KitReportDelimiter);
                            questionsList.Add(question.Id);
                        }
                    }
                }
            }
            var activityIds = activitiesinSteps.Select(x => x.Id).ToList();
            csv.Append(GetUserResponses(questionsList, kitId, portalId, timezone));
            return csv;
        }

        public string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public StringBuilder GetUserResponses(List<int> questions, int kitId, int portalId, string timezone)
        {
            var options = context.OptionsforActivityQuestions.Where(x => questions.Contains(x.QuestionId) && x.IsActive).ToDictionary(x => x.Id, x => x.OptionText);
            var choices = context.KitsinUserPrograms.Include("UserChoices").Include("UserChoices.QuestionsinActivity")
                .Include("UsersinProgram").Include("UsersinProgram.ProgramsinPortal").Include("UsersinProgram.User")
                .Where(x => x.KitId == kitId && x.UsersinProgram.ProgramsinPortal.PortalId == portalId
                && (!x.UsersinProgram.User.FirstName.ToLower().Contains("test") && !x.UsersinProgram.User.LastName.ToLower().Contains("test"))).ToList();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            var csv = new StringBuilder();
            foreach (var choice in choices)
            {
                string completeDate = choice.CompleteDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(choice.CompleteDate.Value, custTZone).ToString() : "";
                csv.AppendLine();
                csv.Append(choice.UsersinProgram.User.Id + KitReportDelimiter + choice.UsersinProgram.User.FirstName + KitReportDelimiter + choice.UsersinProgram.User.LastName + KitReportDelimiter + completeDate + KitReportDelimiter);
                if (choice.UserChoices.Count() > 0)
                {
                    foreach (var questionId in questions)
                    {
                        var userChoice = choice.UserChoices.Where(x => x.QuestionId == questionId).FirstOrDefault();
                        if (userChoice != null)
                        {
                            if (userChoice.QuestionsinActivity.QuestionType == (int)QuestionType.textbox || userChoice.QuestionsinActivity.QuestionType == (int)QuestionType.floatnumber || userChoice.QuestionsinActivity.QuestionType == (int)QuestionType.number)
                            {
                                csv.Append(KitReportDelimiter + EscapeCsvText(userChoice.Value));
                            }
                            else
                            {
                                var choiceValues = userChoice.Value.Split(',');
                                choiceValues = choiceValues.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                                string answerText = string.Empty;
                                foreach (var choiceValue in choiceValues)
                                {
                                    if (!string.IsNullOrEmpty(choiceValue))
                                    {
                                        var key = Convert.ToInt32(choiceValue);
                                        options.TryGetValue(key, out var textValue);
                                        if (textValue != null)
                                        {
                                            answerText = answerText + EscapeCsvText(textValue) + ",";
                                        }
                                        else
                                        {
                                            answerText = answerText + EscapeCsvText(userChoice.Value) + ",";
                                        }
                                    }
                                }
                                csv.Append(KitReportDelimiter + answerText.TrimEnd(','));
                            }
                        }
                        else
                        {
                            csv.Append(KitReportDelimiter);
                        }
                    }
                }
            }
            return csv;
        }

        public ListUnapprovedCarePlanResponse ListUnapprovedCarePlans(UnapprovedCarePlanRequest request)
        {
            ListUnapprovedCarePlanResponse response = new ListUnapprovedCarePlanResponse();
            List<UnapprovedCarePlan> unapprovedCarePlans = new List<UnapprovedCarePlan>();

            PortalReader reader = new PortalReader();
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            bool download = false;
            var organizationsList = reader.GetFilteredOrganizationsList(request.userId).Organizations.Where(x => x.Portals.Count > 0 && x.Portals.Any(y => y.Active && y.CarePlan)).ToList();
            List<int> portalIds = new List<int>();

            if (request.poralId.HasValue)
            {
                portalIds.Add(request.poralId.Value);
                organizationsList = organizationsList.Where(x => x.Portals.Any(y => y.Id == request.poralId.Value)).ToList();
            }
            else
                portalIds = organizationsList.SelectMany(x => x.Portals.Where(y => y.Active && y.CarePlan).Select(y => y.Id)).ToList();

            List<int> organizationIds = organizationsList.Select(x => x.Id.Value).ToList();
            var eligibilityUsers = context.Eligibilities.Where(x => portalIds.Contains(x.PortalId) && (string.IsNullOrEmpty(request.userStatus) || x.UserStatus == request.userStatus)).Select(y => y.UniqueId).ToList();

            if (request.assessmentType == 1 || !request.assessmentType.HasValue)
            {
                var approvedHRAReports = context.CarePlanReports.Where(x => x.ReportGenerated.HasValue && x.ReportGenerated.Value && x.Type == 1).Select(x => x.RefId).ToList();
                var unapprovedHRAUsers = context.Users.Include("HRAs").Where(x => (eligibilityUsers.Contains(x.UniqueId) || (string.IsNullOrEmpty(x.UniqueId) && organizationIds.Contains(x.OrganizationId))) && (!x.FirstName.ToLower().Contains("test") && !x.LastName.ToLower().Contains("test"))
                                        && x.HRAs.Where(y => portalIds.Contains(y.PortalId)).OrderByDescending(y => y.Id).FirstOrDefault() != null
                                        && x.HRAs.Where(y => portalIds.Contains(y.PortalId)).OrderByDescending(y => y.Id).FirstOrDefault().CompleteDate.HasValue
                                        && !approvedHRAReports.Contains(x.HRAs.OrderByDescending(y => y.Id).FirstOrDefault().Id.ToString())).ToList();
                unapprovedCarePlans = unapprovedHRAUsers.Select(x => new UnapprovedCarePlan
                {
                    userName = x.FirstName + " " + x.LastName,
                    userId = x.Id,
                    uniqueId = x.UniqueId,
                    refId = x.HRAs.Where(y => portalIds.Contains(y.PortalId)).FirstOrDefault().Id,
                    reportType = 1,
                    completedDate = x.HRAs.Where(y => portalIds.Contains(y.PortalId)).FirstOrDefault().CompleteDate.Value,
                }).ToList();
            }
            if (request.assessmentType == 2 || !request.assessmentType.HasValue)
            {
                var approvedFollowUpReports = context.CarePlanReports.Where(x => x.ReportGenerated.HasValue && x.ReportGenerated.Value && x.Type == 2).Select(x => x.RefId).ToList();
                var unapprovedFollowUps = context.FollowUps.Include("UsersinProgram").Include("UsersinProgram.User").Where(x => (eligibilityUsers.Contains(x.UsersinProgram.User.UniqueId) || (string.IsNullOrEmpty(x.UsersinProgram.User.UniqueId) && portalIds.Contains(x.UsersinProgram.ProgramsinPortal.PortalId) && organizationIds.Contains(x.UsersinProgram.User.OrganizationId))) && x.UsersinProgram.IsActive && (!x.UsersinProgram.User.FirstName.ToLower().Contains("test") && !x.UsersinProgram.User.LastName.ToLower().Contains("test")) && x.CompleteDate.HasValue && !approvedFollowUpReports.Contains(x.Id.ToString())).ToList();
                unapprovedCarePlans.AddRange(unapprovedFollowUps.Select(x => new UnapprovedCarePlan
                {
                    userName = x.UsersinProgram.User.FirstName + " " + x.UsersinProgram.User.LastName,
                    userId = x.UsersinProgram.User.Id,
                    uniqueId = x.UsersinProgram.User.UniqueId,
                    refId = x.Id,
                    reportType = 2,
                    usersinProgramId = x.UsersinProgram.Id,
                    completedDate = x.CompleteDate.Value
                }).ToList());
            }
            if (totalRecords == 0)
            {
                totalRecords = unapprovedCarePlans.Count();
                if (request.PageSize == 0)
                {
                    request.PageSize = totalRecords;
                    download = true;
                }
            }
            response.UnapprovedCarePlans = unapprovedCarePlans.OrderByDescending(x => x.completedDate).Skip((download ? 0 : request.Page * request.PageSize)).Take(request.PageSize).ToList(); ;
            response.totalRecords = totalRecords;
            response.portalIds = portalIds;
            return response;
        }

        static string EscapeCsvText(string text)
        {
            if (text == null)
                return text;
            if (text.Contains("“"))
            {
                text = text.Replace("“", "\"");
            }
            if (text.Contains("”"))
            {
                text = text.Replace("”", "\"");
            }
            if (text.Contains(","))
            {
                return String.Concat("\"", text, "\"");
            }
            return text;
        }

        public ListLabAlertResponse ListLabAlert(ListLabAlertRequest request)
        {
            ListLabAlertResponse response = new ListLabAlertResponse();
            PortalReader reader = new PortalReader();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.timezone);
            var download = false;
            var organizationsList = reader.GetFilteredOrganizationsList(request.AdminId).Organizations.Select(x => x.Id).ToArray();
            var startdate = request.StartDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.StartDate.Value, custTZone) : System.DateTime.MinValue;
            var enddate = request.EndDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.EndDate.Value, custTZone).AddDays(1) : System.DateTime.MaxValue;
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = context.Labs.Include("User2").Include("User2.Notes").Where(x => (!request.Organization.HasValue || x.User2.OrganizationId == request.Organization)
                                && ((request.alerttype == 1 && (!String.IsNullOrEmpty(x.CoachAlert) || !String.IsNullOrEmpty(x.CriticalAlert))) || (request.alerttype == 2 && !String.IsNullOrEmpty(x.CoachAlert)) || (request.alerttype == 3 && !String.IsNullOrEmpty(x.CriticalAlert)))
                                && (request.labsource == 1 || (request.labsource == 2 && x.HL7 != null) || (request.labsource == 3 && x.HL7 == null))
                                && (organizationsList.Count() != 0 && (!request.userId.HasValue || x.UserId == request.userId.Value) && (x.DateCompleted > startdate && x.DateCompleted < enddate) && organizationsList.Contains(x.User2.OrganizationId))
                                && (!request.status.HasValue || ((request.status == 3 && x.User2.Notes.Where(y => y.RefId == x.Id && y.Type == (int)NoteTypes.Critical_Alert).Count() > 0 && !x.ReviewedBy.HasValue)
                                   || (request.status == 1 && x.ReviewedBy.HasValue)
                                   || (request.status == 2 && !(x.ReviewedBy.HasValue) && x.User2.Notes.Where(y => y.RefId == x.Id && y.Type == (int)NoteTypes.Critical_Alert).Count() == 0)))).OrderByDescending(x => x.UserId).Count();
                if (request.PageSize == 0)
                {
                    request.PageSize = totalRecords;
                    download = true;
                }
            }
            List<Lab> LabAlertDAL = null;
            if (totalRecords > 0)
            {
                LabAlertDAL = context.Labs.Include("User").Include("User1").Include("User2").Include("User2.Notes").Where(x => (!request.Organization.HasValue || x.User2.OrganizationId == request.Organization)
                      && ((request.alerttype == 1 && (!String.IsNullOrEmpty(x.CoachAlert) || !String.IsNullOrEmpty(x.CriticalAlert))) || (request.alerttype == 2 && !String.IsNullOrEmpty(x.CoachAlert)) || (request.alerttype == 3 && !String.IsNullOrEmpty(x.CriticalAlert)))
                      && (request.labsource == 1 || (request.labsource == 2 && x.HL7 != null) || (request.labsource == 3 && x.HL7 == null))
                      && (organizationsList.Count() != 0 && (!request.userId.HasValue || x.UserId == request.userId.Value) && (x.DateCompleted > startdate && x.DateCompleted < enddate) && organizationsList.Contains(x.User2.OrganizationId))
                      && (!request.status.HasValue || ((request.status == 3 && x.User2.Notes.Where(y => y.RefId == x.Id && y.Type == (int)NoteTypes.Critical_Alert).Count() > 0 && !x.ReviewedBy.HasValue)
                          || (request.status == 1 && x.ReviewedBy.HasValue)
                          || (request.status == 2 && !(x.ReviewedBy.HasValue) && x.User2.Notes.Where(y => y.RefId == x.Id && y.Type == (int)NoteTypes.Critical_Alert).Count() == 0)))).OrderByDescending(x => x.UserId).Skip((download ? 0 : request.Page * request.PageSize)).Take(request.PageSize).ToList();
            }
            response.listLabAlertReportResponse = Utility.mapper.Map<IList<DAL.Lab>, IList<LabDto>>(LabAlertDAL);
            response.totalRecords = totalRecords;
            return response;
        }

        public ReviewNoShowResponse ReviewNoShow(ReviewNoShowRequest request)
        {
            ReviewNoShowResponse response = new ReviewNoShowResponse();
            int[] ApptIdList = request.ApptIds.Split(',').Select(str => int.Parse(str)).ToArray();
            foreach (int ApptId in ApptIdList)
            {
                var appointment = context.Appointments.Where(x => x.Id == ApptId).FirstOrDefault();
                if (appointment != null)
                {
                    appointment.NSHandledBy = request.AdminId;
                    context.Appointments.Attach(appointment);
                    context.Entry(appointment).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            response.success = true;
            return response;
        }
    }
}