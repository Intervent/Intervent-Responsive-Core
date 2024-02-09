namespace Intervent.Web.DTO
{
    public class ActivityDetailsDto
    {
        public string StepName { get; set; }
        public int StepNo { get; set; }

        public bool IsActivity { get; set; }

        public ActivityDetailsDto Clone()
        {
            ActivityDetailsDto clone = new ActivityDetailsDto();
            clone.StepName = this.StepName;
            clone.StepNo = this.StepNo;
            clone.IsActivity = this.IsActivity;
            return clone;
        }
    }
}
