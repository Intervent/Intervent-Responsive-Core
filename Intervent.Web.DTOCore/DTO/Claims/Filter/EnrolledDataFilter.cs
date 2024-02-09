namespace Intervent.Web.DTO.DTO.Claims.Filter
{
    public sealed class EnrolledDataFilter : IInputFlatFileFilter
    {
        public IEnumerable<ClaimProcessEnrolledDataDto> EnrolledData { get; private set; }

        public IEnumerable<ClaimProcessHRADto> HRAEnrolled { get; private set; }
        public EnrolledDataFilter(IEnumerable<ClaimProcessEnrolledDataDto> enrolledData, IEnumerable<ClaimProcessHRADto> hRAEnrolled)
        {
            EnrolledData = enrolledData;
            HRAEnrolled = hRAEnrolled;
        }
        public string FilterMessage
        {
            get { return "Unique id is not enrolled in HRA or not in eligibility or not enrolled in any program."; }
        }

        public bool FilterRecord(ClaimsInputFlatFileModel record)
        {
            var rec = EnrolledData.FirstOrDefault(x => x.UniqueId == record.UniqueId && x.PortalId == record.PortalId);
            if (rec != null)
            {
                record.EnrollType = rec.EnrollType;
            }
            var hraRec = HRAEnrolled.FirstOrDefault(x => x.UniqueId == record.UniqueId && x.PortalId == record.PortalId);
            if (hraRec != null)
            {
                record.HasHRA = true;
            }
            return (rec == null && hraRec == null ? true : false);
        }
    }
}
