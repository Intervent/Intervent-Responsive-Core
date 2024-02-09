using CsvHelper.Configuration;
using Intervent.Web.DTO.CustomCsvConverter;

namespace Intervent.Web.DTO
{
    public class IntuityEligibilityStatusCsvClassMap : CsvClassMap<IntuityEligibilityStatusCsvModel>
    {
        readonly string dateFormat = "MM/dd/yyyy";
        public IntuityEligibilityStatusCsvClassMap()
        {
            Map(m => m.PatientUniqueId);
            Map(m => m.BenefitHolderId);
            Map(m => m.Firstname);
            Map(m => m.Lastname);
            Map(m => m.Middlename);
            Map(m => m.Dob).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.Gender).TypeConverter(new GenderConverter(true));
            Map(m => m.EmailAddress);
            Map(m => m.Address1);
            Map(m => m.Address2);
            Map(m => m.City);
            Map(m => m.State);
            Map(m => m.Country);
            Map(m => m.Zip);
            Map(m => m.Phone);
            Map(m => m.UserEnrollmentType);
            Map(m => m.HireDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.TerminationDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.UserStatus);
            Map(m => m.BusinessUnit);
            Map(m => m.RegionCode);
            Map(m => m.UnionFlag);
            Map(m => m.PayType);
            Map(m => m.MedicalPlanCode);
            Map(m => m.MedicalPlanStartDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.MedicalPlanEndDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.TobaccoFlag);
        }
    }
}

