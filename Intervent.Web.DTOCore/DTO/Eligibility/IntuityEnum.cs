namespace Intervent.Web.DTO
{
    using System.ComponentModel;

    public enum EligibilityReason
    {
        [Description("None")]
        None = 0,
        [Description("HRA : Told diabetes")]
        ToldDiabetes = 3,
        [Description("HRA : Takes diabetes medication")]
        TakeMedication = 4,
        [Description("HRA : Fasting glucose out of range")]
        ToldDiabetes_FastingGlucose = 5,
        [Description("HRA : Non-fasting glucose out of range")]
        ToldDiabetes_NonFastingGlucose = 6,
        [Description("HRA : A1c out of range")]
        ToldDiabetes_A1CGlucose = 7,
        [Description("Enrollment Form : Told diabetes")]
        AnsweredDiabetes = 11,
        [Description("Enrollment Form : Takes diabetes medication")]
        Answered_Diabetes_Med = 16,
        [Description("Enrollment Form : A1C Value is outofrange")]
        A1C_OutOfRange = 12,
        [Description("No to diabetes/diabetes medication and lab value in range.")]
        NoMedHistoryLabNotOutofRange = 14,
        [Description("Terminated")]
        Terminated = 15,
        [Description("Retired")]
        Retired = 17,
        [Description("Leave of absence")]
        LoA = 18,
    }

    public enum EligibilityStatus
    {
        [Description("New")]
        New = 0,
        [Description("Eligible")]
        Eligible = 6,
        [Description("Not Eligible")]
        NotEligible = 7,
        [Description("Contact Customer")]
        ContactCustomer = 8,
    }

    public enum DiabeticType
    {
        None = 0,
        Type1 = 1,
        Type2 = 2,
        Gestational = 3,
        Other = 4,
        Unknown = 5,
        NotSupported = 6,
    }

    public enum BoolState
    {
        None = 0,
        Yes = 1,
        No = 2,
        DontKnow = 3,
        NotSupported = 6,
    }

    public enum APIStatus
    {
        SuccessProfile = 1,
        FailedProfile = 2,

        SuccessEligibilityStatus = 3,
        FailedEligibilityStatus = 4,

        SuccessOptingOut = 5,
        FailedOptingOut = 6,

        SuccessPairedDate = 7,
        FailedPairedDate = 8,

        SuccessCreationDate = 9,
        FailedCreationDate = 10,
    }
}
