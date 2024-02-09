namespace Intervent.Web.DTO.DTO.Claims.Filter
{
    public sealed class ChildrenFilter : IInputFlatFileFilter
    {
        //KMUnique SSN view
        public string FilterMessage
        {
            get
            {
                return "Children Record.";
            }
        }

        public bool FilterRecord(ClaimsInputFlatFileModel record)
        {
            if (record.RelationshipToSubscriberCode == ClaimsRelationshipToSubscriberCode.CHILDREN)
            {
                return true;
            }
            return false;
        }
    }
}
