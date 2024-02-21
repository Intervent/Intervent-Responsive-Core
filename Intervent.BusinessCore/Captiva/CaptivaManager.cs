using CsvHelper;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Intervent.Business
{
    public class CaptivaManager : BaseManager
    {
        ParticipantReader _participantReader = new ParticipantReader();
        PortalReader _portalReader = new PortalReader();
        AccountReader _accountReader = new AccountReader();

        private readonly string uniqueIdLabel = "SubjectID";
        private readonly string ToINTERVENT = "ToINTERVENT";
        private readonly string SubjectFolder = "Subjects";
        private readonly string BiometricFolder = "Biometrics";
        private readonly string MedicationFolder = "Medications";

        private readonly string _captivaFolderPath = ConfigurationManager.AppSettings["CaptivaFolderPath"];
        private readonly int captivaOrgId = Convert.ToInt16(ConfigurationManager.AppSettings["captivaOrgId"]);
        private readonly int SystemAdminId = Convert.ToInt32(ConfigurationManager.AppSettings["SystemAdminId"]);

        public void ProcessCaptivaSubjectFiles()
        {
            try
            {
                Dictionary<string, int> orgCache = new Dictionary<string, int>();
                var files = Directory.GetFiles(Path.Combine(_captivaFolderPath, ToINTERVENT, SubjectFolder), "*.csv");
                var eligibilityBuffer = new List<EligibilityDto>();
                List<string> _errorStrings = new List<string>();
                int bufferCount = 1000;
                int portalId = _portalReader.ListPortals(new ListPortalsRequest { organizationId = captivaOrgId, onlyActive = true }).portals.FirstOrDefault().Id;
                foreach (string file in files)
                {
                    string uniqueId = null;
                    using (var sr = new StreamReader(file))
                    {
                        CsvReader reader = new CsvReader(sr);
                        reader.Configuration.Delimiter = ",";
                        reader.Configuration.IsHeaderCaseSensitive = false;
                        reader.Configuration.RegisterClassMap<EligibilityCsvClassMapV7>();
                        reader.Configuration.SkipEmptyRecords = true;
                        reader.Configuration.TrimFields = true;
                        reader.Configuration.TrimHeaders = true;
                        reader.Configuration.WillThrowOnMissingField = false;
                        while (reader.Read())
                        {
                            try
                            {
                                //flush buffers
                                if (eligibilityBuffer.Count() > bufferCount)
                                {
                                    _participantReader.BulkAddEditEligibility(new BulkAddEditEligibilityRequest() { Eligibilities = eligibilityBuffer });
                                    eligibilityBuffer = new List<EligibilityDto>();
                                }

                                EligibilityCsvModel record = reader.GetRecord<EligibilityCsvModel>();
                                if (string.IsNullOrEmpty(reader.GetField<string>(uniqueIdLabel)))
                                {
                                    continue;
                                }
                                uniqueId = reader.GetField<string>(uniqueIdLabel);

                                //existing dto
                                var eligibilityResponse = this._participantReader.ReadEligibilityInMemory(new ListEligibilityRequest() { uniqueId = uniqueId, portalId = portalId });
                                bool isNewRecord = eligibilityResponse.eligibilityList.Count() == 0;
                                if (isNewRecord)
                                    continue;
                                //Validate
                                var validationResults = new List<ValidationResult>();
                                var validationContext = new ValidationContext(record);
                                var isValid = Validator.TryValidateObject(record, validationContext, validationResults, true);
                                if (isValid)
                                {
                                    //if record passed all validatons 
                                    if (string.IsNullOrEmpty(record.TerminationReason))
                                        record.UserStatus = EligibilityUserStatusDto.Active;
                                    else
                                        record.UserStatus = EligibilityUserStatusDto.Terminated;
                                    var newParticipant = eligibilityResponse.eligibilityList.First();
                                    newParticipant.UserStatus = record.UserStatus;
                                    newParticipant.RegionCode = record.RegionCode;
                                    newParticipant.Gender = record.Gender;
                                    newParticipant.HireDate = record.HireDate;
                                    newParticipant.Race = record.Race;
                                    newParticipant.TerminationReason = record.TerminationReason;
                                    eligibilityBuffer.Add(newParticipant);
                                }
                            }
                            catch (CsvMissingFieldException ex)
                            {
                                _errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                            }
                            catch (CsvBadDataException ex)
                            {
                                _errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                            }
                            catch (Exception ex)
                            {
                                _errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                            }
                        }
                    }
                    //flush buffers
                    _participantReader.BulkAddEditEligibility(new BulkAddEditEligibilityRequest() { Eligibilities = eligibilityBuffer });
                    eligibilityBuffer = new List<EligibilityDto>();

                    #region Move to processed directory

                    string archiveDirectory = Path.Combine(Path.Combine(_captivaFolderPath, ToINTERVENT, SubjectFolder), "ARCHIVE");
                    //create archive directory if directory does not exist
                    if (!Directory.Exists(archiveDirectory))
                    {
                        Directory.CreateDirectory(archiveDirectory);
                    }
                    string fileName = file.Split(new char[] { '\\' }).Last();
                    string destinationFile = Path.Combine(archiveDirectory, fileName.Replace(".", "." + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + "."));

                    File.Move(file, destinationFile);

                    #endregion

                    #region Log Eligibility
                    LogReader logreader = new LogReader();
                    var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "CAPTIVA Subject file Loaded on " + DateTime.Now.ToString() + ". ErrorCount " + _errorStrings.Count(), null, null);
                    logreader.WriteLogMessage(logEvent);
                    #endregion
                }

                #region log error
                if (_errorStrings.Count > 0)
                {
                    LogReader logreader = new LogReader();
                    foreach (var errorString in _errorStrings)
                    {
                        var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, "Error in the CAPTIVA Subject file for the record : " + errorString, null, null);
                        logreader.WriteLogMessage(logEvent);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
        }

        public int ProcessCaptivaBiometricFiles()
        {
            List<BiometricCsvModel> biometricResponses = new List<BiometricCsvModel>();
            var filepath = Path.Combine(_captivaFolderPath, ToINTERVENT, BiometricFolder);
            if (string.IsNullOrEmpty(filepath))
            {
                string extraMessage = "Folder path is empty";
                Log.Warn(extraMessage);
            }
            var count = 0;
            try
            {
                var fileNames = Directory.GetFiles(filepath);
                List<string> _errorStrings = new List<string>();
                if (fileNames.Count() == 0)
                {
                    Log.Debug($"No files found.");
                    return count;
                }
                foreach (var file in fileNames)
                {
                    string uniqueId = null;
                    using (var sr = new StreamReader(file))
                    {
                        CsvReader reader = new CsvReader(sr);
                        reader.Configuration.Delimiter = ",";
                        reader.Configuration.IsHeaderCaseSensitive = false;
                        reader.Configuration.RegisterClassMap<BiometricCsvClassMap>();
                        reader.Configuration.SkipEmptyRecords = true;
                        reader.Configuration.TrimFields = true;
                        reader.Configuration.TrimHeaders = true;
                        reader.Configuration.WillThrowOnMissingField = false;
                        while (reader.Read())
                        {
                            try
                            {
                                BiometricCsvModel record = reader.GetRecord<BiometricCsvModel>();
                                uniqueId = reader.GetField<string>(uniqueIdLabel);
                                var validationResults = new List<ValidationResult>();
                                var validationContext = new ValidationContext(record);
                                var isValid = Validator.TryValidateObject(record, validationContext, validationResults, true);
                                biometricResponses.Add(record);
                            }
                            catch (CsvMissingFieldException ex)
                            {
                                _errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                            }
                            catch (CsvBadDataException ex)
                            {
                                _errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                            }
                            catch (Exception ex)
                            {
                                _errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                            }
                        }
                    }

                    //Move to processed directory
                    if (biometricResponses.Count() > 0)
                    {
                        var bio = biometricResponses.OrderBy(x => x.UniqueId).ThenBy(x => x.Visit.Key).ToList();
                        count += UpdateLab(bio, SystemAdminId, captivaOrgId);
                    }

                    string archiveDirectory = Path.Combine(filepath, "ARCHIVE");
                    //create archive directory if directory does not exist
                    if (!Directory.Exists(archiveDirectory))
                        Directory.CreateDirectory(archiveDirectory);

                    string fileName = file.Split(new char[] { '\\' }).Last();
                    string destinationFile = Path.Combine(archiveDirectory, fileName.Replace(".", "." + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + "."));

                    File.Move(file, destinationFile);

                    LogReader logreader = new LogReader();
                    var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "CAPTIVA Biometric file Loaded on " + DateTime.Now.ToString() + ". ErrorCount " + _errorStrings.Count(), null, null);
                    logreader.WriteLogMessage(logEvent);
                    biometricResponses.Clear();
                }

                if (_errorStrings.Count > 0)
                {
                    LogReader logreader = new LogReader();
                    foreach (var errorString in _errorStrings)
                    {
                        var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, "Error in the CAPTIVA Biometric file for the record : " + errorString, null, null);
                        logreader.WriteLogMessage(logEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, "error occurred while processing Biometric data: " + ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
            return count;
        }

        private int UpdateLab(List<BiometricCsvModel> list, int systemAdminId, int captivaOrgId)
        {
            int count = 0;
            var portal = _portalReader.ListPortals(new ListPortalsRequest { organizationId = captivaOrgId, onlyActive = true }).portals.FirstOrDefault();
            var bioList = list.Where(x => x.Weight.HasValue || x.TotalChol.HasValue || x.LDL.HasValue || x.SBP.HasValue).GroupBy(x => new { x.UniqueId, x.Visit.Key });
            try
            {
                foreach (var record in bioList)
                {
                    LabReader labReader = new LabReader();
                    HRAReader hraReader = new HRAReader();
                    FollowUpReader followUpReader = new FollowUpReader();
                    var keys = record.Key;
                    //Ger user information
                    int? userId, hraId = null, usersinProgramId = null;
                    GetUserRequestByUniqueId request = new GetUserRequestByUniqueId();
                    request.OrganizationId = portal.OrganizationId;
                    request.UniqueId = keys.UniqueId.ToString();
                    var userParticipation = _accountReader.GetUserByUniqueId(request);
                    if (userParticipation.User == null)
                    {
                        EligibilityDto eligibility = _participantReader.GetEligibilityByUniqueId(keys.UniqueId.ToString(), portal.Id);
                        if (eligibility == null)
                            continue;
                        var user = Task.Run(() => _accountReader.CreateUserFromEligibility(eligibility, portal.OrganizationId, true)).Result;
                        userId = user.userId;
                    }
                    else
                    {
                        userId = userParticipation.User.Id;
                        var UserinProgram = userParticipation.User.UsersinPrograms.Where(x => x.IsActive == true && x.ProgramsinPortal.PortalId == portal.Id).FirstOrDefault();
                        if (UserinProgram != null)
                            usersinProgramId = UserinProgram.Id;
                        var hra = userParticipation.User.HRAs.Where(x => x.PortalId == portal.Id).OrderByDescending(x => x.Id).FirstOrDefault();
                        if (hra != null)
                            hraId = hra.Id;
                    }
                    if (!hraId.HasValue)
                    {
                        CreateHRARequest hraRequest = new CreateHRARequest();
                        HRADto HA = new HRADto();
                        HA.UserId = userId.Value;
                        HA.PortalId = portal.Id;
                        HA.CreatedBy = systemAdminId;
                        hraRequest.HRA = HA;
                        hraRequest.languageCode = "en-us";
                        hraRequest.UserinProgramId = usersinProgramId;
                        hraId = hraReader.CreateHRA(hraRequest).HRAId;
                    }
                    //Process labs
                    var bioLabs = record.OrderBy(x => x.AssessmentDate).ToList();
                    for (int i = 0; i < bioLabs.Count; i++)
                    {
                        var lab = bioLabs[i];
                        LabDto labDto = new LabDto();
                        labDto.HDL = lab.HDL;
                        labDto.LDL = lab.LDL;
                        labDto.TotalChol = lab.TotalChol;
                        labDto.Trig = lab.Trig;
                        labDto.BloodTestDate = lab.AssessmentDate;
                        labDto.Height = lab.Height;
                        labDto.SBP = lab.SBP;
                        labDto.DBP = lab.DBP;
                        labDto.A1C = lab.A1C;
                        labDto.Weight = lab.Weight;
                        if (lab.DidYouFast != null)
                            labDto.DidYouFast = (byte)(lab.DidYouFast == YesNoDto.Yes ? 1 : 2);
                        else
                            labDto.DidYouFast = null;
                        labDto.LabSelection = 1;
                        if (labDto.Height.HasValue && labDto.Weight.HasValue && !labDto.BMI.HasValue)
                            labDto.BMI = (float)CommonReader.GetBMI(labDto.Height.Value, labDto.Weight.Value);
                        labDto.UserId = userId.Value;
                        labDto.PortalId = portal.Id;

                        var labRequest = new AddLabRequest();
                        labRequest.Lab = labDto;
                        labRequest.updatedBy = systemAdminId;
                        labRequest.skipAssessmentUpdate = true;
                        var labDAL = labReader.ListLabs(new ReadLabRequest
                        {
                            UserId = labDto.UserId,
                            PortalId = labDto.PortalId
                        }).Labs.Where(x => x.BloodTestDate == labDto.BloodTestDate.Value).FirstOrDefault();
                        if (labDAL != null)
                        {
                            if (!labRequest.Lab.DidYouFast.HasValue)
                                labRequest.Lab.DidYouFast = labDAL.DidYouFast;
                            if (!labRequest.Lab.TotalChol.HasValue)
                                labRequest.Lab.TotalChol = labDAL.TotalChol;
                            if (!labRequest.Lab.HDL.HasValue)
                                labRequest.Lab.HDL = labDAL.HDL;
                            if (!labRequest.Lab.LDL.HasValue)
                                labRequest.Lab.LDL = labDAL.LDL;
                            if (!labRequest.Lab.Trig.HasValue)
                                labRequest.Lab.Trig = labDAL.Trig;
                            if (!labRequest.Lab.A1C.HasValue)
                                labRequest.Lab.A1C = labDAL.A1C;
                            if (!labRequest.Lab.Weight.HasValue)
                                labRequest.Lab.Weight = labDAL.Weight;
                            if (!labRequest.Lab.Height.HasValue)
                                labRequest.Lab.Height = labDAL.Height;
                            if (!labRequest.Lab.BMI.HasValue)
                                labRequest.Lab.BMI = labDAL.BMI;
                            if (!labRequest.Lab.SBP.HasValue)
                                labRequest.Lab.SBP = labDAL.SBP;
                            if (!labRequest.Lab.DBP.HasValue)
                                labRequest.Lab.DBP = labDAL.DBP;
                            labRequest.Id = labDAL.Id;
                        }
                        var labResponse = labReader.AddUpdateLab(labRequest);

                        if (keys.Key != 0)
                        {
                            if (usersinProgramId.HasValue)
                            {
                                var updateFUHealthNumbersRequest = new UpdateFUHealthNumbersfromLabRequest();
                                if ((labDto.TotalChol.HasValue || labDto.LDL.HasValue) && labResponse != null)
                                    labDto.Id = labResponse.labId;
                                else
                                    updateFUHealthNumbersRequest.doNotOverrideLab = true;
                                updateFUHealthNumbersRequest.Lab = labDto;
                                updateFUHealthNumbersRequest.UsersInProgramsId = usersinProgramId.Value;
                                updateFUHealthNumbersRequest.updatedBy = systemAdminId;
                                updateFUHealthNumbersRequest.Lab = labReader.CheckLabErrors(updateFUHealthNumbersRequest.Lab);
                                followUpReader.UpdateFUHealthNumbersfromLab(updateFUHealthNumbersRequest);
                            }
                        }
                        count++;
                    }
                    if (keys.Key == 0)
                    {
                        var firstLipid = bioLabs.Where(x => x.TotalChol.HasValue || x.LDL.HasValue).FirstOrDefault();
                        var firstBio = bioLabs.Where(x => x.SBP.HasValue).FirstOrDefault();

                        LabDto labforHRA = new LabDto();

                        if (firstLipid != null)
                        {
                            labforHRA.HDL = firstLipid.HDL;
                            labforHRA.LDL = firstLipid.LDL;
                            labforHRA.TotalChol = firstLipid.TotalChol;
                            labforHRA.Trig = firstLipid.Trig;
                            labforHRA.A1C = firstLipid.A1C;
                            labforHRA.BloodTestDate = firstLipid.AssessmentDate;
                            if (firstLipid.DidYouFast != null)
                                labforHRA.DidYouFast = (byte)(firstLipid.DidYouFast == YesNoDto.Yes ? 1 : 2);
                            else
                                labforHRA.DidYouFast = null;
                        }
                        if (firstBio != null)
                        {
                            labforHRA.Height = firstBio.Height;
                            labforHRA.SBP = firstBio.SBP;
                            labforHRA.DBP = firstBio.DBP;
                            labforHRA.Weight = firstBio.Weight;
                            if (firstBio.Height.HasValue && firstBio.Weight.HasValue)
                                labforHRA.BMI = (float)CommonReader.GetBMI(firstBio.Height.Value, firstBio.Weight.Value);
                        }

                        if (labforHRA.BloodTestDate.HasValue)
                        {
                            var labDAL = labReader.ListLabs(new ReadLabRequest
                            {
                                UserId = userId.Value,
                                PortalId = portal.Id
                            }).Labs.Where(x => x.BloodTestDate == labforHRA.BloodTestDate.Value).FirstOrDefault();
                            if (labDAL != null)
                                labforHRA.Id = labDAL.Id;
                        }
                        labforHRA.UserId = userId.Value;
                        labforHRA.PortalId = portal.Id;
                        var healthNumbersRequest = new UpdateHealthNumbersFromLabRequest();
                        healthNumbersRequest.HRAValidity = portal.HRAValidity.HasValue ? portal.HRAValidity.Value : 30;
                        healthNumbersRequest.overrideCurrentValue = true;
                        healthNumbersRequest.saveNew = true;
                        healthNumbersRequest.lab = labforHRA;
                        healthNumbersRequest.updatedBy = systemAdminId;
                        healthNumbersRequest.lab = labReader.CheckLabErrors(healthNumbersRequest.lab);
                        hraReader.UpdateHealthNumbersFromLab(healthNumbersRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Biometric Service", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
            return count;
        }

        public int ProcessCaptivaMedicationsFiles()
        {
            List<MedicationCsvModel> medicationList = new List<MedicationCsvModel>();
            var filepath = Path.Combine(_captivaFolderPath, ToINTERVENT, MedicationFolder);
            if (string.IsNullOrEmpty(filepath))
            {
                string extraMessage = "Folder path is empty";
                Log.Warn(extraMessage);
            }
            var count = 0;
            try
            {
                var fileNames = Directory.GetFiles(filepath);
                List<string> _errorStrings = new List<string>();
                if (fileNames.Count() == 0)
                {
                    Log.Debug($"No files found.");
                    return count;
                }
                foreach (var file in fileNames)
                {
                    string uniqueId = null;
                    using (var sr = new StreamReader(file))
                    {
                        CsvReader reader = new CsvReader(sr);
                        reader.Configuration.Delimiter = ",";
                        reader.Configuration.IsHeaderCaseSensitive = false;
                        reader.Configuration.RegisterClassMap<MedicationCsvClassMap>();
                        reader.Configuration.SkipEmptyRecords = true;
                        reader.Configuration.TrimFields = true;
                        reader.Configuration.TrimHeaders = true;
                        reader.Configuration.WillThrowOnMissingField = false;
                        while (reader.Read())
                        {
                            try
                            {
                                MedicationCsvModel record = reader.GetRecord<MedicationCsvModel>();
                                uniqueId = reader.GetField<string>(uniqueIdLabel);
                                var validationResults = new List<ValidationResult>();
                                var validationContext = new ValidationContext(record);
                                var isValid = Validator.TryValidateObject(record, validationContext, validationResults, true);
                                medicationList.Add(record);
                            }
                            catch (CsvMissingFieldException ex)
                            {
                                _errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                            }
                            catch (CsvBadDataException ex)
                            {
                                _errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                            }
                            catch (Exception ex)
                            {
                                _errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                            }
                        }
                    }
                    if (medicationList.Count() > 0)
                        count += UpdateDrugs(medicationList, captivaOrgId, SystemAdminId);

                    string archiveDirectory = Path.Combine(filepath, "ARCHIVE");
                    //create archive directory if directory does not exist
                    if (!Directory.Exists(archiveDirectory))
                    {
                        Directory.CreateDirectory(archiveDirectory);
                    }
                    string fileName = file.Split(new char[] { '\\' }).Last();
                    string destinationFile = Path.Combine(archiveDirectory, fileName.Replace(".", "." + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + "."));

                    File.Move(file, destinationFile);

                    LogReader logreader = new LogReader();
                    var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "CAPTIVA medication file Loaded on " + DateTime.Now.ToString() + ". ErrorCount " + _errorStrings.Count(), null, null);
                    logreader.WriteLogMessage(logEvent);
                    medicationList.Clear();
                }

                if (_errorStrings.Count > 0)
                {
                    LogReader logreader = new LogReader();
                    foreach (var errorString in _errorStrings)
                    {
                        var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, "Error in the CAPTIVA medication file for the record : " + errorString, null, null);
                        logreader.WriteLogMessage(logEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, "error occurred while processing medication data: " + ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
            return count;
        }

        private int UpdateDrugs(List<MedicationCsvModel> list, int captivaOrgId, int systemAdminId)
        {
            int count = 0;
            try
            {
                DrugReader reader = new DrugReader();
                var frequencyList = reader.ListFrequency().Frequency;
                int otherFrequency = frequencyList.Where(x => x.DrugFrequency == "Other").Select(x => x.Id).FirstOrDefault();
                foreach (MedicationCsvModel model in list)
                {
                    try
                    {
                        UserDrugDto drug = new UserDrugDto();
                        drug.MedicationName = model.Medication.ToUpper();
                        drug.Dosage = !string.IsNullOrEmpty(model.Dose) && !string.IsNullOrEmpty(model.Units) ? model.Dose + " " + model.Units : !string.IsNullOrEmpty(model.Dose) ? model.Dose : "N/A";
                        drug.Quantity = 1; //default value
                        drug.Formulation = 11;//Other
                        drug.Frequency = frequencyList.Where(x => x.Code == model.Frequency.ToLower()).Select(x => x.Id).FirstOrDefault() != 0 ? frequencyList.Where(x => x.Code == model.Frequency.ToLower()).Select(x => x.Id).FirstOrDefault() : otherFrequency;
                        drug.Condition = 14; //Other
                        drug.Duration = 4; //Other Duration

                        if (model.StartDate.HasValue && model.StartDate.Value != DateTime.MinValue)
                            drug.MedicationStartDate = model.StartDate.Value;
                        else
                            drug.MedicationStartDate = Convert.ToDateTime("1/1/2001");
                        if (model.EndDate.HasValue && model.EndDate.Value != DateTime.MinValue)
                            drug.DiscontinuedOn = model.EndDate.Value;
                        int? userId;
                        var userResponse = _accountReader.GetUserByUniqueId(new GetUserRequestByUniqueId { UniqueId = model.UniqueId, OrganizationId = captivaOrgId });
                        if (userResponse != null && userResponse.User != null && userResponse.User.Id != 0)
                        {
                            userId = userResponse.User.Id;
                        }
                        else
                        {
                            var portal = _portalReader.ListPortals(new ListPortalsRequest { organizationId = captivaOrgId, onlyActive = true }).portals.FirstOrDefault();
                            EligibilityDto eligibility = _participantReader.GetEligibilityByUniqueId(model.UniqueId, portal.Id);
                            if (eligibility == null)
                                continue;
                            var user = Task.Run(() => _accountReader.CreateUserFromEligibility(eligibility, portal.OrganizationId, true)).Result;
                            userId = user.userId;
                        }
                        if (userId.HasValue)
                        {
                            drug.UserId = userId.Value;
                            drug.AddedBy = systemAdminId;
                            drug.AddedOn = DateTime.UtcNow;
                            var result = reader.AddOrEditUserDrug(new AddOrEditDrugRequest { drug = drug });
                            if (result.Succeeded)
                                count++;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogReader logReader = new LogReader();
                        var logEvent = new LogEventInfo(LogLevel.Error, "Medication Service", null, ex.Message, null, ex);
                        logReader.WriteLogMessage(logEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Medication Service", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
            return count;
        }
    }
}
