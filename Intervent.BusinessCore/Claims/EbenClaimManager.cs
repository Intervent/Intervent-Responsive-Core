using CsvHelper;
using Intervent.Business.Account;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.OleDb;

namespace Intervent.Business.Claims
{
    public class EbenClaimManager : BaseManager
    {
        private static string EbenFolderPath = ConfigurationManager.AppSettings["EbenFolderPath"];

        private static string ebenOrgId = ConfigurationManager.AppSettings["EbenOrgId"];

        ClaimsReader claimsReader = null;

        IAccountManager accountManager = null;

        LogReader logreader = null;

        List<string> _errorStrings = new List<string>();

        List<EbenFlatFileModel> recordsList = new List<EbenFlatFileModel>();

        Dictionary<string, ClaimProcessEligibilityDto> eligibility;

        ILookup<string, ClaimCodeDto> claimICDCodesDict;

        ILookup<string, ClaimCodeDto> claimNDCCodesDict;

        IEnumerable<ClaimProcessHRADto> HRAEnrolledUniqueIds;

        IEnumerable<ClaimProcessEnrolledDataDto> enrolledDataUniqueIds;

        Dictionary<string, ClaimProcessTherapeuticClassCodeDto> theraClassCodesDict;

        int portalId = 0;

        int TotalRecordsCount = 0;

        int skippedCount = 0;

        int inserLimit = 200;

        public EbenClaimManager()
        {
            claimsReader = new ClaimsReader();
            accountManager = new AccountManager();
            logreader = new LogReader();
        }

        public void ProcessRxClaimsFiles()
        {
            string claimsPath = EbenFolderPath + "\\Claims";
            string rxPath = EbenFolderPath + "\\Rx";
            int ProcessedRecordsCount = 0;

            var portal = accountManager.CurrentPortalId(int.Parse(ebenOrgId));
            if (portal != null && portal.PortalId.HasValue)
                portalId = portal.PortalId.Value;
            if (portalId == 0)
            {
                string extraMessage = "No Active Portal";
                Log.Warn(extraMessage);
            }

            if (!Directory.Exists(rxPath))
                Directory.CreateDirectory(rxPath);

            if (!Directory.Exists(claimsPath))
                Directory.CreateDirectory(claimsPath);

            try
            {
                var rxFileNames = Directory.GetFiles(rxPath);
                var claimsFileNames = Directory.GetFiles(claimsPath);
                StringCollection fileNames = new StringCollection();
                fileNames.AddRange(rxFileNames);
                fileNames.AddRange(claimsFileNames);

                if (fileNames.Count == 0)
                {
                    Log.Debug($"No files found.");
                    return;
                }

                var response = claimsReader.GetClaimsProcessEligibilityList(new ListClaimProcessEligibilityRequest() { PortalId = portalId });
                eligibility = response.Eligibility.ToDictionary(x => x.UniqueID, x => x, StringComparer.OrdinalIgnoreCase);
                var claimICDCodes = claimsReader.GetIVICDCodes().ClaimCodes;
                var claimNDCCodes = claimsReader.GetIVNDCCodes().ClaimCodes;
                claimICDCodesDict = claimICDCodes.ToLookup(x => x.Code, x => x, StringComparer.OrdinalIgnoreCase);
                claimNDCCodesDict = claimNDCCodes.ToLookup(x => x.Code, x => x, StringComparer.OrdinalIgnoreCase);
                enrolledDataUniqueIds = claimsReader.GetEbenClaimProcessEnrolledDataUniqueIds(portalId).EnrolledDatas;
                HRAEnrolledUniqueIds = claimsReader.GetEbenClaimProcessHRAUniqueIds(portalId).HRAs;
                var theraClassCodes = claimsReader.GetTherapeuticClassCodes().TherapeuticClassCodes;
                theraClassCodesDict = theraClassCodes.ToDictionary(x => x.TheraCode, x => x, StringComparer.OrdinalIgnoreCase);

                foreach (var filePath in fileNames)
                {
                    if (filePath.ToLower().EndsWith(".xlsx"))
                        ReadRxFile(filePath);
                    else
                        ReadClaimsFile(filePath);
                }

                if (recordsList.Count() > 0)
                {
                    ProcessedRecordsCount += ProcessFlatFileRecords(int.Parse(ebenOrgId));
                    recordsList.Clear();
                }

                foreach (var filePath in fileNames)
                {
                    string arDir = filePath.EndsWith(".xlsx") ? rxPath : claimsPath;
                    string archiveDirectory = Path.Combine(arDir, "ARCHIVE");
                    if (!Directory.Exists(archiveDirectory))
                        Directory.CreateDirectory(archiveDirectory);

                    string fileName = filePath.Split(new char[] { '\\' }).Last();
                    string destinationFile = Path.Combine(archiveDirectory, fileName.Replace(".", "." + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + "."));

                    File.Move(filePath, destinationFile);

                    var logEvent = new LogEventInfo(LogLevel.Trace, "EbenClaimsService", null, "Eben " + fileName + " File Loaded on " + DateTime.Now.ToString() + ".", null, null);
                    logreader.WriteLogMessage(logEvent);
                }

                if (_errorStrings.Count > 0)
                {
                    foreach (var errorString in _errorStrings)
                    {
                        var logEvent = new LogEventInfo(LogLevel.Error, "EbenClaimsService", null, "Error in ebne file for the record : " + errorString, null, null);
                        logreader.WriteLogMessage(logEvent);
                    }
                }

                var log = new LogEventInfo(LogLevel.Info, "EbenClaimsService", null, "Eben record count : " + (TotalRecordsCount + skippedCount) + " ,Processed record count : " + ProcessedRecordsCount + ",Skipped records count : " + skippedCount, null, null);
                logreader.WriteLogMessage(log);
            }
            catch (Exception ex)
            {
                var logEvent = new LogEventInfo(LogLevel.Error, "EbenClaimsService", null, "Error occurred while processing Eben data : " + ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
        }

        private void ReadRxFile(string filePath)
        {
            string con = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=yes;IMEX=1'", filePath);
            using (OleDbConnection connection = new OleDbConnection(con))
            {
                connection.Open();
                var tables = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (tables != null)
                {
                    var sheet = tables.Rows[0]["TABLE_NAME"];
                    OleDbCommand command = new OleDbCommand("select * from [" + sheet + "]", connection);
                    using (OleDbDataReader dr = command.ExecuteReader())
                    {
                        var schema = dr.GetSchemaTable();
                        string UniqueId = "";
                        Dictionary<int, string> columns = new Dictionary<int, string>();
                        for (int i = 0; i < schema.Rows.Count; i++)
                            columns.Add(i, schema.Rows[i].ItemArray[0].ToString().ToLower());

                        while (dr.Read())
                        {
                            try
                            {
                                EbenFlatFileModel model = new EbenFlatFileModel();
                                if (!string.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "member id").Key].ToString()))
                                    UniqueId = model.UniqueId = dr[columns.FirstOrDefault(x => x.Value == "member id").Key].ToString();
                                if (!string.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "transaction date").Key].ToString()))
                                    model.DateOfService = DateTime.Parse(dr[columns.FirstOrDefault(x => x.Value == "transaction date").Key].ToString());
                                if (!string.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "rx Date").Key].ToString()))
                                    model.MedicationDate = DateTime.Parse(dr[columns.FirstOrDefault(x => x.Value == "rx date").Key].ToString());
                                if (!string.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "claim number").Key].ToString()))
                                    model.ClaimId = dr[columns.FirstOrDefault(x => x.Value == "claim number").Key].ToString();
                                if (!string.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "drug").Key].ToString()))
                                    model.Drug = dr[columns.FirstOrDefault(x => x.Value == "drug").Key].ToString();
                                if (!string.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "ndc").Key].ToString()))
                                    model.DiagnosisCode = dr[columns.FirstOrDefault(x => x.Value == "ndc").Key].ToString();
                                if (!string.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "client ing cost").Key].ToString()))
                                    model.ClientIngCost = decimal.Parse(dr[columns.FirstOrDefault(x => x.Value == "client ing cost").Key].ToString());
                                if (!string.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "plan paid").Key].ToString()))
                                    model.PlanPaid = decimal.Parse(dr[columns.FirstOrDefault(x => x.Value == "plan paid").Key].ToString());
                                if (!string.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "member paid").Key].ToString()))
                                    model.PatientPayAmount = decimal.Parse(dr[columns.FirstOrDefault(x => x.Value == "member paid").Key].ToString());
                                if (!string.IsNullOrWhiteSpace(dr[columns.FirstOrDefault(x => x.Value == "therapy class").Key].ToString()))
                                    model.TheraClassCode = dr[columns.FirstOrDefault(x => x.Value == "therapy class").Key].ToString();

                                model.TotalAmountPaid = model.PlanPaid + model.PatientPayAmount;
                                model.NetAmountDue = model.ClientIngCost - model.TotalAmountPaid;
                                model.PortalId = portalId;
                                ClaimProcessEligibilityDto elig = null;
                                eligibility.TryGetValue(UniqueId, out elig);
                                TotalRecordsCount++;
                                if (elig == null)
                                {
                                    skippedCount++;
                                    var logEvent = new LogEventInfo(LogLevel.Warn, "EbenClaimsService", null, "Eben data : Could not determine unique id : " + UniqueId, null, null);
                                    logreader.WriteLogMessage(logEvent);
                                    continue;
                                }
                                model.SetClaimCodeCondition(claimNDCCodesDict);
                                model.SetEnrolledDataFields(enrolledDataUniqueIds, HRAEnrolledUniqueIds);
                                model.SetDrugCategory(theraClassCodesDict);
                                if (!model.RelationshipToSubscriberCode.HasValue)
                                    model.RelationshipToSubscriberCode = elig.UserEnrollmentType == "S" ? ClaimsRelationshipToSubscriberCode.SPOUSE : (elig.UserEnrollmentType == "C" ? ClaimsRelationshipToSubscriberCode.CHILDREN : ClaimsRelationshipToSubscriberCode.EMPLOYEE);
                                recordsList.Add(model);
                            }
                            catch (Exception ex)
                            {
                                _errorStrings.Add(UniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                            }
                        }
                    }
                }
            }
        }

        private void ReadClaimsFile(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                CsvReader reader = new CsvReader(sr);
                reader.Configuration.Delimiter = ",";
                reader.Configuration.IsHeaderCaseSensitive = false;
                reader.Configuration.RegisterClassMap<EbenFlatFileClassMap>();
                reader.Configuration.SkipEmptyRecords = true;
                reader.Configuration.TrimFields = true;
                reader.Configuration.TrimHeaders = true;
                reader.Configuration.WillThrowOnMissingField = false;
                while (reader.Read())
                {
                    string UniqueId = "";
                    try
                    {
                        EbenFlatFileModel model = reader.GetRecord<EbenFlatFileModel>();
                        UniqueId = model.UniqueId;
                        ClaimProcessEligibilityDto elig = null;
                        eligibility.TryGetValue(UniqueId, out elig);
                        TotalRecordsCount++;
                        if (elig == null)
                        {
                            skippedCount++;
                            var logEvent = new LogEventInfo(LogLevel.Warn, "EbenClaimsService", null, "Eben data : Could not determine unique id : " + UniqueId, null, null);
                            logreader.WriteLogMessage(logEvent);
                            continue;
                        }
                        model.PortalId = portalId;
                        model.SetClaimCodeCondition(claimICDCodesDict);
                        model.SetEnrolledDataFields(enrolledDataUniqueIds, HRAEnrolledUniqueIds);
                        if (!model.RelationshipToSubscriberCode.HasValue)
                            model.RelationshipToSubscriberCode = elig.UserEnrollmentType == "S" ? ClaimsRelationshipToSubscriberCode.SPOUSE : (elig.UserEnrollmentType == "C" ? ClaimsRelationshipToSubscriberCode.CHILDREN : ClaimsRelationshipToSubscriberCode.EMPLOYEE);
                        recordsList.Add(model);
                    }
                    catch (CsvMissingFieldException ex)
                    {
                        _errorStrings.Add(UniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                    }
                    catch (CsvBadDataException ex)
                    {
                        _errorStrings.Add(UniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                    }
                    catch (Exception ex)
                    {
                        _errorStrings.Add(UniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                    }
                }
            }
        }

        public List<string> GetClaimConditionCodes(EbenFlatFileModel record)
        {
            List<string> codeString = new List<string>();
            if (record.DiagnosisCodeCondition.Count() > 0)
            {
                foreach (var condition in record.DiagnosisCodeList)
                {
                    codeString.Add(condition);
                }
            }
            return codeString;
        }

        private int ProcessFlatFileRecords(int ebenOrgId)
        {
            int count = 0;
            var ClaimConditionCode = claimsReader.GetClaimConditionCodes().ClaimConditionCodes;
            var ClaimConditionCodeDict = ClaimConditionCode.ToDictionary(x => x.ClaimsID + ":" + x.Condition + ":" + x.Code + ":" + x.ConditionDate + ":" + x.CodeDescription, x => x, StringComparer.OrdinalIgnoreCase);
            var insuranceResultSummary = claimsReader.GetInsuranceResultSummaries(ebenOrgId).InsuranceSummaries;
            Dictionary<string, ClaimProcessInsuranceSummaryDto> insuranceResultSummaryDict = insuranceResultSummary.ToDictionary(x => x.UniqueId, x => x, StringComparer.OrdinalIgnoreCase);

            recordsList = recordsList.OrderBy(x => x.UniqueId).ToList();
            List<CandidateReasonForLastChangeDto> sbReasonForLastChange = new List<CandidateReasonForLastChangeDto>();
            List<CandidateConditionsDto> candidateConditionsDto = new List<CandidateConditionsDto>();
            List<CandidateMedicationsDto> candidateMedicationsDto = new List<CandidateMedicationsDto>();
            List<ClaimConditionCodeDto> claimConditionCodeDto = new List<ClaimConditionCodeDto>();
            List<InsuranceSummaryDto> InsuranceSummaryDto = new List<InsuranceSummaryDto>();

            foreach (EbenFlatFileModel record in recordsList)
            {
                string conditionDate = DateTime.UtcNow.ToString("MM/01/yyyy");
                try
                {
                    if (InsuranceSummaryDto.Count() + 5 >= inserLimit)
                    {
                        claimsReader.BulkEditInsuranceSummary(InsuranceSummaryDto);
                        InsuranceSummaryDto.Clear();
                        insuranceResultSummary = claimsReader.GetInsuranceResultSummaries(ebenOrgId).InsuranceSummaries;
                        insuranceResultSummaryDict = insuranceResultSummary.ToDictionary(x => x.UniqueId, x => x, StringComparer.OrdinalIgnoreCase);
                    }

                    InsuranceSummaryDto insuranceSummary = new InsuranceSummaryDto();
                    var insuranceSummaryRecord = insuranceResultSummaryDict.ContainsKey(record.UniqueId) ? insuranceResultSummaryDict[record.UniqueId] : null;
                    if (insuranceSummaryRecord == null)
                    {
                        insuranceSummary.Eligible = true;
                        insuranceSummary.HRA = record.HasHRA;
                        insuranceSummary.EnrollType = record.EnrollType;
                        insuranceSummary.DataSrc = "EBEN";
                        insuranceSummary.CreateDate = DateTime.UtcNow;
                        insuranceSummary.ConditionCount = 0;
                        insuranceSummary.Inactive = false;
                        insuranceSummary.UniqueID = record.UniqueId;
                        insuranceSummary.OrganizationId = ebenOrgId;
                        insuranceSummary.Spouse = record.IsSpouse;
                        insuranceSummary = claimsReader.AddOrEditInsuranceSummary(insuranceSummary).InsuranceSummary;
                        var ins = new ClaimProcessInsuranceSummaryDto()
                        {
                            Id = insuranceSummary.ID,
                            UniqueId = insuranceSummary.UniqueID,
                            PortalId = portalId,
                            OrgId = insuranceSummary.OrganizationId.Value,
                            HRA = insuranceSummary.HRA
                        };
                        insuranceResultSummaryDict.Add(record.UniqueId, ins);
                    }
                    else
                    {
                        insuranceSummary.ID = insuranceSummaryRecord.Id;
                        insuranceSummary.UniqueID = record.UniqueId;
                        insuranceSummary.OrganizationId = ebenOrgId;
                        if (record.EnrollType != insuranceSummaryRecord.EnrollType || record.HasHRA != insuranceSummaryRecord.HRA)
                        {
                            insuranceSummary.EnrollType = record.EnrollType;
                            insuranceSummary.HRA = record.HasHRA;
                            if (InsuranceSummaryDto.Where(i => i.UniqueID == insuranceSummary.UniqueID && i.EnrollType == record.EnrollType && i.HRA == record.HasHRA).Count() == 0)
                                InsuranceSummaryDto.Add(insuranceSummary);
                        }
                    }

                    if (insuranceSummary != null && insuranceSummary.ID != 0)
                    {
                        CandidateReasonForLastChangeDto lastChange = new CandidateReasonForLastChangeDto
                        {
                            ClaimsId = insuranceSummary.ID,
                            ConditionDate = DateTime.UtcNow,
                            RecentConditionDate = DateTime.UtcNow
                        };

                        if (sbReasonForLastChange.Count() + 15 >= inserLimit)
                        {
                            claimsReader.BulkAddCandidateReasonForLastChanges(sbReasonForLastChange);
                            sbReasonForLastChange.Clear();
                        }

                        if ((insuranceSummaryRecord == null || !insuranceSummaryRecord.IsPregnant) && record.IsPregnant)
                        {
                            lastChange.ConditionType = "PREG";
                            if (insuranceSummaryRecord != null && insuranceSummaryRecord.LatestPregnancyDate.HasValue)
                            {
                                var lastPregnancyDate = insuranceSummaryRecord.LatestPregnancyDate.Value;
                                var lapsedDays = (DateTime.ParseExact(conditionDate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture) - lastPregnancyDate).TotalDays;
                                if (lapsedDays >= 180)
                                {
                                    sbReasonForLastChange.Add(lastChange);
                                }
                            }
                            else
                            {
                                sbReasonForLastChange.Add(lastChange);
                            }
                        }

                        if ((insuranceSummaryRecord == null || !insuranceSummaryRecord.IsDiabetic) && record.IsDiabetic && sbReasonForLastChange.Where(x => x.ConditionType == "DIAB" && x.ClaimsId == insuranceSummary.ID).Count() == 0)
                        {
                            lastChange.ConditionType = "DIAB";
                            sbReasonForLastChange.Add(lastChange);
                        }

                        if ((insuranceSummaryRecord == null || !insuranceSummaryRecord.IsSmoking) && record.IsSmoking && sbReasonForLastChange.Where(x => x.ConditionType == "SMOKE" && x.ClaimsId == insuranceSummary.ID).Count() == 0)
                        {
                            lastChange.ConditionType = "SMOKE";
                            sbReasonForLastChange.Add(lastChange);
                        }

                        if ((insuranceSummaryRecord == null || !insuranceSummaryRecord.IsObese) && record.IsObese && sbReasonForLastChange.Where(x => x.ConditionType == "OBESE" && x.ClaimsId == insuranceSummary.ID).Count() == 0)
                        {
                            lastChange.ConditionType = "OBESE";
                            sbReasonForLastChange.Add(lastChange);
                        }

                        if ((insuranceSummaryRecord == null || !insuranceSummaryRecord.HasSleepingDisorder) && record.HasSleepDisorder && sbReasonForLastChange.Where(x => x.ConditionType == "SLEEP" && x.ClaimsId == insuranceSummary.ID).Count() == 0)
                        {
                            lastChange.ConditionType = "SLEEP";
                            sbReasonForLastChange.Add(lastChange);
                        }

                        if ((insuranceSummaryRecord == null || !insuranceSummaryRecord.HasHyperTension) && record.HasHypertension && sbReasonForLastChange.Where(x => x.ConditionType == "HYPTEN" && x.ClaimsId == insuranceSummary.ID).Count() == 0)
                        {
                            lastChange.ConditionType = "HYPTEN";
                            sbReasonForLastChange.Add(lastChange);
                        }

                        if ((insuranceSummaryRecord == null || !insuranceSummaryRecord.HasHeartDisorder) && record.HasHeartDisorder && sbReasonForLastChange.Where(x => x.ConditionType == "CV" && x.ClaimsId == insuranceSummary.ID).Count() == 0)
                        {
                            lastChange.ConditionType = "CV";
                            sbReasonForLastChange.Add(lastChange);
                        }

                        if ((insuranceSummaryRecord == null || !insuranceSummaryRecord.HasLungDisorder) && record.HasLungDisorder && sbReasonForLastChange.Where(x => x.ConditionType == "LUNG" && x.ClaimsId == insuranceSummary.ID).Count() == 0)
                        {
                            lastChange.ConditionType = "LUNG";
                            sbReasonForLastChange.Add(lastChange);
                        }

                        if ((insuranceSummaryRecord == null || !insuranceSummaryRecord.HasCKD) && record.HasCKD && sbReasonForLastChange.Where(x => x.ConditionType == "CKD" && x.ClaimsId == insuranceSummary.ID).Count() == 0)
                        {
                            lastChange.ConditionType = "CKD";
                            sbReasonForLastChange.Add(lastChange);
                        }

                        if ((insuranceSummaryRecord == null || !insuranceSummaryRecord.HasPrediabetes) && record.HasPrediabetes && sbReasonForLastChange.Where(x => x.ConditionType == "PREDIAB" && x.ClaimsId == insuranceSummary.ID).Count() == 0)
                        {
                            lastChange.ConditionType = "PREDIAB";
                            sbReasonForLastChange.Add(lastChange);
                        }

                        if ((insuranceSummaryRecord == null || !insuranceSummaryRecord.NoIVClaimCondition) && record.NoIVClaimCondition && sbReasonForLastChange.Where(x => x.ConditionType == "OTHER" && x.ClaimsId == insuranceSummary.ID).Count() == 0)
                        {
                            lastChange.ConditionType = "OTHER";
                            sbReasonForLastChange.Add(lastChange);
                        }

                        if (string.IsNullOrEmpty(record.Drug))
                        {
                            List<string> codes = new List<string>();
                            codes = GetClaimConditionCodes(record);

                            if (claimConditionCodeDto.Count() + codes.Count() >= inserLimit)
                            {
                                claimsReader.BulkAddClaimConditionCode(claimConditionCodeDto);
                                claimConditionCodeDto.Clear();
                            }

                            if (codes.Count > 0)
                            {
                                foreach (var code in codes)
                                {
                                    var conditionArr = code.Split(':');
                                    var codeDesc = conditionArr[2].Replace("'", "");
                                    ClaimConditionCodeDto dto = null;
                                    DateTime date = record.ServiceStartDate.HasValue ? record.ServiceStartDate.Value : DateTime.UtcNow;
                                    if (!ClaimConditionCodeDict.TryGetValue(insuranceSummary.ID + ":" + conditionArr[0] + ":" + conditionArr[1] + ":" + date + ":" + codeDesc, out dto))
                                    {
                                        dto = new ClaimConditionCodeDto();
                                        dto.ClaimsID = insuranceSummary.ID;
                                        dto.Condition = conditionArr[0];
                                        dto.ConditionDate = date;
                                        dto.Code = conditionArr[1];
                                        dto.CodeDescription = codeDesc;
                                        claimConditionCodeDto.Add(dto);
                                        ClaimConditionCodeDict.Add(dto.ClaimsID + ":" + dto.Condition + ":" + dto.Code + ":" + dto.ConditionDate + ":" + dto.CodeDescription, dto);
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(record.CodeFlag))
                            {
                                if (candidateConditionsDto.Where(x => x.ClaimsID == insuranceSummary.ID && x.ConditionType == record.CodeFlag).Count() == 0)
                                {
                                    CandidateConditionsDto cc = new CandidateConditionsDto();
                                    cc.ClaimsID = insuranceSummary.ID;
                                    if (record.ServiceStartDate.HasValue)
                                        cc.SourceDataDate = record.ServiceStartDate.Value;
                                    cc.ConditionName = null;
                                    cc.ConditionType = record.CodeFlag;
                                    if (record.ServiceStartDate.HasValue)
                                        cc.ConditionDate = record.ServiceStartDate.Value;
                                    if (record.BilledAmount.HasValue)
                                        cc.BilledAmount = record.BilledAmount.Value;
                                    if (record.Copay.HasValue)
                                        cc.Copay = record.Copay.Value;
                                    if (record.Deductible.HasValue)
                                        cc.Deductible = record.Deductible.Value;
                                    if (record.Coinsurance.HasValue)
                                        cc.Coinsurance = record.Coinsurance.Value;
                                    if (record.NetPaid.HasValue)
                                        cc.NetPaid = record.NetPaid.Value;
                                    candidateConditionsDto.Add(cc);

                                    if (candidateConditionsDto.Count() == inserLimit)
                                    {
                                        claimsReader.BulkAddCandidateCondition(candidateConditionsDto);
                                        candidateConditionsDto.Clear();
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(record.Drug))
                        {
                            if (candidateMedicationsDto.Where(x => x.ClaimsId == insuranceSummary.ID && x.MedicationName == record.Drug.ToUpper() && x.MedicationDate == record.MedicationDate).Count() == 0)
                            {
                                CandidateMedicationsDto cm = new CandidateMedicationsDto();
                                cm.ClaimsId = insuranceSummary.ID;
                                cm.SourceDataDate = record.ServiceStartDate.HasValue ? record.ServiceStartDate.Value : DateTime.UtcNow;
                                cm.MedicationType = record.DrugCategory;
                                cm.MedicationName = record.Drug.ToUpper();
                                cm.MedicationDate = record.MedicationDate;
                                if (record.TotalAmountPaid.HasValue)
                                    cm.Total_Amount_Paid_by_All_Source = record.TotalAmountPaid.Value;
                                if (record.PatientPayAmount.HasValue)
                                    cm.Patient_Pay_Amount = record.PatientPayAmount.Value;
                                if (record.Copay.HasValue)
                                    cm.Amount_of_Copay = record.Copay.Value;
                                if (record.Coinsurance.HasValue)
                                    cm.Amount_of_Coinsurance = record.Coinsurance.Value;
                                if (record.NetAmountDue.HasValue)
                                    cm.Net_Amount_Due__Total_Amount_Billed_Paid_ = record.NetAmountDue.Value;

                                candidateMedicationsDto.Add(cm);

                                if (candidateMedicationsDto.Count() == inserLimit)
                                {
                                    claimsReader.BulkAddOrEditCandidateMedications(candidateMedicationsDto);
                                    candidateMedicationsDto.Clear();
                                }
                            }
                        }
                    }
                    else
                    {
                        skippedCount++;
                        var logEvent = new LogEventInfo(LogLevel.Warn, "EbenClaimsService", null, "Eben Insurance Summary : Could not determine unique id : " + record.UniqueId, null, null);
                        logreader.WriteLogMessage(logEvent);
                    }
                    count++;
                }
                catch (Exception ex)
                {
                    var logEvent = new LogEventInfo(LogLevel.Error, "EbenClaimsService", null, "Error while processing Eben data", null, ex);
                    logreader.WriteLogMessage(logEvent);
                }
            }

            try
            {
                if (InsuranceSummaryDto.Count() > 0)
                    claimsReader.BulkEditInsuranceSummary(InsuranceSummaryDto);

                if (candidateConditionsDto.Count() > 0)
                    claimsReader.BulkAddCandidateCondition(candidateConditionsDto);

                if (candidateMedicationsDto.Count() > 0)
                    claimsReader.BulkAddOrEditCandidateMedications(candidateMedicationsDto);

                if (claimConditionCodeDto.Count() > 0)
                    claimsReader.BulkAddClaimConditionCode(claimConditionCodeDto);

                if (sbReasonForLastChange.Count() > 0)
                    claimsReader.BulkAddCandidateReasonForLastChanges(sbReasonForLastChange);
            }
            catch (Exception ex)
            {
                var logEvent = new LogEventInfo(LogLevel.Error, "EbenClaimsService", null, "Error while processing Eben data", null, ex);
                logreader.WriteLogMessage(logEvent);
            }
            return count;
        }
    }
}
