using Intervent.DAL;
using Intervent.HWS;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using NLog;
using System.Configuration;
using static Intervent.HWS.PlacerRequest;
using static Intervent.HWS.PPRFormRequest;

namespace Intervent.Business
{
    public class PlacerManager : BaseManager
    {
        ParticipantReader participantReader = new ParticipantReader();
        PortalReader portalReader = new PortalReader();

        private static readonly string Video = "video";

        private static readonly string Telephone = "telephone";

        int teamsBPOrgId = Convert.ToInt16(ConfigurationManager.AppSettings["TeamsBPOrgId"]);
        string teamsBPURL = ConfigurationManager.AppSettings["TeamsBPURL"];
        string teamsBPApiKey = ConfigurationManager.AppSettings["TeamsBPApiKey"];

        public string CreateEligibility(UserManager<ApplicationUser> userManager, PlacerParticipantRequest userRequest, int _teamsBPOrgId)
        {
            try
            {
                AccountReader accountReader = new AccountReader(userManager);
                AddEditEligibilityRequest eligRequest = new AddEditEligibilityRequest();
                var portalId = portalReader.GetOrganizationById(_teamsBPOrgId).Portals.Where(x => x.Active).FirstOrDefault().Id;
                EligibilityDto eligibility = participantReader.GetEligibilityByUniqueId(userRequest.participant_id, portalId);
                if (eligibility == null)
                {
                    eligibility = new EligibilityDto();
                    eligibility.CreateDate = DateTime.UtcNow;
                }
                eligibility.FirstName = userRequest.first_name;
                eligibility.LastName = userRequest.last_name;
                if (!string.IsNullOrEmpty(userRequest.middle_name))
                    eligibility.MiddleName = userRequest.middle_name;
                if (!string.IsNullOrEmpty(userRequest.email))
                    eligibility.Email = userRequest.email;
                if (!string.IsNullOrEmpty(userRequest.phone_number))
                    eligibility.CellNumber = userRequest.phone_number;
                if (userRequest.date_of_birth.HasValue)
                    eligibility.DOB = userRequest.date_of_birth;
                if (!string.IsNullOrEmpty(userRequest.gender))
                    eligibility.Gender = userRequest.gender.ToLower().Equals("m") ? GenderDto.Male : GenderDto.Female;
                eligibility.UniqueId = userRequest.participant_id;
                if (!string.IsNullOrEmpty(userRequest.provider))
                    eligibility.RegionCode = userRequest.provider;
                if (!string.IsNullOrEmpty(userRequest.zip))
                    eligibility.Zip = userRequest.zip;
                eligibility.PortalId = portalId;
                eligibility.UserStatus = EligibilityUserStatusDto.Active;
                eligibility.UserEnrollmentType = EligibilityUserEnrollmentTypeDto.Patient;
                eligRequest.Eligibility = eligibility;

                var eligResponse = participantReader.AddEditEligibility(eligRequest);
                if (eligResponse.success)
                {
                    // Create new user based on eligibility 
                    GetUserRequestByUniqueId request = new GetUserRequestByUniqueId();
                    request.OrganizationId = _teamsBPOrgId;
                    request.UniqueId = userRequest.participant_id;
                    var userResponse = Task.Run(() => accountReader.GetUserByUniqueId(request)).Result;
                    if (userResponse.User == null)
                    {
                        var user = Task.Run(() => accountReader.CreateUserFromEligibility(eligResponse.Eligibility, _teamsBPOrgId, true)).Result;
                        if (user == null || !user.Succeeded)
                        {
                            string msg = "Eligibility id -" + eligResponse.Eligibility.Id + ". Error - " + (user != null ? user.error.FirstOrDefault() : null);
                            LogReader logReader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Error, "User Creation", null, msg, null, null);
                            logReader.WriteLogMessage(logEvent);
                        }
                        return "success";
                    }
                    else
                        return "success";
                }
                return "Eligibility couldn't be processed.";
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "CreateEligibility", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
                return "Eligibility couldn't be processed.";
            }
        }

        public void ProcessReportList()
        {
            try
            {
                AccountReader accountReader = new AccountReader();

                var users = accountReader.FindUsers(new FindUsersRequest() { OrganizationIds = new List<int>() { teamsBPOrgId } }).Users;
                var portalId = portalReader.GetOrganizationById(teamsBPOrgId).Portals.Where(x => x.Active).FirstOrDefault().Id;
                List<ExternalReportsDto> reportLists = new List<ExternalReportsDto>();
                foreach (var user in users)
                {
                    if (!string.IsNullOrEmpty(user.UniqueId))
                    {
                        EligibilityDto eligibility = participantReader.GetEligibilityByUniqueId(user.UniqueId, portalId);
                        if (eligibility != null)
                        {
                            PullReportListResponse response = Placer.PullReportList(new GetExternalReportsList { uniqueId = user.UniqueId, tenant = eligibility.RegionCode }, teamsBPApiKey, teamsBPURL);
                            if (response.Status && response.ParticipantReportList != null && response.ParticipantReportList.ReportList != null && response.ParticipantReportList.ReportList.Count > 0)
                            {
                                var exitingReports = participantReader.ListExternalReports(new ReportListRequest { UserId = user.Id }).reportLists.Select(x => x.ReportName);
                                foreach (var reportName in response.ParticipantReportList.ReportList)
                                {
                                    if (!exitingReports.Contains(reportName))
                                    {
                                        var reportList = new ExternalReportsDto();
                                        reportList.ReportName = reportName;
                                        reportList.UserId = user.Id;
                                        reportList.CreatedOn = DateTime.UtcNow;
                                        PullReportListResponse reportData = Placer.RetrieveReport(new GetExternalReports { uniqueId = user.UniqueId, reportName = reportName, tenant = eligibility.RegionCode }, teamsBPApiKey, teamsBPURL);
                                        if (reportData.Status && reportData.ExternalReportData != null && reportData.ExternalReportData.ReportData != null)
                                        {
                                            reportList.ReportData = reportData.ExternalReportData.ReportData;
                                            reportLists.Add(reportList);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (reportLists.Count > 0)
                {
                    participantReader.UpdateReportList(reportLists);
                    LogReader logreader = new LogReader();
                    var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "External Reports Processed on " + DateTime.Now.ToString() + ". File Count " + reportLists.Count, null, null);
                    logreader.WriteLogMessage(logEvent);
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
        }

        public void ProcessBillingNotes()
        {
            participantReader.ProcessBillingNotes(new ProcessBillingNotesRequest { orgId = teamsBPOrgId });
            SendCoachingNotes();
        }

        public void SendCoachingNotes()
        {
            try
            {
                var billingNotes = participantReader.GetPendingBillingNotesList().billingNotes;
                foreach (var billingDetails in billingNotes)
                {
                    var request = ProcessRequest(billingDetails);
                    if (request != null)
                    {
                        PostCoachingResponse response = Placer.PostCoachingNotes(request, teamsBPApiKey, teamsBPURL);
                        LogReader logreader = new LogReader();
                        var logEvent = new LogEventInfo(LogLevel.Info, "SendCoachingNotes", null, "Response : " + JsonConvert.SerializeObject(response), null, null);
                        logreader.WriteLogMessage(logEvent);
                        billingDetails.JsonRequest = JsonConvert.SerializeObject(request);
                        billingDetails.Submitted = response.Status;
                        billingDetails.SubmittedOn = DateTime.UtcNow;
                        participantReader.EditBillingNotes(new EditBillingNotesRequest { BillingNote = billingDetails });
                    }
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
        }

        public PPRFormRequest ProcessRequest(BillingNotesDto billingDetails)
        {
            PPRFormRequest request = new PPRFormRequest();
            List<EdVisitList> ed_visit_list = new List<EdVisitList>();
            List<HospitalAdmitsList> hospital_admits_list = new List<HospitalAdmitsList>();
            List<FallsList> falls_list = new List<FallsList>();
            var wellness = billingDetails.WellnessData;
            var data = wellness.TeamsBP_PPR.FirstOrDefault();
            string coachName = null;
            if (wellness.User1.Roles.Where(x => x.Code == "COACH").Count() > 0)
            {
                coachName = wellness.User1.FirstName + " " + wellness.User1.LastName;
            }
            else if (wellness.User2.Roles.Where(x => x.Code == "COACH").Count() > 0)
            {
                coachName = wellness.User2.FirstName + " " + wellness.User2.LastName;
            }
            else
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "SendCoachingNotes", "Coach role not found for billing notes : " + billingDetails.Id);
                logreader.WriteLogMessage(logEvent);
                return null;
            }

            var currentAppointment = wellness.User.Appointments.Where(x => DateTime.Parse(x.Date).Date == wellness.CollectedOn.Date).FirstOrDefault();
            if (currentAppointment != null)
            {
                var futureAppointment = wellness.User.Appointments.Where(x => DateTime.Parse(x.Date).Date > DateTime.Parse(currentAppointment.Date).Date).OrderBy(x => x.Date).FirstOrDefault();

                if (data.HasVisitedED.HasValue && data.HasVisitedED.Value)
                {
                    string[] date_of_visit = data.EDDateOfVisit.Split('|');
                    string[] reason = data.EDVisitReason.Split('|');
                    for (int i = 0; i < date_of_visit.Count(); i++)
                    {
                        ed_visit_list.Add(new EdVisitList
                        {
                            date_of_visit = !string.IsNullOrEmpty(date_of_visit[i].Trim()) ? Convert.ToDateTime(date_of_visit[i]).ToString("yyyy-MM-dd") : null,
                            reason = reason[i]
                        });
                    }
                }
                if (data.WasAdmittedHospital.HasValue && data.WasAdmittedHospital.Value)
                {
                    string[] date_of_admit = data.DateOfAdmit.Split('|');
                    string[] reason = data.HospitalizationReason.Split('|');
                    string[] number_of_days = data.DaysInHospital.Split('|');
                    for (int i = 0; i < date_of_admit.Count(); i++)
                    {
                        hospital_admits_list.Add(new HospitalAdmitsList
                        {
                            date_of_admit = !string.IsNullOrEmpty(date_of_admit[i].Trim()) ? Convert.ToDateTime(date_of_admit[i]).ToString("yyyy-MM-dd") : null,
                            reason = reason[i],
                            number_of_days = !string.IsNullOrEmpty(number_of_days[i].Trim()) ? int.Parse(number_of_days[i]) : 0
                        });
                    }
                }
                if (data.HasFallen.HasValue && data.HasFallen.Value && !string.IsNullOrEmpty(data.HowSeriousWasInjury))
                {
                    string[] injurylist = data.HowSeriousWasInjury.Split('|');
                    foreach (string injury in injurylist)
                    {
                        falls_list.Add(new FallsList
                        {
                            injury = string.IsNullOrEmpty(injury.Trim()) ? null : injury,
                        });
                    }
                }

                request.participant_first_name = wellness.User.FirstName;
                request.participant_last_name = wellness.User.LastName;
                request.participant_birth_date = wellness.User.DOB.HasValue ? wellness.User.DOB.Value.ToString("yyyy-MM-dd") : null;
                request.participant_study_subject_ID = wellness.User.UniqueId;
                request.session_date_time = wellness.CollectedOn.ToString("yyyy-MM-dd HH:mm");
                request.session_length_minutes = billingDetails.TimeSpent;
                request.session_notes = data.Notes;
                request.session_communication_type = currentAppointment.VideoRequired ? Video : Telephone;
                request.e_sig = coachName;
                request.e_sig_date_time = wellness.UpdatedOn.HasValue ? wellness.UpdatedOn.Value.ToString("yyyy-MM-dd HH:mm") : wellness.CollectedOn.ToString("yyyy-MM-dd HH:mm");
                request.coach_name = coachName;

                request.coaching_goals = new CoachingGoals();
                if (participantReader.HasPreviousWellnessData(wellness.User.Id))
                {
                    request.coaching_goals.healthy_diet = participantReader.GetPreviousWellnessData(wellness.User.Id, "NewHDGoal");
                    request.coaching_goals.limiting_alcohol = participantReader.GetPreviousWellnessData(wellness.User.Id, "NewAlcoholGoal");
                    request.coaching_goals.managing_stress = participantReader.GetPreviousWellnessData(wellness.User.Id, "NewStressGoal");
                    request.coaching_goals.managing_weight = participantReader.GetPreviousWellnessData(wellness.User.Id, "NewWeightGoal");
                    request.coaching_goals.monitoring_bp = participantReader.GetPreviousWellnessData(wellness.User.Id, "NewBPMonitoringGoal");
                    request.coaching_goals.other = participantReader.GetPreviousWellnessData(wellness.User.Id, "OtherGoals");
                    request.coaching_goals.physical_activity = participantReader.GetPreviousWellnessData(wellness.User.Id, "NewPAGoal");
                    request.coaching_goals.quit_smoking = participantReader.GetPreviousWellnessData(wellness.User.Id, "NewSmokingGoal");
                    request.coaching_goals.taking_bp_medications = participantReader.GetPreviousWellnessData(wellness.User.Id, "NewBPMedPrescribed");
                    var min = participantReader.GetPreviousWellnessData(wellness.User.Id, "MinGoal");
                    if (min != null)
                        request.coaching_goals.physical_activity_minutes = int.Parse(min);
                    var step = participantReader.GetPreviousWellnessData(wellness.User.Id, "StepGoal");
                    if (step != null)
                        request.coaching_goals.physical_activity_steps = int.Parse(step);
                }
                else
                {
                    if (!string.IsNullOrEmpty(data.NewHDGoal))
                        request.coaching_goals.healthy_diet = data.NewHDGoal;
                    if (!string.IsNullOrEmpty(data.NewAlcoholGoal))
                        request.coaching_goals.limiting_alcohol = data.NewAlcoholGoal;
                    if (!string.IsNullOrEmpty(data.NewStressGoal))
                        request.coaching_goals.managing_stress = data.NewStressGoal;
                    if (!string.IsNullOrEmpty(data.NewWeightGoal))
                        request.coaching_goals.managing_weight = data.NewWeightGoal;
                    if (!string.IsNullOrEmpty(data.NewBPMonitoringGoal))
                        request.coaching_goals.monitoring_bp = data.NewBPMonitoringGoal;
                    if (!string.IsNullOrEmpty(data.OtherGoals))
                        request.coaching_goals.other = data.OtherGoals;
                    if (!string.IsNullOrEmpty(data.NewPAGoal))
                        request.coaching_goals.physical_activity = data.NewPAGoal;
                    if (!string.IsNullOrEmpty(data.NewSmokingGoal))
                        request.coaching_goals.quit_smoking = data.NewSmokingGoal;
                    if (!string.IsNullOrEmpty(data.NewBPMedPrescribed))
                        request.coaching_goals.taking_bp_medications = data.NewBPMedPrescribed;
                    if (data.MinGoal.HasValue)
                        request.coaching_goals.physical_activity_minutes = data.MinGoal.Value;
                    if (data.StepGoal.HasValue)
                        request.coaching_goals.physical_activity_steps = data.StepGoal.Value;
                }

                request.reviewed_coaching_goals = new ReviewedCoachingGoals();
                if (data.HealthyDiet.HasValue)
                    request.reviewed_coaching_goals.healthy_diet = data.HealthyDiet.HasValue;
                if (data.LimitingAlcohol.HasValue)
                    request.reviewed_coaching_goals.limiting_alcohol = data.LimitingAlcohol.HasValue;
                if (wellness.ManageStress.HasValue)
                    request.reviewed_coaching_goals.managing_stress = wellness.ManageStress.HasValue;
                if (data.ManagingWeight.HasValue)
                    request.reviewed_coaching_goals.managing_weight = data.ManagingWeight.HasValue;
                if (data.MonitoredBP.HasValue)
                    request.reviewed_coaching_goals.monitoring_bp = data.MonitoringBP.HasValue;
                if (!string.IsNullOrEmpty(data.OtherGoals))
                    request.reviewed_coaching_goals.other = true;
                if (wellness.PhysicallyActive.HasValue)
                    request.reviewed_coaching_goals.physical_activity = wellness.PhysicallyActive.HasValue;
                if (data.QuitSmoking.HasValue)
                    request.reviewed_coaching_goals.quit_smoking = data.QuitSmoking.HasValue;
                if (data.TakingBPMed.HasValue)
                    request.reviewed_coaching_goals.taking_bp_medications = data.TakingBPMed.HasValue;

                request.reviewed_bp_trends = new ReviewedBpTrends
                {
                    sbp_goal_attained = wellness.SBP.HasValue ? wellness.SBP.Value < 130 ? "Yes" : "No" : null
                };

                request.reviewed_bp_monitoring_trends = new ReviewedBpMonitoringTrends();
                request.reviewed_bp_monitoring_trends.if_not_achieved_why = AddOtherText(data.NotMonitoredReason, data.NotMonitoredReasonText);
                request.reviewed_bp_monitoring_trends.if_achieved_whats_helpful = AddOtherText(data.MonitoredBPHelpful, data.MonitoredBPHelpfulText);
                if (data.MonitoredBP.HasValue)
                {
                    request.reviewed_bp_monitoring_trends.achieved_6_days_per_week = data.MonitoredBP.Value;
                    if (data.MonitoredBPDays.HasValue)
                        request.reviewed_bp_monitoring_trends.if_not_achieved_num_days = data.MonitoredBPDays.Value;
                }

                request.reviewed_bp_medications = new ReviewedBpMedications();
                if (data.CurrentBPMed.HasValue)
                {
                    request.reviewed_bp_medications.current = data.CurrentBPMed.Value;
                    if (data.CurrentBPMed.Value && data.MedicationChanges.HasValue)
                    {
                        request.reviewed_bp_medications.any_changes = data.MedicationChanges.Value;
                        if (data.MedicationChanges.Value)
                            request.reviewed_bp_medications.list_changes = data.MedListChanges;
                    }
                }

                request.reviewed_bp_medication_adherence = new ReviewedBpMedicationAdherence();
                request.reviewed_bp_medication_adherence.if_not_why = AddOtherText(data.ReasonNotTakingMed, data.ReasonNotTakingMedText);
                if (data.TakingMedication.HasValue)
                {
                    request.reviewed_bp_medication_adherence.taking_as_prescribed = data.TakingMedication.Value;
                    if (data.MissedMed.HasValue)
                        request.reviewed_bp_medication_adherence.if_not_num_days_missed = data.MissedMed.Value;
                }

                request.reviewed_physical_activity_trends = new ReviewedPhysicalActivityTrends
                {
                    avg_minutes_day_reviewed = data.AvgMinutesDayReviewed.HasValue && data.AvgMinutesDayReviewed.Value,
                    avg_steps_day_reviewed = data.AvgStepsDayReviewed.HasValue && data.AvgStepsDayReviewed.Value,
                    if_goals_achieved_why = AddOtherText(data.YAcheivedGoal, data.YAcheivedGoalText),
                    if_goals_not_achieved_why = AddOtherText(data.YNotAcheivedGoal, data.YNotAcheivedGoalText),
                    mins_goal_attained = data.MinutesGoalAttained.HasValue && data.MinutesGoalAttained.Value,
                    steps_goal_attained = data.StepsGoalAttained.HasValue && data.StepsGoalAttained.Value
                };

                request.reviewed_sodium_intake = new ReviewedSodiumIntake();
                request.reviewed_sodium_intake.if_not_why = AddOtherText(data.ReasonNotFollowedLSD, data.ReasonNotFollowedLSDText);
                if (data.FollowedLSD.HasValue)
                {
                    request.reviewed_sodium_intake.followed_low_sodium_diet = data.FollowedLSD.Value;
                }
                if (data.ReviewedProblemList.HasValue)
                    request.reviewed_problem_list = data.ReviewedProblemList.Value;

                request.reviewed_ed_visits = new ReviewedEdVisits();
                if (data.HasVisitedED.HasValue)
                {
                    request.reviewed_ed_visits.has_visited_ed = data.HasVisitedED.Value;
                    if (data.HasVisitedED.Value && data.EDVisitsNumber.HasValue)
                        request.reviewed_ed_visits.number_of_visits = data.EDVisitsNumber.Value;
                }
                request.reviewed_ed_visits.ed_visit_list = ed_visit_list;

                request.reviewed_hospital_admits = new ReviewedHospitalAdmits();
                if (data.WasAdmittedHospital.HasValue)
                {
                    request.reviewed_hospital_admits.was_admitted_hospital = data.WasAdmittedHospital.Value;
                    if (data.WasAdmittedHospital.Value && data.HospitalVisitsNumber.HasValue)
                        request.reviewed_hospital_admits.number_of_visits = data.HospitalVisitsNumber.Value;
                }
                request.reviewed_hospital_admits.hospital_admits_list = hospital_admits_list;

                request.reviewed_falls = new ReviewedFalls();
                if (data.HasFallen.HasValue)
                {
                    request.reviewed_falls.has_fallen = data.HasFallen.Value;
                    if (data.HasFallen.Value && data.NumberOfFalls.HasValue)
                        request.reviewed_falls.number_of_falls = data.NumberOfFalls;
                }
                request.reviewed_falls.falls_list = falls_list;

                request.reviewed_rehab_therapy = new ReviewedRehabTherapy();
                request.reviewed_rehab_therapy.if_not_why = AddOtherText(data.NotAttendingReason, data.NotAttendingReasonText);
                if (data.ReferredByProvider.HasValue)
                {
                    request.reviewed_rehab_therapy.referred_by_provider = data.ReferredByProvider.Value;
                    if (!data.ReferredByProvider.Value && data.NeedsProviderReferral.HasValue)
                        request.reviewed_rehab_therapy.needs_provider_referral = data.NeedsProviderReferral.Value;
                    if (data.ReferredByProvider.Value && data.AttendingAsScheduled.HasValue)
                    {
                        request.reviewed_rehab_therapy.attending_as_scheduled = data.AttendingAsScheduled.Value;
                    }
                }

                request.community_or_social_services_utilized = data.CommunityUsing;
                request.emergent_resources_needed = data.CommunityNeeded;
                if (data.ReviewedNeurology.HasValue)
                    request.reviewed_neurology_follow_up_appointments = data.ReviewedNeurology.Value;
                if (data.BPTriage.HasValue)
                    request.reviewed_triage_escalation_process = data.BPTriage.Value;

                request.updated_coaching_goals = new UpdatedCoachingGoals();
                if (!string.IsNullOrEmpty(data.NewHDGoal))
                    request.updated_coaching_goals.healthy_diet = data.NewHDGoal;
                if (!string.IsNullOrEmpty(data.NewAlcoholGoal))
                    request.updated_coaching_goals.limiting_alcohol = data.NewAlcoholGoal;
                if (!string.IsNullOrEmpty(data.NewStressGoal))
                    request.updated_coaching_goals.managing_stress = data.NewStressGoal;
                if (!string.IsNullOrEmpty(data.NewWeightGoal))
                    request.updated_coaching_goals.managing_weight = data.NewWeightGoal;
                if (!string.IsNullOrEmpty(data.NewBPMonitoringGoal))
                    request.updated_coaching_goals.monitoring_bp = data.NewBPMonitoringGoal;
                if (!string.IsNullOrEmpty(data.OtherGoals))
                    request.updated_coaching_goals.other = data.OtherGoals;
                if (!string.IsNullOrEmpty(data.NewPAGoal))
                    request.updated_coaching_goals.physical_activity = data.NewPAGoal;
                if (!string.IsNullOrEmpty(data.NewSmokingGoal))
                    request.updated_coaching_goals.quit_smoking = data.NewSmokingGoal;
                if (!string.IsNullOrEmpty(data.NewBPMedPrescribed))
                    request.updated_coaching_goals.taking_bp_medications = data.NewBPMedPrescribed;
                if (data.MinGoal.HasValue)
                    request.updated_coaching_goals.physical_activity_minutes = data.MinGoal.Value;
                if (data.StepGoal.HasValue)
                    request.updated_coaching_goals.physical_activity_steps = data.StepGoal.Value;

                request.next_coaching_appointment = new NextCoachingAppointment();
                if (futureAppointment != null)
                {
                    request.next_coaching_appointment.confirmed = true;
                    request.next_coaching_appointment.date = DateTime.Parse(futureAppointment.Date).ToString("yyyy-MM-dd");
                    request.next_coaching_appointment.method = futureAppointment.VideoRequired ? Video : Telephone;
                    request.next_coaching_appointment.time = DateTime.Parse(futureAppointment.Date).ToString("HH:mm") + " UTC";
                }
                return request;
            }
            else
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "SendCoachingNotes", "Missing appointment for billing notes : " + billingDetails.Id);
                logreader.WriteLogMessage(logEvent);
                return null;
            }
        }

        public string AddOtherText(string value, string other)
        {
            if (!string.IsNullOrEmpty(value))
                if (value.Contains("(text)") && !string.IsNullOrEmpty(other))
                    return value.Replace("(text)", "(" + other + ")");
            return value;
        }
    }
}
