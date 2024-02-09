namespace Intervent.Web.DTO
{
    public class ClaimProcessEligibilityDto
    {
        public int PortalId { get; set; }

        public int OrgId { get; set; }

        public string UniqueID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string SSN { get; set; }

        public string MedicalPlanCode { get; set; }

        public DateTime? MedicalPlanEndDate { get; set; }

        public string UserStatus { get; set; }

        public string HomeNumber { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string BusinessUnit { get; set; }

        public string UserEnrollmentType { get; set; }

        public string OrgName { get; set; }
    }

    public class CrothalIdDto
    {
        public string OldUniqueId { get; set; }

        public string NewUniqueId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
