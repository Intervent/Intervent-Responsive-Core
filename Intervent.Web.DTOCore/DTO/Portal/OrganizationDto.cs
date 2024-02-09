namespace Intervent.Web.DTO
{
    public class OrganizationDto
    {
        public int? Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }

        public string Url { get; set; }

        public string IntuityEmpUrl { get; set; }

        public string IntuityEmpToken { get; set; }

        public OrganizationDto Organization1 { get; set; }

        public IList<OrganizationDto> Organizations1 { get; set; }

        public int? ParentOrganizationId { get; set; }

        public bool EmailValidationRequired { get; set; }

        public string ContactNumber { get; set; }

        public string ContactEmail { get; set; }

        public bool? TermsForSSO { get; set; }

        public bool? SSO { get; set; }

        public bool OwnCoach { get; set; }

        public byte? IntegrationWith { get; set; }

        public IList<RoleDto> UserRoles { get; set; }

        public IList<PortalDto> Portals { get; set; }

        public IList<CouponCodeDto> CouponCodes { get; set; }
    }
}