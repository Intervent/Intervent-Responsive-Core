using System.ComponentModel;

namespace Intervent.Web.DTO
{
    public class Constants
    {
        public const string Tobacco_Affidavit_TT = "Tobacco Affidavit";
        public const string CDPP_Lab_Choice = "CDPP Lab Choice";
        public const string CDPP_Doctor_Choice = "CDPP Doctor Choice";
        public const string CDPP_Doctor_Labs = "CDPP Doctor Labs";
        public static string[] FollowUpPageSeq = new string[] { PageName.MC.ToString(), PageName.OR.ToString(), PageName.YL.ToString(), PageName.YN.ToString() };
        public const int HRAProgramValidity = 30;
    }
    public enum PageName
    {
        [Description("Medical Conditions")]
        MC,
        [Description("Other Risk Factors")]
        OR,
        [Description("Health Safety and Productivity")]
        YL,
        [Description("Exams and Checks")]
        EC,
        [Description("Interest and Motivation")]
        IN,
        [Description("Health Numbers")]
        YN
    }

    public enum IncentiveTypes
    {
        HRA_Completion = 1,
        Intervent_NSession = 2,
        Intervent_1stCoaching = 3,
        Maternity_Completion = 4,
        Tobacco_Initiative = 5,
        Participant_Survey = 6,
        FollowUp_Evaluation = 7,
        HRA_HealthNumbers = 8,
        Custom_Incentive = 9,
        Navigation_Incentive = 10,
        PCP_incentive = 11,
        Biometric_Review = 12,
        Vitals_Completion = 13,
        Kit_Completion = 14,
        Kit_Goal_Completion = 15
    }

    public enum ProgramInactiveReasons
    {
        SuccessfullyCompleted = 6,
        SystemAutoInactivation = 11,
        ProfileDeactivated = 12
    }

    public enum IntuityEventTypes
    {
        HRA_Completion = 1,
        FollowUp_Completion = 2,
        Coaching_Call_Completion = 3,
        Educational_Module_Update = 4,
        Webinar_Registration = 5,
        Call_Center_Interaction = 6,
        Future_Appointment = 7
    }

    public enum TaskTypes
    {
        Know_Your_Numbers = 5,
        Send_Postcard = 25,
        CDPP_Lab_Choice = 26,
        CDPP_Doctor_Choice = 27,
        CDPP_Doctor_Labs = 28
    }

    public enum TaskStatus
    {
        N,
        A,
        C
    }

    public enum ChangeLogsCategory
    {
        CRM_Contacts = 1,
        CRM_Notes = 2
    }

    public enum AppEnvironment { UAT, PROD }

    public enum EligibilityFormats
    {
        StandardFormat = 1,
        CompassFormat = 2,
        LMCFormat = 3,
        IntuityFormat = 4,
        MetLifeGulfFormat = 5,
        CaptivaFormat = 6
    }

    public enum NotificationTypes
    {
        Feed = 1,
        Notification = 2,
        Both = 3
    }

    public enum EligibilitytoIntuity
    {
        All = 1,
        OnlyDiabetes = 2
    }

    public enum MessageStatus
    {
        Sent = 1,
        NotSent = 2,
        DoNotSend = 3
    }

    public enum HealthDataSource
    {
        HRA = 1,
        FollowUp = 2,
        Labs = 3,
        VitalsLog = 4,
        WellnessData = 5,
        Devices = 6,
        WeightLossDiary = 7
    }

    public class WebHostList
    {
        public static readonly List<string> WebFormHostList = new List<string>{
            { "https://presspogo.com" },
            { "https://www.myintervent.com" },
            { "https://l1imi.myshopify.com" },
            { "https://shop.presspogo.com" },
            { "https://press-pogo-rs-staging.myshopify.com" }
        };
    }
}
