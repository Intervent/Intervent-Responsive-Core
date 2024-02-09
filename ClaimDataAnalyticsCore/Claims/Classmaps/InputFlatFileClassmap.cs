using ClaimDataAnalytics.Claims.Converters;
using ClaimDataAnalytics.Claims.Model;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimDataAnalytics.Claims.Classmaps
{
    public sealed class InputFlatFileClassMap : CsvClassMap<ClaimsInputFlatFileModel>
    {

        const string dateFormat = "MM/dd/yyyy";
        public InputFlatFileClassMap()
        {
            Map(m => m.InputFileRowId);
            Map(m => m.ProviderName);
            Map(m => m.MemberSSN);
            Map(m => m.SubscriberSSN);
            Map(m => m.MemberFirstName);
            Map(m => m.MemberLastName);
            Map(m => m.MemberDateOfBirth).TypeConverter(new DateToStringFormatConverter(dateFormat));
            Map(m => m.ServiceStartDate).TypeConverter(new DateToStringFormatConverter(dateFormat));
            Map(m => m.ServiceEndDate).TypeConverter(new DateToStringFormatConverter(dateFormat));
            //Map(m => m.PrimaryDiagnosisCode);
            //Map(m => m.DiagnosisCode2);
            //Map(m => m.DiagnosisCode3);
            //Map(m => m.DiagnosisCode4);
            //Map(m => m.DiagnosisCode5);
            //Map(m => m.DiagnosisCode6);
            //Map(m => m.DiagnosisCode7);
            //Map(m => m.DiagnosisCode8);
            Map(m => m.BilledAmount);
            Map(m => m.Copay);
            Map(m => m.Deductible);
            Map(m => m.Coinsurance);
            Map(m => m.NetPaid);
            //Map(m => m.GenericDrug);
            //Map(m => m.TheraClassCode);
            Map(m => m.TotalAmountPaidbyAllSource);
            Map(m => m.PatientPayAmount);
            Map(m => m.AmountofCopay);
            Map(m => m.AmountofCoinsurance);
            Map(m => m.NetAmountDue);
            Map(m => m.SpouseSSN);
            Map(m => m.UniqueId);
            //Map(m => m.TotalAmountPaidbyAllSourceX);
            //Map(m => m.PatientPayAmountX);
            //Map(m => m.AmountofCopayX);
            //Map(m => m.AmountofCoinsuranceX);
            //Map(m => m.NetAmountDueX);
            Map(m => m.RelationshipToSubscriberCode);
            //Map(m => m.PrimaryDiagnosisICDCode).TypeConverter(new ClaimCodeConverter());
            //Map(m => m.DiagnosisCode2ICDCode).TypeConverter(new ClaimCodeConverter());
            //Map(m => m.DiagnosisCode3ICDCode).TypeConverter(new ClaimCodeConverter());
            //Map(m => m.DiagnosisCode4ICDCode).TypeConverter(new ClaimCodeConverter());
            //Map(m => m.DiagnosisCode5ICDCode).TypeConverter(new ClaimCodeConverter());
            //Map(m => m.DiagnosisCode6ICDCode).TypeConverter(new ClaimCodeConverter());
            //Map(m => m.DiagnosisCode7ICDCode).TypeConverter(new ClaimCodeConverter());
            //Map(m => m.DiagnosisCode8ICDCode).TypeConverter(new ClaimCodeConverter());
            //Map(m => m.PrimaryDiagnosisCodeCondition).TypeConverter(new ClaimConditionConverter());
            //Map(m => m.DiagnosisCode2Condition).TypeConverter(new ClaimConditionConverter());
            //Map(m => m.DiagnosisCode3Condition).TypeConverter(new ClaimConditionConverter());
            //Map(m => m.DiagnosisCode4Condition).TypeConverter(new ClaimConditionConverter());
            //Map(m => m.DiagnosisCode5Condition).TypeConverter(new ClaimConditionConverter());
            //Map(m => m.DiagnosisCode6Condition).TypeConverter(new ClaimConditionConverter());
            //Map(m => m.DiagnosisCode7Condition).TypeConverter(new ClaimConditionConverter());
            //Map(m => m.DiagnosisCode8Condition).TypeConverter(new ClaimConditionConverter());
            //Map(m => m.OrgId);
            //Map(m => m.DrugCategory);
            //Map(m => m.CodeFlag);
            //Map(m => m.EnrollType);
            //Map(m => m.HasHRA);
            //Map(m => m.IncludeInLivongoOutput);
            Map(m => m.NewUniqueId);
            Map(m => m.OrgName);
        }
    }
}
