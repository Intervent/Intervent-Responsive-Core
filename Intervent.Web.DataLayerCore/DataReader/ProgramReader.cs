using Intervent.DAL;
using Intervent.Framework.Clone;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class ProgramReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public ListProgramsResponse ListPrograms(ListProgramsRequest request)
        {
            ListProgramsResponse response = new ListProgramsResponse();
            var totalRecords = request.totalRecords.HasValue ? request.totalRecords.Value : 0;
            List<Program> programs = new List<Program>();
            if (totalRecords == 0)
            {
                totalRecords = context.Programs.Include("KitsinPrograms").Where(x => (!request.onlyActive.HasValue) || (x.Active == request.onlyActive)).Count();
            }
            if (!request.page.HasValue && !request.pageSize.HasValue)
                programs = context.Programs.Include("KitsinPrograms").Where(x => (!request.onlyActive.HasValue) || (x.Active == request.onlyActive)).ToList();
            else
                programs = context.Programs.Include("KitsinPrograms").Where(x => (!request.onlyActive.HasValue) || (x.Active == request.onlyActive)).OrderBy(x => x.Name).Skip(request.page.Value * request.pageSize.Value).Take(request.pageSize.Value).ToList();
            response.Programs = Utility.mapper.Map<IList<DAL.Program>, IList<ProgramDto>>(programs);
            response.totalRecords = totalRecords;
            return response;
        }

        public ProgramDto ReadProgram(int id)
        {
            var program = context.Programs.Include("KitsinPrograms").Include("KitsinPrograms.Kit").Where(x => x.Id == id).FirstOrDefault();
            return Utility.mapper.Map<DAL.Program, ProgramDto>(program);
        }

        public GetKitsinProgramResponse GetKitsinProgram(GetKitsinProgramRequest request)
        {
            GetKitsinProgramResponse response = new GetKitsinProgramResponse();
            var kits = context.KitsinPrograms.Include("Program").Include("Kit").Where(x => x.ProgramId == request.programId && x.Active == true).ToList();
            response.kitsinProgram = Utility.mapper.Map<IList<DAL.KitsinProgram>, IList<KitsinProgramDto>>(kits);
            return response;
        }

        public AddEditProgramResponse AddEditProgram(AddEditProgramRequest request)
        {
            AddEditProgramResponse response = new AddEditProgramResponse();

            if (request.program.Id > 0)
            {
                var program = context.Programs.Where(x => x.Id == request.program.Id).FirstOrDefault();
                if (program != null)
                {
                    program.Name = request.program.Name;
                    program.Description = request.program.Description;
                    program.ProgramType = request.program.ProgramType;
                    program.RiskLevel = request.program.RiskLevel;
                    program.Active = request.program.Active;
                    program.Smoking = request.program.Smoking;
                    program.Pregancy = request.program.Pregancy;
                    program.ImageUrl = request.program.ImageUrl;
                    context.Programs.Attach(program);
                    context.Entry(program).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else
            {
                DAL.Program program = new DAL.Program();
                program.Name = request.program.Name;
                program.Description = request.program.Description;
                program.ProgramType = request.program.ProgramType;
                program.RiskLevel = request.program.RiskLevel;
                program.Active = request.program.Active;
                program.Smoking = request.program.Smoking;
                program.Pregancy = request.program.Pregancy;
                program.ImageUrl = request.program.ImageUrl;
                program.Active = true;
                context.Programs.Add(program);
                context.SaveChanges();
                request.program.Id = program.Id;
            }
            response.program = request.program;
            response.success = true;
            return response;
        }

        public bool DeleteProgram(int Id)
        {
            var program = context.Programs.Where(x => x.Id == Id).FirstOrDefault();
            if (program != null)
            {
                program.Active = false;
                context.Programs.Attach(program);
                context.Entry(program).State = EntityState.Modified;
                context.SaveChanges();
            }
            return true;
        }

        public int? StartProgram(int Id, out bool isSuccess, DateTime? startDate)
        {
            var program = context.UsersinPrograms.Where(x => x.Id == Id).FirstOrDefault();
            isSuccess = false;
            if (program != null)
            {
                if (!program.StartDate.HasValue)
                {
                    program.StartDate = startDate.HasValue ? startDate : DateTime.UtcNow;
                    context.UsersinPrograms.Attach(program);
                    context.Entry(program).State = EntityState.Modified;
                    context.SaveChanges();
                    isSuccess = true;
                }
                return program.ProgramsinPortalsId;
            }
            return null;
        }

        public DeleteKitfromProgramResponse DeleteKitfromProgram(DeleteKitfromProgramRequest request)
        {
            DeleteKitfromProgramResponse response = new DeleteKitfromProgramResponse();
            var kitinProgram = context.KitsinPrograms.Where(x => x.ProgramId == request.programId && x.KitId == request.kitId).FirstOrDefault();
            if (kitinProgram != null)
            {
                kitinProgram.Active = false;
                context.KitsinPrograms.Attach(kitinProgram);
                context.Entry(kitinProgram).State = EntityState.Modified;
                context.SaveChanges();
            }
            response.success = true;
            return response;
        }

        public AddEditKittoProgramResponse AddEditKittoProgram(AddEditKittoProgramRequest request)
        {
            AddEditKittoProgramResponse response = new AddEditKittoProgramResponse();
            var KitinProgram = context.KitsinPrograms.Where(x => x.ProgramId == request.kitinProgram.ProgramId && x.KitId == request.kitinProgram.KitId).FirstOrDefault();
            if (KitinProgram != null)
            {
                context.KitsinPrograms.Remove(KitinProgram);
            }
            DAL.KitsinProgram newKitinProgram = new DAL.KitsinProgram();
            newKitinProgram.ProgramId = request.kitinProgram.ProgramId;
            newKitinProgram.KitId = request.kitinProgram.KitId;
            newKitinProgram.Order = request.kitinProgram.Order;
            newKitinProgram.Active = true;
            context.KitsinPrograms.Add(newKitinProgram);
            context.SaveChanges();
            response.kitinProgram = request.kitinProgram;
            response.success = true;
            return response;
        }

        public GetProgramsByPortalResponse GetProgramsByPortal(GetProgramsByPortalRequest request)
        {
            GetProgramsByPortalResponse response = new GetProgramsByPortalResponse();
            var ProgramsinPortal = context.ProgramsinPortals.Include("ApptCallTemplate").Include("Program").Where(x => x.PortalId == request.PortalId && (!request.ProgramType.HasValue || request.ProgramType.Value == x.Program.ProgramType) && (!request.onlyActive.HasValue || x.Active == request.onlyActive)).ToList();
            response.ProgramsinPortal = Utility.mapper.Map<IList<DAL.ProgramsinPortal>, IList<ProgramsinPortalDto>>(ProgramsinPortal);
            return response;
        }

        public EnrollinProgramResponse EnrollinProgram(EnrollinProgramRequest request)
        {
            EnrollinProgramResponse response = new EnrollinProgramResponse();
            //check user has already active progran
            var Data = context.UsersinPrograms.Where(x => x.UserId == request.UserId && x.IsActive == true).FirstOrDefault();
            if (Data != null)
            {
                response.success = false;
                return response;
            }
            //get information about program type and kits in that program
            var programinportal = context.ProgramsinPortals.Include("Program").Include("Program.KitsinPrograms").Where(x => x.Id == request.ProgramsinPortalsId).FirstOrDefault();
            //check if the user ever enrolled in coaching program
            UsersinProgram pastUserinPrograms = null;
            if (programinportal.Program.ProgramType != (int)ProgramTypes.SelfHelp)
                pastUserinPrograms = context.UsersinPrograms.Include("ProgramsinPortal").Include("ProgramsinPortal.Program").Where(x => x.UserId == request.UserId && x.ProgramsinPortal.Program.ProgramType == (int)ProgramTypes.Coaching).FirstOrDefault();
            //enroll in program
            DAL.UsersinProgram usersinProgram = new DAL.UsersinProgram();
            usersinProgram.UserId = request.UserId;
            usersinProgram.ProgramsinPortalsId = request.ProgramsinPortalsId;
            usersinProgram.EnrolledBy = request.LoginId;
            if (!string.IsNullOrEmpty(request.Language))
                usersinProgram.Language = request.Language;
            if (request.hraId.HasValue)
                usersinProgram.HRAId = request.hraId.Value;
            //insert start date if it is self-help program
            if (programinportal.Program.ProgramType == (int)ProgramTypes.SelfHelp)
                usersinProgram.StartDate = System.DateTime.UtcNow;
            usersinProgram.EnrolledOn = System.DateTime.UtcNow;
            if (request.CoachId.HasValue)
                usersinProgram.CoachId = request.CoachId.Value;
            usersinProgram.IsActive = true;
            usersinProgram.UpdatedBy = request.LoginId;
            usersinProgram.UpdatedOn = DateTime.UtcNow;
            context.UsersinPrograms.Add(usersinProgram);
            context.SaveChanges();
            if (pastUserinPrograms == null)
            {
                var kits = programinportal.Program.KitsinPrograms.Where(x => x.Active).OrderByDescending(x => x.Order).ToList();
                if (kits != null && kits.Count > 0)
                {
                    AssignKitsToUserProgramRequest assignKitsRequest = new AssignKitsToUserProgramRequest();
                    assignKitsRequest.kitIds = kits.Select(x => x.KitId).ToList();
                    assignKitsRequest.usersinProgramsId = usersinProgram.Id;
                    assignKitsRequest.programType = programinportal.Program.ProgramType;
                    assignKitsRequest.userId = request.UserId;
                    assignKitsRequest.language = request.Language;
                    AssignKitsToUserProgram(assignKitsRequest);
                }
            }
            //update user status
            if (programinportal.Program.ProgramType == (int)ProgramTypes.Coaching)
            {
                UpdateUserTrackingStatus(request.UserId, request.PortalId, false, false);
            }
            UpdateTobaccoIncentive(request.UserId, request.PortalId, request.ProgramsinPortalsId);
            response.success = true;
            response.UsersinProgramId = usersinProgram.Id;
            response.ProgramType = usersinProgram.ProgramsinPortal.Program.ProgramType;
            response.ProgramsInPortalId = usersinProgram.ProgramsinPortalsId;
            return response;
        }

        public void AssignKitsToUserProgram(AssignKitsToUserProgramRequest request)
        {
            var kitMessage = false;
            for (int i = 0; i < request.kitIds.Count; i++)
            {
                DAL.KitsinUserProgram kit = new KitsinUserProgram();
                kit.KitId = request.kitIds[i];
                kit.UsersinProgramsId = request.usersinProgramsId;
                kit.StartDate = System.DateTime.UtcNow;
                kit.IsActive = true;
                context.KitsinUserPrograms.Add(kit);
                context.SaveChanges();
                if (!kitMessage)
                {
                    if (request.kitIds.Count > 1)
                    {
                        CommonReader reader = new CommonReader();
                        reader.AddDashboardMessage(request.userId, IncentiveMessageTypes.Program_Enrollment, "../Program/MyProgram", null);
                    }
                    else
                    {
                        AddKittoDashboardRequest dashboardRequest = new AddKittoDashboardRequest();
                        dashboardRequest.userId = request.userId;
                        dashboardRequest.kitId = kit.KitId;
                        dashboardRequest.kitsinUserProgramId = kit.Id;
                        dashboardRequest.languageCode = request.language;
                        AddKittoDashboard(dashboardRequest);
                        //AddAudiotoDashboard
                        var audioKit = context.Kits.Where(x => x.Id == kit.KitId && !String.IsNullOrEmpty(x.Audio)).FirstOrDefault();
                        if (audioKit != null)
                        {
                            CommonReader reader = new CommonReader();
                            var audio = audioKit.Audio.Split(';');
                            for (int j = 0; j < audio.Count(); j++)
                            {
                                string audioUrl = "../Audio/" + audio[j];
                                reader.AddDashboardMessage(request.userId, IncentiveMessageTypes.Audio_Kit, audioUrl, null, "'" + audioKit.Name + "'");
                            }
                        }
                    }
                    kitMessage = true;
                }
            }
        }

        public void AddKittoDashboard(AddKittoDashboardRequest request)
        {
            var kit = context.Kits.Include("KitTranslations").Where(x => x.Id == request.kitId).FirstOrDefault();
            string kitName;
            var translatedKit = kit.KitTranslations.Where(x => x.LanguageCode == request.languageCode).FirstOrDefault();
            if (translatedKit != null)
                kitName = translatedKit.Name;
            else
                kitName = kit.Name;
            string url = "../Kit/ViewKit/" + request.kitId + "?kitsInUserProgramId=" + request.kitsinUserProgramId;
            CommonReader reader = new CommonReader();
            reader.AddDashboardMessage(request.userId, IncentiveMessageTypes.New_Kit, url, null, "'" + kitName + "'");
        }


        public void PopulateQuestionValue(ActivitiesinStepsDto activity, int kitsInUserProgramsId)
        {
            Dictionary<int, List<QuestionsinActivityDto>> questionIds = new Dictionary<int, List<QuestionsinActivityDto>>();
            //Merge passive questions with original questions
            if (activity.PassiveQuestionsInActivities != null)
            {
                foreach (var passive in activity.PassiveQuestionsInActivities)
                {
                    activity.QuestionsinActivities.Add(new QuestionsinActivityDto(passive));
                }
            }
            foreach (QuestionsinActivityDto question in activity.QuestionsinActivities.Where(a => a.IsActive))
            {
                List<QuestionsinActivityDto> questions = new List<QuestionsinActivityDto>();
                if (questionIds.ContainsKey(question.Id))
                    questions = questionIds[question.Id];
                questions.Add(question);
                questionIds[question.Id] = questions;
            }
            var answers = context.UserChoices.Where(x => questionIds.Keys.Contains(x.QuestionId) && x.KitsInUserProgramsId == kitsInUserProgramsId).ToList();
            foreach (var answer in answers)
            {
                if (questionIds.Keys.Contains(answer.QuestionId))
                {
                    var questions = questionIds[answer.QuestionId];
                    foreach (var question in questions)
                        question.Value = answer.Value;
                }
            }
            activity.QuestionsinActivities = activity.QuestionsinActivities.Where(q => q.IsActive).OrderBy(q => q.SequenceNo).ToList();
        }

        public void PopulateQuizValue(IList<QuizinStepDto> quizList, int kitsInUserProgramsId)
        {
            Dictionary<int, QuizinStepDto> quizIds = new Dictionary<int, QuizinStepDto>();
            foreach (QuizinStepDto quiz in quizList)
            {
                quizIds.Add(quiz.Id, quiz);
            }

            var answers = context.UserQuizChoices.Where(x => quizIds.Keys.Contains(x.QuizId) && x.KitsInUserProgramsId == kitsInUserProgramsId).ToList();
            foreach (var answer in answers)
            {
                if (quizIds.Keys.Contains(answer.QuizId))
                {
                    var quiz = quizIds[answer.QuizId];
                    quiz.Value = answer.Value;
                }
            }
        }

        public void ChangeExercisePlanWeekStatus(ChangeExercisePlanWeekStatusRequest request)
        {
            var ExercisePlan = context.HRA_ExercisePlans.Where(x => x.HraId == request.hraId && x.Weeknumber == request.week).FirstOrDefault();
            bool status = ExercisePlan.Completed;
            bool newStatus;
            if (status)
                newStatus = false;
            else
                newStatus = true;
            ExercisePlan.Completed = newStatus;
            context.HRA_ExercisePlans.Attach(ExercisePlan);
            context.Entry(ExercisePlan).State = EntityState.Modified;
            context.SaveChanges();
        }

        public GetUserProgramHistoryResponse GetUserProgramHistory(GetUserProgramHistoryRequest request)
        {
            GetUserProgramHistoryResponse response = new GetUserProgramHistoryResponse();
            var userinPrograms = context.UsersinPrograms.Include("User1").Include("User1.AdminProperty").Include("ProgramsinPortal").Include("ProgramsinPortal.Program").Include("ProgramsinPortal.Portal").Include("ProgramsinPortal.Portal.PortalFollowUps").Include("ProgramInactiveReason")
                .Include("KitsinUserPrograms").Include("KitsinUserPrograms.Kit").Include("KitsinUserPrograms.Kit.KitTopic")
                .Include("KitsinUserPrograms.KitsinUserProgramGoals").Include("FollowUps").Include("ProgramsinPortal.Portal.PortalFollowUps.FollowUpType")
                .Where(x => x.UserId == request.userId).OrderByDescending(x => x.Id).ToList();

            response.usersinPrograms = Utility.mapper.Map<IList<DAL.UsersinProgram>, IList<UsersinProgramDto>>(userinPrograms);
            response.hasActiveProgram = userinPrograms.Where(x => x.IsActive == true).Count() > 0;

            if (response.usersinPrograms != null && response.usersinPrograms.Count > 0)
            {
                foreach (var program in response.usersinPrograms)
                {
                    if (program.KitsinUserPrograms != null && program.KitsinUserPrograms.Count > 0)
                    {
                        program.KitsinUserPrograms = program.KitsinUserPrograms.OrderByDescending(x => x.Id).ToList();
                        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(request.timeZone);
                        foreach (var kit in program.KitsinUserPrograms)
                        {
                            if (!String.IsNullOrEmpty(request.languageCode) && !request.languageCode.Equals("en-us"))
                            {
                                var kitTranslation = context.KitTranslations.Where(x => x.KitId == kit.KitId && x.LanguageCode == request.languageCode).FirstOrDefault();
                                if (kitTranslation != null)
                                    kit.Kit.Name = kitTranslation.Name;
                                kit.Kit.KitTopic.Name = context.KitTopics.Where(x => x.Id == kit.Kit.Topic).FirstOrDefault().LanguageItem;
                            }
                            kit.StartDate = TimeZoneInfo.ConvertTimeFromUtc(kit.StartDate, timeZone);
                        }
                    }
                }
            }
            return response;
        }

        public IList<UsersinProgramDto> GetUserProgramsByPortal(GetUserProgramHistoryRequest request)
        {
            var userinPrograms = context.UsersinPrograms.Include("User1").Include("ProgramsinPortal")
                .Where(x => x.UserId == request.userId && x.ProgramsinPortal.PortalId == request.portalId).OrderByDescending(x => x.Id).ToList();
            return Utility.mapper.Map<IList<DAL.UsersinProgram>, IList<UsersinProgramDto>>(userinPrograms);

        }
        public AddKittoUserProgramResponse AddKittoUserProgram(AddKittoUserProgramRequest request)
        {
            AddKittoUserProgramResponse response = new AddKittoUserProgramResponse();

            DAL.KitsinUserProgram kitsinUserProgram = new DAL.KitsinUserProgram();
            kitsinUserProgram.KitId = request.kitsId;
            kitsinUserProgram.UsersinProgramsId = request.userinProgramId;
            kitsinUserProgram.StartDate = System.DateTime.UtcNow;
            kitsinUserProgram.IsActive = true;
            context.KitsinUserPrograms.Add(kitsinUserProgram);
            context.SaveChanges();

            //add to participant's dashboard
            AddKittoDashboardRequest dashboardRequest = new AddKittoDashboardRequest();
            dashboardRequest.userId = request.UserId;
            dashboardRequest.kitId = kitsinUserProgram.KitId;
            dashboardRequest.kitsinUserProgramId = kitsinUserProgram.Id;
            dashboardRequest.languageCode = request.languageCode;
            AddKittoDashboard(dashboardRequest);
            var kit = context.Kits.Include("KitTranslations").Where(x => x.Id == request.kitsId).FirstOrDefault();
            response.success = true;
            var tranlatedKit = kit.KitTranslations.Where(x => x.LanguageCode == request.languageCode).FirstOrDefault();
            if (tranlatedKit != null)
                response.KitName = tranlatedKit.Name;
            else
                response.KitName = kit.Name;
            return response;
        }

        public DeleteKitinUserProgramResponse DeleteKitinUserProgram(DeleteKitinUserProgramRequest request)
        {
            DeleteKitinUserProgramResponse response = new DeleteKitinUserProgramResponse();
            var kitsinUserProgram = context.KitsinUserPrograms.Where(x => x.Id == request.id).FirstOrDefault();
            if (kitsinUserProgram != null)
            {
                kitsinUserProgram.IsActive = false;
                kitsinUserProgram.UpdatedOn = DateTime.UtcNow;
                kitsinUserProgram.UpdatedBy = request.UpdatedBy;
                context.KitsinUserPrograms.Attach(kitsinUserProgram);
                context.Entry(kitsinUserProgram).State = EntityState.Modified;
                context.SaveChanges();
            }
            var userDashboardMessages = context.UserDashboardMessages.Where(x => x.Url.StartsWith("../Kit/ViewKit/") && x.Url.EndsWith(request.id.ToString())).FirstOrDefault();
            if (userDashboardMessages != null)
            {
                userDashboardMessages.Active = false;
                context.UserDashboardMessages.Attach(userDashboardMessages);
                context.Entry(userDashboardMessages).State = EntityState.Modified;
                context.SaveChanges();
            }
            response.success = true;
            return response;
        }

        public UpdateUserinProgramResponse UpdateUserinProgram(UpdateUserinProgramRequest request)
        {
            UpdateUserinProgramResponse response = new UpdateUserinProgramResponse();
            if (!request.AssignedFollowup)
            {
                var userinProgram = context.UsersinPrograms
                    .Where(x => x.Id == request.UsersinProgramId).FirstOrDefault();
                if (userinProgram != null)
                {
                    var currentProgram = CloneUtil.DeepClone<DAL.UsersinProgram>(userinProgram);
                    if (request.InactiveReasonId.HasValue)
                    {
                        userinProgram.IsActive = false;
                        userinProgram.InactiveReason = request.InactiveReasonId;
                        userinProgram.InactiveDate = DateTime.UtcNow;
                        if (request.InactiveReasonId == (int)ProgramInactiveReasons.SuccessfullyCompleted)
                        {
                            userinProgram.CompleteDate = DateTime.UtcNow;
                            response.isProgramCompleted = true;
                        }
                    }
                    if (!string.IsNullOrEmpty(request.Language))
                        userinProgram.Language = request.Language;
                    if (request.CoachId.HasValue)
                        userinProgram.CoachId = request.CoachId;
                    if (request.PrograminPortalId > 0)
                        userinProgram.ProgramsinPortalsId = request.PrograminPortalId;
                    //if (request.AssignedFollowup)
                    //    userinProgram.AssignedFollowUp = request.AssignedFollowup;
                    userinProgram.UpdatedBy = request.LoginId;
                    userinProgram.UpdatedOn = DateTime.UtcNow;
                    context.UsersinPrograms.Attach(userinProgram);
                    context.Entry(userinProgram).State = EntityState.Modified;
                    UserHistoryReader.LogUserChanges(currentProgram, userinProgram, request.userId, request.LoginId.Value, UserHistoryCategoryDto.UserProgram);
                    context.SaveChanges();
                }
            }
            if (request.AssignedFollowup)
            {
                var userinProgram = context.UsersinPrograms.Where(x => x.Id == request.UsersinProgramId).FirstOrDefault();
                var currentProgram = CloneUtil.DeepClone<DAL.UsersinProgram>(userinProgram);
                FollowUpReader Followupreader = new FollowUpReader();
                var user = Followupreader.TriggerFollowUp(request.systemAdminId, request.UsersinProgramId, request.LoginId);
                if (user != null && user.Count() > 0)
                {
                    userinProgram.AssignedFollowUp = (byte)((userinProgram.AssignedFollowUp ?? 0) + 1);
                    userinProgram.UpdatedBy = request.LoginId;
                }
                UserHistoryReader.LogUserChanges(currentProgram, userinProgram, request.userId, request.LoginId.Value, UserHistoryCategoryDto.UserProgram);
                response.user = user[0];
            }
            UpdateTobaccoIncentive(request.userId, request.PortalId, request.PrograminPortalId);
            var userIncentive = context.UserIncentives.Include("PortalIncentive").Where(p => p.PortalIncentive.PortalId == request.PortalId && p.PortalIncentive.IncentiveTypeId == (int)IncentiveTypes.Tobacco_Initiative && p.UserId == request.userId && request.PrograminPortalId != p.PortalIncentive.RefId).FirstOrDefault();
            response.success = true;
            return response;
        }

        public void UpdateTobaccoIncentive(int userId, int portalId, int programinPortalId)
        {
            var userIncentive = context.UserIncentives.Include("PortalIncentive").Where(p => p.PortalIncentive.PortalId == portalId && p.PortalIncentive.IncentiveTypeId == (int)IncentiveTypes.Tobacco_Initiative && p.UserId == userId && programinPortalId != p.PortalIncentive.RefId).FirstOrDefault();
            if (userIncentive != null)
            {
                var portalIncentive = context.PortalIncentives.Where(p => p.PortalId == portalId && p.IncentiveTypeId == (int)IncentiveTypes.Tobacco_Initiative && programinPortalId == p.RefId).FirstOrDefault();
                if (portalIncentive != null)
                {
                    userIncentive.PortalIncentiveId = portalIncentive.Id;
                    context.UserIncentives.Attach(userIncentive);
                    context.Entry(userIncentive).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }
        public UpdateUserinProgramResponse ActivateUserinProgram(UpdateUserinProgramRequest request)
        {
            UpdateUserinProgramResponse response = new UpdateUserinProgramResponse();
            var userinProgramtoClone = context.UsersinPrograms
                     .Where(x => x.Id == request.UsersinProgramId).FirstOrDefault();
            var currentProgram = CloneUtil.DeepClone<DAL.UsersinProgram>(userinProgramtoClone);
            var userinProgram = context.UsersinPrograms.Include("ProgramsinPortal").Include("ProgramsinPortal.Program")
                .Where(x => x.Id == request.UsersinProgramId).FirstOrDefault();
            if (userinProgram.InactiveReason != null)
            {
                userinProgram.IsActive = true;
                userinProgram.InactiveReason = null;
                userinProgram.InactiveDate = null;
                if (userinProgram.CompleteDate.HasValue)
                    userinProgram.CompleteDate = null;
                userinProgram.UpdatedBy = request.LoginId;
                userinProgram.UpdatedOn = DateTime.UtcNow;
                UserHistoryReader.LogUserChanges(currentProgram, userinProgram, request.userId, request.LoginId.Value, UserHistoryCategoryDto.UserProgram);
                context.SaveChanges();
            }
            //update user status
            if (userinProgram.ProgramsinPortal.Program.ProgramType == (int)ProgramTypes.Coaching)
            {
                UpdateUserTrackingStatus(userinProgram.UserId, request.PortalId, null, false);
            }
            response.success = true;
            return response;
        }

        public void UpdateUserTrackingStatus(int UserId, int PortalId, bool? DeclinedEnroll, bool? DoNotTrack)
        {
            ParticipantReader reader = new ParticipantReader();
            UpdateUserTrackingStatusRequest statusRequest = new UpdateUserTrackingStatusRequest();
            statusRequest.UserId = UserId;
            statusRequest.PortalId = PortalId;
            if (DeclinedEnroll.HasValue)
                statusRequest.DeclinedEnroll = DeclinedEnroll;
            if (DoNotTrack.HasValue)
                statusRequest.DoNotTrack = DoNotTrack;
            reader.UpdateUserTrackingStatus(statusRequest);
        }

        public UsersinProgramDto GetUserProgramForHRA(int hraId)
        {
            var userinProgram = context.UsersinPrograms.Include("ProgramsinPortal").Include("ProgramsinPortal.Program")
                .Include("KitsinUserPrograms").Include("KitsinUserPrograms.Kit").Where(x => x.HRAId == hraId).FirstOrDefault();
            return Utility.mapper.Map<DAL.UsersinProgram, UsersinProgramDto>(userinProgram);
        }

        public UsersinProgramDto GetUserinProgramDetails(int userinProgramId)
        {
            var userinProgram = context.UsersinPrograms.Include("User1").Include("User1.AdminProperty").Include("User1.Specializations").Include("User1.Languages").Include("ProgramsinPortal").Include("ProgramsinPortal.Program")
                .Include("KitsinUserPrograms").Include("KitsinUserPrograms.Kit").Include("KitsinUserPrograms.Kit.KitTopic").Include("KitsinUserPrograms.Kit.KitTranslations").Include("FollowUps").Where(x => x.Id == userinProgramId).FirstOrDefault();
            if (userinProgram.Language != null && userinProgram.Language != ListOptions.DefaultLanguage && !string.IsNullOrEmpty(userinProgram.User1.AdminProperty.ProfileLanguageItem))
            {
                LanguageReader langReader = new LanguageReader();
                var codes = new List<string>();
                if (!string.IsNullOrEmpty(userinProgram.User1.AdminProperty.ProfileLanguageItem))
                    codes.Add(userinProgram.User1.AdminProperty.ProfileLanguageItem);
                if (!string.IsNullOrEmpty(userinProgram.ProgramsinPortal.DescforUserLanguageItem))
                    codes.Add(userinProgram.ProgramsinPortal.DescforUserLanguageItem);
                if (!string.IsNullOrEmpty(userinProgram.ProgramsinPortal.NameforUserLanguageItem))
                    codes.Add(userinProgram.ProgramsinPortal.NameforUserLanguageItem);
                var langItems = langReader.GetLanguageItems(codes, userinProgram.Language);
                if (langItems != null && langItems.Count > 0)
                {
                    var item = langItems.Find(l => l.ItemCode == userinProgram.User1.AdminProperty.ProfileLanguageItem);
                    if (item != null)
                        userinProgram.User1.AdminProperty.Profile = item.Text;
                    item = langItems.Find(l => l.ItemCode == userinProgram.ProgramsinPortal.DescforUserLanguageItem);
                    if (item != null)
                        userinProgram.ProgramsinPortal.DescriptionforUser = item.Text;
                    item = langItems.Find(l => l.ItemCode == userinProgram.ProgramsinPortal.NameforUserLanguageItem);
                    if (item != null)
                        userinProgram.ProgramsinPortal.NameforUser = item.Text;
                }
            }
            return Utility.mapper.Map<DAL.UsersinProgram, UsersinProgramDto>(userinProgram);
        }

        public GetKitsHistoryforUserResponse GetKitsHistoryforUser(GetKitsHistoryforUserRequest request)
        {
            GetKitsHistoryforUserResponse response = new GetKitsHistoryforUserResponse();
            var userinPrograms = context.UsersinPrograms.Include("KitsinUserPrograms").Include("KitsinUserPrograms.Kit")
                .Where(x => x.UserId == request.UserId).ToList();
            if (userinPrograms != null && userinPrograms.Count > 0)
            {
                List<KitsinUserProgramDto> KitsinUserPrograms = new List<KitsinUserProgramDto>();
                for (int i = 0; i < userinPrograms.Count; i++)
                {
                    KitsinUserPrograms.AddRange(Utility.mapper.Map<IList<DAL.KitsinUserProgram>, IList<KitsinUserProgramDto>>(userinPrograms[i].KitsinUserPrograms.ToList()));
                }
                response.KitsinUserPrograms = KitsinUserPrograms;
            }
            return response;
        }

        public ListInactiveReasonResponse ListInactiveReasons(ListInactiveReasonRequest request)
        {
            ListInactiveReasonResponse response = new ListInactiveReasonResponse();
            var inactiveReasion = context.ProgramInactiveReasons.Where(x => x.Show == true).ToList();
            response.InactiveReasion = Utility.mapper.Map<IList<DAL.ProgramInactiveReason>, IList<ProgramInactiveReasonDto>>(inactiveReasion).ToList();
            return response;

        }

        public GetProgramEnrollmentResponse GetProgramEnrollmentTask(GetProgramEnrollmentRequest request)
        {
            GetProgramEnrollmentResponse response = new GetProgramEnrollmentResponse();
            if (context.AdminTasks.Where(x => x.UserId == request.userId && x.TaskTypeId == request.taskTypeId && x.Status == "N").Count() > 0)
                response.success = true;
            return response;
        }

        #region Appointment Template
        public AddOrEditAppointmentTemplateResponse ReadAppointmentTemplate(AddOrEditAppointmentTemplateRequest request)
        {
            AddOrEditAppointmentTemplateResponse response = new AddOrEditAppointmentTemplateResponse();
            var lst = context.ApptCallTemplate.Include("ApptCallIntervals").Where(x => x.Id == request.templateid).FirstOrDefault();
            response.CallTemplate = Utility.mapper.Map<DAL.ApptCallTemplate, AppointmentCallTemplateDto>(lst);
            response.CallIntervals = Utility.mapper.Map<ICollection<DAL.ApptCallInterval>, ICollection<AppointmentCallIntervalDto>>(lst.ApptCallIntervals);
            return response;
        }
        public AddOrEditAppointmentTemplateResponse AddOrEditAppointmentTemplate(AddOrEditAppointmentTemplateRequest request)
        {
            AddOrEditAppointmentTemplateResponse response = new AddOrEditAppointmentTemplateResponse();
            return response;
        }

        public ListAppointmentTemplateResponse AppointmentTemplateList(ListAppointmentTemplateRequest request)
        {
            ListAppointmentTemplateResponse response = new ListAppointmentTemplateResponse();
            int? templateId = null;
            if (!String.IsNullOrEmpty(request.AppointmentTemplateId))
                templateId = Int32.Parse(request.AppointmentTemplateId);
            var lst = context.ApptCallTemplate.Where(x => String.IsNullOrEmpty(request.AppointmentTemplateId) || x.Id == templateId.Value).ToList();
            response.CallTemplates = Utility.mapper.Map<IList<DAL.ApptCallTemplate>, IList<AppointmentCallTemplateDto>>(lst);
            return response;
        }

        public AddOrEditAppointmentIntervalResponse AppointmentTemplateIntervalList(AddOrEditAppointmentIntervalRequest request)
        {
            AddOrEditAppointmentIntervalResponse response = new AddOrEditAppointmentIntervalResponse();
            int templateId = Int32.Parse(request.AppointmentTemplateId);
            var lst = context.ApptCallInterval.Where(x => x.ApptCallTemplateId == templateId).ToList();
            response.CallIntervals = Utility.mapper.Map<IList<DAL.ApptCallInterval>, IList<AppointmentCallIntervalDto>>(lst);
            return response;
        }

        public AddOrEditAppointmentTemplateResponse CreateAppointmentTemplate(AddOrEditAppointmentTemplateRequest request)
        {
            AddOrEditAppointmentTemplateResponse response = new AddOrEditAppointmentTemplateResponse();
            if (request.templateid == null)
            {
                DAL.ApptCallTemplate template = new DAL.ApptCallTemplate();
                template.TemplateName = request.Template.TemplateName;
                template.NoOfWeeks = request.Template.NoOfWeeks;
                template.NoOfCalls = request.Template.NoOfCalls;
                template.IsActive = request.Template.IsActive;
                template.UpdatedDate = request.Template.UpdatedDate;
                template.UpdatedBy = request.Template.UpdatedBy;

                context.ApptCallTemplate.Add(template);
                context.SaveChanges();
                foreach (var intervalDto in request.Template.CallIntervals)
                {
                    DAL.ApptCallInterval interval = new DAL.ApptCallInterval();
                    interval.ApptCallTemplateId = template.Id;
                    interval.CallNumber = intervalDto.CallNumber;
                    interval.IntervalInDays = intervalDto.IntervalInDays;
                    context.ApptCallInterval.Add(interval);
                }
                context.SaveChanges();
                response.status = true;
                }
            else
            {
                ProgramsinPortal programsinPortal = new ProgramsinPortal();
                programsinPortal = context.ProgramsinPortals.Where(x => x.ApptCallTemplateId == request.templateid).FirstOrDefault();
                if (programsinPortal == null)
                {
                    var template = context.ApptCallTemplate.Where(x => x.Id == request.templateid).FirstOrDefault();
                    template.TemplateName = request.Template.TemplateName;
                    template.NoOfWeeks = request.Template.NoOfWeeks;
                    template.NoOfCalls = request.Template.NoOfCalls;
                    template.IsActive = request.Template.IsActive;
                    template.UpdatedDate = request.Template.UpdatedDate;
                    template.UpdatedBy = request.Template.UpdatedBy;
                    context.SaveChanges();
                    response.status = true;
                }
                else
                {
                    response.status = false;
                }
            }
            return response;
        }

        #endregion        
    }
}