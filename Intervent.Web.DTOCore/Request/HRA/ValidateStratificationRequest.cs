namespace Intervent.Web.DTO
{
    public class ValidateStratificationRequest
    {
        public DateTime AssessmentCompletionDate { get; set; }

        public DateTime labCompletionDate { get; set; }

        public int HRAValidity { get; set; }
    }
}
