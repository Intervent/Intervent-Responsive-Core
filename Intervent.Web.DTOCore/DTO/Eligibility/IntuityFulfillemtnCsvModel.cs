using System.ComponentModel.DataAnnotations;

namespace Intervent.Web.DTO
{
    public class IntuityFulfillmentCsvModel
    {

        public int IntuityEligibilityId { get; set; }

        [MaxLength(100)]
        public string UniqueId { get; set; }

        [MaxLength(128)]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [MaxLength(128)]
        public string LastName { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        public bool SendMeter { get; set; }

        public byte CartridgeQty { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }


        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string StateOrProvince { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }

        [MaxLength(15)]
        public string ZipOrPostalCode { get; set; }

        public string PhoneNumber { get; set; }

        public string SerialNo { get; set; }

        public DateTime? RefillRequestDate { get; set; }
    }
}
