using Intervent.Web.DTO;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InterventWebApp
{
    public class WebinarController : BaseController
    {
        private readonly AppSettings _appSettings;

        public WebinarController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult Webinars()
        {
            return View();
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListWebinars(int page, int pageSize, int? totalRecords)
        {
            var response = WebinarUtility.ListWebinars(page, pageSize, totalRecords);
            var custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            return Json(new
            {
                Records = response.webinars.OrderByDescending(x => x.StartTime).Select(x => new
                {
                    x.Id,
                    x.WebinarId,
                    x.Topic,
                    x.Duration,
                    StartTime = TimeZoneInfo.ConvertTimeFromUtc(x.StartTime.Value, custTZone).ToString(),
                }),
                TotalRecords = response.totalRecords
            });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult GetWebinar(int id)
        {
            var response = WebinarUtility.GetWebinar(id);
            var custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            response.webinar.Password = response.webinar.Password != null ? response.webinar.Password : "";
            response.webinar.StartTimeString = TimeZoneInfo.ConvertTimeFromUtc(response.webinar.StartTime.Value, custTZone).ToString();
            response.webinar.ImageUrl = !string.IsNullOrEmpty(response.webinar.ImageUrl) ? response.webinar.ImageUrl : "";
            response.webinar.VideoUrl = !string.IsNullOrEmpty(response.webinar.VideoUrl) ? response.webinar.VideoUrl : "";
            response.webinar.PresentedByName = response.webinar.User != null ? response.webinar.User.FirstName + " " + response.webinar.User.LastName : "";
            List<WebinarOccurrence> occurrenceList = new List<WebinarOccurrence>();
            foreach (WebinarOccurrenceDto occurrence in response.webinar.WebinarOccurrences)
            {
                var occurrenceDate = TimeZoneInfo.ConvertTimeFromUtc(occurrence.StartTime, custTZone);
                occurrenceList.Add(new WebinarOccurrence
                {
                    OccurrenceId = occurrence.OccurrenceId,
                    Date = occurrenceDate.ToString("MMMM dd, yyyy"),
                    Time = occurrenceDate.ToShortTimeString(),
                    Status = occurrence.Status,
                    Duration = occurrence.Duration,
                    VideoUrl = !string.IsNullOrEmpty(occurrence.VideoUrl) ? occurrence.VideoUrl : "",
                    Handout = !string.IsNullOrEmpty(occurrence.Handout) ? occurrence.Handout : ""
                });
            }
            return Json(new { Record = response.webinar, OccurrenceList = occurrenceList });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult CreateOrUpdateWebinar(CreateOrUpdateWebinarModel model, bool updateWebinar)
        {
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            model.webinarDateUTC = TimeZoneInfo.ConvertTimeToUtc(model.webinarDate, custTZone);
            model.userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            model.systemAdminId = _appSettings.SystemAdminId;
            model.zoomClientId = _appSettings.ZoomClientId;
            model.zoomClientSecret = _appSettings.ZoomClientSecret;
            model.zoomOAuthURL = _appSettings.ZoomOAuthURL;
            model.zoomAccountId = _appSettings.ZoomAccountId;
            model.zoomAPIURL = _appSettings.ZoomAPIURL;
            model.zoomUserId = _appSettings.ZoomUserId;
            if (model.id == 0)
            {
                var response = WebinarUtility.CreateWebinar(model);
                return Json(new { Message = response });
            }
            else
            {
                string response;
                if (updateWebinar)
                {
                    response = WebinarUtility.UpdateWebinar(model);
                }
                else
                {
                    WebinarUtility.EditWebinar(model);
                    response = "Webinar updated.";
                }
                return Json(new { Message = response });
            }
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult AssignWebinar(int webinarId, string organizationIds)
        {
            var response = WebinarUtility.AssignWebinar(webinarId, organizationIds);
            return Json(new { Result = "OK", Record = response });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult RemoveAssignedWebinars(int Id)
        {
            var response = WebinarUtility.RemoveAssignedWebinars(Id);
            return Json(new { Result = "OK", Record = response });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListAssignedWebinars(int webinarId)
        {
            var result = WebinarUtility.ListAssignedWebinars(webinarId);
            return Json(new
            {
                Result = "OK",
                Records = result.webinars.Select(x => new
                {
                    Id = x.Id,
                    OrganizationName = x.Organization.Name
                })
            });
        }

        public ActionResult DashBoard()
        {
            WebinarDashboardModel model = new WebinarDashboardModel();
            string picturePath = "ProfilePictures/";
            var response = WebinarUtility.ListUserWebinars(HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value);
            model.upcomingWebinars = new List<WebinarDetails>();
            model.completedWebinars = new List<WebinarDetails>();
            foreach (var Webinar in response.webinars.OrderBy(x => x.Webinar.StartTime).Select(x => x.Webinar).ToList())
            {
                WebinarDetails webinar = new WebinarDetails();
                webinar.id = Webinar.Id;
                webinar.webinarId = Webinar.WebinarId;
                webinar.topic = Webinar.Topic;
                webinar.agenda = Webinar.Agenda;
                webinar.handout = Webinar.Handout;
                webinar.type = Webinar.Type.Value;
                webinar.presentedByName = Webinar.User.FirstName + " " + Webinar.User.LastName;
                webinar.presentedByImageUrl = !string.IsNullOrEmpty(Webinar.User.Picture) ? picturePath + Webinar.User.Picture : picturePath + CommonUtility.GetGenderSpecificImage(Webinar.User);
                webinar.isRegistered = Webinar.RegisteredUsersforWebinars.Any(x => x.UserId == HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
                webinar.userJoinUrl = Webinar.RegisteredUsersforWebinars.Any(x => x.UserId == HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value) ? Webinar.RegisteredUsersforWebinars.Where(x => x.UserId == HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).FirstOrDefault().UserJoinUrl : "";
                TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
                DateTime startTime = new DateTime();
                if (webinar.type == (int)WebinarTypes.RecurringWebinarFixedTime)
                {
                    webinar.presentedByBio = Webinar.User.AdminProperty != null ? Webinar.User.AdminProperty.Profile : "";
                    if (Webinar.WebinarOccurrences.Where(x => x.Status == "available" && x.StartTime >= DateTime.UtcNow).Count() > 0)
                    {
                        var latestWebinar = Webinar.WebinarOccurrences.Where(x => x.Status == "available" && x.StartTime >= DateTime.UtcNow).OrderBy(x => x.StartTime).FirstOrDefault();
                        Webinar.StartTime = latestWebinar.StartTime;
                        Webinar.Duration = latestWebinar.Duration;
                        webinar.handout = latestWebinar.Handout;
                        webinar.occurrences = new List<WebinarOccurrence>();
                        foreach (WebinarOccurrenceDto occurrence in Webinar.WebinarOccurrences.ToList())
                        {
                            var occurrenceDate = TimeZoneInfo.ConvertTimeFromUtc(occurrence.StartTime, custTZone);
                            webinar.occurrences.Add(new WebinarOccurrence
                            {
                                Date = occurrenceDate.ToString("MMMM dd, yyyy"),
                                Time = occurrenceDate.ToShortTimeString(),
                                Duration = occurrence.Duration,
                                Handout = occurrence.Handout
                            });
                        }

                        startTime = TimeZoneInfo.ConvertTimeFromUtc(Webinar.StartTime.Value, custTZone);
                        if (Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal)))
                        {
                            webinar.startTime = startTime.ToString("hh: mm tt");
                            if (Webinar.Duration.HasValue)
                                webinar.endTime = Translate.Message("L4034") + " " + startTime.AddMinutes(Webinar.Duration.Value).ToString("hh: mm tt");
                            webinar.dateDetails = startTime.ToString("d MMMM");
                            webinar.webinarImageUrl = !string.IsNullOrEmpty(Webinar.ImageUrl) ? Webinar.ImageUrl : "Images/webinar/placeholder-image.svg";
                            model.upcomingWebinars.Add(webinar);
                        }
                    }

                    if (Webinar.WebinarOccurrences.Where(x => x.StartTime < DateTime.UtcNow).Count() > 0)
                    {
                        int i = 1;
                        var pastWebinars = Webinar.WebinarOccurrences.Where(x => x.StartTime < DateTime.UtcNow).OrderBy(x => x.StartTime).ToList();
                        foreach (WebinarOccurrenceDto occurrence in pastWebinars)
                        {
                            WebinarDetails completedWebinar = new WebinarDetails(webinar);
                            completedWebinar.topic = completedWebinar.topic + " (Session " + i++ + ")";
                            Webinar.StartTime = occurrence.StartTime;
                            Webinar.Duration = occurrence.Duration;
                            Webinar.VideoUrl = occurrence.VideoUrl;
                            Webinar.Handout = occurrence.Handout;
                            completedWebinar.handout = occurrence.Handout;
                            startTime = TimeZoneInfo.ConvertTimeFromUtc(Webinar.StartTime.Value, custTZone);
                            completedWebinar.startDate = startTime;
                            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone);
                            TimeSpan difference = now - startTime;
                            completedWebinar.daysCount = difference.Days.ToString();
                            if (!string.IsNullOrEmpty(Webinar.VideoUrl))
                            {
                                if (Webinar.VideoUrl.Split('/')[Webinar.VideoUrl.Split('/').Length - 1].Split('=').Length > 1)
                                {
                                    completedWebinar.webinarVideoId = Webinar.VideoUrl.Split('/')[Webinar.VideoUrl.Split('/').Length - 1].Split('=')[1];
                                }
                                else
                                {
                                    completedWebinar.webinarVideoId = Webinar.VideoUrl.Split('/')[Webinar.VideoUrl.Split('/').Length - 1];
                                }
                            }
                            model.completedWebinars.Add(completedWebinar);
                        }
                    }
                }
                else
                {
                    startTime = TimeZoneInfo.ConvertTimeFromUtc(Webinar.StartTime.Value, custTZone);
                    webinar.startDate = startTime;
                    if (Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal)) && Webinar.StartTime >= DateTime.UtcNow)
                    {
                        webinar.startTime = startTime.ToString("hh: mm tt");
                        if (Webinar.Duration.HasValue)
                            webinar.endTime = Translate.Message("L4034") + " " + startTime.AddMinutes(Webinar.Duration.Value).ToString("hh: mm tt");
                        webinar.dateDetails = startTime.ToString("d MMMM");
                        webinar.webinarImageUrl = !string.IsNullOrEmpty(Webinar.ImageUrl) ? Webinar.ImageUrl : "Images/webinar/placeholder-image.svg";
                        webinar.presentedByBio = Webinar.User.AdminProperty != null ? Webinar.User.AdminProperty.Profile : "";
                        model.upcomingWebinars.Add(webinar);
                    }
                    else
                    {
                        webinar.presentedByBio = Webinar.User.AdminProperty != null ? Webinar.User.AdminProperty.Profile : "";
                        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone);
                        TimeSpan difference = now - startTime;
                        webinar.daysCount = difference.Days.ToString();
                        if (!string.IsNullOrEmpty(Webinar.VideoUrl))
                        {
                            if (Webinar.VideoUrl.Split('/')[Webinar.VideoUrl.Split('/').Length - 1].Split('=').Length > 1)
                            {
                                webinar.webinarVideoId = Webinar.VideoUrl.Split('/')[Webinar.VideoUrl.Split('/').Length - 1].Split('=')[1];
                            }
                            else
                            {
                                webinar.webinarVideoId = Webinar.VideoUrl.Split('/')[Webinar.VideoUrl.Split('/').Length - 1];
                            }
                        }
                        model.completedWebinars.Add(webinar);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult RegisterUserforWebinar(int id, string webinarId)
        {
            string phone = AccountUtility.GetPreferredPhoneNumber(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            var webResponse = WebinarUtility.RegisterUserforWebinar(HttpContext.Session.GetString(SessionContext.ParticipantName), HttpContext.Session.GetString(SessionContext.ParticipantEmail), phone, id, webinarId, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, _appSettings.ZoomClientId, _appSettings.ZoomClientSecret, _appSettings.ZoomOAuthURL, _appSettings.ZoomAccountId, _appSettings.ZoomAPIURL);

            return Json(new { Status = webResponse.Status, JoinUrl = webResponse.join_url });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult AddOrEditWebinarOccurrences(string occurrenceId, string videoUrl, string handout)
        {
            var response = WebinarUtility.AddOrEditWebinarOccurrences(occurrenceId, videoUrl, handout);
            return Json(new { Status = response });
        }
    }
}