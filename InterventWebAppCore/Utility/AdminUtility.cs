using Intervent.DAL;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class AdminUtility
    {
        public static ReadAdminResponse ReadAdmin(int userId)
        {
            AdminReader reader = new AdminReader();
            ReadAdminRequest request = new ReadAdminRequest();
            request.userId = userId;
            return reader.ReadUserAdmin(request);
        }

        public static async Task<UpdateUserResponse> UpdateAdminProfile(UserManager<ApplicationUser> userManager, AdminProfileModel model, int userId, bool adminAccess, string roles = null,
            string specialities = null, string languages = null, string coachStates = null, string profileLanguage = null)
        {
            UpdateUserRequest request = new UpdateUserRequest();
            request.UpdatedByUserId = userId;
            UserDto user = new UserDto();
            user.Id = model.user.Id;
            user.NamePrefix = model.user.NamePrefix;
            user.FirstName = model.user.FirstName;
            user.LastName = model.user.LastName;
            user.MiddleName = model.user.MiddleName;
            user.Suffix = model.user.Suffix;
            user.DOB = new DateTime(model.year, model.month, model.day);
            user.Gender = model.user.Gender;
            user.Address = model.user.Address;
            user.Address2 = model.user.Address2;
            user.City = model.user.City;
            user.State = model.user.State;
            user.Zip = model.user.Zip;
            user.Country = model.user.Country;
            user.HomeNumber = model.user.HomeNumber;
            user.WorkNumber = model.user.WorkNumber;
            user.CellNumber = model.user.CellNumber;
            user.Email = model.user.Email;
            user.Occupation = model.user.Occupation;
            user.OrganizationId = model.user.OrganizationId;
            user.TimeZoneId = model.user.TimeZoneId;
            user.Unit = model.user.Unit;
            user.IsActive = model.user.IsActive;
            user.InactiveReason = model.user.InactiveReason;
            request.user = user;
            request.FromAdmin = true;

            AccountReader reader = new AccountReader(userManager);
            var response = await reader.UpdateUser(request);
            if (response.Succeeded && adminAccess)
            {
                //create role
                AddUserToRoleRequest roleRequest = new AddUserToRoleRequest();
                roleRequest.newRole = roles;
                roleRequest.currentRole = model.CurrentRole;
                roleRequest.userId = model.user.Id;
                var roleResponse = await reader.AddUserToRole(roleRequest);
                //create admin properties
                AddEditAdminPropRequest propRequest = new AddEditAdminPropRequest();
                AdminPropertyDto adminDto = new AdminPropertyDto();
                adminDto.Id = model.user.Id;
                if (profileLanguage == ListOptions.DefaultLanguage)
                    adminDto.Profile = model.user.AdminProperty.Profile;
                else
                    adminDto.ProfileLanguageItem = LanguageType.PRO.ToString() + model.user.Id;
                adminDto.Video = model.user.AdminProperty.Video;
                adminDto.MeetingId = model.user.AdminProperty.Video.Value ? model.user.AdminProperty.MeetingId : "";
                adminDto.AllowAppt = model.user.AdminProperty.AllowAppt;
                propRequest.AdminProperty = adminDto;
                var propResponse = reader.AddEditAdminProperty(propRequest);
                if (profileLanguage != ListOptions.DefaultLanguage)
                {
                    LanguageReader langReader = new LanguageReader();
                    Dictionary<string, string> codes = new Dictionary<string, string>();
                    codes.Add(adminDto.ProfileLanguageItem, model.user.AdminProperty.Profile);
                    langReader.SaveLanguageItems(codes, profileLanguage);
                }
                //add coach specialities
                AddCoachSpecializationRequest specializationRequest = new AddCoachSpecializationRequest();
                specializationRequest.userId = model.user.Id;
                specializationRequest.specializations = specialities;
                reader.AddEditSpecializations(specializationRequest);
                //add coach languages
                AddCoachLanguageRequest languageRequest = new AddCoachLanguageRequest();
                languageRequest.userId = model.user.Id;
                languageRequest.languages = languages;
                reader.AddEditUserLanguages(languageRequest);
                //add coach states
                AddCoachStateRequest stateRequest = new AddCoachStateRequest();
                stateRequest.userId = model.user.Id;
                stateRequest.states = coachStates;
                reader.AddEditCoachStates(stateRequest);
            }
            if (response.primaryFieldsChanged)
            {
                GetAllHRAsforUserRequest hraRequest = new GetAllHRAsforUserRequest();
                hraRequest.UserId = user.Id;
                HRAReader hraReader = new HRAReader();
                var hraResponse = hraReader.GetAllHRAsforUser(hraRequest);
                if (hraResponse != null && hraResponse.HRAIds != null && hraResponse.HRAIds.Count > 0)
                {
                    for (int i = 0; i < hraResponse.HRAIds.Count; i++)
                    {
                        hraReader.StratifyHRA(hraResponse.HRAIds[i]);
                    }
                }
            }
            if (!string.IsNullOrEmpty(model.existingEmail))
            {
                string email = "";
                if (!model.user.Email.Contains("noemail.myintervent.com") && !model.user.Email.Contains("samlnoemail.com"))
                    email = model.user.Email + ",";
                if (!model.existingEmail.Contains("noemail.myintervent.com") && !model.user.Email.Contains("samlnoemail.com"))
                    email = email + model.existingEmail;
                if (!string.IsNullOrEmpty(email))
                    NotificationUtility.CreateChangePasswordNotificationEvent(NotificationEventTypeDto.ChangeEmail, request.user.Id, email, true);
            }
            return response;
        }

        public static AdminDashboardResponse GetAdminDashboardDetails()
        {
            AdminReader reader = new AdminReader();
            return reader.GetAdminDashboardDetails();
        }

        public static ListUsersResponse ListUsers(string name, int userId, bool? usersWithRole)
        {
            AdminReader reader = new AdminReader();
            ListUsersRequest request = new ListUsersRequest();
            request.UserId = userId;
            request.name = name;
            if (usersWithRole.HasValue)
                request.usersWithRole = usersWithRole.Value;
            return reader.ListUsers(request);
        }
        public static ListTaskTypeResponse ListTaskType(int adminId)
        {
            AdminReader reader = new AdminReader();
            ListTaskTypeRequest request = new ListTaskTypeRequest();
            bool IsLMCCoach = false;
            var Adminresponse = ReadAdmin(adminId).admin;
            if (Adminresponse.user.Roles.Count != 0)
            {
                var UserRoles = Adminresponse.user.Roles.Select(x => x.Name).ToList();
                IsLMCCoach = UserRoles.Contains(Constants.LMCCoach) ? true : false;
            }
            request.IsLMCCoach = IsLMCCoach;
            return reader.ListTaskType(request);
        }

        public static GetTaskListResponse GetTaskList(DateTime? startDate, DateTime? endDate, int[] taskTypeId, int? ownerId, string status, int page, int pageSize, int? totalRecords, string Organizations, bool download, int userId, string timeZone)
        {
            AdminReader reader = new AdminReader();
            GetTaskListRequest request = new GetTaskListRequest();
            request.startDate = startDate;
            request.endDate = endDate;
            request.taskTypeId = taskTypeId;
            request.Organizations = Organizations;
            if (ownerId.HasValue)
                request.ownerId = ownerId.Value;
            request.status = status;
            request.timezone = timeZone;
            if (download)
            {
                request.page = 1;
                request.pageSize = 0;
                request.totalRecords = 0;
            }
            else
            {
                request.page = page;
                request.pageSize = pageSize;
                request.totalRecords = totalRecords;
            }
            request.userId = userId;
            return reader.GetTaskList(request);
        }

        public static ReadTaskResponse ReadTask(int? taskId, int? userId, int? taskType)
        {
            AdminReader reader = new AdminReader();
            ReadTaskRequest request = new ReadTaskRequest();
            request.taskId = taskId;
            request.userId = userId;
            request.taskType = taskType;
            var response = reader.ReadTask(request);
            return response;
        }

        public static AddEditTaskResponse AddEditTask(int? id, int taskTypeId, string status, int? user, int owner, string comment, bool isActive = true)
        {
            AdminReader reader = new AdminReader();
            AddEditTaskRequest request = new AddEditTaskRequest();
            TasksDto task = new TasksDto();
            if (id.HasValue)
                task.Id = id.Value;
            task.TaskTypeId = taskTypeId;
            task.Status = status;
            if (user.HasValue)
                task.UserId = user.Value;
            task.Owner = owner;
            task.Comment = comment;
            task.UpdatedBy = task.CreatedBy = user.Value;
            task.IsActive = isActive;
            request.task = task;
            var response = reader.AddEditTask(request);
            return response;
        }

        public static AddEditTaskResponse AdminTaskStausBulkUpdate(string taskids, string status, int userId)
        {
            AdminReader reader = new AdminReader();
            AdminTaskStausBulkUpdateRequest request = new AdminTaskStausBulkUpdateRequest();
            request.taskids = taskids;
            request.status = status;
            request.userId = userId;
            var response = reader.AdminTaskStausBulkUpdate(request);
            return response;
        }


        public static SelectList TaskStatusList()
        {
            List<KeyValue> task = new List<KeyValue>();
            task.Add(new KeyValue() { Value = "N", Text = Translate.Message("L698") });
            task.Add(new KeyValue() { Value = "I", Text = Translate.Message("L699") });
            task.Add(new KeyValue() { Value = "C", Text = Translate.Message("L7") });
            SelectList tasks = new SelectList(task, "Value", "Text");

            return tasks;
        }

        public static string GetProfileText(int id, string language)
        {
            AdminReader reader = new AdminReader();
            return reader.GetProfileText(id, language);
        }

        public static SelectList RoleCodeList()
        {
            List<KeyValue> task = new List<KeyValue>();
            task.Add(new KeyValue() { Value = "", Text = "Select RoleCode" });
            task.Add(new KeyValue() { Value = RoleCode.Administrator, Text = RoleCode.Administrator });
            task.Add(new KeyValue() { Value = RoleCode.Coach, Text = RoleCode.Coach });
            task.Add(new KeyValue() { Value = RoleCode.CSR, Text = RoleCode.CSR });
            SelectList tasks = new SelectList(task, "Value", "Text");

            return tasks;
        }

        public static IList<AdminModuleDto> ListAdminModule()
        {
            AdminReader reader = new AdminReader();
            return reader.ListAdminModule();
        }

        public static ListParticipantHistorySearchResponse SearchParticipantHistory(int participantId, int? userHistoryCategoryId, DateTime? startDate, DateTime? endDate, int page, int pageSize, int? totalRecords)
        {
            ListParticipantHistorySearchRequest request = new ListParticipantHistorySearchRequest();
            request.UserHistoryCategoryId = userHistoryCategoryId;
            request.UserId = participantId;
            request.StartDate = startDate;
            if (endDate.HasValue)
                request.EndDate = endDate.Value.AddDays(1);
            request.Page = page;
            request.PageSize = pageSize;
            request.TotalRecords = totalRecords;
            AdminReader reader = new AdminReader();
            return reader.ListParticpantHistorySearch(request);
        }

        public static GetParticipantHistoryResponse GetParticipantHistory(int id)
        {
            return new AdminReader().GetParticipantHistory(new GetParticipantHistoryRequest { id = id });
        }
    }
}