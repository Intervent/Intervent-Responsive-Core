using ClaimDataAnalytics.Eligibility.Converter;
using ClaimDataAnalytics.Eligibility.CsvModel;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimDataAnalytics.Eligibility.Classmap
{
    public sealed class EligibilityCsvClassMap : CsvClassMap<EligibilityCsvModel>
    {
        readonly string dateFormat = "yyyyMMdd";
        public EligibilityCsvClassMap()
        {

            Map(m => m.UniqueId);
            Map(m => m.FirstName);
            Map(m => m.LastName);
            Map(m => m.DOB).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.Gender).TypeConverter(new GenderConverter());
            Map(m => m.SSN);
            Map(m => m.Address1).Name("Address1");
            Map(m => m.Address2).Name("Address2");
            Map(m => m.City);
            Map(m => m.Country);
            Map(m => m.StateOrProvince).Name("State");
            Map(m => m.ZipOrPostalCode).Name("ZIP");
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
            Map(m => m.DOB).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.Gender).TypeConverter(new GenderConverter());
            Map(m => m.SSN);
            Map(m => m.Address1).Name("Address1");
            Map(m => m.Address2).Name("Address2");
            Map(m => m.City);
            Map(m => m.Country);
            Map(m => m.StateOrProvince).Name("State");
            Map(m => m.ZipOrPostalCode).Name("Zip");
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
          //  Map(m => m.EducationalAssociates).TypeConverter(new YesNoConverter());
            Map(m => m.MedicalPlanStartDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.MedicalPlanEndDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.MedicalPlanCode);
            Map(m => m.PayType).TypeConverter(new PayTypeConverter());
            Map(m => m.UserStatus).TypeConverter(new EligibilityStatusConverter());

            //Map(m => m.DentalPlanCode);
            //Map(m => m.DentalPlanStartDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            //Map(m => m.DentalPlanEndDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            //Map(m => m.VisionPlanCode);
            //Map(m => m.PayrollArea);
            //Map(m => m.VisionPlanStartDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            //Map(m => m.VisionPlanEndDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);

        }
    }

    public sealed class EligibilityCsvClassMapV2 : CsvClassMap<EligibilityCsvModel>
    {
        readonly string dateFormat = "yyyyMMdd";
        public EligibilityCsvClassMapV2()
        {

            Map(m => m.UniqueId);
            Map(m => m.FirstName).Name("PatientFirstName");
            Map(m => m.LastName).Name("PatientLastName");
            Map(m => m.DOB).Name("PatientDOB").TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            // Map(m => m.Gender).TypeConverter(new CsvHelper.TypeConversion.EnumConverter(typeof(Gender)));
            Map(m => m.Gender).TypeConverter(new GenderConverter());
            Map(m => m.SSN);
            Map(m => m.Address1).Name("Address1");
            Map(m => m.Address2).Name("Address2");
            Map(m => m.City);
            Map(m => m.Country);
            Map(m => m.StateOrProvince).Name("State");
            Map(m => m.ZipOrPostalCode).Name("Zip");
            Map(m => m.EmailAddress);
            Map(m => m.HomePhone);
            Map(m => m.EmployeeUniqueId).Name("Custom9");
            Map(m => m.HireDate).Name("Custom13").TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
           // Map(m => m.TerminatedDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
          //  Map(m => m.DeathDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.BusinessUnit).Name("Custom4");
          //  Map(m => m.RegionCode);
            // Map(m => m.UnionFlag).TypeConverter(new CsvHelper.TypeConversion.EnumConverter(typeof(YesNo)));
          //  Map(m => m.UnionFlag).TypeConverter(new YesNoConverter());
            Map(m => m.UserEnrollmentType).Name("Custom7").TypeConverter(new UserEnrollmentTypeConverter(true));
            //Map(m => m.TobaccoFlag).TypeConverter(new YesNoConverter());
            Map(m => m.MedicalPlanStartDate).Name("Custom22").TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.MedicalPlanEndDate).Name("Custom23").TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.MedicalPlanCode).Name("Custom21");
            //Map(m => m.PayType).TypeConverter(new PayTypeConverter());
            Map(m => m.UserStatus).Name("Custom16").TypeConverter(new EligibilityStatusConverter());
        }
    }
    public sealed class EligibilityCsvClassMapV3 : CsvClassMap<EligibilityCsvModel>
    {
        readonly string dateFormat = "yyyyMMdd";
        public EligibilityCsvClassMapV3()
        {

            Map(m => m.UniqueId);
            Map(m => m.FirstName).Name("PatientFirstName");
            Map(m => m.LastName).Name("PatientLastName");
            Map(m => m.DOB).Name("PatientDOB").TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            // Map(m => m.Gender).TypeConverter(new CsvHelper.TypeConversion.EnumConverter(typeof(Gender)));
            Map(m => m.Gender).TypeConverter(new GenderConverter());
            Map(m => m.SSN);
            Map(m => m.Address1).Name("Address1");
            Map(m => m.Address2).Name("Address2");
            Map(m => m.City);
            Map(m => m.Country);
            Map(m => m.StateOrProvince).Name("State");
            Map(m => m.ZipOrPostalCode).Name("Zip");
            //Map(m => m.EmailAddress);
            Map(m => m.HomePhone);
            Map(m => m.EmployeeUniqueId).Name("Custom9");
            Map(m => m.HireDate).Name("Custom13").TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            // Map(m => m.TerminatedDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            //  Map(m => m.DeathDate).TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.BusinessUnit).Name("Custom4");
            //  Map(m => m.RegionCode);
            // Map(m => m.UnionFlag).TypeConverter(new CsvHelper.TypeConversion.EnumConverter(typeof(YesNo)));
            //  Map(m => m.UnionFlag).TypeConverter(new YesNoConverter());
            Map(m => m.UserEnrollmentType).Name("Custom7").TypeConverter(new UserEnrollmentTypeConverter(true));
            //Map(m => m.TobaccoFlag).TypeConverter(new YesNoConverter());
            Map(m => m.MedicalPlanStartDate).Name("Custom22").TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.MedicalPlanEndDate).Name("Custom23").TypeConverter<CsvHelper.TypeConversion.DateTimeConverter>().TypeConverterOption(dateFormat);
            Map(m => m.MedicalPlanCode).Name("Custom21");
            //Map(m => m.PayType).TypeConverter(new PayTypeConverter());
            Map(m => m.UserStatus).Name("Custom16").TypeConverter(new EligibilityStatusConverter());
        }
    }
}
