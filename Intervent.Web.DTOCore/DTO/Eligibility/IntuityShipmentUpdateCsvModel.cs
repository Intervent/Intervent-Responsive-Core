using System.ComponentModel.DataAnnotations;

namespace Intervent.Web.DTO
{
    public class IntuityShipmentCsvModel
    {
        [Required]
        [MaxLength(100)]
        public string UniqueId { get; set; }

        public int OrgId { get; set; }

        public DateTime? DateSent { get; set; }

        public string SerialNo { get; set; }

        public byte? CartridgeQty { get; set; }

        public string Tracking { get; set; }

        public string EmployeeOrderType { get; set; }

        public string SoNbr { get; set; }

        public bool Valid()
        {
            if (string.IsNullOrEmpty(this.UniqueId) || this.OrgId == 0)
            {
                return false;
            }

            return true;
        }

        public void MapToEligibilityDAL(DAL.IntuityEligibility dal)
        {
            if (!string.IsNullOrEmpty(this.SerialNo))
            {
                dal.SerialNumber = this.SerialNo;
            }
            //
            dal.DateUpdated = DateTime.UtcNow;
        }

        public void MapToFulfillmentDAL(DAL.IntuityFulfillments dal)
        {
            dal.RefillSentDate = this.DateSent;
            dal.RefillSent = this.CartridgeQty;
            dal.DateUpdated = DateTime.UtcNow;
            dal.SerialNumber = this.SerialNo;
            dal.TrackingNumber = this.Tracking;
            dal.SoNbr = this.SoNbr;
            if (!dal.RefillRequestDate.HasValue)
            {
                dal.RefillRequestDate = dal.RefillSentDate;
            }
            if (dal.RefillRequested == 0)
            {
                dal.RefillRequested = this.CartridgeQty.Value;
            }
            if (this.EmployeeOrderType == "INITIAL")
                dal.SendMeter = true;
            else
                dal.SendMeter = false;
        }
    }
}
