namespace InterventWebApp.Helpers
{
    public class AppSettings
    {
        private static IConfigurationRoot _configuration;

        static AppSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();
        }

        public static string GetAppSetting(string key)
        {
            return _configuration[key];
        }

        public string IronPdfLicenseKey { get; set; }
        public string PDFConverterLicence { get; set; }
        public int SystemAdminId { get; set; }
        public int ReminderTime { get; set; }
        public int DaysForFulfillmentRequest { get; set; }
        public bool RequiresHttps { get; set; }
        public bool VerifyDeviceLogin { get; set; }
        public bool ClientValidationEnabled { get; set; }
        public bool UnobtrusiveJavaScriptEnabled { get; set; }
        public string IntuityDTCOrgCode { get; set; }
        public string EbenOrgCode { get; set; }
        public int SouthUniversityOrgId { get; set; }
        public int TeamsBPOrgId { get; set; }
        public int SalesForceOrgId { get; set; }
        public int IntuityOrgId { get; set; }
        public int eBenOrgId { get; set; }
        public int CityofPoolerOrgId { get; set; }
        public int MetLifeGulfOrgId { get; set; }
        public string FilePath { get; set; }
        public string PDFReportPath { get; set; }
        public string InvoicePath { get; set; }
        public string ReportPath { get; set; }
        public string MobileWearableLogPath { get; set; }
        public string WearableFilePath { get; set; }
        public string TrumpiaUserName { get; set; }
        public string TrumpiaKey { get; set; }
        public string Mobile_number { get; set; }
        public string CallPointeAPIKey { get; set; }
        public string LivongoURL { get; set; }
        public string LivongoAuthorizationKey { get; set; }
        public string EdlogicIdpUrl { get; set; }
        public string EdlogiSpUrl { get; set; }
        public string EdIssuer { get; set; }
        public string EdSPIssuer { get; set; }
        public string IssuerUrl { get; set; }
        public string RedirectUrl { get; set; }
        public string LabCorpUserName { get; set; }
        public string LabCorpPassword { get; set; }
        public string LabCorpAccountNumber { get; set; }
        public string SecureEmail { get; set; }
        public string SecureEmailPassword { get; set; }
        public string InfoEmail { get; set; }
        public string SMPTAddress { get; set; }
        public string PortNumber { get; set; }
        public string PreviousPasswordLimit { get; set; }
        public string EmailUrl { get; set; }
        public string ZoomAPIURL { get; set; }
        public string ZoomUserId { get; set; }
        public string ZoomOAuthURL { get; set; }
        public string ZoomClientId { get; set; }
        public string ZoomClientSecret { get; set; }
        public string ZoomAccountId { get; set; }
        public string TeamsBPURL { get; set; }
        public string TeamsBPApiKey { get; set; }
        public string SFaxAPIUrl { get; set; }
        public string SFaxAPIKey { get; set; }
        public string SFaxUsername { get; set; }
        public string ASAUrl { get; set; }
        public string ASATokenValidation { get; set; }
        public string ASASecret { get; set; }
        public string ASAStudyID { get; set; }
        public string ASAStudyAbbreviation { get; set; }
        public string IntuityAPIURL { get; set; }
        public string IntuityURL { get; set; }
        public string IntuityAPIKey { get; set; }
        public string IntuityRedirectUrl { get; set; }
        public string IntuityOAuthURL { get; set; }
        public string IntuityClientId { get; set; }
        public string IntuityClientIssuer { get; set; }
        public string GarminApiUrl { get; set; }
        public string GarminConsumerKey { get; set; }
        public string GarminConsumerSecret { get; set; }
        public string GarminConnectUrl { get; set; }
        public string GarminUserEndPointUrl { get; set; }
        public string MediOrbisUrl { get; set; }
        public string MediOrbisSecretKey { get; set; }
        public string MediOrbisClientId { get; set; }
        public string MediOrbisClientSecret { get; set; }
        public string FitbitApiUrl { get; set; }
        public string FitbitAuthUrl { get; set; }
        public string FitbitClientId { get; set; }
        public string FitbitClientSecret { get; set; }
        public string DexcomApiUrl { get; set; }
        public string DexcomRedirectUrl { get; set; }
        public string DexcomClientId { get; set; }
        public string DexcomClientSecret { get; set; }
        public string OmronApiUrl { get; set; }
        public string OmronAuthUrl { get; set; }
        public string OmronRedirectUrl { get; set; }
        public string OmronClientId { get; set; }
        public string OmronClientSecret { get; set; }
        public string WithingsApiUrl { get; set; }
        public string WithingsAuthUrl { get; set; }
        public string WithingsClientId { get; set; }
        public string WithingsClientSecret { get; set; }
        public string WithingsMode { get; set; }
        public string GoogleFitClientId { get; set; }
        public string GoogleFitClientSecret { get; set; }
        public string JwtValidIssuer { get; set; }
        public string JwtValidAudience { get; set; }
        public string JwtSecret { get; set; }
        public string TwilioAccountSID { get; set; }
        public string TwilioAuthToken { get; set; }
        public string TwilioFrom { get; set; }
        public int SessionTimeOut { get; set; }
        public string MailAttachmentPath { get; set; }
        public string DTCOrgCode { get; set; }
        public int RetailOrgId { get; set; }
    }
}