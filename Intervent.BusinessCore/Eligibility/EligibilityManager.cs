using CsvHelper;
using Framework.Exceptions.Eligibility;
using Intervent.Business.Account;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Intervent.Web.DTO.Diff;
using Newtonsoft.Json;
using NLog;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;

namespace Intervent.Business.Eligibility
{
    public class EligibilityManager : BaseManager, IEligibilityManager
    {
        ParticipantReader _participantReader = new ParticipantReader();
        IAccountManager _accountManager = new AccountManager();

        const string ELIGIBILITY_IMPORT_CREATED_BY_USER = "ImportProcess";
        const string UniqueIdColNameFor5 = "Unique Member ID";
        const string UniqueIdColNameFor6 = "SubjectID";
        private readonly string FromINTERVENT = "FromINTERVENT";


        public IEnumerable<OrganizationDto> GetOrganizationsEligibleForImport()
        {
            return new PortalReader().ListOrganizations(new ListOrganizationsRequest()).Organizations.Where(x => x.Active && (x.Portals.Where(y => y.Active && y.EligibilityImportLoadFlag).Count() > 0));
        }

        public void LoadEligibilityFiles()
        {
            try
            {
                var orgs = GetOrganizationsEligibleForImport();
                foreach (OrganizationDto org in orgs)
                {
                    Dictionary<string, int> orgCache = new Dictionary<string, int>();
                    var eligibilityFolderPath = org.Portals.Where(y => y.Active).FirstOrDefault().EligibilityFolderPath;
                    //get the compaign id/portal id
                    var portalId = _accountManager.CurrentPortalId(org.Id.Value).PortalId;
                    if (!portalId.HasValue)
                    {
                        LogReader reader = new LogReader();
                        var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "No active portal for " + org.Name + " organization.", null, null);
                        reader.WriteLogMessage(logEvent);
                        continue;
                    }
                    if (String.IsNullOrEmpty(eligibilityFolderPath))
                    {
                        string extraMessage = "Folder path in table is empty for organization " + org.Description;
                        Log.Warn(extraMessage);
                        continue;
                    }
                    var eligibilityFormat = org.Portals.Where(x => x.Id == portalId).FirstOrDefault().EligibilityFormat;

                    if (eligibilityFormat == (byte)EligibilityFormats.MetLifeGulfFormat)
                    {
                        orgCache = new PortalReader().GetActiveChildOrgNodesWithActivePortal(org.Id.Value);
                    }
                    //check if the folder has file
                    string[] files = null;
                    try
                    {
                        files = Directory.GetFiles(eligibilityFolderPath, "*.csv");
                    }
                    catch (Exception ex)
                    {
                        LogReader reader = new LogReader();
                        var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, ex.Message, null, null);
                        reader.WriteLogMessage(logEvent);
                        continue;
                    }
                    var eligibilityLogBuffer = new List<EligibilityImportLogDto>();
                    var eligibilityBuffer = new List<EligibilityDto>();
                    int bufferCount = 200;
                    foreach (string file in files)
                    {
                        int row = 0;
                        string uniqueId = null;
                        int successCount = 0;
                        int errorCount = 0;
                        StringBuilder captivaCSV = new StringBuilder();
                        TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                        List<String> errorStrings = new List<string>();
                        using (var sr = new StreamReader(file))
                        {
                            CsvReader reader = new CsvReader(sr);
                            reader.Configuration.Delimiter = "|";
                            reader.Configuration.IsHeaderCaseSensitive = false;
                            var uniqueIdColName = "UniqueId";
                            if (eligibilityFormat == (byte)EligibilityFormats.IntuityFormat)
                            {
                                reader.Configuration.RegisterClassMap<EligibilityCsvClassMapV3>();
                            }
                            else if (eligibilityFormat == (byte)EligibilityFormats.LMCFormat)
                            {
                                reader.Configuration.RegisterClassMap<EligibilityCsvClassMapV2>();
                            }
                            else if (eligibilityFormat == (byte)EligibilityFormats.CompassFormat)
                            {
                                reader.Configuration.RegisterClassMap<EligibilityCsvClassMapV1>();
                            }
                            else if (eligibilityFormat == (byte)EligibilityFormats.MetLifeGulfFormat)
                            {
                                uniqueIdColName = UniqueIdColNameFor5;
                                reader.Configuration.RegisterClassMap<EligibilityCsvClassMapV5>();
                            }
                            else if (eligibilityFormat == (byte)EligibilityFormats.CaptivaFormat)
                            {
                                uniqueIdColName = UniqueIdColNameFor6;
                                reader.Configuration.Delimiter = ",";
                                reader.Configuration.RegisterClassMap<EligibilityCsvClassMapV6>();
                                captivaCSV.AppendLine(string.Join(",", new string[] { "SubjectID", "ProcessedDate", "Status" }));
                            }
                            else
                            {
                                reader.Configuration.RegisterClassMap<EligibilityCsvClassMap>();
                            }
                            reader.Configuration.SkipEmptyRecords = true;
                            reader.Configuration.TrimFields = true;
                            reader.Configuration.TrimHeaders = true;
                            reader.Configuration.WillThrowOnMissingField = true;
                            // reader.Configuration.BadDataCallback()
                            row = 0;
                            while (reader.Read())
                            {
                                uniqueId = null;
                                try
                                {
                                    //flush buffers
                                    if (eligibilityLogBuffer.Count() > bufferCount || eligibilityBuffer.Count() > bufferCount)
                                    {
                                        BulkAddEligibilityandLogBufferResponseRequest bulkAddRequest = new BulkAddEligibilityandLogBufferResponseRequest();
                                        bulkAddRequest.eligibilityBuffer = eligibilityBuffer;
                                        bulkAddRequest.eligibilityLogBuffer = eligibilityLogBuffer;
                                        bulkAddRequest.eligibilityFormat = eligibilityFormat;
                                        bulkAddRequest.portalId = portalId;
                                        var bulkAddEligibilityResponse = BulkAddEligibilityandLogBuffer(bulkAddRequest);
                                        eligibilityBuffer = new List<EligibilityDto>();
                                        eligibilityLogBuffer = new List<EligibilityImportLogDto>();
                                        successCount = successCount + bulkAddEligibilityResponse.successCount;
                                        captivaCSV.Append(bulkAddEligibilityResponse.captivaCSV);
                                    }

                                    if (String.IsNullOrEmpty(reader.GetField<string>(uniqueIdColName)) || (eligibilityFormat != 5 && (String.IsNullOrEmpty(reader.GetField<string>("FirstName")) || String.IsNullOrEmpty(reader.GetField<string>("LastName")))))
                                    {
                                        eligibilityLogBuffer.Add(EligibilityImportLogDto("N", "Missing data", portalId.Value, reader.GetField<string>(uniqueIdColName), true, default(int?), reader.GetField<string>("FirstName"), reader.GetField<string>("LastName")));
                                        continue;
                                    }

                                    uniqueId = reader.GetField<string>(uniqueIdColName);
                                    if (eligibilityFormat == (byte)EligibilityFormats.LMCFormat)
                                        uniqueId = Regex.Replace(uniqueId, @"[A-Za-z]+", "");
                                    row++;
                                    EligibilityCsvModel record = reader.GetRecord<EligibilityCsvModel>();

                                    if (eligibilityFormat == (byte)EligibilityFormats.MetLifeGulfFormat)
                                    {
                                        if (string.IsNullOrEmpty(record.CompanyCode) || !orgCache.ContainsKey(record.CompanyCode))
                                        {
                                            eligibilityLogBuffer.Add(EligibilityImportLogDto("N", "Missing Company Code in MetLife Gulf Eligibility", portalId.Value, record.UniqueId, true, default(int?), record.CompanyCode, reader.GetField<string>("LastName")));
                                            continue;
                                        }
                                        //Default value for mandatory fields
                                        record.FirstName = record.LastName = "N/A";
                                        record.EmailAddress = " ";
                                        record.UserEnrollmentType = EligibilityUserEnrollmentTypeDto.Employee;
                                        portalId = orgCache[record.CompanyCode];
                                    }

                                    #region Nullable Datetime corrections
                                    //csv helper sets a default date for nullable properties
                                    if (record.HireDate.HasValue && record.HireDate.Value.Year == 1)
                                    {
                                        record.HireDate = null;
                                    }
                                    if (record.DeathDate.HasValue && record.DeathDate.Value.Year == 1)
                                    {
                                        record.DeathDate = null;
                                    }

                                    if (record.TerminatedDate.HasValue && record.TerminatedDate.Value.Year == 1)
                                    {
                                        record.TerminatedDate = null;
                                    }
                                    if (record.DOB.HasValue && record.DOB.Value.Year == 1)
                                    {
                                        record.DOB = null;
                                    }
                                    if (record.MedicalPlanEndDate.HasValue && record.MedicalPlanEndDate.Value.Year == 1)
                                    {
                                        record.MedicalPlanEndDate = null;
                                    }

                                    if (record.MedicalPlanStartDate.HasValue && record.MedicalPlanStartDate.Value.Year == 1)
                                    {
                                        record.MedicalPlanStartDate = null;
                                    }

                                    if (record.DentalPlanEndDate.HasValue && record.DentalPlanEndDate.Value.Year == 1)
                                    {
                                        record.DentalPlanEndDate = null;
                                    }

                                    if (record.DentalPlanStartDate.HasValue && record.DentalPlanStartDate.Value.Year == 1)
                                    {
                                        record.DentalPlanStartDate = null;
                                    }

                                    if (record.VisionPlanEndDate.HasValue && record.VisionPlanEndDate.Value.Year == 1)
                                    {
                                        record.VisionPlanEndDate = null;
                                    }

                                    if (record.VisionPlanStartDate.HasValue && record.VisionPlanStartDate.Value.Year == 1)
                                    {
                                        record.VisionPlanStartDate = null;
                                    }

                                    if (record.CoachingExpirationDate.HasValue && record.CoachingExpirationDate.Value.Year == 1)
                                    {
                                        record.CoachingExpirationDate = null;
                                    }
                                    if (record.Lab_Date.HasValue && record.Lab_Date.Value.Year == 1)
                                    {
                                        record.Lab_Date = null;
                                    }
                                    if (eligibilityFormat == (byte)EligibilityFormats.LMCFormat)
                                    {
                                        record.UserStatus = EligibilityUserStatusDto.Active;
                                        record.UserEnrollmentType = EligibilityUserEnrollmentTypeDto.Patient;
                                        record.BusinessUnit = "LMC";
                                    }
                                    if (eligibilityFormat == (byte)EligibilityFormats.IntuityFormat && record.UserEnrollmentType == null)
                                    {
                                        record.UserStatus = EligibilityUserStatusDto.Active;
                                        record.UserEnrollmentType = EligibilityUserEnrollmentTypeDto.Patient;
                                    }
                                    if (eligibilityFormat == (byte)EligibilityFormats.CaptivaFormat)
                                    {
                                        record.UserEnrollmentType = EligibilityUserEnrollmentTypeDto.Patient;
                                        if (!record.EmailAddress.Contains('@'))
                                            record.EmailAddress = "";

                                    }
                                    #endregion                                    

                                    //existing dto
                                    var eligibilityResponse = this._participantReader.ReadEligibilityInMemory(new ListEligibilityRequest() { uniqueId = record.UniqueId, portalId = portalId });
                                    bool isNewRecord = eligibilityResponse.eligibilityList.Count() == 0;
                                    var currentEligibilityId = isNewRecord ? -1 : eligibilityResponse.eligibilityList.First().Id.Value;

                                    var validationResults = new List<ValidationResult>();
                                    var validationContext = new ValidationContext(record);
                                    var isValid = Validator.TryValidateObject(record, validationContext, validationResults, true);
                                    if (!isValid)
                                    {
                                        eligibilityLogBuffer.Add(EligibilityImportLogDto(isNewRecord ? "N" : "C", JsonConvert.SerializeObject(validationResults), portalId.Value, record.UniqueId, true, (isNewRecord ? default(int?) : currentEligibilityId)));
                                        errorStrings.Add(record.UniqueId + "," + DateTime.Now.ToString() + "," + JsonConvert.SerializeObject(validationResults).Replace(",", " "));
                                        errorCount++;
                                        if (eligibilityFormat == (byte)EligibilityFormats.CaptivaFormat)
                                        {
                                            captivaCSV.AppendLine(record.UniqueId.ToString() + "," + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToString() + "," + JsonConvert.SerializeObject(validationResults).Replace(",", " ") + "");
                                        }
                                    }
                                    else
                                    {
                                        //if record passed all validatons
                                        //create new dto
                                        var newParticipant = _participantReader.MapCsvModelToDto(record);
                                        newParticipant.PortalId = portalId.Value;

                                        if (isNewRecord)
                                        {
                                            if (eligibilityFormat == (byte)EligibilityFormats.CaptivaFormat)
                                            {
                                                newParticipant.UserStatus = EligibilityUserStatusDto.Active;
                                                newParticipant.EnrollmentStatus = 4;
                                            }
                                            //new record
                                            newParticipant.CreateDate = DateTime.UtcNow;
                                            eligibilityBuffer.Add(newParticipant);
                                            eligibilityLogBuffer.Add(EligibilityImportLogDto("N", null, portalId.Value, uniqueId, false, null, newParticipant.FirstName, newParticipant.LastName));
                                        }
                                        else
                                        {
                                            if (eligibilityFormat == (byte)EligibilityFormats.CaptivaFormat)
                                            {
                                                var eligibility = eligibilityResponse.eligibilityList.First();
                                                newParticipant.Gender = eligibility.Gender;
                                                newParticipant.TerminatedDate = eligibility.TerminatedDate;
                                                newParticipant.RegionCode = eligibility.RegionCode;
                                                newParticipant.Race = eligibility.Race;
                                                newParticipant.HireDate = eligibility.HireDate;
                                                newParticipant.TerminationReason = eligibility.TerminationReason;
                                                newParticipant.UserStatus = eligibility.UserStatus;
                                            }
                                            if (eligibilityResponse.eligibilityList.FirstOrDefault().FirstEligibleDate.HasValue)
                                                newParticipant.FirstEligibleDate = eligibilityResponse.eligibilityList.FirstOrDefault().FirstEligibleDate;

                                            //compare
                                            newParticipant.CreateDate = eligibilityResponse.eligibilityList.First().CreateDate;
                                            var changes = PropertyChanges(eligibilityResponse.eligibilityList.First(), newParticipant, uniqueIdColName);
                                            if (changes.Count() == 0)
                                            {
                                                //no changes. do not update the record. dont log
                                                eligibilityLogBuffer.Add(EligibilityImportLogDto("U", null, portalId.Value, uniqueId, false, currentEligibilityId, newParticipant.FirstName, newParticipant.LastName));
                                                successCount++;
                                                if (eligibilityFormat == (byte)EligibilityFormats.CaptivaFormat)
                                                    captivaCSV.AppendLine(record.UniqueId.ToString() + "," + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToString() + ",Received");
                                            }
                                            else
                                            {
                                                //there are changes
                                                newParticipant.Id = eligibilityResponse.eligibilityList.First().Id.Value;
                                                eligibilityBuffer.Add(newParticipant);
                                                eligibilityLogBuffer.Add(EligibilityImportLogDto("C", null, portalId.Value, uniqueId, false, currentEligibilityId, newParticipant.FirstName, newParticipant.LastName, JsonConvert.SerializeObject(changes)));
                                                if (eligibilityFormat == (byte)EligibilityFormats.CaptivaFormat)
                                                {
                                                    var phoneNumberChange = changes.Where(x => x.PropertyName.Contains("HomeNumber") || x.PropertyName.Contains("CellNumber")).FirstOrDefault();
                                                    if (phoneNumberChange != null)
                                                    {
                                                        AccountReader acctReader = new AccountReader();
                                                        GetUserRequestByUniqueId userRequest = new GetUserRequestByUniqueId();
                                                        userRequest.OrganizationId = org.Id.Value;
                                                        userRequest.UniqueId = uniqueId;
                                                        var user = acctReader.GetUserByUniqueId(userRequest);
                                                        if (user != null)
                                                        {
                                                            string phoneNumberText = "";
                                                            var homeNumberChange = changes.Where(x => x.PropertyName.Contains("HomeNumber")).FirstOrDefault();
                                                            if (homeNumberChange != null)
                                                                phoneNumberText = "Home Number #" + homeNumberChange.NewValue + "; ";
                                                            var cellNumberChange = changes.Where(x => x.PropertyName.Contains("CellNumber")).FirstOrDefault();
                                                            if (cellNumberChange != null)
                                                                phoneNumberText = phoneNumberText + "Cell Number #" + cellNumberChange.NewValue;
                                                            NotesDto noteDto = new NotesDto();
                                                            noteDto.Admin = Convert.ToInt32(ConfigurationManager.AppSettings["SystemAdminId"]);
                                                            noteDto.PortalId = newParticipant.PortalId;
                                                            noteDto.userId = user.User.Id;
                                                            noteDto.NotesDate = DateTime.MinValue;
                                                            noteDto.Type = (int)NoteTypes.Other;
                                                            noteDto.Text = "Phone number(s) changed in eligibility: " + phoneNumberText;
                                                            _participantReader.AddEditNotes(new AddNotesRequest { note = noteDto, TimeZone = "Eastern Standard Time" });
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (CsvMissingFieldException ex)
                                {
                                    errorCount++;
                                    eligibilityLogBuffer.Add(EligibilityImportLogDto("N", ex.Message + "-" + ex.StackTrace, portalId.Value, uniqueId, true, eligibilityId: null));
                                    errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                                    if (eligibilityFormat == (byte)EligibilityFormats.CaptivaFormat)
                                        captivaCSV.AppendLine(uniqueId + "," + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToString() + "," + ex.Message);

                                }
                                catch (CsvBadDataException ex)
                                {
                                    errorCount++;
                                    eligibilityLogBuffer.Add(EligibilityImportLogDto("N", ex.Message + "-" + ex.StackTrace, portalId.Value, uniqueId, true, eligibilityId: null));
                                    errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                                    if (eligibilityFormat == (byte)EligibilityFormats.CaptivaFormat)
                                        captivaCSV.AppendLine(uniqueId + "," + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToString() + "," + ex.Message);
                                }
                                catch (Exception ex)
                                {
                                    errorCount++;
                                    eligibilityLogBuffer.Add(EligibilityImportLogDto("N", ex.Message + "-" + ex.StackTrace, portalId.Value, uniqueId, true, eligibilityId: null));
                                    errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                                    if (eligibilityFormat == (byte)EligibilityFormats.CaptivaFormat)
                                        captivaCSV.AppendLine(uniqueId + "," + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToString() + "," + ex.Message);
                                }
                            }
                        }
                        BulkAddEligibilityandLogBufferResponseRequest request = new BulkAddEligibilityandLogBufferResponseRequest();
                        request.eligibilityBuffer = eligibilityBuffer;
                        request.eligibilityLogBuffer = eligibilityLogBuffer;
                        request.eligibilityFormat = eligibilityFormat;
                        request.portalId = portalId;
                        var bulkAddResponse = BulkAddEligibilityandLogBuffer(request);
                        eligibilityBuffer = new List<EligibilityDto>();
                        eligibilityLogBuffer = new List<EligibilityImportLogDto>();
                        successCount = successCount + bulkAddResponse.successCount;
                        captivaCSV.Append(bulkAddResponse.captivaCSV);

                        #region Move to processed directory

                        string archiveDirectory = Path.Combine(eligibilityFolderPath, "ARCHIVE");
                        //create archive directory if directory does not exist
                        if (!System.IO.Directory.Exists(archiveDirectory))
                        {
                            System.IO.Directory.CreateDirectory(archiveDirectory);
                        }
                        string fileName = file.Split(new char[] { '\\' }).Last();
                        string destinationFile = Path.Combine(archiveDirectory, fileName.Replace(".", "." + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + "."));

                        File.Move(file, destinationFile);

                        #endregion

                        #region create eligibility confirmation file

                        if (eligibilityFormat == (byte)EligibilityFormats.CaptivaFormat && captivaCSV.Length > 0)
                        {
                            string processedFileLogsDirectory = Path.Combine(ConfigurationManager.AppSettings["CaptivaFolderPath"], FromINTERVENT, "Eligibility_Processed");
                            if (!System.IO.Directory.Exists(processedFileLogsDirectory))
                            {
                                System.IO.Directory.CreateDirectory(processedFileLogsDirectory);
                            }
                            var processedFileName = fileName.Split('_')[0];
                            string processedFile = Path.Combine(processedFileLogsDirectory, processedFileName + "_Processed_" + DateTime.UtcNow.ToString("yyyyMMdd") + ".csv");
                            File.WriteAllText(processedFile, captivaCSV.ToString());
                        }
                        #endregion

                        #region create error file
                        if (errorCount > 0)
                        {
                            string errorDirectory = Path.Combine(eligibilityFolderPath, "ErrorFiles");
                            //create error directory if directory does not exist
                            if (!System.IO.Directory.Exists(errorDirectory))
                            {
                                System.IO.Directory.CreateDirectory(errorDirectory);
                            }
                            string errorFile = Path.Combine(errorDirectory, org.Name + "_" + DateTime.UtcNow.ToString("yyyyMMdd") + ".csv");
                            var csv = new StringBuilder();
                            csv.AppendLine("UniqueId,Date & Time, Error Message");
                            if (errorStrings.Count() > 0)
                            {
                                foreach (var errorString in errorStrings)
                                {
                                    csv.AppendLine(errorString);
                                }
                            }
                            File.WriteAllText(errorFile, csv.ToString());
                        }
                        #endregion

                        #region Auto terminate records
                        var terminatedRecordsCount = 0;
                        if (org.Portals.Where(y => y.Active).FirstOrDefault().EligibilityTerminateFlag)
                        {
                            //call stored procedure and get the rowcount for terminated records
                            terminatedRecordsCount = TerminateNotSentRecords(portalId.Value);
                        }
                        var duplicateRecords = DeleteDuplicateRecords(portalId.Value);
                        #endregion

                        #region IntuityUser

                        if (org.IntegrationWith.HasValue && org.IntegrationWith.Value == (int)IntegrationPartner.Intuity)
                        {
                            try
                            {
                                IntuityManager mgr = new IntuityManager(ConfigurationManager.AppSettings["DTCOrgCode"], ConfigurationManager.AppSettings["EbenOrgCode"], Convert.ToInt32(ConfigurationManager.AppSettings["SystemAdminId"]));
                                mgr.SendEligibilityUpdate(portalId.Value);
                            }
                            catch (Exception ex)
                            {
                                LogReader reader = new LogReader();
                                var logIntuityEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Intuity eligibility update failed for " + org.Name + " organization.", null, ex);
                                reader.WriteLogMessage(logIntuityEvent);
                            }

                            try
                            {
                                //IntuityEligibilityManager jobMgr = new IntuityEligibilityManager();
                                //jobMgr.RevalidateEligibility(org.Id.Value, portalId.Value);
                            }
                            catch (Exception ex)
                            {
                                LogReader reader = new LogReader();
                                var logIntuityEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Intuity eligibility revalidation for " + org.Name + " organization.", null, ex);
                                reader.WriteLogMessage(logIntuityEvent);

                            }
                        }
                        #endregion

                        #region Log Eligibility

                        LogReader logreader = new LogReader();
                        var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Eligibility Files Loaded on " + DateTime.Now.ToString() + " for " + org.Name + ". ErrorCount " + errorCount + "; SuccessCount = " + successCount + "; DuplicatesCount = " + duplicateRecords, null, null);
                        logreader.WriteLogMessage(logEvent);

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
        }


        EligibilityImportLogDto EligibilityImportLogDto(string action, string errorDetails, int portalId, string uniqueId, bool isLoadError, int? eligibilityId, string firstName = null, string lastName = null, string changes = null, string createdByUser = null)
        {
            var logRequest = new EligibilityImportLogDto();
            logRequest.Action = action;
            if (!String.IsNullOrEmpty(createdByUser))
            {
                logRequest.CreatedByUser = createdByUser;
            }
            else
            {
                logRequest.CreatedByUser = ELIGIBILITY_IMPORT_CREATED_BY_USER;

            }
            logRequest.ErrorDetails = errorDetails;
            logRequest.IsLoadError = isLoadError;
            logRequest.PortalId = portalId;
            if (eligibilityId.HasValue)
            {
                logRequest.EligibilityId = eligibilityId;
            }
            logRequest.UniqueId = uniqueId;
            logRequest.FirstName = firstName;
            logRequest.LastName = lastName;
            logRequest.ChangedFields = changes;

            return logRequest;
        }

        public void AddEligibilityImportLog(string action, string errorDetails, int portalId, string uniqueId, bool isLoadError, int? eligibilityId, string firstName = null, string lastName = null, string changes = null, string createdByUser = null)
        {
            var logRequest = new EligibilityImportLogDto();
            logRequest.Action = action;
            if (!String.IsNullOrEmpty(createdByUser))
            {
                logRequest.CreatedByUser = createdByUser;
            }
            else
            {
                logRequest.CreatedByUser = ELIGIBILITY_IMPORT_CREATED_BY_USER;

            }
            logRequest.ErrorDetails = errorDetails;
            logRequest.IsLoadError = isLoadError;
            logRequest.PortalId = portalId;
            if (eligibilityId.HasValue)
            {
                logRequest.EligibilityId = eligibilityId;
            }
            logRequest.UniqueId = uniqueId;
            logRequest.FirstName = firstName;
            logRequest.LastName = lastName;
            logRequest.ChangedFields = changes;

            _participantReader.AddEligibilityImportLog(new AddEligibilityImportLogRequest() { EligibilityImportLog = logRequest });
        }

        public BulkAddEligibilityandLogBufferResponse BulkAddEligibilityandLogBuffer(BulkAddEligibilityandLogBufferResponseRequest request)
        {
            BulkAddEligibilityandLogBufferResponse response = new BulkAddEligibilityandLogBufferResponse();
            TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            _participantReader.BulkAddEditEligibility(new BulkAddEditEligibilityRequest() { Eligibilities = request.eligibilityBuffer });
            var uniqueIds = request.eligibilityBuffer.Select(x => x.UniqueId).ToList();
            var processedUniqueIds = _participantReader.GetProcessedEligibilityList(new GetProcessedEligibilityRequest() { uniqueIds = uniqueIds, portalId = request.portalId.Value }).processedUniqueIds;

            //proccessed records 
            StringBuilder captivaCSV = new StringBuilder();
            response.successCount = processedUniqueIds.Count();
            foreach (var uniqueId in uniqueIds)
            {
                if (processedUniqueIds.Contains(uniqueId))
                {
                    if (request.eligibilityFormat == (byte)EligibilityFormats.CaptivaFormat)
                        captivaCSV.AppendLine(uniqueId + "," + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToString() + ",Received");
                }
                else
                {
                    if (request.eligibilityFormat == (byte)EligibilityFormats.CaptivaFormat)
                        captivaCSV.AppendLine(uniqueId + "," + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToString() + ",Not Processed");
                    var item = request.eligibilityLogBuffer.Where(x => x.UniqueId == uniqueId).FirstOrDefault();
                    if (item != null)
                    {
                        item.ErrorDetails = "Not Processed";
                        item.IsLoadError = true;
                    }
                }
            }
            response.captivaCSV = captivaCSV;
            //flush buffers
            _participantReader.BulkAddEligibilityImportLog(new BulkAddEligibilityImportLogRequest() { EligibilityImportLogs = request.eligibilityLogBuffer });
            return response;
        }

        static IEnumerable<PropertyCompare> PropertyChanges(EligibilityDto existingObject, EligibilityDto newObject, string uniqueIdColName)
        {
            //check if organization id and unique id are the same
            if (newObject.UniqueId != existingObject.UniqueId)
            {
                throw new EligibilityException("Unique id is not the same");
            }

            string[] propertiesNotToCompareArray = new string[] { "Id", "PortalId", uniqueIdColName, "Portal" };

            return PropertyCompare.FindDifferences(existingObject, newObject, propertiesNotToCompareArray);
        }

        public AddEditEligibilityResponse AddEditEligibilityRecord(AddEditEligibilityRequest request)
        {
            return this._participantReader.AddEditEligibility(request);
        }

        public void UpdateUserEligibilitySetting(UpdateUserEligibilitySettingRequest request)
        {
            this._participantReader.UpdateUserEligibilitySetting(request);
        }

        public GetUserEligibilitySettingResponse GetUserEligibilitySetting(GetUserEligibilitySettingRequest request)
        {
            return this._participantReader.GetUserEligibilitySetting(request);
        }

        public int TerminateNotSentRecords(int portalId)
        {
            return this._participantReader.TerminateNotSentEligibilityRecords(portalId);
        }

        public int DeleteDuplicateRecords(int portalId)
        {
            return this._participantReader.DeleteDuplicateRecords(portalId);
        }
    }
}
