namespace Intervent.Web.DTO
{
    public sealed class ClaimsInputFlatFileModel
    {
        public int InputFileRowId { get; set; }
        public string ProviderName { get; set; }
        public string MemberSSN { get; set; }
        public string MemberFirstName { get; set; }
        public string MemberLastName { get; set; }
        public DateTime? MemberDateOfBirth { get; set; }//change string to date
        public DateTime? ServiceStartDate { get; set; }//change string to date
        public string PrimaryDiagnosisCode { get; set; }
        public string DiagnosisCode2 { get; set; }
        public string DiagnosisCode3 { get; set; }
        public string DiagnosisCode4 { get; set; }
        public string DiagnosisCode5 { get; set; }
        public string DiagnosisCode6 { get; set; }
        public string DiagnosisCode7 { get; set; }
        public string DiagnosisCode8 { get; set; }

        public IEnumerable<string> PrimaryDiagnosisICDCode { get; set; }
        public IEnumerable<string> DiagnosisCode2ICDCode { get; set; }
        public IEnumerable<string> DiagnosisCode3ICDCode { get; set; }
        public IEnumerable<string> DiagnosisCode4ICDCode { get; set; }
        public IEnumerable<string> DiagnosisCode5ICDCode { get; set; }
        public IEnumerable<string> DiagnosisCode6ICDCode { get; set; }
        public IEnumerable<string> DiagnosisCode7ICDCode { get; set; }
        public IEnumerable<string> DiagnosisCode8ICDCode { get; set; }

        public IEnumerable<ClaimsProcessClaimCodeCondition> PrimaryDiagnosisCodeCondition { get; set; }
        public IEnumerable<ClaimsProcessClaimCodeCondition> DiagnosisCode2Condition { get; set; }
        public IEnumerable<ClaimsProcessClaimCodeCondition> DiagnosisCode3Condition { get; set; }
        public IEnumerable<ClaimsProcessClaimCodeCondition> DiagnosisCode4Condition { get; set; }
        public IEnumerable<ClaimsProcessClaimCodeCondition> DiagnosisCode5Condition { get; set; }
        public IEnumerable<ClaimsProcessClaimCodeCondition> DiagnosisCode6Condition { get; set; }
        public IEnumerable<ClaimsProcessClaimCodeCondition> DiagnosisCode7Condition { get; set; }
        public IEnumerable<ClaimsProcessClaimCodeCondition> DiagnosisCode8Condition { get; set; }

        public string DrugCategory { get; set; }

        public string CodeFlag { get; set; }

        public string EnrollType { get; set; }

        public bool HasHRA { get; set; }

        public decimal? BilledAmount { get; set; }//db type is money
        public decimal? Copay { get; set; }//db type is money
        public decimal? Deductible { get; set; }//db type is money
        public decimal? Coinsurance { get; set; }//db type is money
        public decimal? NetPaid { get; set; }//db type is money
        //Drug Name
        public string GenericDrug { get; set; }
        public string TheraClassCode { get; set; }
        public decimal? TotalAmountPaidbyAllSource { get; set; }//db type is money//TotalAmountPaid 
        public decimal? PatientPayAmount { get; set; }//db type is money //Member_Amount_Paid
        public decimal? AmountofCopay { get; set; }//db type is money
        public decimal? AmountofCoinsurance { get; set; }//db type is money
        public decimal? NetAmountDue { get; set; }//db type is money,(Total Amount Billed - Paid), db col name is Net Amount Due(Total Amount Billed - Paid)//AmountBilled
        // [IFFRef] [int] IDENTITY(1,1) NOT NULL,
        public bool IsSpouse { get { return RelationshipToSubscriberCode != null && RelationshipToSubscriberCode == ClaimsRelationshipToSubscriberCode.SPOUSE; } }

        public string SpouseSSN { get; set; }
        public string UniqueId { get; set; }

        public string TotalAmountPaidbyAllSourceX { get; set; } //db column is varchar 8
        public string PatientPayAmountX { get; set; } //db column is varchar 8
        public string AmountofCopayX { get; set; } //db column is varchar 8
        public string AmountofCoinsuranceX { get; set; } //db column is varchar 8
        public string NetAmountDueX { get; set; } //db column is varchar 8, db column is NetAmountDueX(Total Amount Billed - Paid)

        // public DateTime? DOBDate { get { return String.IsNullOrEmpty(MemberDateOfBirth) ? default(DateTime?) : DateTime.ParseExact(MemberDateOfBirth, "yyyyMMdd", CultureInfo.InvariantCulture); } }

        public ClaimsRelationshipToSubscriberCode? RelationshipToSubscriberCode { get; set; }

        #region Livongo
        public string LvDiagnosisCode1 { get; set; }
        public string LvDiagnosisCode2 { get; set; }
        public string LvDiagnosisCode3 { get; set; }
        public string LvDiagnosisCode4 { get; set; }
        public string LvDiagnosisCode5 { get; set; }
        public string LvDiagnosisCode6 { get; set; }
        public string LvDiagnosisCode7 { get; set; }
        public string LvDiagnosisCode8 { get; set; }
        public string LvDiagnosisCode9 { get; set; }
        public string LvDiagnosisCode10 { get; set; }

        public string LvNDCCode { get; set; }

        public string Diagnosis1 { get; set; }
        public string Diagnosis2 { get; set; }
        public string Diagnosis3 { get; set; }

        public string NDCDrugId { get; set; }
        public string ClaimId { get; set; }

        public string GroupId { get; set; }

        public string MemberNumber { get; set; }

        public ClaimsProcessGender MemberGender { get; set; }

        public DateTime? DateOfService { get; set; }

        public DateTime? PaidDate { get; set; }
        public DateTime? CoverageStartDate { get; set; }

        public DateTime? CoverageEndDate { get; set; }

        public string TypeOfCoverage { get; set; }

        public string DrugStrength { get; set; }

        public string DrugName { get; set; }

        public decimal? TotalDaysSupply { get; set; }

        public decimal? TotalDispensedQuantity { get; set; }

        public decimal? MemberCost { get; set; }

        public string OrgName { get; set; }

        public string ProcedureCode { get; set; }
        public string ProcedureCode1 { get; set; }
        public string RevenueCode { get; set; }
        // public string AmountBilled { get; set; }
        //  public string TotalAmountPaid { get; set; }
        //   public string MemberAmountPaid { get; set; }

        public string MemberPhone { get; set; }
        public string MemberEmail { get; set; }
        public string MemberAddress1 { get; set; }
        public string MemberAddress2 { get; set; }
        public string MemberCity { get; set; }
        public string MemberState { get; set; }
        public string MemberZip { get; set; }

        public DateTime? MedPlanTerminationDate { get; set; }
        public string TerminatedStatus { get; set; }
        public string LastDateProcessed { get; set; }
        public string LcoRef { get; set; }
        public string BusinessUnit { get; set; }

        public bool IncludeInLivongoOutput { get; set; }
        #endregion



        public bool IsPregnant
        {
            get
            {
                return HasCondition(ClaimsProcessClaimCodeCondition.PREG);
            }
        }

        public bool IsObese
        {
            get
            {
                return HasCondition(ClaimsProcessClaimCodeCondition.OBESE);
            }
        }

        public bool IsSmoking
        {
            get
            {
                return HasCondition(ClaimsProcessClaimCodeCondition.SMOKE);
            }
        }

        public bool HasLungDisorder
        {
            get
            {
                return HasCondition(ClaimsProcessClaimCodeCondition.LUNG);
            }
        }
        public bool IsDiabetic
        {
            get
            {
                return HasCondition(ClaimsProcessClaimCodeCondition.DIAB);
            }
        }

        public bool HasHeartDisorder
        {
            get
            {
                return HasCondition(ClaimsProcessClaimCodeCondition.CV);
            }
        }

        public bool HasSleepDisorder
        {
            get
            {
                return HasCondition(ClaimsProcessClaimCodeCondition.SLEEP);
            }
        }

        public bool HasHypertension
        {
            get
            {
                return HasCondition(ClaimsProcessClaimCodeCondition.HYPTEN);
            }
        }

        public bool HasCKD
        {
            get
            {
                return HasCondition(ClaimsProcessClaimCodeCondition.CKD);
            }
        }

        public bool HasPrediabetes
        {
            get
            {
                return HasCondition(ClaimsProcessClaimCodeCondition.PREDIAB);
            }
        }

        public bool NoIVClaimCondition
        {
            get
            {
                return HasCondition(ClaimsProcessClaimCodeCondition.OTHER);
            }
        }


        bool HasCondition(ClaimsProcessClaimCodeCondition condition)
        {
            if (PrimaryDiagnosisCodeCondition.Contains(condition)
             || DiagnosisCode2Condition.Contains(condition)
             || DiagnosisCode3Condition.Contains(condition)
             || DiagnosisCode4Condition.Contains(condition)
             || DiagnosisCode5Condition.Contains(condition)
             || DiagnosisCode6Condition.Contains(condition)
             || DiagnosisCode7Condition.Contains(condition)
             || DiagnosisCode8Condition.Contains(condition))
                return true;
            return false;
        }

        public int PortalId { get; set; }

        public int OrgId { get; set; }

        public bool SkipUniqueId { get; set; }

        public string SkipReason { get; set; }

        public void SetUniqueId(IDictionary<string, ClaimProcessEligibilityDto> eligibilities, IDictionary<string, ClaimProcessEligibilityDto> eligibilitiesUniqueId, out string extendedLog)
        {
            extendedLog = "";
            if (ClaimProviderDto.CVS_CAREMARK.Name != ProviderName)
            {
                ClaimProcessEligibilityDto eligibility = null;
                string userEnrollmentType = (ClaimsRelationshipToSubscriberCode.SPOUSE == RelationshipToSubscriberCode) ? "S" : (ClaimsRelationshipToSubscriberCode.CHILDREN == RelationshipToSubscriberCode ? "C" : "E");
                if (eligibilities.TryGetValue(MemberSSN + userEnrollmentType, out eligibility))
                {
                    #region First Pass
                    if (eligibility.FirstName == MemberFirstName && PortalId == eligibility.PortalId)
                    {
                        UniqueId = eligibility.UniqueID;
                        PortalId = eligibility.PortalId;
                        OrgId = eligibility.OrgId;
                        return;
                    }
                    else
                    {
                        extendedLog += "SSN matched. Could not match by First Name.";
                    }
                    #endregion

                    #region Second pass
                    if (this.MemberDateOfBirth.HasValue && eligibility.DateOfBirth.HasValue)
                    {
                        if (MemberDateOfBirth.Value == eligibility.DateOfBirth.Value && PortalId == eligibility.PortalId)
                        {
                            UniqueId = eligibility.UniqueID;
                            PortalId = eligibility.PortalId;
                            OrgId = eligibility.OrgId;
                            return;
                        }
                        else
                        {
                            extendedLog += "SSN matched. Could not match by DOB.";
                        }
                    }
                    #endregion
                }
                else
                {
                    extendedLog += "SSN did not match.";
                    //sometime spouse record cames with the primary's first name or DOB
                }
                if (String.IsNullOrEmpty(UniqueId) && IsSpouse)
                {
                    extendedLog += "Spouse record.";
                    #region Third Pass
                    //get primary elig record
                    if (eligibilities.TryGetValue(MemberSSN + "E", out eligibility))
                    {
                        extendedLog += "Matched primary record.";
                        var spouseUniqueId = eligibility.UniqueID.Replace("EH9", "EH8");
                        ClaimProcessEligibilityDto eligibilitySpouse = null;
                        if (eligibilitiesUniqueId.TryGetValue(spouseUniqueId, out eligibilitySpouse))
                        {
                            if ((eligibilitySpouse.FirstName == MemberFirstName || (eligibilitySpouse.DateOfBirth.HasValue && MemberDateOfBirth.HasValue && eligibilitySpouse.DateOfBirth.Value == MemberDateOfBirth.Value)) && PortalId == eligibility.PortalId)
                            {
                                UniqueId = eligibilitySpouse.UniqueID;
                                MemberSSN = eligibilitySpouse.SSN;
                                PortalId = eligibilitySpouse.PortalId;
                                OrgId = eligibilitySpouse.OrgId;
                                return;
                            }
                            else
                            {
                                extendedLog += $"Name or DOB did not match.";
                            }
                        }
                        else
                        {
                            extendedLog += $"Could not find a match with spouse unique id {spouseUniqueId}.";
                        }
                    }
                    #endregion
                }
            }
        }

        public void CrothalIdChanges(IDictionary<string, CrothalIdDto> crothalIDs)
        {
            //only for cvs
            if (ProviderName == ClaimProviderDto.CVS_CAREMARK.Name)
            {
                CrothalIdDto newId = null;
                if (crothalIDs.TryGetValue(UniqueId, out newId))
                {
                    if (MemberFirstName == newId.FirstName || MemberLastName == newId.LastName)
                    {
                        UniqueId = newId.NewUniqueId;//replace old with new
                    }
                }
            }
        }

        public void SetSSN(IDictionary<string, ClaimProcessEligibilityDto> eligibilities, IDictionary<string, ClaimProcessEligibilityDto> childrenEligibilities)
        {
            ClaimProcessEligibilityDto eligibility = null;
            if (eligibilities.TryGetValue(UniqueId, out eligibility))
            {
                #region First pass
                if (MemberFirstName == eligibility.FirstName && PortalId == eligibility.PortalId)
                {
                    MemberSSN = eligibility.SSN;
                    PortalId = eligibility.PortalId;
                    OrgId = eligibility.OrgId;
                    return;
                }
                #endregion

                #region Second pass
                if (MemberDateOfBirth.HasValue && eligibility.DateOfBirth.HasValue && MemberDateOfBirth.Value == eligibility.DateOfBirth.Value)
                {
                    MemberSSN = eligibility.SSN;
                    PortalId = eligibility.PortalId;
                    OrgId = eligibility.OrgId;
                    return;
                }
                #endregion

            }
            var isChild = (RelationshipToSubscriberCode.HasValue && RelationshipToSubscriberCode.Value == ClaimsRelationshipToSubscriberCode.CHILDREN) ? true : false;
            if (eligibility == null && isChild)
            {
                if (childrenEligibilities.TryGetValue(UniqueId + MemberFirstName, out eligibility))
                {
                    #region First pass
                    if (MemberDateOfBirth.HasValue && eligibility.DateOfBirth.HasValue && MemberDateOfBirth.Value == eligibility.DateOfBirth.Value)
                    {
                        MemberSSN = eligibility.SSN;
                        PortalId = eligibility.PortalId;
                        OrgId = eligibility.OrgId;
                        return;
                    }
                    #endregion

                }
            }
            else
            {
                SkipUniqueId = true;
                //clear unique id to be skipped
                SkipReason = "NOT FOUND IN ELIGIBILITY";
            }

        }

        public void SetDrugCategory(IDictionary<string, ClaimProcessTherapeuticClassCodeDto> theraClassCodes)
        {
            if (!String.IsNullOrEmpty(GenericDrug) && !String.IsNullOrEmpty(TheraClassCode))
            {
                ClaimProcessTherapeuticClassCodeDto category = null;
                if (theraClassCodes.TryGetValue(TheraClassCode, out category))
                {
                    DrugCategory = category.DrugCategory;
                }
            }
        }

        Dictionary<ClaimsProcessClaimCodeCondition, string> CheckInterventConditions(string diagnosisCode, ILookup<string, ClaimCodeDto> ClaimCodes)
        {
            Dictionary<ClaimsProcessClaimCodeCondition, string> conditions = new Dictionary<ClaimsProcessClaimCodeCondition, string>();
            if (!String.IsNullOrEmpty(diagnosisCode))
            {
                if (ClaimCodes.Contains(diagnosisCode))
                {
                    foreach (var rec in ClaimCodes[diagnosisCode])
                    {
                        conditions.Add((ClaimsProcessClaimCodeCondition)Enum.Parse(typeof(ClaimsProcessClaimCodeCondition), rec.CodeFlag), rec.CodeFlag + ":" + rec.Code + ":" + rec.CodeDescription);
                    }
                }
            }
            return conditions;
        }

        public void SetClaimCodeCondition(ILookup<string, ClaimCodeDto> claimCodes)
        {

            var PrimaryDiagnosis = CheckInterventConditions(PrimaryDiagnosisCode, claimCodes);
            var Diagnosis2 = CheckInterventConditions(DiagnosisCode2, claimCodes);
            var Diagnosis3 = CheckInterventConditions(DiagnosisCode3, claimCodes);
            var Diagnosis4 = CheckInterventConditions(DiagnosisCode4, claimCodes);
            var Diagnosis5 = CheckInterventConditions(DiagnosisCode5, claimCodes);
            var Diagnosis6 = CheckInterventConditions(DiagnosisCode6, claimCodes);
            var Diagnosis7 = CheckInterventConditions(DiagnosisCode7, claimCodes);
            var Diagnosis8 = CheckInterventConditions(DiagnosisCode8, claimCodes);

            PrimaryDiagnosisCodeCondition = PrimaryDiagnosis.Keys;
            DiagnosisCode2Condition = Diagnosis2.Keys;
            DiagnosisCode3Condition = Diagnosis3.Keys;
            DiagnosisCode4Condition = Diagnosis4.Keys;
            DiagnosisCode5Condition = Diagnosis5.Keys;
            DiagnosisCode6Condition = Diagnosis6.Keys;
            DiagnosisCode7Condition = Diagnosis7.Keys;
            DiagnosisCode8Condition = Diagnosis8.Keys;

            PrimaryDiagnosisICDCode = PrimaryDiagnosis.Values;
            DiagnosisCode2ICDCode = Diagnosis2.Values;
            DiagnosisCode3ICDCode = Diagnosis3.Values;
            DiagnosisCode4ICDCode = Diagnosis4.Values;
            DiagnosisCode5ICDCode = Diagnosis5.Values;
            DiagnosisCode6ICDCode = Diagnosis6.Values;
            DiagnosisCode7ICDCode = Diagnosis7.Values;
            DiagnosisCode8ICDCode = Diagnosis8.Values;

            if (DiagnosisCode8Condition.Count() > 0)
            {
                foreach (var diagCode8 in claimCodes[DiagnosisCode8])
                {
                    //used in candidate condition
                    if (diagCode8.CodeSource.StartsWith("CPT") || diagCode8.CodeSource.StartsWith("ICD9V3") || diagCode8.CodeSource.StartsWith("ICD10PCS"))
                    {
                        CodeFlag = diagCode8.CodeFlag;
                    }
                }
            }
            var conditions = new List<ClaimsProcessClaimCodeCondition>();
            conditions.Add(ClaimsProcessClaimCodeCondition.OTHER);
            if (PrimaryDiagnosisCodeCondition.Count() == 0 && !String.IsNullOrEmpty(PrimaryDiagnosisCode))
            {
                PrimaryDiagnosisCodeCondition = conditions;
                var codes = new List<string>();
                codes.Add("OTHER:" + PrimaryDiagnosisCode + ":N/A");
                PrimaryDiagnosisICDCode = codes;
            }
            if (DiagnosisCode2Condition.Count() == 0 && !String.IsNullOrEmpty(DiagnosisCode2))
            {
                DiagnosisCode2Condition = conditions;
                var codes = new List<string>();
                codes.Add("OTHER:" + DiagnosisCode2 + ":N/A");
                DiagnosisCode2ICDCode = codes;
            }
            if (DiagnosisCode3Condition.Count() == 0 && !String.IsNullOrEmpty(DiagnosisCode3))
            {
                DiagnosisCode3Condition = conditions;
                var codes = new List<string>();
                codes.Add("OTHER:" + DiagnosisCode3 + ":N/A");
                DiagnosisCode3ICDCode = codes;
            }
            if (DiagnosisCode4Condition.Count() == 0 && !String.IsNullOrEmpty(DiagnosisCode4))
            {
                DiagnosisCode4Condition = conditions;
                var codes = new List<string>();
                codes.Add("OTHER:" + DiagnosisCode4 + ":N/A");
                DiagnosisCode4ICDCode = codes;
            }
            if (DiagnosisCode5Condition.Count() == 0 && !String.IsNullOrEmpty(DiagnosisCode5))
            {
                DiagnosisCode5Condition = conditions;
                var codes = new List<string>();
                codes.Add("OTHER:" + DiagnosisCode5 + ":N/A");
                DiagnosisCode5ICDCode = codes;
            }
            if (DiagnosisCode6Condition.Count() == 0 && !String.IsNullOrEmpty(DiagnosisCode6))
            {
                DiagnosisCode6Condition = conditions;
                var codes = new List<string>();
                codes.Add("OTHER:" + DiagnosisCode6 + ":N/A");
                DiagnosisCode6ICDCode = codes;
            }
            if (DiagnosisCode7Condition.Count() == 0 && !String.IsNullOrEmpty(DiagnosisCode7))
            {
                DiagnosisCode7Condition = conditions;
                var codes = new List<string>();
                codes.Add("OTHER:" + DiagnosisCode7 + ":N/A");
                DiagnosisCode7ICDCode = codes;
            }
            if (DiagnosisCode8Condition.Count() == 0 && !String.IsNullOrEmpty(DiagnosisCode8))
            {
                DiagnosisCode8Condition = conditions;
                var codes = new List<string>();
                codes.Add("OTHER:" + DiagnosisCode8 + ":N/A");
                DiagnosisCode8ICDCode = codes;
            }

            //if (PrimaryDiagnosisCodeCondition.Count() == 0 && DiagnosisCode2Condition.Count() == 0 && DiagnosisCode3Condition.Count() == 0 && DiagnosisCode4Condition.Count() == 0
            //    && DiagnosisCode5Condition.Count() == 0 && DiagnosisCode6Condition.Count() == 0 && DiagnosisCode7Condition.Count() == 0 && DiagnosisCode8Condition.Count() == 0)
            //{
            //    List<ClaimsProcessClaimCodeCondition> conditions = new List<ClaimsProcessClaimCodeCondition>();
            //    conditions.Add(ClaimsProcessClaimCodeCondition.OTHER);
            //    PrimaryDiagnosisCodeCondition = conditions;
            //}
        }

        public void SetEnrolledDataFields(IEnumerable<ClaimProcessEnrolledDataDto> enrolledData, IEnumerable<ClaimProcessHRADto> hRAEnrolled)
        {
            var rec = enrolledData.FirstOrDefault(x => x.UniqueId == UniqueId && x.PortalId == PortalId);
            if (rec != null)
            {
                EnrollType = rec.EnrollType;
            }
            var hraRec = hRAEnrolled.FirstOrDefault(x => x.UniqueId == UniqueId && x.PortalId == PortalId);
            if (hraRec != null)
            {
                HasHRA = true;
            }
        }

        List<String> livongoMedicalPlanCodesNotInList = new List<string>() { "1BAL", "2BAL", "3HCN", "1HUP", "3HAP", "3HCL" };
        public void SetLivongoFields(IDictionary<string, LivongoICDCodesDto> livongoICDCodes, IDictionary<string, LivongoNDCCodesDto> livongoNDCCodes, IDictionary<string, ClaimProcessEligibilityDto> eligibilitieswithSSN, IDictionary<string, ClaimProcessEligibilityDto> eligibilitieswithUniqueId, IDictionary<string, ClaimProcessEligibilityDto> childreneligibilitieswithUniqueId)
        {
            List<string> codes = new List<string>();
            string userEnrollmentType = (ClaimsRelationshipToSubscriberCode.SPOUSE == RelationshipToSubscriberCode) ? "S" : (ClaimsRelationshipToSubscriberCode.CHILDREN == RelationshipToSubscriberCode ? "C" : "E");
            if (this.ProviderName == ClaimProviderDto.BCBS.Name || this.ProviderName == ClaimProviderDto.UHC.Name)
            {
                //var elig = eligibility.FirstOrDefault(x => x.SSN == MemberSSN && PortalIds.Contains(x.PortalId) && !livongoMedicalPlanCodesNotInList.Contains(x.MedicalPlanCode) && !x.MedicalPlanCode.StartsWith("H") && x.UserStatus != "T");
                //if (elig == null)
                //    return;
                var haseligibility = false;
                ClaimProcessEligibilityDto elig = null;
                if (eligibilitieswithSSN.TryGetValue(MemberSSN + userEnrollmentType, out elig))
                {
                    haseligibility = true;
                }
                else if (eligibilitieswithUniqueId.TryGetValue(UniqueId, out elig))
                {
                    haseligibility = true;
                }
                else if (childreneligibilitieswithUniqueId.TryGetValue(UniqueId, out elig))
                {
                    haseligibility = true;
                }
                if (haseligibility)
                {
                    if (PortalId == elig.PortalId && !livongoMedicalPlanCodesNotInList.Contains(elig.MedicalPlanCode) && !elig.MedicalPlanCode.StartsWith("H") && elig.UserStatus != "T")
                    {
                        //check if Diagnosis 1, 2 or 3 is a Livongo code

                        if (!String.IsNullOrEmpty(Diagnosis1))
                        {
                            LivongoICDCodesDto lvCode1 = null;
                            if (!livongoICDCodes.TryGetValue(Diagnosis1, out lvCode1))
                            {
                                Diagnosis1 = null;
                            }
                            else
                            {
                                codes.Add(lvCode1.Code + ":" + lvCode1.CodeDescription);
                            }
                        }

                        if (!String.IsNullOrEmpty(Diagnosis2))
                        {
                            LivongoICDCodesDto lvCode2 = null;
                            if (!livongoICDCodes.TryGetValue(Diagnosis2, out lvCode2))
                            {
                                Diagnosis2 = null;
                            }
                            else
                            {
                                codes.Add(lvCode2.Code + ":" + lvCode2.CodeDescription);
                            }
                        }

                        if (!String.IsNullOrEmpty(Diagnosis3))
                        {
                            LivongoICDCodesDto lvCode3 = null;
                            if (!livongoICDCodes.TryGetValue(Diagnosis3, out lvCode3))
                            {
                                Diagnosis3 = null;
                            }
                            else
                            {
                                codes.Add(lvCode3.Code + ":" + lvCode3.CodeDescription);
                            }
                        }
                        if (!String.IsNullOrEmpty(Diagnosis1) || !String.IsNullOrEmpty(Diagnosis2) || !String.IsNullOrEmpty(Diagnosis3))
                        {
                            OrgName = elig.OrgName;
                            MemberPhone = elig.HomeNumber;
                            MemberEmail = elig.Email;
                            MemberAddress1 = elig.Address;
                            MemberAddress2 = elig.Address2;
                            MemberCity = elig.City;
                            MemberState = elig.State;
                            MemberZip = elig.Zip;
                            BusinessUnit = elig.BusinessUnit;
                            MedPlanTerminationDate = elig.MedicalPlanEndDate;
                            TerminatedStatus = elig.UserStatus;
                            IncludeInLivongoOutput = true;
                            PrimaryDiagnosisICDCode = codes;
                        }
                    }
                }

            }
            else if (this.ProviderName == ClaimProviderDto.CVS_CAREMARK.Name)
            {
                var haseligibility = false;
                ClaimProcessEligibilityDto elig = null;
                if (eligibilitieswithSSN.TryGetValue(MemberSSN + userEnrollmentType, out elig))
                {
                    haseligibility = true;
                }
                else if (eligibilitieswithUniqueId.TryGetValue(UniqueId, out elig))
                {
                    haseligibility = true;
                }
                else if (childreneligibilitieswithUniqueId.TryGetValue(UniqueId, out elig))
                {
                    haseligibility = true;
                }
                if (haseligibility)
                {
                    if (PortalId == elig.PortalId && !livongoMedicalPlanCodesNotInList.Contains(elig.MedicalPlanCode) && !elig.MedicalPlanCode.StartsWith("H") && elig.UserStatus != "T")
                    {
                        //check if Diagnosis 1 is a Livongo ICD or NDC code

                        if (!String.IsNullOrEmpty(Diagnosis1))
                        {
                            LivongoICDCodesDto lvCode1 = null;
                            if (!livongoICDCodes.TryGetValue(Diagnosis1, out lvCode1))
                            {
                                Diagnosis1 = null;
                            }
                            else
                            {
                                codes.Add(lvCode1.Code + ":" + lvCode1.CodeDescription);
                            }
                        }
                        if (!String.IsNullOrEmpty(NDCDrugId))
                        {
                            LivongoNDCCodesDto lvNDCCode = null;
                            if (!livongoNDCCodes.TryGetValue(NDCDrugId, out lvNDCCode))
                            {
                                NDCDrugId = null;
                            }
                            else
                            {
                                codes.Add(lvNDCCode.Code);
                            }
                        }
                        if (!String.IsNullOrEmpty(Diagnosis1) || !String.IsNullOrEmpty(NDCDrugId))
                        {
                            OrgName = elig.OrgName;
                            MemberPhone = elig.HomeNumber;
                            MemberEmail = elig.Email;
                            MemberAddress1 = elig.Address;
                            MemberAddress2 = elig.Address2;
                            MemberCity = elig.City;
                            MemberState = elig.State;
                            MemberZip = elig.Zip;
                            BusinessUnit = elig.BusinessUnit;
                            MedPlanTerminationDate = elig.MedicalPlanEndDate;
                            TerminatedStatus = elig.UserStatus;
                            IncludeInLivongoOutput = true;
                            PrimaryDiagnosisICDCode = codes;
                        }
                    }
                }
            }
            else if (this.ProviderName == ClaimProviderDto.AETNA.Name)
            {
                var haseligibility = false;
                ClaimProcessEligibilityDto elig = null;
                if (eligibilitieswithSSN.TryGetValue(MemberSSN + userEnrollmentType, out elig))
                {
                    haseligibility = true;
                }
                else if (eligibilitieswithUniqueId.TryGetValue(UniqueId, out elig))
                {
                    haseligibility = true;
                }
                else if (childreneligibilitieswithUniqueId.TryGetValue(UniqueId, out elig))
                {
                    haseligibility = true;
                }
                if (haseligibility)
                {

                    if (PortalId == elig.PortalId && !livongoMedicalPlanCodesNotInList.Contains(elig.MedicalPlanCode) && !elig.MedicalPlanCode.StartsWith("H") && elig.UserStatus != "T")
                    {
                        //check if Diagnosis 1, 2 or 3 is a Livongo code

                        if (!String.IsNullOrEmpty(LvDiagnosisCode1))
                        {
                            LivongoICDCodesDto lvCode1 = null;
                            if (!livongoICDCodes.TryGetValue(LvDiagnosisCode1, out lvCode1))
                            {
                                LvDiagnosisCode1 = null;
                            }
                            else
                            {
                                codes.Add(lvCode1.Code + ":" + lvCode1.CodeDescription);
                            }
                        }

                        if (!String.IsNullOrEmpty(LvDiagnosisCode2))
                        {
                            LivongoICDCodesDto lvCode2 = null;
                            if (!livongoICDCodes.TryGetValue(LvDiagnosisCode2, out lvCode2))
                            {
                                LvDiagnosisCode2 = null;
                            }
                            else
                            {
                                codes.Add(lvCode2.Code + ":" + lvCode2.CodeDescription);
                            }
                        }

                        if (!String.IsNullOrEmpty(LvDiagnosisCode3))
                        {
                            LivongoICDCodesDto lvCode3 = null;
                            if (!livongoICDCodes.TryGetValue(LvDiagnosisCode3, out lvCode3))
                            {
                                LvDiagnosisCode3 = null;
                            }
                            else
                            {
                                codes.Add(lvCode3.Code + ":" + lvCode3.CodeDescription);
                            }
                        }
                        if (!String.IsNullOrEmpty(LvDiagnosisCode4))
                        {
                            LivongoICDCodesDto lvCode4 = null;
                            if (!livongoICDCodes.TryGetValue(LvDiagnosisCode4, out lvCode4))
                            {
                                LvDiagnosisCode4 = null;
                            }
                            else
                            {
                                codes.Add(lvCode4.Code + ":" + lvCode4.CodeDescription);
                            }
                        }

                        if (!String.IsNullOrEmpty(LvDiagnosisCode5))
                        {
                            LivongoICDCodesDto lvCode5 = null;
                            if (!livongoICDCodes.TryGetValue(LvDiagnosisCode5, out lvCode5))
                            {
                                LvDiagnosisCode5 = null;
                            }
                            else
                            {
                                codes.Add(lvCode5.Code + ":" + lvCode5.CodeDescription);
                            }
                        }

                        if (!String.IsNullOrEmpty(LvDiagnosisCode6))
                        {
                            LivongoICDCodesDto lvCode6 = null;
                            if (!livongoICDCodes.TryGetValue(LvDiagnosisCode6, out lvCode6))
                            {
                                LvDiagnosisCode6 = null;
                            }
                            else
                            {
                                codes.Add(lvCode6.Code + ":" + lvCode6.CodeDescription);
                            }
                        }
                        if (!String.IsNullOrEmpty(LvDiagnosisCode7))
                        {
                            LivongoICDCodesDto lvCode7 = null;
                            if (!livongoICDCodes.TryGetValue(LvDiagnosisCode7, out lvCode7))
                            {
                                LvDiagnosisCode7 = null;
                            }
                            else
                            {
                                codes.Add(lvCode7.Code + ":" + lvCode7.CodeDescription);
                            }
                        }

                        if (!String.IsNullOrEmpty(LvDiagnosisCode8))
                        {
                            LivongoICDCodesDto lvCode8 = null;
                            if (!livongoICDCodes.TryGetValue(LvDiagnosisCode8, out lvCode8))
                            {
                                LvDiagnosisCode8 = null;
                            }
                            else
                            {
                                codes.Add(lvCode8.Code + ":" + lvCode8.CodeDescription);
                            }
                        }

                        if (!String.IsNullOrEmpty(LvDiagnosisCode9))
                        {
                            LivongoICDCodesDto lvCode9 = null;
                            if (!livongoICDCodes.TryGetValue(LvDiagnosisCode9, out lvCode9))
                            {
                                LvDiagnosisCode9 = null;
                            }
                            else
                            {
                                codes.Add(lvCode9.Code + ":" + lvCode9.CodeDescription);
                            }
                        }
                        if (!String.IsNullOrEmpty(LvDiagnosisCode10))
                        {
                            LivongoICDCodesDto lvCode10 = null;
                            if (!livongoICDCodes.TryGetValue(LvDiagnosisCode10, out lvCode10))
                            {
                                LvDiagnosisCode10 = null;
                            }
                            else
                            {
                                codes.Add(lvCode10.Code + ":" + lvCode10.CodeDescription);
                            }
                        }

                        if (!String.IsNullOrEmpty(LvDiagnosisCode1) || !String.IsNullOrEmpty(LvDiagnosisCode2) || !String.IsNullOrEmpty(LvDiagnosisCode3)
                            || !String.IsNullOrEmpty(LvDiagnosisCode4) || !String.IsNullOrEmpty(LvDiagnosisCode5) || !String.IsNullOrEmpty(LvDiagnosisCode6)
                            || !String.IsNullOrEmpty(LvDiagnosisCode7) || !String.IsNullOrEmpty(LvDiagnosisCode8) || !String.IsNullOrEmpty(LvDiagnosisCode9)
                             || !String.IsNullOrEmpty(LvDiagnosisCode10))
                        {
                            OrgName = elig.OrgName;
                            MemberPhone = elig.HomeNumber;
                            MemberEmail = elig.Email;
                            MemberAddress1 = elig.Address;
                            MemberAddress2 = elig.Address2;
                            MemberCity = elig.City;
                            MemberState = elig.State;
                            MemberZip = elig.Zip;
                            BusinessUnit = elig.BusinessUnit;
                            MedPlanTerminationDate = elig.MedicalPlanEndDate;
                            TerminatedStatus = elig.UserStatus;
                            IncludeInLivongoOutput = true;
                            PrimaryDiagnosisICDCode = codes;
                        }
                    }

                    //set diag1, diag2 and diag3 from livongo condition codes
                    //do we need to empty the diag conditions before setting it again?
                    LivongoDiagnosisMap(LvDiagnosisCode1);
                    LivongoDiagnosisMap(LvDiagnosisCode2);
                    LivongoDiagnosisMap(LvDiagnosisCode3);
                    LivongoDiagnosisMap(LvDiagnosisCode4);
                    LivongoDiagnosisMap(LvDiagnosisCode5);
                    LivongoDiagnosisMap(LvDiagnosisCode6);
                    LivongoDiagnosisMap(LvDiagnosisCode7);
                    LivongoDiagnosisMap(LvDiagnosisCode8);
                    LivongoDiagnosisMap(LvDiagnosisCode9);
                    LivongoDiagnosisMap(LvDiagnosisCode10);
                }
            }
        }

        void LivongoDiagnosisMap(string livongoCondition)
        {
            if (!String.IsNullOrEmpty(livongoCondition))
            {
                if (String.IsNullOrEmpty(Diagnosis1))
                    Diagnosis1 = livongoCondition;
                else
                {
                    if (String.IsNullOrEmpty(Diagnosis2))
                        Diagnosis2 = livongoCondition;
                    else
                        Diagnosis3 = livongoCondition;
                }
            }
        }
    }



    //choosing enums for writing the data
    public enum ClaimsRelationshipToSubscriberCode { EMPLOYEE, SPOUSE, CHILDREN }

    public enum ClaimsProcessGender { M, F, U }

    public enum ClaimsProcessClaimCodeCondition { OBESE, SMOKE, HYPTEN, LUNG, DIAB, PREG, CV, SLEEP, DIABLV, OTHER, CKD, PREDIAB }

}
