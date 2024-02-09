using Intervent.DAL;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Text;

namespace Intervent.Business
{
    public class HraManager : BaseManager
    {
        HRAReader hraReader;
        ParticipantReader reader;
        AccountReader accountReader;

        public HraManager(UserManager<ApplicationUser> userManager)
        {
            hraReader = new HRAReader();
            reader = new ParticipantReader();
            accountReader = new AccountReader(userManager);
        }

        public String livongoHRAFileName = "LivongoHRAData-" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
        public String livongoHRAExcelFileName = "LivongoHRAData-" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";

        public APIHRAQuestionResponse GetHRAAssessment(string userId, int salesForceOrgId)
        {
            APIHRAQuestionResponse response = new APIHRAQuestionResponse();
            ReadUserParticipationRequest request = new ReadUserParticipationRequest();
            request.UserId = Int32.Parse(userId);
            var getUserResponse = Task.Run(() => accountReader.GetUser(new GetUserRequest() { id = request.UserId })).Result.User;
            if (getUserResponse == null || getUserResponse.OrganizationId != salesForceOrgId)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessage = "User Not Found";
                return response;
            }
            var user = reader.ReadUserParticipation(request).user;

            List<QuestionDto> questions = hraReader.GetHRAQuestions(new GetHRAQuestionRequest()).Questions.ToList();
            List<QuestionCategory> categories = new List<QuestionCategory>();
            IEnumerable<QuestionCategoryDto> requestCategories = QuestionCategoryDto.GetHRA();
            HRADto hra = new HRADto();
            ReadHRARequest hraRequest = new ReadHRARequest();
            hraRequest.userId = user.Id;
            hraRequest.portalId = user.Organization.Portals.Where(x => x.Active == true).FirstOrDefault().Id;
            hra = hraReader.ReadHRAByPortal(hraRequest).hra;
            foreach (QuestionCategoryDto requestCategory in requestCategories)
            {
                object HRA = null;
                if (hra != null)
                {
                    if (requestCategory.Id == 1)
                    {
                        HRA = hra.MedicalCondition;
                    }
                    else if (requestCategory.Id == 2)
                    {
                        HRA = hra.OtherRiskFactors;
                    }
                    else if (requestCategory.Id == 3)
                    {
                        HRA = hra.HSP;
                    }
                    else if (requestCategory.Id == 4)
                    {
                        HRA = hra.Exams;
                    }
                    else if (requestCategory.Id == 5)
                    {
                        HRA = hra.Interest;
                    }
                    else if (requestCategory.Id == 6)
                    {
                        HRA = hra.HealthNumbers;
                    }
                }
                QuestionCategory category = new QuestionCategory();
                category.Completed = HRA == null ? "False" : "True";
                category.CategoryId = requestCategory.Id;
                category.CategoryDescription = requestCategory.CategoryName;
                var categoryQuestions = questions.Where(x => x.QuestionCategoryId == requestCategory.Id);
                bool skipQuestionNumbering = (requestCategory.Id == QuestionCategoryDto.HealthMeasurements.Id);
                category.Questions = BuildQuestionHierarchy(categoryQuestions, categoryQuestions.Where(x => x.ParentQuestionId.HasValue == false), String.Empty, "1", user, skipQuestionNumbering, HRA);
                categories.Add(category);
            }
            response.Categories = categories;
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        public APIHRAQuestionResponse GetUserProfile(string userId, int salesForceOrgId)
        {
            UserDto user = null;
            APIHRAQuestionResponse response = new APIHRAQuestionResponse();
            if (userId != null && userId != "0")
            {
                user = new UserDto();
                ReadUserParticipationRequest request = new ReadUserParticipationRequest();
                request.UserId = Int32.Parse(userId);
                var getUserResponse = Task.Run(() => accountReader.GetUser(new GetUserRequest() { id = request.UserId })).Result.User;
                if (getUserResponse == null || getUserResponse.OrganizationId != salesForceOrgId)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessage = "User Not Found";
                    return response;
                }
                user = reader.ReadUserParticipation(request).user;
            }

            HRADto hra = new HRADto();
            List<QuestionDto> questions = hraReader.GetHRAQuestions(new GetHRAQuestionRequest()).Questions.ToList();
            List<QuestionCategory> categories = new List<QuestionCategory>();
            IEnumerable<QuestionCategoryDto> requestCategories = QuestionCategoryDto.GetProfile();
            foreach (QuestionCategoryDto requestCategory in requestCategories)
            {
                QuestionCategory category = new QuestionCategory();
                category.CategoryId = requestCategory.Id;
                category.CategoryDescription = requestCategory.CategoryName;
                category.Completed = user != null ? "True" : "False";
                var categoryQuestions = questions.Where(x => x.QuestionCategoryId == requestCategory.Id);
                bool skipQuestionNumbering = (requestCategory.Id == QuestionCategoryDto.HealthMeasurements.Id);
                category.Questions = BuildQuestionHierarchy(categoryQuestions, categoryQuestions.Where(x => x.ParentQuestionId.HasValue == false), String.Empty, "1", user, skipQuestionNumbering, user);
                categories.Add(category);
            }
            response.Categories = categories;
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        static IEnumerable<Web.DTO.Question> BuildQuestionHierarchy(IEnumerable<QuestionDto> allQuestions, IEnumerable<QuestionDto> requestQuestions, string questionNumberPrefix, string startSequence, UserDto user, bool skipQuestionNumberGeneration, object hra)
        {
            if (requestQuestions == null || requestQuestions.Count() == 0)
                return null;
            List<Web.DTO.Question> questions = new List<Web.DTO.Question>();
            requestQuestions = requestQuestions.OrderBy(x => x.ParentQuestionValue).ThenBy(x => x.SortOrder);
            int i = Int32.Parse(startSequence);
            string prevConditionValue = null, HeightInch = null;
            foreach (QuestionDto requestQuestion in requestQuestions)
            {
                if (hra != null && requestQuestion.MappingColumnName != null)
                {
                    if (requestQuestion.MappingColumnName == "Height")
                    {
                        string height = Utils.GetValue(hra, "Height");
                        if (!string.IsNullOrEmpty(height))
                        {
                            float inches = float.Parse(height);
                            requestQuestion.DefaultValue = ((int)(inches / 12)).ToString();
                            HeightInch = ((double)(inches % 12)).ToString();
                        }
                    }
                    else if (requestQuestion.MappingColumnName == "HeightInch" && !string.IsNullOrEmpty(HeightInch))
                    {
                        requestQuestion.DefaultValue = HeightInch;
                    }
                    else
                        Utils.SetPropertyValuetoString(requestQuestion, hra);
                }
                if (!String.IsNullOrEmpty(requestQuestion.RestrictionCriteria))
                {
                    int count = 0;
                    if (requestQuestion.RestrictionCriteria.Contains("FEMALE") && user.Gender.HasValue)
                    {
                        count++;
                        if (!IsFemale(user.Gender.Value.ToString()))
                            continue;
                    }
                    else if (requestQuestion.RestrictionCriteria.Contains("MALE") && user.Gender.HasValue)
                    {
                        count++;
                        if (!IsMale(user.Gender.Value.ToString()))
                            continue;
                    }
                    if (requestQuestion.RestrictionCriteria.Contains("GINA") && user.Organization.Portals[0].Active == true)
                    {
                        count++;
                        if (!(user.Organization.Portals[0].GINAQuestion))
                            continue;
                    }
                    if (requestQuestion.RestrictionCriteria.Contains("AGE") && user.DOB.HasValue)
                    {
                        count++;
                        DateTime now = DateTime.Today;
                        int age = now.Year - user.DOB.Value.Year;
                        if (now < user.DOB.Value.AddYears(age)) age--;
                        int ageLimit = Convert.ToInt16(requestQuestion.RestrictionCriteria.Substring(requestQuestion.RestrictionCriteria.IndexOf("AGE") + 4, 2));
                        if (age > ageLimit)
                            continue;
                    }
                    if (count == 0)
                    {
                        throw new ArgumentNullException(requestQuestion.RestrictionCriteria + " restriction criteria not found");
                    }
                }
                if (!String.IsNullOrEmpty(prevConditionValue) && prevConditionValue != requestQuestion.ParentQuestionValue)
                {
                    i = 1;//reset numbering scheme 
                }
                Web.DTO.Question question = new Web.DTO.Question();
                if (requestQuestion.ParentQuestionId.HasValue)
                {
                    question.Condition = new Condition();
                    question.Condition.ParentQuestionId = requestQuestion.ParentQuestionId.Value;
                    question.Condition.Value = prevConditionValue = requestQuestion.ParentQuestionValue;
                }
                question.ControlType = ControlTypeDto.GetByKey(requestQuestion.ControlTypeId).Description;
                //todo -  question.ControlValues
                if (requestQuestion.ValuesStaticMethodName == "YESNO")
                {
                    question.ControlValues = YesNoControlValues().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "YESNOD")
                {
                    question.ControlValues = YesNoDontKnowControlValues().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "PRESC_MEDICATION")
                {
                    question.ControlValues = PresMedicationControlValues().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "OTHER_TOBACCO_TYPE")
                {
                    question.ControlValues = OtherTobaccoTypeControlValues().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "PHYSICAL_PROBLEMS")
                {
                    question.ControlValues = PhysicalProblemsTypeControlValues().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "SLEEP_APNEA")
                {
                    question.ControlValues = SleepApneaControlValues().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "SMOKEING_DETAILS")
                {
                    question.ControlValues = ListOptions.SmokingDetails().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "ARM_USED")
                {
                    question.ControlValues = ListOptions.ArmUsed().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "STATE_INDICATE")
                {
                    question.ControlValues = ListOptions.GetStateOfHealthLists().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "LIFE_SATISFACTION")
                {
                    question.ControlValues = ListOptions.GetLifeSatisfactionList().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "JOB_SATISFACTION")
                {
                    question.ControlValues = ListOptions.GetJobSatisfactionList().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "RELAX_MED")
                {
                    question.ControlValues = ListOptions.GetRelaxMedList().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "MISS_DAYS")
                {
                    question.ControlValues = ListOptions.GetMissDays().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "TIMES")
                {
                    question.ControlValues = ListOptions.GetTimes().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "HEALTH_PROBLEMS")
                {
                    question.ControlValues = ListOptions.GetHealthProblems().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "EXAMS")
                {
                    question.ControlValues = GetExams().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "IMMUNIZATIONS")
                {
                    question.ControlValues = GetImmunizations().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "INTEREST")
                {
                    question.ControlValues = GetInterest().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "PREFIX")
                {
                    question.ControlValues = ListOptions.GetNamePrefixList().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "GENDER")
                {
                    question.ControlValues = ListOptions.GetGenderList(null).Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "RACE")
                {
                    question.ControlValues = ListOptions.GetRaceList().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "COUNTRY")
                {
                    question.ControlValues = ListCountries().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "STATE")
                {
                    question.ControlValues = ListStates().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "CONTACT_MODES")
                {
                    question.ControlValues = ListOptions.GetPreferredContactMode().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "PREFERRED_CONTACT_TIMES")
                {
                    question.ControlValues = ListOptions.GetPreferredContactTimes().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "REFERRED_BY")
                {
                    question.ControlValues = ListOptions.GetSource().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "TIME_ZONE")
                {
                    question.ControlValues = GetTimeZones().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "LANGUAGE_PREFERENCE")
                {
                    question.ControlValues = ListOptions.GetLanguagePreference().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else if (requestQuestion.ValuesStaticMethodName == "UNITS")
                {
                    question.ControlValues = ListOptions.GetUnits().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
                }
                else
                {
                    if (!String.IsNullOrEmpty(requestQuestion.ValuesStaticMethodName))
                        throw new ArgumentException(requestQuestion.ValuesStaticMethodName + " control values is not found.");
                }

                question.DefaultValue = requestQuestion.DefaultValue;
                question.HelpText = requestQuestion.HelpText;
                question.IsRequired = requestQuestion.IsRequired;
                question.QuestionId = requestQuestion.Id;
                question.RequiredExpression = requestQuestion.RequiredExpression;
                //e.g.Have you ever been told by a medical doctor or health care professional you have any of the following:
                if (!skipQuestionNumberGeneration)
                    question.QuestionNumber = (IsNestedQuestionHeading(requestQuestion.ControlTypeId)) ? null : (String.IsNullOrEmpty(questionNumberPrefix) ? Convert.ToString(i) : String.Join(".", questionNumberPrefix, Convert.ToString(i)));
                question.QuestionText = requestQuestion.QuestionText;
                question.SortOrder = requestQuestion.SortOrder;

                question.Row = requestQuestion.RowPosition.HasValue ? requestQuestion.RowPosition.Value.ToString() : null;
                question.Column = requestQuestion.ColumnPosition.HasValue ? requestQuestion.ColumnPosition.Value.ToString() : null;
                question.Placeholder = requestQuestion.Placeholder;
                question.AllowDecimalInput = requestQuestion.AllowDecimalInput;
                question.ControlSuffix = requestQuestion.ControlSuffix;

                if (!String.IsNullOrEmpty(requestQuestion.ValidatorExpression))
                {
                    var expressionList = requestQuestion.ValidatorExpression.Split(';');
                    if (expressionList.Count() > 0)
                    {
                        var errorMessageList = requestQuestion.ValidatorErrorMessage.Split(';');
                        List<QuestionValidator> expressions = new List<QuestionValidator>();
                        for (int j = 0; j < expressionList.Count(); j++)
                        {
                            QuestionValidator expression = new QuestionValidator();
                            expression.Expression = expressionList[j];
                            expression.ErrorMessage = errorMessageList[j];
                            expression.Type = "Expression";
                            expression.ValidateOn = requestQuestion.ValidateOn;
                            expressions.Add(expression);
                        }
                        question.Validator = expressions;
                    }
                }
                question.SubQuestions = BuildQuestionHierarchy(allQuestions, allQuestions.Where(x => x.ParentQuestionId == requestQuestion.Id), question.QuestionNumber, String.IsNullOrEmpty(question.QuestionNumber) ? i.ToString() : "1", user, SkipQuestionNumberCreation(requestQuestion), hra);
                questions.Add(question);
                if (IsNestedQuestionHeading(requestQuestion.ControlTypeId))
                {
                    i = question.SubQuestions.Max(x => Int32.Parse(x.QuestionNumber));
                }
                i++;
            }
            return questions;
        }

        static bool IsNestedQuestionHeading(int controlTypeId)
        {
            return controlTypeId == ControlTypeDto.StartHeadingWithYesNoLabel.Id;// || controlTypeId == ControlTypeDto.GroupWithNoneQuestion.Id;
        }

        static bool SkipQuestionNumberCreation(QuestionDto question)
        {
            return question.QuestionCategoryId == QuestionCategoryDto.HealthMeasurements.Id || question.ControlTypeId == ControlTypeDto.GroupWithNoneQuestion.Id || question.ControlTypeId == ControlTypeDto.GroupWithAllQuestion.Id;
        }

        #region Select List 

        static IEnumerable<ControlValue> YesNoControlValues()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Yes", Value = "1" });
            values.Add(new ControlValue() { Text = "No", Value = "2" });
            return values;
        }

        static IEnumerable<ControlValue> YesNoDontKnowControlValues()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Yes", Value = "1" });
            values.Add(new ControlValue() { Text = "No", Value = "2" });
            values.Add(new ControlValue() { Text = "Don't know", Value = "3" });
            return values;
        }

        static IEnumerable<ControlValue> PresMedicationControlValues()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Seasonal allergies", Value = "SEASONAL_ALLERGIES" });
            values.Add(new ControlValue() { Text = "Heartburn/acid reflux", Value = "ACID_REFLUX" });
            values.Add(new ControlValue() { Text = "Ulcer", Value = "ULCER" });
            values.Add(new ControlValue() { Text = "Migraines", Value = "MIGRAINES" });
            values.Add(new ControlValue() { Text = "Osteoporosis", Value = "OSTEOPOROSIS" });
            values.Add(new ControlValue() { Text = "Anxiety", Value = "ANXIETY" });
            values.Add(new ControlValue() { Text = "Depression", Value = "DEPRESSION" });
            values.Add(new ControlValue() { Text = "Low back pain", Value = "LOW_BACK_PAIN" });
            values.Add(new ControlValue() { Text = "No, I have not taken prescription medication for any of these conditions during the past twelve months", Value = "NO" });
            return values;
        }

        static IEnumerable<ControlValue> OtherTobaccoTypeControlValues()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Smoke cigars?", Value = "SMOKE_CIGARS" });
            values.Add(new ControlValue() { Text = "Smoke a pipe?", Value = "SMOKE_PIPE" });
            values.Add(new ControlValue() { Text = "Use 'smokeless' tobacco ? ", Value = "SMOKELESS_TOBACOO" });
            values.Add(new ControlValue() { Text = "Smoke hookah, water pipes, kreteks, sticks, or bidis?", Value = "SMOKE_HOOKAH" });
            values.Add(new ControlValue() { Text = "Use any other form of tobacco?", Value = "SOMKE_OTHER" }); return values;
        }

        static IEnumerable<ControlValue> PhysicalProblemsTypeControlValues()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Arthritis", Value = "ARTHRITIS" });
            values.Add(new ControlValue() { Text = "Breathing problem", Value = "BREATHING_PROBLEM" });
            values.Add(new ControlValue() { Text = "Back injury", Value = "BACK_INJURY" });
            values.Add(new ControlValue() { Text = "Chronic pain", Value = "CHRONIC_PAIN" });
            values.Add(new ControlValue() { Text = "Other physical limitation", Value = "OTHER_PROBLEMS" }); return values;
        }

        static IEnumerable<ControlValue> SleepApneaControlValues()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Despite an adequate number of hours of sleep, I still feel tired or sleepy upon awakening.", Value = "FEEL_TIRED" });
            values.Add(new ControlValue() { Text = "I snore loudly each night.", Value = "SNORE_LOUDLY" });
            values.Add(new ControlValue() { Text = "I have been told that I have frequent pauses in breathing while sleeping.", Value = "PAUSES_BREATHING" });
            values.Add(new ControlValue() { Text = "I have headaches in the morning.", Value = "HEADACHES" });
            values.Add(new ControlValue() { Text = "I feel very tired or sleepy during the day.", Value = "FEEL-TIERD" });
            values.Add(new ControlValue() { Text = "None of the above apply to me.", Value = "NO" }); return values;
        }

        static IEnumerable<ControlValue> GetExams()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Complete physical exam during the past three years.", Value = "COMPLETE_PHYSICAL_EXAM" });
            values.Add(new ControlValue() { Text = "Stool test for colon cancer during the past 12 months.", Value = "STOOL_TEST" });
            values.Add(new ControlValue() { Text = "Sigmoidoscopy during the past five years.", Value = "SIGMOIDOSCOPY" });
            values.Add(new ControlValue() { Text = "Colonoscopy during the past 10 years.", Value = "COLONOSCOPY" });
            values.Add(new ControlValue() { Text = "Pap test during the past three years", Value = "PAP_TEST" });
            values.Add(new ControlValue() { Text = "PSA test for prostate cancer during the past three years.", Value = "PSA_TEST" });
            values.Add(new ControlValue() { Text = "Bone density test for osteoporosis during the past three years", Value = "BONE_DENSITY_TEST" });
            values.Add(new ControlValue() { Text = "Mammogram during the past two years", Value = "MAMMOGRAM" });
            values.Add(new ControlValue() { Text = "Dental exam during the past 12 months.", Value = "DENTAL_EXAM" });
            values.Add(new ControlValue() { Text = "Blood pressure during the past 12 months.", Value = "BLOOD_PRESSURE" });
            values.Add(new ControlValue() { Text = "Fasting cholesterol test during the past 12 months.", Value = "FAST_CHOL_TEST" });
            values.Add(new ControlValue() { Text = "Fasting glucose (blood sugar) during the past 12 months.", Value = "FAST_GLU" });
            values.Add(new ControlValue() { Text = "Eye exam during the past two years, or during the past 12 months if you have diabetes.", Value = "EYE_EXAM" });
            values.Add(new ControlValue() { Text = "No, I have not had any of the above checkups/tests.", Value = "NO" });
            return values;
        }

        static IEnumerable<ControlValue> GetImmunizations()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Tetanus vaccine during the past 10 years.", Value = "TETANUS_VACCINE" });
            values.Add(new ControlValue() { Text = "Flu vaccine during the past 12 months.", Value = "FLU_VACCINE" });
            values.Add(new ControlValue() { Text = "Measles - Mumps - Rubella vaccine (ever). Note: Please also check this box if you know for certain you have had all three of these illnesses in the past", Value = "MEASLES_VACCINE" });
            values.Add(new ControlValue() { Text = "Chickenpox vaccine (ever). Note: Please also check this box if you know for certain you have had chickenpox in the past", Value = "CHICKENPOX_VACCINE" });
            values.Add(new ControlValue() { Text = "Hepatitis B vaccine (ever).", Value = "HEP_B_VACCINE" });
            values.Add(new ControlValue() { Text = "Shingles vaccine (ever).", Value = "SHINGLES_VACCINE" });
            values.Add(new ControlValue() { Text = "HPV (Human Papillomavirus) vaccine (ever).", Value = "HPV_VACCINE" });
            values.Add(new ControlValue() { Text = "Pneumonia vaccine (ever).", Value = "PNEUMONIA_VACCINE" });
            values.Add(new ControlValue() { Text = "No, I have not had any of the above immunizations/vaccines.", Value = "NO" });
            return values;
        }

        static IEnumerable<ControlValue> GetInterest()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "0", Value = "0" });
            values.Add(new ControlValue() { Text = "1", Value = "1" });
            values.Add(new ControlValue() { Text = "2", Value = "2" });
            values.Add(new ControlValue() { Text = "3", Value = "3" });
            values.Add(new ControlValue() { Text = "4", Value = "4" });
            values.Add(new ControlValue() { Text = "5", Value = "5" });
            values.Add(new ControlValue() { Text = "6", Value = "6" });
            values.Add(new ControlValue() { Text = "7", Value = "7" });
            values.Add(new ControlValue() { Text = "8", Value = "8" });
            values.Add(new ControlValue() { Text = "9", Value = "9" });
            return values;
        }

        static IEnumerable<ControlValue> ListCountries()
        {
            List<ControlValue> values = new List<ControlValue>();
            CommonReader reader = new CommonReader();
            ListCountriesRequest request = new ListCountriesRequest();
            IList<CountryDto> Countries = reader.ListCountries(request).Countries;
            foreach (var country in Countries)
            {
                if (country.UNCode == "USA")
                    values.Add(new ControlValue() { Text = country.Name, Value = country.Id.ToString() });
            }
            return values;
        }

        static IEnumerable<ControlValue> ListStates()
        {
            List<ControlValue> values = new List<ControlValue>();
            CommonReader reader = new CommonReader();
            ListCountriesRequest countryRequest = new ListCountriesRequest();
            ListStatesRequest request = new ListStatesRequest();
            request.CountryId = reader.ListCountries(countryRequest).Countries.Where(x => x.UNCode == "USA").FirstOrDefault().Id;
            IList<StateDto> States = reader.ListStates(request).States;
            foreach (var state in States)
            {
                values.Add(new ControlValue() { Text = state.Name, Value = state.Id.ToString() });
            }
            return values;
        }

        static IEnumerable<ControlValue> GetTimeZones()
        {
            List<ControlValue> values = new List<ControlValue>();
            CommonReader reader = new CommonReader();
            ReadTimeZonesRequest request = new ReadTimeZonesRequest();
            IList<TimeZoneDto> TimeZones = reader.GetTimeZones(request).TimeZones;
            foreach (var TimeZone in TimeZones)
            {
                values.Add(new ControlValue() { Text = TimeZone.TimeZoneDisplay, Value = TimeZone.Id.ToString() });
            }
            return values;
        }

        #endregion

        #region Restrictions
        static bool IsFemale(string gender)
        {
            return gender == "2";
        }

        static bool IsMale(string gender)
        {
            return gender == "1";
        }
        #endregion

        #region Save 

        public APISaveHRAQuestionResponse SaveHRAQuestion(APISaveHRAQuestionRequest request, int salesForceOrgId)
        {
            var saveResponse = new APISaveHRAQuestionResponse();
            List<int> pageSequence = new List<int>(10);
            if (request == null || request.Questions == null || request.Questions.Count() == 0)
            {
                saveResponse.StatusCode = HttpStatusCode.NoContent;
                return saveResponse;
            }
            string HRAPageSeqDone = "";
            int HRAId;
            DateTime? CompletedDate = null;
            List<QuestionDto> questions = hraReader.GetHRAQuestions(new GetHRAQuestionRequest()).Questions.ToList();
            MedicalConditionsDto medicalConditions = null;
            OtherRiskFactorsDto otherRiskFactors = null;
            HSPDto hsp = null;
            ExamsandShotsDto examsandShots = null;
            InterestsDto interests = null;
            HealthNumbersDto healthNumbers = null;

            ReadUserParticipationRequest userRequest = new ReadUserParticipationRequest();
            userRequest.UserId = Int32.Parse(request.UserId);
            var getUserResponse = Task.Run(() => accountReader.GetUser(new GetUserRequest() { id = userRequest.UserId })).Result.User;
            if (getUserResponse == null || getUserResponse.OrganizationId != salesForceOrgId)
            {
                saveResponse.StatusCode = HttpStatusCode.NotFound;
                saveResponse.ErrorMessage = "User Not Found";
                return saveResponse;
            }
            var user = reader.ReadUserParticipation(userRequest).user;

            ReadHRARequest hraRequest = new ReadHRARequest();
            hraRequest.userId = user.Id;
            hraRequest.portalId = user.Organization.Portals.Where(x => x.Active == true).FirstOrDefault().Id;
            var hraResponse = hraReader.ReadHRAByPortal(hraRequest);
            if (hraResponse.hra == null)
            {
                //create HRA Id
                CreateHRARequest HRArequest = new CreateHRARequest();
                HRADto HA = new HRADto();
                HA.UserId = user.Id;
                HA.PortalId = user.Organization.Portals[0].Id;
                HA.CreatedBy = user.Id;
                HRArequest.HRA = HA;
                HRArequest.languageCode = user.LanguagePreference;
                HRAId = hraReader.CreateHRA(HRArequest).HRAId;
            }
            else
            {
                HRAId = hraResponse.hra.Id;
                HRAPageSeqDone = hraResponse.hra.HAPageSeqDone;
                if (hraResponse.hra.CompleteDate.HasValue)
                    CompletedDate = hraResponse.hra.CompleteDate;
            }


            foreach (APISaveHRAQuestion requestQuestion in request.Questions)
            {

                var question = questions.First(x => x.Id == requestQuestion.QuestionId);
                if (question.MappingTableName == "HRA_MedicalConditions" && question.MappingColumnName != null)
                {
                    if (medicalConditions == null)
                    {
                        medicalConditions = new MedicalConditionsDto();
                        medicalConditions.AllergyMed = medicalConditions.RefluxMed = medicalConditions.UlcerMed = medicalConditions.MigraineMed = medicalConditions.OsteoporosisMed = medicalConditions.AnxietyMed = medicalConditions.DepressionMed = medicalConditions.BackPainMed = medicalConditions.NoPrescMed = 2;
                        medicalConditions.HRAId = HRAId;
                    }
                    Utils.SetPropertyValueFromString(medicalConditions, question.MappingColumnName, requestQuestion.QuestionValue);
                }
                else if (question.MappingTableName == "HRA_OtherRiskFactors" && question.MappingColumnName != null)
                {
                    if (otherRiskFactors == null)
                    {
                        otherRiskFactors = new OtherRiskFactorsDto();
                        otherRiskFactors.Cigar = otherRiskFactors.Pipe = otherRiskFactors.SmokelessTob = otherRiskFactors.SmokelessTob = otherRiskFactors.OtherFormofTob = otherRiskFactors.WaterPipes = otherRiskFactors.FeelTired = otherRiskFactors.Snore = otherRiskFactors.BreathPause = otherRiskFactors.Headache = otherRiskFactors.Sleepy = otherRiskFactors.NoIssue = otherRiskFactors.Arthritis = otherRiskFactors.BreathProb = otherRiskFactors.BackInjury = otherRiskFactors.ChronicPain = otherRiskFactors.OtherPhysLimit = 2;
                        otherRiskFactors.HRAId = HRAId;
                    }
                    Utils.SetPropertyValueFromString(otherRiskFactors, question.MappingColumnName, requestQuestion.QuestionValue);
                }
                else if (question.MappingTableName == "HRA_HSP" && question.MappingColumnName != null)
                {
                    if (hsp == null)
                    {
                        hsp = new HSPDto();
                        hsp.HRAId = HRAId;
                    }
                    Utils.SetPropertyValueFromString(hsp, question.MappingColumnName, requestQuestion.QuestionValue);
                }
                else if (question.MappingTableName == "HRA_ExamsandShots" && question.MappingColumnName != null)
                {
                    if (examsandShots == null)
                    {
                        examsandShots = new ExamsandShotsDto();
                        examsandShots.PhysicalExam = examsandShots.StoolTest = examsandShots.SigTest = examsandShots.ColTest = examsandShots.PSATest = examsandShots.PapTest = examsandShots.BoneTest = examsandShots.Mammogram = examsandShots.DentalExam = examsandShots.BPCheck = examsandShots.CholTest = examsandShots.GlucoseTest = examsandShots.EyeExam = examsandShots.NoTest = examsandShots.TetanusShot = examsandShots.FluShot = examsandShots.MMR = examsandShots.Varicella = examsandShots.HepBShot = examsandShots.ShinglesShot = examsandShots.HPVShot = examsandShots.PneumoniaShot = examsandShots.NoShots = 2;
                        examsandShots.HRAId = HRAId;
                    }
                    Utils.SetPropertyValueFromString(examsandShots, question.MappingColumnName, requestQuestion.QuestionValue);
                }
                else if (question.MappingTableName == "HRA_Interests" && question.MappingColumnName != null)
                {
                    if (interests == null)
                    {
                        interests = new InterestsDto();
                        interests.HRAId = HRAId;
                    }
                    Utils.SetPropertyValueFromString(interests, question.MappingColumnName, requestQuestion.QuestionValue);
                }
                else if (question.MappingTableName == "HRA_HealthNumbers" && question.MappingColumnName != null)
                {
                    if (healthNumbers == null)
                    {
                        healthNumbers = new HealthNumbersDto();
                        healthNumbers.HRAId = HRAId;
                    }
                    Utils.SetPropertyValueFromString(healthNumbers, question.MappingColumnName, requestQuestion.QuestionValue);
                }

            }
            if (medicalConditions != null)
            {
                AddEditMedicalConditionsRequest medicalConditionsrequest = new AddEditMedicalConditionsRequest();
                medicalConditionsrequest.UserId = user.Id;
                medicalConditionsrequest.UpdatedByUserId = user.Id;
                medicalConditionsrequest.StoreHistory = false;
                medicalConditionsrequest.medicalCondition = medicalConditions;
                if (!CompletedDate.HasValue && (HRAPageSeqDone == null || !HRAPageSeqDone.Contains("MC")))
                {
                    HRADto HRA = new HRADto();
                    HRA.HAPageSeqDone = HRAPageSeqDone = HRAPageSeqDone + "MC.";
                    if (GetPageSeqDetails(HRAPageSeqDone))
                        CompletedDate = HRA.CompleteDate = DateTime.UtcNow;
                    medicalConditionsrequest.medicalCondition.HRA = HRA;
                }
                var response = hraReader.AddEditMedicalCondition(medicalConditionsrequest);
                pageSequence.Add(1);
            }
            if (otherRiskFactors != null)
            {
                AddEditOtherRiskFactorsRequest otherRiskFactorsrequest = new AddEditOtherRiskFactorsRequest();
                otherRiskFactorsrequest.UserId = user.Id;
                otherRiskFactorsrequest.OtherRiskFactors = otherRiskFactors;
                otherRiskFactorsrequest.UpdatedByUserId = user.Id;
                otherRiskFactorsrequest.StoreHistory = false;
                if (!CompletedDate.HasValue && (HRAPageSeqDone == null || !HRAPageSeqDone.Contains("OR")))
                {
                    HRADto HRA = new HRADto();
                    HRA.HAPageSeqDone = HRAPageSeqDone = HRAPageSeqDone + "OR.";
                    if (GetPageSeqDetails(HRAPageSeqDone))
                        CompletedDate = HRA.CompleteDate = DateTime.UtcNow;
                    otherRiskFactorsrequest.OtherRiskFactors.HRA = HRA;
                }
                var response = hraReader.AddEditOtherRisks(otherRiskFactorsrequest);
                pageSequence.Add(2);
            }
            if (hsp != null)
            {
                AddEditHSPRequest HSPrequest = new AddEditHSPRequest();
                HSPrequest.UserId = user.Id;
                HSPrequest.HSP = hsp;
                HSPrequest.UpdatedByUserId = user.Id;
                HSPrequest.StoreHistory = false;
                if (!CompletedDate.HasValue && (HRAPageSeqDone == null || !HRAPageSeqDone.Contains("YL")))
                {
                    HRADto HRA = new HRADto();
                    HRA.HAPageSeqDone = HRAPageSeqDone = HRAPageSeqDone + "YL.";
                    if (GetPageSeqDetails(HRAPageSeqDone))
                        CompletedDate = HRA.CompleteDate = DateTime.UtcNow;
                    HSPrequest.HSP.HRA = HRA;
                }
                var response = hraReader.AddEditHSP(HSPrequest);
                pageSequence.Add(3);
            }
            if (examsandShots != null)
            {
                AddEditExamsandShotsRequest examsandShotsrequest = new AddEditExamsandShotsRequest();
                examsandShotsrequest.UserId = user.Id;
                examsandShotsrequest.exams = examsandShots;
                examsandShotsrequest.UpdatedByUserId = user.Id;
                examsandShotsrequest.StoreHistory = false;
                if (!CompletedDate.HasValue && (HRAPageSeqDone == null || !HRAPageSeqDone.Contains("EC")))
                {
                    HRADto HRA = new HRADto();
                    HRA.HAPageSeqDone = HRAPageSeqDone = HRAPageSeqDone + "EC.";
                    if (GetPageSeqDetails(HRAPageSeqDone))
                        CompletedDate = HRA.CompleteDate = DateTime.UtcNow;
                    examsandShotsrequest.exams.HRA = HRA;
                }
                var response = hraReader.AddEditExams(examsandShotsrequest);
                pageSequence.Add(4);
            }
            if (interests != null)
            {
                AddEditInterestsRequest interestsrequest = new AddEditInterestsRequest();
                interestsrequest.UserId = user.Id;
                interestsrequest.interest = interests;
                interestsrequest.UpdatedByUserId = user.Id;
                interestsrequest.StoreHistory = false;
                if (!CompletedDate.HasValue && (HRAPageSeqDone == null || !HRAPageSeqDone.Contains("IN")))
                {
                    HRADto HRA = new HRADto();
                    HRA.HAPageSeqDone = HRAPageSeqDone = HRAPageSeqDone + "IN.";
                    if (GetPageSeqDetails(HRAPageSeqDone))
                        CompletedDate = HRA.CompleteDate = DateTime.UtcNow;
                    interestsrequest.interest.HRA = HRA;
                }
                var response = hraReader.AddEditInterest(interestsrequest);
                pageSequence.Add(5);
            }
            if (healthNumbers != null)
            {
                AddEditHealthNumbersRequest healthNumbersrequest = new AddEditHealthNumbersRequest();
                healthNumbersrequest.UserId = user.Id;
                healthNumbersrequest.HealthNumbers = healthNumbers;
                healthNumbersrequest.UpdatedByUserId = user.Id;
                healthNumbersrequest.StoreHistory = false;
                if (!CompletedDate.HasValue && (HRAPageSeqDone == null || !HRAPageSeqDone.Contains("YN")))
                {
                    HRADto HRA = new HRADto();
                    HRA.HAPageSeqDone = HRAPageSeqDone = HRAPageSeqDone + "YN.";
                    if (GetPageSeqDetails(HRAPageSeqDone))
                        CompletedDate = HRA.CompleteDate = DateTime.UtcNow;
                    healthNumbersrequest.HealthNumbers.HRA = HRA;
                }
                if (healthNumbers.Height.HasValue)
                {
                    healthNumbers.Height = (healthNumbers.Height * 12);
                    if (healthNumbers.HeightInch.HasValue && healthNumbers.HeightInch.Value > 0)
                        healthNumbers.Height = healthNumbers.HeightInch + healthNumbers.Height;
                }
                var response = hraReader.AddEditHealthNumbers(healthNumbersrequest);
                pageSequence.Add(6);
            }
            saveResponse.PageSequenceDone = pageSequence.ToArray();
            saveResponse.StatusCode = HttpStatusCode.OK;
            return saveResponse;
        }

        public APIProfileSaveResponse SaveUserProfile(APISaveUserProfileRequest request, int salesForceOrgId)
        {
            var saveResponse = new APIProfileSaveResponse();
            if (request == null || request.Questions == null || request.Questions.Count() == 0)
            {
                saveResponse.StatusCode = HttpStatusCode.NoContent;
                return saveResponse;
            }
            List<QuestionDto> questions = hraReader.GetHRAQuestions(new GetHRAQuestionRequest()).Questions.ToList();
            UserDto user = null;
            foreach (APISaveUserProfileQuestion requestQuestion in request.Questions)
            {
                var question = questions.First(x => x.Id == requestQuestion.QuestionId);
                if (question.MappingTableName == "Users" && question.MappingColumnName != null)
                {
                    if (user == null)
                    {
                        user = new UserDto();
                    }
                    Utils.SetPropertyValueFromString(user, question.MappingColumnName, requestQuestion.QuestionValue);
                }
            }
            if (user != null)
            {
                if (request.UserId == null)
                {
                    RegisterUserRequest userRequest = new RegisterUserRequest();
                    user.OrganizationId = salesForceOrgId;
                    userRequest.User = user;
                    var response = Task.Run(() => accountReader.ApiCreateUser(userRequest)).Result;
                    saveResponse.StatusCode = response.Succeeded ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
                    if (response.Succeeded)
                        saveResponse.UserId = response.userId;
                    else
                        saveResponse.ErrorMessage = response.error.FirstOrDefault().Description;
                }
                else
                {
                    user.Id = Int32.Parse(request.UserId);
                    var getUserResponse = Task.Run(() => accountReader.GetUser(new GetUserRequest() { id = user.Id })).Result.User;
                    if (getUserResponse != null && getUserResponse.OrganizationId == salesForceOrgId)
                    {
                        UpdateUserRequest userRequest = new UpdateUserRequest();
                        user.IsActive = true;
                        userRequest.user = user;
                        userRequest.UpdatedByUserId = user.Id;
                        var response = Task.Run(() => accountReader.UpdateUser(userRequest)).Result;
                        saveResponse.StatusCode = response.Succeeded ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
                        if (response.Succeeded)
                            saveResponse.UserId = user.Id;
                        else
                            saveResponse.ErrorMessage = response.error.FirstOrDefault().Description;
                    }
                    else
                    {
                        saveResponse.StatusCode = HttpStatusCode.NotFound;
                        saveResponse.ErrorMessage = "User Not Found";
                    }
                }
            }
            return saveResponse;
        }

        public static bool GetPageSeqDetails(string PageSeq)
        {
            if (PageSeq.Contains("MC") && PageSeq.Contains("OR") && PageSeq.Contains("YL") && PageSeq.Contains("EC") && PageSeq.Contains("IN") && PageSeq.Contains("YN"))
                return true;
            else
                return false;
        }

        private class SelectListItem : ListItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }

        #endregion

        public int LivongoWeeklyProcess(string livongoHRAsFilePath)
        {
            CommonReader commonReader = new CommonReader();
            ParticipantReader reader = new ParticipantReader();
            var livongoHRAs = reader.LivongoWeeklyHRAProcess();
            var builder = new StringBuilder();
            var header = builder.AppendLine("Company|Coverage_Start_Date|Coverage_End_Date|Type_of_Coverage|Member_First_Name|Member_Last_Name|SSN|Member_ID|Member_Gender|Member_Date_of_Birth|Member_Phone|Member_Email|Member_Address1|Member_Address2|Member_City|Member_State|Member_Zip|Language_Preference|A1c|Date_of_A1c|Fasting_Blood_Glucose|WasGlucoseTestFasting|Date_of_HRA_Blood_Test|Self_Identified_Having_Diabetes|Intervent_Detected_Diabetes|Take_Antidiabetic_Meds|Take_Insulin|Intervention_From_INTERVENT|Business_Unit");
            IEnumerable<string> response = livongoHRAs
                .Select(x => String.Join("|", x.Company,
                x.Coverage_Start_Date,
                x.Coverage_End_Date,
                x.Type_of_Coverage,
                x.Member_First_Name,
                x.Member_Last_Name,
                x.SSN,
                x.Member_ID,
                x.Member_Gender,
                x.Member_Date_of_Birth,
                x.Member_Phone,
                x.Member_Email,
                x.Member_Address1,
                x.Member_Address2,
                x.Member_City,
                x.Member_State,
                x.Member_Zip,
                x.Language_Preference,
                x.A1c,
                x.Date_of_A1c,
                x.Fasting_Blood_Glucose,
                x.WasGlucoseTestFasting,
                x.Date_of_HRA_Blood_Test,
                x.Self_Identified_Having_Diabetes,
                x.Intervent_Detected_Diabetes,
                x.Take_Antidiabetic_Meds,
                x.Take_Insulin,
                x.Intervention_From_INTERVENT,
                x.Business_Unit
                ));
            builder.AppendLine(String.Join(Environment.NewLine, response));
            System.IO.File.WriteAllText(
                System.IO.Path.Combine(
                livongoHRAsFilePath, livongoHRAFileName),
                builder.ToString());
            int[] livongoDateColumnIndexes = new int[] { 2, 3, 10, 20, 23 };
            var livongoTextColumnIndexes = new Dictionary<int, string>();
            livongoTextColumnIndexes.Add(7, "000000000");
            livongoTextColumnIndexes.Add(17, "00000");
            commonReader.ConverttoExcel(Path.Combine(livongoHRAsFilePath, livongoHRAFileName), Path.Combine(livongoHRAsFilePath, livongoHRAExcelFileName), "LivongoHRAData", '|', livongoDateColumnIndexes, livongoTextColumnIndexes);
            if (File.Exists(Path.Combine(livongoHRAsFilePath, livongoHRAFileName)))
            {
                // If file found, delete it    
                File.Delete(Path.Combine(livongoHRAsFilePath, livongoHRAFileName));
            }
            return livongoHRAs.Count();
        }
    }

    public static class Utils
    {
        /// <summary>
        /// Sets a value in an object, used to hide all the logic that goes into
        ///     handling this sort of thing, so that is works elegantly in a single line.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public static void SetPropertyValueFromString(this object target,
                                      string propertyName, string propertyValue)
        {
            PropertyInfo oProp = target.GetType().GetProperty(propertyName);
            Type tProp = oProp.PropertyType;

            //Nullable properties have to be treated differently, since we 
            //  use their underlying property to set the value in the object
            if (tProp.IsGenericType
                && tProp.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                //if it's null, just set the value from the reserved word null, and return
                if (propertyValue == null)
                {
                    oProp.SetValue(target, null, null);
                    return;
                }

                //Get the underlying type property instead of the nullable generic
                tProp = new NullableConverter(oProp.PropertyType).UnderlyingType;
            }

            //use the converter to get the correct value
            oProp.SetValue(target, Convert.ChangeType(propertyValue, tProp), null);
        }

        public static void SetPropertyValuetoString(this object target, object property)
        {
            string propertyName = target.GetType().GetProperties().Single(pi => pi.Name == "MappingColumnName").GetValue(target, null).ToString();
            var value = property.GetType().GetProperties().Single(pi => pi.Name == propertyName).GetValue(property, null);
            if (value != null)
            {
                PropertyInfo oProp = target.GetType().GetProperty("DefaultValue");
                oProp.SetValue(target, value.ToString(), null);
            }
        }

        public static string GetValue(object property, string propertyName)
        {
            string Height = null;
            var value = property.GetType().GetProperties().Single(pi => pi.Name == propertyName).GetValue(property, null);
            if (value != null)
            {
                Height = value.ToString();
            }
            return Height;
        }
    }
}
