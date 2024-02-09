namespace Intervent.Web.DTO
{
    public class ClaimCodeDto
    {
        public string Code { get; set; }

        public string CodeFlag { get; set; }

        public string CodeSource { get; set; }

        public string CodeDescription { get; set; }

        public bool IncludeME { get; set; }
    }

    public class ClaimProcessInsuranceSummaryDto
    {
        public int Id { get; set; }

        public string UniqueId { get; set; }

        public int PortalId { get; set; }

        public int OrgId { get; set; }

        public bool IsPregnant { get; set; }

        public bool IsObese { get; set; }

        public bool HasLungDisorder { get; set; }

        public bool HasCKD { get; set; }

        public bool HasPrediabetes { get; set; }

        public bool NoIVClaimCondition { get; set; }

        public bool HasHeartDisorder { get; set; }

        public bool HasSleepingDisorder { get; set; }

        public bool HasHyperTension { get; set; }

        public bool IsSmoking { get; set; }

        public bool IsDiabetic { get; set; }

        public bool IsDiabeticLivongo { get; set; }

        public DateTime? LatestPregnancyDate { get; set; }

        public bool HRA { get; set; }

        public string EnrollType { get; set; }

    }

    public class ClaimProcessEnrolledDataDto
    {
        public string UniqueId { get; set; }

        public string EnrollType { get; set; }

        public int PortalId { get; set; }
    }

    public class ClaimProcessHRADto
    {
        public string UniqueId { get; set; }

        public int PortalId { get; set; }
    }

    public class ClaimProcessTherapeuticClassCodeDto
    {
        public string TheraCode { get; set; }

        public string DrugCategory { get; set; }
    }

    public class LivongoICDCodesDto
    {
        // public int LvRef { get; set; }
        public string Code { get; set; }

        public string CodeFlag { get; set; }
        public string CodeSource { get; set; }

        public string CodeDescription { get; set; }
    }

    public class LivongoNDCCodesDto
    {
        public string Code { get; set; }

    }
}
