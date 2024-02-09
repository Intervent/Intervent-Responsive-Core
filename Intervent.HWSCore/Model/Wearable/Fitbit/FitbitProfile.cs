namespace Intervent.HWS.Model
{
    public class FitbitProfile : ProcessResponse
    {
        public User user { get; set; }

        public class User
        {
            public int age { get; set; }
            public bool ambassador { get; set; }
            public string avatar { get; set; }
            public string avatar150 { get; set; }
            public string avatar640 { get; set; }
            public int averageDailySteps { get; set; }
            public bool challengesBeta { get; set; }
            public string clockTimeDisplayFormat { get; set; }
            public bool corporate { get; set; }
            public bool corporateAdmin { get; set; }
            public string dateOfBirth { get; set; }
            public string displayName { get; set; }
            public string displayNameSetting { get; set; }
            public string distanceUnit { get; set; }
            public string encodedId { get; set; }
            public Features features { get; set; }
            public string firstName { get; set; }
            public string foodsLocale { get; set; }
            public string fullName { get; set; }
            public string gender { get; set; }
            public string glucoseUnit { get; set; }
            public double height { get; set; }
            public string heightUnit { get; set; }
            public bool isBugReportEnabled { get; set; }
            public bool isChild { get; set; }
            public bool isCoach { get; set; }
            public string languageLocale { get; set; }
            public string lastName { get; set; }
            public bool legalTermsAcceptRequired { get; set; }
            public string locale { get; set; }
            public string memberSince { get; set; }
            public bool mfaEnabled { get; set; }
            public int offsetFromUTCMillis { get; set; }
            public bool sdkDeveloper { get; set; }
            public string sleepTracking { get; set; }
            public string startDayOfWeek { get; set; }
            public double strideLengthRunning { get; set; }
            public string strideLengthRunningType { get; set; }
            public double strideLengthWalking { get; set; }
            public string strideLengthWalkingType { get; set; }
            public string swimUnit { get; set; }
            public string timezone { get; set; }
            public List<object> topBadges { get; set; }
            public bool visibleUser { get; set; }
            public string waterUnit { get; set; }
            public string waterUnitName { get; set; }
            public double weight { get; set; }
            public string weightUnit { get; set; }
        }

        public class Features
        {
            public bool exerciseGoal { get; set; }
        }
    }
}
