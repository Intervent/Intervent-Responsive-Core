using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class KitReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public IList<KitTopicsDto> GetKitTopics()
        {
            var kitTopics = context.KitTopics.Where(x => x.Active == true).ToList();
            return Utility.mapper.Map<IList<DAL.KitTopic>, IList<KitTopicsDto>>(kitTopics);
        }

        public void CloneKit(int kitId)
        {
            GetKitDetailsRequest kitRequest = new GetKitDetailsRequest();
            kitRequest.id = kitId;
            var kit = GetKitDetails(kitRequest);
            kit.kit.Id = 0;
            kit.kit.Name = kit.kit.Name + "-Copy";
            Dictionary<int, int> questionMapping = new Dictionary<int, int>();
            AddEditEduKitRequest addKitRequest = new AddEditEduKitRequest();
            addKitRequest.eduKit = kit.kit;
            var addeditResponse = AddEditEduKit(addKitRequest);
            addKitRequest.eduKit = addeditResponse.eduKit;
            addKitRequest.eduKit.Id = addeditResponse.KitId;
            AddEditEduKit(addKitRequest);
            List<PassiveQuestionsInActivitiesDto> passiveQuestionList = new List<PassiveQuestionsInActivitiesDto>();
            foreach (var prompt in kit.kit.PromptsinKits)
            {
                prompt.KitId = addeditResponse.KitId;
                prompt.Id = 0;
                AddEditPrompts(prompt);
            }
            foreach (var step in kit.kit.StepsinKits)
            {
                var stepId = step.Id;
                step.KitId = addeditResponse.KitId;
                step.Id = 0;
                AddEditStepinKitRequest addeditStepRequest = new AddEditStepinKitRequest();
                addeditStepRequest.stepsinKits = step;
                var addEditStepResponse = AddEditStepinKit(addeditStepRequest);
                foreach (var activity in step.ActivitiesinSteps)
                {
                    List<int> ids = new List<int>();
                    ids.Add(activity.Id);
                    activity.StepId = addEditStepResponse.StepId;
                    activity.Id = 0;
                    AddEditActivityinStepRequest addEditActitity = new AddEditActivityinStepRequest();
                    addEditActitity.activityinStep = activity;
                    var activityResponse = AddEditActivityinStep(addEditActitity);
                    var stepActivityResponse = ReadActivityinStepById(ids, null).FirstOrDefault();
                    if (stepActivityResponse != null)
                    {
                        foreach (var question in stepActivityResponse.QuestionsinActivities.Where(x => x.ParentId == null))
                        {
                            int oldQuetionId = question.Id;
                            int queId;
                            questionMapping.TryGetValue(oldQuetionId, out queId);
                            if (queId == 0)
                            {
                                question.ActivityId = activityResponse.activityId;
                                question.Id = 0;
                                AddEditQuestioninActivityRequest questionRequest = new AddEditQuestioninActivityRequest();
                                questionRequest.questioninActivity = question;
                                var questionResponse = AddEditQuestioninActivity(questionRequest);

                                if (!questionMapping.ContainsKey(oldQuetionId))
                                {
                                    questionMapping.Add(oldQuetionId, questionResponse.QuestionId);
                                }
                                foreach (var option in question.OptionsforActivityQuestions)
                                {
                                    var optionId = option.Id;
                                    option.Id = 0;
                                    option.QuestionId = questionResponse.QuestionId;
                                    AddEditOptionsRequest optionRequest = new AddEditOptionsRequest();
                                    optionRequest.OptionsForQuestions = option;
                                    var optionResponse = AddEditOptions(optionRequest);
                                    foreach (var subQuestion in stepActivityResponse.QuestionsinActivities.Where(x => x.ParentId == optionId))
                                    {
                                        int suboldQuetionId = question.Id;
                                        subQuestion.ActivityId = activityResponse.activityId;
                                        subQuestion.ParentId = optionResponse.id;
                                        subQuestion.Id = 0;
                                        AddEditQuestioninActivityRequest subquestionRequest = new AddEditQuestioninActivityRequest();
                                        subquestionRequest.questioninActivity = subQuestion;
                                        var subquestionResponse = AddEditQuestioninActivity(subquestionRequest);

                                        if (!questionMapping.ContainsKey(suboldQuetionId))
                                        {
                                            questionMapping.Add(suboldQuetionId, subquestionResponse.QuestionId);
                                        }
                                        foreach (var suboption in subQuestion.OptionsforActivityQuestions)
                                        {
                                            suboption.Id = 0;
                                            suboption.QuestionId = subquestionResponse.QuestionId;
                                            AddEditOptionsRequest suboptionRequest = new AddEditOptionsRequest();
                                            suboptionRequest.OptionsForQuestions = suboption;
                                            var suboptionResponse = AddEditOptions(suboptionRequest);
                                        }
                                    }
                                }
                            }
                        }
                        foreach (var passiveQuestion in stepActivityResponse.PassiveQuestionsInActivities)
                        {
                            passiveQuestion.ActivityId = activityResponse.activityId;
                            passiveQuestionList.Add(passiveQuestion);
                        }
                    }

                }
                var quizList = ReadQuizinKit(stepId, null);
                foreach (var quiz in quizList)
                {
                    quiz.Id = 0;
                    quiz.StepId = addEditStepResponse.StepId;
                    AddEditQuizRequest quizRequest = new AddEditQuizRequest();
                    quizRequest.quizinKit = quiz;
                    var quizResponse = AddEditQuiz(quizRequest);
                    foreach (var option in quiz.optionsforQuiz)
                    {
                        option.Id = 0;
                        option.QuizId = quizResponse.QuizId;
                        AddEditOptionsforQuizRequest quizrequest = new AddEditOptionsforQuizRequest();
                        quizrequest.optionforQuiz = option;
                        var optionResponse = AddEditOptionsforQuiz(quizrequest);
                    }
                }
            }
            foreach (var passive in passiveQuestionList)
            {
                if (questionMapping.ContainsKey(passive.QuestionId))
                {
                    passive.QuestionId = questionMapping[passive.QuestionId];
                    AddEditPassiveQuestionsInActivityRequest request = new AddEditPassiveQuestionsInActivityRequest();
                    request.PassiveQuestions = passive;
                    AddEditPassiveQuestionsInActivity(request);
                }
            }

        }

        public ListEduKitsResponse ListEduKits(ListEduKitsRequest request)
        {
            ListEduKitsResponse response = new ListEduKitsResponse();
            var totalRecords = request.totalRecords.HasValue ? request.totalRecords.Value : 0;
            List<Kit> kits = new List<Kit>();
            if (totalRecords == 0)
            {
                totalRecords = context.Kits.Include("KitTopic").Where(x => x.Active == true && (request.PortalId == 0 || x.Portal.Select(y => y.Id).Contains(request.PortalId)))
                     .OrderBy(k => k.KitTopic.Name).ThenBy(k => k.InvId.Length).ThenBy(k => k.InvId).Count();
            }
            if (!request.page.HasValue && !request.pageSize.HasValue)
                kits = context.Kits.Include("KitTopic").Where(x => x.Active == true && (request.PortalId == 0 || x.Portal.Select(y => y.Id).Contains(request.PortalId))).OrderBy(k => k.KitTopic.Name).ThenBy(k => k.InvId.Length).ThenBy(k => k.InvId).ToList();
            else
                kits = context.Kits.Include("KitTopic").Where(x => x.Active == true).OrderBy(k => k.KitTopic.Name).ThenBy(k => k.InvId.Length).ThenBy(k => k.InvId).OrderBy(x => x.Name).Skip(request.page.Value * request.pageSize.Value).Take(request.pageSize.Value).ToList();
            response.EduKits = Utility.mapper.Map<IList<DAL.Kit>, IList<KitsDto>>(kits);
            response.totalRecords = totalRecords;
            return response;
        }

        public List<KitsDto> ListKitsforPortal(int portalId)
        {
            var kits = context.Portals.Where(x => x.Id == portalId).Select(x => x.Kits.ToList()).FirstOrDefault();
            return Utility.mapper.Map<List<DAL.Kit>, List<KitsDto>>(kits);
        }

        public bool DeleteEduKit(int Id)
        {
            var kit = context.Kits.Where(x => x.Id == Id).FirstOrDefault();
            if (kit != null)
            {
                kit.Active = false;
                context.Kits.Attach(kit);
                context.Entry(kit).State = EntityState.Modified;
                context.SaveChanges();
            }
            return true;
        }

        public bool AddEditKitTranslation(KitTranslationDto request)
        {
            AddEditEduKitResponse response = new AddEditEduKitResponse();


            var kitTranslation = context.KitTranslations.Where(x => x.KitId == request.KitId && x.LanguageCode == request.LanguageCode).FirstOrDefault();
            if (kitTranslation != null)
            {
                kitTranslation.Name = request.Name;
                kitTranslation.Description = request.Description;
                kitTranslation.KeyConcepts = request.KeyConcepts;
                kitTranslation.Audio = request.Audio;
                kitTranslation.Pdf = request.Pdf;
                kitTranslation.PublishedDate = request.PublishedDate;
                kitTranslation.LastUpdated = request.LastUpdated;
                context.KitTranslations.Attach(kitTranslation);
                context.Entry(kitTranslation).State = EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                kitTranslation = new DAL.KitTranslation();

                kitTranslation.Name = request.Name;
                kitTranslation.Description = request.Description;
                kitTranslation.KeyConcepts = request.KeyConcepts;
                kitTranslation.Audio = request.Audio;
                kitTranslation.Pdf = request.Pdf;
                kitTranslation.KitId = request.KitId;
                kitTranslation.LanguageCode = request.LanguageCode;
                kitTranslation.PublishedDate = request.PublishedDate;
                kitTranslation.PublishedDate = request.LastUpdated;
                context.KitTranslations.Add(kitTranslation);
                context.SaveChanges();
            }
            return true;
        }


        public AddEditEduKitResponse AddEditEduKit(AddEditEduKitRequest request)
        {
            AddEditEduKitResponse response = new AddEditEduKitResponse();

            if (request.eduKit.Id > 0)
            {
                var kit = context.Kits.Where(x => x.Id == request.eduKit.Id).FirstOrDefault();
                if (kit != null)
                {
                    kit.Name = request.eduKit.Name;
                    kit.Description = request.eduKit.Description;
                    kit.Topic = request.eduKit.Topic;
                    kit.KeyConcepts = request.eduKit.KeyConcepts;
                    kit.Audio = request.eduKit.Audio;
                    kit.Pdf = request.eduKit.Pdf;
                    kit.InvId = request.eduKit.InvId;
                    kit.PublishedDate = request.eduKit.PublishedDate;
                    kit.LastUpdated = request.eduKit.LastUpdated;
                    context.Kits.Attach(kit);
                    context.Entry(kit).State = EntityState.Modified;
                    context.SaveChanges();
                    response.KitId = kit.Id;
                }
            }
            else
            {
                DAL.Kit kit = new DAL.Kit();

                kit.Name = request.eduKit.Name;
                kit.Description = request.eduKit.Description;
                kit.Topic = request.eduKit.Topic;
                kit.Active = true;
                kit.PublishedDate = request.eduKit.PublishedDate;
                kit.LastUpdated = request.eduKit.LastUpdated;
                context.Kits.Add(kit);
                context.SaveChanges();
                response.KitId = kit.Id;
            }
            response.eduKit = request.eduKit;
            response.success = true;
            return response;
        }

        public UploadKitPdfResponse UploadPdf(UploadKitPdfRequest request)
        {
            UploadKitPdfResponse response = new UploadKitPdfResponse();
            response.UploadedPdf = string.Empty;
            var kit = context.Kits.Where(x => x.Id == request.kitId).FirstOrDefault();
            if (kit != null)
            {
                response.UploadedPdf = kit.Pdf = string.IsNullOrEmpty(kit.Pdf) ? request.pdfUrl : (kit.Pdf + ";" + request.pdfUrl);
                context.Kits.Attach(kit);
                context.Entry(kit).State = EntityState.Modified;
                context.SaveChanges();
            }
            return response;
        }

        public UploadKitPdfResponse UploadTranslationPdf(UploadKitPdfRequest request)
        {
            UploadKitPdfResponse response = new UploadKitPdfResponse();
            response.UploadedPdf = string.Empty;
            var kit = context.KitTranslations.Where(x => x.KitId == request.kitId && x.LanguageCode == request.languageCode).FirstOrDefault();
            if (kit != null)
            {
                response.UploadedPdf = kit.Pdf = string.IsNullOrEmpty(kit.Pdf) ? request.pdfUrl : (kit.Pdf + ";" + request.pdfUrl);
                context.KitTranslations.Attach(kit);
                context.Entry(kit).State = EntityState.Modified;
                context.SaveChanges();
            }
            return response;
        }

        public string ReadKeyConcepts(int kitId, string languageCode)
        {
            var kit = context.Kits.Where(x => x.Id == kitId).FirstOrDefault();
            if (kit != null)
            {
                if (!string.IsNullOrEmpty(languageCode) && languageCode != ListOptions.DefaultLanguage)
                {
                    DAL.KitTranslation translation = context.KitTranslations.Where(k => k.KitId == kitId && k.LanguageCode == languageCode).FirstOrDefault();
                    if (translation != null)
                    {
                        return translation.KeyConcepts;
                    }
                }
                return kit.KeyConcepts;
            }
            return null;
        }

        public GetKitDetailsResponse GetKitDetails(GetKitDetailsRequest request)
        {
            GetKitDetailsResponse response = new GetKitDetailsResponse();
            if (!string.IsNullOrEmpty(request.language) && request.language != ListOptions.DefaultLanguage)
            {
                var kittranslation = context.KitTranslations.Where(x => x.KitId == request.id && x.LanguageCode == request.language).FirstOrDefault();
                response.KitTranslation = Utility.mapper.Map<DAL.KitTranslation, KitTranslationDto>(kittranslation);
                return response;
            }
            var kit = context.Kits.Include("StepsinKits").Include("StepsinKits.ActivitiesinSteps").Include("PromptsInKits").Where(x => x.Id == request.id).FirstOrDefault();
            response.kit = Utility.mapper.Map<DAL.Kit, KitsDto>(kit);
            return response;
        }

        #region Table

        public ListRowResponse AddRow(AddRowRequest request)
        {
            ListRowResponse response = new ListRowResponse();
            var options = context.OptionsforActivityQuestions.Where(x => x.QuestionId == request.ParentId && x.IsActive).ToList();
            int rowId = 1;
            if (options != null && options.Count() > 0)
            {
                rowId = int.Parse(options.LastOrDefault().OptionText) + 1;
            }
            AddEditOptionsRequest optionRequest = new AddEditOptionsRequest();
            optionRequest.OptionsForQuestions = new OptionsforActivityQuestionDto();
            optionRequest.OptionsForQuestions.OptionText = rowId.ToString();
            optionRequest.OptionsForQuestions.QuestionId = request.ParentId;
            var addEditResponse = AddEditOptions(optionRequest);
            response.Rows = addEditResponse.options;
            return response;
        }

        public ListColumnResponse ReadColumns(int rowId)
        {
            return GetColumns(rowId);
        }

        public ListRowResponse ReadRows(int questionId)
        {
            ListRowResponse response = new ListRowResponse();
            var optionList = context.OptionsforActivityQuestions.Where(x => x.QuestionId == questionId && x.IsActive).ToList();
            if (optionList != null)
                response.Rows = Utility.mapper.Map<IList<DAL.OptionsforActivityQuestion>, IList<OptionsforActivityQuestionDto>>(optionList);
            if (response.Rows != null && response.Rows.Count() > 0)
            {
                //userinput has default sequenceno 1
                var userInput = response.Rows.Where(x => x.SequenceNo.HasValue);
                if (userInput.Count() > 0)
                {
                    response.RowCount = userInput.Count();
                    var parentId = userInput.FirstOrDefault().Id;
                    var userInputQuestions = context.QuestionsinActivities.Where(x => x.ParentId == parentId).ToList();
                    response.ColumnCount = userInputQuestions.Count();
                }
            }
            return response;
        }


        public ListColumnResponse AddEditColumn(AddEditColumnRequest request)
        {
            AddEditQuestioninActivityRequest questionRequest = new AddEditQuestioninActivityRequest();
            questionRequest.questioninActivity = new QuestionsinActivityDto();
            questionRequest.language = request.Language;
            questionRequest.questioninActivity.ActivityId = request.Column.ActivityId;
            questionRequest.questioninActivity.ParentId = request.Column.RowId;
            questionRequest.questioninActivity.Id = request.Column.ColumnId;
            questionRequest.questioninActivity.IsActive = true;
            questionRequest.questioninActivity.QuestionText = request.Column.QuestionText;
            if (request.Column.QuestionText.StartsWith(CommonConstants.TableOptionText))
            {
                questionRequest.questioninActivity.SequenceNo = 1;
            }
            if (request.Column.QuestionType.HasValue)
            {
                questionRequest.questioninActivity.QuestionType = request.Column.QuestionType.Value;
            }
            var questionResponse = AddEditQuestioninActivity(questionRequest);
            List<OptionsforActivityQuestion> existingOptions = new List<OptionsforActivityQuestion>();
            OptionsforActivityQuestionDto rowSpanOption, columnSpanOption, styleOption;
            rowSpanOption = columnSpanOption = styleOption = null;

            rowSpanOption = new OptionsforActivityQuestionDto();
            rowSpanOption.SequenceNo = CommonConstants.RowSpanOption;
            rowSpanOption.OptionText = request.Column.RowSpan.ToString();
            rowSpanOption.QuestionId = questionResponse.QuestionId;

            columnSpanOption = new OptionsforActivityQuestionDto();
            columnSpanOption.SequenceNo = CommonConstants.ColSpanOption;
            columnSpanOption.OptionText = request.Column.ColumnSpan.ToString();
            columnSpanOption.QuestionId = questionResponse.QuestionId;

            styleOption = new OptionsforActivityQuestionDto();
            styleOption.SequenceNo = CommonConstants.StyleOption;
            styleOption.OptionText = request.Column.Style;
            styleOption.QuestionId = questionResponse.QuestionId;

            if (request.Column.ColumnId > 0)
            {
                existingOptions = context.OptionsforActivityQuestions.Where(x => x.QuestionId == request.Column.ColumnId && x.IsActive).ToList();
                if (existingOptions.Count() > 0)
                {
                    var rowSpan = existingOptions.Where(x => x.SequenceNo == CommonConstants.RowSpanOption).FirstOrDefault();
                    if (rowSpan != null)
                    {
                        rowSpanOption.Id = rowSpan.Id;

                    }
                    var columnSpan = existingOptions.Where(x => x.SequenceNo == CommonConstants.ColSpanOption).FirstOrDefault();
                    if (columnSpan != null)
                    {
                        columnSpanOption.Id = columnSpan.Id;
                    }

                    var style = existingOptions.Where(x => x.SequenceNo == CommonConstants.StyleOption).FirstOrDefault();
                    if (style != null)
                    {
                        styleOption.Id = style.Id;
                    }
                }
            }

            StoredProcedures sp = new StoredProcedures();
            sp.CreateOptions(rowSpanOption.QuestionId, rowSpanOption.Id, rowSpanOption.SequenceNo.Value, rowSpanOption.OptionText);
            sp.CreateOptions(columnSpanOption.QuestionId, columnSpanOption.Id, columnSpanOption.SequenceNo.Value, columnSpanOption.OptionText);
            sp.CreateOptions(styleOption.QuestionId, styleOption.Id, styleOption.SequenceNo.Value, styleOption.OptionText);

            return GetColumns(request.Column.RowId);
        }

        private ListColumnResponse GetColumns(int rowId)
        {
            ListColumnResponse response = new ListColumnResponse();
            LanguageReader langReader = new LanguageReader();
            List<ColumnDto> columns = new List<ColumnDto>();
            using (var newContext = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                var questions = newContext.QuestionsinActivities.Where(x => x.ParentId == rowId).ToList();
                foreach (var question in questions)
                {
                    ColumnDto column = new ColumnDto();
                    column.RowId = rowId;
                    column.ColumnId = question.Id;
                    if (!string.IsNullOrEmpty(question.TextLangItemCode))
                        column.LanguageTextValue = langReader.GetLanguageItemByCode(question.TextLangItemCode);
                    column.QuestionText = question.QuestionText;
                    column.QuestionType = question.QuestionType;
                    column.ActivityId = question.ActivityId;
                    var options = newContext.OptionsforActivityQuestions.Where(x => x.QuestionId == question.Id && x.IsActive);
                    if (options.Count() > 0)
                    {
                        var rowSpan = options.Where(x => x.SequenceNo == CommonConstants.RowSpanOption).FirstOrDefault();
                        if (rowSpan != null)
                        {
                            column.RowSpan = short.Parse(rowSpan.OptionText);
                        }
                        var columnSpan = options.Where(x => x.SequenceNo == CommonConstants.ColSpanOption).FirstOrDefault();
                        if (columnSpan != null)
                        {
                            column.ColumnSpan = short.Parse(columnSpan.OptionText);
                        }

                        var style = options.Where(x => x.SequenceNo == CommonConstants.StyleOption).FirstOrDefault();
                        if (style != null)
                        {
                            column.Style = style.OptionText;
                        }
                    }
                    columns.Add(column);
                }
                response.Columns = columns;
            }
            response.Columns = columns;
            return response;
        }

        public AddEditTableOptionResponse AddEditTableOption(AddEditTableOptionRequest request)
        {
            AddEditTableOptionResponse response = new AddEditTableOptionResponse();
            StoredProcedures sp = new StoredProcedures();
            //
            if (request.Row > 0)
            {
                for (var i = 0; i < request.Row; i++)
                {
                    var optText = string.Format("User Input {0}", (i + 1));
                    var optionResponse = sp.CreateOptions(request.ParentId, null, 1, optText);
                    //
                    for (int col = 0; col < request.Column; col++)
                    {
                        var questionText = string.Format("{0}Row{1}Col{2}", Intervent.Web.DataLayer.CommonConstants.TableOptionText, i, col);
                        sp.InsertQuestions(request.ActivityId, questionText, (int)QuestionType.textbox, true, optionResponse);
                    }
                }
            }
            response.IsSuccess = true;
            return response;
        }
        #endregion

        #region TableCheckbox

        public void SetColSpan(int parentQuestionId, short colSpan)
        {
            AddChkOptionRow("", parentQuestionId, colSpan);
        }

        private int AddChkOptionRow(string language, int parentQuestionId, short? colSpan)
        {
            var option = context.OptionsforActivityQuestions.Where(x => x.QuestionId == parentQuestionId && x.OptionText == CommonConstants.TableOptionText && x.IsActive).FirstOrDefault();
            if (option == null)
            {
                AddEditOptionsRequest optionRequest = new AddEditOptionsRequest();
                optionRequest.OptionsForQuestions = new OptionsforActivityQuestionDto();
                optionRequest.language = language;
                optionRequest.OptionsForQuestions.OptionText = CommonConstants.TableOptionText;
                optionRequest.OptionsForQuestions.QuestionId = parentQuestionId;
                short sequence = 1;
                if (colSpan.HasValue)
                {
                    sequence = colSpan.Value;
                }

                optionRequest.OptionsForQuestions.SequenceNo = sequence;
                var addEditResponse = AddEditOptions(optionRequest);
                return addEditResponse.id;
            }
            else if (colSpan.HasValue && option.SequenceNo.HasValue && option.SequenceNo.Value != colSpan.Value)
            {
                StoredProcedures sp = new StoredProcedures();
                sp.CreateOptions(parentQuestionId, option.Id, colSpan.Value, CommonConstants.TableOptionText);

            }
            return option.Id;

        }

        public TableCheckboxResponse AddTableChkBox(AddEditTableCheckboxRequest request)
        {
            TableCheckboxResponse response = new TableCheckboxResponse();
            StoredProcedures sp = new StoredProcedures();
            AddEditQuestioninActivityRequest questionRequest = new AddEditQuestioninActivityRequest();
            questionRequest.questioninActivity = new QuestionsinActivityDto();
            questionRequest.questioninActivity.ParentId = AddChkOptionRow(request.Language, request.ParentQuestionId, null);
            //
            if (!request.IsOption)
            {
                AddEditOptionsRequest optionRequest = new AddEditOptionsRequest();
                optionRequest.OptionsForQuestions = new OptionsforActivityQuestionDto();
                optionRequest.OptionsForQuestions.Id = request.Question.IndexId;
                optionRequest.language = request.Language;
                optionRequest.OptionsForQuestions.OptionText = request.Question.IndexText;
                optionRequest.OptionsForQuestions.QuestionId = request.ParentQuestionId;
                optionRequest.OptionsForQuestions.SequenceNo = request.Question.Sequence;
                var addEditResponse = AddEditOptions(optionRequest);
                questionRequest.questioninActivity.ParentId = addEditResponse.id;
            }
            else
            {
                short sequence = 1;
                if (request.Question.Sequence.HasValue)
                    sequence = request.Question.Sequence.Value;
                questionRequest.questioninActivity.SequenceNo = sequence;
                questionRequest.questioninActivity.QuestionType = (byte)QuestionType.checkbox;
            }
            //
            questionRequest.language = request.Language;
            questionRequest.questioninActivity.ActivityId = request.ActivityId;
            questionRequest.questioninActivity.Id = request.Question.QuestionId;
            questionRequest.questioninActivity.IsActive = true;
            questionRequest.questioninActivity.QuestionText = request.Question.QuestionText;
            var questionResponse = AddEditQuestioninActivity(questionRequest);
            //if (request.Question.ColSpan.HasValue && request.Question.ColSpan.Value > 0)
            //{
            //    var columnSpanOption = new OptionsforActivityQuestionDto();
            //    columnSpanOption.SequenceNo = CommonConstants.ColSpanOption;
            //    columnSpanOption.OptionText = request.Question.ColSpan.Value.ToString();
            //    columnSpanOption.QuestionId = questionResponse.QuestionId;
            //    if (request.Question.QuestionId > 0)
            //    {
            //        var columnSpan = context.OptionsforActivityQuestions.Where(x => x.QuestionId == request.Question.QuestionId && x.SequenceNo == CommonConstants.ColSpanOption).FirstOrDefault();
            //        //
            //        if (columnSpan != null)
            //        {
            //            columnSpanOption.Id = columnSpan.Id;
            //        }
            //    }
            //    sp.CreateOptions(columnSpanOption.QuestionId, columnSpanOption.Id, columnSpanOption.SequenceNo.Value, columnSpanOption.OptionText);

            //}
            //if (!string.IsNullOrEmpty(request.Question.style))
            //{
            //    var styleOption = new OptionsforActivityQuestionDto();
            //    styleOption.SequenceNo = CommonConstants.StyleOption;
            //    styleOption.OptionText = request.Question.style;
            //    styleOption.QuestionId = questionResponse.QuestionId;
            //    if (request.Question.QuestionId > 0)
            //    {
            //        var style = context.OptionsforActivityQuestions.Where(x => x.QuestionId == request.Question.QuestionId && x.SequenceNo == CommonConstants.StyleOption).FirstOrDefault();
            //        //
            //        if (style != null)
            //        {
            //            styleOption.Id = style.Id;
            //        }
            //    }
            //    sp.CreateOptions(styleOption.QuestionId, styleOption.Id, styleOption.SequenceNo.Value, styleOption.OptionText);

            //}
            var tableChkResponse = ReadTableChkBox(request.ParentQuestionId);
            InsertOptions(tableChkResponse, request, questionResponse.QuestionId);
            return tableChkResponse;
        }

        private void InsertOptions(TableCheckboxResponse response, AddEditTableCheckboxRequest request, int questionId)
        {
            StoredProcedures sp = new StoredProcedures();
            //option 
            if (request.Question.QuestionId <= 0)
            {
                if (request.IsOption)
                {
                    foreach (var ques in response.Questions)
                    {
                        short sequence = 0;
                        if (request.Question.Sequence.HasValue)
                        {
                            sequence = request.Question.Sequence.Value;
                        }
                        sp.CreateOptions(ques.QuestionId, null, sequence, request.Question.QuestionText);
                    }
                }
                else
                {
                    foreach (var opt in response.Options)
                    {
                        short sequence = 0;
                        if (opt.Sequence.HasValue)
                        {
                            sequence = opt.Sequence.Value;
                        }
                        sp.CreateOptions(questionId, null, sequence, opt.QuestionText);
                    }
                }
            }
        }

        public TableCheckboxResponse ReadTableChkBox(int questionId)
        {
            TableCheckboxResponse response = new TableCheckboxResponse();
            LanguageReader langReader = new LanguageReader();
            response.Questions = new List<ChkColumnDto>();
            response.Options = new List<ChkColumnDto>();
            var options = context.OptionsforActivityQuestions.Where(x => x.QuestionId == questionId && x.IsActive).ToList();
            foreach (var option in options)
            {
                if (option.OptionText == CommonConstants.TableOptionText)
                {
                    response.IndexId = option.Id;
                    response.ColSpan = option.SequenceNo.Value;
                    var chkOptions = context.QuestionsinActivities.Where(x => x.ParentId == option.Id && x.IsActive).ToList();
                    //
                    foreach (var chk in chkOptions)
                    {
                        ChkColumnDto ques = new ChkColumnDto();
                        ques.IndexId = option.Id;
                        ques.QuestionId = chk.Id;
                        ques.IndexText = CommonConstants.TableOptionText;
                        ques.QuestionText = chk.QuestionText;
                        ques.Sequence = chk.SequenceNo;
                        if (!string.IsNullOrEmpty(chk.TextLangItemCode))
                            ques.LanguageTextValue = langReader.GetLanguageItemByCode(chk.TextLangItemCode);
                        response.Options.Add(ques);
                    }
                }
                else
                {
                    ChkColumnDto ques = new ChkColumnDto();
                    ques.IndexId = option.Id;
                    ques.IndexText = option.OptionText;
                    var quest = context.QuestionsinActivities.Where(x => x.ParentId == option.Id && x.IsActive).FirstOrDefault();
                    ques.QuestionId = quest.Id;
                    ques.QuestionText = quest.QuestionText;
                    ques.Sequence = quest.SequenceNo;
                    if (!string.IsNullOrEmpty(quest.TextLangItemCode))
                        ques.LanguageTextValue = langReader.GetLanguageItemByCode(quest.TextLangItemCode);
                    if (!string.IsNullOrEmpty(option.TextLangItemCode))
                        ques.IndexTextValue = langReader.GetLanguageItemByCode(option.TextLangItemCode);

                    //if(quest.OptionsforActivityQuestions != null && quest.OptionsforActivityQuestions.Count() > 0)
                    //{
                    //    var columnSpan = quest.OptionsforActivityQuestions.Where(x => x.SequenceNo == CommonConstants.ColSpanOption).FirstOrDefault();
                    //    if(columnSpan != null)
                    //    {
                    //        ques.ColSpan = short.Parse(columnSpan.OptionText);
                    //    }
                    //    var style = quest.OptionsforActivityQuestions.Where(x => x.SequenceNo == CommonConstants.StyleOption).FirstOrDefault();
                    //    if (style != null)
                    //    {
                    //        ques.style = style.OptionText;
                    //    }
                    //}
                    response.Questions.Add(ques);
                }
            }
            return response;
        }
        #endregion

        public AddEditStepinKitResponse AddEditStepinKit(AddEditStepinKitRequest request)
        {
            AddEditStepinKitResponse response = new AddEditStepinKitResponse();
            if (request.stepsinKits.Id > 0)
            {
                var stepinKit = context.StepsinKits.Where(x => x.Id == request.stepsinKits.Id).FirstOrDefault();
                if (stepinKit != null)
                {
                    if (!string.IsNullOrEmpty(request.language) && request.language != ListOptions.DefaultLanguage)
                    {
                        if (string.IsNullOrEmpty(stepinKit.TextLangItemCode))
                            stepinKit.TextLangItemCode = LanguageType.ST.ToString() + stepinKit.Id;
                        if (string.IsNullOrEmpty(stepinKit.NameLangItemCode))
                            stepinKit.NameLangItemCode = LanguageType.SN.ToString() + stepinKit.Id;
                        Dictionary<string, string> codes = new Dictionary<string, string>();
                        codes.Add(stepinKit.TextLangItemCode, request.stepsinKits.Text);
                        codes.Add(stepinKit.NameLangItemCode, request.stepsinKits.Name);
                        LanguageReader reader = new LanguageReader();
                        reader.SaveLanguageItems(codes, request.language);
                    }
                    else
                    {
                        stepinKit.Text = request.stepsinKits.Text;
                        stepinKit.StepNo = request.stepsinKits.StepNo;
                        stepinKit.Name = request.stepsinKits.Name;
                        stepinKit.IsActive = request.stepsinKits.IsActive;
                        stepinKit.IsSubStep = request.stepsinKits.IsSubStep;
                        stepinKit.IsAppendix = request.stepsinKits.IsAppendix;
                        stepinKit.IsGoal = request.stepsinKits.IsGoal;
                    }
                    context.StepsinKits.Attach(stepinKit);
                    context.Entry(stepinKit).State = EntityState.Modified;
                    context.SaveChanges();
                    response.StepId = stepinKit.Id;
                }
            }
            else
            {
                DAL.StepsinKit stepinKit = new DAL.StepsinKit();
                stepinKit.Text = request.stepsinKits.Text;
                stepinKit.StepNo = request.stepsinKits.StepNo;
                stepinKit.Name = request.stepsinKits.Name;
                stepinKit.KitId = request.stepsinKits.KitId;
                stepinKit.IsActive = request.stepsinKits.IsActive;
                stepinKit.IsSubStep = request.stepsinKits.IsSubStep;
                stepinKit.IsAppendix = request.stepsinKits.IsAppendix;
                stepinKit.IsGoal = request.stepsinKits.IsGoal;
                using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                {
                    context1.StepsinKits.Add(stepinKit);
                    context1.SaveChanges();
                }
                response.StepId = stepinKit.Id;
            }
            response.success = true;
            return response;
        }

        public ReadStepinKitResponse ReadStepinKit(ReadStepinKitRequest request)
        {
            ReadStepinKitResponse response = new ReadStepinKitResponse();
            var step = context.StepsinKits.Where(x => x.Id == request.id).FirstOrDefault();
            response.stepinKit = Utility.mapper.Map<DAL.StepsinKit, StepsinKitsDto>(step);
            if (response.stepinKit != null)
            {
                LanguageReader reader = new LanguageReader();
                if (!request.readlanguage)
                {
                    var codes = new List<string>();
                    if (!string.IsNullOrEmpty(step.TextLangItemCode))
                        codes.Add(step.TextLangItemCode);
                    if (!string.IsNullOrEmpty(step.NameLangItemCode))
                        codes.Add(step.NameLangItemCode);
                    if (codes.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(request.language) && request.language != ListOptions.DefaultLanguage)
                        {
                            var langItems = reader.GetLanguageItems(codes, request.language);
                            if (langItems != null && langItems.Count > 0)
                            {
                                var item = langItems.Find(l => l.ItemCode == step.NameLangItemCode);
                                if (item != null)
                                    response.stepinKit.Name = item.Text;
                                item = langItems.Find(l => l.ItemCode == step.TextLangItemCode);
                                if (item != null)
                                    response.stepinKit.Text = item.Text;
                            }
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(step.NameLangItemCode))
                    {
                        if (!string.IsNullOrEmpty(step.TextLangItemCode))
                        {
                            response.stepinKit.TextLanguageValue = reader.GetLanguageItemByCode(step.TextLangItemCode);
                        }
                        if (!string.IsNullOrEmpty(step.NameLangItemCode))
                        {
                            response.stepinKit.NameLanguageValue = reader.GetLanguageItemByCode(step.NameLangItemCode);
                        }
                    }
                }
            }
            return response;
        }

        public ReadStepsinKitListResponse StepsinKitListGet(ReadStepsinKitListRequest request)
        {
            ReadStepsinKitListResponse response = new ReadStepsinKitListResponse();
            List<StepsinKitsDto> stepsList = new List<StepsinKitsDto>();
            var stepsinKit = context.StepsinKits.Where(x => x.KitId == request.id && x.IsActive).ToList();
            foreach (var step in stepsinKit)
            {
                stepsList.Add(Utility.mapper.Map<DAL.StepsinKit, StepsinKitsDto>(step));
            }
            response.StepsinKits = stepsList;
            return response;
        }

        public ReadActivityinStepResponse ReadActivityinStep(ReadActivityinStepRequest request)
        {
            ReadActivityinStepResponse response = new ReadActivityinStepResponse();
            LanguageReader langReader = new LanguageReader();
            var step = context.ActivitiesinSteps.Include("QuestionsinActivities").Include("QuestionsinActivities.OptionsforActivityQuestions").Where(x => x.Id == request.ActivityId).FirstOrDefault();
            if (step != null)
            {
                response.Activity = Utility.mapper.Map<DAL.ActivitiesinStep, ActivitiesinStepsDto>(step);
                if (!string.IsNullOrEmpty(step.TopTextLangItemCode))
                    response.Activity.TopTextLanguageValue = langReader.GetLanguageItemByCode(step.TopTextLangItemCode);
                if (!string.IsNullOrEmpty(step.TopTextLangItemCode))
                    response.Activity.BottomTextLanguageValue = langReader.GetLanguageItemByCode(step.BottomTextLangItemCode);

                if (response.Activity.QuestionsinActivities != null)
                {
                    foreach (var question in response.Activity.QuestionsinActivities)
                    {
                        question.QuestionTypeText = ((QuestionType)question.QuestionType).ToString();
                        if (!string.IsNullOrEmpty(question.TextLangItemCode))
                            question.LanguageTextValue = langReader.GetLanguageItemByCode(question.TextLangItemCode);
                        foreach (var option in question.OptionsforActivityQuestions)
                        {
                            if (!string.IsNullOrEmpty(option.TextLangItemCode))
                                option.LanguageTextValue = langReader.GetLanguageItemByCode(option.TextLangItemCode);

                        }
                    }
                }
            }
            response.PassiveQuestions = ReadPassiveQuestions(response.Activity.Id);
            foreach (var passiveQues in response.PassiveQuestions)
            {
                if (!string.IsNullOrEmpty(passiveQues.TextLangItemCode))
                    passiveQues.LanguageTextValue = langReader.GetLanguageItemByCode(passiveQues.TextLangItemCode);

            }
            return response;
        }

        public List<ActivitiesinStepsDto> ReadActivityinStepById(List<int> ids, string languageCode)
        {
            var actList = context.ActivitiesinSteps.Include("QuestionsinActivities").Include("PassiveQuestionsInActivities").Include("QuestionsinActivities.OptionsforActivityQuestions").Where(x => ids.Contains(x.Id));
            var activityList = new List<ActivitiesinStepsDto>();
            if (actList != null)
            {
                IList<ActivitiesinStep> activities = actList.ToList();
                if (!string.IsNullOrEmpty(languageCode))
                {
                    activities = TranslateActivity(languageCode, activities);
                }

                foreach (var activity in activities)
                {
                    activity.QuestionsinActivities = activity.QuestionsinActivities.Where(q => q.IsActive).ToList();
                    activityList.Add(Utility.mapper.Map<DAL.ActivitiesinStep, ActivitiesinStepsDto>(activity));
                }
            }
            return activityList;
        }
        private IList<ActivitiesinStep> TranslateActivity(string languageCode, IList<ActivitiesinStep> activities)
        {
            #region Language Mapping
            Dictionary<string, ActivitiesinStep> bottomActivity = new Dictionary<string, ActivitiesinStep>();
            Dictionary<string, ActivitiesinStep> topActivity = new Dictionary<string, ActivitiesinStep>();
            Dictionary<string, QuestionsinActivity> textQuestion = new Dictionary<string, QuestionsinActivity>();
            Dictionary<string, PassiveQuestionsInActivity> textPassive = new Dictionary<string, PassiveQuestionsInActivity>();
            Dictionary<string, OptionsforActivityQuestion> textOptions = new Dictionary<string, OptionsforActivityQuestion>();
            List<string> langCode = new List<string>();
            foreach (var activity in activities)
            {
                if (!string.IsNullOrEmpty(activity.BottomTextLangItemCode))
                {
                    langCode.Add(activity.BottomTextLangItemCode);
                    bottomActivity.Add(activity.BottomTextLangItemCode, activity);
                }
                if (!string.IsNullOrEmpty(activity.TopTextLangItemCode))
                {
                    langCode.Add(activity.TopTextLangItemCode);
                    topActivity.Add(activity.TopTextLangItemCode, activity);
                }
                foreach (var question in activity.QuestionsinActivities)
                {
                    if (!string.IsNullOrEmpty(question.TextLangItemCode))
                    {
                        langCode.Add(question.TextLangItemCode);
                        textQuestion.Add(question.TextLangItemCode, question);
                    }
                    foreach (var option in question.OptionsforActivityQuestions)
                    {
                        if (!string.IsNullOrEmpty(option.TextLangItemCode))
                        {
                            langCode.Add(option.TextLangItemCode);
                            textOptions.Add(option.TextLangItemCode, option);
                        }
                    }
                }
                foreach (var question in activity.PassiveQuestionsInActivities)
                {
                    if (!string.IsNullOrEmpty(question.TextLangItemCode))
                    {
                        langCode.Add(question.TextLangItemCode);
                        textPassive.Add(question.TextLangItemCode, question);
                    }
                }
            }
            LanguageReader langReader = new LanguageReader();
            var languageItems = langReader.GetLanguageItemDict(langCode, languageCode);

            foreach (var activity in bottomActivity)
            {
                if (languageItems.ContainsKey(activity.Key))
                {
                    activity.Value.BottomText = languageItems[activity.Key].Text;
                }
            }
            foreach (var activity in topActivity)
            {
                if (languageItems.ContainsKey(activity.Key))
                {
                    activity.Value.TopText = languageItems[activity.Key].Text;
                }
            }
            foreach (var question in textQuestion)
            {
                if (languageItems.ContainsKey(question.Key))
                {
                    question.Value.QuestionText = languageItems[question.Key].Text;
                }
            }
            foreach (var question in textPassive)
            {
                if (languageItems.ContainsKey(question.Key))
                {
                    question.Value.QuestionText = languageItems[question.Key].Text;
                }
            }
            foreach (var option in textOptions)
            {
                if (languageItems.ContainsKey(option.Key))
                {
                    option.Value.OptionText = languageItems[option.Key].Text;
                }
            }
            #endregion

            return activities;
        }

        public KitsinUserProgram GetKitsInUserProgramId(int kitsInUserProgramId)
        {
            return context.KitsinUserPrograms
                       .Where(x => x.Id == kitsInUserProgramId).FirstOrDefault();
        }

        public AddUserChoiceResponse AddUserOptions(AddUserChoiceRequest request, string DTCOrgCode)
        {
            AddUserChoiceResponse response = new AddUserChoiceResponse();
            if (request.KitsInUserProgramsId > 0)
            {
                if (request.UserAnswer != null)
                {
                    if (!request.IsQuiz)
                    {
                        foreach (var option in request.UserAnswer)
                        {
                            int questionId = int.Parse(option.Text);
                            var choice = context.UserChoices.Where(x => x.KitsInUserProgramsId == request.KitsInUserProgramsId && x.QuestionId == questionId).FirstOrDefault();
                            if (choice != null)
                            {
                                if (string.IsNullOrEmpty(option.Value))
                                {
                                    context.UserChoices.Remove(choice);
                                }
                                else
                                {
                                    choice.Value = option.Value;
                                    context.UserChoices.Attach(choice);
                                    context.Entry(choice).State = EntityState.Modified;
                                }
                                context.SaveChanges();
                            }
                            else if (!string.IsNullOrEmpty(option.Value))
                            {
                                choice = new UserChoice();
                                choice.QuestionId = questionId;
                                choice.KitsInUserProgramsId = request.KitsInUserProgramsId;
                                choice.Value = option.Value;
                                choice.DateCreated = DateTime.UtcNow;
                                context.UserChoices.Add(choice);
                                context.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        foreach (var option in request.UserAnswer)
                        {
                            int quizId = int.Parse(option.Text);
                            var choice = context.UserQuizChoices.Where(x => x.KitsInUserProgramsId == request.KitsInUserProgramsId && x.QuizId == quizId).FirstOrDefault();
                            if (choice != null)
                            {
                                choice.Value = option.Value;
                                context.UserQuizChoices.Attach(choice);
                                context.Entry(choice).State = EntityState.Modified;
                                context.SaveChanges();
                            }
                            else
                            {
                                choice = new UserQuizChoice();
                                choice.QuizId = quizId;
                                choice.KitsInUserProgramsId = request.KitsInUserProgramsId;
                                choice.Value = option.Value;
                                choice.DateCreated = DateTime.UtcNow;
                                context.UserQuizChoices.Add(choice);
                                context.SaveChanges();
                            }
                        }
                    }
                }
                if (request.PercentComplete > 0)
                {
                    var kitsinUserProgram = context.KitsinUserPrograms.Include("UsersinProgram").Include("UsersinProgram.ProgramsinPortal").Include("UsersinProgram.User").Include("UsersinProgram.User.Organization").Where(x => x.Id == request.KitsInUserProgramsId).FirstOrDefault();
                    if (kitsinUserProgram != null)
                    {
                        kitsinUserProgram.UpdatedBy = request.UpdatedBy;
                        kitsinUserProgram.UpdatedOn = DateTime.UtcNow;
                        if (kitsinUserProgram.PercentCompleted < 100)
                            kitsinUserProgram.PercentCompleted = request.PercentComplete;
                        if (request.PercentComplete == 100 && !kitsinUserProgram.CompleteDate.HasValue)
                        {
                            kitsinUserProgram.CompleteDate = DateTime.UtcNow;
                        }
                        if (request.IsIntuityUser && kitsinUserProgram.UsersinProgram.UserId == request.ParticipantId)
                        {
                            var educationalModuleEvent = context.IntuityEvents.Where(x => x.UserId == kitsinUserProgram.UsersinProgram.UserId && x.EventType == (int)IntuityEventTypes.Educational_Module_Update && x.EventDate.Value.Date == kitsinUserProgram.UpdatedOn.Value.Date).ToList();
                            if (educationalModuleEvent.Count() == 0)
                            {
                                IntuityReader intuityReader = new IntuityReader();
                                AddIntuityEventRequest intuityEventRequest = new AddIntuityEventRequest();
                                intuityEventRequest.intuityEvent = new IntuityEventDto();
                                intuityEventRequest.intuityEvent.UserId = kitsinUserProgram.UsersinProgram.UserId;
                                intuityEventRequest.organizationCode = kitsinUserProgram.UsersinProgram.User.Organization.Code;
                                intuityEventRequest.intuityEvent.UniqueId = kitsinUserProgram.UsersinProgram.User.UniqueId;
                                intuityEventRequest.intuityEvent.EventType = (int)IntuityEventTypes.Educational_Module_Update;
                                intuityEventRequest.intuityEvent.CreatedBy = request.UpdatedBy;
                                intuityEventRequest.intuityEvent.EventDate = kitsinUserProgram.UpdatedOn;
                                intuityReader.AddIntuityEvent(intuityEventRequest, DTCOrgCode);
                            }
                        }
                        context.KitsinUserPrograms.Attach(kitsinUserProgram);
                        context.Entry(kitsinUserProgram).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
            }
            response.success = true;
            return response;
        }

        public const char Separator = '-'; //hyphens
        public const char ActivitySeparator = '_'; //underscore

        public GetKitByIdResponse ReviewKit(GetReviewKitRequest request)
        {
            GetKitByIdResponse response = new GetKitByIdResponse();
            var kitsInUserProgram = GetKitsInUserProgramId(request.kitsInUserProgramId);
            if (kitsInUserProgram != null)
            {
                var kit = context.Kits.Include("KitTopic")
                       .Select(c => new KitsDto
                       {
                           Id = c.Id,
                           KeyConcepts = c.KeyConcepts,
                           Description = c.Description,
                           Audio = c.Audio,
                           Name = c.Name,
                           KitTopic = new KitTopicsDto
                           {
                               Color = c.KitTopic.Color
                           },
                           Pdf = c.Pdf,
                           StepsinKits = c.StepsinKits.Where(x => x.IsActive).Select(sk =>
                           new StepsinKitsDto
                           {
                               Id = sk.Id,
                               KitId = sk.KitId,
                               Text = sk.Text,
                               IsActive = sk.IsActive,
                               IsSubStep = sk.IsSubStep,
                               IsAppendix = sk.IsAppendix,
                               IsGoal = sk.IsGoal,
                               StepNo = sk.StepNo,
                               Name = sk.Name,
                               ActivitiesinSteps = sk.ActivitiesinSteps.Where(x => x.IsActive).Select(qa => new ActivitiesinStepsDto
                               {
                                   Id = qa.Id,
                                   IsActive = qa.IsActive,
                                   AllowUpdate = qa.AllowUpdate,
                                   SequenceNo = qa.SequenceNo,
                                   WithinStep = qa.WithinStep,
                                   BottomText = qa.BottomText,
                                   TopText = qa.TopText,
                                   StepId = qa.StepId,
                                   PassiveQuestionsInActivities = qa.PassiveQuestionsInActivities.Where(x => x.IsActive).Select(pq => new PassiveQuestionsInActivitiesDto
                                   {
                                       IsActive = pq.IsActive,
                                       QuestionId = pq.QuestionId,
                                       QuestionText = pq.QuestionText,
                                       SequenceNo = pq.SequenceNo,
                                       ActivityId = pq.ActivityId
                                   }).OrderBy(qs => qs.SequenceNo).ToList(),
                                   QuestionsinActivities = qa.QuestionsinActivities.Where(x => x.IsActive).Select(qs => new QuestionsinActivityDto
                                   {
                                       Id = qs.Id,
                                       IsActive = qs.IsActive,
                                       ActivityId = qs.ActivityId,
                                       QuestionText = qs.QuestionText,
                                       QuestionType = qs.QuestionType,
                                       ShowVertical = qs.ShowVertical,
                                       SequenceNo = qs.SequenceNo,
                                       ParentId = qs.ParentId,
                                       OptionsforActivityQuestions = qs.OptionsforActivityQuestions.Select(op => new OptionsforActivityQuestionDto
                                       {
                                           IsActive = op.IsActive,
                                           Id = op.Id,
                                           IsAnswer = op.IsAnswer,
                                           OptionText = op.OptionText,
                                           Points = op.Points,
                                           QuestionId = op.QuestionId,
                                           SequenceNo = op.SequenceNo
                                       }).Where(ops => (ops.IsActive.HasValue && ops.IsActive.Value)).OrderBy(ops => ops.SequenceNo).ThenBy(ops => ops.Id).ToList(),
                                   }).OrderBy(qs => qs.SequenceNo).ToList(),
                               }).OrderBy(qa => qa.SequenceNo).ToList(),
                               QuizInSteps = sk.QuizinSteps.Where(x => x.IsActive).Select(qz => new QuizinStepDto
                               {
                                   Id = qz.Id,
                                   StepId = qz.StepId,
                                   IsActive = qz.IsActive,
                                   QuizText = qz.QuizText,
                                   QuizType = qz.QuizType,
                                   optionsforQuiz = qz.OptionsforQuizs.Where(x => x.IsActive).Select(oq => new OptionsforQuizDto
                                   {
                                       Id = oq.Id,
                                       IsActive = oq.IsActive,
                                       IsDefault = oq.IsDefault,
                                       OptionText = oq.OptionText,
                                       QuizId = oq.QuizId
                                   }).ToList(),
                               }).OrderBy(qz => qz.Id).ToList(),
                           }).OrderBy(sk => sk.StepNo).ToList(),
                           PromptsinKits = c.PromptsInKits.Where(x => x.IsActive).Select(pk =>
                           new PromptDto
                           {
                               Id = pk.Id,
                               KitId = pk.KitId,
                               Description = pk.Description,
                               IsActive = pk.IsActive,
                               IsBottom = pk.IsBottom,
                               RefId = pk.RefId,
                               RefType = pk.RefType,
                               DateUpdated = pk.DateUpdated,
                               DisplayOrder = pk.DisplayOrder
                           }).OrderBy(pk => pk.DisplayOrder).ThenBy(pk => pk.DateUpdated).ToList(),
                       })
                       .Where(x => x.Id == kitsInUserProgram.KitId).FirstOrDefault();


                var promptsinKitsCompleted = GetPromptsinKitsCompletedList(request.kitsInUserProgramId);
                if (promptsinKitsCompleted != null && promptsinKitsCompleted.Count > 0)
                    kit.PromptsinKitsCompleted = promptsinKitsCompleted;

                response.EduKit = kit;
                response.KitsInUserProgramId = request.kitsInUserProgramId;
                response.PercentCompleted = kitsInUserProgram.PercentCompleted;
                ProgramReader reader = new ProgramReader();
                foreach (var step in response.EduKit.StepsinKits.Where(x => x.IsActive))
                {
                    foreach (var activity in step.ActivitiesinSteps.Where(x => x.IsActive))
                        reader.PopulateQuestionValue(activity, response.KitsInUserProgramId);
                    reader.PopulateQuizValue(step.QuizInSteps, response.KitsInUserProgramId);
                }

                response.KitColor = kit.KitTopic.Color;
                response.KitsinUserProgramGoal = GetKitsActionGoals(new KitActionGoalsRequest { participantId = request.userId }).KitsinUserProgramGoals.Where(x => x.KitsinUserProgramId == request.kitsInUserProgramId).FirstOrDefault();
            }
            return response;
        }

        public GetKitByIdResponse GetKitById(GetKitByIdRequest request)
        {
            GetKitByIdResponse response = new GetKitByIdResponse();
            string language = string.Empty;
            if (request.preview != true)
            {
                var UsersinProgram = context.KitsinUserPrograms.Include("UsersinProgram").Where(x => x.UsersinProgramsId == request.userinProgramId && x.KitId == request.kitId && x.Id == request.kitsInUserProgramId && x.IsActive == true).FirstOrDefault();
                if (UsersinProgram == null)
                {
                    response.success = false;
                    return response;
                }
                if (UsersinProgram.UsersinProgram.Language != ListOptions.DefaultLanguage)
                {
                    language = UsersinProgram.UsersinProgram.Language;
                }
            }
            else if (!string.IsNullOrEmpty(request.PreviewLanguage))
            {
                language = request.PreviewLanguage;
            }
            var kitStep = context.Kits.Include("KitTopic")
                       .Select(c => new
                       {
                           kit = c,
                           kitTopic = c.KitTopic,
                           StepsinKits = c.StepsinKits.Select(sk =>
                           new
                           {
                               StepId = sk.Id,
                               IsActive = sk.IsActive,
                               IsSubStep = sk.IsSubStep,
                               IsAppendix = sk.IsAppendix,
                               IsGoal = sk.IsGoal,
                               StepNo = sk.StepNo,
                               StepName = sk.Name,
                               NameLangCode = sk.NameLangItemCode,
                               ActivitiesinSteps = sk.ActivitiesinSteps.Select(qa => new
                               {
                                   ActivityId = qa.Id,
                                   IsActive = qa.IsActive,
                                   Sequence = qa.SequenceNo,
                                   WithinStep = qa.WithinStep
                               }).OrderBy(act => act.Sequence).ToList(),
                               QuizInStep = sk.QuizinSteps,
                           }).OrderBy(sk => sk.StepNo).ToList()
                       })
                       .Where(x => x.kit.Id == request.kitId).FirstOrDefault();

            //Translate language
            #region Translate Language
            if (!string.IsNullOrEmpty(language) && kitStep.kit != null)
            {
                DAL.KitTranslation translation = context.KitTranslations.Where(k => k.KitId == request.kitId && k.LanguageCode == language).FirstOrDefault();
                if (translation != null)
                {
                    kitStep.kit.Name = translation.Name;
                    kitStep.kit.Description = translation.Description;
                    kitStep.kit.KeyConcepts = translation.KeyConcepts;
                    kitStep.kit.Pdf = translation.Pdf;
                    kitStep.kit.Audio = translation.Audio;
                }
            }
            #endregion

            if (kitStep != null && kitStep.StepsinKits != null)
            {
                response.LanguageCode = language;
                response.StepNames = new List<Step>();
                response.PageIdentifier = new List<PageIdentifier>();
                List<dynamic> appendixList = new List<dynamic>();
                List<dynamic> goalList = new List<dynamic>();
                foreach (var step in kitStep.StepsinKits)
                {
                    if (step.IsActive)
                    {
                        if (step.IsAppendix) appendixList.Add(step);
                        else if (step.IsGoal) goalList.Add(step);
                        else
                            PopulateStepDetails(step, response);
                    }
                    if (step.QuizInStep != null && step.QuizInStep.Where(q => q.IsActive).Count() > 0)
                        response.PageIdentifier.Add(new PageIdentifier(step.StepId.ToString(), response.StepNames[response.StepNames.Count - 1].PageIdentifier));
                }

                if (goalList.Count > 0)
                {
                    foreach (var goal in goalList)
                        PopulateStepDetails(goal, response);
                }
                response.TotalPages = response.PageIdentifier.Count;

                if (!string.IsNullOrEmpty(kitStep.kit.KeyConcepts))
                {
                    response.PageIdentifier.Add(new PageIdentifier("KC"));
                    response.StepNames.Add(new Step("Key Concepts", "KC", response.PageIdentifier.Count, ""));
                }

                if (appendixList.Count > 0)
                {
                    foreach (var appendix in appendixList)
                        PopulateStepDetails(appendix, response);
                }

                response.EduKit = Utility.mapper.Map<DAL.Kit, KitsDto>(kitStep.kit);
                response.KitColor = kitStep.kitTopic.Color;

                #region Translate Step
                if (!string.IsNullOrEmpty(language))
                {
                    List<string> langCode = new List<string>();
                    Dictionary<string, Step> stepsinKits = new Dictionary<string, Step>();
                    foreach (var step in response.StepNames)
                    {
                        if (!string.IsNullOrEmpty(step.NameLangCode))
                        {
                            langCode.Add(step.NameLangCode);
                            stepsinKits.Add(step.NameLangCode, step);
                        }
                    }
                    LanguageReader langReader = new LanguageReader();
                    var languageItems = langReader.GetLanguageItemDict(langCode, language);

                    foreach (var langItem in languageItems)
                    {
                        var step = stepsinKits[langItem.Key];
                        step.StepName = langItem.Value.Text;
                    }
                }
                #endregion
            }

            if (request.kitsInUserProgramId.HasValue)
            {
                var kitsInUserProgram = GetKitsInUserProgramId(request.kitsInUserProgramId.Value);
                if (kitsInUserProgram != null)
                {
                    response.KitsInUserProgramId = kitsInUserProgram.Id;
                    response.PercentCompleted = kitsInUserProgram.PercentCompleted;
                    response.ListenedAudio = kitsInUserProgram.ListenedAudio;
                    var currentIndex = (response.PercentCompleted * (response.PageIdentifier.Count + 1)) / 100;
                    foreach (var step in response.StepNames)
                        if (step.Index > currentIndex)
                            step.HideLink = true;
                }
            }
            response.success = true;
            return response;
        }

        private void PopulateStepDetails(dynamic step, GetKitByIdResponse response)
        {
            string separator = Separator.ToString();
            Step currentStep = null;
            if (!step.IsSubStep)
            {
                if (!step.IsAppendix && !step.IsGoal)
                    currentStep = new Step(string.Format("Section {0}: {1}", step.StepNo, step.StepName), step.IsAppendix, step.NameLangCode);
                else
                    currentStep = new Step(step.StepName, step.IsAppendix, step.NameLangCode);
                response.StepNames.Add(currentStep);
            }

            var identifier = step.StepId.ToString() + separator;
            IEnumerable<dynamic> activityList = step.ActivitiesinSteps;
            var activities = activityList.OrderBy(a => a.Sequence);
            if (activities != null && activities.Count() > 0)
            {
                foreach (var activity in activities)
                {
                    if (activity != null && activity.IsActive)
                    {
                        if (!activity.WithinStep)
                        {
                            if (!response.ConatinsIdentifier(identifier))
                            {
                                response.PageIdentifier.Add(new PageIdentifier(identifier));
                                if (currentStep != null && string.IsNullOrEmpty(currentStep.PageIdentifier))
                                {
                                    currentStep.PageIdentifier = identifier;
                                    currentStep.Index = response.PageIdentifier.Count;
                                }
                            }
                            identifier = separator + activity.ActivityId + ActivitySeparator.ToString();
                        }
                        else
                        {
                            identifier += activity.ActivityId.ToString() + ActivitySeparator.ToString();
                        }
                    }
                }

            }
            if (!response.ConatinsIdentifier(identifier))
            {
                response.PageIdentifier.Add(new PageIdentifier(identifier));
                if (currentStep != null && string.IsNullOrEmpty(currentStep.PageIdentifier))
                {
                    currentStep.PageIdentifier = identifier;
                    currentStep.Index = response.PageIdentifier.Count;
                }
            }

        }

        public SearchQuestionsResponse SearchQuestion(SearchQuestionsRequest request)
        {
            SearchQuestionsResponse response = new SearchQuestionsResponse();
            List<PassiveQuestionsDTO> PassiveQuestions = new List<PassiveQuestionsDTO>();
            var kitStep = context.Kits
                       .Select(c => new
                       {
                           KitId = c.Id,
                           StepsinKits = c.StepsinKits.Select(sk =>
                           new
                           {
                               StepName = sk.Name,
                               ActivitiesinSteps = sk.ActivitiesinSteps.Select(qa => new
                               {
                                   ActivityId = qa.Id,
                                   Questions = qa.QuestionsinActivities.Select(q => new
                                   {
                                       QuestionText = q.QuestionText,
                                       QuestionType = q.QuestionType,
                                       QuestionId = q.Id,
                                       IsActive = q.IsActive
                                   }).Where(q => q.QuestionText.Contains(request.searchText) && q.IsActive && (!request.questionType.HasValue || q.QuestionType == request.questionType.Value))
                               }).Where(a => !request.activityId.HasValue || (request.activityId.HasValue && a.ActivityId == request.activityId.Value))
                           })

                       })
                       .Where(x => x.KitId == request.kitId).FirstOrDefault();
            if (kitStep != null && kitStep.StepsinKits.Count() > 0)
            {
                foreach (var step in kitStep.StepsinKits)
                {
                    if (step != null && step.ActivitiesinSteps.Count() > 0)
                    {
                        foreach (var activity in step.ActivitiesinSteps)
                        {
                            if (activity != null && activity.Questions.Count() > 0)
                            {
                                foreach (var question in activity.Questions)
                                {
                                    PassiveQuestionsDTO dto = new PassiveQuestionsDTO
                                    {
                                        QuestionId = question.QuestionId,
                                        QuestionText = question.QuestionText,
                                        QuestionTypeText = ((QuestionType)question.QuestionType).ToString(),
                                        StepName = step.StepName
                                    };
                                    PassiveQuestions.Add(dto);
                                }
                            }
                        }
                    }
                }
            }
            response.PassiveQuestions = PassiveQuestions;
            return response;
        }

        public GetStepWithActivityResponse GetStepWithActivity(GetStepWithActivityRequest request)
        {
            GetStepWithActivityResponse response = new GetStepWithActivityResponse();
            StepsinKitsDto StepsinKits = new StepsinKitsDto();
            var step = context.StepsinKits.Include("ActivitiesinSteps").Include("ActivitiesinSteps.PassiveQuestionsInActivities").Include("ActivitiesinSteps.QuestionsinActivities").Include("ActivitiesinSteps.QuestionsinActivities.OptionsforActivityQuestions").Where(x => x.Id == request.stepId && x.IsActive).FirstOrDefault();
            if (step != null)
            {
                if (!string.IsNullOrEmpty(request.languageCode))
                {
                    List<string> langCodes = new List<string>();
                    if (!string.IsNullOrEmpty(step.NameLangItemCode))
                        langCodes.Add(step.NameLangItemCode);
                    if (!string.IsNullOrEmpty(step.TextLangItemCode))
                        langCodes.Add(step.TextLangItemCode);
                    if (langCodes.Count > 0)
                    {
                        LanguageReader langReader = new LanguageReader();
                        var languageItems = langReader.GetLanguageItemDict(langCodes, request.languageCode);
                        if (!string.IsNullOrEmpty(step.NameLangItemCode) && languageItems.ContainsKey(step.NameLangItemCode))
                        {
                            step.Name = languageItems[step.NameLangItemCode].Text;
                        }
                        if (!string.IsNullOrEmpty(step.TextLangItemCode) && languageItems.ContainsKey(step.TextLangItemCode))
                        {
                            step.Text = languageItems[step.TextLangItemCode].Text;
                        }
                    }
                    TranslateActivity(request.languageCode, step.ActivitiesinSteps.ToList());
                }
                step.ActivitiesinSteps = step.ActivitiesinSteps.Where(a => request.activityIds.Contains(a.Id)).ToList();
                StepsinKits = Utility.mapper.Map<DAL.StepsinKit, StepsinKitsDto>(step);
            }
            response.stepsinKits = StepsinKits;
            return response;
        }

        public AddEditActivityinStepResponse AddEditActivityinStep(AddEditActivityinStepRequest request)
        {
            AddEditActivityinStepResponse response = new AddEditActivityinStepResponse();
            if (request.activityinStep.Id > 0)
            {
                var questioninActivity = context.ActivitiesinSteps.Where(x => x.Id == request.activityinStep.Id).FirstOrDefault();
                if (questioninActivity != null)
                {
                    if (!string.IsNullOrEmpty(request.language) && request.language != ListOptions.DefaultLanguage)
                    {
                        if (string.IsNullOrEmpty(questioninActivity.TopTextLangItemCode))
                            questioninActivity.TopTextLangItemCode = LanguageType.AT.ToString() + questioninActivity.Id;
                        if (string.IsNullOrEmpty(questioninActivity.BottomTextLangItemCode))
                            questioninActivity.BottomTextLangItemCode = LanguageType.AB.ToString() + questioninActivity.Id;
                        Dictionary<string, string> codes = new Dictionary<string, string>();
                        codes.Add(questioninActivity.TopTextLangItemCode, request.activityinStep.TopText);
                        codes.Add(questioninActivity.BottomTextLangItemCode, request.activityinStep.BottomText);
                        LanguageReader reader = new LanguageReader();
                        reader.SaveLanguageItems(codes, request.language);
                    }
                    else
                    {
                        questioninActivity.TopText = request.activityinStep.TopText;
                        questioninActivity.BottomText = request.activityinStep.BottomText;
                        questioninActivity.WithinStep = request.activityinStep.WithinStep;
                        questioninActivity.IsActive = request.activityinStep.IsActive;
                        questioninActivity.AllowUpdate = request.activityinStep.AllowUpdate;
                        questioninActivity.SequenceNo = short.Parse(request.activityinStep.SequenceNo.ToString());
                    }
                    context.ActivitiesinSteps.Attach(questioninActivity);
                    context.Entry(questioninActivity).State = EntityState.Modified;
                    context.SaveChanges();
                    response.activityId = questioninActivity.Id;
                }
            }
            else
            {
                DAL.ActivitiesinStep questioninActivity = new DAL.ActivitiesinStep();
                questioninActivity.TopText = request.activityinStep.TopText;
                questioninActivity.BottomText = request.activityinStep.BottomText;
                questioninActivity.WithinStep = request.activityinStep.WithinStep;
                questioninActivity.StepId = request.activityinStep.StepId;
                questioninActivity.IsActive = request.activityinStep.IsActive;
                questioninActivity.AllowUpdate = request.activityinStep.AllowUpdate;
                questioninActivity.SequenceNo = short.Parse(request.activityinStep.SequenceNo.ToString());
                using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                {
                    context1.ActivitiesinSteps.Add(questioninActivity);
                    context1.SaveChanges();
                }
                response.activityId = questioninActivity.Id;
            }
            response.success = true;
            return response;
        }

        public AddEditQuestionResponse AddEditQuestioninActivity(AddEditQuestioninActivityRequest request)
        {
            AddEditQuestionResponse response = new AddEditQuestionResponse();
            if (request.questioninActivity.Id > 0)
            {
                var questioninActivity = context.QuestionsinActivities.Where(x => x.Id == request.questioninActivity.Id).FirstOrDefault();
                if (questioninActivity != null)
                {
                    if (!string.IsNullOrEmpty(request.language) && request.language != ListOptions.DefaultLanguage)
                    {
                        if (string.IsNullOrEmpty(questioninActivity.TextLangItemCode))
                            questioninActivity.TextLangItemCode = LanguageType.QET.ToString() + questioninActivity.Id;
                        Dictionary<string, string> codes = new Dictionary<string, string>();
                        codes.Add(questioninActivity.TextLangItemCode, request.questioninActivity.QuestionText);
                        LanguageReader reader = new LanguageReader();
                        reader.SaveLanguageItems(codes, request.language);
                        questioninActivity.OptionsforActivityQuestion = null;
                    }
                    else
                    {
                        questioninActivity.QuestionText = request.questioninActivity.QuestionText;
                        questioninActivity.QuestionType = request.questioninActivity.QuestionType;
                        questioninActivity.SequenceNo = request.questioninActivity.SequenceNo;
                        questioninActivity.IsActive = request.questioninActivity.IsActive;
                        questioninActivity.IsRequired = request.questioninActivity.IsRequired;
                        questioninActivity.ShowVertical = request.questioninActivity.ShowVertical;
                        questioninActivity.ParentId = request.questioninActivity.ParentId;
                        questioninActivity.OptionsforActivityQuestion = null;
                    }
                    context.QuestionsinActivities.Attach(questioninActivity);
                    context.Entry(questioninActivity).State = EntityState.Modified;
                    context.SaveChanges();
                    response.QuestionId = questioninActivity.Id;
                }
            }
            else
            {
                DAL.QuestionsinActivity questioninActivity = new DAL.QuestionsinActivity();
                questioninActivity.QuestionText = request.questioninActivity.QuestionText;
                questioninActivity.QuestionType = request.questioninActivity.QuestionType;
                questioninActivity.ActivityId = request.questioninActivity.ActivityId;
                questioninActivity.SequenceNo = request.questioninActivity.SequenceNo;
                questioninActivity.IsActive = request.questioninActivity.IsActive;
                questioninActivity.ShowVertical = request.questioninActivity.ShowVertical;
                questioninActivity.ParentId = request.questioninActivity.ParentId;
                questioninActivity.OptionsforActivityQuestion = null;
                using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                {
                    context1.QuestionsinActivities.Add(questioninActivity);
                    context1.SaveChanges();
                }
                response.QuestionId = questioninActivity.Id;
            }
            response.success = true;
            response.questionsinActivity = ReadQuestionsInActivty(request.questioninActivity.ActivityId);
            response.passiveQuestions = ReadPassiveQuestions(request.questioninActivity.ActivityId);
            return response;
        }

        public IList<QuestionsinActivityDto> ReadQuestionsInActivty(int activityId)
        {
            IList<QuestionsinActivityDto> questionsDto = null;
            var questions = context.QuestionsinActivities.Include("OptionsforActivityQuestions").Where(x => x.ActivityId == activityId && x.IsActive).ToList();
            if (questions != null)
                questionsDto = Utility.mapper.Map<IList<DAL.QuestionsinActivity>, IList<QuestionsinActivityDto>>(questions);
            if (questionsDto != null)
            {
                foreach (var question in questionsDto)
                    question.QuestionTypeText = ((QuestionType)question.QuestionType).ToString();
            }

            return questionsDto;
        }


        //public List<PassiveQuestionsDTO>
        public AddEditOptionsResponse AddEditOptions(AddEditOptionsRequest request)
        {
            AddEditOptionsResponse response = new AddEditOptionsResponse();
            response.id = request.OptionsForQuestions.Id;
            if (request.OptionsForQuestions.IsAnswer.HasValue && request.OptionsForQuestions.IsAnswer.Value)
            {
                var option = context.OptionsforActivityQuestions.Where(x => x.QuestionId == request.OptionsForQuestions.QuestionId && x.IsAnswer.HasValue && x.IsAnswer.Value && x.IsActive).FirstOrDefault();
                if (option != null)
                {
                    option.IsAnswer = false;
                    context.OptionsforActivityQuestions.Attach(option);
                    context.Entry(option).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            if (request.OptionsForQuestions.Id > 0)
            {
                var options = context.OptionsforActivityQuestions.Where(x => x.Id == request.OptionsForQuestions.Id && x.IsActive).FirstOrDefault();
                if (options != null)
                {
                    if (!string.IsNullOrEmpty(request.language) && request.language != ListOptions.DefaultLanguage)
                    {
                        if (string.IsNullOrEmpty(options.TextLangItemCode))
                            options.TextLangItemCode = LanguageType.OT.ToString() + options.Id;
                        Dictionary<string, string> codes = new Dictionary<string, string>();
                        codes.Add(options.TextLangItemCode, request.OptionsForQuestions.OptionText);
                        LanguageReader reader = new LanguageReader();
                        reader.SaveLanguageItems(codes, request.language);
                    }
                    else
                    {
                        options.OptionText = request.OptionsForQuestions.OptionText;
                        options.IsAnswer = request.OptionsForQuestions.IsAnswer;
                        options.SequenceNo = request.OptionsForQuestions.SequenceNo;
                        options.Points = request.OptionsForQuestions.Points;
                        if (request.OptionsForQuestions.IsActive.HasValue)
                        {
                            options.IsActive = request.OptionsForQuestions.IsActive.Value;
                        }
                        else
                        {
                            options.IsActive = true;
                        }
                    }
                    context.OptionsforActivityQuestions.Attach(options);
                    context.Entry(options).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else
            {
                var options = new DAL.OptionsforActivityQuestion();
                options.OptionText = !String.IsNullOrEmpty(request.OptionsForQuestions.OptionText) ? request.OptionsForQuestions.OptionText : "&nbsp;";
                options.QuestionId = request.OptionsForQuestions.QuestionId;
                options.IsAnswer = request.OptionsForQuestions.IsAnswer;
                options.SequenceNo = request.OptionsForQuestions.SequenceNo;
                options.Points = request.OptionsForQuestions.Points;
                if (request.OptionsForQuestions.IsActive.HasValue)
                {
                    options.IsActive = request.OptionsForQuestions.IsActive.Value;
                }
                else
                {
                    options.IsActive = true;
                }
                 using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                 {
                     context1.OptionsforActivityQuestions.Add(options);
                     context1.SaveChanges();
                 }
                response.id = options.Id;
            }
            response.success = true;
            var optionList = context.OptionsforActivityQuestions.Where(x => x.QuestionId == request.OptionsForQuestions.QuestionId && x.IsActive).ToList();
            if (optionList != null)
                response.options = Utility.mapper.Map<IList<DAL.OptionsforActivityQuestion>, IList<OptionsforActivityQuestionDto>>(optionList);
            return response;
        }

        public AddEditQuestionResponse AddEditPassiveQuestionsInActivity(AddEditPassiveQuestionsInActivityRequest request)
        {
            AddEditQuestionResponse response = new AddEditQuestionResponse();

            var passiveQuestion = context.PassiveQuestionsInActivities.Where(x => x.QuestionId == request.PassiveQuestions.QuestionId && x.ActivityId == request.PassiveQuestions.ActivityId).FirstOrDefault();
            if (passiveQuestion != null)
            {
                if (!string.IsNullOrEmpty(request.language) && request.language != ListOptions.DefaultLanguage)
                {
                    if (string.IsNullOrEmpty(passiveQuestion.TextLangItemCode))
                        passiveQuestion.TextLangItemCode = LanguageType.PQET.ToString() + passiveQuestion.ActivityId + "-" + passiveQuestion.QuestionId;
                    Dictionary<string, string> codes = new Dictionary<string, string>();
                    codes.Add(passiveQuestion.TextLangItemCode, request.PassiveQuestions.QuestionText);
                    LanguageReader reader = new LanguageReader();
                    reader.SaveLanguageItems(codes, request.language);
                }
                else
                {
                    passiveQuestion.SequenceNo = request.PassiveQuestions.SequenceNo;
                    passiveQuestion.QuestionText = request.PassiveQuestions.QuestionText;
                    passiveQuestion.IsActive = request.PassiveQuestions.IsActive;
                }
                context.PassiveQuestionsInActivities.Attach(passiveQuestion);
                context.Entry(passiveQuestion).State = EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                passiveQuestion = new DAL.PassiveQuestionsInActivity();
                passiveQuestion.ActivityId = request.PassiveQuestions.ActivityId;
                passiveQuestion.QuestionId = request.PassiveQuestions.QuestionId;
                passiveQuestion.SequenceNo = request.PassiveQuestions.SequenceNo;
                passiveQuestion.QuestionText = request.PassiveQuestions.QuestionText;
                passiveQuestion.IsActive = request.PassiveQuestions.IsActive;
                context.PassiveQuestionsInActivities.Add(passiveQuestion);
                context.SaveChanges();
            }
            response.success = true;
            response.passiveQuestions = ReadPassiveQuestions(request.PassiveQuestions.ActivityId);
            response.questionsinActivity = ReadQuestionsInActivty(request.PassiveQuestions.ActivityId);
            return response;
        }

        public IList<PassiveQuestionsInActivitiesDto> ReadPassiveQuestions(int activityId)
        {
            var passiveQuestions = context.PassiveQuestionsInActivities.Where(x => x.ActivityId == activityId && x.IsActive).ToList();
            if (passiveQuestions != null)
                return Utility.mapper.Map<IList<DAL.PassiveQuestionsInActivity>, IList<PassiveQuestionsInActivitiesDto>>(passiveQuestions);
            return null;

        }

        public IList<QuizinStepDto> ReadQuizinKit(int stepId, string languageCode)
        {
            var quiz = context.QuizinSteps.Include("OptionsforQuizs").Where(x => x.StepId == stepId && x.IsActive).ToList();
            if (quiz != null)
            {
                var quizDto = Utility.mapper.Map<IList<DAL.QuizinStep>, IList<QuizinStepDto>>(quiz);
                if (quizDto != null)
                {
                    LanguageReader langReader = new LanguageReader();
                    if (string.IsNullOrEmpty(languageCode))
                    {
                        foreach (var q in quizDto)
                        {
                            q.QuizTypeText = ((QuestionType)q.QuizType).ToString();
                            if (!string.IsNullOrEmpty(q.TextLangItemCode))
                                q.LanguageTextValue = langReader.GetLanguageItemByCode(q.TextLangItemCode);
                            foreach (var o in q.optionsforQuiz)
                            {
                                if (!string.IsNullOrEmpty(o.TextLangItemCode))
                                    o.LanguageTextValue = langReader.GetLanguageItemByCode(o.TextLangItemCode);
                            }
                        }
                    }
                    else if (languageCode != ListOptions.DefaultLanguage)
                    {
                        List<string> itemCodes = new List<string>();
                        Dictionary<string, QuizinStepDto> quizList = new Dictionary<string, QuizinStepDto>();
                        Dictionary<string, OptionsforQuizDto> optList = new Dictionary<string, OptionsforQuizDto>();
                        foreach (var q in quizDto)
                        {
                            q.QuizTypeText = ((QuestionType)q.QuizType).ToString();
                            if (!string.IsNullOrEmpty(q.TextLangItemCode))
                            {
                                quizList.Add(q.TextLangItemCode, q);
                                itemCodes.Add(q.TextLangItemCode);
                            }
                            foreach (var o in q.optionsforQuiz)
                            {
                                if (!string.IsNullOrEmpty(o.TextLangItemCode))
                                {
                                    itemCodes.Add(o.TextLangItemCode);
                                    optList.Add(o.TextLangItemCode, o);
                                }
                            }
                        }

                        var languageItems = langReader.GetLanguageItems(itemCodes, languageCode);
                        foreach (var item in languageItems)
                        {
                            if (!string.IsNullOrEmpty(item.Text))
                            {
                                if (quizList.ContainsKey(item.ItemCode))
                                {
                                    quizList[item.ItemCode].QuizText = item.Text;
                                }
                                else if (optList.ContainsKey(item.ItemCode))
                                {
                                    optList[item.ItemCode].OptionText = item.Text;
                                }
                            }
                        }

                    }
                }
                return quizDto;
            }
            return null;
        }

        public AddEditQuizResponse AddEditQuiz(AddEditQuizRequest request)
        {
            AddEditQuizResponse response = new AddEditQuizResponse();
            if (request.quizinKit.Id > 0)
            {
                var quizinKit = context.QuizinSteps.Where(x => x.Id == request.quizinKit.Id).FirstOrDefault();
                if (quizinKit != null)
                {
                    if (!string.IsNullOrEmpty(request.language) && request.language != ListOptions.DefaultLanguage)
                    {
                        if (string.IsNullOrEmpty(quizinKit.TextLangItemCode))
                            quizinKit.TextLangItemCode = LanguageType.QZT.ToString() + quizinKit.Id;
                        Dictionary<string, string> codes = new Dictionary<string, string>();
                        codes.Add(quizinKit.TextLangItemCode, request.quizinKit.QuizText);
                        LanguageReader reader = new LanguageReader();
                        reader.SaveLanguageItems(codes, request.language);
                    }
                    else
                    {
                        quizinKit.QuizText = request.quizinKit.QuizText;
                        quizinKit.QuizType = request.quizinKit.QuizType;
                        quizinKit.IsActive = request.quizinKit.IsActive;
                    }
                    context.QuizinSteps.Attach(quizinKit);
                    context.Entry(quizinKit).State = EntityState.Modified;
                    context.SaveChanges();
                    response.QuizId = quizinKit.Id;
                }
            }
            else
            {
                DAL.QuizinStep quizinKit = new DAL.QuizinStep();
                quizinKit.QuizText = request.quizinKit.QuizText;
                quizinKit.QuizType = request.quizinKit.QuizType;
                quizinKit.StepId = request.quizinKit.StepId;
                quizinKit.IsActive = request.quizinKit.IsActive;
                using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                {
                    context1.QuizinSteps.Add(quizinKit);
                    context1.SaveChanges();
                }
                response.QuizId = quizinKit.Id;
            }
            response.success = true;
            var quizs = context.QuizinSteps.Include("OptionsforQuizs").Where(x => x.StepId == request.quizinKit.StepId).ToList();
            if (quizs != null)
                response.quizinKit = Utility.mapper.Map<IList<DAL.QuizinStep>, IList<QuizinStepDto>>(quizs);
            if (response.quizinKit != null)
            {
                foreach (var quiz in response.quizinKit)
                    quiz.QuizTypeText = ((QuestionType)quiz.QuizType).ToString();
            }
            return response;
        }

        public AddEditOptionsforQuizResponse AddEditOptionsforQuiz(AddEditOptionsforQuizRequest request)
        {
            AddEditOptionsforQuizResponse response = new AddEditOptionsforQuizResponse();
            if (request.optionforQuiz.IsDefault)
            {
                var option = context.OptionsforQuizs.Where(x => x.QuizId == request.optionforQuiz.QuizId && x.IsDefault).FirstOrDefault();
                if (option != null)
                {
                    option.IsDefault = false;
                    context.OptionsforQuizs.Attach(option);
                    context.Entry(option).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            if (request.optionforQuiz.Id > 0)
            {
                var options = context.OptionsforQuizs.Where(x => x.Id == request.optionforQuiz.Id).FirstOrDefault();
                if (options != null)
                {
                    if (!string.IsNullOrEmpty(request.language) && request.language != ListOptions.DefaultLanguage)
                    {
                        if (string.IsNullOrEmpty(options.TextLangItemCode))
                            options.TextLangItemCode = LanguageType.QZOT.ToString() + options.Id;
                        Dictionary<string, string> codes = new Dictionary<string, string>();
                        codes.Add(options.TextLangItemCode, request.optionforQuiz.OptionText);
                        LanguageReader reader = new LanguageReader();
                        reader.SaveLanguageItems(codes, request.language);
                    }
                    else
                    {
                        options.OptionText = request.optionforQuiz.OptionText;
                        options.IsActive = request.optionforQuiz.IsActive;
                        options.IsDefault = request.optionforQuiz.IsDefault;
                    }
                    context.OptionsforQuizs.Attach(options);
                    context.Entry(options).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else
            {
                var options = new DAL.OptionsforQuiz();
                options.OptionText = request.optionforQuiz.OptionText;
                options.QuizId = request.optionforQuiz.QuizId;
                options.IsActive = request.optionforQuiz.IsActive;
                options.IsDefault = request.optionforQuiz.IsDefault;
                 using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                 {
                     context1.OptionsforQuizs.Add(options);
                     context1.SaveChanges();
                 }
            }
            response.success = true;
            var optionList = context.OptionsforQuizs.Where(x => x.QuizId == request.optionforQuiz.QuizId).ToList();
            if (optionList != null)
                response.optionsforQuiz = Utility.mapper.Map<IList<DAL.OptionsforQuiz>, IList<OptionsforQuizDto>>(optionList);
            return response;
        }

        public ListAssignedKitsResponse ListAssignedKits(ListAssignedKitsRequest request)
        {
            ListAssignedKitsResponse response = new ListAssignedKitsResponse();
            var KitsinUserProgram = context.KitsinUserPrograms.Include("UsersinProgram").Include("Kit").Where(x => x.UsersinProgram.UserId == request.UserId).ToList();
            response.KitsinUserPrograms = Utility.mapper.Map<IList<DAL.KitsinUserProgram>, IList<KitsinUserProgramDto>>(KitsinUserProgram);
            return response;
        }

        public bool AddEditPrompts(PromptDto prompt)
        {
            var promptinKitsDAL = context.PromptsInKits.Where(x => x.Id == prompt.Id).FirstOrDefault();
            if (promptinKitsDAL != null)
            {
                var updatesPromptinKitsDAL = Utility.mapper.Map<PromptDto, PromptsInKit>(prompt);
                context.Entry(promptinKitsDAL).CurrentValues.SetValues(updatesPromptinKitsDAL);
            }
            else
            {
                promptinKitsDAL = Utility.mapper.Map<PromptDto, PromptsInKit>(prompt);
                context.PromptsInKits.Add(promptinKitsDAL);
            }
            context.SaveChanges();
            return true;
        }

        public ListPromptinKitsResponse ListPrompts(ListPromptsinKitsRequest request)
        {
            ListPromptinKitsResponse response = new ListPromptinKitsResponse();
            var promptList = context.PromptsInKits.Where(x => x.KitId == request.KitId && x.IsActive).OrderBy(x => x.DisplayOrder).ToList();
            response.PromptsinKits = Utility.mapper.Map<IList<DAL.PromptsInKit>, IList<PromptDto>>(promptList);
            return response;
        }

        public GetActivitiesinStepResponse ActivitiesinStepByKitId(GetActivitiesinStepRequest request)
        {
            GetActivitiesinStepResponse response = new GetActivitiesinStepResponse();
            var activities = context.ActivitiesinSteps.Include("StepsinKit").Where(x => x.StepsinKit.KitId == request.KitId && x.StepsinKit.IsActive).ToList();
            List<ActivitiesinStepsDto> activityList = new List<ActivitiesinStepsDto>();
            foreach (var activity in activities)
            {
                activity.QuestionsinActivities = activity.QuestionsinActivities.Where(q => q.IsActive).ToList();
                activityList.Add(Utility.mapper.Map<DAL.ActivitiesinStep, ActivitiesinStepsDto>(activity));
            }
            response.ActivitiesList = activityList;
            return response;
        }

        public ReadPromptResponse ReadPrompt(ReadPromptRequest request)
        {
            ReadPromptResponse response = new ReadPromptResponse();
            var promptData = context.PromptsInKits.Where(x => x.Id == request.id && x.IsActive).FirstOrDefault();
            response.prompt = Utility.mapper.Map<DAL.PromptsInKit, PromptDto>(promptData);
            return response;
        }

        public bool CompletePrompt(PromptsinKitsCompletedDto request)
        {
            var promptinKitsCompletedDAL = context.PromptsinKitsCompleteds.Where(x => x.KitsInUserProgramId == request.KitsInUserProgramId && x.PromptId == request.PromptId).FirstOrDefault();
            if (promptinKitsCompletedDAL != null)
            {
                request.Id = promptinKitsCompletedDAL.Id;
                var updatedPromptinKitsCompletedDAL = Utility.mapper.Map<PromptsinKitsCompletedDto, PromptsinKitsCompleted>(request);
                context.Entry(promptinKitsCompletedDAL).CurrentValues.SetValues(updatedPromptinKitsCompletedDAL);
            }
            else
            {
                promptinKitsCompletedDAL = Utility.mapper.Map<PromptsinKitsCompletedDto, PromptsinKitsCompleted>(request);
                context.PromptsinKitsCompleteds.Add(promptinKitsCompletedDAL);
            }
            context.SaveChanges();
            return true;
        }

        public IList<PromptsinKitsCompletedDto> GetPromptsinKitsCompletedList(int kitsInUserProgramId)
        {
            var promptsinKitsCompleted = context.PromptsinKitsCompleteds
                .Where(x => x.KitsInUserProgramId == kitsInUserProgramId && (x.IsActive == null || x.IsActive == true)).ToList();

            List<PromptsinKitsCompletedDto> kitsCompletedList = new List<PromptsinKitsCompletedDto>();
            if (promptsinKitsCompleted != null && promptsinKitsCompleted.Count > 0)
            {
                foreach (var record in promptsinKitsCompleted)
                {
                    PromptsinKitsCompletedDto completedKit = new PromptsinKitsCompletedDto();
                    completedKit.Id = record.Id;
                    completedKit.KitsInUserProgramId = record.KitsInUserProgramId;
                    completedKit.PromptId = record.PromptId;
                    completedKit.UpdatedBy = record.UpdatedBy;
                    completedKit.UpdatedDate = record.UpdatedDate;
                    kitsCompletedList.Add(completedKit);
                }
            }
            return kitsCompletedList;
        }

        public bool ListenedAudio(int kitsinUserProgramId)
        {
            var kitsinUserProgram = context.KitsinUserPrograms.Where(x => x.Id == kitsinUserProgramId).FirstOrDefault();
            if (kitsinUserProgram != null)
            {
                kitsinUserProgram.ListenedAudio = true;
                context.KitsinUserPrograms.Attach(kitsinUserProgram);
                context.Entry(kitsinUserProgram).State = EntityState.Modified;
                context.SaveChanges();
            }
            return true;
        }

        public AddEditActionGoalResponse AddEditActionGoals(AddEditActionGoalRequest request)
        {
            AddEditActionGoalResponse response = new AddEditActionGoalResponse();
            if (request.goalsId.HasValue || request.kitsInUserProgramId.HasValue)
            {
                var kitsinUserProgramGoals = context.KitsinUserProgramGoals.Where(x => x.Id == request.goalsId || x.KitsinUserProgramId == request.kitsInUserProgramId).FirstOrDefault();
                if (kitsinUserProgramGoals != null)
                {
                    if (request.achieveGoal)
                    {
                        kitsinUserProgramGoals.GoalsAchieved = request.achieveGoal;
                        kitsinUserProgramGoals.DateAchieved = DateTime.UtcNow;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(request.goals))
                        {
                            context.KitsinUserProgramGoals.Remove(kitsinUserProgramGoals);
                            context.SaveChanges();
                            response.status = true;
                            return response;
                        }
                        kitsinUserProgramGoals.Goals = request.goals;

                    }
                    context.KitsinUserProgramGoals.Attach(kitsinUserProgramGoals);
                    context.Entry(kitsinUserProgramGoals).State = EntityState.Modified;
                    context.SaveChanges();
                    response.kitsGoalsId = kitsinUserProgramGoals.Id;
                    response.status = true;
                    return response;
                }
            }
            var kitsinUserProgramGoal = new DAL.KitsinUserProgramGoal();
            kitsinUserProgramGoal.KitsinUserProgramId = request.kitsInUserProgramId.Value;
            kitsinUserProgramGoal.Goals = request.goals;
            context.KitsinUserProgramGoals.Add(kitsinUserProgramGoal);
            context.SaveChanges();
            response.kitsGoalsId = kitsinUserProgramGoal.Id;
            response.status = true;
            return response;
        }

        public KitActionGoalsResponse GetKitsActionGoals(KitActionGoalsRequest request)
        {
            KitActionGoalsResponse response = new KitActionGoalsResponse();
            var KitsinUserProgramGoals = context.KitsinUserProgramGoals.Include("KitsinUserProgram").Include("KitsinUserProgram.UsersinProgram").Where(x => x.KitsinUserProgram.UsersinProgram.UserId == request.participantId).ToList();
            response.KitsinUserProgramGoals = Utility.mapper.Map<IList<DAL.KitsinUserProgramGoal>, IList<KitsinUserProgramGoalDto>>(KitsinUserProgramGoals);
            return response;

        }
    }
}
