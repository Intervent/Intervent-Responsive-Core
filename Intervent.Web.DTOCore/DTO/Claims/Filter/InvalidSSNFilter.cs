namespace Intervent.Web.DTO.DTO.Claims.Filter
{
    public sealed class InvalidSSNFilter : IInputFlatFileFilter
    {
        string msg = "SSN is invalid";
        public string FilterMessage
        {
            get { return msg; }
        }

        public bool FilterRecord(ClaimsInputFlatFileModel record)
        {
            //if (record.ProviderName == ClaimProviderDto.CVS_CAREMARK.Name)
            //    return false;
            if (String.IsNullOrEmpty(record.MemberSSN))
                return true;
            else if (record.MemberSSN.Contains("000000") || record.MemberSSN.Contains("111111") || record.MemberSSN.Contains("222222") ||
                record.MemberSSN.Contains("333333") || record.MemberSSN.Contains("444444") || record.MemberSSN.Contains("555555") || record.MemberSSN.Contains("666666") ||
                record.MemberSSN.Contains("777777") || record.MemberSSN.Contains("888888") || record.MemberSSN.Contains("999999"))
                return true;
            return false;
        }
    }
}
