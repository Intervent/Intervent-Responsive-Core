namespace Intervent.Web.DTO
{
    public sealed class EbenFlatFileModel
    {
        public string UniqueId { get; set; }

        public bool HasHRA { get; set; }

        public int PortalId { get; set; }

        public string ClaimId { get; set; }

        public string GroupId { get; set; }

        public DateTime? MemberDateOfBirth { get; set; }

        public DateTime? ServiceStartDate { get; set; }

        public DateTime? DateOfService { get; set; }

        public string CodeFlag { get; set; }

        public string DiagnosisCode { get; set; }

        public IEnumerable<ClaimsProcessClaimCodeCondition> DiagnosisCodeCondition { get; set; }

        public IEnumerable<string> DiagnosisCodeList { get; set; }

        public string ProcedureCode { get; set; }

        public string TheraClassCode { get; set; }

        public DateTime? MedicationDate { get; set; }

        public string Drug { get; set; }

        public string DrugCategory { get; set; }

        public string EnrollType { get; set; }

        public DateTime? PaidDate { get; set; }

        public decimal? BilledAmount { get; set; }

        public decimal? Copay { get; set; }

        public decimal? Deductible { get; set; }

        public decimal? Coinsurance { get; set; }

        public decimal? NetPaid { get; set; }

        public decimal? TotalAmountPaid { get; set; }

        public decimal? PatientPayAmount { get; set; }

        public decimal? NetAmountDue { get; set; }

        public decimal? ClientIngCost { get; set; }

        public decimal? PlanPaid { get; set; }

        public ClaimsRelationshipToSubscriberCode? RelationshipToSubscriberCode { get; set; }

        public bool IsSpouse
        {
            get { return RelationshipToSubscriberCode != null && RelationshipToSubscriberCode == ClaimsRelationshipToSubscriberCode.SPOUSE; }
        }

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
            if (DiagnosisCodeCondition.Contains(condition))
                return true;
            return false;
        }

        public void SetClaimCodeCondition(ILookup<string, ClaimCodeDto> claimCodes)
        {
            var PrimaryDiagnosis = CheckInterventConditions(DiagnosisCode, claimCodes);
            DiagnosisCodeCondition = PrimaryDiagnosis.Keys;
            DiagnosisCodeList = PrimaryDiagnosis.Values;
            var conditions = new List<ClaimsProcessClaimCodeCondition>();
            conditions.Add(ClaimsProcessClaimCodeCondition.OTHER);
            if (DiagnosisCodeCondition.Count() == 0 && !string.IsNullOrEmpty(DiagnosisCode))
            {
                DiagnosisCodeCondition = conditions;
                var codes = new List<string>();
                codes.Add("OTHER:" + DiagnosisCode + ":N/A");
                DiagnosisCodeList = codes;
            }

            foreach (var diagCode in claimCodes[DiagnosisCode])
            {
                CodeFlag = diagCode.CodeFlag;
            }
        }

        Dictionary<ClaimsProcessClaimCodeCondition, string> CheckInterventConditions(string diagnosisCode, ILookup<string, ClaimCodeDto> ClaimCodes)
        {
            Dictionary<ClaimsProcessClaimCodeCondition, string> conditions = new Dictionary<ClaimsProcessClaimCodeCondition, string>();
            if (!string.IsNullOrEmpty(diagnosisCode) && ClaimCodes.Contains(diagnosisCode))
            {
                foreach (var rec in ClaimCodes[diagnosisCode])
                {
                    conditions.Add((ClaimsProcessClaimCodeCondition)Enum.Parse(typeof(ClaimsProcessClaimCodeCondition), rec.CodeFlag), rec.CodeFlag + ":" + rec.Code + ":" + rec.CodeDescription);
                }
            }
            return conditions;
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

        public void SetDrugCategory(IDictionary<string, ClaimProcessTherapeuticClassCodeDto> theraClassCodes)
        {
            if (!string.IsNullOrEmpty(Drug) && !string.IsNullOrEmpty(TheraClassCode))
            {
                ClaimProcessTherapeuticClassCodeDto category;
                if (theraClassCodes.TryGetValue(TheraClassCode, out category))
                {
                    DrugCategory = category.DrugCategory;
                }
            }
        }
    }
}
