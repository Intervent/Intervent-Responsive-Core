using Intervent.DAL;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Microsoft.AspNetCore.Identity;
using NLog;
using System.Security.Claims;

namespace InterventWebApp
{
    public class MobileUtility
    {
        public static UserIdentity GetUserSession(ClaimsIdentity identity)
        {
            try
            {
                if (identity == null)
                    return null;
                if (identity.AuthenticationType != "Bearer")
                    return null;

                return new UserIdentity
                {
                    UserId = int.Parse(identity.Claims.Where(c => c.Type == "UserId").Select(c => c.Value).FirstOrDefault()),
                    FullName = identity.Claims.Any(c => c.Type == "FullName") ? identity.Claims.Where(c => c.Type == "FullName").Select(c => c.Value).FirstOrDefault() : null,
                    Module = identity.Claims.Any(c => c.Type == "Module") ? identity.Claims.Where(c => c.Type == "Module").Select(c => c.Value).FirstOrDefault() : null,
                    RoleCode = identity.Claims.Any(c => c.Type == "RoleCode") ? identity.Claims.Where(c => c.Type == "RoleCode").Select(c => c.Value).FirstOrDefault() : null,
                    ExpirationUrl = identity.Claims.Any(c => c.Type == "ExpirationUrl") ? identity.Claims.Where(c => c.Type == "ExpirationUrl").Select(c => c.Value).FirstOrDefault() : null,
                    TimeZone = identity.Claims.Any(c => c.Type == "TimeZone") ? identity.Claims.Where(c => c.Type == "TimeZone").Select(c => c.Value).FirstOrDefault() : null,
                    TimeZoneName = identity.Claims.Any(c => c.Type == "TimeZoneName") ? identity.Claims.Where(c => c.Type == "TimeZoneName").Select(c => c.Value).FirstOrDefault() : null,
                    UserName = identity.Claims.Any(c => c.Type == "UserName") ? identity.Claims.Where(c => c.Type == "UserName").Select(c => c.Value).FirstOrDefault() : null,
                    DeviceId = identity.Claims.Any(c => c.Type == "DeviceId") ? identity.Claims.Where(c => c.Type == "DeviceId").Select(c => c.Value).FirstOrDefault() : null,
                    Token = identity.Claims.Any(c => c.Type == "Token") ? identity.Claims.Where(c => c.Type == "Token").Select(c => c.Value).FirstOrDefault() : null,
                    SingleSignOn = identity.Claims.Any(c => c.Type == "SingleSignOn") ? identity.Claims.Where(c => c.Type == "SingleSignOn").Select(c => c.Value).FirstOrDefault() == "true" : false,
                    MobileSignOn = identity.Claims.Any(c => c.Type == "MobileSignOn") ? identity.Claims.Where(c => c.Type == "MobileSignOn").Select(c => c.Value).FirstOrDefault() == "true" : false,
                };
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API", null, "GetUserSession : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return null;
            }
        }

        public static object GetUserNotifications(int participantId, string timeZone, string timeZoneName, string baseUrl)
        {
            try
            {
                AccountReader accountReader = new AccountReader();
                NotificationsResponse response = new NotificationsResponse();
                var user = accountReader.GetBasicUserInfo(participantId);
                var dateFormat = user.Country1 != null ? user.Country1.DateFormat : "MM/dd/yyyy";
                var notification = ParticipantUtility.GetDashboadMessages(participantId, 0, 10, null, user.Organization.Portals.Where(x => x.Active == true).FirstOrDefault().StartDate.ToString(), true, timeZone, timeZoneName, dateFormat, (int)NotificationTypes.Notification, true);
                dateFormat += " hh:mm tt";
                IList<Notification> Notifications = new List<Notification>();
                if (notification.dashboardMessages != null)
                {
                    foreach (var msg in notification.dashboardMessages)
                    {
                        Notification data = new Notification
                        {
                            message_id = msg.Id,
                            message = msg.Message,
                            destination_url = msg.Url != null ? baseUrl + msg.Url.Replace("../", "/") : null,
                            date = msg.CreatedOn.ToString(dateFormat),
                            icon_url = msg.DashboardMessageType.Image != null && !string.IsNullOrEmpty(baseUrl) ? baseUrl + "/Images/mobile/" + msg.DashboardMessageType.Image.Replace(".svg", ".png") : null
                        };
                        Notifications.Add(data);
                    }
                }
                response.notification = Notifications;
                return response;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + participantId + ")", null, "GetUserNotifications : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return null;
            }
        }

        public static async Task<UpdateUserResponse> UpdateUserSettings(UserManager<ApplicationUser> userManager, int userId, UpdateSettings request, string deviceId)
        {
            try
            {
                AccountReader reader = new AccountReader(userManager);
                var result = await AccountUtility.GetUser(userManager, null, null, userId, null, deviceId);
                UserDto user = result.User;
                reader.AddOrEditNotificationToken(userId, deviceId, request.notification_token);

                user.Text = request.receive_text ? (byte)1 : (byte)2;
                if (request.time_zone != 0)
                    user.TimeZoneId = request.time_zone;
                UpdateUserResponse response = await reader.UpdateUser(new UpdateUserRequest { FromAdmin = false, IsLocked = false, user = user, UpdatedByUserId = user.Id }, true);
                return response;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userId + ")", null, "UpdateUserSettings : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return new UpdateUserResponse { Succeeded = false };
            }
        }

        public static DashBoardModel GetDashBoardModel(int participantId, string timeZone, string expirationUrl, string baseUrl)
        {
            AccountReader accountReader = new AccountReader();
            DeviceReader reader = new DeviceReader();
            DashBoardModel response = new DashBoardModel();
            List<Link> links = new List<Link>();
            int hraId = 0;
            string hraCompleteDate = "";
            try
            {
                string picturePath = baseUrl + "/Images/mobile/";
                string profilePicturePath = "/ProfilePictures/";
                var connectedDevices = WearableUtility.GetUserWearableDevices(participantId);
                response.blood_pressure = GetBloodPressureResponse(DevicesUtility.ListBiometricsData(7, null, null, participantId));
                response.physical_activity = GetPhysicalActivityResponse(DevicesUtility.ListPhysicalActivityData(7, null, null, participantId));
                response.terms_and_conditions = baseUrl + "/Home/ServiceAgreement?modal=false&closeModal=true&mobileView=true";
                response.about_app_url = baseUrl + "/Home/AboutApp";
                response.webview_oauth_url = baseUrl + "/MobileOAuth/AuthenticateUser";
                response.expiration_url = expirationUrl;
                response.physical_activity_graph_url = baseUrl + "/Devices?showGraphFor=1";
                response.blood_pressure_graph_url = baseUrl + "/Devices?showGraphFor=2";
                response.glucose_graph_url = baseUrl + "/Devices?showGraphFor=3";
                response.physical_activity_header = "Physical Activity";
                response.blood_pressure_header = "Blood Pressure";
                response.glucose_header = "Glucose";
                response.graph_link_text = "More info";
                response.watch_video = baseUrl + "/WatchVideo/testVideo.mp4";

                var user = accountReader.GetBasicUserInfo(participantId, true);
                response.first_name = user.FirstName;
                response.last_name = user.LastName;
                response.profile_picture = !string.IsNullOrEmpty(user.Picture) ? baseUrl + profilePicturePath + user.Picture : baseUrl + picturePath + CommonUtility.GetGenderSpecificImage(user).Replace(".svg", ".png");
                var portal = user.Organization.Portals.Where(x => x.Active == true).FirstOrDefault();
                user.PortalId = portal.Id;
                var hra = user.HRAs.Where(x => x.PortalId == user.PortalId && x.Portal.Active == true).OrderByDescending(x => x.Id).FirstOrDefault();
                if (hra != null)
                {
                    hraId = hra.Id;
                    if (hra.CompleteDate.HasValue)
                        hraCompleteDate = hra.CompleteDate.Value.ToString();
                }

                response.show_glucose_graph = reader.HasGlucoseData(user.Id) || IsDiaborPreDiab(user);
                if (response.show_glucose_graph)
                    response.glucose = GetGlucoseResponse(DevicesUtility.ListGlucoseData(7, null, null, participantId, timeZone, hraId, hraCompleteDate));

                if (user.Complete.HasValue && !user.Complete.Value)
                {
                    string url = baseUrl + "/Participant/InitialDashboard";
                    if (portal.HasHRA == (int)HRAStatus.No || portal.HasHRA == (int)HRAStatus.Optional)
                        url += "New";
                    links.Add(new Link { name = "My Profile", icon = picturePath + "avatar-male.png", url = url });
                }
                else
                {
                    if (portal.HasHRA == (int)HRAStatus.Yes && string.IsNullOrEmpty(hraCompleteDate))
                    {
                        links.Add(new Link { name = "My HRA", icon = picturePath + "hra.png", url = baseUrl + "/HRA/HRADashboard" });
                    }
                    else
                    {
                        var usersinProgram = user.UsersinPrograms.Where(x => x.ProgramsinPortal.PortalId == user.PortalId).OrderByDescending(x => x.Id).FirstOrDefault();
                        if (!string.IsNullOrEmpty(hraCompleteDate))
                            links.Add(new Link { name = "My HRA", icon = picturePath + "hra.png", url = baseUrl + "/HRA/HRADashboard" });
                        if (ShowSelfScheduling(user, usersinProgram, portal, hra))
                            links.Add(new Link { name = "My Coach", icon = picturePath + "coach.png", url = baseUrl + "/Participant/MyCoach" });
                        if (IsRescheduling(user, usersinProgram, hra) || (usersinProgram != null && usersinProgram.ProgramsinPortal.program.ProgramType == 2))
                            links.Add(new Link { name = "My Calls", icon = picturePath + "calls.png", url = baseUrl + "/Scheduler/Reschedule" });
                        links.Add(new Link { name = "My Labs", icon = picturePath + "labsweb.png", url = baseUrl + "/Lab/MyLabs" });
                        if (usersinProgram != null && usersinProgram.Id != 0)
                        {
                            links.Add(new Link { name = "My Kits", icon = picturePath + "kits.png", url = baseUrl + "/Program/MyProgram/#View-MyKit-Page" });
                            links.Add(new Link { name = "Recipes", icon = picturePath + "recipes.png", url = baseUrl + "/Recipe/Recipes" });
                        }
                    }
                }
                response.links = links;

                if (connectedDevices.Count() == 0)
                    response.device_alert = new DeviceAlert { type = "device", title = "No devices connected", description = "Connect new device to continue.", icon_url = picturePath + "nodevice.png" };
                else if (response.blood_pressure.blood_pressure == null && response.physical_activity.activity_time.Count() == 0 && response.physical_activity.activity_steps.Count() == 0 && (!response.show_glucose_graph || response.show_glucose_graph && response.glucose.pre_meal == null && response.glucose.post_meal == null && response.glucose.random == null))
                    response.device_alert = new DeviceAlert { type = "alert", title = "Let's reconnect", description = "It's been 7 days since we have received updated data for you. Make sure your device is synced and you are using it regularly.", icon_url = picturePath + "away.png" };
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + participantId + ")", null, "GetDashBoardModel : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
            return response;
        }

        public static bool IsDiaborPreDiab(UserDto user)
        {
            ParticipantReader participantReader = new ParticipantReader();
            /*HRADto hra = user.HRA;
            if (hra != null && hra.MedicalCondition.ToldDiabetes == 1 || hra.MedicalCondition.DiabetesMed == 1 || (hra.HealthNumbers.A1C.HasValue && hra.HealthNumbers.A1C >= 5.7) || 
                (hra.HealthNumbers.Glucose.HasValue && hra.HealthNumbers.Glucose >= 100 && hra.HealthNumbers.DidYouFast == 1) 
                || (hra.HealthNumbers.Glucose.HasValue && hra.HealthNumbers.Glucose >= 140 && hra.HealthNumbers.DidYouFast == 2))
            {
                return true;
            } 
            else */
            if (participantReader.HasDiabetes(new CheckTobaccoUserRequest { participantId = user.Id, portalId = user.PortalId }))
            {
                return true;
            }
            return false;
        }

        public static bool IsRescheduling(UserDto user, UsersinProgramDto usersinProgram, HRADto hra)
        {
            if (usersinProgram != null)
            {
                ParticipantReader reader = new ParticipantReader();
                GetCoachingCountRequest request = new GetCoachingCountRequest();
                request.userId = user.Id;
                request.portalId = user.PortalId;
                request.refId = usersinProgram.Id;
                if (hra != null)
                    request.hraId = hra.Id;
                var coachingNotes = reader.GetCoachingCount(request);
                if (coachingNotes.count >= 1)
                    return true;
            }
            if (user.Appointments.Where(x => DateTime.Parse(x.Date) > DateTime.Now && x.Active == true).OrderBy(x => x.Date).Count() > 0)
            {
                return true;
            }
            return false;
        }

        public static bool ShowSelfScheduling(UserDto user, UsersinProgramDto usersinProgram, PortalDto portal, HRADto hra)
        {
            if ((usersinProgram == null || usersinProgram.ProgramsinPortal.program.ProgramType != 2)
                && user.Appointments.Where(x => DateTime.Parse(x.Date) > DateTime.Now && x.Active == true).OrderBy(x => x.Date).Count() == 0
                && portal.HasCoachingProgram
                && portal.ShowProgramOption && portal.SelfScheduling
                && EligibilityUserStatus(user.UniqueId, user.PortalId) != "T"
                && HasCoachingConditions(user, hra))
            {
                return true;
            }
            return false;
        }

        public static string EligibilityUserStatus(string uniqueId, int portalId)
        {
            Eligibility eligibility = new EligibilityReader().GetEligibility(uniqueId, portalId);
            return eligibility != null ? eligibility.UserStatus : null;
        }

        public static bool HasCoachingConditions(UserDto user, HRADto hra)
        {
            PortalReader reader = new PortalReader();
            ParticipantReader participantReader = new ParticipantReader();
            bool All = false;
            bool Diabetes = false;
            bool Postmenopausal = false;
            List<PortalCoachingConditionsDto> CoachingConditions = reader.GetPortalCoachingConditions(user.PortalId);
            if (CoachingConditions != null)
            {
                foreach (PortalCoachingConditionsDto schedulingTool in CoachingConditions)
                {
                    if (schedulingTool.CoachCondId == PortalCoachingConditions.All.GetHashCode())
                        All = true;
                    if (schedulingTool.CoachCondId == PortalCoachingConditions.Diabetes.GetHashCode())
                        Diabetes = true;
                    if (schedulingTool.CoachCondId == PortalCoachingConditions.Postmenopausal.GetHashCode())
                        Postmenopausal = true;
                }
            }
            return All || (Diabetes && participantReader.HasDiabetes(new CheckTobaccoUserRequest { participantId = user.Id, portalId = user.PortalId })) || (Postmenopausal && IsPostmenopausal(hra, user.DOB.Value.ToString(), user.Gender));
        }

        public static bool IsPostmenopausal(HRADto hra, string DOB, byte? gender)
        {
            ParticipantReader reader = new ParticipantReader();
            if (hra != null)
                return reader.IsPostmenopausal(hra.Id, DOB, gender);
            else
                return false;
        }

        public static async Task<string> ChangePassword(UserManager<ApplicationUser> userManager, UserIdentity userIdentity, ChangePassword request)
        {
            try
            {
                var errMsg1 = Translate.Message("L1977");
                var errMsg2 = Translate.Message("L1976");
                var errMsg3 = string.Format(Translate.Message("L3050"), 3);
                var response = await AccountUtility.GetUser(userManager, userIdentity.UserName, request.old_password, null, true, userIdentity.DeviceId);
                if (response.User != null)
                {
                    var updateresponse = await AccountUtility.ChangePassword(userManager, userIdentity.UserId, request.old_password, request.new_password);
                    if (!updateresponse.success)
                    {
                        return errMsg3;
                    }
                    return errMsg1;
                }
                else
                    return errMsg2;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userIdentity.UserId + ")", null, "ChangePassword : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return null;
            }
        }

        private static BloodPressure GetBloodPressureResponse(BiometricsData bloodPressure)
        {
            try
            {
                BloodPressure blood_pressure = new BloodPressure();
                if (bloodPressure.graphData != null && bloodPressure.graphData.Count() > 0)
                {
                    List<BP> bp = new List<BP>();
                    foreach (var data in bloodPressure.graphData)
                        bp.Add(new BP { date = data.date, diastolic = data.open, systolic = data.close });

                    blood_pressure.blood_pressure = bp.OrderBy(x => Convert.ToDateTime(x.date)).ToList();
                    if (bloodPressure.SystolicValue < 120 && bloodPressure.DiastolicValue < 80)
                    {
                        blood_pressure.range_color = "#91dc5a";
                        blood_pressure.range_text = Translate.Message("L4010");
                    }
                    else
                    {
                        blood_pressure.range_color = "#ff5757";
                        blood_pressure.range_text = Translate.Message("L4011");
                    }
                    blood_pressure.average_diastolic = bloodPressure.DiastolicValue;
                    blood_pressure.average_systolic = bloodPressure.SystolicValue;
                    blood_pressure.diastolic_color = "#67b7dc";
                    blood_pressure.systolic_color = "#a367dc";
                    blood_pressure.unit = Translate.Message("L4058");
                }
                return blood_pressure;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API", null, "GetBloodPressure : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return new BloodPressure();
            }
        }

        private static PhysicalActivity GetPhysicalActivityResponse(RoutineData activity)
        {
            try
            {
                List<Activity> activityStep = new List<Activity>();
                List<Activity> activityTime = new List<Activity>();
                if (activity.graphData.Count() > 0)
                {
                    foreach (var data in activity.graphData)
                        activityTime.Add(new Activity { date = data.date, value = data.value2 });
                }
                if (activity.graphData1.Count() > 0)
                {
                    foreach (var data in activity.graphData1)
                        activityStep.Add(new Activity { date = data.date, value = data.value });
                }

                return new PhysicalActivity { steps_color = "#03a9f4", time_color = "#ffd949", steps_unit = "Steps/Day", time_unit = "Minutes/day", average_steps = Convert.ToInt32(activity.avgSteps), average_time = Convert.ToInt32(activity.avgTime), activity_time = activityTime.OrderBy(x => Convert.ToDateTime(x.date)).ToList(), activity_steps = activityStep.OrderBy(x => Convert.ToDateTime(x.date)).ToList(), average_calories = Convert.ToInt32(activity.avgCalories) };
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API", null, "GetPhysicalActivity : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return new PhysicalActivity();
            }
        }

        private static Glucose GetGlucoseResponse(GlucoseData glucose)
        {
            try
            {
                Glucose response = new Glucose();
                List<Graph> preMeal = new List<Graph>();
                List<Graph> postMeal = new List<Graph>();
                List<Graph> random = new List<Graph>();
                if (glucose.graphData != null && glucose.graphData.Count() > 0)
                {
                    foreach (var data in glucose.graphData.OrderBy(x => Convert.ToDateTime(x.x)).ToList())
                    {
                        if (data.ay.HasValue)
                            preMeal.Add(new Graph { time = data.x, value = data.ay });
                        else if (data.by.HasValue)
                            postMeal.Add(new Graph { time = data.x, value = data.by });
                        else
                            random.Add(new Graph { time = data.x, value = data.cy });
                    }
                    response.pre_meal = preMeal.OrderBy(x => x.time).ToList();
                    response.post_meal = postMeal.OrderBy(x => x.time).ToList();
                    response.random = random.OrderBy(x => x.time).ToList();
                    response.average = glucose.average.Value;
                    response.average_pre_meal = glucose.premeal.Value;
                    response.average_post_meal = glucose.postmeal.Value;
                    response.unit = Translate.Message("L4025");
                    response.pre_meal_color = "#a367dc";
                    response.post_meal_color = "#67b7dc";
                    response.random_color = "#d2f241";

                    if (glucose.premeal.Value > glucose.premeal.preMealGoalTo || glucose.premeal.Value < glucose.premeal.preMealGoalFrom)
                    {
                        response.pre_meal_range_text = Translate.Message("L4011");
                        response.range_text = Translate.Message("L4011");
                        response.pre_meal_range_color = "#ff5757";
                        response.range_color = "#ff5757";
                    }
                    else
                    {
                        response.pre_meal_range_text = Translate.Message("L4010");
                        response.pre_meal_range_color = "#91dc5a";
                        response.range_text = Translate.Message("L4010");
                        response.range_color = "#91dc5a";
                    }

                    if (glucose.postmeal.Value >= glucose.postmeal.postMealGoalTo || glucose.postmeal.Value < glucose.postmeal.postMealGoalFrom)
                    {
                        response.post_meal_range_text = Translate.Message("L4011");
                        response.post_meal_range_color = "#ff5757";
                    }
                    else
                    {
                        response.post_meal_range_text = Translate.Message("L4010");
                        response.post_meal_range_color = "#91dc5a";
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API", null, "GetGlucoseResponse : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return null;
            }
        }

        public static UserProfile GetUserProfile(int participantId, string deviceId, string baseUrl)
        {
            try
            {
                AccountReader accountReader = new AccountReader();
                CommonReader commonReader = new CommonReader();
                MobileReader mobileReader = new MobileReader();
                string picturePath = "/ProfilePictures/";
                var user = accountReader.GetBasicUserInfo(participantId, false, true);
                var dateFormat = user.Country1 != null ? user.Country1.DateFormat : "MM/dd/yyyy";
                var deviceDetails = mobileReader.CanShowMobileNotificationDetails(participantId, deviceId);
                CoachProfile coachProfile = null;
                if (user.UsersinPrograms != null && user.UsersinPrograms.Count > 0)
                {
                    var userinPrograms = user.UsersinPrograms.Where(x => x.IsActive && x.CoachId.HasValue).OrderByDescending(x => x.Id).ToList();
                    if (userinPrograms != null && userinPrograms.Count > 0)
                    {
                        var coach = userinPrograms.FirstOrDefault().User1;
                        if (coach != null)
                        {
                            var roleCode = coach.Roles.Select(x => x.Name).FirstOrDefault();
                            coachProfile = new CoachProfile
                            {
                                first_name = coach.FirstName,
                                last_name = coach.LastName,
                                role = roleCode,
                                gender = coach.Gender.HasValue && coach.Gender.Value == 1 ? "M" : "F",
                                profile = coach.AdminProperty.Profile,
                                profile_picture = !string.IsNullOrEmpty(baseUrl) && !string.IsNullOrEmpty(coach.Picture) ? baseUrl + picturePath + coach.Picture : baseUrl + "/Images/mobile/" + CommonUtility.GetGenderSpecificImage(coach).Replace(".svg", ".png"),
                            };
                        }
                    }
                }
                return new UserProfile
                {
                    first_name = user.FirstName,
                    middle_name = user.MiddleName,
                    last_name = user.LastName,
                    email = user.Email,
                    dob = user.DOB.HasValue ? user.DOB.Value.ToString(dateFormat) : null,
                    date_format = dateFormat,
                    phone = GetUserPhoneNo(user),
                    gender = user.Gender.HasValue && user.Gender.Value == 1 ? "M" : "F",
                    address1 = user.Address,
                    address2 = user.Address2,
                    city = user.City,
                    state_id = user.State.HasValue ? user.State.Value : 0,
                    state = user.State.HasValue && user.Country.HasValue ? commonReader.ListStates(new ListStatesRequest { CountryId = user.Country.Value }).States.Where(x => x.Id == user.State.Value).Select(x => x.Name).FirstOrDefault() : null,
                    country_id = user.Country.HasValue ? user.Country.Value : 0,
                    country = user.Country.HasValue ? commonReader.ListCountries(new ListCountriesRequest()).Countries.Where(x => x.Id == user.Country.Value).Select(x => x.Name).FirstOrDefault() : null,
                    zip = user.Zip,
                    profile_picture = !string.IsNullOrEmpty(user.Picture) ? baseUrl + picturePath + user.Picture : baseUrl + "/Images/mobile/" + CommonUtility.GetGenderSpecificImage(user).Replace(".svg", ".png"),
                    time_zone = user.TimeZoneId.HasValue ? user.TimeZoneId.Value : 0,
                    coach_profile = coachProfile,
                    settings = new Settings
                    {
                        edit_profile_url = baseUrl + "/Participant/InitialDashboard?openProfile=true",
                        receive_text = user.Text.HasValue && user.Text.Value == 1,
                        time_zones = CommonUtility.GetTimeZones(null).TimeZones.Select(x => new TimeZone { name = Translate.Message(x.TimeZoneDisplay), id = x.Id }),
                        mobile_notification = deviceDetails.notificationEnabled,
                        show_mobile_notification = deviceDetails.canShowNotification
                    }
                };
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + participantId + ")", null, "GetUserProfile : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return null;
            }
        }

        public static StatesList ListStates(StateRequest request)
        {
            try
            {
                var list = CommonUtility.ListStates(request.country_id);
                if (list != null)
                {
                    return new StatesList
                    {
                        states = list.OrderBy(t => t.Name).Select(x => new State { name = x.Name, id = x.Id, country_id = x.CountryId }).ToList()
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API", null, "ListStates : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return null;
            }
        }

        public static CountriesList ListCountries()
        {
            try
            {
                return new CountriesList
                {
                    countries = CommonUtility.ListCountries().OrderBy(t => t.Name).Select(x => new Country { name = x.Name, id = x.Id }).ToList(),
                };
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API", null, "ListStatesAndCountries : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return null;
            }
        }

        public static FeedsResponse GetUserFeeds(int participantId, string timeZone, string timeZoneName, string baseUrl)
        {
            try
            {
                AccountReader accountReader = new AccountReader();
                FeedsResponse response = new FeedsResponse();
                var user = accountReader.GetBasicUserInfo(participantId);
                var dateFormat = user.Country1 != null ? user.Country1.DateFormat : "MM/dd/yyyy";
                var feeds = ParticipantUtility.GetDashboadMessages(participantId, 0, 10, null, user.Organization.Portals.Where(x => x.Active == true).FirstOrDefault().StartDate.ToString(), true, timeZone, timeZoneName, dateFormat, (int)NotificationTypes.Feed, false);
                dateFormat += " hh:mm tt";
                IList<Feed> Feeds = new List<Feed>();
                if (feeds.dashboardMessages != null)
                {
                    foreach (var feed in feeds.dashboardMessages)
                    {
                        Feeds.Add(new Feed
                        {
                            message_id = feed.Id,
                            message = feed.Message,
                            destination_url = feed.Url != null ? baseUrl + feed.Url.Replace("../", "/") : null,
                            date = feed.CreatedOn.ToString(dateFormat),
                            icon_url = feed.DashboardMessageType.Image != null && !string.IsNullOrEmpty(baseUrl) ? baseUrl + "/Images/mobile/" + feed.DashboardMessageType.Image.Replace(".svg", ".png") : null
                        });
                    }
                }
                response.feed = Feeds;
                return response;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + participantId + ")", null, "GetUserFeeds : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return new FeedsResponse();
            }
        }

        public static WearableResponse GetWearableDetails(int participantId, string deviceId, string baseUrl)
        {
            try
            {
                WearableResponse response = new WearableResponse();
                var deviceTypes = WearableUtility.GetWearableDevices(null);
                var allConnectedDevices = WearableUtility.GetUserWearableDevices(participantId);
                var webConnectedDevices = allConnectedDevices.Where(x => x.WearableDevice.Type == (int)WearableDeviceType.Web).ToList();

                response.wearables = new List<WearableDevicesResponse>();
                if (deviceTypes.Where(x => x.Type == (int)WearableDeviceType.Mobile && x.Name.ToLower() == "google fit").Any())
                {
                    response.is_google_fit_connected = allConnectedDevices.Where(x => x.WearableDevice.Type == (int)WearableDeviceType.Mobile && x.WearableDevice.Name.ToLower() == "google fit" && x.DeviceId == deviceId).Any();
                    if (response.is_google_fit_connected)
                    {
                        response.connected_device_count += 1;
                        response.google_fit_external_id = allConnectedDevices.Where(x => x.WearableDevice.Type == (int)WearableDeviceType.Mobile && x.WearableDevice.Name.ToLower() == "google fit" && x.DeviceId == deviceId).FirstOrDefault().ExternalUserId;
                    }
                }
                if (deviceTypes.Where(x => x.Type == (int)WearableDeviceType.Mobile && x.Name.ToLower() == "apple health").Any())
                {
                    response.is_apple_health_connected = allConnectedDevices.Where(x => x.WearableDevice.Type == (int)WearableDeviceType.Mobile && x.WearableDevice.Name.ToLower() == "apple health" && x.DeviceId == deviceId).Any();
                    if (response.is_apple_health_connected)
                    {
                        response.connected_device_count += 1;
                        response.apple_health_external_id = allConnectedDevices.Where(x => x.WearableDevice.Type == (int)WearableDeviceType.Mobile && x.WearableDevice.Name.ToLower() == "apple health" && x.DeviceId == deviceId).FirstOrDefault().ExternalUserId;
                    }
                }
                response.connected_device_count += webConnectedDevices.Count();

                foreach (var device in deviceTypes.Where(x => x.Type == (int)WearableDeviceType.Web).ToList())
                {
                    WearableDevicesResponse wearableDevice = new WearableDevicesResponse();
                    string connectedDeviceId = webConnectedDevices.Where(y => y.WearableDeviceId == device.Id).FirstOrDefault() != null ? webConnectedDevices.Where(y => y.WearableDeviceId == device.Id).FirstOrDefault().Id.ToString() : "";
                    wearableDevice.connected = connectedDeviceId != "";
                    wearableDevice.connect_url = baseUrl + "/Wearable/" + device.AuthUrl;
                    wearableDevice.disconnect_url = baseUrl + "/Wearable/DisconnectDevice?deviceId=" + connectedDeviceId;
                    wearableDevice.display_name = device.Name;
                    wearableDevice.logo_url = baseUrl + "/Images/wearables/" + device.Icon;
                    wearableDevice.type = device.Name;

                    response.wearables.Add(wearableDevice);
                }
                return response;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + participantId + ")", null, "GetWearableDetails : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return null;
            }
        }

        public static DailyVitals GetDailyVitals(int userId, string timeZone)
        {
            try
            {
                AccountReader accountReader = new AccountReader();
                DailyVitals response = new DailyVitals();
                var user = accountReader.GetBasicUserInfo(userId);
                var questions = CommonUtility.GetDailyVitalsQuestionsList();
                var vitalsLog = JournalUtility.ListVitalsLog(new DiaryListModel { page = 0, pageSize = 1 }, userId);
                CommonReader reader = new CommonReader();
                IList<MeasurementsDto> measurements = reader.GetMeasurements();
                var userMeasurement = reader.ListMeasurements((int)user.Unit, measurements);
                TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                var dailyVitals = vitalsLog.dailyVitals.Where(x => x.Date.Value.Date == TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone).Date).FirstOrDefault();
                if (dailyVitals != null)
                    response.vitals_id = dailyVitals.Id;
                response.has_pending_vitals = (dailyVitals != null && dailyVitals.AerobicExercise.HasValue && dailyVitals.HealthyEating.HasValue &&
                        dailyVitals.Hydration.HasValue && dailyVitals.Alcohol.HasValue && dailyVitals.Tobacco.HasValue &&
                        dailyVitals.Medications.HasValue && dailyVitals.Sleep.HasValue && dailyVitals.Stress.HasValue &&
                        dailyVitals.Happy.HasValue) ? false : true;
                response.assessment_header = "In the last 24 hours, did you";
                response.assessment_title = "Update your progress";
                response.assessment_description = "Answer 10 simple questions about your lifestyle habits";
                response.assessment_button = "Start Now";

                foreach (var question in questions)
                {
                    VitalsLog vital = new VitalsLog();
                    vital.question_no = int.Parse(question.Value);
                    vital.question_text = Translate.Message(question.Text);
                    if (vital.question_no == 1)
                    {
                        vital.answer_types.Add(new AnswerType
                        {
                            type = "float",
                            text = Translate.Message(userMeasurement.Measurements[BioLookup.Weight].MeasurementUnit),
                            value = ""
                        });
                    }
                    else
                    {
                        vital.answer_types.Add(new AnswerType
                        {
                            type = "string",
                            text = Translate.Message("L2"),
                            value = "1"
                        });
                        vital.answer_types.Add(new AnswerType
                        {
                            type = "string",
                            text = Translate.Message("L3"),
                            value = "2"
                        });
                    }
                    switch (vital.question_no)
                    {
                        case 1:
                            vital.validiation = new Validation
                            {
                                min = userMeasurement.Measurements[BioLookup.Weight].Min.Value,
                                max = userMeasurement.Measurements[BioLookup.Weight].Max.Value
                            };
                            if (dailyVitals != null && dailyVitals.Weight.HasValue)
                            {
                                if (user.Unit == (int)Unit.Metric)
                                {
                                    dailyVitals.Weight = ListOptions.ToMetric(dailyVitals.Weight.Value, BioLookup.Weight, reader.GetMeasurements());
                                }
                                vital.answer = dailyVitals.Weight.Value.ToString();
                            }
                            break;
                        case 2:
                            if (dailyVitals != null && dailyVitals.AerobicExercise.HasValue)
                                vital.answer = dailyVitals.AerobicExercise.Value.ToString();
                            break;
                        case 3:
                            if (dailyVitals != null && dailyVitals.HealthyEating.HasValue)
                                vital.answer = dailyVitals.HealthyEating.Value.ToString();
                            break;
                        case 4:
                            if (dailyVitals != null && dailyVitals.Hydration.HasValue)
                                vital.answer = dailyVitals.Hydration.Value.ToString();
                            break;
                        case 5:
                            vital.question_text = string.Format(Translate.Message(question.Text), user.Gender.HasValue && user.Gender.Value == 1 ? 2 : 1);
                            if (dailyVitals != null && dailyVitals.Alcohol.HasValue)
                                vital.answer = dailyVitals.Alcohol.Value.ToString();
                            break;
                        case 6:
                            if (dailyVitals != null && dailyVitals.Tobacco.HasValue)
                                vital.answer = dailyVitals.Tobacco.Value.ToString();
                            break;
                        case 7:
                            vital.answer_types.Add(new AnswerType
                            {
                                type = "string",
                                text = Translate.Message("L100"),
                                value = "3"
                            });
                            if (dailyVitals != null && dailyVitals.Medications.HasValue)
                                vital.answer = dailyVitals.Medications.Value.ToString();
                            break;
                        case 8:
                            if (dailyVitals != null && dailyVitals.Sleep.HasValue)
                                vital.answer = dailyVitals.Sleep.Value.ToString();
                            break;
                        case 9:
                            if (dailyVitals != null && dailyVitals.Stress.HasValue)
                                vital.answer = dailyVitals.Stress.Value.ToString();
                            break;
                        case 10:
                            if (dailyVitals != null && dailyVitals.Happy.HasValue)
                                vital.answer = dailyVitals.Happy.Value.ToString();
                            break;
                    }

                    response.daily_vitals.Add(vital);
                }
                return response;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userId + ")", null, "GetDailyVitals : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return null;
            }
        }

        public static object SaveDailyVitals(int userId, string timeZone, SaveDailyVitals request)
        {
            try
            {
                AccountReader accountReader = new AccountReader();
                VitalsModel model = new VitalsModel();
                model.DailyVitals = new VitalsLogDto();
                model.DailyVitals.Id = request.vitals_id;
                var user = accountReader.GetBasicUserInfo(userId);
                switch (request.question_no)
                {
                    case "1":
                        model.DailyVitals.HasWeight = 1;
                        model.DailyVitals.Weight = float.Parse(request.answer);
                        if (model.DailyVitals.Weight.HasValue && user.Unit == (int)Unit.Metric)
                        {
                            CommonReader reader = new CommonReader();
                            model.DailyVitals.Weight = ListOptions.ToImperial(model.DailyVitals.Weight.Value, BioLookup.Weight, reader.GetMeasurements());
                        }

                        break;
                    case "2":
                        model.DailyVitals.AerobicExercise = request.answer.Equals("1") ? (byte)1 : (byte)2;
                        break;
                    case "3":
                        model.DailyVitals.HealthyEating = request.answer.Equals("1") ? (byte)1 : (byte)2;
                        break;
                    case "4":
                        model.DailyVitals.Hydration = request.answer.Equals("1") ? (byte)1 : (byte)2;
                        break;
                    case "5":
                        model.DailyVitals.Alcohol = request.answer.Equals("1") ? (byte)1 : (byte)2;
                        break;
                    case "6":
                        model.DailyVitals.Tobacco = request.answer.Equals("1") ? (byte)1 : (byte)2;
                        break;
                    case "7":
                        model.DailyVitals.Medications = request.answer.Equals("3") ? (byte)3 : request.answer.Equals("2") ? (byte)2 : (byte)1;
                        break;
                    case "8":
                        model.DailyVitals.Sleep = request.answer.Equals("1") ? (byte)1 : (byte)2;
                        break;
                    case "9":
                        model.DailyVitals.Stress = request.answer.Equals("1") ? (byte)1 : (byte)2;
                        break;
                    case "10":
                        model.DailyVitals.Happy = request.answer.Equals("1") ? (byte)1 : (byte)2;
                        break;
                    default: break;
                }
                var result = JournalUtility.AddEditDailyVitals(userId, null, user.Organization.Portals.Where(x => x.Active == true).FirstOrDefault().Id, timeZone, model);
                return new SaveDailyVitalsResponse { vitals_id = result.DailyVitalsId, has_pending_vitals = result.hasPendingVitals, success = result.success };
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userId + ")", null, "SaveDailyVitals : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return null;
            }
        }

        private static string GetUserPhoneNo(UserDto user)
        {
            if (user.ContactMode.HasValue)
            {
                if (user.ContactMode.Value == 1 && !string.IsNullOrEmpty(user.HomeNumber))
                    return user.HomeNumber;
                if (user.ContactMode.Value == 2 && !string.IsNullOrEmpty(user.WorkNumber))
                    return user.WorkNumber;
                if (user.ContactMode.Value == 3 && !string.IsNullOrEmpty(user.CellNumber))
                    return user.CellNumber;
            }
            else if (!string.IsNullOrEmpty(user.CellNumber))
                return user.CellNumber;
            return "";
        }

        public static async Task<ChangeEmailResponse> ChangeEmail(UserManager<ApplicationUser> userManager, int userId, ChangeEmail request, string deviceId)
        {
            try
            {
                AccountReader reader = new AccountReader(userManager);
                ChangeEmailResponse response = new ChangeEmailResponse { status = false };
                var result = await AccountUtility.GetUser(userManager, null, null, userId, null, deviceId);
                UserDto user = result.User;

                string email = user.Email + "," + request.email;
                if (!string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(request.email) && !user.Email.Trim().Equals(request.email.Trim()))
                {
                    user.Email = request.email;
                }
                else
                {
                    response.message = "Email id can't be same.";
                    return response;
                }
                UpdateUserResponse userresponse = await reader.UpdateUser(new UpdateUserRequest { FromAdmin = false, IsLocked = false, user = user, UpdatedByUserId = user.Id });
                if (userresponse.Succeeded)
                {
                    NotificationUtility.CreateChangePasswordNotificationEvent(NotificationEventTypeDto.ChangeEmail, user.Id, email, true);
                    response.status = true;
                    response.message = "Successfully updated";
                }
                else
                {
                    response.message = userresponse.error.FirstOrDefault().Description;
                }
                return response;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userId + ")", null, "ChangeEmail : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return new ChangeEmailResponse { status = false };
            }
        }

        public static async Task<UpdateUserResponse> UpdateUserProfile(UserManager<ApplicationUser> userManager, int userId, UserProfileRequest request, string deviceId)
        {
            try
            {
                AccountReader reader = new AccountReader(userManager);
                var result = await AccountUtility.GetUser(userManager, null, null, userId, null, deviceId);
                UserDto user = result.User;
                if (!string.IsNullOrEmpty(request.first_name))
                    user.FirstName = request.first_name.Trim();
                user.MiddleName = request.middle_name;
                if (!string.IsNullOrEmpty(request.last_name))
                    user.LastName = request.last_name.Trim();
                if (request.dob.HasValue && request.dob.Value != DateTime.MinValue)
                    user.DOB = request.dob;
                if (!string.IsNullOrEmpty(request.phone))
                {
                    if (user.ContactMode.HasValue)
                    {
                        if (user.ContactMode == 1)
                            user.HomeNumber = request.phone;
                        else if (user.ContactMode == 2)
                            user.WorkNumber = request.phone;
                        else if (user.ContactMode == 3)
                            user.CellNumber = request.phone;
                    }
                    else
                    {
                        user.CellNumber = request.phone;
                    }
                }
                if (!string.IsNullOrEmpty(request.address1))
                    user.Address = request.address1.Trim();
                user.Address2 = request.address2;
                if (!string.IsNullOrEmpty(request.city))
                    user.City = request.city.Trim();
                if (request.state_id.HasValue)
                    user.State = request.state_id.Value;
                if (request.country_id.HasValue)
                    user.Country = request.country_id.Value;
                if (!string.IsNullOrEmpty(request.zip))
                    user.Zip = request.zip.Trim();

                UpdateUserResponse response = await reader.UpdateUser(new UpdateUserRequest { FromAdmin = false, IsLocked = false, user = user, UpdatedByUserId = user.Id });
                return response;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userId + ")", null, "UpdateUserProfile : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return new UpdateUserResponse { Succeeded = false };
            }
        }

        public static List<MessageDashBoard> GetDashBoardMessages(int userId, DashBoardMessageRequest request, string timeZone, int systemAdminId)
        {
            try
            {
                List<MessageDashBoard> model = new List<MessageDashBoard>();
                var messages = MessageUtility.ListUserMessages(userId, userId, false, true, false, request.drafts, false, string.IsNullOrEmpty(request.search_text) ? null : "%" + request.search_text + "%", request.unread, false, null, null, null, timeZone, systemAdminId);
                if (messages.parentMessages != null)
                {
                    foreach (MessageDto message in messages.parentMessages)
                    {
                        MessageDashBoard newMsg = new MessageDashBoard();
                        newMsg.date = message.LastMessageDate.ToString("MMM dd, hh:mm tt");
                        newMsg.has_attachment = message.hasAttachment;
                        newMsg.id = message.Id;
                        newMsg.is_draft = !message.IsSent;
                        newMsg.is_read = message.StatusId != 1;
                        newMsg.is_sent = message.IsSent;
                        newMsg.message = !string.IsNullOrEmpty(message.RecentMessage) ? message.RecentMessage : message.MessageBody;
                        newMsg.subject = message.Subject;
                        newMsg.unread_count = 0;
                        model.Add(newMsg);
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userId + ")", null, "GetDashBoardMessages : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return null;
            }
        }

        public static List<Message> GetMessages(int userId, int messageId, string timeZone, int systemAdminId, string baseUrl)
        {
            try
            {
                List<Message> model = new List<Message>();
                var messages = MessageUtility.GetMessageDetails(userId, 0, messageId, true, false, timeZone, false, systemAdminId);
                if (messages.Messages != null)
                {
                    int parent_message_id = 0;
                    foreach (MessageDto message in messages.Messages)
                    {
                        if (!(!message.IsSent && !string.IsNullOrEmpty(message.CreatorName)))
                        {
                            var fileExist = !string.IsNullOrEmpty(message.Attachment) && File.Exists("~/Messageuploads/" + message.Attachment);
                            if (parent_message_id == 0)
                                parent_message_id = message.Id;
                            Message newMsg = new Message();
                            newMsg.date = message.CreateDate.ToString("dddd, MMM dd, yyyy, hh:mm tt");
                            newMsg.parent_message_id = parent_message_id;
                            newMsg.attachment_name = fileExist ? message.Attachment : null;
                            newMsg.attachment_url = fileExist ? baseUrl + "/Messageuploads/" + message.Attachment : null;
                            newMsg.attachment_size = fileExist ? FileSizeFormatter.FormatSize(new FileInfo("~/Messageuploads/" + message.Attachment).Length) : null;
                            newMsg.id = message.Id;
                            newMsg.is_draft = !message.IsSent;
                            newMsg.is_read = message.IsSent && message.MessageRecipients.FirstOrDefault().IsRead;
                            newMsg.is_sent = message.IsSent;
                            newMsg.message = message.MessageBody;
                            newMsg.subject = message.Subject;
                            newMsg.can_delete = message.CanShowDelete && !string.IsNullOrEmpty(newMsg.attachment_url);
                            newMsg.is_user_message = string.IsNullOrEmpty(message.CreatorName);
                            newMsg.creator_name = message.CreatorName;
                            newMsg.creator_role = message.CreatorRole;
                            newMsg.status = newMsg.is_read ? "Read" : "Not seen yet";
                            model.Add(newMsg);
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userId + ")", null, "GetMessages : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return null;
            }
        }

        public static List<Message> AddMessage(int userId, int systemAdminId, AddMessageRequest request, string timeZone, string role, string baseUrl)
        {
            try
            {
                int? parentMessageId = null;
                if (request.parent_message_id != 0)
                    parentMessageId = request.parent_message_id;
                var updateResponse = MessageUtility.AddEditMessage(request.message_id, userId, systemAdminId, request.subject, request.message.Trim(), request.attachement_name, request.is_sent, parentMessageId, role, systemAdminId);
                if (updateResponse.updatedId != 0)
                {
                    int msgId = parentMessageId != null ? request.parent_message_id : updateResponse.updatedId;
                    return GetMessages(userId, msgId, timeZone, systemAdminId, baseUrl);
                }
                return new List<Message>();
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userId + ")", null, "AddMessage : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return null;
            }
        }

        public static bool DeleteAttachment(int userId, DeleteMessageRequest request, int systemAdminId)
        {
            try
            {
                if (File.Exists(Path.Combine("~/Messageuploads", request.attachement_name)))
                    File.Delete(Path.Combine("~/Messageuploads", request.attachement_name));
                MessageUtility.DeleteAttachment(request.message_id, systemAdminId);
                return true;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userId + ")", null, "DeleteAttachment : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return false;
            }
        }

        public static async Task<VerifyUserResponse> VerifyPassword(UserManager<ApplicationUser> userManager, UserIdentity userIdentity, VerifyPassword request)
        {
            try
            {
                return await AccountUtility.VerifyUser(userManager, userIdentity.UserName, request.password, userIdentity.DeviceId);
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userIdentity.UserId + ")", null, "VerifyPassword : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return null;
            }
        }

        public static string GetWatchVideo(int userId, string baseUrl)
        {
            try
            {
                return baseUrl + "/WatchVideo/testVideo.mp4";
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "Mobile API - UserId (" + userId + ")", null, "GetWatchVideo : " + ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
                return null;
            }
        }

        public static bool SaveRefreshToken(ClaimsModel claims, string refreshToken)
        {
            AccountReader reader = new AccountReader();
            return reader.AddOrEditRefreshToken(claims.userId, claims.deviceId, claims.expiresIn, refreshToken);
        }

        public static bool ValidateRefreshToken(ClaimsModel claims, string refreshToken)
        {
            AccountReader reader = new AccountReader();
            return reader.ValidateRefreshToken(claims.userId, claims.deviceId, claims.expiresIn, refreshToken);
        }
    }
}
