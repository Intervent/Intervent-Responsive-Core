using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class AdminReader
    {
        InterventDatabase dbcontext = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        InterventDatabase GetContext()
        {
            return new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());
        }

        public ReadAdminResponse ReadUserAdmin(ReadAdminRequest request)
        {
            ReadAdminResponse response = new ReadAdminResponse();
            var user = dbcontext.Users.Include("AdminProperty").Include("UserRoles").Include("UserRoles.AdminModules").Include("Country1")
                .Where(x => x.Id == request.userId).FirstOrDefault();
            AdminDto admin = new AdminDto();
            admin.user = Utility.mapper.Map<DAL.User, UserDto>(user);
            response.admin = admin;
            return response;
        }

        public ListUsersResponse ListUsers(ListUsersRequest request)
        {
            ListUsersResponse response = new ListUsersResponse();
            PortalReader reader = new PortalReader();
            var organizationsList = reader.GetFilteredOrganizationsList(request.UserId).Organizations.Select(x => x.Id).ToArray();
            var users = dbcontext.Users.Include("Organization").Include("UserRoles").Where(x => (x.IsActive != false || (x.InactiveReason != 3 && x.InactiveReason != 5 && x.InactiveReason != 6)) && (organizationsList.Count() != 0 && organizationsList.Contains(x.OrganizationId)) &&
                (x.FirstName + " " + x.LastName).ToLower().Contains(request.name.ToLower()) && (!request.usersWithRole || x.UserRoles.Count() > 0)).ToList();
            if (users != null)
            {
                List<UserDto> usersDto = new List<UserDto>();
                for (int i = 0; i < users.Count; i++)
                {
                    UserDto userDto = new UserDto();
                    userDto.Id = users[i].Id;
                    userDto.UserName = users[i].UserName;
                    userDto.Email = users[i].Email;
                    userDto.PhoneNumber = users[i].PhoneNumber;
                    userDto.NamePrefix = users[i].NamePrefix;
                    userDto.FirstName = users[i].FirstName;
                    userDto.LastName = users[i].LastName;
                    userDto.DOB = users[i].DOB;
                    userDto.Gender = users[i].Gender;
                    userDto.Race = users[i].Race;
                    userDto.RaceOther = users[i].RaceOther;
                    userDto.Address = users[i].Address;
                    userDto.Address2 = users[i].Address2;
                    userDto.City = users[i].City;
                    userDto.State = users[i].State;
                    userDto.Country = users[i].Country;
                    userDto.Zip = users[i].Zip;
                    userDto.HomeNumber = users[i].HomeNumber;
                    userDto.WorkNumber = users[i].WorkNumber;
                    userDto.CellNumber = users[i].CellNumber;
                    userDto.TimeZoneId = users[i].TimeZoneId;
                    userDto.PreferredContactTimeId = users[i].PreferredContactTimeId;
                    userDto.ProfessionId = users[i].ProfessionId;
                    userDto.OrganizationId = users[i].OrganizationId;
                    userDto.Occupation = users[i].Occupation;
                    userDto.Source = users[i].Source;
                    userDto.SourceOther = users[i].SourceOther;
                    userDto.Text = users[i].Text;
                    userDto.Picture = users[i].Picture;
                    userDto.Complete = users[i].Complete;
                    userDto.ContactMode = users[i].ContactMode;
                    userDto.Organization = Utility.mapper.Map<DAL.Organization, OrganizationDto>(users[i].Organization);
                    usersDto.Add(userDto);
                }
                response.Users = usersDto;
            }

            return response;
        }

        public ListTaskTypeResponse ListTaskType(ListTaskTypeRequest request)
        {
            ListTaskTypeResponse response = new ListTaskTypeResponse();
            List<int> LMCTaskTypes = CommonReader.GetLMCTaskTypes();
            var taskTypes = dbcontext.TaskTypes.Where(x => x.IsActive.Equals(true) && (request.IsLMCCoach == false || LMCTaskTypes.Contains(x.Id))).ToList();
            response.taskTypes = Utility.mapper.Map<IList<DAL.TaskType>, IList<TaskTypesDto>>(taskTypes);
            return response;
        }

        public List<DAL.TaskType> TaskTypes()
        {
            return dbcontext.TaskTypes.Where(x => x.IsActive.Equals(true)).ToList();
        }

        public GetTaskListResponse GetTaskList(GetTaskListRequest request)
        {
            GetTaskListResponse response = new GetTaskListResponse();
            bool download = false;
            var taskId = request.taskTypeId[0];
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.timezone);
            var startdate = request.startDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.startDate.Value, custTZone) : System.DateTime.MinValue;
            var enddate = request.endDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.endDate.Value, custTZone).AddDays(1) : System.DateTime.MaxValue;
            var totalRecords = request.totalRecords.HasValue ? request.totalRecords.Value : 0;
            var organizationsList = new List<int?>();
            if (request.ownerId == null)
            {
                PortalReader reader = new PortalReader();
                if (String.IsNullOrEmpty(request.Organizations))
                {
                    organizationsList = reader.GetFilteredOrganizationsList(request.userId).Organizations.Select(x => x.Id).ToList();
                }
                else
                {
                    int?[] OrganizatinLst = request.Organizations.Split(',').Select(str => (int?)int.Parse(str)).ToArray();
                    organizationsList.AddRange(OrganizatinLst);
                }
            }
            if (totalRecords == 0)
            {
                totalRecords = dbcontext.AdminTasks.Include("User").Include("User.State1").Include("User.Organization").Include("User1").Include("TaskType")
                .Where(x => (x.CreatedOn >= startdate) && (x.CreatedOn <= enddate) && (taskId == 0 || request.taskTypeId.Contains(x.TaskTypeId))
                && (string.IsNullOrEmpty(request.status) || x.Status == request.status) && (x.Owner == request.ownerId || organizationsList.Contains(x.User.OrganizationId)) && x.IsActive).Count();
                if (request.pageSize == 0)
                {
                    request.pageSize = totalRecords;
                    download = true;
                }
            }
            var tasks = new List<AdminTask>();
            tasks = dbcontext.AdminTasks.Include("User").Include("User.State1").Include("User.Organization").Include("User1").Include("TaskType")
            .Where(x => (x.CreatedOn >= startdate) && (x.CreatedOn <= enddate) && (taskId == 0 || request.taskTypeId.Contains(x.TaskTypeId))
            && (string.IsNullOrEmpty(request.status) || x.Status == request.status) && (x.Owner == request.ownerId || organizationsList.Contains(x.User.OrganizationId)) && x.IsActive).OrderByDescending(x => x.Id).Skip(download ? 0 : request.page * request.pageSize).Take(request.pageSize).ToList();
            response.tasks = Utility.mapper.Map<IList<DAL.AdminTask>, IList<TasksDto>>(tasks);
            response.totalRecords = totalRecords;
            return response;
        }

        public AdminDashboardResponse GetAdminDashboardDetails()
        {
            AdminDashboardResponse response = new AdminDashboardResponse();
            DateTime duration = DateTime.UtcNow.AddMonths(-3);
            response.ProgramsCount = dbcontext.Programs.Where(x => x.Active).Count();
            response.OrganizationsCount = dbcontext.Organizations.Where(x => x.Active).Count();
            response.NewClients = dbcontext.Users.Where(x => x.CreatedOn > duration && x.IsActive).Count();
            response.ActiveClients = dbcontext.Users.Where(x => x.IsActive).Count();
            response.Coaches = dbcontext.Users.Include("UserRoles").Where(x => x.UserRoles.Where(r => r.Code == "COACH" || r.Code == "CSR").Count() > 0).Count();
            return response;
        }

        public ReadTaskResponse ReadTask(ReadTaskRequest request)
        {
            ReadTaskResponse response = new ReadTaskResponse();
            var task = dbcontext.AdminTasks.Include("User").Include("User1").Include("TaskType")
                .Where(x => (request.taskId.HasValue && x.Id == request.taskId.Value) ||
                    (request.userId.HasValue && request.taskType.HasValue && x.UserId == request.userId.Value && x.TaskTypeId == request.taskType && x.Status == "N")).FirstOrDefault();
            response.task = Utility.mapper.Map<DAL.AdminTask, TasksDto>(task);
            return response;
        }

        public AddEditTaskResponse AddEditTask(AddEditTaskRequest request)
        {
            AddEditTaskResponse response = new AddEditTaskResponse();
            AdminTask task = null;
            if (request.task.Id > 0)
                task = dbcontext.AdminTasks.Where(x => x.Id == request.task.Id).FirstOrDefault();
            else if (request.task.TaskTypeId == (int)DTO.TaskTypes.Know_Your_Numbers || request.task.TaskTypeId == (int)DTO.TaskTypes.CDPP_Doctor_Choice ||
                request.task.TaskTypeId == (int)DTO.TaskTypes.CDPP_Lab_Choice || request.task.TaskTypeId == (int)DTO.TaskTypes.CDPP_Doctor_Labs)
            {
                task = dbcontext.AdminTasks.Where(x => x.TaskTypeId == request.task.TaskTypeId && x.UserId == request.task.UserId && x.IsActive == true && (x.Status == DTO.TaskStatus.N.ToString() || x.Status == DTO.TaskStatus.A.ToString())).OrderByDescending(x => x.Id).FirstOrDefault();
                if (task == null && request.task.Status == null)
                {
                    return null;
                }
            }
            if (task != null)
            {
                task.TaskTypeId = request.task.TaskTypeId;
                if (request.task.Status != null)
                    task.Status = request.task.Status;
                task.Owner = request.task.Owner;
                task.Comment = request.task.Comment;
                task.UpdatedOn = System.DateTime.UtcNow;
                task.UpdatedBy = request.task.UpdatedBy;
                task.IsActive = request.task.IsActive;
                dbcontext.AdminTasks.Attach(task);
                dbcontext.Entry(task).State = EntityState.Modified;
            }
            else
            {
                DAL.AdminTask newAdminTask = new DAL.AdminTask();
                newAdminTask.TaskTypeId = request.task.TaskTypeId;
                newAdminTask.Status = request.task.Status;
                newAdminTask.Owner = request.task.Owner;
                newAdminTask.Comment = request.task.Comment;
                newAdminTask.CreatedOn = System.DateTime.UtcNow;
                newAdminTask.CreatedBy = request.task.CreatedBy;
                newAdminTask.IsActive = true;
                newAdminTask.UserId = request.task.UserId;
                dbcontext.AdminTasks.Add(newAdminTask);
            }
            dbcontext.SaveChanges();
            response.success = true;
            return response;
        }

        public AddEditTaskResponse AdminTaskStausBulkUpdate(AdminTaskStausBulkUpdateRequest request)
        {
            AddEditTaskResponse response = new AddEditTaskResponse();
            int[] TaskIdList = request.taskids.Split(',').Select(str => int.Parse(str)).ToArray();
            foreach (int Id in TaskIdList)
            {
                var adminTask = dbcontext.AdminTasks.Where(x => x.Id == Id).FirstOrDefault();
                adminTask.Status = request.status;
                adminTask.UpdatedOn = System.DateTime.UtcNow;
                adminTask.UpdatedBy = request.userId;
                dbcontext.AdminTasks.Attach(adminTask);
                dbcontext.Entry(adminTask).State = EntityState.Modified;
            }
            dbcontext.SaveChanges();
            response.success = true;
            return response;
        }

        public string GetProfileText(int id, string language)
        {
            string profileText = string.Empty;
            var admin = dbcontext.AdminProperties.Where(x => x.Id == id).FirstOrDefault();
            if (language != ListOptions.DefaultLanguage)
            {
                LanguageReader langReader = new LanguageReader();
                var profile = langReader.GetLanguageItem(LanguageType.PRO, admin.Id, language);
                if (profile != null)
                    profileText = profile.Text;
            }
            else
                profileText = admin.Profile;
            return profileText;
        }

        public IList<AdminModuleDto> ListAdminModule()
        {
            var adminModules = dbcontext.AdminModules.ToList();
            return Utility.mapper.Map<IList<DAL.AdminModule>, IList<AdminModuleDto>>(adminModules);
        }

        public ListParticipantHistorySearchResponse ListParticpantHistorySearch(ListParticipantHistorySearchRequest request)
        {
            ListParticipantHistorySearchResponse response = new ListParticipantHistorySearchResponse();
            if (request.UserId < 1)
            {
                response.UserChanges = Enumerable.Empty<UserHistoryDto>();
                return response;
            }

            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                using (InterventDatabase ctx = GetContext())
                {
                    totalRecords = dbcontext.UserHistories.Where(x => x.UserId == request.UserId && (!request.UserHistoryCategoryId.HasValue || request.UserHistoryCategoryId.Value == x.UserHistoryCategoryId) && (!request.StartDate.HasValue || x.LogDate >= request.StartDate.Value) && (!request.EndDate.HasValue || x.LogDate <= request.EndDate.Value)).Count();
                }
            }
            using (InterventDatabase ctx = GetContext())
            {
                response.UserChanges = dbcontext.UserHistories
                .Where(x => x.UserId == request.UserId && (!request.UserHistoryCategoryId.HasValue || request.UserHistoryCategoryId.Value == x.UserHistoryCategoryId) && (!request.StartDate.HasValue || x.LogDate >= request.StartDate.Value) && (!request.EndDate.HasValue || x.LogDate <= request.EndDate.Value))
                .OrderByDescending(x => x.Id).Skip(request.Page * request.PageSize).Take(request.PageSize)
                .Select(x => new UserHistoryDto()
                {
                    Id = x.Id,
                    LogDate = x.LogDate,
                    UserHistoryCategoryId = x.UserHistoryCategoryId
                }).ToList();
            }
            response.TotalRecords = totalRecords;
            return response;
        }

        public GetParticipantHistoryResponse GetParticipantHistory(GetParticipantHistoryRequest request)
        {
            GetParticipantHistoryResponse response = new GetParticipantHistoryResponse();
            using (InterventDatabase ctx = GetContext())
            {
                response.UserChange = dbcontext.UserHistories
                .Where(x => x.Id == request.id)
                .Select(x => new UserHistoryDto()
                {
                    Id = x.Id,
                    LogDate = x.LogDate,
                    UserHistoryCategoryId = x.UserHistoryCategoryId,
                    Changes = x.Changes,
                    UpdatedByName = x.User1.FirstName + " " + x.User1.LastName

                }).FirstOrDefault();
            }
            return response;
        }

        public void CreateTaskforMissedAppt(int systemAdminId)
        {
            GetNotesResponse response = new GetNotesResponse();
            var notes = dbcontext.Notes.Include("Portal").Include("User").Include("User.Appointments").Include("User.AdminTasks").Include("User.UsersinPrograms").Include("User.UserTrackingStatuses")
                .Where(x => x.Type == (int)NoteTypes.Tracking && x.User.Appointments.Where(y => y.Date > DateTime.UtcNow && y.Active == true).Count() == 0 &&
                x.User.AdminTasks.Where(z => z.TaskTypeId == (int)Intervent.Web.DTO.TaskTypes.Send_Postcard && z.IsActive == true).Count() == 0 &&
                x.Portal.Active == true && x.User.IsActive == true && x.NotesDate >= x.Portal.StartDate && x.NotesDate <= x.Portal.EndDate &&
                x.User.UsersinPrograms.Where(p => p.IsActive == true).Count() > 0 && x.NotesDate > x.User.UsersinPrograms.Where(p => p.IsActive == true).FirstOrDefault().StartDate &&
                x.User.UserTrackingStatuses.Where(u => u.DoNotTrack == true && u.PortalId == x.PortalId).Count() == 0)
                .GroupBy(x => new { x.userId, x.PortalId })
                .Where(x => x.Count() >= 5)
                .Select(x => x.Key.userId).ToList();
            foreach (var user in notes)
            {
                DAL.AdminTask newAdminTask = new DAL.AdminTask();
                newAdminTask.TaskTypeId = (int)Intervent.Web.DTO.TaskTypes.Send_Postcard;
                newAdminTask.Status = "N";
                newAdminTask.Owner = systemAdminId;
                newAdminTask.Comment = "Send postcard";
                newAdminTask.CreatedOn = System.DateTime.UtcNow;
                newAdminTask.CreatedBy = systemAdminId;
                newAdminTask.IsActive = true;
                newAdminTask.UserId = user;
                dbcontext.AdminTasks.Add(newAdminTask);
            }
            dbcontext.SaveChanges();
        }

        public ListNewsletterResponse ListNewsletters(GetNewsletterListRequest request)
        {
            ListNewsletterResponse response = new ListNewsletterResponse();
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = dbcontext.Newsletters.Count();

            }
            if (request.PageSize == 0)
            {
                request.PageSize = totalRecords;
            }
            var newsletters = new List<Newsletter>();
            newsletters = dbcontext.Newsletters.OrderByDescending(x => x.Id).Skip(request.Page * request.PageSize).Take(request.PageSize).ToList();
            response.Newsletters = Utility.mapper.Map<IList<DAL.Newsletter>, IList<NewsletterDto>>(newsletters);
            response.TotalRecords = totalRecords;
            return response;
        }

        public IList<AssignedNewsletterDto> ListAssignedNewsletter(int newsletterId)
        {
            var assignedNewsletter = dbcontext.AssignedNewsletters.Include("Organization").OrderBy(x => x.Date).Where(x => x.NewsletterId == newsletterId).ToList();
            return Utility.mapper.Map<IList<DAL.AssignedNewsletter>, IList<AssignedNewsletterDto>>(assignedNewsletter);
        }
    }
}