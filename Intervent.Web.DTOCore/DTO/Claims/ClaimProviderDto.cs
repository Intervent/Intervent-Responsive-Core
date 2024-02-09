namespace Intervent.Web.DTO
{
    public class ClaimProviderDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int[] OrganizationIds { get; set; }

        public string DirectoryName { get; set; }

        public bool IsActive { get; set; }

        public bool IsFixedLayout { get; set; }

        public string TransformerClassName { get; set; }

        public string Delimiter { get; set; }

        public bool HasHeader { get; set; }

        public bool SetSSN { get; set; }

        public string SourceFilePath
        {
            get { return Path.Combine(BaseDirectoryPath, this.DirectoryName); }
        }

        public string MonthlySourceDirectoryPath
        {
            get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, this.DirectoryName); }
        }

        static string MonthlyDirectoryName
        {
            get { return DateTime.UtcNow.ToString("yyyyMM"); }
        }

        //public string InputFlatFilePath { get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, String.Concat("iff", this.Name, ".csv")); } }

        //public string LivongoClaimsOutputFilePath { get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, String.Concat("livongoClaimsOutput", this.Name, ".csv")); } }
        public static string InputFlatFilePath
        {
            get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, "iff.csv"); }
        }

        public static string LivongoClaimsOutputFilePath
        {
            get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, "livongoClaimsOutput.csv"); }
        }

        public static string LivongoClaimsOutputFilePathExcel
        {
            get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, "LivongoAllData-" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx"); }
        }

        public static string EligibilityFlatFilePath
        {
            get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, "eligibility.json"); }
        }

        public static string CrothalIdFilePath
        {
            get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, "crothal.json"); }
        }

        public static string ClaimCodesFilePath
        {
            get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, "claimcodes.json"); }
        }

        public static string ExistingInsuranceSummaryFilePath
        {
            get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, "irsExisting.json"); }
        }

        public static string EnrolledDataUniqueIdsFilePath
        {
            get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, "enrolledData.json"); }
        }

        public static string HRAEnrolledUniqueIdsFilePath
        {
            get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, "hraEnrolled.json"); }
        }

        public static string TheraClassCodesFilePath
        {
            get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, "theraClassCodes.json"); }
        }

        public static string LivongoICDCodesFilePath
        {
            get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, "livongoICDCodes.json"); }
        }

        public static string LivongoNDCCodesFilePath
        {
            get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, "livongoNDCCodes.json"); }
        }

        public static string ClaimConditionCodesFilePath
        {
            get { return Path.Combine(BaseDirectoryPath, MonthlyDirectoryName, "ClaimConditionCodes.json"); }
        }

        private ClaimProviderDto(int id, string name, string directoryName, bool isActive, bool isFixedLayout, string fixedLayoutTranformerClassName, string delimiter, bool hasHeader, bool setSSN)
        {
            Id = id;
            Name = name;
            DirectoryName = directoryName;
            IsActive = isActive;
            IsFixedLayout = isFixedLayout;
            TransformerClassName = fixedLayoutTranformerClassName;
            Delimiter = delimiter;
            HasHeader = hasHeader;
            SetSSN = setSSN;
        }

        public static ClaimProviderDto AETNA = new ClaimProviderDto(1, "AETNA", "AETNA", true, true, "Intervent.Business.Claims.AetnaFixedLayoutTransformer", "|", false, false);

        public static ClaimProviderDto BCBS = new ClaimProviderDto(2, "BCBS", "BCBS", true, true, "Intervent.Business.Claims.BCBSFixedLayoutTransformer", "|", false, false);

        public static ClaimProviderDto CVS_CAREMARK = new ClaimProviderDto(3, "CVS", "CVS_CAREMARK", true, true, "Intervent.Business.Claims.CvsFixedLayoutTransformer", "|", false, true);

        public static ClaimProviderDto UHC = new ClaimProviderDto(4, "UHC", "UHC", true, false, "", "~", true, false);

        //        public static string BaseDirectoryPath = ConfigurationManager.AppSettings["ClaimProcessBasePath"];//@"C:\Intervent\ClaimsProcess";
        public static string BaseDirectoryPath { get; set; }

        public static string ProcessedDirectoryName = @"Processed";

        public static string ErrorDirectoryName = @"Error";

        //public static string FinalSqlBegin = Path.Combine(BaseDirectoryPath,"finalSqlBegin.txt");

        //public static string FinalSqlEnd = Path.Combine(BaseDirectoryPath, "finalSqlEnd.txt");

        // public static string FinalClaimsProcessSql =
        public static IEnumerable<ClaimProviderDto> GetAll()
        {
            List<ClaimProviderDto> lst = new List<ClaimProviderDto>();
            lst.Add(ClaimProviderDto.AETNA);
            lst.Add(ClaimProviderDto.BCBS);
            lst.Add(ClaimProviderDto.CVS_CAREMARK);
            lst.Add(ClaimProviderDto.UHC);
            return lst.Where(x => x.IsActive);
        }

        public static ClaimProviderDto GetByKey(int id)
        {
            return GetAll().Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
