using Intervent.Web.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.OleDb;
using System.Web;

namespace InterventWebApp
{
    public class KitController : BaseController
    {
        private readonly IHostEnvironment environment;

        public KitController(IHostEnvironment environment)
        {
            this.environment = environment;
        }

        #region Manage Kits

        [ModuleControl(Modules.Kits)]
        public ActionResult KitList()
        {
            ViewData["languageList"] = CommonUtility.GetLanguages().Languages.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.LanguageCode });
            return View();
        }

        [ModuleControl(Modules.Kits)]
        public ActionResult KitDetails(int id)
        {
            KitModel model = new KitModel();
            model.Id = id;
            ViewData["languageList"] = CommonUtility.GetLanguages().Languages.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.LanguageCode });
            ViewData["refTypeList"] = CommonUtility.GetPromptRefTypeList();
            ViewData["refIdList"] = CommonUtility.GetPromptRefTypeList();
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return View(model);
        }

        [ModuleControl(Modules.Kits)]
        public JsonResult SearchQuestion(int kitId, string searchText, int? questionType, int? activityId)
        {
            var response = KitUtility.SearchQuestion(kitId, searchText, questionType, activityId);
            return Json(new { Result = "OK", Record = response.PassiveQuestions });
        }

        [ModuleControl(Modules.Kits)]
        [HttpPost]
        public JsonResult GetKitDetails(int id, string language)
        {
            var response = KitUtility.GetKitDetails(id, language);
            return Json(new { Result = "OK", Record = response.kit, Translation = response.KitTranslation });
        }

        [ModuleControl(Modules.Kits)]
        [HttpPost]
        public JsonResult AddKit(string name, string description, short topic)
        {
            var response = KitUtility.AddEditEduKit(null, name, description, topic, null, null, null, null, null, null);
            return Json(new { Result = "OK", Record = response.eduKit });
        }

        [ModuleControl(Modules.Kits)]
        [HttpPost]
        public JsonResult EditKit(int id, string name, string description, short topic, string keyConcepts, string audio, string pdf, string inventoryId, string language, DateTime? publishDate, DateTime? lastUpdated)
        {
            if (!string.IsNullOrEmpty(language) && language != ListOptions.DefaultLanguage)
            {
                var response = KitUtility.AddEditEduKitTranslation(id, name, description, keyConcepts, audio, pdf, language, publishDate, lastUpdated);
                return Json(new { Result = "OK", Status = response });
            }
            else
            {
                var response = KitUtility.AddEditEduKit(id, name, description, topic, keyConcepts, audio, pdf, inventoryId, publishDate, lastUpdated);
                return Json(new { Result = "OK", Record = response.eduKit });
            }
        }

        [ModuleControl(Modules.Kits)]
        [HttpPost]
        public JsonResult DeleteEduKit(int Id)
        {
            var response = KitUtility.DeleteEduKit(Id);
            return Json(new { Result = "OK" });
        }

        [ModuleControl(Modules.Kits)]
        [HttpPost]
        public JsonResult GetKitTopics()
        {
            var eduKitTOpics = KitUtility.GetKitTopics();
            return Json(new { Result = "OK", Options = eduKitTOpics });
        }

        [ModuleControl(Modules.Kits)]
        [HttpPost]
        public JsonResult AddEditStepinKit(int kitId, int? id, string text, string stepNo, string stepName, bool isActive, bool isSubStep, bool isAppendix, bool isGoal, string language)
        {
            var response = KitUtility.AddEditStepinKit(kitId, id, text, stepNo, stepName, isActive, isSubStep, isAppendix, isGoal, language);
            return Json(new { Result = "OK", Record = response });
        }

        [ModuleControl(Modules.Kits)]
        [HttpPost]
        public JsonResult ReadStepinKit(int id, string language)
        {
            var response = KitUtility.ReadStepinKit(id, language);
            return Json(new { Result = "OK", Record = response.stepinKit });
        }

        [ModuleControl(Modules.Kits)]
        public ActionResult Activity(int kitId, int? id, int? activityId)
        {
            ActivityModel model = new ActivityModel();
            model.KitId = kitId;
            if (id.HasValue)
                model.StepId = id.Value;
            if (activityId.HasValue)
                model.ActivityId = activityId.Value;
            model.LanguageList = CommonUtility.GetLanguages().Languages.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.LanguageCode });
            return PartialView("_Activity", model);
        }

        [ModuleControl(Modules.Kits)]
        public JsonResult ReadActivityinStep(int id)
        {
            var response = KitUtility.ReadActivityinStep(id);
            return Json(new { Result = "OK", Record = response });
        }

        [ModuleControl(Modules.Kits)]
        public JsonResult ReadQuestionText(int questionId, string language)
        {
            var response = KitUtility.ReadLanguageText(Intervent.Web.DataLayer.LanguageType.QET, questionId, language);
            return Json(new { Result = "OK", Record = response });
        }

        [ModuleControl(Modules.Kits)]
        public JsonResult ReadPassiveQuestionText(int questionId, string language)
        {
            var response = KitUtility.ReadLanguageText(Intervent.Web.DataLayer.LanguageType.PQET, questionId, language);
            return Json(new { Result = "OK", Record = response });
        }


        [ModuleControl(Modules.Kits)]
        [HttpPost]
        public JsonResult GetInputTypes()
        {
            var inputTypes = KitUtility.GetInputTypes().Select(c => new { DisplayText = c.Text, Value = c.Value }).OrderBy(s => s.DisplayText);
            return Json(new { Result = "OK", Options = inputTypes });
        }

        [ModuleControl(Modules.Kits)]
        public JsonResult AddEditActivityinStep(int stepId, string topText, string bottomText, bool withinStep, bool isActive, bool allowUpdate, int? sequence, int? id, string language)
        {
            var response = KitUtility.AddEditActivityinStep(stepId, id, topText, bottomText, withinStep, isActive, allowUpdate, sequence, language);
            return Json(new { Result = "OK", Record = response });
        }

        [ModuleControl(Modules.Kits)]
        public JsonResult AddPassiveQuestionsInActivity(int activityId, int questionId, string questionText, int? sequence, bool isActive, string language)
        {
            var response = KitUtility.AddEditPassiveQuestionsInActivity(activityId, questionId, questionText, sequence, isActive, language);
            return Json(new { Result = "OK", Record = response });
        }

        [ModuleControl(Modules.Kits)]
        public JsonResult AddEditQuestioninActivity(int activityId, string questionText, byte questionType, int? Id, bool isActive, int? sequence, bool isVertical, string language, int? parentId, bool isRequired)
        {
            var response = KitUtility.AddEditQuestioninActivity(activityId, Id, questionText, questionType, isActive, sequence, isVertical, language, parentId, isRequired);
            return Json(new { Result = "OK", Record = response });
        }

        [ModuleControl(Modules.Kits)]
        public JsonResult AddEditOptions(string questionOption, int questionId, int? id, bool? isAnswer, bool? isActive, short? sequence, int? points, string language)
        {
            var response = KitUtility.AddEditOptions(questionOption, questionId, id, isAnswer, sequence, points, language, isActive);
            return Json(new { Result = "OK", Record = response });
        }

        [ModuleControl(Modules.Kits)]
        public ActionResult Quiz(int id)
        {
            QuizModel model = new QuizModel();
            model.stepId = id;
            model.LanguageList = CommonUtility.GetLanguages().Languages.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.LanguageCode });
            return PartialView("_Quiz", model);
        }

        [ModuleControl(Modules.Kits)]
        public JsonResult ReadQuizinKit(int stepId, string language)
        {
            var response = KitUtility.ReadQuizinKit(stepId, language);
            return Json(new { Result = "OK", Record = response });
        }

        [ModuleControl(Modules.Kits)]
        public JsonResult AddEditQuiz(int stepId, string quizText, byte quizType, int? Id, bool isActive, string language)
        {
            var response = KitUtility.AddEditQuiz(stepId, Id, quizText, quizType, isActive, language);
            return Json(new { Result = "OK", Record = response });
        }

        [ModuleControl(Modules.Kits)]
        public JsonResult AddEditOptionsforQuiz(string quizOption, int quizId, int? id, bool isDefault, bool isActive, string language)
        {
            var response = KitUtility.AddEditOptionsforQuiz(quizOption, quizId, id, isDefault, isActive, language);
            return Json(new { Result = "OK", Record = response });
        }

        [ModuleControl(Modules.Kits)]
        [HttpPost]
        public JsonResult UploadKitFile(IFormFile FileUpload, string language)
        {
            var count = 0;
            var filerror = "";
            var status = true;
            try
            {
                if (FileUpload != null && !string.IsNullOrEmpty(language))
                {
                    var res = LoadAndSaveExcelKitData(FileUpload, language);
                    count = res.Item1;
                    filerror = res.Item2;
                }
            }
            catch
            {
                status = false;
            }
            return Json(new { Count = count, Error = filerror, Status = status });
        }

        [ModuleControl(Modules.Kits)]
        public Tuple<int, string> LoadAndSaveExcelKitData(IFormFile FileUpload, string lang)
        {
            List<KitsDto> kits = new List<KitsDto>();
            List<StepsinKitsDto> steps = new List<StepsinKitsDto>();
            List<ActivitiesinStepsDto> activities = new List<ActivitiesinStepsDto>();
            List<QuestionsinActivityDto> questions = new List<QuestionsinActivityDto>();
            List<OptionsforActivityQuestionDto> questionOptions = new List<OptionsforActivityQuestionDto>();
            List<QuizinStepDto> quizList = new List<QuizinStepDto>();
            List<OptionsforQuizDto> quizChoices = new List<OptionsforQuizDto>();
            string fileerror = "";
            string filename = FileUpload.FileName;
            if (filename.EndsWith(".xlsx"))
             {
                 string targetpath = environment.ContentRootPath + "/temp/";
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
                     if (tables == null) return new Tuple<int, string>(0, "No excel table found");
                     for (int rowIndex = 0; rowIndex < tables.Rows.Count; rowIndex++)
                     {
                         var sheet = tables.Rows[rowIndex]["TABLE_NAME"];
                         OleDbCommand command = new OleDbCommand("select * from [" + sheet + "]", connection);
                         using (OleDbDataReader dr = command.ExecuteReader())
                         {
                             var schema = dr.GetSchemaTable();
                             Dictionary<int, string> columns = new Dictionary<int, string>();
                             for (int i = 0; i < schema.Rows.Count; i++)
                             {
                                 columns.Add(i, schema.Rows[i].ItemArray[0].ToString().ToLower());
                             }

                             if (sheet.ToString().ToLower().Contains("stepsinkits"))
                             {
                                 #region StepsInKits Conversion
                                 while (dr.Read())
                                 {
                                     StepsinKitsDto step = new StepsinKitsDto();

                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "id").Key].ToString()))
                                         step.Id = int.Parse(dr[columns.FirstOrDefault(x => x.Value == "id").Key].ToString());
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "kitid").Key].ToString()))
                                         step.KitId = int.Parse(dr[columns.FirstOrDefault(x => x.Value == "kitid").Key].ToString());
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "name").Key].ToString()))
                                         step.Name = dr[columns.FirstOrDefault(x => x.Value == "name").Key].ToString();
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "text").Key].ToString()))
                                         step.Text = dr[columns.FirstOrDefault(x => x.Value == "text").Key].ToString();
                                     steps.Add(step);
                                 }
                                 #endregion
                             }
                             else if (sheet.ToString().ToLower().Contains("kit"))
                             {
                                 #region Kit Conversion
                                 while (dr.Read())
                                 {
                                     KitsDto kit = new KitsDto();

                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "id").Key].ToString()))
                                         kit.Id = int.Parse(dr[columns.FirstOrDefault(x => x.Value == "id").Key].ToString());
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "name").Key].ToString()))
                                         kit.Name = dr[columns.FirstOrDefault(x => x.Value == "name").Key].ToString();
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "description").Key].ToString()))
                                         kit.Description = dr[columns.FirstOrDefault(x => x.Value == "description").Key].ToString();
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "keyconcepts").Key].ToString()))
                                         kit.KeyConcepts = dr[columns.FirstOrDefault(x => x.Value == "keyconcepts").Key].ToString();
                                     kits.Add(kit);
                                 }
                                 #endregion
                             }
                             else if (sheet.ToString().ToLower().Contains("activitiesinsteps"))
                             {
                                 #region ActivitiesinSteps Conversion
                                 while (dr.Read())
                                 {
                                     ActivitiesinStepsDto activity = new ActivitiesinStepsDto();

                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "id").Key].ToString()))
                                         activity.Id = int.Parse(dr[columns.FirstOrDefault(x => x.Value == "id").Key].ToString());
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "stepid").Key].ToString()))
                                         activity.StepId = int.Parse(dr[columns.FirstOrDefault(x => x.Value == "stepid").Key].ToString());
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "toptext").Key].ToString()))
                                         activity.TopText = dr[columns.FirstOrDefault(x => x.Value == "toptext").Key].ToString();
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "bottomtext").Key].ToString()))
                                         activity.BottomText = dr[columns.FirstOrDefault(x => x.Value == "bottomtext").Key].ToString();
                                     activities.Add(activity);
                                 }
                                 #endregion
                             }
                             else if (sheet.ToString().ToLower().Contains("questionsinactivities"))
                             {
                                 #region QuestionsinActivity Conversion
                                 while (dr.Read())
                                 {
                                     QuestionsinActivityDto question = new QuestionsinActivityDto();

                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "id").Key].ToString()))
                                         question.Id = int.Parse(dr[columns.FirstOrDefault(x => x.Value == "id").Key].ToString());
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "activityid").Key].ToString()))
                                         question.ActivityId = int.Parse(dr[columns.FirstOrDefault(x => x.Value == "activityid").Key].ToString());
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "questiontext").Key].ToString()))
                                         question.QuestionText = dr[columns.FirstOrDefault(x => x.Value == "questiontext").Key].ToString();
                                     questions.Add(question);
                                 }
                                 #endregion
                             }
                             else if (sheet.ToString().ToLower().Contains("optionsinactivityquestions"))
                             {
                                 #region OptionsforActivityQuestion Conversion
                                 while (dr.Read())
                                 {
                                     OptionsforActivityQuestionDto option = new OptionsforActivityQuestionDto();

                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "id").Key].ToString()))
                                         option.Id = int.Parse(dr[columns.FirstOrDefault(x => x.Value == "id").Key].ToString());
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "questionid").Key].ToString()))
                                         option.QuestionId = int.Parse(dr[columns.FirstOrDefault(x => x.Value == "questionid").Key].ToString());
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "optiontext").Key].ToString()))
                                         option.OptionText = dr[columns.FirstOrDefault(x => x.Value == "optiontext").Key].ToString();
                                     questionOptions.Add(option);
                                 }
                                 #endregion
                             }
                             else if (sheet.ToString().ToLower().Contains("quizinsteps"))
                             {
                                 #region QuizinStep Conversion
                                 while (dr.Read())
                                 {
                                     QuizinStepDto quiz = new QuizinStepDto();

                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "id").Key].ToString()))
                                         quiz.Id = int.Parse(dr[columns.FirstOrDefault(x => x.Value == "id").Key].ToString());
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "stepid").Key].ToString()))
                                         quiz.StepId = int.Parse(dr[columns.FirstOrDefault(x => x.Value == "stepid").Key].ToString());
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "quiztext").Key].ToString()))
                                         quiz.QuizText = dr[columns.FirstOrDefault(x => x.Value == "quiztext").Key].ToString();
                                     quizList.Add(quiz);
                                 }
                                 #endregion
                             }

                             else if (sheet.ToString().ToLower().Contains("quizchoices"))
                             {
                                 #region OptionsforQuiz Conversion
                                 while (dr.Read())
                                 {
                                     OptionsforQuizDto quizOption = new OptionsforQuizDto();

                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "id").Key].ToString()))
                                         quizOption.Id = int.Parse(dr[columns.FirstOrDefault(x => x.Value == "id").Key].ToString());
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "quizid").Key].ToString()))
                                         quizOption.QuizId = int.Parse(dr[columns.FirstOrDefault(x => x.Value == "quizid").Key].ToString());
                                     if (!String.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "optiontext").Key].ToString()))
                                         quizOption.OptionText = dr[columns.FirstOrDefault(x => x.Value == "optiontext").Key].ToString();
                                     quizChoices.Add(quizOption);
                                 }
                                 #endregion
                             }
                         }
                     }
                     connection.Close();
                     System.IO.File.Delete(pathToExcelFile);
                 }
             }

            #region Save Kit Data

            #region ErrorIds
            List<int> kitIds = new List<int>();
            List<int> stepIds = new List<int>();
            List<int> activityIds = new List<int>();
            List<int> questionIds = new List<int>();
            List<int> questionOptionIds = new List<int>();
            List<int> quizIds = new List<int>();
            List<int> quizOptionIds = new List<int>();
            int kitCount = 0;
            #endregion

            foreach (var kit in kits)
            {
                try
                {
                    KitUtility.AddEditEduKitTranslation(kit.Id, kit.Name, kit.Description, kit.KeyConcepts, "", "", lang, null, null);
                    kitCount++;
                }
                catch
                {
                    kitIds.Add(kit.Id);
                }
            }

            foreach (var step in steps)
            {
                try
                {
                    KitUtility.AddEditStepinKit(step.KitId, step.Id, step.Text, "", step.Name, true, false, false, false, lang);
                }
                catch
                {
                    stepIds.Add(step.Id);
                }
            }
            foreach (var activity in activities)
            {
                try
                {
                    KitUtility.AddEditActivityinStep(activity.StepId, activity.Id, activity.TopText, activity.BottomText, true, true, true, null, lang);
                }
                catch
                {
                    activityIds.Add(activity.Id);
                }
            }
            foreach (var question in questions)
            {
                try
                {
                    KitUtility.AddEditQuestioninActivity(question.ActivityId, question.Id, question.QuestionText, 1, true, null, false, lang, null, true);
                }
                catch
                {
                    questionIds.Add(question.Id);
                }
            }
            foreach (var option in questionOptions)
            {
                try
                {
                    if (!string.IsNullOrEmpty(option.OptionText))
                        KitUtility.AddEditOptions(option.OptionText, option.QuestionId, option.Id, null, null, null, lang);
                }
                catch
                {
                    questionOptionIds.Add(option.Id);
                }
            }
            foreach (var quiz in quizList)
            {
                try
                {
                    KitUtility.AddEditQuiz(quiz.StepId, quiz.Id, quiz.QuizText, 1, true, lang);
                }
                catch
                {
                    quizIds.Add(quiz.Id);
                }
            }
            foreach (var quizChoice in quizChoices)
            {
                try
                {
                    KitUtility.AddEditOptionsforQuiz(quizChoice.OptionText, quizChoice.QuizId, quizChoice.Id, false, true, lang);
                }
                catch
                {
                    quizOptionIds.Add(quizChoice.Id);
                }
            }
            #endregion

            #region Error Construction
            if (kitIds.Count() > 0)
            {
                fileerror = string.Format("KitId(s): {1}", fileerror, string.Join(",", kitIds.ToArray()));
            }
            if (stepIds.Count() > 0)
            {
                fileerror = string.Format("{0} StepId(s): {1}", fileerror, string.Join(",", stepIds.ToArray()));
            }
            if (activityIds.Count() > 0)
            {
                fileerror = string.Format("{0} ActivityId(s): {1}", fileerror, string.Join(",", activityIds.ToArray()));
            }
            if (questionIds.Count() > 0)
            {
                fileerror = string.Format("{0} QuestionId(s): {1}", fileerror, string.Join(",", questionIds.ToArray()));
            }
            if (questionOptionIds.Count() > 0)
            {
                fileerror = string.Format("{0} QuestionOptionId(s): {1}", fileerror, string.Join(",", questionOptionIds.ToArray()));
            }
            if (quizIds.Count() > 0)
            {
                fileerror = string.Format("{0} QuizId(s): {1}", fileerror, string.Join(",", quizIds.ToArray()));
            }
            if (quizOptionIds.Count() > 0)
            {
                fileerror = string.Format("{0} QuizOptionId(s): {1}", fileerror, string.Join(",", quizOptionIds.ToArray()));
            }
            if (!string.IsNullOrEmpty(fileerror))
            {
                fileerror = string.Format("Failed to process the following Entries: {0}", fileerror);
            }

            #endregion

            return new Tuple<int, string>(kitCount, fileerror);
        }

        #endregion

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListEduKits(int? page, int? pageSize, int? totalRecords)
        {
            var response = KitUtility.ListEduKits(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId), page, pageSize, totalRecords, false);

            return Json(new { Result = "OK", Records = response.EduKits, TotalRecords = response.totalRecords });
        }

        [Authorize]
        public ActionResult ViewKit(int id, int? kitsInUserProgramId, bool? preview, string prevLanguage, int? index, string pageIdentifier)
        {
            var response = KitUtility.GetKitById(HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value, id, kitsInUserProgramId, preview, prevLanguage);
            response.preview = preview ?? false;
            response.pageIdentifier = pageIdentifier;
            response.index = index;
            response.UserinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId);
            if (response.success == false)
                return RedirectToAction("MyProgram", "Program");
            else
                return PartialView("ViewKit", response);
        }

        [Authorize]
        public ActionResult ReviewKit(int kitsInUserProgramId, bool? readOnly, bool? userView)
        {
            ViewData["readonly"] = readOnly;
            ViewData["userView"] = userView;
            var response = KitUtility.ReviewKit(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, kitsInUserProgramId);
            return PartialView("ReviewKit", response);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AddTableOption(int row, int column, int questionId, int activityId)
        {
            var result = KitUtility.AddEditTableOption(row, column, questionId, activityId);
            return Json(new { Result = "OK", Record = result });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AddRow(int questionId)
        {
            var result = KitUtility.AddRow(questionId);
            return Json(new { Result = "OK", Record = result });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult ReadRows(int questionId)
        {
            var result = KitUtility.ReadRows(questionId);
            return Json(new { Result = "OK", Record = result });
        }


        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult ReadColumns(int rowId)
        {
            var result = KitUtility.ReadColumns(rowId);
            return Json(new { Result = "OK", Record = result });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AddColumn(int activityId, int rowId, int columnId, byte? questionType, short rowSpan, short colSpan, string style, string text, string language)
        {
            var result = KitUtility.AddColumn(activityId, rowId, columnId, questionType, rowSpan, colSpan, style, text, language);
            return Json(new { Result = "OK", Record = result });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult SetColSpan(int parentQuesId, short? sequence)
        {
            KitUtility.SetColSpan(parentQuesId, sequence);
            return Json(new { Result = "OK", Record = true });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AddTableChkBox(int indexId, int quesId, int parentQuesId, int activityId, string indexText, string questionText, short? sequence, string language, bool isOption)
        {
            var result = KitUtility.AddTableChkBox(indexId, quesId, parentQuesId, activityId, indexText, questionText, sequence, language, isOption);
            return Json(new { Result = "OK", Record = result });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult ReadTableChkBox(int parentQuesId)
        {
            var result = KitUtility.ReadTableChkBox(parentQuesId);
            return Json(new { Result = "OK", Record = result });
        }



        [Authorize]
        public ActionResult KitStepDetails(int kitId, string pageIdentifier, int kitsInUserProgramsId, string languageCode)
        {
            var response = KitUtility.GetKitByIdentifier(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, kitId, pageIdentifier, kitsInUserProgramsId, languageCode);
            return PartialView("_KitStepDetails", response);
        }

        [Authorize]
        public ActionResult Kit()
        {
            return View();
        }


        [ModuleControl(Modules.Kits)]
        [HttpPost]
        public JsonResult AddEditPrompts([FromBody] PromptModel model)
        {
            var result = KitUtility.AddEditPrompts(HttpContext.Session.GetInt32(SessionContext.UserId).Value, model);
            return Json(new { Result = "OK", Record = result });
        }

        [ModuleControl(Modules.Kits)]
        public JsonResult ListPrompts(int id)
        {
            var result = KitUtility.ListPrompts(id);
            return Json(new { Result = "OK", Record = result });
        }

        [ModuleControl(Modules.Kits)]
        public JsonResult ReadPrompt(int id)
        {
            var response = KitUtility.ReadPrompt(id);
            return Json(new { Result = "OK", Record = response.prompt });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ReferenceIdListGet(int kitId, int refTypeId)
        {
            var result = KitUtility.ReferenceIdListGet(kitId, refTypeId);
            return Json(new { Result = "OK", Record = result });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult CompletePrompt([FromBody] PromptsinKitsCompletedModel model)
        {
            var result = KitUtility.CompletePrompt(HttpContext.Session.GetInt32(SessionContext.UserId).Value, model);
            return Json(new { Result = "OK", Record = result });
        }

        [Authorize]
        public JsonResult ListenedAudio(int kitsinUserProgramId)
        {
            var response = KitUtility.ListenedAudio(kitsinUserProgramId);
            return Json(new { Result = "OK" });
        }

        [Authorize]
        public JsonResult BPInfo()
        {
            var id = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            var response = ReportUtility.ReadHRAReport(id);
            var bpRisk = ReportUtility.GetBPRisk(response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer), null, true, null);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            var HRADate = TimeZoneInfo.ConvertTimeFromUtc(response.hra.CompleteDate.Value, custTZone).ToShortDateString();
            return Json(new { bpRisk.sbpRiskChart, bpRisk.dbpRiskChart, bpRisk.riskText, HRADate });
        }

        [Authorize]
        public JsonResult WeightInfo()
        {
            var id = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            var Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
            var response = ReportUtility.ReadHRAReport(id);
            var weightRisk = ReportUtility.GetOverweightRisk(response.hra, null, Measurements, null, HttpContext.Session.GetInt32(SessionContext.Unit).Value, true);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            var HRADate = TimeZoneInfo.ConvertTimeFromUtc(response.hra.CompleteDate.Value, custTZone).ToShortDateString();
            return Json(new { weightRisk, HRADate });
        }

        [Authorize]
        public JsonResult GlucoseInfo()
        {
            var id = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            var response = ReportUtility.ReadHRAReport(id);
            var glucoseRisk = ReportUtility.GetDiabetesRisk(response.hra.User, response.hra, true, null, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            var HRADate = TimeZoneInfo.ConvertTimeFromUtc(response.hra.CompleteDate.Value, custTZone).ToShortDateString();
            return Json(new { glucoseRisk, HRADate });
        }

        [Authorize]
        public JsonResult CholInfo()
        {
            var id = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            var Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
            var response = ReportUtility.ReadHRAReport(id);
            var CTRisk = ReportUtility.GetCTRisk(response.hra.User, response.hra, HttpContext.Session.GetInt32(SessionContext.HRAVer), null, Measurements, null, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            var HRADate = TimeZoneInfo.ConvertTimeFromUtc(response.hra.CompleteDate.Value, custTZone).ToShortDateString();
            return Json(new { CTRisk, HRADate });
        }

        [Authorize]
        public JsonResult TenYearCHDRisk()
        {
            var id = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            var response = ReportUtility.ReadHRAReport(id);
            var hDSRisks = ReportUtility.GetHDSRisk(response.hra.User, response.hra, null);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            var HRADate = TimeZoneInfo.ConvertTimeFromUtc(response.hra.CompleteDate.Value, custTZone).ToShortDateString();
            return Json(new { hDSRisks, HRADate });
        }

        [ModuleControl(Modules.Kits)]
        public JsonResult CloneKit(int kitId)
        {
            var response = KitUtility.CloneKit(kitId);
            return Json(new { response });
        }

        [HttpPost]
        public JsonResult AddEditActionGoals(int? kitsInUserProgramId, string goal, int? goalsId, bool? achieveGoal)
        {
            var result = KitUtility.AddEditActionGoals(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.AdminId), HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId), kitsInUserProgramId, goal, goalsId, achieveGoal);
            return Json(new { Result = "OK", Record = result });
        }

        [Authorize]
        public JsonResult GetKitsActionGoals()
        {
            var response = KitUtility.GetKitsActionGoals(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);

            return Json(new { Result = "OK", Records = response });
        }

        [Authorize]
        public ActionResult GetCompletedKits()
        {
            CompletedKitsModel model = new CompletedKitsModel();
            var programHistory = ProgramUtility.GetUserProgramHistory(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, User.TimeZone(), HttpContext.Session.GetString(SessionContext.LanguagePreference) != null ? HttpContext.Session.GetString(SessionContext.LanguagePreference) : "");
            if (programHistory != null && programHistory.usersinPrograms.Where(x => x.Id == HttpContext.Session.GetInt32(SessionContext.UserinProgramId) && x.KitsinUserPrograms.Count() > 0).Count() > 0)
            {
                model.KitsinUserPrograms = programHistory.usersinPrograms.Where(x => x.Id == HttpContext.Session.GetInt32(SessionContext.UserinProgramId)).FirstOrDefault().KitsinUserPrograms.ToList();
            }
            else
            {
                var pastPrograms = programHistory.usersinPrograms.Where(x => x.Id != HttpContext.Session.GetInt32(SessionContext.UserinProgramId) && x.KitsinUserPrograms.Count() > 0).ToList();
                if (pastPrograms.Count > 0)
                    model.KitsinUserPrograms = pastPrograms.FirstOrDefault().KitsinUserPrograms.Where(y => y.CompleteDate.HasValue).ToList();
            }
            return PartialView("_CompletedKits", model);
        }

        [ModuleControl(Modules.Kits)]
        [HttpPost]
        public JsonResult AssignKit([FromBody] AssignKitModel model)
        {
            var response = KitUtility.AddKittoPrograms(model.KitId, model.OrganizationIds);
            return Json(new { Result = "OK", Record = response });
        }


    }
}