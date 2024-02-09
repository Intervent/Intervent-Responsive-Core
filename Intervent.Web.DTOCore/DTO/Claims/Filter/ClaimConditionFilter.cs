namespace Intervent.Web.DTO.DTO.Claims.Filter
{
    public sealed class ClaimConditionFilter : IInputFlatFileFilter
    {
        //KMUnique SSN view
        public string FilterMessage
        {
            get
            {
                return "No claim conditions of interest.";
            }
        }

        public bool FilterRecord(ClaimsInputFlatFileModel record)
        {
            if (record.PrimaryDiagnosisCodeCondition.Count() == 0 && record.DiagnosisCode2Condition.Count() == 0 && record.DiagnosisCode3Condition.Count() == 0
                && record.DiagnosisCode4Condition.Count() == 0 && record.DiagnosisCode5Condition.Count() == 0 && record.DiagnosisCode6Condition.Count() == 0
                && record.DiagnosisCode7Condition.Count() == 0 && record.DiagnosisCode8Condition.Count() == 0)
            {
                return true;
            }
            return false;
        }
    }
}
