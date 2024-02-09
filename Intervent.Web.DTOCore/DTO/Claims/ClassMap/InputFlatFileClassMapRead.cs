﻿using CsvHelper.Configuration;
using Intervent.Web.DTO.DTO.Claims.CustomConverter;

namespace Intervent.Web.DTO
{

    public sealed class InputFlatFileClassMapRead : CsvClassMap<ClaimsInputFlatFileModel>
    {

        const string dateFormat = "MM/dd/yyyy";

        public InputFlatFileClassMapRead()
        {
            Map(m => m.ProviderName);
            Map(m => m.MemberSSN);
            Map(m => m.MemberFirstName);
            Map(m => m.MemberLastName);
            Map(m => m.MemberDateOfBirth).TypeConverter(new DateFormatConverter(dateFormat));
            Map(m => m.ServiceStartDate).TypeConverter(new DateFormatConverter(dateFormat));
            Map(m => m.PrimaryDiagnosisCode);
            Map(m => m.DiagnosisCode2);
            Map(m => m.DiagnosisCode3);
            Map(m => m.DiagnosisCode4);
            Map(m => m.DiagnosisCode5);
            Map(m => m.DiagnosisCode6);
            Map(m => m.DiagnosisCode7);
            Map(m => m.DiagnosisCode8);
            Map(m => m.BilledAmount);
            Map(m => m.Copay);
            Map(m => m.Deductible);
            Map(m => m.Coinsurance);
            Map(m => m.NetPaid);
            Map(m => m.GenericDrug);
            Map(m => m.TheraClassCode);
            Map(m => m.TotalAmountPaidbyAllSource);
            Map(m => m.PatientPayAmount);
            Map(m => m.AmountofCopay);
            Map(m => m.AmountofCoinsurance);
            Map(m => m.NetAmountDue);
            Map(m => m.SpouseSSN);
            Map(m => m.UniqueId);
            Map(m => m.TotalAmountPaidbyAllSourceX);
            Map(m => m.PatientPayAmountX);
            Map(m => m.AmountofCopayX);
            Map(m => m.AmountofCoinsuranceX);
            Map(m => m.NetAmountDueX);
            Map(m => m.RelationshipToSubscriberCode);
            Map(m => m.PrimaryDiagnosisICDCode).TypeConverter(new ClaimCodeConverter());
            Map(m => m.DiagnosisCode2ICDCode).TypeConverter(new ClaimCodeConverter());
            Map(m => m.DiagnosisCode3ICDCode).TypeConverter(new ClaimCodeConverter());
            Map(m => m.DiagnosisCode4ICDCode).TypeConverter(new ClaimCodeConverter());
            Map(m => m.DiagnosisCode5ICDCode).TypeConverter(new ClaimCodeConverter());
            Map(m => m.DiagnosisCode6ICDCode).TypeConverter(new ClaimCodeConverter());
            Map(m => m.DiagnosisCode7ICDCode).TypeConverter(new ClaimCodeConverter());
            Map(m => m.DiagnosisCode8ICDCode).TypeConverter(new ClaimCodeConverter());
            Map(m => m.PrimaryDiagnosisCodeCondition).TypeConverter(new ClaimConditionConverter());
            Map(m => m.DiagnosisCode2Condition).TypeConverter(new ClaimConditionConverter());
            Map(m => m.DiagnosisCode3Condition).TypeConverter(new ClaimConditionConverter());
            Map(m => m.DiagnosisCode4Condition).TypeConverter(new ClaimConditionConverter());
            Map(m => m.DiagnosisCode5Condition).TypeConverter(new ClaimConditionConverter());
            Map(m => m.DiagnosisCode6Condition).TypeConverter(new ClaimConditionConverter());
            Map(m => m.DiagnosisCode7Condition).TypeConverter(new ClaimConditionConverter());
            Map(m => m.DiagnosisCode8Condition).TypeConverter(new ClaimConditionConverter());
            Map(m => m.OrgId);
            Map(m => m.DrugCategory);
            Map(m => m.CodeFlag);
            Map(m => m.EnrollType);
            Map(m => m.HasHRA);
            Map(m => m.IncludeInLivongoOutput);
        }
    }
}
