namespace Intervent.Web.DTO
{
    public class IntuityEligibilityDto
    {
        public int? Id { get; set; }

        public string UniqueId { get; set; }

        public int OrganizationId { get; set; }

        public EligibilityStatus EligibilityStatus { get; set; }

        public EligibilityReason EligibilityReason { get; set; }

        public string SerialNumber { get; set; }

        public bool? OverrideStatus { get; set; }

        public int? UpdatedBy { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string email { get; set; }

        public string PhoneNumber { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public int? State { get; set; }

        public int? Country { get; set; }

        public string Zip { get; set; }

        public DateTime? OptingOut { get; set; }

        public static IntuityEligibilityDto CreateIntuityEligibility(int orgId, string uniqueId)
        {
            IntuityEligibilityDto dto = new IntuityEligibilityDto();
            dto.UniqueId = uniqueId;
            dto.OrganizationId = orgId;
            dto.EligibilityStatus = EligibilityStatus.NotEligible;
            return dto;
        }

        public DAL.IntuityEligibility MapToIntuityEligibility(DAL.IntuityEligibility source)
        {
            var dal = new DAL.IntuityEligibility();
            if (source == null)
            {
                dal.OrganizationId = this.OrganizationId;
                dal.UniqueId = this.UniqueId;
                dal.DateCreated = DateTime.UtcNow;
            }
            else
            {
                dal.OrganizationId = source.OrganizationId;
                dal.UniqueId = source.UniqueId;
                dal.DateCreated = source.DateCreated;
                dal.Id = source.Id;

            }
            if (this.EligibilityReason != EligibilityReason.None)
            {
                dal.EligibilityReason = (byte)this.EligibilityReason;
            }
            dal.OverrideStatus = this.OverrideStatus;
            dal.UpdatedBy = this.UpdatedBy;
            dal.SerialNumber = this.SerialNumber;
            dal.EligibilityStatus = (byte)this.EligibilityStatus;
            dal.DateUpdated = DateTime.UtcNow;
            return dal;
        }

        public static IntuityEligibilityDto MapToIntuityEligibilityDto(DAL.IntuityEligibility dal)
        {
            var dto = new IntuityEligibilityDto();
            dto.Id = dal.Id;
            dto.UniqueId = dal.UniqueId;
            dto.OrganizationId = dal.OrganizationId;
            dto.SerialNumber = dal.SerialNumber;
            dto.OverrideStatus = dal.OverrideStatus;
            dto.UpdatedBy = dal.UpdatedBy;
            dto.FirstName = dal.FirstName;
            dto.LastName = dal.LastName;
            dto.email = dal.email;
            dto.PhoneNumber = dal.PhoneNumber;
            dto.AddressLine1 = dal.AddressLine1;
            dto.AddressLine2 = dal.AddressLine2;
            dto.City = dal.City;
            dto.State = dal.State;
            dto.Country = dal.Country;
            dto.Zip = dal.Zip;
            dto.OptingOut = dal.OptingOut;
            dto.EligibilityStatus = (EligibilityStatus)dal.EligibilityStatus;
            if (dal.EligibilityReason.HasValue)
            {
                dto.EligibilityReason = (EligibilityReason)dal.EligibilityReason;
            }
            return dto;
        }

        public static DAL.IntuityEligibility MapToIntuityEligibility(IntuityEligibilityDto dto)
        {
            var dal = new DAL.IntuityEligibility();
            dal.UniqueId = dto.UniqueId;
            dal.OrganizationId = dto.OrganizationId;
            dal.EligibilityStatus = (byte)dto.EligibilityStatus;
            dal.EligibilityReason = (byte)dto.EligibilityReason;
            dal.SerialNumber = dto.SerialNumber;
            dal.OverrideStatus = dto.OverrideStatus;
            dal.UpdatedBy = dto.UpdatedBy;
            dal.FirstName = dto.FirstName;
            dal.LastName = dto.LastName;
            dal.email = dto.email;
            dal.PhoneNumber = dto.PhoneNumber;
            dal.AddressLine1 = dto.AddressLine1;
            dal.AddressLine2 = dto.AddressLine2;
            dal.City = dto.City;
            dal.State = dto.State;
            dal.Country = dto.Country;
            dal.Zip = dto.Zip;
            dal.OptingOut = dto.OptingOut;

            return dal;
        }
    }

}