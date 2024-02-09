namespace Intervent.Web.DTO
{
    public class IntuityFulfillmentsDto
    {
        public int? Id { get; set; }

        public int IntuityEligibilityId { get; set; }

        public string SerialNumber { get; set; }

        public byte RefillRequested { get; set; }

        public DateTime? RefillRequestDate { get; set; }

        public byte? RefillSent { get; set; }

        public DateTime? RefillSentDate { get; set; }

        public string TrackingNumber { get; set; }

        public bool? SendMeter { get; set; }


        public DAL.IntuityFulfillments MapToIntuityFulfillment(DAL.IntuityFulfillments source)
        {
            var dal = new DAL.IntuityFulfillments();

            dal.DateCreated = source.DateCreated;
            dal.DateUpdated = source.DateUpdated;
            dal.IntuityEligibilityId = source.IntuityEligibilityId;
            dal.RefillRequestDate = source.RefillRequestDate;
            dal.RefillRequested = source.RefillRequested;
            dal.RefillSent = source.RefillSent;
            dal.RefillSentDate = source.RefillSentDate;
            dal.TrackingNumber = source.TrackingNumber;
            dal.SendMeter = source.SendMeter;
            dal.SerialNumber = source.SerialNumber;
            dal.SoNbr = source.SoNbr;
            return dal;
        }
    }

    public class IntuityShipmentResponse
    {
        public DAL.IntuityEligibility Eligibility { get; set; }
        public DAL.IntuityFulfillments Fulfillment { get; set; }
    }
}
