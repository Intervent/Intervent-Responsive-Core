using Intervent.DAL;
using Intervent.HWS;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Intervent.Web.DTO.Diff;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data.OleDb;
using System.Security.Claims;

namespace InterventWebApp
{
    public class AdminController : BaseController
    {
        private readonly IOptions<AppSettings> iOptionAppSettings;
        private readonly AppSettings _appSettings;
        private readonly IHostEnvironment environment;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings, IHostEnvironment environment)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            iOptionAppSettings = appSettings;
            this.environment = environment;
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AdminDashboard()
        {
            AdminDashboardModel model = CheckPendingParticipantTimer();
            int UserId = HttpContext.Session.GetInt32(SessionContext.AdminId).Value;
            ClearParticipantSession();
            HttpContext.Session.SetInt32(SessionContext.SessionTimeout, _appSettings.SessionTimeOut);
            var response = AdminUtility.ReadAdmin(UserId).admin;
            model.picture = response.user.Picture;
            HttpContext.Session.SetString(SessionContext.DateFormat, response.user.Country1 != null ? response.user.Country1.DateFormat : "MM/dd/yyyy");
            if (response.user.Roles.Count != 0)
            {
                model.AdminModules = response.user.Roles[0].AdminModules;
                model.messageCount = MessageUtility.GetMessageCountForDashboard(UserId, true, null, false, _appSettings.SystemAdminId).MessageBoardCount;
            }
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AdminDashboardNew()
        {
            AdminDashboardModel model = CheckPendingParticipantTimer();
            int UserId = HttpContext.Session.GetInt32(SessionContext.AdminId).Value;
            ClearParticipantSession();
            HttpContext.Session.SetInt32(SessionContext.SessionTimeout, _appSettings.SessionTimeOut);
            var response = AdminUtility.ReadAdmin(UserId).admin;
            model.UserId = response.user.Id;
            model.FirstName = response.user.FirstName;
            model.LastName = response.user.LastName;
            HttpContext.Session.SetString(SessionContext.DateFormat, response.user.Country1 != null ? response.user.Country1.DateFormat : "MM/dd/yyyy");
            if (response.user.Roles.Count != 0)
            {
                // model.MessageCount = MessageUtility.GetMessageCountForDashboard(UserId, true, null).MessageBoardCount;
                //model.IsAdmin = response.user.Roles.Where(x => x.Code == "ADMIN").Count() > 0;
                model.IsCoach = response.user.Roles.Where(x => x.Code == "COACH").Count() > 0;
                model.IsCSR = response.user.Roles.Where(x => x.Code == "CSR").Count() > 0;
                if (model.IsAdmin)
                    model.DashBoardDetails = AdminUtility.GetAdminDashboardDetails();
            }
            model.ContactRequirements = ParticipantUtility.GetAllContactRequirements().Select(x => new SelectListItem { Text = x.AlertType, Value = x.Id.ToString() });
            return View(model);
        }

        private AdminDashboardModel CheckPendingParticipantTimer()
        {
            AdminDashboardModel model = new AdminDashboardModel();
            if (HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue)
            {
                var pendingTimer = ParticipantUtility.CheckPendingParticipantTimer(HttpContext.Session.GetInt32(SessionContext.AdminId).Value);
                if (pendingTimer != null && pendingTimer.timer != null)
                {
                    model.ShowStopTimer = true;
                    model.ParticipantId = pendingTimer.timer.UserId;
                    model.ParticipantName = pendingTimer.timer.User.FirstName + " " + pendingTimer.timer.User.LastName;
                    model.TrackerStartTime = pendingTimer.timer.StartTime;
                    model.TimeTrackingDispositionList = ParticipantUtility.GetTimeTrackingDispositionList().Where(x => x.ShowInUI).Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() }).OrderBy(x => x.Text);
                }
            }
            return model;
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AdminMenu()
        {
            AdminMenuModel model = new AdminMenuModel();
            var response = AdminUtility.ReadAdmin(HttpContext.Session.GetInt32(SessionContext.AdminId).Value).admin;
            if (response.user.Roles.Count != 0)
            {
                model.AdminModules = response.user.Roles[0].AdminModules.OrderBy(x => x.Name).ToList();
                model.MessageCount = MessageUtility.GetMessageCountForDashboard(HttpContext.Session.GetInt32(SessionContext.AdminId).Value, true, null, false, _appSettings.SystemAdminId).MessageBoardCount;
            }
            return PartialView("_AdminMenu", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult ParticipantSearch([FromBody] AdvancedSearchRequestModel request)
        {
            AdvancedSearchModel model = new AdvancedSearchModel();
            AdvancedSearchUsersResponse response = ParticipantUtility.AdvancedSearchUsers(request, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            model.Result = response.Result;
            model.TotalRecords = response.TotalRecords;
            return PartialView("_ParticipantResult", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult GetAppointments([FromBody] GetAppointmentsModel request)
        {
            AppointmentsListModel model = new AppointmentsListModel();
            var response = SchedulerUtility.GetAppointments(request.startDate, request.endDate, HttpContext.Session.GetInt32(SessionContext.AdminId).Value, null, null, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null);
            if (response.Appointments != null)
            {
                model.Appointments = response.Appointments.OrderBy(x => x.Date).ToList();
                if (request.recentlyEnrolled.HasValue && request.recentlyEnrolled.Value)
                {
                    DateTime date = DateTime.UtcNow.AddMonths(-3);
                    model.Appointments = model.Appointments.Where(x => x.User.CreatedOn > date).ToList();
                }
            }
            model.Date = request.startDate;
            return PartialView("_MyAppointments", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult RecentlyEnrolledUsers()
        {
            RecentlyEnrolledModel model = new RecentlyEnrolledModel();
            model.Users = ParticipantUtility.RecentlyEnrolledUsers(Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value));
            return PartialView("_RecentlyEnrolled", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult MissedAppointments([FromBody] MissedAppointmentsModel request)
        {
            AppointmentsListModel model = new AppointmentsListModel();
            var response = SchedulerUtility.GetAppointments(request.startDate, request.endDate, HttpContext.Session.GetInt32(SessionContext.AdminId).Value, null, null, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null);
            if (response.Appointments != null)
            {
                model.Appointments = response.Appointments.OrderBy(x => x.Date).ToList();
                DateTime date = DateTime.UtcNow.AddMonths(-3);
                model.Appointments = model.Appointments.Where(x => x.User.CreatedOn > date).ToList();
            }
            model.Date = request.startDate;
            return PartialView("_MissedAppointments", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListUsers(string name, bool? hideOwnUser, bool? favContacts, bool? usersWithRole)
        {
            var response = AdminUtility.ListUsers(name, Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value), usersWithRole);
            if (hideOwnUser.HasValue && hideOwnUser.Value)
                response.Users.Remove(response.Users.Where(x => x.Id == HttpContext.Session.GetInt32(SessionContext.AdminId).Value).FirstOrDefault());
            if (favContacts.HasValue && favContacts.Value)
            {
                var favResponse = MessageUtility.ListFavoriteContacts(HttpContext.Session.GetInt32(SessionContext.UserId).Value);
                if (favResponse != null && favResponse.favoriteContacts != null)
                {
                    foreach (var fav in favResponse.favoriteContacts)
                    {
                        response.Users.Remove(response.Users.Where(x => x.Id == fav.Id).FirstOrDefault());
                    }
                }
            }
            if (response.Users != null)
            {
                UserNamewithId users = new UserNamewithId();

                users.userList = (from item in response.Users
                                  select new
                                  {
                                      Id = item.Id,
                                      Name = item.FirstName + " " + item.LastName + " (Org: " + item.Organization.Name.Split(' ').First() + "; Id: " + item.Id + ")"
                                  }).ToArray();

                return Json(users.userList);
            }
            return Json(response);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public async Task<ActionResult> AdminProfile(int? Id)
        {
            AdminProfileModel model = new AdminProfileModel();
            model.BaseUrl = _appSettings.EmailUrl;
            model.Countries = CommonUtility.ListCountries().OrderBy(t => t.Code == "US" ? 1 : 2).ThenBy(t => t.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            model.TimeZones = CommonUtility.GetTimeZones(null).TimeZones.Select(x => new SelectListItem { Text = Translate.Message(x.TimeZoneDisplay), Value = x.Id.ToString() })
                .OrderBy(t => t.Text);
            model.NamePrefixList = ListOptions.GetNamePrefixList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });

            model.GenderList = ListOptions.GetGenderList(null).Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.years = CommonUtility.GetYears(true).Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
            model.months = CommonUtility.GetMonths().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
            model.days = CommonUtility.GetDays().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
            model.Roles = AccountUtility.ListRoles().Select(x => new SelectListItem { Text = x.Name, Value = x.Name });
            model.Specialization = AccountUtility.ListSpecialization(false, GetParticipantPortalId()).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageId), Value = x.Id.ToString() });
            model.Language = CommonUtility.GetLanguages().Languages.Select(x => new SelectListItem { Text = Translate.Message(x.Language), Value = x.Id.ToString() }).ToList();
            model.States1 = CommonUtility.ListStates(41).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            model.Units = ListOptions.GetUnits().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.ListOrganizations = PortalUtility.ListOrganizations(new OrganizationListModel() { removechildOrganizations = false }).Organizations.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            ViewData["languageList"] = CommonUtility.GetLanguages().Languages.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.LanguageCode });
            UserDto user = new UserDto();
            AdminPropertyDto adminProperty = new AdminPropertyDto();
            if (Id.HasValue)
            {
                var response = await AccountUtility.GetUser(_userManager, null, null, Id.Value, null, null);
                user.Id = response.User.Id;
                user.FirstName = response.User.FirstName;
                user.LastName = response.User.LastName;
                user.MiddleName = response.User.MiddleName;
                user.Suffix = response.User.Suffix;
                user.Email = response.User.Email;
                user.NamePrefix = response.User.NamePrefix;
                if (response.User.DOB.HasValue)
                {
                    model.year = response.User.DOB.Value.Year;
                    model.month = response.User.DOB.Value.Month;
                    model.day = response.User.DOB.Value.Day;
                }
                user.Gender = response.User.Gender;
                user.Address = response.User.Address;
                user.Address2 = response.User.Address2;
                user.City = response.User.City;
                user.State = response.User.State;
                user.Country = response.User.Country;
                user.Zip = response.User.Zip;
                user.HomeNumber = response.User.HomeNumber;
                user.WorkNumber = response.User.WorkNumber;
                user.CellNumber = response.User.CellNumber;
                user.Picture = response.User.Picture;
                user.Occupation = response.User.Occupation;
                user.TimeZoneId = response.User.TimeZoneId;
                user.OrganizationId = response.User.OrganizationId;
                user.Unit = response.User.Unit;
                user.IsActive = response.User.IsActive;
                user.InactiveReason = response.User.InactiveReason;
                user.Organization = response.User.Organization;
                if (response.User.Roles != null && response.User.Roles.Count > 0)
                {
                    model.UserRoles = response.User.Roles;
                    model.CurrentRole = String.Join("-", model.UserRoles.Select(x => x.Name).ToArray());
                }
                if (response.User.Specializations != null && response.User.Specializations.Count > 0)
                {
                    model.UserSpecializations = response.User.Specializations;
                    model.CurrentSpecialization = String.Join("-", model.UserSpecializations.Select(x => x.Id).ToArray());
                }
                if (response.User.Languages != null && response.User.Languages.Count > 0)
                {
                    model.UserLanguages = response.User.Languages;
                    model.CurrentLanguage = String.Join("-", model.UserLanguages.Select(x => x.Id).ToArray());
                    if (model.UserLanguages.Count == 1 && model.UserLanguages[0].Language.Equals("French"))
                    {
                        model.NamePrefixList = model.NamePrefixList.Where(x => x.Value != "MS");
                    }
                }
                if (response.User.CoachStates != null && response.User.CoachStates.Count > 0)
                {
                    model.CoachStates = response.User.CoachStates;
                    model.CurrenCoachtState = String.Join("-", model.CoachStates.Select(x => x.Id).ToArray());
                }
                if (response.User.AdminProperty != null)
                {
                    adminProperty.Profile = response.User.AdminProperty.Profile;
                    adminProperty.Video = response.User.AdminProperty.Video;
                    adminProperty.AllowAppt = response.User.AdminProperty.AllowAppt;
                    adminProperty.MeetingId = response.User.AdminProperty.MeetingId;
                }
                user.AdminProperty = adminProperty;
            }
            if (user.Country.HasValue)
                model.States = CommonUtility.ListStates(user.Country.Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            else
                model.States = Enumerable.Empty<SelectListItem>();
            model.user = user;
            model.user.BaseUrl = _appSettings.EmailUrl;
            return PartialView("_AdminProfile", model);
        }

        [HttpPost]
        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public async Task<JsonResult> AdminProfile(AdminProfileModel model, string roles = null, string specialities = null,
            string languages = null, string eligibleStates = null, string profileLanguage = null)
        {
            if (model.user.Id > 0)
            {
                bool adminAccess = false;
                if (CommonUtility.IsSuperAdmin(User.RoleCode()))
                {
                    adminAccess = true;
                }
                await AdminUtility.UpdateAdminProfile(_userManager, model, Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value), adminAccess, roles, specialities, languages, eligibleStates, profileLanguage);
                if (model.user.Id == Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value))
                {
                    var Identity = new ClaimsIdentity(User.Identity);
                    Identity.TryRemoveClaim(Identity.FindFirst("TimeZone"));
                    Identity.AddClaim(new Claim("TimeZone", CommonUtility.GetTimeZones(model.user.TimeZoneId).TimeZones[0].TimeZoneId));
                    //TODO AuthenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(Identity), new AuthenticationProperties { IsPersistent = true });
                }
            }
            return Json("success");
        }

        [ModuleControl(null, RoleCode.Administrator)]
        [HttpPost]
        public JsonResult GetProfileText(int id, string language)
        {
            var response = AdminUtility.GetProfileText(id, language);
            return Json(new { Result = "OK", Record = response });
        }

        #region Roles

        [ModuleControl(Modules.Groups)]
        public ActionResult Groups()
        {
            GroupsModel model = new GroupsModel();
            model.UserRoleCodes = AdminUtility.RoleCodeList();
            model.AdminModules = AdminUtility.ListAdminModule().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            model.OrganizationList = PortalUtility.ListOrganizations(new OrganizationListModel() { removechildOrganizations = false }).Organizations.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            return View(model);
        }

        [ModuleControl(Modules.Groups)]
        public JsonResult ListRoles()
        {
            var response = AccountUtility.ListRoles();

            return Json(new
            {
                Result = "OK",
                Records = response.Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    AdminModules = x.AdminModules
                })
            });
        }

        [ModuleControl(Modules.Groups)]
        public JsonResult ReadRole(int id)
        {
            var response = AccountUtility.ReadRole(id);
            var orgList = response.role.Organizations.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            response.role.Organizations = null;
            return Json(new { Result = "OK", Record = response.role, Organizations = orgList });
        }

        [HttpPost]
        [ModuleControl(Modules.Groups)]
        public JsonResult AddEditRole(int? id, string name, string code, string adminModules, string organizations)
        {
            var response = AccountUtility.AddEditRole(id, name, code, adminModules, organizations);
            return Json(new { Result = "OK", Record = response });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public async Task<JsonResult> GetUsersByRole(bool allUserswithRole, int? organization, string firstName, string lastName, int? page, int? pageSize, int? totalRecords)
        {
            var response = await AccountUtility.GetUsersByRole(_userManager, allUserswithRole, organization, firstName, lastName, page, pageSize, totalRecords, HttpContext.Session.GetInt32(SessionContext.UserId).Value);

            var users = (from item in response.users
                         select new
                         {
                             Id = item.Id,
                             Name = item.FirstName + " " + item.LastName,
                             Email = item.Email,
                             HomeNumber = item.HomeNumber,
                             CellNumber = item.CellNumber,
                             DOB = item.DOB.HasValue ? CommonUtility.dateFormater(item.DOB.Value, false, HttpContext.Session.GetString(SessionContext.DateFormat)) : ""
                         }).ToArray();

            if (response != null)
                return Json(new { Result = "OK", Records = users, TotalRecords = response.totalRecords });
            else
                return Json(new { Result = "OK", Records = new List<UserDto>(), TotalRecordCount = 0 });
        }

        #endregion

        [ModuleControl(Modules.Users, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AdminUsers()
        {
            AdminUsersModel model = new AdminUsersModel();
            model.OrganizationList = PortalUtility.ListOrganizations(null).Organizations.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public ActionResult UploadScreeningData()
        {
            AdminUsersModel model = new AdminUsersModel();
            model.Units = ListOptions.GetUnits().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult Email()
        {
            return PartialView("_Email");
        }

        #region Task Management

        [ModuleControl(Modules.Tasks)]
        public async Task<ActionResult> TaskManagement(bool? BackToReport)
        {
            TaskManagementModel model = new TaskManagementModel();
            if (BackToReport.HasValue && BackToReport.Value)
            {
                model.tempData = JsonConvert.DeserializeObject<FilterReportTempData>(TempData["filterReportData"] as string);
            }
            model.TaskList = AdminUtility.ListTaskType(HttpContext.Session.GetInt32(SessionContext.AdminId).Value).taskTypes.OrderBy(x => x.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            model.StatusList = AdminUtility.TaskStatusList();
            model.OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).OrderBy(x => x.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            var response = await AccountUtility.GetUsersByRole(_userManager, true, null, null, null, null, null, null, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            model.OwnerList = response.users.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() });
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return View(model);
        }

        [ModuleControl(Modules.Tasks)]
        public JsonResult GetTaskList(DateTime? startDate, DateTime? endDate, int[] taskTypeId, int? ownerId, string status, int page, int pageSize, int totalRecords, string organizationIds, bool download)
        {
            List<int> taskTypeIndexes = new List<int>();
            List<int?> organizationIndexes = new List<int?>();
            taskTypeId = taskTypeId != null ? taskTypeId : new int[] { 0 };
            var response = AdminUtility.GetTaskList(startDate, endDate, taskTypeId, ownerId, status, page, pageSize, totalRecords, organizationIds, download, HttpContext.Session.GetInt32(SessionContext.UserId).Value, User.TimeZone());
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            int?[] OrganizatoinLst = !string.IsNullOrEmpty(organizationIds) ? organizationIds.Split(',').Select(str => (int?)int.Parse(str)).ToArray() : null;
            var taskTypes = AdminUtility.ListTaskType(HttpContext.Session.GetInt32(SessionContext.AdminId).Value).taskTypes.OrderBy(x => x.Name).Select(x => x.Id.ToString()).ToList();

            for (int i = 0; i < taskTypeId.Length; i++)
            {
                taskTypeIndexes.Add(taskTypes.IndexOf(taskTypeId[i].ToString()));
            }
            if (OrganizatoinLst != null)
            {
                var organizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).OrderBy(x => x.Name).Select(x => x.Id.ToString()).ToList();
                for (int i = 0; i < OrganizatoinLst.Length; i++)
                {
                    organizationIndexes.Add(organizationList.IndexOf(OrganizatoinLst[i].ToString()));
                }
            }
            FilterReportTempData filterReportTempData = new FilterReportTempData
            {
                page = page,
                totalRecords = totalRecords,
                startDate = startDate,
                endDate = endDate,
                Owner = ownerId,
                Task = taskTypeId,
                TaskIndex = taskTypeIndexes.ToArray(),
                TaskStatus = status,
                organizationIndex = organizationIndexes != null ? organizationIndexes.ToArray() : null,
                organizations = OrganizatoinLst != null ? OrganizatoinLst : null
            };
            TempData["filterReportData"] = JsonConvert.SerializeObject(filterReportTempData);
            return Json(new
            {
                Result = "OK",
                TotalRecords = response.totalRecords,
                Records = response.tasks.Select(x => new
                {
                    Id = x.Id,
                    Task = x.TaskType.Name,
                    Owner = x.User1.FirstName + " " + x.User1.LastName.Substring(0, 1),
                    Status = x.Status,
                    StartDate = TimeZoneInfo.ConvertTimeFromUtc(x.CreatedOn, custTZone).ToShortDateString(),
                    User = x.User.FirstName + " " + x.User.LastName,
                    DOB = x.User.DOB.Value.ToShortDateString(),
                    Gender = x.User.Gender.HasValue ? (x.User.Gender == 1 ? GenderDto.Male.Description : GenderDto.Female.Description) : null,
                    Company = x.User.Organization.Name,
                    UserId = x.User.Id,
                    UniqueID = x.User.UniqueId,
                    Email = x.User.Email,
                    Address = x.User.Address,
                    Address2 = x.User.Address2,
                    City = x.User.City,
                    State = x.User.State.HasValue ? x.User.State1.Name : "",
                    ZipCode = x.User.Zip,
                    CellNumber = x.User.CellNumber,
                    HomeNumber = x.User.HomeNumber,
                    WorkNumber = x.User.WorkNumber
                })
            });
        }

        [ModuleControl(Modules.Tasks)]
        public JsonResult ListTaskType()
        {
            var tasks = AdminUtility.ListTaskType(HttpContext.Session.GetInt32(SessionContext.AdminId).Value).taskTypes.Select(c => new { DisplayText = c.Name, Value = c.Id }).OrderBy(s => s.DisplayText);
            return Json(new { Result = "OK", Options = tasks });
        }

        [ModuleControl(Modules.Tasks)]
        public JsonResult ListTaskStatus()
        {
            var tasks = AdminUtility.TaskStatusList().Select(c => new { DisplayText = c.Text, Value = c.Value }).OrderBy(s => s.DisplayText);
            return Json(new { Result = "OK", Options = tasks });
        }

        [ModuleControl(Modules.Tasks)]
        public JsonResult ReadTask(int? taskId, int? userId, int? taskType)
        {
            var response = AdminUtility.ReadTask(taskId, userId, taskType).task;
            if (response != null)
            {
                return Json(new
                {
                    Result = "OK",
                    Id = response.Id,
                    Status = response.Status,
                    StatusDate = response.Status,
                    TypeId = response.TaskType.Id,
                    Comment = response.Comment,
                    Owner = response.User1.Id
                });
            }
            else
                return Json(new { Result = "None" });
        }

        [Authorize]
        public JsonResult AddEditTask(int? id, int taskTypeId, string status, int? user, int owner, string comment)
        {
            var response = AdminUtility.AddEditTask(id, taskTypeId, status, user, owner, comment, true);
            return Json(new { Result = "OK", Portal = response.success });
        }

        [ModuleControl(Modules.Tasks)]
        public JsonResult AdminTaskStausBulkUpdate(string taskids, string status)
        {
            var response = AdminUtility.AdminTaskStausBulkUpdate(taskids, status, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            return Json(new { Result = "OK", Portal = response.success });
        }

        #endregion

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListAdminModule()
        {
            var response = AdminUtility.ListAdminModule();
            return Json(response);
        }

        /*private Microsoft.Owin.Security.IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }*/

        [HttpGet]
        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult ParticipantHistorySearch(int id)
        {
            ParticipantHistorySearchModel model = new ParticipantHistorySearchModel();
            model.UserHistoryCategories = CommonUtility.GetUserHistoryCategories();
            model.UserHistoryCategoryId = id;
            int adminId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            model.ParticipantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            model.AbsoluteUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}{HttpContext.Request.Path}";
            ReadParticipantInfoResponse participantResponse = ParticipantUtility.ReadParticipantInfo(id, adminId);
            if (participantResponse != null && participantResponse.user != null)
            {
                model.ParticipantName = String.Join(" ", participantResponse.user.FirstName, participantResponse.user.LastName);
            }
            return View(model);
        }

        [HttpPost]
        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ParticipantHistorySearch(int id, int? userHistoryCategoryId, DateTime? startDate, DateTime? endDate, int page, int pageSize, int totalRecords)
        {
            var response = AdminUtility.SearchParticipantHistory(id, userHistoryCategoryId, startDate, endDate, page, pageSize, totalRecords);
            TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            return Json(new
            {
                TotalRecords = response.TotalRecords,
                Records = (from u in response.UserChanges
                           join uhc in UserHistoryCategoryDto.GetAll() on u.UserHistoryCategoryId equals uhc.Id
                           select new
                           {
                               Id = u.Id,
                               LogDate = TimeZoneInfo.ConvertTimeFromUtc(u.LogDate, Zone).ToString(),
                               Category = uhc.Description
                           }).ToList()
            });
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public PartialViewResult ParticipantHistoryDetailedInfo(int id)
        {
            var model = new ParticipantHistoryChangesModel();
            var ParticipantHistory = AdminUtility.GetParticipantHistory(id);
            int UserHistoryCategoryId = ParticipantHistory.UserChange.UserHistoryCategoryId;
            IList<PropertyCompare> changes = JsonConvert.DeserializeObject<IList<PropertyCompare>>(ParticipantHistory.UserChange.Changes);
            IEnumerable<string> tinyintDataTypeProperties = TinyIntDataTypeProperties();
            IEnumerable<string> bitDataTypeProperties = BitDataTypeProperties();
            IEnumerable<string> dropdownDataTypeProperties = DropdownDataTypeProperties();
            foreach (PropertyCompare change in changes)
            {
                if (tinyintDataTypeProperties.Contains(change.PropertyName))
                {
                    if (!String.IsNullOrEmpty(change.CurrentValue))
                    {
                        if (change.CurrentValue == "1")
                        {
                            change.CurrentValue = "Yes";
                        }
                        else if (change.CurrentValue == "2")
                        {
                            change.CurrentValue = "No";

                        }
                    }
                    if (!String.IsNullOrEmpty(change.NewValue))
                    {
                        if (change.NewValue == "1")
                        {
                            change.NewValue = "Yes";
                        }
                        else if (change.NewValue == "2")
                        {
                            change.NewValue = "No";

                        }
                    }
                }
                else if (bitDataTypeProperties.Contains(change.PropertyName))
                {
                    if (!String.IsNullOrEmpty(change.CurrentValue) && change.CurrentValue == "1")
                    {
                        change.CurrentValue = "Yes";
                    }
                    else if (!String.IsNullOrEmpty(change.CurrentValue) && change.CurrentValue == "2")
                    {
                        change.CurrentValue = "No";
                    }
                    else
                    {
                        change.CurrentValue = "Don't know";
                    }
                    if (!String.IsNullOrEmpty(change.NewValue) && change.NewValue == "1")
                    {
                        change.NewValue = "Yes";
                    }
                    else if (!String.IsNullOrEmpty(change.NewValue) && change.NewValue == "2")
                    {
                        change.NewValue = "No";
                    }
                    else
                    {
                        change.NewValue = "Don't know";
                    }
                }
                else if (dropdownDataTypeProperties.Contains(change.PropertyName))//dropdown
                {
                    if (!String.IsNullOrEmpty(change.CurrentValue))
                    {
                        change.CurrentValue = GetDropDownVal(change.PropertyName, change.CurrentValue, UserHistoryCategoryId);
                    }
                    if (!String.IsNullOrEmpty(change.NewValue))
                    {
                        change.NewValue = GetDropDownVal(change.PropertyName, change.NewValue, UserHistoryCategoryId);
                    }
                }
            }
            changes.Remove(changes.Where(x => x.PropertyName.Equals("UpdatedOn")).FirstOrDefault());
            var property = new PropertyCompare();
            property.NewValue = ParticipantHistory.UserChange.UpdatedByName;
            property.PropertyName = "Updated By";
            changes.Add(property);
            model.Changes = changes;
            return PartialView("_ParticipantHistoryDetailedInfo", model);
        }

        static IEnumerable<string> TinyIntDataTypeProperties()
        {
            List<string> lst = new List<string>();
            lst.Add("PhysicalExam");
            lst.Add("StoolTest");
            lst.Add("ColTest");
            lst.Add("ColStoolTest");
            lst.Add("SigTest");
            lst.Add("PSATest");
            lst.Add("PapTest");
            lst.Add("BoneTest");
            lst.Add("Mammogram");
            lst.Add("DentalExam");
            lst.Add("BPCheck");
            lst.Add("CholTest");
            lst.Add("GlucoseTest");
            lst.Add("EyeExam");
            lst.Add("NoTest");
            lst.Add("TetanusShot");
            lst.Add("FluShot");
            lst.Add("MMR");
            lst.Add("Varicella");
            lst.Add("HepBShot");
            lst.Add("ShinglesShot");
            lst.Add("HPVShot");
            lst.Add("PneumoniaShot");
            lst.Add("NoShots");
            lst.Add("MetSyn");
            lst.Add("DidYouFast");
            lst.Add("TextDrive");
            lst.Add("DUI");
            lst.Add("RideDUI");
            lst.Add("RideNoBelt");
            lst.Add("Speed10");
            lst.Add("BikeNoHelmet");
            lst.Add("MBikeNoHelmet");
            lst.Add("LiftRight");
            lst.Add("Pregnant");
            lst.Add("BreastFeed");
            lst.Add("Stroke");
            lst.Add("HeartAttack");
            lst.Add("Angina");
            lst.Add("ToldArteries");
            lst.Add("ToldBabyNine");
            lst.Add("ToldBlock");
            lst.Add("ToldHighBP");
            lst.Add("ToldHighChol");
            lst.Add("ToldDiabetes");
            lst.Add("ToldGestDiab");
            lst.Add("ToldPolycyst");
            lst.Add("ToldAsthma");
            lst.Add("ToldBronchitis");
            lst.Add("ToldCancer");
            lst.Add("ToldKidneyDisease");
            lst.Add("OtherChronic");
            lst.Add("OtherChronicProb");
            lst.Add("HighBPMed");
            lst.Add("HighCholMed");
            lst.Add("DiabetesMed");
            lst.Add("Insulin");
            lst.Add("AnginaMed");
            lst.Add("ToldHeartBlock");
            lst.Add("HeartFailMed");
            lst.Add("HeartCondMed");
            lst.Add("AsthmaMed");
            lst.Add("BronchitisMed");
            lst.Add("ArthritisMed");
            lst.Add("Osteoarthritis");
            lst.Add("Rheumatoid");
            lst.Add("Psoriatic");
            lst.Add("Spondylitis");
            lst.Add("OtherOrthritis");
            lst.Add("Crohns");
            lst.Add("OtherChronicMed");
            lst.Add("BloodThinMed");
            lst.Add("AllergyMed");
            lst.Add("RefluxMed");
            lst.Add("UlcerMed");
            lst.Add("MigraineMed");
            lst.Add("OsteoporosisMed");
            lst.Add("AnxietyMed");
            lst.Add("DepressionMed");
            lst.Add("BackPainMed");
            lst.Add("NoPrescMed");
            lst.Add("SmokeCig");
            lst.Add("OtherTobacco");
            lst.Add("Cigar");
            lst.Add("Pipe");
            lst.Add("SmokelessTob");
            lst.Add("ECig");
            lst.Add("VigExer");
            lst.Add("VigExerPFW");
            lst.Add("ModExer");
            lst.Add("ModExerPFW");
            lst.Add("ExertPain");
            lst.Add("LowFatDiet");
            lst.Add("LowFatDietPFW");
            lst.Add("HealthyCarb");
            lst.Add("HealthyCarbPFW");
            lst.Add("TwoAlcohol");
            lst.Add("OneAlcohol");
            lst.Add("OverWeight");
            lst.Add("FeelStress");
            lst.Add("FeelStressPFW");
            lst.Add("FeelAnxiety");
            lst.Add("FeelAnxietyPFW");
            lst.Add("FeelDepression");
            lst.Add("OnlyDepression");
            lst.Add("PhysicalProb");
            lst.Add("Arthritis");
            lst.Add("BreathProb");
            lst.Add("BackInjury");
            lst.Add("ChronicPain");
            lst.Add("OtherPhysLimit");
            lst.Add("SleepApnea");
            lst.Add("FeelTired");
            lst.Add("Snore");
            lst.Add("BreathPause");
            lst.Add("Headache");
            lst.Add("Sleepy");
            lst.Add("NoIssue");
            lst.Add("SleepApneaMed");
            lst.Add("WaterPipes");
            lst.Add("OtherFormofTob");
            return lst;
        }

        static IEnumerable<string> BitDataTypeProperties()
        {
            List<string> lst = new List<string>();
            lst.Add("ASCVD");
            lst.Add("Diabetes");
            lst.Add("ReduceLdl");
            lst.Add("RaiseHDL");
            lst.Add("ReduceTrig");
            lst.Add("ReduceBP");
            lst.Add("ReduceGluc");
            lst.Add("NutMaint");
            lst.Add("NutLowBMI");
            lst.Add("NutLtFat");
            lst.Add("NutUnderWeight");
            lst.Add("NutLtSatFat");
            lst.Add("NutDrinkWater");
            lst.Add("DrClearRef1");
            lst.Add("DrClearRef2");
            lst.Add("BloodTestRef");
            lst.Add("MedRef1");
            lst.Add("MedRef2");
            lst.Add("LdlRef1");
            lst.Add("LdlRef2");
            lst.Add("LdlRef3");
            lst.Add("LdlRef4");
            lst.Add("LdlRef5");
            lst.Add("LdlRef6");
            lst.Add("LdlRef7");
            lst.Add("HdlRef1");
            lst.Add("HdlRef2");
            lst.Add("TrigRef1");
            lst.Add("TrigRef2");
            lst.Add("TrigRef3");
            lst.Add("TrigRef4");
            lst.Add("TrigRef5");
            lst.Add("MedRef3");
            lst.Add("CholRef1");
            lst.Add("CholRef2");
            lst.Add("GlucRef1");
            lst.Add("GlucRef2");
            lst.Add("GlucRef3");
            lst.Add("A1CRef1");
            lst.Add("A1CRef2");
            lst.Add("AspRef");
            lst.Add("NicRef");
            lst.Add("BPRef");
            lst.Add("RMRRef");
            lst.Add("GoalsGenerated");
            lst.Add("FireExting");
            lst.Add("SmokeDetect");
            lst.Add("PlanBecPreg");
            lst.Add("HeartHist");
            lst.Add("CancerHist");
            lst.Add("DiabetesHist");
            return lst;
        }

        static IEnumerable<string> DropdownDataTypeProperties()
        {
            List<string> lst = new List<string>();
            lst.Add("InactiveReason");
            lst.Add("ProgramsinPortalsId");
            lst.Add("UpdatedBy");
            lst.Add("CoachId");
            lst.Add("State");
            lst.Add("StateOfHealth");
            lst.Add("LifeSatisfaction");
            lst.Add("JobSatisfaction");
            lst.Add("RelaxMed");
            lst.Add("WorkMissPers");
            lst.Add("WorkMissFam");
            lst.Add("EmergRoomVisit");
            lst.Add("AdmintHosp");
            lst.Add("DrVisitPers");
            lst.Add("ProductivityLoss");
            lst.Add("TimeZoneId");
            lst.Add("SmokeHist");
            lst.Add("Race");
            lst.Add("ContactMode");
            lst.Add("PreferredContactTimeId");
            lst.Add("Source");
            lst.Add("AdmitHosp");
            lst.Add("Country");
            lst.Add("Unit");
            lst.Add("BPArm");
            lst.Add("Gender");

            return lst;
        }

        public string GetDropDownVal(string property, string value, int userHistoryCategoryId)
        {
            var result = "";
            if (property.Equals("InactiveReason"))
            {
                if (userHistoryCategoryId == UserHistoryCategoryDto.UserProfile.Id)
                    result = AccountUtility.ListInactiveReason().Select(x => new SelectListItem { Text = x.Reason, Value = x.Id.ToString() }).Where(x => x.Value == value).FirstOrDefault().Text;
                else if (userHistoryCategoryId == UserHistoryCategoryDto.UserProgram.Id)
                    result = ProgramUtility.ListInactiveReasons().Select(x => new SelectListItem { Text = x.Reason, Value = x.Id.ToString() }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("ProgramsinPortalsId"))
            {
                result = PortalUtility.ReadPrograminPortal(Convert.ToInt32(value.ToString())).programinPortal.NameforAdmin;
            }
            if (property.Equals("UpdatedBy") || property.Equals("CoachId"))
            {
                var user = ParticipantUtility.ReadUserParticipation(Convert.ToInt32(value.ToString())).user;
                result = user.FirstName + " " + user.LastName;
            }
            if (property.Equals("State"))
            {
                result = CommonUtility.ListAllStates().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("Race"))
            {
                result = CommonUtility.ListRace(null).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageCode), Value = x.Id.ToString() }).Where(x => x.Value == value).FirstOrDefault().Text;

            }
            if (property.Equals("Source"))
            {
                result = ListOptions.GetSource().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("ContactMode"))
            {
                result = ListOptions.GetPreferredContactMode().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("PreferredContactTimeId"))
            {
                result = ListOptions.GetPreferredContactTimes().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("StateOfHealth"))
            {
                result = ListOptions.GetStateOfHealthLists().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("LifeSatisfaction"))
            {
                result = ListOptions.GetLifeSatisfactionList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("JobSatisfaction"))
            {
                result = ListOptions.GetJobSatisfactionList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("RelaxMed"))
            {
                result = ListOptions.GetRelaxMedList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("WorkMissPers") || property.Equals("WorkMissFam"))
            {
                result = ListOptions.GetMissDays().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("EmergRoomVisit") || property.Equals("AdmitHosp") || property.Equals("DrVisitPers"))
            {
                result = ListOptions.GetTimes().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("ProductivityLoss"))
            {
                result = ListOptions.GetHealthProblems().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("TimeZoneId"))
            {
                result = CommonUtility.GetTimeZones(null).TimeZones.Select(x => new SelectListItem { Text = Translate.Message(x.TimeZoneDisplay), Value = x.Id.ToString() }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("SmokeHist"))
            {
                result = ListOptions.SmokingDetails().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("Unit"))
            {
                result = ListOptions.GetUnits().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("Country"))
            {
                result = CommonUtility.ListCountries().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("BPArm"))
            {
                result = ListOptions.ArmUsed().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).Where(x => x.Value == value).FirstOrDefault().Text;
                return result;
            }
            if (property.Equals("Gender"))
            {
                result = ListOptions.GetGenderList(null).Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).Where(x => x.Value == value).FirstOrDefault().Text;
                return result;
            }
            return result;
        }

        [ModuleControl(null, RoleCode.Administrator)]
        [HttpPost]
        public async Task<JsonResult> UploadLabFile(IFormFile FileUpload, int orgId, int filetype, int unit)
        {
            string filerror = "";
            var count = 0;
            bool error = false;
            Dictionary<string, string> uploaderror = new Dictionary<string, string>();
            List<LabResponse> labResponse = new List<LabResponse>();
            var portal = PortalUtility.ReadOrganization("", orgId).organization.Portals.Where(x => x.Active == true).FirstOrDefault();
            if (FileUpload != null)
            {
                if (filetype == 1)
                {
                    string hl7Value = "";
                    using (var memoryStream = new MemoryStream())
                    {
                        await FileUpload.CopyToAsync(memoryStream);
                        byte[] fileBytes = memoryStream.ToArray();
                    }
                    using (var memoryStream = new MemoryStream())
                    {
                        await FileUpload.CopyToAsync(memoryStream);
                        byte[] binData = memoryStream.ToArray();
                        hl7Value = System.Text.Encoding.UTF8.GetString(binData);
                    }
                    labResponse = Labcorp.LoadHL7File(hl7Value, "Quest");
                }
                else
                {
                    labResponse = LoadExcelLabData(FileUpload, unit);
                }
                foreach (var lab in labResponse)
                {
                    var eligibility = ParticipantUtility.GetEligibility(null, lab.UniqueId, portal.Id);
                    #region Eligibility
                    if (eligibility.Eligibility != null)
                    {
                        var registered = AccountUtility.GetUserByUniqueId(orgId, eligibility.Eligibility.UniqueId);
                        int? userId, hraId;
                        userId = hraId = null;
                        if (registered.User == null)
                        {
                            #region User Registration
                            AccountController controller = new AccountController(_userManager, iOptionAppSettings, environment);
                            var email = eligibility.Eligibility.Email;
                            if (string.IsNullOrEmpty(eligibility.Eligibility.Email))
                            {
                                email = eligibility.Eligibility.FirstName + eligibility.Eligibility.LastName + "@noemail.myintervent.com";
                            }
                            var register = await controller.CreateAccount(eligibility.Eligibility, email, CommonUtility.GeneratePassword(10, 3), false, false, false);
                            if (register.Succeeded)
                            {
                                userId = register.userId;
                            }
                            else
                            {
                                uploaderror.Add(eligibility.Eligibility.UniqueId, "User could not be created because " + register.error);
                                continue;
                            }
                            #endregion
                        }
                        else
                        {
                            var user = ParticipantUtility.ReadParticipantInfo(registered.User.Id, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
                            userId = user.user.Id;
                            if (user.hra != null)
                            {
                                hraId = user.hra.Id;
                            }
                        }
                        if (portal.HasHRA.Value == (int)HRAStatus.Yes && !hraId.HasValue)
                        {
                            hraId = HRAUtility.CreateHRA(userId.Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, _appSettings.SystemAdminId, HttpContext.Session.GetInt32(SessionContext.UserinProgramId), portal.Id, true);
                        }
                        PostLabData(lab, userId.Value, portal.Id, portal.HRAValidity);
                        count++;
                    }
                    else
                    {
                        uploaderror.Add(lab.UniqueId, "The unique id could not be found.");
                    }
                    #endregion
                }

            }
            
            if (uploaderror.Count > 0)
            {
                LogReader reader = new LogReader();
                reader.AddScreeningData(uploaderror, orgId);
                error = true;
            }
            return Json(new { Count = count, Error = filerror, Status = error });

        }

        [ModuleControl(null, RoleCode.Administrator)]
        public List<LabResponse> LoadExcelLabData(IFormFile FileUpload, int unit)
        {
            List<LabResponse> labResponse = new List<LabResponse>();
            string filename = FileUpload.FileName;
            if (filename.EndsWith(".xlsx"))
            {
                string targetpath = environment.ContentRootPath + "~/temp/";
                if (!Directory.Exists(targetpath))
                    Directory.CreateDirectory(targetpath);
                string pathToExcelFile = Path.Combine(targetpath, filename);
                using (var fileStream = new FileStream(pathToExcelFile, FileMode.Create))
                {
                    FileUpload.CopyToAsync(fileStream);
                }
                string con = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=yes;IMEX=1'", pathToExcelFile);
                using (OleDbConnection connection = new OleDbConnection(con))
                {
                    connection.Open();
                    var tables = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (tables == null) return labResponse;
                    var sheet = tables.Rows[0]["TABLE_NAME"];
                    OleDbCommand command = new OleDbCommand("select * from [" + sheet + "]", connection);
                    using (OleDbDataReader dr = command.ExecuteReader())
                    {
                        var schema = dr.GetSchemaTable();
                        Dictionary<int, string> columns = new Dictionary<int, string>();
                        for (int i = 0; i < schema.Rows.Count; i++)
                        {
                            columns.Add(i, schema.Rows[i].ItemArray[0].ToString().ToLower());
                        }
                        while (dr.Read())
                        {
                            LabResponse lab = new LabResponse();
                            #region Excel Conversion
                            if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "uniqueid").Key].ToString()))
                                lab.UniqueId = dr[columns.FirstOrDefault(x => x.Value == "uniqueid").Key].ToString();
                            if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "bparm").Key].ToString()))
                                lab.BPArm = Byte.Parse(dr[columns.FirstOrDefault(x => x.Value == "bparm").Key].ToString());
                            if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "biotestdate").Key].ToString()))
                                lab.BloodTestDate = DateTime.Parse(dr[columns.FirstOrDefault(x => x.Value == "biotestdate").Key].ToString());
                            if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "sbp").Key].ToString()))
                                lab.SBP = Convert.ToInt16(dr[columns.FirstOrDefault(x => x.Value == "sbp").Key].ToString());
                            if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "dbp").Key].ToString()))
                                lab.DBP = Convert.ToInt16(dr[columns.FirstOrDefault(x => x.Value == "dbp").Key].ToString());
                            if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "fasting").Key].ToString()))
                                lab.DidYouFast = Byte.Parse(dr[columns.FirstOrDefault(x => x.Value == "fasting").Key].ToString());
                            if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "totchol").Key].ToString()))
                                lab.TotalChol = float.Parse(dr[columns.FirstOrDefault(x => x.Value == "totchol").Key].ToString());
                            if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "trig").Key].ToString()))
                                lab.Trig = float.Parse(dr[columns.FirstOrDefault(x => x.Value == "trig").Key].ToString());
                            if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "ldl").Key].ToString()))
                                lab.LDL = float.Parse(dr[columns.FirstOrDefault(x => x.Value == "ldl").Key].ToString());
                            if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "hdl").Key].ToString()))
                                lab.HDL = float.Parse(dr[columns.FirstOrDefault(x => x.Value == "hdl").Key].ToString());
                            if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "gluc").Key].ToString()))
                                lab.Glucose = float.Parse(dr[columns.FirstOrDefault(x => x.Value == "gluc").Key].ToString());
                            if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "hba1c").Key].ToString()))
                                lab.A1C = float.Parse(dr[columns.FirstOrDefault(x => x.Value == "hba1c").Key].ToString());
                            if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "height").Key].ToString()))
                            {
                                if (unit == (int)Unit.Metric)
                                    lab.HeightCM = float.Parse(dr[columns.FirstOrDefault(x => x.Value == "height").Key].ToString());
                                else
                                    lab.Height = float.Parse(dr[columns.FirstOrDefault(x => x.Value == "height").Key].ToString());
                            }
                            if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "weight").Key].ToString()))
                                lab.Weight = float.Parse(dr[columns.FirstOrDefault(x => x.Value == "weight").Key].ToString());
                            if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "waist").Key].ToString()))
                                lab.Waist = float.Parse(dr[columns.FirstOrDefault(x => x.Value == "waist").Key].ToString());
                            if (unit == (int)Unit.Metric)
                                ListOptions.CovertIntoImperial(lab, CommonUtility.MeasurementRange());
                            labResponse.Add(lab);
                            #endregion
                        }
                    }
                    connection.Close();
                    System.IO.File.Delete(pathToExcelFile);
                }
            }
            return labResponse;
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public void PostLabData(LabResponse model, int userId, int portalId, int? HRAValidity)
        {
            HRA_HealthNumbers healthNumbers = new HRA_HealthNumbers();
            AddLabModel labModel = new AddLabModel();
            LabDto lab = new LabDto();
            lab.TotalChol = model.TotalChol;
            lab.BPArm = model.BPArm;
            lab.A1C = model.A1C;
            lab.DBP = model.DBP;
            lab.SBP = model.SBP;
            lab.Glucose = model.Glucose;
            lab.Trig = model.Trig;
            lab.Height = model.Height;
            lab.Waist = model.Waist;
            lab.Weight = model.Weight;
            lab.HDL = model.HDL;
            lab.LDL = model.LDL;
            lab.BMI = model.BMI;
            lab.DidYouFast = model.DidYouFast;
            lab.BloodTestDate = model.BloodTestDate;
            lab.HighCotinine = model.HighCotinine;
            lab.UserId = userId;
            lab.PortalId = portalId;
            lab.LabSelection = 2;
            labModel.Lab = lab;
            labModel.SaveNew = true;
            labModel.HRAValidity = HRAValidity;
            if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                labModel.updatedBy = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            if (HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue)
                labModel.participantPortalId = HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value;
            if (HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue)
                labModel.participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            labModel.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            LabUtility.UpdateLabs(_appSettings.SystemAdminId, labModel);
        }
    }
}