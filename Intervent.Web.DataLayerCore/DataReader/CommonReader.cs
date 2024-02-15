using Google.Apis.Auth.OAuth2;
using Intervent.DAL;
using Intervent.HWS.Model.FCM;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NLog;
using OfficeOpenXml;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Intervent.Web.DataLayer
{
    public class IncentiveMessageTypes
    {
        public const string HRA_Incentive = "HRA_Incentive";
        public const string PP_Incentive = "PP_Incentive";
        public const string TC_Incentive = "TC_Incentive";
        public const string Incentive = "Incentive";
        public const string Custom_Incentive = "Custom_Incentive";
        public const string Follow_Up = "Follow Up";
        public const string Satisfaction_Survey = "Satisfaction_Survey";
        public const string FollowUp_Incentive = "FollowUp_Incentive";
        public const string Navigation_Incentive = "Navigation_Incentive";
        public const string HRA_HealthNumbersIncentive = "HealthNumbers_Incentive";
        public const string Biometric_ReviewIncentive = "BiometricReview_Incentive";
        public const string New_Kit = "New Kit";
        public const string Audio_Kit = "Kit Audio";
        public const string Followup_Report = "Followup_Report";
        public const string Appointment = "Appointment";
        public const string AssignRecipe = "Assign Recipe";
        public const string Lab = "Lab";
        public const string LabResults = "Lab Results";
        public const string DPP_Eligibile = "DPP_Eligibile";
        public const string DPP_Ineligibile = "DPP_Ineligibile";
        public const string DPP_Enroll = "DPP_Enroll";
        public const string Program_Enrollment = "Program_Enrollment";
        public const string Program_Ineligible = "Program_Ineligible";
        public const string New_Message = "New_Message";
        public const string KitCompletionStayOnTrack = "Kit_Completion_Stay_On_Track";
        public const string KitCompletionKnowingIsNotDoing = "Kit_Completion_Knowing_Is_Not_Doing";
        public const string KitCompletionRecommitToYourHealth = "Kit_Completion_Recommit_To_Your_Health";
        public const string KitCompletionDontDelayAnyLonger = "Kit_Completion_Dont_Delay_Any_Longer";
        public const string Motivation = "Motivation";
        public const string Profile_Complete = "Profile_Complete";
        public const string Newsletter = "Newsletter";
        public const string Motivation_Message = "Motivation_Message";
    }


    public class CommonConstants
    {
        public const string TableOptionText = "option";

        public const int RowSpanOption = 1;

        public const int ColSpanOption = 2;

        public const int StyleOption = 3;
    }

    public class LMCTaskTypeId
    {
        public const int AbnormalLabResult = 24;

        public const int ElevatedA1FBG = 23;

        public const int ParticipantEnrolled = 22;

        public const int ParticipantScreening = 21;
    }

    public enum QuestionType
    {
        textbox = 1,
        radiobutton = 2,
        checkbox = 3,
        dropdown = 4,
        expression = 5,
        table = 6,
        tablecheckbox = 7,
        number = 8,
        floatnumber = 9,
        none = 0,
        textarea = 11
    }

    public enum LabChoices
    {
        DoctorsOffice = 1,
        LabCorp = 2,
        None = 3
    }

    public enum Unit
    {
        Imperial = 1,
        Metric = 2
    }

    public enum IntegrationPartner
    {
        Intuity = 1,
        Activate = 2,
        LMC = 3,
        Captiva = 4
    }

    public enum GlucoseSource
    {
        Validic = 1,
        Livongo = 2,
        Intuity = 3,
        Dexcom = 4,
        GoogleFit = 5,
        AppleHealth = 6
    }

    public enum Measurements
    {
        Cholesterol = 0,
        Triglycerides = 1,
        HDL = 2,
        LDL = 3,
        Glucose = 4,
        Weight = 5,
        Height = 6,
        Waist = 7,
        SBP = 8,
        DBP = 9,
        A1C = 10
    }

    public enum SelfSchedulingLimit
    {
        LMC = 7
    }
    public enum TagType
    {
        Other = 1,
        Course = 2,
        Cuisine = 3,
        Dietary = 4
    }

    public enum NoteTypes
    {
        Coaching = 1,
        Outreach = 2,
        Tracking = 3,
        Note = 4,
        Other = 5,
        OtherReferrals = 6,
        BiometricReview = 7,
        PCPReferral = 8,
        Navigation_Call = 9,
        Critical_Alert = 10,
        HRA_Review = 11,
        Complaint = 12,
        MarketingFeedback = 13
    }

    public enum ProgramTypes
    {
        SelfHelp = 1,
        Coaching = 2
    }

    public enum FormType
    {
        PatientRelease = 1,
        MedicalClearance = 2
    }

    public enum KitTopic
    {
        Questionnaires = 8
    }

    public class AccessToken
    {
        public const string Intuity = "Intuity";

        public const string Token = "AccessToken";

        public const string RefreshToken = "RefreshToken";

        public const string Webinar = "Webinar";
    }

    public class CommonReader
    {
        private string EncryptionKey = "zbbIaxkTpCnoKB6kCwqPXmZ5p5X0kamUMFPEwCip";
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public static double GetBMI(float height, float weight)
        {
            double BMI = 0;
            if (height > 0 && weight > 0)
            {
                var newWeight = weight / 2.2;
                var newHeight = height / 39.37;
                BMI = Math.Round(newWeight / (newHeight * newHeight), 1);
            }
            return BMI;
        }

        public List<MeasurementsDto> MeasurementRange()
        {
            IList<MeasurementsDto> measurements = ListMeasurements((int)Unit.Imperial, GetMeasurements()).Measurements;
            return measurements.ToList();
        }

        public ListMeasurementsResponse ListMeasurements(int measurementType, IEnumerable<dynamic> Measurements)
        {
            ListMeasurementsResponse response = new ListMeasurementsResponse();
            List<MeasurementsDto> measurements = new List<MeasurementsDto>();
            foreach (var measurement in Measurements)
            {
                if (measurementType == (int)Unit.Imperial)
                {
                    MeasurementsDto Measurement = new MeasurementsDto();
                    Measurement.Name = measurement.Name;
                    Measurement.MeasurementUnit = measurement.ImperialUnits;
                    Measurement.Min = measurement.ImperialMin;
                    Measurement.Max = measurement.ImperialMax;
                    Measurement.Limit = measurement.ImperialLimit;
                    Measurement.ConversionValue = measurement.ConversionValue;
                    measurements.Add(Measurement);
                }
                else if (measurementType == (int)Unit.Metric)
                {
                    MeasurementsDto Measurement = new MeasurementsDto();
                    Measurement.Name = measurement.Name;
                    Measurement.MeasurementUnit = measurement.MetricUnits;
                    if (Measurement.Name.Equals("Weight"))
                    {
                        Measurement.Min = (float?)Math.Round(measurement.MetricMin, 1);
                        Measurement.Max = (float?)Math.Round(measurement.MetricMax, 1);
                    }
                    else
                    {
                        Measurement.Min = measurement.MetricMin;
                        Measurement.Max = measurement.MetricMax;
                    }
                    Measurement.Limit = measurement.MetricLimit;
                    Measurement.ConversionValue = measurement.ConversionValue;
                    measurements.Add(Measurement);
                }
            }
            response.Measurements = measurements;
            return response;
        }

        public IList<MeasurementsDto> GetMeasurements()
        {
            var measurements = context.Measurements.ToList();
            IList<MeasurementsDto> Measurements = Utility.mapper.Map<IList<DAL.Measurement>, IList<MeasurementsDto>>(measurements);
            return Measurements;
        }
        public ListCountriesResponse ListCountries(ListCountriesRequest request)
        {
            ListCountriesResponse response = new ListCountriesResponse();

            var countries = context.Countries.ToList();

            response.Countries = Utility.mapper.Map<IList<DAL.Country>, IList<CountryDto>>(countries);

            return response;
        }

        public GetCountryResponse GetCountry(GetCountryRequest request)
        {
            GetCountryResponse response = new GetCountryResponse();
            var country = context.Countries.Where(x => x.UNCode == request.code || x.Code == request.code).FirstOrDefault();
            if (country != null)
                response.country = Utility.mapper.Map<DAL.Country, CountryDto>(country);
            return response;
        }

        public ListStatesResponse ListStates(ListStatesRequest request)
        {
            ListStatesResponse response = new ListStatesResponse();

            var states = context.States.Where(x => x.CountryId == request.CountryId).OrderBy(x => x.Name).ToList();

            response.States = Utility.mapper.Map<IList<DAL.State>, IList<StateDto>>(states);

            return response;
        }

        public ListReportTypesResponse ListReportTypes(ListReportTypesRequest request)
        {
            ListReportTypesResponse response = new ListReportTypesResponse();

            var reportTypes = context.ReportTypes.Where(x => (!request.hasHRA.HasValue || (request.hasHRA.Value && x.Id == 1)) || (!request.hasFollowUp.HasValue || (request.hasFollowUp.Value && x.Id == 2)) || (!request.hasCarePlan.HasValue || (request.hasCarePlan.Value && x.Id == 3))).ToList();

            response.reportTypes = Utility.mapper.Map<IList<DAL.ReportType>, IList<ReportTypeDto>>(reportTypes);

            return response;
        }

        public ListRaceResponse ListRace(ListRaceRequest request)
        {
            ListRaceResponse response = new ListRaceResponse();

            var states = context.Races.Where(x => (!request.CountryId.HasValue) || x.CountryId == request.CountryId).ToList();

            response.Race = Utility.mapper.Map<IList<DAL.Race>, IList<RaceDto>>(states);

            return response;
        }

        public bool CheckIfOther(int type)
        {
            var race = context.Races.Where(x => x.Id == type).FirstOrDefault();
            if (race != null && race.RaceType == 4)
                return true;
            else
                return false;
        }

        public ReadBusinessUnitResponse ReadBusinessUnit(ReadBusinessUnitRequest request)
        {
            ReadBusinessUnitResponse response = new ReadBusinessUnitResponse();
            var businessUnit = context.BusinessUnits.Where(x => x.Code == request.code || x.Name == request.name).FirstOrDefault();
            if (businessUnit != null)
                response.businessUnit = Utility.mapper.Map<DAL.BusinessUnit, BusinessUnitDto>(businessUnit);
            return response;
        }

        public ListLanguagesResponse GetLanguages(string languageCode = null)
        {
            ListLanguagesResponse response = new ListLanguagesResponse();
            var languages = context.Languages.Where(x => languageCode == null || x.LanguageCode == languageCode).ToList();
            response.Languages = Utility.mapper.Map<List<DAL.Languages>, List<LanguagesDto>>(languages);
            return response;
        }

        public List<FollowUpTypeDto> GetFollowUpTypes()
        {
            var types = context.FollowUpTypes.ToList();
            var response = Utility.mapper.Map<List<DAL.FollowUpType>, List<FollowUpTypeDto>>(types);
            return response;
        }

        public ListStatesResponse ListAllStates()
        {
            ListStatesResponse response = new ListStatesResponse();
            var states = context.States.ToList();
            response.States = Utility.mapper.Map<IList<DAL.State>, IList<StateDto>>(states);
            return response;
        }

        public GetStateResponse GetState(GetStateRequest request)
        {
            GetStateResponse response = new GetStateResponse();
            var state = context.States.Where(x => x.Code == request.stateCode || x.Name == request.stateCode).FirstOrDefault();
            if (state != null)
                response.state = Utility.mapper.Map<DAL.State, StateDto>(state);
            return response;
        }

        public GetNoteTypesResponse GetNotesTypeList(GetNoteTypesRequest request)
        {
            GetNoteTypesResponse response = new GetNoteTypesResponse();
            List<DAL.NoteType> NotesType = new List<DAL.NoteType>();
            NotesType = context.NoteTypes.Where(n => (!request.isNoteExist || n.Id != (int)NoteTypes.Note) && (!request.isCoachNoteExist || n.Id != (int)NoteTypes.Coaching) && (!request.isBioNoteExist || n.Id != (int)NoteTypes.BiometricReview) && (request.showHRAReviewNote == true || n.Id != (int)NoteTypes.HRA_Review)).ToList();
            var noteTypes = Utility.mapper.Map<IList<DAL.NoteType>, IList<NoteTypesDto>>(NotesType).ToList();
            response.NoteType = noteTypes;
            return response;
        }

        public GetReferralTypesResponse GetReferralTypeList()
        {
            GetReferralTypesResponse response = new GetReferralTypesResponse();
            var ReferralTypes = context.ReferralTypes.ToList();
            var referralTypes = Utility.mapper.Map<IList<DAL.ReferralTypes>, IList<ReferralTypesDto>>(ReferralTypes).ToList();
            response.ReferralType = referralTypes;
            return response;
        }
        public GetKeyValueResponse GetQuestionTypes()
        {
            GetKeyValueResponse questionTypes = new GetKeyValueResponse();
            questionTypes.KeyValueList = new List<KeyValueDTO>();
            foreach (QuestionType qt in Enum.GetValues(typeof(QuestionType)).Cast<QuestionType>().ToList())
                questionTypes.KeyValueList.Add(new KeyValueDTO(qt.ToString(), ((int)qt).ToString()));
            return questionTypes;
        }

        public GetKeyValueResponse GetTagTypes()
        {
            GetKeyValueResponse questionTypes = new GetKeyValueResponse();
            questionTypes.KeyValueList = new List<KeyValueDTO>();
            foreach (TagType qt in Enum.GetValues(typeof(TagType)).Cast<TagType>().ToList())
                questionTypes.KeyValueList.Add(new KeyValueDTO(qt.ToString(), ((int)qt).ToString()));
            return questionTypes;
        }

        public int? GetTimeZoneByLocationCode(string location)
        {
            int? id = null;

            if (!string.IsNullOrEmpty(location))
            {
                var timeZone = context.TimeZones.Where(x => x.TimeZoneLocation == location).FirstOrDefault();
                if (timeZone != null)
                {
                    id = timeZone.Id;
                }
            }
            return id;
        }

        public int? GetTimeZoneByTimeZoneId(string TimeZoneId)
        {
            int? id = null;

            if (!string.IsNullOrEmpty(TimeZoneId))
            {
                var timeZone = context.TimeZones.Where(x => x.TimeZoneId == TimeZoneId).FirstOrDefault();
                if (timeZone != null)
                {
                    id = timeZone.Id;
                }
            }
            return id;
        }

        public ReadTimeZonesResponse GetTimeZones(ReadTimeZonesRequest request)
        {
            ReadTimeZonesResponse response = new ReadTimeZonesResponse();
            IList<DAL.TimeZone> timeZones;
            if (request.Id.HasValue)
                timeZones = context.TimeZones.Where(x => x.Id == request.Id).ToArray();
            else
                timeZones = context.TimeZones.Where(x => !string.IsNullOrEmpty(x.CountryFilterList)).ToArray();
            response.TimeZones = Utility.mapper.Map<IList<DAL.TimeZone>, IList<TimeZoneDto>>(timeZones);
            return response;
        }

        public GetProfessionsResponse GetProfessions(GetProfessionsRequest request)
        {
            GetProfessionsResponse response = new GetProfessionsResponse();
            var professions = context.Professions.ToList();
            response.Professions = Utility.mapper.Map<IList<DAL.Profession>, IList<ProfessionDto>>(professions);
            return response;
        }

        public void AddDashboardMessage(int userId, string type, string url, int? relatedId, params object[] args)
        {
            var dashboardMessage = context.DashboardMessageTypes.Where(x => x.Type == type).FirstOrDefault();
            List<UserLoggedInDevicesDto> deviceDetails = new MobileReader().GetUserNotificationDevices(userId);
            if (dashboardMessage != null)
            {
                UserDashboardMessage userMessage = new UserDashboardMessage();
                userMessage.UserId = userId;
                if (args.Count() != 0)
                {
                    var arg = "";
                    for (var i = 0; i < args.Count(); i++)
                    {
                        if (args.Count() > 1 && args[1] != null)
                        {
                            arg = arg + "|" + args[i].ToString();
                        }
                        else if (args[i] != null)
                        {
                            arg = args[i].ToString();
                        }
                    }
                    if (args.Count() > 1 && args[1] != null)
                    {
                        userMessage.Parameters = arg.Substring(1);
                    }
                    else
                    {
                        userMessage.Parameters = arg;
                    }
                }
                if (url != null)
                    userMessage.Url = url;
                else if (dashboardMessage.Url != null)
                    userMessage.Url = dashboardMessage.Url;
                if (relatedId.HasValue)
                    userMessage.RelatedId = relatedId;
                userMessage.New = true;
                userMessage.CreatedOn = DateTime.UtcNow;
                userMessage.MessageType = dashboardMessage.Id;
                userMessage.Active = true;
                userMessage.Status = deviceDetails.Count() > 0 ? (byte)MessageStatus.NotSent : (byte)MessageStatus.DoNotSend;
                context.UserDashboardMessages.Add(userMessage);
                context.SaveChanges();

                bool requiresHttps;
                bool.TryParse(ConfigurationManager.AppSettings["RequiresHttps"], out requiresHttps);
                /* if (requiresHttps && userMessage.Status == (byte)MessageStatus.NotSent && userMessage.DashboardMessageType.NotificationType != (int)NotificationTypes.Feed)
                 {
                     var user = deviceDetails.FirstOrDefault().User;
                     string language = string.IsNullOrEmpty(user.LanguagePreference) ? "en-US" : user.LanguagePreference.ToLower();
                     if (userMessage.RelatedId.HasValue && userMessage.DashboardMessageType.Type == "Appointment")
                     {
                         SchedulerReader schedulerReader = new SchedulerReader();
                         string timeZone = string.IsNullOrEmpty(user.TimeZone) ? "Eastern Standard Time" : user.TimeZone;
                         var dateFormat = user.Country1 != null ? user.Country1.DateFormat : "MM/dd/yyyy";
                         var appointment = schedulerReader.GetAppointmentDetails(new GetAppointmentDetailsRequest { apptId = userMessage.RelatedId.Value, timeZone = timeZone }).appointment;
                         DateTime time = DateTime.Parse(appointment.Date);
                         userMessage.Message = string.Format(GlobalTranslator.Message("" + userMessage.DashboardMessageType.LanguageItem + "", language), time.ToString(dateFormat + " hh:mm tt") + " (" + timeZone + ")");
                     }
                     else
                     {
                         if (userMessage.Parameters != null)
                         {
                             var param = userMessage.Parameters.Split('|');
                             for (var j = 0; j < param.Length; j++)
                             {
                                 param[j] = GlobalTranslator.Message("" + param[j] + "", language);
                             }
                             userMessage.Message = string.Format(GlobalTranslator.Message("" + userMessage.DashboardMessageType.LanguageItem + "", language), param);
                         }
                         else if (userMessage.Message != null && userMessage.Parameters == null)
                         {
                             userMessage.Message = userMessage.Message;
                         }
                         else
                         {
                             userMessage.Message = GlobalTranslator.Message("" + userMessage.DashboardMessageType.LanguageItem + "", language);
                         }
                     }
                     foreach (var device in deviceDetails)
                     {
                         SendNotification(userId, device.DeviceId, device.Token, userMessage.DashboardMessageType.Subject, userMessage.Message, userMessage.Id);
                     }
                 }*/
            }
        }

        public bool SendNotification(int userId, string deviceId, string DeviceToken, string title, string msg, int msgId)
        {
            LogReader logreader = new LogReader();
            try
            {
                string fileName = "";//System.Web.Hosting.HostingEnvironment.MapPath("~/intervent-mobile-apps-firebase-adminsdk-9k8vh-d3a3410a49.json"); string scopes = "https://www.googleapis.com/auth/firebase.messaging";
                string scopes = "https://www.googleapis.com/auth/firebase.messaging"; 
                var bearertoken = "";
                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    bearertoken = GoogleCredential
                      .FromStream(stream)
                      .CreateScoped(scopes)
                      .UnderlyingCredential
                      .GetAccessTokenForRequestAsync().Result;
                }

                FCMSendNotification fcmMsg = new FCMSendNotification();
                fcmMsg.message = new HWS.Model.FCM.Message();
                fcmMsg.message.token = DeviceToken;
                fcmMsg.message.data = new Data();
                fcmMsg.message.data.title = title;
                fcmMsg.message.data.body = msg;
                fcmMsg.message.notification = new Notification();
                fcmMsg.message.notification.title = title;
                fcmMsg.message.notification.body = msg;

                var jsonObj = JsonSerializer.Serialize(fcmMsg);
                var data = new StringContent(jsonObj, Encoding.UTF8, "application/json");

                var clientHandler = new HttpClientHandler();
                var client = new HttpClient(clientHandler);
                client.BaseAddress = new Uri("https://fcm.googleapis.com/v1/projects/intervent-mobile-apps/messages:send");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearertoken);
                data.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = client.PostAsync("https://fcm.googleapis.com/v1/projects/intervent-mobile-apps/messages:send", data).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return new ParticipantReader().UpdateDashboardMessage(new UpdateDashboardMessageRequest { id = msgId, Status = (byte)MessageStatus.Sent }).success;
                }
                var jsonResponse = response.Content.ReadAsStringAsync().Result;
                logreader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "Send Notification Service", null, "Error while send notification to user (" + userId + ") device : " + deviceId + ", Message : " + jsonResponse, null, null));
            }
            catch (Exception ex)
            {
                var logEvent = new LogEventInfo(LogLevel.Error, "Send Notification Service", null, "Error while send notification to user (" + userId + ") device : " + deviceId, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
            return false;
        }
        public GetExercisePlanResponse GetExercisePlan(GetExercisePlanRequest request)
        {

            GetExercisePlanResponse ExercisePlanResult = new GetExercisePlanResponse();
            List<HRA_ExercisePlan> Planresults = context.HRA_ExercisePlans.Where(x => x.HraId == request.hraId).ToList();
            ExercisePlanResult.ExercisePlan = Utility.mapper.Map<List<HRA_ExercisePlan>, List<ExercisePlanDto>>(Planresults);
            return ExercisePlanResult;
        }

        public bool DeleteIncentiveMessages(PortalIncentiveDto portalIncentive, int userId, string incentiveType)
        {
            var dashboardMessages = context.DashboardMessageTypes.Where(x => x.Type == incentiveType).ToList();
            foreach (var message in dashboardMessages)
            {
                List<DAL.UserDashboardMessage> userMessages = context.UserDashboardMessages.Where(x => x.MessageType == message.Id && x.UserId == userId && x.Active == true).ToList();
                if (userMessages != null && userMessages.Count() > 0)
                {
                    DAL.UserDashboardMessage userDashboardMessage = null;
                    if (incentiveType == IncentiveMessageTypes.Incentive)
                    {
                        userDashboardMessage = userMessages.Where(u => (u.Message != null && u.Message.Contains(portalIncentive.Points + " ")) || (u.Message == null && u.Parameters == portalIncentive.Points.ToString())).FirstOrDefault();
                        if (userDashboardMessage == null && message.languageCode == "es")
                            userDashboardMessage = userMessages.FirstOrDefault();
                    }
                    else
                    {
                        userDashboardMessage = userMessages.FirstOrDefault();
                    }

                    if (userDashboardMessage != null)
                    {
                        userDashboardMessage.Active = false;
                        context.UserDashboardMessages.Attach(userDashboardMessage);
                        context.Entry(userDashboardMessage).State = EntityState.Modified;
                        context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        public ReadMedicalPlanResponse ReadMedicalPlan(ReadMedicalPlanRequest request)
        {
            ReadMedicalPlanResponse response = new ReadMedicalPlanResponse();
            var medicalPlanCode = context.MedicalPlanCodes.Where(x => x.Code == request.code).FirstOrDefault();
            response.medicalPlanCode = Utility.mapper.Map<DAL.MedicalPlanCode, MedicalPlanCodeDto>(medicalPlanCode);
            return response;
        }

        public void UpdateLastVisited(UpdateLastVisitedRequest request)
        {
            var user = context.Users.Where(x => x.Id == request.UserId).FirstOrDefault();
            if (user != null)
            {
                user.LastVisited = request.url;
                context.SaveChanges();
            }
        }

        public GetPregnencyStatusResponse GetPregnencyStatus(int userId, int hraId)
        {
            GetPregnencyStatusResponse response = new GetPregnencyStatusResponse();
            DateTime? hraStartDate = null;
            if (hraId != 0)
            {
                var hra = context.HRAs.Include("HRA_MedicalConditions").Where(x => x.Id == hraId && x.HRA_MedicalConditions.Pregnant == 1 && x.HRA_MedicalConditions.PregDueDate.HasValue && x.HRA_MedicalConditions.PregDueDate > DateTime.UtcNow).FirstOrDefault();
                if (hra != null)
                {
                    hraStartDate = hra.StartDate;
                    response.pregDueDate = hra.HRA_MedicalConditions.PregDueDate.Value;
                    response.isPregnant = true;
                }
            }
            var wellnessData = context.WellnessDatas.Where(x => x.UserId == userId && (hraStartDate == null || x.UpdatedOn >= hraStartDate)).OrderByDescending(x => x.UpdatedOn).FirstOrDefault();
            if (wellnessData != null && wellnessData.isPregnant.HasValue && wellnessData.isPregnant.Value && wellnessData.DueDate.HasValue && wellnessData.DueDate > DateTime.UtcNow)
            {
                response.pregDueDate = wellnessData.DueDate.Value;
                response.isPregnant = true;
            }
            else if (wellnessData != null)
            {
                response.pregDueDate = null;
                response.isPregnant = false;
            }
            return response;
        }

        public void ConverttoExcel(string csvFilePath, string excelFilePath, string worksheetsName, char delimiter, int[] dateColumnIndexesToFormat = null, Dictionary<int, string> textColumnIndexesToFormat = null)
        {
            try
            {
                bool firstRowIsHeader = true;

                var excelTextFormat = new ExcelTextFormat();
                excelTextFormat.Delimiter = delimiter;
                excelTextFormat.EOL = "\r";

                var excelFileInfo = new FileInfo(excelFilePath);
                var csvFileInfo = new FileInfo(csvFilePath);

                using (ExcelPackage package = new ExcelPackage(excelFileInfo))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetsName);
                    worksheet.Cells["A1"].LoadFromText(csvFileInfo, excelTextFormat, OfficeOpenXml.Table.TableStyles.None, firstRowIsHeader);
                    if (dateColumnIndexesToFormat != null && dateColumnIndexesToFormat.Any())
                    {
                        foreach (int i in dateColumnIndexesToFormat)
                        {
                            worksheet.Column(i).Style.Numberformat.Format = "yyyy-MM-dd";
                        }
                    }
                    if (textColumnIndexesToFormat != null && textColumnIndexesToFormat.Count > 0)
                    {
                        foreach (KeyValuePair<int, string> keyValue in textColumnIndexesToFormat)
                        {
                            worksheet.Column(keyValue.Key).Style.Numberformat.Format = keyValue.Value;
                        }
                    }
                    package.Save();
                }
            }
            catch
            {

            }
        }

        public List<RaffleTypesDto> GetRaffleTypes()
        {
            List<RaffleTypesDto> raffleTypes = new List<RaffleTypesDto>();
            var raffles = context.RaffleTypes.ToList();
            raffleTypes = Utility.mapper.Map<List<DAL.RaffleTypes>, List<RaffleTypesDto>>(raffles);
            return raffleTypes;
        }


        public List<PortalIncentiveDto> GetGiftCards(int incentiveTypeId, int portalId)
        {
            List<PortalIncentiveDto> giftCards = new List<PortalIncentiveDto>();
            var incentives = context.PortalIncentives.Where(x => x.PortalId == portalId && x.IncentiveTypeId == incentiveTypeId && x.IsActive).ToList();
            giftCards = Utility.mapper.Map<List<DAL.PortalIncentive>, List<PortalIncentiveDto>>(incentives);
            return giftCards;
        }

        public int GetAge(DateTime birthday)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - birthday.Year;
            if (now < birthday.AddYears(age)) age--;

            return age;
        }

        public static DAL.UserDashboardMessage MapToUserDashboardDAL(UserDashboardMessageDto source, DAL.UserDashboardMessage target)
        {
            if (source == null)
                return target;
            target.Parameters = source.Parameters;
            target.Url = source.Url;
            target.New = source.New;
            target.UserId = source.UserId;
            target.CreatedOn = source.CreatedOn;
            target.MessageType = source.MessageType;
            target.RelatedId = source.RelatedId;
            target.Active = source.Active;
            target.Status = new MobileReader().GetUserNotificationDevices(source.UserId).Count() > 0 ? (byte)MessageStatus.NotSent : (byte)MessageStatus.DoNotSend;
            return target;
        }

        public static List<int> GetLMCTaskTypes()
        {
            return new List<int>() { LMCTaskTypeId.AbnormalLabResult, LMCTaskTypeId.ElevatedA1FBG, LMCTaskTypeId.ParticipantEnrolled, LMCTaskTypeId.ParticipantScreening };
        }

        public static string GetGenderSpecificImage(User user)
        {
            return user.Gender == 1 ? "avatar-male.svg" : "avatar-female.svg";
        }

        public ListDepartmentResponse ListDepartments(ListDepartmentRequest request)
        {
            ListDepartmentResponse response = new ListDepartmentResponse();
            var departments = context.Departments.Where(x => x.OrganizationId == request.organizatioId).ToList();
            if (departments != null)
                response.Departments = Utility.mapper.Map<List<DAL.Department>, List<DepartmentDto>>(departments);
            return response;
        }

        public string GetAccessToken(GetAccessTokenRequest request)
        {
            var token = context.AccessTokens.Where(x => x.OrgName.Equals(request.OrgName) && x.Name.Equals(request.AccessTokenName)).FirstOrDefault();
            return (token != null && token.Token != null) ? token.Token : "";
        }

        public static class BillingServiceTypes
        {
            public const string HealthCoaching = "Health Coaching";

            public const string MonitorAndSupplies = "Glucose Monitoring";
        }

        public List<ProviderDto> GetProvidersList(int orgId)
        {
            var providers = context.Providers.Where(x => x.OrgId == orgId).ToList();
            return Utility.mapper.Map<List<DAL.Provider>, List<ProviderDto>>(providers);
        }

        public string Encrypt(string encryptString)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                cipherText = cipherText.Replace(" ", "+");
                if (IsBase64(cipherText))
                {

                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    using (Aes encryptor = Aes.Create())
                    {
                        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(cipherBytes, 0, cipherBytes.Length);
                                cs.Close();
                            }
                            cipherText = Encoding.Unicode.GetString(ms.ToArray());
                        }
                    }
                    return cipherText;
                }

            }
            catch { }
            return null;
        }

        public bool IsBase64(string base64String)
        {
            if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0 || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
                return false;
            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch { }
            return false;
        }
    }
}