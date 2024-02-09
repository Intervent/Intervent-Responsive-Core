using CsvHelper.Configuration;

namespace Intervent.Web.DTO
{
    public class IntuityActivityCsvClassMap : CsvClassMap<IntuityActivityCsvModel>
    {
        public IntuityActivityCsvClassMap()
        {
            Map(m => m.UniqueID);
            Map(m => m.EventType);
            Map(m => m.DateTimeStamp);
        }
    }
}

