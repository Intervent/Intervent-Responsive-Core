using System.Net;

namespace Intervent.Web.DTO
{
    public class HRAQuestionResponse
    {
        public IEnumerable<QuestionDto> Questions { get; set; }
    }

    public class ControlValue
    {
        public string Text { get; set; }

        public string DisplayText { get; set; }

        public string Value { get; set; }
    }
    public class ListItem
    {
        public string Text { get; set; }

        public string Value { get; set; }
    }

    public class QuestionValidator
    {
        public string ValidateOn { get; set; }

        public string Type { get; set; }

        public string Expression { get; set; }

        public string ErrorMessage { get; set; }

    }

    public class Condition
    {
        public int ParentQuestionId { get; set; }
        public string Value { get; set; }
    }

    public class Question
    {
        public int QuestionId { get; set; }
        public string QuestionNumber { get; set; }
        public string QuestionText { get; set; }
        public string ControlType { get; set; }
        public IEnumerable<ListItem> ControlValues { get; set; }
        public string DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public string RequiredExpression { get; set; }
        public IEnumerable<QuestionValidator> Validator { get; set; }
        public int SortOrder { get; set; }
        public string HelpText { get; set; }
        public Condition Condition { get; set; }
        public string Row { get; set; }
        public string Column { get; set; }
        public string ControlSuffix { get; set; }
        public string Placeholder { get; set; }
        public bool AllowDecimalInput { get; set; }
        public IEnumerable<Question> SubQuestions { get; set; }
    }

    public class QuestionCategory
    {
        public int CategoryId { get; set; }

        public string CategoryDescription { get; set; }

        public string Completed { get; set; }

        public IEnumerable<Question> Questions { get; set; }
    }

    public class APIHRAQuestionResponse
    {
        public IEnumerable<QuestionCategory> Categories { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string ErrorMessage { get; set; }
    }

    //public class APIHRASaveResponse
    //{
    //    public bool Status { get; set; }
    //}

    public class APIProfileSaveResponse
    {

        public int UserId { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string ErrorMessage { get; set; }


    }

    //public class ProfileSaveResponse
    //{
    //    public HttpStatusCode StatusCode { get; set; }

    //    public string ErrorMessage { get; set; }

    //    public APIProfileSaveResponse SaveResponse { get; set; }
    //}

    public class APISaveHRAQuestionResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public int[] PageSequenceDone { get; set; }
    }

    public class APISaveHRAQuestionRequest
    {

        public IEnumerable<APISaveHRAQuestion> Questions { get; set; }

        public string UserId { get; set; }
    }

    public class APISaveHRAQuestion
    {
        public int QuestionId { get; set; }

        public string QuestionValue { get; set; }
    }
    public class APISaveUserProfileRequest
    {
        public IEnumerable<APISaveUserProfileQuestion> Questions { get; set; }

        public string UserId { get; set; }

    }

    public class APISaveUserProfileQuestion
    {
        public int QuestionId { get; set; }

        public string QuestionValue { get; set; }
    }
}
