using Intervent.Web.DTO;

namespace Intervent.Business.EmailTriggers
{
    public class EnrollInSelfHelpButNotCoachingTrigger : BaseEmailTrigger, IEmailTrigger
    {
        public override int NotificationTemplateId
        {
            get { return 5; }
        }


        public override int NotificationEventTypeId
        {
            get { return 5; }
        }

        public override EmailTriggerCondition Trigger
        {
            get { return EmailTriggerCondition.IncompleteLabButEnrolledInSelfHelp; }
        }
    }
}
