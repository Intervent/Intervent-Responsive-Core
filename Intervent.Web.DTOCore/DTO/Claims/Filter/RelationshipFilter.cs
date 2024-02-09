namespace Intervent.Web.DTO.DTO.Claims.Filter
{
    public sealed class RelationshipFilter : IInputFlatFileFilter
    {
        public string FilterMessage { get { return "relationship to subscriber code is null"; } }

        public bool FilterRecord(ClaimsInputFlatFileModel record)
        {
            return record.RelationshipToSubscriberCode == null;
        }
    }
    public sealed class AgeFilter : IInputFlatFileFilter
    {
        public string FilterMessage { get { return "age is less than 18"; } }

        public bool FilterRecord(ClaimsInputFlatFileModel record)
        {
            if (record.MemberDateOfBirth.HasValue)
            {
                var DateOfBirth = Convert.ToDateTime(record.MemberDateOfBirth);
                var age = DateTime.Now.Year - DateOfBirth.Year;
                if (age < 18)
                    return true;
            }
            return false;
        }
    }
}
