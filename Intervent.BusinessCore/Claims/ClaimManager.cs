using CsvHelper;
using Intervent.Business.Account;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Intervent.Web.DTO.DTO.Claims.Filter;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Intervent.Business.Claims
{
    public class ClaimManager : BaseManager, IClaimManager
    {
        IAccountManager accountManager = null;
        ClaimsReader claimsReader = null;

        public ClaimManager()
        {
            claimsReader = new ClaimsReader();
            accountManager = new AccountManager();
        }
        public ListClaimsConditionsResponse GetConditionsList(GetCandidateConditionListRequest request)
        {
            return claimsReader.GetConditionsList(request);
        }

        public ListClaimsMedicationResponse GetMedicationsList(GetCandidateMedicationListRequest request)
        {
            return claimsReader.GetMedicationsList(request);
        }

        public IEnumerable<DiagnosisCodeDto> GetDiagnosisCodes()
        {
            return claimsReader.GetDiagnosisCodes();
        }

        #region Process Claims
        public void ProcessClaims()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var orgList = accountManager.GetOrganizationToProcessClaims();
            foreach (var org in orgList)
            {
                Dictionary<int, GetCurrentPortalIdResponse> portalIdwithProcessingCodes = new Dictionary<int, GetCurrentPortalIdResponse>();
                var portal = accountManager.CurrentPortalId(org);
                var portalId = portal.PortalId.Value;
                portalIdwithProcessingCodes.Add(portal.PortalId.Value, portal);
                ClaimProviderDto.BaseDirectoryPath = portal.ClaimsDirectoryPath;

                //check if the directories exist for current month and year
                string directoryName = DateTime.UtcNow.ToString("yyyyMM"); //String.Concat(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
                string monthDirectoryPath = Path.Combine(ClaimProviderDto.BaseDirectoryPath, directoryName);
                var claimProviders = ClaimProviderDto.GetAll();

                //do this step first since it would catch any error with the underlying data
                #region Pull table data needed to process the files (Eligibility)
                //// eligibility data is used to update the unique id

                //for each month create the directory tree
                if (!Directory.Exists(monthDirectoryPath))
                {
                    Directory.CreateDirectory(monthDirectoryPath);
                    foreach (var claimProvider in claimProviders)
                    {
                        Directory.CreateDirectory(Path.Combine(monthDirectoryPath, claimProvider.DirectoryName));
                        Directory.CreateDirectory(Path.Combine(monthDirectoryPath, claimProvider.DirectoryName, ClaimProviderDto.ProcessedDirectoryName));
                        Directory.CreateDirectory(Path.Combine(monthDirectoryPath, claimProvider.DirectoryName, ClaimProviderDto.ErrorDirectoryName));
                    }
                }

                var response = claimsReader.GetClaimsProcessEligibilityList(new ListClaimProcessEligibilityRequest() { PortalId = portalId });
                var eligibility = response.Eligibility.Where(x => x.SSN != "" && x.SSN != null && x.UserEnrollmentType != "C").ToDictionary(x => String.Concat(x.SSN, x.UserEnrollmentType), x => x, StringComparer.OrdinalIgnoreCase);
                string eligibilityJson = JsonConvert.SerializeObject(eligibility);
                File.WriteAllText(ClaimProviderDto.EligibilityFlatFilePath, eligibilityJson, Encoding.UTF8);
                var eligibilitySSNKey = response.Eligibility.Where(x => x.SSN != "" && x.SSN != null && !String.IsNullOrEmpty(x.MedicalPlanCode)).ToDictionary(x => String.Concat(x.SSN, x.UserEnrollmentType), x => x, StringComparer.OrdinalIgnoreCase);
                var eligibilityUniqueIdKey = response.Eligibility.Where(x => x.UniqueID != "" && x.UniqueID != null && x.UserEnrollmentType != "C" && !String.IsNullOrEmpty(x.MedicalPlanCode)).ToDictionary(x => x.UniqueID, x => x, StringComparer.OrdinalIgnoreCase);
                var childreneligibilityUniqueIdKey = response.Eligibility.Where(x => x.UniqueID != "" && x.UniqueID != null && x.UserEnrollmentType == "C" && !String.IsNullOrEmpty(x.MedicalPlanCode)).ToDictionary(x => String.Concat(x.UniqueID + x.FirstName), x => x, StringComparer.OrdinalIgnoreCase);

                var crothalIDs = claimsReader.GetClaimsCrothalIDs().CrothalIds;
                string crothalJson = JsonConvert.SerializeObject(crothalIDs);
                File.WriteAllText(ClaimProviderDto.CrothalIdFilePath, crothalJson, Encoding.UTF8);
                var crothalIDsDict = crothalIDs.ToDictionary(x => x.OldUniqueId, x => x, StringComparer.OrdinalIgnoreCase);

                var claimCodes = claimsReader.GetClaimsCodes().ClaimCodes;
                string claimCodesJson = JsonConvert.SerializeObject(claimCodes);
                File.WriteAllText(ClaimProviderDto.ClaimCodesFilePath, claimCodesJson, Encoding.UTF8);
                var claimCodesDict = claimCodes.ToLookup(x => x.Code, x => x, StringComparer.OrdinalIgnoreCase);
                //var claimCodeDictWithoutLivongo = claimCodes.Where(x => x.IncludeME == false).ToLookup(x=> x.Code, x => x, StringComparer.OrdinalIgnoreCase);

                var enrolledDataUniqueIds = claimsReader.GetClaimProcessEnrolledDataUniqueIds(portalId).EnrolledDatas;
                string enrolledDataUniqueIdsJson = JsonConvert.SerializeObject(enrolledDataUniqueIds);
                File.WriteAllText(ClaimProviderDto.EnrolledDataUniqueIdsFilePath, enrolledDataUniqueIdsJson, Encoding.UTF8);
                //  var enrolledDataUniqueIdsDict = enrolledDataUniqueIds.ToDictionary(x => x.UniqueId, x => x, StringComparer.OrdinalIgnoreCase);

                var HRAEnrolledUniqueIds = claimsReader.GetClaimProcessHRAUniqueIds(portalId).HRAs;
                string HRAEnrolledUniqueIdsJson = JsonConvert.SerializeObject(HRAEnrolledUniqueIds);
                File.WriteAllText(ClaimProviderDto.HRAEnrolledUniqueIdsFilePath, HRAEnrolledUniqueIdsJson, Encoding.UTF8);

                var theraClassCodes = claimsReader.GetTherapeuticClassCodes().TherapeuticClassCodes;
                string theraClassCodesJson = JsonConvert.SerializeObject(theraClassCodes);
                File.WriteAllText(ClaimProviderDto.TheraClassCodesFilePath, theraClassCodesJson, Encoding.UTF8);
                var theraClassCodesDict = theraClassCodes.ToDictionary(x => x.TheraCode, x => x, StringComparer.OrdinalIgnoreCase);

                var livongoICDCodes = claimsReader.GetLivongoICDCodes().ICDCodes;
                string livongoICDCodesJson = JsonConvert.SerializeObject(livongoICDCodes);
                File.WriteAllText(ClaimProviderDto.LivongoICDCodesFilePath, livongoICDCodesJson, Encoding.UTF8);
                var livongoICDCodesDict = livongoICDCodes.ToDictionary(x => x.Code, x => x, StringComparer.OrdinalIgnoreCase);

                var livongoNDCCodes = claimsReader.GetLivongoNDCCodes().NDCCodes;
                string livongoNDCCodesJson = JsonConvert.SerializeObject(livongoNDCCodes);
                File.WriteAllText(ClaimProviderDto.LivongoNDCCodesFilePath, livongoNDCCodesJson, Encoding.UTF8);
                var livongoNDCCodesDict = livongoNDCCodes.ToDictionary(x => x.Code, x => x, StringComparer.OrdinalIgnoreCase);

                var ClaimConditionCode = claimsReader.GetClaimConditionCodes().ClaimConditionCodes;
                string ClaimConditionCodeJson = JsonConvert.SerializeObject(ClaimConditionCode);
                File.WriteAllText(ClaimProviderDto.ClaimConditionCodesFilePath, ClaimConditionCodeJson, Encoding.UTF8);
                var ClaimConditionCodeDict = ClaimConditionCode.ToDictionary(x => x.ClaimsID + ":" + x.Condition + ":" + x.Code + ":" + x.CodeDescription + ":" + x.ConditionDate, x => x, StringComparer.OrdinalIgnoreCase);

                #region Pull necessary files for creating the sql
                var insuranceResultSummary = claimsReader.GetInsuranceResultSummaries(org).InsuranceSummaries;
                string insuranceResultSummaryJson = JsonConvert.SerializeObject(insuranceResultSummary);
                File.WriteAllText(ClaimProviderDto.ExistingInsuranceSummaryFilePath, insuranceResultSummaryJson, Encoding.UTF8);
                Dictionary<string, ClaimProcessInsuranceSummaryDto> insuranceResultSummaryDict = insuranceResultSummary.ToDictionary(x => string.Concat(x.UniqueId, x.OrgId), x => x, StringComparer.OrdinalIgnoreCase);


                #endregion
                #endregion

                #region Move files from parent directory to monthly directory and do transforms for fixed layout files
                Parallel.ForEach(claimProviders, (claimProvider) =>
                {
                    try
                    {
                        var fileNames = Directory.GetFiles(claimProvider.SourceFilePath);

                        if (fileNames.Count() == 0)
                        {
                            Log.Debug($"No files found for provider {claimProvider.Name} ");
                            return;
                        }

                        if (fileNames.Count() > 1)
                        {
                            Log.Error($"Skipping provider. More than one file found for provider {claimProvider.Name} ");
                            return;
                        }

                        //came here means one file found
                        string fileName = fileNames.First();

                        //move file to the monthly folder
                        File.Move(fileName, Path.Combine(claimProvider.MonthlySourceDirectoryPath, Path.GetFileName(fileName)));


                        //check if the file is a fixed layout file
                        fileName = Directory.GetFiles(claimProvider.MonthlySourceDirectoryPath).First();
                        if (claimProvider.IsFixedLayout)
                        {
                            string extension = Path.GetExtension(fileName);
                            string fileNameWithoutPath = Path.GetFileNameWithoutExtension(fileName);
                            string destinationFilePath = Path.Combine(claimProvider.MonthlySourceDirectoryPath, fileNameWithoutPath) + "WithDelimiter" + extension;
                            BaseFixedLayoutTransformer transformer = (BaseFixedLayoutTransformer)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(claimProvider.TransformerClassName);
                            transformer.Transform(fileName, destinationFilePath);
                            File.Move(fileName, Path.Combine(Path.GetDirectoryName(fileName), ClaimProviderDto.ProcessedDirectoryName, Path.GetFileName(fileName)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"error occurred while processing claim for provider {claimProvider.Name}");
                    }

                });
                #endregion

                #region write header files
                if (!File.Exists(ClaimProviderDto.InputFlatFilePath))
                {
                    using (TextWriter writer = new StreamWriter(ClaimProviderDto.InputFlatFilePath))
                    {
                        using (var csv = new CsvWriter(writer))
                        {
                            csv.Configuration.Encoding = Encoding.UTF8;
                            csv.Configuration.Delimiter = "|";
                            csv.Configuration.RegisterClassMap<InputFlatFileClassMap>();
                            csv.WriteHeader(typeof(ClaimsInputFlatFileModel));
                        }
                    }
                }
                if (!File.Exists(ClaimProviderDto.LivongoClaimsOutputFilePath))
                {
                    using (TextWriter writer = new StreamWriter(ClaimProviderDto.LivongoClaimsOutputFilePath))
                    {
                        using (var csv = new CsvWriter(writer))
                        {
                            csv.Configuration.Encoding = Encoding.UTF8;
                            csv.Configuration.RegisterClassMap<LivongoOutputClassMap>();
                            csv.WriteHeader(typeof(ClaimsInputFlatFileModel));
                        }
                    }
                }
                #endregion

                #region read files using csv map and save a single input flat file
                foreach (var claimProvider in claimProviders)
                {

                    var filePaths = Directory.GetFiles(claimProvider.MonthlySourceDirectoryPath).Where(x => !x.EndsWith("log.txt"));
                    if (filePaths.Count() == 0)
                    {
                        continue;
                    }
                    #endregion
                    var filePath = filePaths.First();
                    int row = 0;
                    try
                    {
                        //log file
                        string logFilePath = Path.Combine(Path.GetDirectoryName(filePath), "log.txt");
                        Log.Info($"log file path is {logFilePath} ");
                        //append to writer
                        using (var sr = new StreamReader(filePath))
                        using (TextWriter sw = new StreamWriter(ClaimProviderDto.InputFlatFilePath, true))
                        using (TextWriter swLivongo = new StreamWriter(ClaimProviderDto.LivongoClaimsOutputFilePath, true))
                        using (TextWriter processLog = new StreamWriter(logFilePath, false))
                        {
                            processLog.WriteLine($"Begin file processing - {DateTime.UtcNow.ToString("HH:mm:ss")}");
                            using (CsvReader reader = new CsvReader(sr))
                            using (CsvWriter writer = new CsvWriter(sw))
                            using (CsvWriter writerLivongo = new CsvWriter(swLivongo))
                            {
                                // writer config
                                writer.Configuration.Encoding = Encoding.UTF8;
                                writer.Configuration.Delimiter = "|";
                                writer.Configuration.RegisterClassMap<InputFlatFileClassMap>();

                                // writer config
                                writerLivongo.Configuration.Encoding = Encoding.UTF8;
                                writerLivongo.Configuration.RegisterClassMap<LivongoOutputClassMap>();

                                // reader config
                                reader.Configuration.Delimiter = claimProvider.Delimiter;
                                reader.Configuration.HasHeaderRecord = claimProvider.HasHeader;
                                reader.Configuration.IsHeaderCaseSensitive = false;
                                reader.Configuration.SkipEmptyRecords = true;
                                reader.Configuration.TrimFields = true;
                                reader.Configuration.TrimHeaders = true;
                                reader.Configuration.WillThrowOnMissingField = true;
                                if (claimProvider == ClaimProviderDto.BCBS)
                                    reader.Configuration.RegisterClassMap<BCBSFileClassMap>();
                                else if (claimProvider == ClaimProviderDto.AETNA)
                                    reader.Configuration.RegisterClassMap<AetnaFileClassMap>();
                                else if (claimProvider == ClaimProviderDto.CVS_CAREMARK)
                                    reader.Configuration.RegisterClassMap<CVSCaremarkClassMap>();
                                else if (claimProvider == ClaimProviderDto.UHC)
                                    reader.Configuration.RegisterClassMap<UHCClassMap>();
                                else
                                {
                                    Log.Error($"Class map not defined for provider {claimProvider.Name}");
                                    return;
                                }
                                row = 0;
                                //Build pre filters
                                List<IInputFlatFileFilter> preFilters = new List<IInputFlatFileFilter>();
                                preFilters.Add(new RelationshipFilter());
                                preFilters.Add(new AgeFilter());
                                // not adding the SSN filter for all providers since CVS files might not have it yet
                                if (!claimProvider.SetSSN)
                                {
                                    preFilters.Add(new InvalidSSNFilter());
                                }
                                //Build post filters
                                List<IInputFlatFileFilter> postFilters = new List<IInputFlatFileFilter>();
                                if (claimProvider.SetSSN)//found an instance where spouse SSN is blank for unique ID 00149860EH8
                                {
                                    postFilters.Add(new InvalidSSNFilter());
                                }
                                //postFilters.Add(new ClaimConditionFilter());
                                postFilters.Add(new ChildrenFilter());
                                //  postFilters.Add(new EnrolledDataFilter(enrolledDataUniqueIds, HRAEnrolledUniqueIds));
                                while (reader.Read())
                                {
                                    row++;
                                    try
                                    {
                                        //Data->Values->ResultsView->[0] will give you the index
                                        // "Row: '2' (1 based)\r\nType: 'Intervent.Web.DTO.ClaimsInputFlatFileModel'\r\nField Index: '125' (0 based)\r\nField Value: '000000003}'\r\n"
                                        ClaimsInputFlatFileModel record = reader.GetRecord<ClaimsInputFlatFileModel>();
                                        //set the provider name
                                        record.ProviderName = claimProvider.Name;
                                        record.InputFileRowId = row;
                                        #region Pre Filters

                                        bool isRecordFiltered = false;
                                        foreach (var filter in preFilters)
                                        {
                                            if (filter.FilterRecord(record))
                                            {
                                                processLog.WriteLine($"WARN:Row {row} is skipped. Reason - {filter.FilterMessage}");
                                                isRecordFiltered = true;
                                                break;
                                            }
                                        }
                                        if (isRecordFiltered)
                                            continue;
                                        #endregion

                                        #region Set unique id or SSN from eligibility
                                        record.PortalId = portalId;
                                        record.CrothalIdChanges(crothalIDsDict);//only for CVS
                                        string extendedLog = "";
                                        if (claimProvider.SetSSN)
                                            record.SetSSN(eligibilityUniqueIdKey, childreneligibilityUniqueIdKey);
                                        else
                                            record.SetUniqueId(eligibilitySSNKey, eligibilityUniqueIdKey, out extendedLog);
                                        var userEnrollmentType = (ClaimsRelationshipToSubscriberCode.SPOUSE == record.RelationshipToSubscriberCode) ? "S" : (ClaimsRelationshipToSubscriberCode.CHILDREN == record.RelationshipToSubscriberCode ? "C" : "E");
                                        ClaimProcessEligibilityDto elig = null;
                                        eligibility.TryGetValue(record.MemberSSN + userEnrollmentType, out elig);
                                        if (String.IsNullOrEmpty(record.UniqueId) && elig == null)
                                        {
                                            processLog.WriteLine($"WARN:Row {row} is skipped. Reason - could not determine unique id. {extendedLog}");
                                            continue;
                                        }
                                        else if (record.SkipUniqueId && elig == null)
                                        {
                                            processLog.WriteLine($"WARN:Row {row} is skipped. unique id is {record.UniqueId}. Reason - {record.SkipReason}.");
                                            continue;
                                        }
                                        else
                                        {
                                            if (!String.IsNullOrEmpty(record.UniqueId) && !record.SkipUniqueId && portalIdwithProcessingCodes[record.PortalId].ProcessLivongoCodes)
                                            {
                                                record.SetLivongoFields(livongoICDCodesDict, livongoNDCCodesDict, eligibilitySSNKey, eligibilityUniqueIdKey, childreneligibilityUniqueIdKey);
                                            }
                                            else if (elig != null)
                                            {
                                                record.UniqueId = elig.UniqueID;
                                                record.OrgId = elig.OrgId;
                                                record.PortalId = elig.PortalId;
                                                record.OrgName = elig.OrgName;
                                            }
                                            record.SetClaimCodeCondition(claimCodesDict);
                                            record.SetEnrolledDataFields(enrolledDataUniqueIds, HRAEnrolledUniqueIds);
                                        }
                                        #endregion
                                        if (record.IncludeInLivongoOutput)
                                        {
                                            writerLivongo.WriteRecord(record);
                                        }
                                        #region Post Filters
                                        foreach (var filter in postFilters)
                                        {
                                            if (filter.FilterRecord(record))
                                            {
                                                processLog.WriteLine($"WARN:Row {row} with unique id {record.UniqueId} is skipped. Reason - {filter.FilterMessage}");
                                                isRecordFiltered = true;
                                                break;
                                            }
                                        }
                                        if (isRecordFiltered)
                                            continue;
                                        #endregion
                                        record.SetDrugCategory(theraClassCodesDict);
                                        // success
                                        writer.WriteRecord(record);
                                        processLog.WriteLine($"SUCCESS:Row {row} has passed all validations and insert into input flat file");
                                    }
                                    catch (CsvHelperException ex)
                                    {
                                        string msg = "";
                                        foreach (System.Collections.DictionaryEntry de in ex.Data)
                                        {
                                            msg += de.Value;
                                        }
                                        processLog.WriteLine($"ERROR:Row {row} is skipped due to an unhandled exception {ex.Message}");
                                        processLog.WriteLine(ex.Data["CsvHelper"]);
                                        Log.Error(ex);
                                    }
                                    catch (Exception ex)
                                    {
                                        processLog.WriteLine($"ERROR:Row {row} is skipped due to an unhandled exception {ex.Message}");
                                        Log.Error(ex);
                                    }


                                }
                            }
                            processLog.WriteLine($" End file processing - {DateTime.UtcNow.ToString("HH:mm:ss")}");
                        }

                        File.Move(filePath, Path.Combine(Path.GetDirectoryName(filePath), ClaimProviderDto.ProcessedDirectoryName, Path.GetFileName(filePath)));
                    }
                    catch (CsvHelperException ex)
                    {
                        Log.Error(ex);
                        File.Move(filePath, Path.Combine(Path.GetDirectoryName(filePath), ClaimProviderDto.ErrorDirectoryName, Path.GetFileName(filePath)));
                    }
                    catch (Exception ex)
                    {
                        File.Move(filePath, Path.Combine(Path.GetDirectoryName(filePath), ClaimProviderDto.ErrorDirectoryName, Path.GetFileName(filePath)));
                        Log.Error(ex);
                    }
                }

                #region Convert CSV to Excel
                CommonReader commonReader = new CommonReader();
                int[] livongoDateColumnIndexes = new int[] { 5, 6, 7, 8, 17, 40, 42 };
                commonReader.ConverttoExcel(ClaimProviderDto.LivongoClaimsOutputFilePath, ClaimProviderDto.LivongoClaimsOutputFilePathExcel, "Livongo Data", ',', livongoDateColumnIndexes);

                #endregion

                #region Step 2 Insurance result Summary


                #region create sql from input flat file
                string AETNAsqlFilePath = Path.Combine(Path.GetDirectoryName(ClaimProviderDto.InputFlatFilePath), $"AETNAinsuranceSummarySql.txt");
                string BCBSsqlFilePath = Path.Combine(Path.GetDirectoryName(ClaimProviderDto.InputFlatFilePath), $"BCBSinsuranceSummarySql.txt");
                string CVSsqlFilePath = Path.Combine(Path.GetDirectoryName(ClaimProviderDto.InputFlatFilePath), $"CVSinsuranceSummarySql.txt");
                string UHCsqlFilePath = Path.Combine(Path.GetDirectoryName(ClaimProviderDto.InputFlatFilePath), $"UHCinsuranceSummarySql.txt");
                string isLogFilePath = Path.Combine(Path.GetDirectoryName(ClaimProviderDto.InputFlatFilePath), $"insuranceSummaryLog.txt");
                //ID, identity field
                string insuranceSummaryInsertSql = @"INSERT INTO InsuranceSummary(EarliestServiceDate, Eligible, HRA, EnrollType, DataSrc, CreateDate, LastModifiedDate, ConditionCount, Inactive, UniqueID, OrganizationId, Spouse) VALUES (";
                string insuranceSummaryUpdateSql = @"UPDATE InsuranceSummary SET ";
                //string insuranceSummaryUpdateSqlCondition1 = @" WHERE ID = {0};";
                //string insuranceSummaryUpdateSqlCondition2 = @" WHERE UniqueId = '{0}' and OrganizationId = {1};";
                string candidateMedicationInsertSql = "Insert into CandidateMedications ( [SourceDataDate] ,[MedicationType] ,[MedicationName] ,[MedicationDate] ,[Total Amount Paid by All Source] ,[Patient Pay Amount] ,[Amount of Copay] ,[Amount of Coinsurance] ,[Net Amount Due (Total Amount Billed-Paid)] ,[ClaimsID] ) VALUES (";
                string candidateConditionInsertSql = "Insert into CandidateConditions ( [SourceDataDate] ,[ConditionName] ,[ConditionType] ,[ConditionDate] ,[BilledAmount] ,[Copay] ,[Deductible] ,[Coinsurance] ,[NetPaid] ,[ClaimsID] ) VALUES (";
                string ifnotexistsCRFLSql = "IF NOT EXISTS (SELECT Top(1)* FROM CandidateReasonForLastChange C WHERE c.ConditionType = ";
                string candidateReasonForLastChangeSql = "Insert into CandidateReasonForLastChange ([ConditionType],[ConditionDate],[ClaimsId]) values (";
                string ifnotexistsCCCSql = "IF NOT EXISTS (SELECT Top(1)* FROM ClaimConditionCode CC WHERE cc.Condition = ";
                string claimConditionCodeInsertSql = "Insert into ClaimConditionCode ([ClaimsID],[Condition],[ConditionDate],[Code],[CodeDescription]) values (";
                //append to writer
                using (var sr = new StreamReader(ClaimProviderDto.InputFlatFilePath))
                using (TextWriter AETNAsw = new StreamWriter(AETNAsqlFilePath, true))
                using (TextWriter BCBSsw = new StreamWriter(BCBSsqlFilePath, true))
                using (TextWriter CVSsw = new StreamWriter(CVSsqlFilePath, true))
                using (TextWriter UHCsw = new StreamWriter(UHCsqlFilePath, true))
                using (TextWriter isLog = new StreamWriter(isLogFilePath, false))
                {
                    using (CsvReader reader = new CsvReader(sr))
                    {
                        //reader config
                        reader.Configuration.Delimiter = "|";
                        reader.Configuration.HasHeaderRecord = true;
                        reader.Configuration.IsHeaderCaseSensitive = false;
                        reader.Configuration.SkipEmptyRecords = true;
                        reader.Configuration.TrimFields = true;
                        reader.Configuration.TrimHeaders = true;
                        reader.Configuration.WillThrowOnMissingField = true;
                        reader.Configuration.RegisterClassMap<InputFlatFileClassMapRead>();
                        int row = 0;
                        StringBuilder sbSql = new StringBuilder();
                        StringBuilder sbReasonForLastChange = new StringBuilder();
                        Dictionary<string, string> codeString = new Dictionary<string, string>();
                        string conditionDate = DateTime.UtcNow.ToString("MM/01/yyyy");
                        string conditionDateWithSemicolon = DateTime.UtcNow.ToString("MM/01/yyyy") + ";";
                        //moved code to beginSql
                        //AETNAsw.WriteLine("DECLARE @NewClaimId int;");
                        //BCBSsw.WriteLine("DECLARE @NewClaimId int;");
                        //CVSsw.WriteLine("DECLARE @NewClaimId int;");
                        //UHCsw.WriteLine("DECLARE @NewClaimId int;");
                        bool hasConditionChanged = false;
                        var lstUniqueIdsNotInIRS = new Dictionary<string, List<string>>();
                        //there are multiple input records for the same unique id in a sequential order
                        ClaimProcessUniqueId processingUniqueId = new ClaimProcessUniqueId();
                        var records = reader.GetRecords<ClaimsInputFlatFileModel>().OrderBy(x => x.ProviderName).ThenBy(x => x.UniqueId);
                        int insuranceSummaryInserts = 0;
                        int candidateMedicationInserts = 0;
                        int candidateConditionInserts = 0;
                        int reasonForLastChangeInserts = 0;
                        int insuranceSummaryUpdates = 0;
                        int medicationwithnoclaimconditions = 0;
                        string providerName = "";
                        bool newClaim = false;
                        foreach (ClaimsInputFlatFileModel record in records)
                        {
                            row++;
                            sbSql.Clear();
                            sbReasonForLastChange.Clear();
                            hasConditionChanged = false;
                            newClaim = false;
                            codeString.Clear();
                            try
                            {
                                // ClaimsInputFlatFileModel record = reader.GetRecord<ClaimsInputFlatFileModel>();
                                var insuranceSummaryRecord = insuranceResultSummaryDict.ContainsKey(String.Concat(record.UniqueId, record.OrgId)) ? insuranceResultSummaryDict[String.Concat(record.UniqueId, record.OrgId)] : null;
                                isLog.WriteLine($"Processing row {row} - unique id {record.UniqueId}");
                                processingUniqueId.HasOnlyOtherCondition = true;
                                if (record.UniqueId != processingUniqueId.UniqueId)
                                {
                                    processingUniqueId.UniqueId = record.UniqueId;
                                    processingUniqueId.IsFirstPass = true;
                                    processingUniqueId.SetClaimIdPreviouslyRun = false;
                                    processingUniqueId.IsPregnantAfter180DaysSatisfied = false;
                                    processingUniqueId.Conditions = new List<ClaimsProcessClaimCodeCondition>();
                                    if (processingUniqueId.ProviderName != record.ProviderName && row > 1)
                                    {
                                        //write to logs the count
                                        isLog.WriteLine($"Provider name is {processingUniqueId.ProviderName}");
                                        isLog.WriteLine($"Insurance Summary insert count is {insuranceSummaryInserts}");
                                        isLog.WriteLine($"Insurance Summary update count is {insuranceSummaryUpdates}");
                                        isLog.WriteLine($"Candidate conditions insert count is {candidateConditionInserts}");
                                        isLog.WriteLine($"Candidate medications insert count is {candidateMedicationInserts}");
                                        isLog.WriteLine($"Reason for last change insert count is {reasonForLastChangeInserts}");
                                        isLog.WriteLine($"Medications with no IV claim conditions insert count is {medicationwithnoclaimconditions}");
                                        processingUniqueId.ProviderName = providerName = record.ProviderName;
                                        insuranceSummaryInserts = 0;
                                        insuranceSummaryUpdates = 0;
                                        candidateConditionInserts = 0;
                                        candidateMedicationInserts = 0;
                                        reasonForLastChangeInserts = 0;
                                        medicationwithnoclaimconditions = 0;
                                    }
                                }
                                else
                                {
                                    processingUniqueId.IsFirstPass = false;
                                }
                                isLog.WriteLine($"First pass is {processingUniqueId.IsFirstPass}");
                                #region insert record if not found in insurance summary
                                if (insuranceSummaryRecord == null)
                                {
                                    isLog.WriteLine($"Unique id {record.UniqueId} not found in insurance summary");
                                    if (!lstUniqueIdsNotInIRS.ContainsKey(record.UniqueId))
                                    {
                                        isLog.WriteLine($"Building the insert sql for Unique id {record.UniqueId}");
                                        lstUniqueIdsNotInIRS.Add(record.UniqueId, new List<string>());
                                        //build the insert sql
                                        sbSql.Append(insuranceSummaryInsertSql);

                                        // sbSql.Append(ID);
                                        sbSql.Append("null,");
                                        sbSql.Append("1,");//Eligible
                                        sbSql.Append("1,");//HRA
                                        sbSql.Append("'" + record.EnrollType + "',");//Enroll Type(set in filter)
                                        sbSql.Append("'" + record.ProviderName + "',");
                                        sbSql.Append($"'{conditionDate}',");
                                        sbSql.Append($"'{conditionDate}',");
                                        sbSql.Append("0,");
                                        sbSql.Append("0,'");
                                        sbSql.Append(record.UniqueId);
                                        sbSql.Append($"',{record.OrgId},");
                                        sbSql.Append(record.IsSpouse ? "1" : "0");
                                        sbSql.Append(");");
                                        //sbSql.Append(Environment.NewLine);
                                        insuranceSummaryInserts++;
                                        newClaim = true;
                                    }
                                    else
                                    {
                                        isLog.WriteLine($"Script created in previous iteration. Not building the insert sql for Unique id {record.UniqueId}");
                                        newClaim = true;
                                    }
                                }
                                if (record.UniqueId != processingUniqueId.UniqueId || processingUniqueId.IsFirstPass || (!processingUniqueId.IsFirstPass && !processingUniqueId.SetClaimIdPreviouslyRun))
                                {
                                    if (insuranceSummaryRecord == null)
                                    {
                                        sbSql.Append($"SELECT @NewClaimId = Id FROM InsuranceSummary WHERE UNIQUEID = '{record.UniqueId}' AND OrganizationId = {record.OrgId};");
                                    }
                                    else
                                    {
                                        sbSql.Append($"SET @NewClaimId = {insuranceSummaryRecord.Id};");
                                    }
                                }
                                if (insuranceSummaryRecord != null)
                                {
                                    isLog.WriteLine($"Unique id {record.UniqueId} found in insurance summary");
                                    if (!insuranceSummaryRecord.HRA && record.HasHRA && processingUniqueId.IsFirstPass)
                                    {
                                        isLog.WriteLine($"Update HRA for Claims Id {insuranceSummaryRecord.Id}");
                                        sbSql.Append($"update InsuranceSummary set HRA = 1 where ID = @NewClaimId;");
                                    }
                                    if (insuranceSummaryRecord.EnrollType != record.EnrollType && !String.IsNullOrEmpty(record.EnrollType) && processingUniqueId.IsFirstPass)
                                    {
                                        isLog.WriteLine($"Update Enroll Type for Claims Id {insuranceSummaryRecord.Id}");
                                        sbSql.Append($"update InsuranceSummary set EnrollType = '{record.EnrollType}' where ID = @NewClaimId;");
                                    }
                                }
                                #endregion

                                #region Update conditions
                                bool runUpdateScript = false;
                                bool noIVclaimcondition = false;
                                sbSql.Append(insuranceSummaryUpdateSql);
                                if (((insuranceSummaryRecord == null && !lstUniqueIdsNotInIRS[record.UniqueId].Contains("PREG")) || !(insuranceSummaryRecord != null && insuranceSummaryRecord.IsPregnant)) && record.IsPregnant)
                                {
                                    if (!IsConditionAlreadyProcessed(processingUniqueId, ClaimsProcessClaimCodeCondition.PREG, record.ServiceStartDate))
                                    {
                                        sbReasonForLastChange.Append("PREG");
                                        sbReasonForLastChange.Append(conditionDateWithSemicolon);
                                        hasConditionChanged = true;
                                        processingUniqueId.HasOnlyOtherCondition = false;
                                        isLog.WriteLine($"Pregnancy condition is new for unique id {record.UniqueId}");
                                    }
                                    else
                                    {
                                        isLog.WriteLine($"Pregnancy condition already processed for unique id {record.UniqueId}");
                                    }

                                }
                                else if (record.IsPregnant)
                                {
                                    isLog.WriteLine($"Pregnancy condition previously existed for {record.UniqueId}");
                                }
                                //check if pregnant again
                                if (!processingUniqueId.IsPregnantAfter180DaysSatisfied && insuranceSummaryRecord != null && insuranceSummaryRecord.IsPregnant && record.IsPregnant && insuranceSummaryRecord.LatestPregnancyDate.HasValue)
                                {
                                    if (insuranceSummaryRecord.LatestPregnancyDate.HasValue)
                                    {
                                        var lastPregnancyDate = insuranceSummaryRecord.LatestPregnancyDate.Value;
                                        var lapsedDays = (DateTime.ParseExact(conditionDate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture) - lastPregnancyDate).TotalDays;
                                        if (lapsedDays >= 180)
                                        {
                                            if (!IsConditionAlreadyProcessed(processingUniqueId, ClaimsProcessClaimCodeCondition.PREG, record.ServiceStartDate))
                                            {
                                                isLog.WriteLine($"pregnancy condition did lapse 180 days for {record.UniqueId}");
                                                sbReasonForLastChange.Append("PREG");
                                                sbReasonForLastChange.Append(conditionDateWithSemicolon);
                                                hasConditionChanged = true;
                                                runUpdateScript = true;
                                                processingUniqueId.HasOnlyOtherCondition = false;
                                                processingUniqueId.IsPregnantAfter180DaysSatisfied = true;
                                            }
                                        }
                                        else
                                        {
                                            isLog.WriteLine($"pregnancy condition did not lapse 180 days for {record.UniqueId}");
                                        }
                                    }
                                }
                                if (((insuranceSummaryRecord == null && !lstUniqueIdsNotInIRS[record.UniqueId].Contains("DIAB")) || !(insuranceSummaryRecord != null && insuranceSummaryRecord.IsDiabetic)) && record.IsDiabetic)
                                {
                                    if (!IsConditionAlreadyProcessed(processingUniqueId, ClaimsProcessClaimCodeCondition.DIAB, record.ServiceStartDate))
                                    {
                                        sbReasonForLastChange.Append("DIAB");
                                        sbReasonForLastChange.Append(conditionDateWithSemicolon);
                                        hasConditionChanged = true;
                                        processingUniqueId.HasOnlyOtherCondition = false;
                                        isLog.WriteLine($"Diabetic condition is new for unique id {record.UniqueId}");
                                    }
                                    else
                                    {
                                        isLog.WriteLine($"Diabetic condition already processed for unique id {record.UniqueId}");
                                    }
                                }
                                else if (record.IsDiabetic)
                                {
                                    isLog.WriteLine($"Diabetic condition previously existed for {record.UniqueId}");
                                }

                                if (((insuranceSummaryRecord == null && !lstUniqueIdsNotInIRS[record.UniqueId].Contains("DIABLV")) || !(insuranceSummaryRecord != null && insuranceSummaryRecord.IsDiabeticLivongo)) && record.IncludeInLivongoOutput)
                                {
                                    if (!IsConditionAlreadyProcessed(processingUniqueId, ClaimsProcessClaimCodeCondition.DIABLV, record.ServiceStartDate))
                                    {
                                        sbReasonForLastChange.Append("DIABLV");
                                        sbReasonForLastChange.Append(conditionDateWithSemicolon);
                                        hasConditionChanged = true;
                                        processingUniqueId.HasOnlyOtherCondition = false;
                                        isLog.WriteLine($"Diabetic livongo condition is new for unique id {record.UniqueId}");
                                    }
                                    else
                                    {
                                        isLog.WriteLine($"Diabetic livongo condition already processed for unique id {record.UniqueId}");
                                    }
                                }
                                else if (record.IncludeInLivongoOutput)
                                {
                                    isLog.WriteLine($"Livongo diabetic condition previously existed for {record.UniqueId}");
                                }
                                if (((insuranceSummaryRecord == null && !lstUniqueIdsNotInIRS[record.UniqueId].Contains("SMOKE")) || !(insuranceSummaryRecord != null && insuranceSummaryRecord.IsSmoking)) && record.IsSmoking)
                                {
                                    if (!IsConditionAlreadyProcessed(processingUniqueId, ClaimsProcessClaimCodeCondition.SMOKE, record.ServiceStartDate))
                                    {
                                        sbReasonForLastChange.Append("SMOKE");
                                        sbReasonForLastChange.Append(conditionDateWithSemicolon);
                                        hasConditionChanged = true;
                                        processingUniqueId.HasOnlyOtherCondition = false;
                                        isLog.WriteLine($"Smoking condition is new for unique id {record.UniqueId}");
                                    }
                                    else
                                    {
                                        isLog.WriteLine($"Smoking condition already processed for unique id {record.UniqueId}");
                                    }
                                }
                                else if (record.IsSmoking)
                                {
                                    isLog.WriteLine($"Smoking condition previously existed for {record.UniqueId}");
                                }
                                if (((insuranceSummaryRecord == null && !lstUniqueIdsNotInIRS[record.UniqueId].Contains("HYPTEN")) || !(insuranceSummaryRecord != null && insuranceSummaryRecord.HasHyperTension)) && record.HasHypertension)
                                {
                                    if (!IsConditionAlreadyProcessed(processingUniqueId, ClaimsProcessClaimCodeCondition.HYPTEN, record.ServiceStartDate))
                                    {
                                        sbReasonForLastChange.Append("HYPTEN");
                                        sbReasonForLastChange.Append(conditionDateWithSemicolon);
                                        hasConditionChanged = true;
                                        processingUniqueId.HasOnlyOtherCondition = false;
                                        isLog.WriteLine($"Hypertension condition is new for unique id {record.UniqueId}");
                                    }
                                    else
                                    {
                                        isLog.WriteLine($"Hypertension condition already processed for unique id {record.UniqueId}");
                                    }
                                }
                                else if (record.HasHypertension)
                                {
                                    isLog.WriteLine($"Hypertension condition previously existed for {record.UniqueId}");
                                }
                                if (((insuranceSummaryRecord == null && !lstUniqueIdsNotInIRS[record.UniqueId].Contains("OBESE")) || !(insuranceSummaryRecord != null && insuranceSummaryRecord.IsObese)) && record.IsObese)
                                {
                                    if (!IsConditionAlreadyProcessed(processingUniqueId, ClaimsProcessClaimCodeCondition.OBESE, record.ServiceStartDate))
                                    {
                                        sbReasonForLastChange.Append("OBESE");
                                        sbReasonForLastChange.Append(conditionDateWithSemicolon);
                                        hasConditionChanged = true;
                                        processingUniqueId.HasOnlyOtherCondition = false;
                                        isLog.WriteLine($"Obese condition is new for unique id {record.UniqueId}");
                                    }
                                    else
                                    {
                                        isLog.WriteLine($"Obese condition already processed for unique id {record.UniqueId}");
                                    }
                                }
                                else if (record.IsObese)
                                {
                                    isLog.WriteLine($"Obese condition previously existed for {record.UniqueId}");
                                }
                                if (((insuranceSummaryRecord == null && !lstUniqueIdsNotInIRS[record.UniqueId].Contains("SLEEP")) || !(insuranceSummaryRecord != null && insuranceSummaryRecord.HasSleepingDisorder)) && record.HasSleepDisorder)
                                {
                                    if (!IsConditionAlreadyProcessed(processingUniqueId, ClaimsProcessClaimCodeCondition.SLEEP, record.ServiceStartDate))
                                    {
                                        sbReasonForLastChange.Append("SLEEP");
                                        sbReasonForLastChange.Append(conditionDateWithSemicolon);
                                        hasConditionChanged = true;
                                        processingUniqueId.HasOnlyOtherCondition = false;
                                        isLog.WriteLine($"Sleep condition is new for unique id {record.UniqueId}");
                                    }
                                    else
                                    {
                                        isLog.WriteLine($"Sleep condition already processed for unique id {record.UniqueId}");
                                    }
                                }
                                else if (record.HasSleepDisorder)
                                {
                                    isLog.WriteLine($"Sleep condition previously existed for {record.UniqueId}");
                                }
                                if (((insuranceSummaryRecord == null && !lstUniqueIdsNotInIRS[record.UniqueId].Contains("CV")) || !(insuranceSummaryRecord != null && insuranceSummaryRecord.HasHeartDisorder)) && record.HasHeartDisorder)
                                {
                                    if (!IsConditionAlreadyProcessed(processingUniqueId, ClaimsProcessClaimCodeCondition.CV, record.ServiceStartDate))
                                    {
                                        sbReasonForLastChange.Append("CV");
                                        sbReasonForLastChange.Append(conditionDateWithSemicolon);
                                        hasConditionChanged = true;
                                        processingUniqueId.HasOnlyOtherCondition = false;
                                        isLog.WriteLine($"CV condition is new for unique id {record.UniqueId}");
                                    }
                                    else
                                    {
                                        isLog.WriteLine($"CV condition already processed for unique id {record.UniqueId}");
                                    }
                                }
                                else if (record.HasHeartDisorder)
                                {
                                    isLog.WriteLine($"Heart disorder condition previously existed for {record.UniqueId}");
                                }
                                if (((insuranceSummaryRecord == null && !lstUniqueIdsNotInIRS[record.UniqueId].Contains("LUNG")) || !(insuranceSummaryRecord != null && insuranceSummaryRecord.HasLungDisorder)) && record.HasLungDisorder)
                                {
                                    if (!IsConditionAlreadyProcessed(processingUniqueId, ClaimsProcessClaimCodeCondition.LUNG, record.ServiceStartDate))
                                    {
                                        sbReasonForLastChange.Append("LUNG");
                                        sbReasonForLastChange.Append(conditionDateWithSemicolon);
                                        hasConditionChanged = true;
                                        processingUniqueId.HasOnlyOtherCondition = false;
                                        isLog.WriteLine($"Lung condition is new for unique id {record.UniqueId}");
                                    }
                                    else
                                    {
                                        isLog.WriteLine($"Lung condition already processed for unique id {record.UniqueId}");
                                    }
                                }
                                else if (record.HasLungDisorder)
                                {
                                    isLog.WriteLine($"Lung condition previously existed for {record.UniqueId}");
                                }
                                if (((insuranceSummaryRecord == null && !lstUniqueIdsNotInIRS[record.UniqueId].Contains("CKD")) || !(insuranceSummaryRecord != null && insuranceSummaryRecord.HasCKD)) && record.HasCKD)
                                {
                                    if (!IsConditionAlreadyProcessed(processingUniqueId, ClaimsProcessClaimCodeCondition.CKD, record.ServiceStartDate))
                                    {
                                        sbReasonForLastChange.Append("CKD");
                                        sbReasonForLastChange.Append(conditionDateWithSemicolon);
                                        hasConditionChanged = true;
                                        processingUniqueId.HasOnlyOtherCondition = false;
                                        isLog.WriteLine($"CKD condition is new for unique id {record.UniqueId}");
                                    }
                                    else
                                    {
                                        isLog.WriteLine($"CKD condition already processed for unique id {record.UniqueId}");
                                    }
                                }
                                else if (record.HasCKD)
                                {
                                    isLog.WriteLine($"CKD condition previously existed for {record.UniqueId}");
                                }

                                if (((insuranceSummaryRecord == null && !lstUniqueIdsNotInIRS[record.UniqueId].Contains("PREDIAB")) || !(insuranceSummaryRecord != null && insuranceSummaryRecord.HasPrediabetes)) && record.HasPrediabetes)
                                {
                                    if (!IsConditionAlreadyProcessed(processingUniqueId, ClaimsProcessClaimCodeCondition.PREDIAB, record.ServiceStartDate))
                                    {
                                        sbReasonForLastChange.Append("PREDIAB");
                                        sbReasonForLastChange.Append(conditionDateWithSemicolon);
                                        hasConditionChanged = true;
                                        processingUniqueId.HasOnlyOtherCondition = false;
                                        isLog.WriteLine($"Prediabetes condition is new for unique id {record.UniqueId}");
                                    }
                                    else
                                    {
                                        isLog.WriteLine($"Prediabetes condition already processed for unique id {record.UniqueId}");
                                    }
                                }
                                else if (record.HasPrediabetes)
                                {
                                    isLog.WriteLine($"Prediabetes condition previously existed for {record.UniqueId}");
                                }

                                if (((insuranceSummaryRecord == null && !lstUniqueIdsNotInIRS[record.UniqueId].Contains("OTHER")) || !(insuranceSummaryRecord != null && insuranceSummaryRecord.NoIVClaimCondition)) && record.NoIVClaimCondition)
                                {
                                    if (!IsConditionAlreadyProcessed(processingUniqueId, ClaimsProcessClaimCodeCondition.OTHER, record.ServiceStartDate))
                                    {
                                        sbReasonForLastChange.Append("OTHER");
                                        sbReasonForLastChange.Append(conditionDateWithSemicolon);
                                        hasConditionChanged = true;
                                        noIVclaimcondition = true;
                                        isLog.WriteLine($"OTHER condition is new for unique id {record.UniqueId}");
                                    }
                                    else
                                    {
                                        isLog.WriteLine($"OTHER condition already processed for unique id {record.UniqueId}");
                                    }
                                }
                                else if (record.NoIVClaimCondition)
                                {
                                    isLog.WriteLine($"OTHER condition previously existed for {record.UniqueId}");
                                }
                                if (sbReasonForLastChange.Length > 0)
                                {
                                    isLog.WriteLine($"Reason for last change is {sbReasonForLastChange.ToString()} for unique id {record.UniqueId}");
                                    // sbSql.Append($" ReasonForLastChange = {insuranceSummaryRecord == null ? "" : insuranceSummaryRecord.},");
                                    // if (insuranceSummaryRecord != null && !String.IsNullOrEmpty(insuranceSummaryRecord.ReasonForLastChange))
                                    // {
                                }
                                else
                                {
                                    isLog.WriteLine($"Reason for last change is empty for unique id {record.UniqueId}");
                                }
                                if (record.ServiceStartDate.HasValue && processingUniqueId.Conditions.Count() > 0)//to do - logic for earliest service date
                                {
                                    if (!processingUniqueId.ServiceStartDate.HasValue || (processingUniqueId.ServiceStartDate.HasValue && record.ServiceStartDate.Value < processingUniqueId.ServiceStartDate.Value))
                                    {
                                        processingUniqueId.ServiceStartDate = record.ServiceStartDate;
                                        sbSql.Append($" EarliestServiceDate = '{record.ServiceStartDate.Value.ToString("MM/dd/yyyy")}',");
                                        runUpdateScript = true;
                                    }
                                }

                                //WHERE clause
                                //remove the , character
                                if (!runUpdateScript)
                                {
                                    sbSql.Replace("UPDATE InsuranceSummary SET", "");
                                }
                                if (runUpdateScript)
                                {
                                    if (insuranceSummaryRecord == null)
                                    {
                                        if (sbSql[sbSql.Length - 1] == ',')
                                            sbSql.Remove(sbSql.Length - 1, 1);
                                        //sbSql.Append(String.Format(insuranceSummaryUpdateSqlCondition2, record.UniqueId, record.OrgId));
                                        sbSql.Append(" WHERE ID = @NewClaimId;");//last modifed date taken care during insert
                                    }
                                    else
                                    {
                                        // sbSql.Append(String.Format(insuranceSummaryUpdateSqlCondition1, insuranceSummaryRecord.Id));
                                        sbSql.Append($" LastModifiedDate = '{conditionDate}' WHERE ID = @NewClaimId;");
                                        insuranceSummaryUpdates++;
                                    }
                                }
                                #endregion

                                #region Code for Condition
                                List<string> codes = new List<string>();
                                codes = GetClaimConditionCodes(record);
                                foreach (var code in codes)
                                {
                                    var conditionArr = code.Split(':');
                                    var codeDesc = conditionArr[2].Replace("'", "");
                                    ClaimConditionCodeDto dto = null;
                                    var date = "";
                                    if (record.ServiceStartDate.HasValue)
                                        date = record.ServiceStartDate.Value.ToString("MM/dd/yyyy");
                                    else
                                        date = DateTime.UtcNow.ToString("MM/dd/yyyy");
                                    if (insuranceSummaryRecord == null || !ClaimConditionCodeDict.TryGetValue(insuranceSummaryRecord.Id + ":" + conditionArr[0] + ":" + conditionArr[1] + ":" + date + ":" + codeDesc, out dto))
                                    {
                                        isLog.WriteLine($"Conditions with new code for unique id {record.UniqueId}");
                                        sbSql.Append(ifnotexistsCCCSql);
                                        sbSql.Append($"'{conditionArr[0]}'");
                                        sbSql.Append("AND cc.Code = ");
                                        sbSql.Append($"'{conditionArr[1]}'");
                                        sbSql.Append("AND cc.ConditionDate = '");
                                        sbSql.Append(date);
                                        sbSql.Append("' and cc.ClaimsId =  ");
                                        sbSql.Append("@NewClaimId)");
                                        sbSql.Append(" BEGIN ");
                                        sbSql.Append(claimConditionCodeInsertSql);
                                        sbSql.Append("@NewClaimId,");
                                        sbSql.Append($"'{conditionArr[0]}'");
                                        sbSql.Append(",'");
                                        sbSql.Append(date);
                                        sbSql.Append("',");
                                        sbSql.Append($"'{conditionArr[1]}'");
                                        sbSql.Append(",");
                                        sbSql.Append($"'{codeDesc}'");
                                        sbSql.Append(");");
                                        sbSql.Append("END ");
                                    }
                                    else
                                    {
                                        isLog.WriteLine($"Conditions with same code previously existed for unique id {record.UniqueId}");
                                    }
                                }
                                #endregion

                                bool insertIntoCandidateTables = false;
                                #region Candidate Reason For Last Change
                                //interested in only conditions which changed for the first time
                                if (hasConditionChanged && sbReasonForLastChange.Length > 0)
                                {
                                    isLog.WriteLine($"Conditions changed for unique id {record.UniqueId}");
                                    var conditionsArray = sbReasonForLastChange.ToString().Split(';');
                                    var d = DateTime.Parse(conditionDate).Date;
                                    var existingConditions = new List<string>();
                                    foreach (var condition in conditionsArray)
                                    {
                                        if (!String.IsNullOrEmpty(condition))
                                        {
                                            var conditionType = condition.Substring(0, condition.Length - 10);
                                            sbSql.Append(ifnotexistsCRFLSql);
                                            sbSql.Append($"'{conditionType}'");
                                            sbSql.Append(" and c.ConditionDate =  ");
                                            sbSql.Append("'" + d.ToString("MM/dd/yyyy") + "'");
                                            sbSql.Append(" and c.ClaimsId =  ");
                                            sbSql.Append("@NewClaimId)");
                                            sbSql.Append("BEGIN ");
                                            sbSql.Append(candidateReasonForLastChangeSql);
                                            sbSql.Append($"'{conditionType}'");
                                            sbSql.Append(",'");
                                            sbSql.Append(d.ToString("MM/dd/yyyy"));
                                            sbSql.Append("',");
                                            sbSql.Append("@NewClaimId");
                                            sbSql.Append(");");
                                            sbSql.Append("END ");
                                            insertIntoCandidateTables = true;
                                            reasonForLastChangeInserts++;
                                            if (insuranceSummaryRecord == null)
                                            {
                                                lstUniqueIdsNotInIRS.TryGetValue(record.UniqueId, out existingConditions);
                                                if (!existingConditions.Contains(conditionType))
                                                {
                                                    existingConditions.Add(conditionType);
                                                    lstUniqueIdsNotInIRS[record.UniqueId] = existingConditions;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    isLog.WriteLine($"No conditions changed for unique id {record.UniqueId}");
                                }
                                #endregion

                                #region Candidate Medication
                                if (!String.IsNullOrEmpty(record.GenericDrug))
                                {

                                    sbSql.Append(candidateMedicationInsertSql);
                                    sbSql.Append($"'{conditionDate}',");
                                    sbSql.Append($"'{record.DrugCategory}',");
                                    sbSql.Append($"'{record.GenericDrug}',");
                                    if (record.ServiceStartDate.HasValue)
                                        sbSql.Append($"'{record.ServiceStartDate.Value.ToString("MM/dd/yyyy")}',");
                                    else
                                        sbSql.Append("null,");
                                    sbSql.Append(record.TotalAmountPaidbyAllSource.HasValue ? record.TotalAmountPaidbyAllSource.Value.ToString() : "null");
                                    sbSql.Append(",");
                                    sbSql.Append(record.PatientPayAmount.HasValue ? record.PatientPayAmount.Value.ToString() : "null");
                                    sbSql.Append(",");
                                    sbSql.Append(record.AmountofCopay.HasValue ? record.AmountofCopay.Value.ToString() : "null");
                                    sbSql.Append(",");
                                    sbSql.Append(record.AmountofCoinsurance.HasValue ? record.AmountofCoinsurance.Value.ToString() : "null");
                                    sbSql.Append(",");
                                    sbSql.Append(record.NetAmountDue.HasValue ? record.NetAmountDue.Value.ToString() : "null");
                                    sbSql.Append(",");
                                    sbSql.Append("@NewClaimId");
                                    sbSql.Append(");");
                                    insertIntoCandidateTables = true;
                                    candidateMedicationInserts++;
                                    if (newClaim && noIVclaimcondition)
                                    {
                                        medicationwithnoclaimconditions++;
                                    }
                                }
                                else
                                {
                                    isLog.WriteLine($"Generic Drug is empty for unique id {record.UniqueId}");
                                }
                                #endregion

                                #region Candidate Condition
                                if (!String.IsNullOrEmpty(record.CodeFlag))
                                {
                                    isLog.WriteLine($"Code Flag is not empty for unique id {record.UniqueId}");
                                    sbSql.Append(candidateConditionInsertSql);
                                    sbSql.Append($"'{conditionDate}',");
                                    sbSql.Append("null,");
                                    sbSql.Append($"'{record.CodeFlag}',");
                                    if (record.ServiceStartDate.HasValue)
                                        sbSql.Append($"'{record.ServiceStartDate.Value.ToString("MM/dd/yyyy")}',");
                                    else
                                        sbSql.Append("null,");
                                    sbSql.Append(record.BilledAmount.HasValue ? record.BilledAmount.Value.ToString() : "null");
                                    sbSql.Append(",");
                                    sbSql.Append(record.Copay.HasValue ? record.Copay.Value.ToString() : "null");
                                    sbSql.Append(",");
                                    sbSql.Append(record.Deductible.HasValue ? record.Deductible.Value.ToString() : "null");
                                    sbSql.Append(",");
                                    sbSql.Append(record.Coinsurance.HasValue ? record.Coinsurance.Value.ToString() : "null");
                                    sbSql.Append(",");
                                    sbSql.Append(record.NetPaid.HasValue ? record.NetPaid.Value.ToString() : "null");
                                    sbSql.Append(",");
                                    sbSql.Append("@NewClaimId");
                                    sbSql.Append(");");
                                    insertIntoCandidateTables = true;
                                    candidateConditionInserts++;
                                }
                                else
                                {
                                    isLog.WriteLine($"Code Flag is empty for unique id {record.UniqueId}");
                                }
                                #endregion
                                if (sbSql.Length > 0 && insertIntoCandidateTables)
                                {
                                    if (record.ProviderName == ClaimProviderDto.AETNA.Name)
                                        AETNAsw.WriteLine(sbSql.ToString());
                                    if (record.ProviderName == ClaimProviderDto.BCBS.Name)
                                        BCBSsw.WriteLine(sbSql.ToString());
                                    if (record.ProviderName == ClaimProviderDto.CVS_CAREMARK.Name)
                                        CVSsw.WriteLine(sbSql.ToString());
                                    if (record.ProviderName == ClaimProviderDto.UHC.Name)
                                        UHCsw.WriteLine(sbSql.ToString());
                                    processingUniqueId.SetClaimIdPreviouslyRun = true;
                                }

                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex);
                            }
                        }

                        //write to logs the count
                        isLog.WriteLine($"Provider name is {providerName}");
                        isLog.WriteLine($"Insurance Summary insert count is {insuranceSummaryInserts}");
                        isLog.WriteLine($"Insurance Summary update count is {insuranceSummaryUpdates}");
                        isLog.WriteLine($"Candidate conditions insert count is {candidateConditionInserts}");
                        isLog.WriteLine($"Candidate medications insert count is {candidateMedicationInserts}");
                        isLog.WriteLine($"Reason for last change insert count is {reasonForLastChangeInserts}");
                    }

                }
                #endregion
                #endregion

                #region Create the final sql
                List<string> SQLinputFilePaths = new List<string>();
                SQLinputFilePaths.Add(AETNAsqlFilePath);
                SQLinputFilePaths.Add(BCBSsqlFilePath);
                SQLinputFilePaths.Add(CVSsqlFilePath);
                SQLinputFilePaths.Add(UHCsqlFilePath);

                List<string> finalSQLinputFilePaths = new List<string>();
                int maxLines = 20000;
                int fileSequence = 0;
                string finalSqlFilePath = null;
                string beginFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Claims", "finalSqlBegin.txt");
                string endFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Claims", "finalSqlEnd.txt");

                foreach (var sqlfilepath in SQLinputFilePaths)
                {
                    StreamWriter writer = null;
                    try
                    {
                        using (StreamReader inputfile = new System.IO.StreamReader(sqlfilepath))
                        {
                            int count = 0;
                            string line;

                            while ((line = inputfile.ReadLine()) != null)
                            {

                                if (writer == null || count >= maxLines)
                                {
                                    if (writer != null)
                                    {
                                        //wait until the next SET
                                        if (!line.TrimStart().StartsWith("SET"))
                                        {
                                            writer.WriteLine(line);
                                            string tempLine = null;
                                            while ((tempLine = inputfile.ReadLine()) != null)
                                            {
                                                if (tempLine.TrimStart().StartsWith("SET"))
                                                {
                                                    line = tempLine;
                                                    break;
                                                }
                                                else
                                                {
                                                    writer.WriteLine(tempLine);
                                                }
                                            }
                                        }
                                        //write the end of file content
                                        using (StreamReader endFile = new System.IO.StreamReader(endFilePath))
                                        {
                                            string endLine;

                                            while ((endLine = endFile.ReadLine()) != null)
                                            {
                                                writer.WriteLine(endLine);
                                            }
                                        }
                                        writer.Close();

                                        writer = null;
                                        fileSequence++;
                                    }
                                    finalSqlFilePath = Path.Combine(Path.GetDirectoryName(ClaimProviderDto.InputFlatFilePath), $"finalInsuranceSummarySql" + fileSequence + ".txt");
                                    writer = new System.IO.StreamWriter(finalSqlFilePath, true);
                                    finalSQLinputFilePaths.Add(finalSqlFilePath);
                                    //write the start of file content
                                    using (StreamReader beginFile = new System.IO.StreamReader(beginFilePath))
                                    {
                                        string beginLine;

                                        while ((beginLine = beginFile.ReadLine()) != null)
                                        {
                                            writer.WriteLine(beginLine);
                                        }
                                    }

                                    count = 0;
                                }

                                writer.WriteLine(line);

                                ++count;
                            }
                            if (writer != null)
                            {
                                //write the end of file content
                                using (StreamReader endFile = new System.IO.StreamReader(endFilePath))
                                {
                                    string endLine;

                                    while ((endLine = endFile.ReadLine()) != null)
                                    {
                                        writer.WriteLine(endLine);
                                    }
                                }
                                writer.Close();
                                fileSequence++;
                            }
                        }
                    }
                    finally
                    {
                        if (writer != null)
                            writer.Close();
                    }
                }



                #endregion

                //write all the files to be automated
                string fileToAutomate = Path.Combine(Path.GetDirectoryName(ClaimProviderDto.InputFlatFilePath), $"filesToAutomate.txt");
                File.WriteAllLines(fileToAutomate, finalSQLinputFilePaths);
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Log.Info("RunTime " + elapsedTime);


            }
        }
        #endregion

        bool IsConditionAlreadyProcessed(ClaimProcessUniqueId processingUniqueId, ClaimsProcessClaimCodeCondition condition, DateTime? conditionStartDate)
        {
            var result = processingUniqueId.Conditions.Contains(condition);// && conditionStartDate.HasValue && processingUniqueId.ServiceStartDate.HasValue && conditionStartDate.Value == processingUniqueId.ServiceStartDate.Value;
            if (!result)
            {
                processingUniqueId.ServiceStartDate = conditionStartDate;
                processingUniqueId.Conditions.Add(condition);
            }
            return result;
        }

        public List<string> GetClaimConditionCodes(ClaimsInputFlatFileModel record)
        {
            List<string> codeString = new List<string>();
            if (record.PrimaryDiagnosisCodeCondition.Count() > 0)
            {
                foreach (var condition in record.PrimaryDiagnosisICDCode)
                {
                    codeString.Add(condition);
                }
            }
            if (record.DiagnosisCode2Condition.Count() > 0)
            {
                foreach (var condition in record.DiagnosisCode2ICDCode)
                {
                    codeString.Add(condition);
                }
            }
            if (record.DiagnosisCode3Condition.Count() > 0)
            {
                foreach (var condition in record.DiagnosisCode3ICDCode)
                {
                    codeString.Add(condition);
                }
            }
            if (record.DiagnosisCode4Condition.Count() > 0)
            {
                foreach (var condition in record.DiagnosisCode4ICDCode)
                {
                    codeString.Add(condition);
                }
            }
            if (record.DiagnosisCode5Condition.Count() > 0)
            {
                foreach (var condition in record.DiagnosisCode5ICDCode)
                {
                    codeString.Add(condition);
                }
            }
            if (record.DiagnosisCode6Condition.Count() > 0)
            {
                foreach (var condition in record.DiagnosisCode6ICDCode)
                {
                    codeString.Add(condition);
                }
            }
            if (record.DiagnosisCode7Condition.Count() > 0)
            {
                foreach (var condition in record.DiagnosisCode7ICDCode)
                {
                    codeString.Add(condition);
                }
            }
            if (record.DiagnosisCode8Condition.Count() > 0)
            {
                foreach (var condition in record.DiagnosisCode8ICDCode)
                {
                    codeString.Add(condition);
                }
            }
            return codeString;
        }
    }

    public sealed class ClaimProcessUniqueId
    {
        public string UniqueId { get; set; }

        public bool IsFirstPass { get; set; }

        public List<ClaimsProcessClaimCodeCondition> Conditions { get; set; }

        public DateTime? ServiceStartDate { get; set; }

        public bool IsPregnantAfter180DaysSatisfied { get; set; }

        public string ProviderName { get; set; }

        public bool SetClaimIdPreviouslyRun { get; set; }

        public bool HasOnlyOtherCondition { get; set; }
    }
}