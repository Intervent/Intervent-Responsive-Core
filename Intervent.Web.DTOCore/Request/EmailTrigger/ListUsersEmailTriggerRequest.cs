namespace Intervent.Web.DTO
{
    public class ListUsersEmailTriggerRequest
    {
        public int NotificationEventTypeId { get; set; }

        public EmailTriggerCondition TriggerCondition { get; set; }

    }

    public enum EmailTriggerCondition { AppointmentConfirmation, IncompletProfile, IncompleteHRA, IncompleteLab, IncompleteLabButEnrolledInSelfHelp, WeeklyAppointmentReminder, FollowUp };
}
