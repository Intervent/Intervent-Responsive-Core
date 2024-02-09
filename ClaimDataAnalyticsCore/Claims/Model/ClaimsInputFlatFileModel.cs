using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimDataAnalytics.Claims.Model
{
    public sealed class ClaimsInputFlatFileModel
    {
        public int InputFileRowId { get; set; }
        public string ProviderName { get; set; }
        public string MemberSSN { get; set; }
        public string SubscriberSSN { get; set; }
        public string MemberFirstName { get; set; }
        public string MemberLastName { get; set; }
        public DateTime? MemberDateOfBirth { get; set; }//change string to date
        public DateTime? ServiceStartDate { get; set; }//change string to date
        public DateTime? ServiceEndDate { get; set; }//change string to date
        //public string PrimaryDiagnosisCode { get; set; }
        //public string DiagnosisCode2 { get; set; }
        //public string DiagnosisCode3 { get; set; }
        //public string DiagnosisCode4 { get; set; }
        //public string DiagnosisCode5 { get; set; }
        //public string DiagnosisCode6 { get; set; }
        //public string DiagnosisCode7 { get; set; }
        //public string DiagnosisCode8 { get; set; }

        //public IEnumerable<string> PrimaryDiagnosisICDCode { get; set; }
        //public IEnumerable<string> DiagnosisCode2ICDCode { get; set; }
        //public IEnumerable<string> DiagnosisCode3ICDCode { get; set; }
        //public IEnumerable<string> DiagnosisCode4ICDCode { get; set; }
        //public IEnumerable<string> DiagnosisCode5ICDCode { get; set; }
        //public IEnumerable<string> DiagnosisCode6ICDCode { get; set; }
        //public IEnumerable<string> DiagnosisCode7ICDCode { get; set; }
        //public IEnumerable<string> DiagnosisCode8ICDCode { get; set; }

        //public IEnumerable<ClaimsProcessClaimCodeCondition> PrimaryDiagnosisCodeCondition { get; set; }
        //public IEnumerable<ClaimsProcessClaimCodeCondition> DiagnosisCode2Condition { get; set; }
        //public IEnumerable<ClaimsProcessClaimCodeCondition> DiagnosisCode3Condition { get; set; }
        //public IEnumerable<ClaimsProcessClaimCodeCondition> DiagnosisCode4Condition { get; set; }
        //public IEnumerable<ClaimsProcessClaimCodeCondition> DiagnosisCode5Condition { get; set; }
        //public IEnumerable<ClaimsProcessClaimCodeCondition> DiagnosisCode6Condition { get; set; }
        //public IEnumerable<ClaimsProcessClaimCodeCondition> DiagnosisCode7Condition { get; set; }
        //public IEnumerable<ClaimsProcessClaimCodeCondition> DiagnosisCode8Condition { get; set; }

        //public string DrugCategory { get; set; }

        //public string CodeFlag { get; set; }

        public string EnrollType { get; set; }

        //public bool HasHRA { get; set; }

        public decimal? BilledAmount { get; set; }//db type is money
        public decimal? AllowedAmount { get; set; }//db type is money
        public decimal? Copay { get; set; }//db type is money
        public decimal? Deductible { get; set; }//db type is money
        public decimal? Coinsurance { get; set; }//db type is money
        public decimal? NetPaid { get; set; }//db type is money
        //Drug Name
        //public string GenericDrug { get; set; }
        //public string TheraClassCode { get; set; }
        public decimal? TotalAmountPaidbyAllSource { get; set; }//db type is money//TotalAmountPaid 
        public decimal? PatientPayAmount { get; set; }//db type is money //Member_Amount_Paid
        public decimal? AmountofCopay { get; set; }//db type is money
        public decimal? AmountofCoinsurance { get; set; }//db type is money
        public decimal? NetAmountDue { get; set; }//db type is money,(Total Amount Billed - Paid), db col name is Net Amount Due(Total Amount Billed - Paid)//AmountBilled
        public bool IsSpouse { get { return RelationshipToSubscriberCode != null && RelationshipToSubscriberCode == ClaimsRelationshipToSubscriberCode.SPOUSE; } }

        public string SpouseSSN { get; set; }
        public string UniqueId { get; set; }

        public string TotalAmountPaidbyAllSourceX { get; set; } //db column is varchar 8
        public string PatientPayAmountX { get; set; } //db column is varchar 8
        public string AmountofCopayX { get; set; } //db column is varchar 8
        public string AmountofCoinsuranceX { get; set; } //db column is varchar 8
        public string NetAmountDueX { get; set; } //db column is varchar 8, db column is NetAmountDueX(Total Amount Billed - Paid)


        public ClaimsRelationshipToSubscriberCode? RelationshipToSubscriberCode { get; set; }

        public string OrgName { get; set; }

        public DateTime CreateDate { get; set; }
        public string MemberNumber { get; set; }

        public ClaimsProcessGender MemberGender { get; set; }

        public DateTime? DateOfService { get; set; }

        public int? EligibilityId { get; set; }

        public string NewUniqueId { get; set; }



        public void SetUniqueId(IDictionary<string, ClaimProcessEligibilityDto> eligibilities, IDictionary<string, ClaimProcessEligibilityDto> eligibilitiesUniqueId)
        {
            if (ClaimProviderDto.CVS_CAREMARK.Name != ProviderName)
            {
                ClaimProcessEligibilityDto eligibility = null;
                string userEnrollmentType = (ClaimsRelationshipToSubscriberCode.SPOUSE == RelationshipToSubscriberCode) ? "S" : (ClaimsRelationshipToSubscriberCode.CHILDREN == RelationshipToSubscriberCode ? "C" : "E");
                //Console.WriteLine($"User enrollment type is {userEnrollmentType}");
                if (eligibilities.TryGetValue(MemberSSN + userEnrollmentType, out eligibility))
                {
                   // Console.WriteLine("came here");
                    #region First Pass
                    if (eligibility.FirstName == MemberFirstName)
                    {
                        NewUniqueId = eligibility.UniqueID;
                        EligibilityId = eligibility.EligibilityId;
                        return;
                    }
                    #endregion

                    #region Second pass
                    if (this.MemberDateOfBirth.HasValue && eligibility.DateOfBirth.HasValue)
                    {
                        if (MemberDateOfBirth.Value == eligibility.DateOfBirth.Value)
                        {
                            NewUniqueId = eligibility.UniqueID;
                            EligibilityId = eligibility.EligibilityId;
                            return;
                        }
                        else
                        {
                           // extendedLog += "SSN matched. Could not match by DOB.";
                        }
                    }
                    #endregion
                }
                else
                {
                    //extendedLog += "SSN did not match.";
                    //sometime spouse record cames with the primary's first name or DOB
                }
                if (String.IsNullOrEmpty(UniqueId) && IsSpouse)
                {
                    //extendedLog += "Spouse record.";
                    #region Third Pass
                    //get primary elig record
                    if (eligibilities.TryGetValue(MemberSSN + "E", out eligibility))
                    {
                       // extendedLog += "Matched primary record.";
                        var spouseUniqueId = eligibility.UniqueID.Replace("EH9", "EH8");
                        ClaimProcessEligibilityDto eligibilitySpouse = null;
                        if (eligibilitiesUniqueId.TryGetValue(spouseUniqueId, out eligibilitySpouse))
                        {
                            if ((eligibilitySpouse.FirstName == MemberFirstName || (eligibilitySpouse.DateOfBirth.HasValue && MemberDateOfBirth.HasValue && eligibilitySpouse.DateOfBirth.Value == MemberDateOfBirth.Value)))
                            {
                                NewUniqueId = eligibilitySpouse.UniqueID;
                                MemberSSN = eligibilitySpouse.SSN;
                                EligibilityId = eligibility.EligibilityId;
                                return;
                            }
                            else
                            {
                               // extendedLog += $"Name or DOB did not match.";
                            }
                        }
                        else
                        {
                           // extendedLog += $"Could not find a match with spouse unique id {spouseUniqueId}.";
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
                        NewUniqueId = newId.NewUniqueId;//replace old with new
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
                if (MemberFirstName == eligibility.FirstName)
                {
                    MemberSSN = eligibility.SSN;
                    EligibilityId = eligibility.EligibilityId;
                    return;
                }
                #endregion

                #region Second pass
                if (MemberDateOfBirth.HasValue && eligibility.DateOfBirth.HasValue && MemberDateOfBirth.Value == eligibility.DateOfBirth.Value)
                {
                    MemberSSN = eligibility.SSN;
                    EligibilityId = eligibility.EligibilityId;
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
                        EligibilityId = eligibility.EligibilityId;
                        return;
                    }
                    #endregion

                }
            }
            else
            {
                //SkipUniqueId = true;
                ////clear unique id to be skipped
                //SkipReason = "NOT FOUND IN ELIGIBILITY";
            }

        }

    }



    //choosing enums for writing the data
    public enum ClaimsRelationshipToSubscriberCode { EMPLOYEE, SPOUSE, CHILDREN }

    public enum ClaimsProcessGender { M, F, U }

    public enum ClaimsProcessClaimCodeCondition { OBESE, SMOKE, HYPTEN, LUNG, DIAB, PREG, CV, SLEEP, DIABLV, OTHER, CKD, PREDIAB }
}
