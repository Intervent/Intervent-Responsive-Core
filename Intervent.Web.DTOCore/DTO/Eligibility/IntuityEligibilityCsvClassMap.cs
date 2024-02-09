using CsvHelper.Configuration;
using Intervent.Web.DTO.CustomCsvConverter;


namespace Intervent.Web.DTO
{
    public class IntuityEligibilityCsvClassMap : CsvClassMap<IntuityEligibilityCsvModel>
    {
        readonly string dateFormat = "yyyyMMdd";
        public IntuityEligibilityCsvClassMap()
        {
            Map(m => m.UniqueId);
            Map(m => m.FirstName);
            Map(m => m.LastName);
            Map(m => m.OrganizationCode);
            Map(m => m.Email);
            Map(m => m.HasDiabetes);
            Map(m => m.Diabeteyear);
            Map(m => m.TakeDiabeteMed);
            Map(m => m.TakeInsulin);
            Map(m => m.HadA1CTest);
            Map(m => m.A1CValue);
            Map(m => m.DOB).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.Gender).TypeConverter(new GenderConverter(true));
            Map(m => m.DiabetesType).TypeConverter(new EnumConverter(typeof(DiabeticType)));
            Map(m => m.TookA1C).TypeConverter(new EnumConverter(typeof(BoolState)));
        }
    }
}
