namespace Intervent.Web.DTO
{
    public class EnrollmentStatusDto
    {
        public byte Code { get; set; }

        public string Description { get; set; }
        private EnrollmentStatusDto(byte code, string desc)
        {
            Code = code;
            Description = desc;
        }

        public static EnrollmentStatusDto CurrentReferral = new EnrollmentStatusDto(4, "Current referral");
        public static EnrollmentStatusDto Trying = new EnrollmentStatusDto(3, "Trying");
        public static EnrollmentStatusDto Enrolled = new EnrollmentStatusDto(2, "Enrolled");
        public static EnrollmentStatusDto Declined = new EnrollmentStatusDto(1, "Declined");
        public static EnrollmentStatusDto UnableToReach = new EnrollmentStatusDto(5, "Unable to reach");
        public static EnrollmentStatusDto NoLongerinStudy = new EnrollmentStatusDto(6, "No Longer in Study");

        public static IEnumerable<EnrollmentStatusDto> GetAll()
        {
            List<EnrollmentStatusDto> lst = new List<EnrollmentStatusDto>();
            lst.Add(EnrollmentStatusDto.CurrentReferral);
            lst.Add(EnrollmentStatusDto.Trying);
            lst.Add(EnrollmentStatusDto.Enrolled);
            lst.Add(EnrollmentStatusDto.Declined);
            lst.Add(EnrollmentStatusDto.UnableToReach);
            lst.Add(EnrollmentStatusDto.NoLongerinStudy);
            return lst;
        }

        public static EnrollmentStatusDto GetByKey(byte code)
        {
            return GetAll().Where(x => x.Code == code).FirstOrDefault();
        }
    }
}
