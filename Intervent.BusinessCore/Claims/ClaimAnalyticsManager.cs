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
    public class ClaimAnalyticsManager : BaseManager
    {
        IAccountManager accountManager = null;
        ClaimsReader claimsReader = null;

        public ClaimAnalyticsManager()
        {
            claimsReader = new ClaimsReader();
            accountManager = new AccountManager();
        }
        #region Process Claims
        public void ProcessClaims()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            ClaimProviderDto.BaseDirectoryPath = "C:\\Users\\Admin\\Desktop\\Keerthi\\Reporting";

            //check if the directories exist for current month and year
            string directoryName = DateTime.UtcNow.ToString("yyyyMM"); //String.Concat(DateTime.UtcNow.Y ear, DateTime.UtcNow.Month);
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

            var response = claimsReader.GetEligibilityList();
            var eligibility = response.Eligibility.Where(x => x.SSN != "" && x.SSN != null && x.UserEnrollmentType != "C").ToDictionary(x => String.Concat(x.SSN, x.UserEnrollmentType), x => x, StringComparer.OrdinalIgnoreCase);
            string eligibilityJson = JsonConvert.SerializeObject(eligibility);
            File.WriteAllText(ClaimProviderDto.EligibilityFlatFilePath, eligibilityJson, Encoding.UTF8);
            var eligibilitySSNKey = response.Eligibility.Where(x => x.SSN != "" && x.SSN != null && !String.IsNullOrEmpty(x.MedicalPlanCode)).ToDictionary(x => String.Concat(x.SSN, x.UserEnrollmentType), x => x, StringComparer.OrdinalIgnoreCase);
            var eligibilityUniqueIdKey = response.Eligibility.Where(x => x.UniqueID != "" && x.UniqueID != null && x.UserEnrollmentType != "C" && !String.IsNullOrEmpty(x.MedicalPlanCode)).ToDictionary(x => x.UniqueID, x => x, StringComparer.OrdinalIgnoreCase);
            var childreneligibilityUniqueIdKey = response.Eligibility.Where(x => x.UniqueID != "" && x.UniqueID != null && x.UserEnrollmentType == "C" && !String.IsNullOrEmpty(x.MedicalPlanCode)).ToDictionary(x => String.Concat(x.UniqueID + x.FirstName), x => x, StringComparer.OrdinalIgnoreCase);

            var livongoICDCodes = claimsReader.GetLivongoICDCodes().ICDCodes;
            string livongoICDCodesJson = JsonConvert.SerializeObject(livongoICDCodes);
            File.WriteAllText(ClaimProviderDto.LivongoICDCodesFilePath, livongoICDCodesJson, Encoding.UTF8);
            var livongoICDCodesDict = livongoICDCodes.ToDictionary(x => x.Code, x => x, StringComparer.OrdinalIgnoreCase);

            var livongoNDCCodes = claimsReader.GetLivongoNDCCodes().NDCCodes;
            string livongoNDCCodesJson = JsonConvert.SerializeObject(livongoNDCCodes);
            File.WriteAllText(ClaimProviderDto.LivongoNDCCodesFilePath, livongoNDCCodesJson, Encoding.UTF8);
            var livongoNDCCodesDict = livongoNDCCodes.ToDictionary(x => x.Code, x => x, StringComparer.OrdinalIgnoreCase);


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

                    foreach (var filename in fileNames)
                    {
                        string fileName = filename;

                        //move file to the monthly folder
                        File.Move(fileName, Path.Combine(claimProvider.MonthlySourceDirectoryPath, Path.GetFileName(fileName)));
                    }

                    //check if the file is a fixed layout file
                    var files = Directory.GetFiles(claimProvider.MonthlySourceDirectoryPath);
                    foreach (var fileName in files)
                    {
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
                foreach (var filePath in filePaths)
                {
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
                                        //record.PortalId = portalId;
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
                                            if (!String.IsNullOrEmpty(record.UniqueId) && !record.SkipUniqueId)
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
                                        }
                                        #endregion
                                        if (record.IncludeInLivongoOutput)
                                        {
                                            writerLivongo.WriteRecord(record);
                                        }
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
            }
        }
    }
}