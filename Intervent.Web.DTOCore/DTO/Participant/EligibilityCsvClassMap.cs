using CsvHelper.Configuration;
using Intervent.Web.DTO.CustomCsvConverter;

namespace Intervent.Web.DTO
{
    public sealed class EligibilityCsvClassMap : CsvClassMap<EligibilityCsvModel>
    {
        readonly string dateFormat = "yyyyMMdd";
        public EligibilityCsvClassMap()
        {

            Map(m => m.UniqueId);
            Map(m => m.FirstName);
            Map(m => m.LastName);
            Map(m => m.MiddleName);
            Map(m => m.DOB).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.Gender).TypeConverter(new GenderConverter());
            Map(m => m.SSN);
            Map(m => m.Address1);
            Map(m => m.Address2);
            Map(m => m.City);
            Map(m => m.Country);
            Map(m => m.StateOrProvince);
            Map(m => m.ZipOrPostalCode);
            Map(m => m.EmailAddress);
            Map(m => m.HomePhone);
            Map(m => m.EmployeeUniqueId);
            Map(m => m.HireDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.TerminatedDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.DeathDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.BusinessUnit);
            Map(m => m.RegionCode);
            Map(m => m.UnionFlag).TypeConverter(new YesNoConverter());
            Map(m => m.UserEnrollmentType).TypeConverter(new UserEnrollmentTypeConverter());
            Map(m => m.TobaccoFlag).TypeConverter(new YesNoConverter());
            Map(m => m.MedicalPlanStartDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.MedicalPlanEndDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.MedicalPlanCode);
            Map(m => m.PayType).TypeConverter(new PayTypeConverter());
            Map(m => m.UserStatus).TypeConverter(new EligibilityStatusConverter());
        }
    }

    public sealed class EligibilityCsvClassMapV1 : CsvClassMap<EligibilityCsvModel>
    {
        readonly string dateFormat = "yyyyMMdd";
        public EligibilityCsvClassMapV1()
        {

            Map(m => m.UniqueId);
            Map(m => m.FirstName);
            Map(m => m.LastName);
            Map(m => m.MiddleName);
            Map(m => m.DOB).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.Gender).TypeConverter(new GenderConverter());
            Map(m => m.SSN);
            Map(m => m.Address1);
            Map(m => m.Address2);
            Map(m => m.City);
            Map(m => m.Country);
            Map(m => m.StateOrProvince);
            Map(m => m.ZipOrPostalCode);
            Map(m => m.EmailAddress);
            Map(m => m.HomePhone);
            Map(m => m.EmployeeUniqueId);
            Map(m => m.HireDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.TerminatedDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.DeathDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.BusinessUnit);
            Map(m => m.RegionCode);
            Map(m => m.UnionFlag).TypeConverter(new YesNoConverter());
            Map(m => m.UserEnrollmentType).TypeConverter(new UserEnrollmentTypeConverter());
            Map(m => m.TobaccoFlag).TypeConverter(new YesNoConverter());
            Map(m => m.EducationalAssociates).TypeConverter(new YesNoConverter());
            Map(m => m.MedicalPlanStartDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.MedicalPlanEndDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.MedicalPlanCode);
            Map(m => m.PayType).TypeConverter(new PayTypeConverter());
            Map(m => m.UserStatus).TypeConverter(new EligibilityStatusConverter());

            Map(m => m.DentalPlanCode);
            Map(m => m.DentalPlanStartDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.DentalPlanEndDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.VisionPlanCode);
            Map(m => m.PayrollArea);
            Map(m => m.VisionPlanStartDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.VisionPlanEndDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);

        }
    }

    public sealed class EligibilityCsvClassMapV5 : CsvClassMap<EligibilityCsvModel>
    {
        public EligibilityCsvClassMapV5()
        {
            Map(m => m.UniqueId).Name("Unique Member ID");
            Map(m => m.CompanyCode).Name("Company Name");
            Map(m => m.MedicalPlanCode).Name("Member policy type");
            Map(m => m.BusinessUnit).Name("Department");
            Map(m => m.Country).Name("Country");
            Map(m => m.UserStatus).Name("Member Monthly Status").TypeConverter(new EligibilityStatusConverter());
        }
    }

    public sealed class EligibilityCsvClassMapV2 : CsvClassMap<EligibilityCsvModel>
    {
        readonly string dateFormat = "yyyyMMdd";
        public EligibilityCsvClassMapV2()
        {
            Map(m => m.UniqueId).TypeConverter(new EligibilityLMCUniqueIdConverter()); ;
            Map(m => m.FirstName);
            Map(m => m.LastName);
            Map(m => m.DOB).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.Address1);
            Map(m => m.Address2);
            Map(m => m.City);
            Map(m => m.HomePhone);
            Map(m => m.StateOrProvince);
            Map(m => m.ZipOrPostalCode);
            Map(m => m.EmailAddress);
            Map(m => m.Ref_LastName);
            Map(m => m.Ref_FirstName);
            Map(m => m.Ref_PractNum);
            Map(m => m.Ref_OfficeName);
            Map(m => m.Ref_City);
            Map(m => m.Ref_Province);
            Map(m => m.Ref_OfficePhone);
            Map(m => m.Ref_FaxPhone);
            Map(m => m.Lab_Date).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.Lab_Fasting).TypeConverter(new EligibilityDidYouFastConverter());
            Map(m => m.Lab_A1C);
            Map(m => m.Lab_FBS);
        }
    }

    public sealed class EligibilityCsvClassMapV3 : CsvClassMap<EligibilityCsvModel>
    {
        readonly string dateFormat = "MM/dd/yyyy";
        public EligibilityCsvClassMapV3()
        {
            Map(m => m.UniqueId);
            Map(m => m.FirstName);
            Map(m => m.LastName);
            Map(m => m.EmailAddress).Name("ShopifyEmailID");
            Map(m => m.Phone).Name("PhoneNumber");
            Map(m => m.HireDate).Name("CreateDate");
            Map(m => m.CoachingEnabled).Name("CoachingFlag").TypeConverter(new TrueFalseConverter());
            Map(m => m.CoachingExpirationDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
        }
    }


    public sealed class EligibilityCsvClassMapV6 : CsvClassMap<EligibilityCsvModel>
    {
        readonly string dateFormat = "yyyy/MM/dd";
        public EligibilityCsvClassMapV6()
        {

            Map(m => m.UniqueId).Name("SubjectID");
            Map(m => m.FirstName);
            Map(m => m.LastName);
            Map(m => m.MiddleName).Name("MiddleInitial");
            Map(m => m.DOB).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.Address1);
            Map(m => m.Address2);
            Map(m => m.City);
            Map(m => m.StateOrProvince).Name("State");
            Map(m => m.ZipOrPostalCode).Name("ZipCode");
            Map(m => m.Country).Name("Country");
            Map(m => m.HomePhone).Name("Phone1");
            Map(m => m.CellNumber).Name("Phone2");
            Map(m => m.EmailAddress);
        }
    }

    public sealed class EligibilityCsvClassMapV7 : CsvClassMap<EligibilityCsvModel>
    {
        readonly string dateFormat = "yyyy/MM/dd";
        public EligibilityCsvClassMapV7()
        {
            Map(m => m.UniqueId).Name("SubjectID");
            Map(m => m.RegionCode).Name("SiteID");
            Map(m => m.Gender).Name("Sex").TypeConverter(new GenderConverter(true));
            Map(m => m.Race).Name("Race");
            Map(m => m.HireDate).Name("EnrollmentDate").TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.TerminationReason).Name("TerminationReason");
        }
    }

    public sealed class BiometricCsvClassMap : CsvClassMap<BiometricCsvModel>
    {
        readonly string dateFormat = "yyyy/MM/dd";
        public BiometricCsvClassMap()
        {
            Map(m => m.UniqueId).Name("SubjectID");
            Map(m => m.Visit).TypeConverter(new VisitTypeConverter());
            Map(m => m.SBP);
            Map(m => m.DBP);
            Map(m => m.Height);
            Map(m => m.Weight);
            Map(m => m.TotalChol).Name("TotalCholesterol");
            Map(m => m.HDL);
            Map(m => m.LDL);
            Map(m => m.Trig).Name("Triglycerides");
            Map(m => m.A1C).Name("HemA1c");
            Map(m => m.DidYouFast).Name("Fasting").TypeConverter(new YesNoConverter(true));
            Map(m => m.AssessmentDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
        }
    }

    public sealed class MedicationCsvClassMap : CsvClassMap<MedicationCsvModel>
    {
        readonly string dateFormat = "yyyy/MM/dd";
        public MedicationCsvClassMap()
        {
            Map(m => m.UniqueId).Name("SubjectID");
            Map(m => m.Medication);
            Map(m => m.StartDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.EndDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.Dose);
            Map(m => m.Units);
            Map(m => m.Frequency);
        }
    }
}
