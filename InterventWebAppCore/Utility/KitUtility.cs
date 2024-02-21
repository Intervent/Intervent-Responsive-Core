using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;

namespace InterventWebApp
{
    public class KitUtility
    {
        public static ListEduKitsResponse ListEduKits(int? participantPortalId, int? page, int? pageSize, int? totalRecords, bool portalKits = false)
        {
            KitReader reader = new KitReader();
            ListEduKitsRequest request = new ListEduKitsRequest();
            if (portalKits)
            {
                request.PortalId = participantPortalId.Value;
            }
            request.page = page;
            request.pageSize = pageSize;
            request.totalRecords = totalRecords;
            return reader.ListEduKits(request);
        }

        public static bool DeleteEduKit(int Id)
        {
            KitReader reader = new KitReader();
            return reader.DeleteEduKit(Id);
        }

        public static UploadKitPdfResponse UploadPdf(int kitId, string pdfUrl, string langCode)
        {
            KitReader reader = new KitReader();
            UploadKitPdfRequest request = new UploadKitPdfRequest();
            request.kitId = kitId;
            request.pdfUrl = pdfUrl;
            request.languageCode = langCode;
            if (!string.IsNullOrEmpty(langCode) && langCode != ListOptions.DefaultLanguage)
                return reader.UploadTranslationPdf(request);
            else
                return reader.UploadPdf(request);
        }

        public static bool AddEditEduKitTranslation(int id, string name, string description, string keyConcepts, string audio, string pdf, string language, DateTime? publishDate, DateTime? lastUpdated)
        {
            KitReader reader = new KitReader();
            KitTranslationDto edukit = new KitTranslationDto();
            edukit.KitId = id;
            edukit.Name = name;
            edukit.Description = description;
            edukit.KeyConcepts = keyConcepts;
            edukit.Audio = audio;
            edukit.Pdf = pdf;
            edukit.LanguageCode = language;
            edukit.PublishedDate = publishDate;
            edukit.LastUpdated = lastUpdated;
            return reader.AddEditKitTranslation(edukit);
        }
        public static AddEditEduKitResponse AddEditEduKit(int? id, string name, string description, short topic, string keyConcepts, string audio, string pdf, string inventoryId, DateTime? publishDate, DateTime? lastUpdated)
        {
            KitReader reader = new KitReader();
            AddEditEduKitRequest request = new AddEditEduKitRequest();
            KitsDto edukit = new KitsDto();
            if (id.HasValue)
                edukit.Id = id.Value;
            edukit.Name = name;
            edukit.Description = description;
            edukit.Topic = topic;
            edukit.KeyConcepts = keyConcepts;
            edukit.Audio = audio;
            edukit.Pdf = pdf;
            edukit.InvId = inventoryId;
            edukit.PublishedDate = publishDate;
            edukit.LastUpdated = lastUpdated;
            request.eduKit = edukit;
            return reader.AddEditEduKit(request);
        }

        public static GetKitDetailsResponse GetKitDetails(int id, string language)
        {
            KitReader reader = new KitReader();
            GetKitDetailsRequest request = new GetKitDetailsRequest();
            request.id = id;
            request.language = language;
            return reader.GetKitDetails(request);
        }

        public static SearchQuestionsResponse SearchQuestion(int kitId, string searchText, int? questionType, int? activityId)
        {
            KitReader reader = new KitReader();
            SearchQuestionsRequest request = new SearchQuestionsRequest();
            request.kitId = kitId;
            request.searchText = searchText;
            request.questionType = questionType;
            request.activityId = activityId;
            return reader.SearchQuestion(request);
        }

        public static GetKitByIdResponse GetKitById(int? userinProgramId, int kitId, int? kitsInUserProgramId, bool? preview, string prevLanguage)
        {
            KitReader reader = new KitReader();
            GetKitByIdRequest request = new GetKitByIdRequest();
            request.kitId = kitId;
            request.kitsInUserProgramId = kitsInUserProgramId;
            request.userinProgramId = preview != true ? (userinProgramId.HasValue ? userinProgramId.Value : 0) : 0;
            request.preview = preview;
            request.PreviewLanguage = prevLanguage;
            return reader.GetKitById(request);
        }

        public static ReadPageResponse GetKitByIdentifier(int? participantId, int kitId, string pageIdentifier, int kitsInUserProgramsId, string languageCode)
        {
            KitReader reader = new KitReader();
            ProgramReader programReader = new ProgramReader();
            string[] ids = pageIdentifier.Split(KitReader.Separator);
            ReadPageResponse response = new ReadPageResponse();
            int sId;
            if (ids.Length > 1)
            {
                response.Step = new StepsinKitsDto();
                response.Step.ActivitiesinSteps = new List<ActivitiesinStepsDto>();
                int stepId;
                int.TryParse(ids[0], out stepId);

                if (stepId == 0)
                {
                    response.Step.ActivitiesinSteps = reader.ReadActivityinStepById(GetActivityIdList(ids[1]), languageCode);
                }
                // response.ActivitiesinSteps.Add(reader.GetStepWithActivity(stepId));            
                else if (string.IsNullOrEmpty(ids[1]))
                {
                    ReadStepinKitRequest request = new ReadStepinKitRequest();
                    request.id = stepId;
                    request.language = languageCode;
                    response.Step = reader.ReadStepinKit(request).stepinKit;
                }
                else
                {
                    GetStepWithActivityRequest request = new GetStepWithActivityRequest();
                    request.stepId = stepId;
                    request.activityIds = GetActivityIdList(ids[1]);
                    request.languageCode = languageCode;
                    response.Step = reader.GetStepWithActivity(request).stepsinKits;
                }

                if (response.Step.ActivitiesinSteps != null && response.Step.ActivitiesinSteps.Count > 0)
                {
                    foreach (var activity in response.Step.ActivitiesinSteps)
                        programReader.PopulateQuestionValue(activity, kitsInUserProgramsId);
                }

            }
            else if (ids[0] == "KC")
            {
                response.KeyConcepts = reader.ReadKeyConcepts(kitId, languageCode);
            }
            else if (int.TryParse(ids[0], out sId))
            {
                response.Quiz = reader.ReadQuizinKit(sId, languageCode);
                programReader.PopulateQuizValue(response.Quiz, kitsInUserProgramsId);
            }
            response.KitsInUserProgramId = kitsInUserProgramsId;
            if (participantId.HasValue)
                response.KitsinUserProgramGoal = reader.GetKitsActionGoals(new KitActionGoalsRequest { participantId = participantId.Value }).KitsinUserProgramGoals.Where(x => x.KitsinUserProgramId == kitsInUserProgramsId).FirstOrDefault();
            return response;
        }

        public static GetKitByIdResponse ReviewKit(int? participantId, int kitsInUserProgramId)
        {
            KitReader reader = new KitReader();
            return reader.ReviewKit(new GetReviewKitRequest { kitsInUserProgramId = kitsInUserProgramId, userId = participantId.Value });
        }

        private static List<int> GetActivityIdList(string ids)
        {
            List<int> activityIds = new List<int>();
            string[] aIds = ids.Split(KitReader.ActivitySeparator);
            foreach (var id in aIds)
            {
                if (!string.IsNullOrEmpty(id))
                    activityIds.Add(int.Parse(id));
            }
            return activityIds;
        }

        public static string ReadLanguageText(LanguageType type, int id, string language)
        {
            LanguageReader reader = new LanguageReader();
            var item = reader.GetLanguageItem(type, id, language);
            if (item != null)
            {
                return item.Text;
            }
            else
            {
                return null;
            }
        }

        public static AddEditStepinKitResponse AddEditStepinKit(int kitId, int? id, string text, string stepNo, string stepName, bool isActive, bool isSubStep, bool isAppendix, bool isGoal, string language)
        {
            KitReader reader = new KitReader();
            AddEditStepinKitRequest request = new AddEditStepinKitRequest();
            request.language = language;
            StepsinKitsDto stepsinKits = new StepsinKitsDto();
            stepsinKits.KitId = kitId;
            if (id.HasValue)
                stepsinKits.Id = id.Value;
            stepsinKits.Text = text;
            stepsinKits.Name = stepName;
            stepsinKits.StepNo = stepNo;
            stepsinKits.IsActive = isActive;
            stepsinKits.IsSubStep = isSubStep;
            stepsinKits.IsAppendix = isAppendix;
            stepsinKits.IsGoal = isGoal;
            request.stepsinKits = stepsinKits;
            return reader.AddEditStepinKit(request);
        }

        public static ReadStepinKitResponse ReadStepinKit(int id, string language)
        {
            KitReader reader = new KitReader();
            ReadStepinKitRequest request = new ReadStepinKitRequest();
            request.id = id;
            request.language = language;
            return reader.ReadStepinKit(request);
        }

        public static ReadActivityinStepResponse ReadActivityinStep(int activityId)
        {
            KitReader reader = new KitReader();
            ReadActivityinStepRequest request = new ReadActivityinStepRequest();
            request.ActivityId = activityId;
            return reader.ReadActivityinStep(request);
        }

        public static SelectList GetInputTypes()
        {

            SelectList inputTypes = new SelectList(new CommonReader().GetQuestionTypes().KeyValueList, "Value", "Text");

            return inputTypes;
        }

        public static AddEditQuestionResponse AddEditQuestioninActivity(int activityId, int? id, string questionText, byte questionType, bool isActive, int? sequence, bool isVertical, string language, int? parentId, bool isRequired)
        {
            KitReader reader = new KitReader();
            AddEditQuestioninActivityRequest request = new AddEditQuestioninActivityRequest();
            request.language = language;
            QuestionsinActivityDto questioninActivity = new QuestionsinActivityDto();
            questioninActivity.ActivityId = activityId;
            if (id.HasValue)
                questioninActivity.Id = id.Value;
            questioninActivity.QuestionType = questionType;
            questioninActivity.QuestionText = questionText;
            questioninActivity.IsActive = isActive;
            questioninActivity.IsRequired = isRequired;
            questioninActivity.ShowVertical = isVertical;
            questioninActivity.ParentId = parentId;
            questioninActivity.SequenceNo = sequence.HasValue ? (short)sequence.Value : (short)1;
            request.questioninActivity = questioninActivity;
            return reader.AddEditQuestioninActivity(request);
        }

        public static AddEditQuestionResponse AddEditPassiveQuestionsInActivity(int activityId, int questionId, string questionText, int? sequence, bool isActive, string language)
        {
            KitReader reader = new KitReader();
            AddEditPassiveQuestionsInActivityRequest request = new AddEditPassiveQuestionsInActivityRequest();
            request.language = language;
            PassiveQuestionsInActivitiesDto questioninActivity = new PassiveQuestionsInActivitiesDto();
            questioninActivity.ActivityId = activityId;
            questioninActivity.QuestionId = questionId;
            questioninActivity.QuestionText = questionText;
            questioninActivity.IsActive = isActive;
            questioninActivity.SequenceNo = sequence.HasValue ? (short)sequence.Value : (short)1;
            request.PassiveQuestions = questioninActivity;
            return reader.AddEditPassiveQuestionsInActivity(request);
        }

        public static AddEditActivityinStepResponse AddEditActivityinStep(int stepId, int? id, string topText, string bottomText, bool withinStep, bool isActive, bool allowUpdate, int? sequence, string language)
        {
            KitReader reader = new KitReader();
            AddEditActivityinStepRequest request = new AddEditActivityinStepRequest();
            request.language = language;
            ActivitiesinStepsDto activityinStep = new ActivitiesinStepsDto();
            activityinStep.StepId = stepId;
            if (id.HasValue)
                activityinStep.Id = id.Value;
            activityinStep.TopText = topText;
            activityinStep.BottomText = bottomText;
            activityinStep.WithinStep = withinStep;
            activityinStep.IsActive = isActive;
            activityinStep.AllowUpdate = allowUpdate;
            activityinStep.SequenceNo = sequence.HasValue ? sequence.Value : 1;
            request.activityinStep = activityinStep;
            return reader.AddEditActivityinStep(request);
        }

        public static AddEditOptionsResponse AddEditOptions(string questionOption, int questionId, int? id, bool? isAnswer, short? sequence, int? points, string language, bool? isActive = null)
        {
            KitReader reader = new KitReader();
            AddEditOptionsRequest request = new AddEditOptionsRequest();
            request.language = language;
            request.OptionsForQuestions = new OptionsforActivityQuestionDto();
            request.OptionsForQuestions.OptionText = questionOption;
            request.OptionsForQuestions.QuestionId = questionId;
            request.OptionsForQuestions.Id = id.HasValue ? id.Value : 0;
            request.OptionsForQuestions.IsAnswer = isAnswer;
            request.OptionsForQuestions.Points = points;
            request.OptionsForQuestions.IsActive = isActive;
            request.OptionsForQuestions.SequenceNo = sequence;
            return reader.AddEditOptions(request);
        }

        public static IList<QuizinStepDto> ReadQuizinKit(int stepId, string languageCode)
        {
            KitReader reader = new KitReader();
            return reader.ReadQuizinKit(stepId, languageCode);
        }

        public static AddEditQuizResponse AddEditQuiz(int stepId, int? id, string quizText, byte quizType, bool isActive, string language)
        {
            KitReader reader = new KitReader();
            AddEditQuizRequest request = new AddEditQuizRequest();
            request.language = language;
            QuizinStepDto quizinKit = new QuizinStepDto();
            quizinKit.StepId = stepId;
            if (id.HasValue)
                quizinKit.Id = id.Value;
            quizinKit.QuizType = quizType;
            quizinKit.QuizText = quizText;
            quizinKit.IsActive = isActive;
            request.quizinKit = quizinKit;
            return reader.AddEditQuiz(request);
        }

        public static AddEditOptionsforQuizResponse AddEditOptionsforQuiz(string quizOption, int quizId, int? id, bool isDefault, bool isActive, string language)
        {
            KitReader reader = new KitReader();
            AddEditOptionsforQuizRequest request = new AddEditOptionsforQuizRequest();
            request.language = language;
            OptionsforQuizDto option = new OptionsforQuizDto();
            option.OptionText = quizOption;
            option.QuizId = quizId;
            option.IsDefault = isDefault;
            option.IsActive = isActive;
            if (id.HasValue)
                option.Id = id.Value;
            request.optionforQuiz = option;
            return reader.AddEditOptionsforQuiz(request);
        }

        public static bool CloneKit(int kitId)
        {
            KitReader reader = new KitReader();
            reader.CloneKit(kitId);
            return true;
        }

        public static IList<KitTopicsDto> GetKitTopics()
        {
            KitReader reader = new KitReader();
            return reader.GetKitTopics();
        }

        public static ListAssignedKitsResponse ListAssignedKits(int? participantId)
        {
            KitReader reader = new KitReader();
            ListAssignedKitsRequest request = new ListAssignedKitsRequest();
            request.UserId = participantId.Value;
            return reader.ListAssignedKits(request);
        }
        #region PromptsinKits
        public static bool AddEditPrompts(int? userId, PromptModel model)
        {
            KitReader reader = new KitReader();
            PromptDto prompt = new PromptDto();
            prompt = model.Prompt;
            prompt.DateUpdated = DateTime.UtcNow;
            prompt.UpdatedBy = userId.Value;
            return reader.AddEditPrompts(prompt);
        }

        public static bool AddEditTableOption(int row, int column, int parentId, int activityId)
        {
            KitReader reader = new KitReader();
            AddEditTableOptionRequest option = new Intervent.Web.DTO.AddEditTableOptionRequest();
            option.Row = row;
            option.Column = column;
            option.ParentId = parentId;
            option.ActivityId = activityId;
            return reader.AddEditTableOption(option).IsSuccess;
        }

        public static ListRowResponse AddRow(int parentId)
        {
            KitReader reader = new KitReader();
            AddRowRequest row = new AddRowRequest();
            row.ParentId = parentId;
            return reader.AddRow(row);
        }

        public static ListRowResponse ReadRows(int questionId)
        {
            KitReader reader = new KitReader();
            return reader.ReadRows(questionId);
        }

        public static ListColumnResponse ReadColumns(int rowId)
        {
            KitReader reader = new KitReader();
            return reader.ReadColumns(rowId);
        }

        public static ListColumnResponse AddColumn(int activityId, int rowId, int columnId, byte? questionType, short rowSpan, short colSpan, string style, string text, string language)
        {
            KitReader reader = new KitReader();
            AddEditColumnRequest column = new AddEditColumnRequest();
            column.Column = new ColumnDto();
            column.Language = language;
            column.Column.ColumnId = columnId;
            column.Column.RowId = rowId;
            column.Column.ActivityId = activityId;
            column.Column.QuestionText = text;
            column.Column.RowSpan = rowSpan;
            column.Column.ColumnSpan = colSpan;
            column.Column.QuestionType = questionType;
            column.Column.Style = style;
            return reader.AddEditColumn(column);
        }

        public static ListPromptinKitsResponse ListPrompts(int id)
        {
            KitReader reader = new KitReader();
            ListPromptsinKitsRequest response = new ListPromptsinKitsRequest();
            response.KitId = id;
            return reader.ListPrompts(response);
        }

        public static TableCheckboxResponse ReadTableChkBox(int questionid)
        {
            KitReader reader = new KitReader();
            return reader.ReadTableChkBox(questionid);
        }
        public static void SetColSpan(int parentQuesId, short? sequence)
        {
            if (sequence.HasValue)
            {
                KitReader reader = new KitReader();
                reader.SetColSpan(parentQuesId, sequence.Value);
            }
        }

        public static TableCheckboxResponse AddTableChkBox(int indexId, int QuesId, int parentQuesId, int activityId, string indexText, string questionText, short? sequence, string language, bool isOption)
        {
            KitReader reader = new KitReader();
            AddEditTableCheckboxRequest request = new AddEditTableCheckboxRequest();
            request.Question = new ChkColumnDto();
            request.ActivityId = activityId;
            request.Question.IndexId = indexId;
            request.Question.QuestionId = QuesId;
            request.ParentQuestionId = parentQuesId;

            request.Question.Sequence = sequence;
            request.Language = language;
            request.IsOption = isOption;
            request.Question.IndexText = indexText;
            request.Question.QuestionText = questionText;
            return reader.AddTableChkBox(request);
        }


        public static List<SelectListItem> ReferenceIdListGet(int id, int refId)
        {
            KitReader reader = new KitReader();
            ReadStepinKitRequest request = new ReadStepinKitRequest();
            request.id = id;
            List<StepsinKitsDto> stepsList = new List<StepsinKitsDto>();
            List<ActivitiesinStepsDto> activitiesList = new List<ActivitiesinStepsDto>();
            if (refId == 1)
                stepsList = reader.StepsinKitListGet(new ReadStepsinKitListRequest { id = id }).StepsinKits;
            else
                activitiesList = reader.ActivitiesinStepByKitId(new GetActivitiesinStepRequest { KitId = id }).ActivitiesList;

            List<SelectListItem> listItems = new List<SelectListItem>();
            if (stepsList != null && stepsList.Count > 0)
            {
                var StrBuilder = new System.Text.StringBuilder();
                foreach (var data in stepsList)
                {
                    data.Name = data.Name != "" ? " (" + data.Name + ")" : "";
                    if (StrBuilder.ToString() != "" && StrBuilder.ToString().Contains(data.Name))
                        data.Name = "";
                    else
                        StrBuilder.Append(data.Name);

                    listItems.Add(new SelectListItem
                    {
                        Text = data.Id.ToString() + data.Name,
                        Value = data.Id.ToString()
                    });
                }
            }
            else if (activitiesList != null && activitiesList.Count > 0)
            {
                foreach (var data in activitiesList)
                {
                    data.TopText = Regex.Replace(data.TopText, "<.*?>", String.Empty);
                    data.TopText = data.TopText != "" && data.TopText.Length < 50 ? " (" + data.TopText + ")" : data.TopText;
                    listItems.Add(new SelectListItem
                    {
                        Text = data.TopText != "" && data.TopText.Length >= 50 ? data.Id.ToString() + " (" + data.TopText.Substring(0, 50) + ")" : data.Id.ToString() + data.TopText,
                        Value = data.Id.ToString()
                    });
                }
            }
            return listItems;
        }

        public static ReadPromptResponse ReadPrompt(int id)
        {
            KitReader reader = new KitReader();
            ReadPromptRequest request = new ReadPromptRequest();
            request.id = id;
            return reader.ReadPrompt(request);

        }

        public static bool CompletePrompt(int? userId, PromptsinKitsCompletedModel model)
        {
            KitReader reader = new KitReader();
            PromptsinKitsCompletedDto request = new PromptsinKitsCompletedDto();
            request = model.CompletePromptsinKit;
            request.UpdatedBy = userId.Value;
            request.UpdatedDate = DateTime.UtcNow;
            return reader.CompletePrompt(request);
        }

        public static bool ListenedAudio(int kitsinUserProgramId)
        {
            KitReader reader = new KitReader();
            return reader.ListenedAudio(kitsinUserProgramId);
        }
        #endregion PromptsinKits



        public static AddEditActionGoalResponse AddEditActionGoals(int? participantId, int? adminId, int? participantPortalId, int? kitsInUserProgramId, string goals, int? goalsId, bool? achieveGoal)
        {
            KitReader reader = new KitReader();
            AddEditActionGoalRequest request = new AddEditActionGoalRequest();
            request.participantId = participantId.Value;
            request.kitsInUserProgramId = kitsInUserProgramId;
            request.goals = goals;
            request.goalsId = goalsId;
            request.achieveGoal = achieveGoal.HasValue ? achieveGoal.Value : false;
            var response = reader.AddEditActionGoals(request);
            if (response.status & request.achieveGoal)
            {
                AwardIncentivesRequest incentivesRequest = new AwardIncentivesRequest();
                incentivesRequest.incentiveType = IncentiveTypes.Kit_Goal_Completion;
                incentivesRequest.userId = request.participantId;
                incentivesRequest.portalId = participantPortalId.Value;
                incentivesRequest.isEligible = true;
                incentivesRequest.pointsIncentiveMessage = IncentiveMessageTypes.Incentive;
                incentivesRequest.reference = request.kitsInUserProgramId.ToString();
                if (adminId.HasValue)
                    incentivesRequest.adminId = adminId.Value;
                IncentiveReader incentiveReader = new IncentiveReader();
                incentiveReader.AwardIncentives(incentivesRequest);
            }
            return response;
        }

        public static KitActionGoalsResponse GetKitsActionGoals(int? participantId)
        {
            KitReader reader = new KitReader();
            KitActionGoalsRequest request = new KitActionGoalsRequest();
            request.participantId = participantId.Value;
            return reader.GetKitsActionGoals(request);
        }

        public static bool AddKittoPrograms(int kitId, string organizationIds)
        {
            AccountReader _accountReader = new AccountReader();
            var userlist = _accountReader.FindUsers(new FindUsersRequest() { OrganizationIds = organizationIds?.Split(',')?.Select(Int32.Parse)?.ToList() }).Users.Where(x => x.IsActive && x.UsersinPrograms.Any(y => y.IsActive)).ToList();
            if (userlist != null && userlist.Count() > 0)
            {
                foreach (var user in userlist)
                {
                    var usersinProgram = user.UsersinPrograms.Where(x => x.IsActive).FirstOrDefault();
                    if (usersinProgram != null && !usersinProgram.KitsinUserPrograms.Any(x => x.KitId == kitId))
                    {
                        bool kitAlert = user.Organization.Portals.Where(x => x.Active).FirstOrDefault().KitAlert;
                        ProgramUtility.AddKittoUserProgram(usersinProgram.Id, kitId, user.LanguagePreference, user.Id, user.Organization.ContactEmail, user.Organization.ContactNumber, kitAlert);
                    }
                }
            }
            return true;
        }
    }
}