namespace InterventWebApp
{
    public class Constants
    {
        public const string PortalList = "PortalList";
        public const string LMCCoach = "LMC coach";
        public const string riskColor = "#ff3939";
        public const string noRiskColor = "#2fff2f";
        public const string NORMAL = "NORMAL";
        public const string OUTOFRANGE = "OUTOFRANGE";
        public const string ABNORMAL = "ABNORMAL";
        public const string CRITICAL = "CRITICAL";
        public const string McAllenOrgId = "49";
        public const string MaternityManagement = "3";
    }

    public class Modules
    {
        public const string Kits = "Manage Kits";
        public const string Users = "Manage Users";
        public const string Groups = "Manage Groups";
        public const string Organizations = "Manage Organizations";
        public const string Programs = "Manage Programs";
        public const string Tasks = "Manage Tasks";
        public const string Recipes = "Manage Recipes";
        public const string Eligibility = "Manage Eligibility";
        public const string Scheduler = "Scheduler";
        public const string Profile = "Create Profile";
        public const string Reports = "Generate Reports";
    }

    public class RoleCode
    {
        public const string Administrator = "ADMIN";

        public const string Coach = "COACH";

        public const string CSR = "CSR";
    }

    public enum HRAStatus
    {
        Yes = 1,
        No = 2,
        Optional = 3
    }

    public enum HRAVersions
    {
        FullVersion = 1,
        IntuityVersion = 2,
        ActivateVersion = 3,
        CaptivaVersion = 4
    }

    public enum Integrations
    {
        Intuity = 1,
        Activate = 2,
        LMC = 3,
        Captiva = 4
    }

    public enum CarePlanTypes
    {
        Stroke = 1,
        CVD = 2,
        BloodPressure = 3,
        WeightManagement = 4
    }

    public enum PortalCoachingConditions
    {
        All = 1,
        Diabetes = 2,
        Postmenopausal = 3
    }

    public enum EligibilityFormats
    {
        StandardFormat = 1,
        CompassFormat = 2,
        LMCFormat = 3,
        IntuityFormat = 4,
        MetLifeGulfFormat = 5,
        CaptivaFormat = 6
    }

    public enum EligibilitytoIntuity
    {
        All = 1,
        OnlyDiabetes = 2
    }

    public enum HRAs
    {
        HRAProgramValidity = 30
    }

    public class RaffleTypes
    {
        public const string Monthly = "Monthly Raffle";
        public const string Quaterly = "Quaterly Raffle";
        public const string SemiAnnual = "Semi Annual Raffle";
        public const string Annual = "Annual Raffle";
    }

    public enum FormTypes
    {
        PatientReleaseForm = 1,
        MedicalClearanceForm = 2,
        KnowYourNumbersForm = 3,
        TestimonialForm = 4,
        TobaccoReleaseForm = 5
    }

    public class SessionContext
    {
        public const string UserId = "UserId";

        public const string HasSP = "HasSP";

        public const string SSOState = "SSOState";

        public const string ParticipantId = "ParticipantId";

        public const string ParticipantName = "ParticipantName";

        public const string ParticipantEmail = "ParticipantEmail";

        public const string ParticipantTimeZone = "ParticipantTimeZone";

        public const string ParticipantTimeZoneName = "ParticipantTimeZoneName";

        public const string ParticipantLanguagePreference = "ParticipantLanguagePreference";

        public const string AdminId = "AdminId";

        public const string ParticipantPortalId = "ParticipantPortalId";

        public const string HRAId = "HRAId";

        public const string LanguagePreference = "LanguagePreference";

        public const string Gender = "Gender";

        public const string Zip = "Zip";

        public const string HRACompleteDate = "HRACompleteDate";

        public const string UserinProgramId = "UserinProgramId";

        public const string ProgramsInPortalId = "ProgramsInPortalId";

        public const string EnrolledinCoaching = "EnrolledinCoaching";

        public const string ProgramType = "ProgramType";

        public const string DOB = "DOB";

        public const string HRAPageSeqDone = "HRAPageSeqDone";

        public const string HRAPageSeq = "HRAPageSeq";

        public const string FollowUpPageSeqDone = "FollowUpPageSeqDone";

        public const string AssignedFollowUp = "AssignedFollowUp";

        public const string FollowUpId = "FollowUpId";

        public const string FollowUpCompleteDate = "FollowUpCompleteDate";

        public const string OrganizationId = "OrganizationId";

        public const string OrganizationName = "OrganizationName";

        public const string OrganizationCode = "OrganizationCode";

        public const string HasActivePortal = "ActivePortal";

        public const string NextApptDate = "NextApptDate";

        public const string NextApptId = "NextApptId";

        public const string LandingPage = "LandingPage";

        public const string OrgContactNumber = "OrgContactNumber";

        public const string OrgContactEmail = "OrgContactEmail";

        public const string CoachingProgram = "CoachingProgram";

        public const string SelfHelpProgram = "SelfHelpProgram";

        public const string ShowProgramOption = "ShowProgramOption";

        public const string SelfScheduling = "SelfScheduling";

        public const string ShowPricing = "ShowPricing";

        public const string ClientNameInReport = "ClientNameInReport";

        public const string HRAValidity = "HRAValidity";

        public const string UniqueId = "UniqueId";

        public const string Unit = "Unit";

        public const string MailScoreCard = "MailScoreCard";

        public const string CarePlan = "CarePlan";

        public const string UserStatus = "UserStatus";

        public const string TermsSSO = "TermsSSO";

        public const string TermsAccepted = "TermsAccepted";

        public const string VisitedTab = "VisitedTab";

        public const string HRAVer = "HRAVer";

        public const string IntegrationWith = "IntegrationWith";

        public const string SSO = "SSO";

        public const string StateId = "StateId";

        public const string Challenges = "Challenges";

        public const string DateFormat = "DateFormat";

        public const string AssignPrograms = "AssignPrograms";

        public const string SessionTimeout = "SessionTimeout";

        public const string isPregnant = "isPregnant";

        public const string AllowCardiacQuestion = "AllowCardiacQuestion";

        public const string IsParticipantView = "IsParticipantView";

        public const string HasHRA = "HasHRA";

        public const string FollowUpValidity = "FollowUpValidity";

        public const string MessageCount = "MessageCount";

        public const string ShowPostmenopausal = "ShowPostmenopausal";

        public const string NeedCareplanApproval = "NeedCareplanApproval";

        public const string CareplanPath = "CareplanPath";

        public const string CarePlanType = "CarePlanType";

        public const string AssessmentName = "AssessmentName";

        public const string InActiveParticipantPortalId = "InActiveParticipantPortalId";

        public const string GarminOAuthSecretKey = "GarminOAuthSecretKey";

        public const string ProgramCode = "ProgramCode";

        public const string IsMediOrbisUser = "IsMediOrbisUser";

        public const string SingleSignOn = "SingleSignOn";

        public const string MobileSignOn = "MobileSignOn";

        public const string KitAlert = "KitAlert";
    }

    public class HealthNumberType
    {
        public const string Labs = "Labs";

        public const string HRA = "HRA";

        public const string FollowUp = "FollowUp";
    }

    public enum ProviderDetails
    {
        PlainText = 1,
        DropDown = 2
    }

    public enum WebinarTypes
    {
        Webinar = 5,
        RecurringWebinarNoFixedTime = 6,
        RecurringWebinarFixedTime = 9
    }
}