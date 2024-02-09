namespace InterventWebApp.Models
{
    public class UserRegistrationModel
    {
        public string OrganizationId { get; set; }

        public string OrganizationEmail { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string SID { get; set; }

        public DateTime DOB { get; set; }

        public string UniqueID { get; set; }

        public string PersonnelType { get; set; }

        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string CouponCode { get; set; }

        public string IDText { get; set; }

        public DateTime? SignUpDate { get; set; }

        public int ProviderId { get; set; }

        public string DeviceId { get; set; }
    }

    public class VerifyPrimaryUserInformationModel
    {
        public string DOB { get; set; }

        public string RegisterData { get; set; }

        public DateTime? SignUpDate { get; set; }
    }
}