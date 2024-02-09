namespace Intervent.Web.DTO
{
    public class NotificationEventTypeDto
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public virtual NotificationCategoryDto NotificationCategory { get; set; }

        public NotificationEventTypeDto()
        {

        }

        private NotificationEventTypeDto(int id, string desc)
        {
            Id = id;
            Description = desc;
        }

        public static NotificationEventTypeDto IncompleteProfile = new NotificationEventTypeDto(1, "Incomplete Profile");
        public static NotificationEventTypeDto IncompleteHra = new NotificationEventTypeDto(2, "Incomplete HRA");
        public static NotificationEventTypeDto IncompleteLab = new NotificationEventTypeDto(3, "Incomplete Lab");
        public static NotificationEventTypeDto NotEnrolledInCoaching = new NotificationEventTypeDto(4, "Not enrolled in coaching");
        public static NotificationEventTypeDto EnrolledInSelfHelpButNotCoaching = new NotificationEventTypeDto(5, "Enrolled in self help but not coaching");
        public static NotificationEventTypeDto AppointmentConfirmation = new NotificationEventTypeDto(6, "Appointment Confirmation");
        public static NotificationEventTypeDto WeeklyAppointmentReminder = new NotificationEventTypeDto(7, "Weekly appointment reminder");
        public static NotificationEventTypeDto MissedAppointment = new NotificationEventTypeDto(8, "Missed appointment");
        public static NotificationEventTypeDto Tracking = new NotificationEventTypeDto(9, "Tracking");
        public static NotificationEventTypeDto ReviewKit = new NotificationEventTypeDto(10, "Review kit");
        public static NotificationEventTypeDto FollowUp = new NotificationEventTypeDto(11, "Follow Up");
        public static NotificationEventTypeDto Welcome = new NotificationEventTypeDto(12, "Welcome");
        public static NotificationEventTypeDto ChangePassword = new NotificationEventTypeDto(13, "Change Password");
        public static NotificationEventTypeDto ChangeEmail = new NotificationEventTypeDto(14, "Change Email");
        public static NotificationEventTypeDto SubmitLabResults = new NotificationEventTypeDto(15, "Submit Lab Results to Doctors Office");
        public static NotificationEventTypeDto CompleteLabswithLabCorp = new NotificationEventTypeDto(16, "Completing Your Labs with Your LabCorp");
        public static NotificationEventTypeDto LabSuccessfullyCompleted = new NotificationEventTypeDto(17, "Lab Successfully Completed");
        public static NotificationEventTypeDto DoctorsOfficeReminder = new NotificationEventTypeDto(18, "Remainder for Doctors Office");
        public static NotificationEventTypeDto LabCorpReminder = new NotificationEventTypeDto(19, "Remainder for Lab Crop");
        public static NotificationEventTypeDto CriticalAlert = new NotificationEventTypeDto(20, "Critical Alert for Dr.Gordon");
        public static NotificationEventTypeDto NewEmail = new NotificationEventTypeDto(21, "New mail");
        public static NotificationEventTypeDto ForgotPassword = new NotificationEventTypeDto(22, "Forgot Password");
        public static NotificationEventTypeDto ResendConfirmtaion = new NotificationEventTypeDto(23, "Resend Confirmtaion");
        public static NotificationEventTypeDto BirthdayWishes = new NotificationEventTypeDto(24, "Birthday Wishes");
        public static NotificationEventTypeDto RejectedLabs = new NotificationEventTypeDto(26, "Rejected Labs");
        public static NotificationEventTypeDto LMCProgram = new NotificationEventTypeDto(27, "LMC Program");
        public static NotificationEventTypeDto QualifyforDPP = new NotificationEventTypeDto(28, "Qualify for DPP");
        public static NotificationEventTypeDto NotQualifyforDPP = new NotificationEventTypeDto(29, "Not Qualify for DPP");
        public static NotificationEventTypeDto OneStepCloser = new NotificationEventTypeDto(30, "One Step Closer");
        public static NotificationEventTypeDto POGOStarted = new NotificationEventTypeDto(31, "POGO Started");
        public static NotificationEventTypeDto FollowUpLabs = new NotificationEventTypeDto(32, "Follow Up Labs");
        public static NotificationEventTypeDto MissedOutreach = new NotificationEventTypeDto(33, "Missed Outreach");
        public static NotificationEventTypeDto SystemDowntime = new NotificationEventTypeDto(34, "SystemDowntime");
        public static NotificationEventTypeDto POGOTerminated = new NotificationEventTypeDto(35, "POGO Terminated");
        public static NotificationEventTypeDto KitCompletionMotivation = new NotificationEventTypeDto(36, "Kit Completion - Motivation");
        public static NotificationEventTypeDto KitCompletionStayOnTrack = new NotificationEventTypeDto(37, "Kit Completion - Stay On Track");
        public static NotificationEventTypeDto KitCompletionKnowingIsNotDoing = new NotificationEventTypeDto(38, "Kit Completion - Knowing Is Not Doing");
        public static NotificationEventTypeDto KitCompletionRecommitToYourHealth = new NotificationEventTypeDto(39, "Kit Completion - Recommit To Your Health");
        public static NotificationEventTypeDto KitCompletionDontDelayAnyLonger = new NotificationEventTypeDto(40, "Kit Completion - Dont Delay Any Longer");
        public static NotificationEventTypeDto MotivaitonMail = new NotificationEventTypeDto(41, "Motivaiton Mail");
        public static NotificationEventTypeDto SecurityCode = new NotificationEventTypeDto(42, "Security Code");

        static IEnumerable<NotificationEventTypeDto> GetAll()
        {
            List<NotificationEventTypeDto> lst = new List<NotificationEventTypeDto>();
            lst.Add(IncompleteProfile);
            lst.Add(IncompleteHra);
            lst.Add(IncompleteLab);
            lst.Add(NotEnrolledInCoaching);
            lst.Add(EnrolledInSelfHelpButNotCoaching);
            lst.Add(AppointmentConfirmation);
            lst.Add(WeeklyAppointmentReminder);
            lst.Add(MissedAppointment);
            lst.Add(Tracking);
            lst.Add(ReviewKit);
            lst.Add(FollowUp);
            lst.Add(Welcome);
            lst.Add(ChangePassword);
            lst.Add(ChangeEmail);
            lst.Add(SubmitLabResults);
            lst.Add(CompleteLabswithLabCorp);
            lst.Add(LabSuccessfullyCompleted);
            lst.Add(DoctorsOfficeReminder);
            lst.Add(LabCorpReminder);
            lst.Add(CriticalAlert);
            lst.Add(NewEmail);
            lst.Add(ForgotPassword);
            lst.Add(ResendConfirmtaion);
            lst.Add(BirthdayWishes);
            lst.Add(RejectedLabs);
            lst.Add(LMCProgram);
            lst.Add(QualifyforDPP);
            lst.Add(NotQualifyforDPP);
            lst.Add(OneStepCloser);
            lst.Add(POGOStarted);
            lst.Add(FollowUpLabs);
            lst.Add(MissedOutreach);
            lst.Add(SystemDowntime);
            lst.Add(POGOTerminated);
            lst.Add(KitCompletionMotivation);
            lst.Add(KitCompletionStayOnTrack);
            lst.Add(KitCompletionKnowingIsNotDoing);
            lst.Add(KitCompletionRecommitToYourHealth);
            lst.Add(KitCompletionDontDelayAnyLonger);
            lst.Add(MotivaitonMail);
            lst.Add(SecurityCode);
            return lst;
        }

        public static NotificationEventTypeDto GetByKey(int id)
        {
            return GetAll().Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
