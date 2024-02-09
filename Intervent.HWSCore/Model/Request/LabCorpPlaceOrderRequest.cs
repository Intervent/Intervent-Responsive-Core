namespace Intervent.HWS.Request
{
    public class LabCorpPlaceOrderRequest
    {
        public string OrderNumber { get; set; }

        public string Comments { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Gender { get; set; }

        public DateTime DOB { get; set; }

        public string PatientId { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public bool? Fasting { get; set; }

        public string LabProcedures { get; set; }

        public string labCorpUserName { get; set; }

        public string labCorpPassword { get; set; }

        public string labCorpAccountNumber { get; set; }

    }
}
