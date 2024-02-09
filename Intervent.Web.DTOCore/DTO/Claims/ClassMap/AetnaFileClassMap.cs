using CsvHelper.Configuration;
using Intervent.Web.DTO.DTO.Claims.CustomConverter;
using Intervent.Web.DTO.DTO.Claims.CustomConverter.AETNA;

namespace Intervent.Web.DTO
{
    public sealed class AetnaFileClassMap : CsvClassMap<ClaimsInputFlatFileModel>
    {
        public AetnaFileClassMap()
        {
            //16 is also SSN
            Map(m => m.MemberSSN).Index(24).TypeConverter(new SSNConverter());//case when len(ltrim(rtrim([Member''s SSN])))= 11 then RIGHT(ltrim(rtrim([Member''s SSN])),9) else ltrim(rtrim([Member''s SSN])) End
            Map(m => m.MemberFirstName).Index(28);//	,[Member''s First Name]
            Map(m => m.MemberLastName).Index(27);//	,[Member''s Last Name]
            //should i use employees DOB or member's DOB
            Map(m => m.MemberDateOfBirth).Index(31).TypeConverter(new DateFormatConverter("yyyy-MM-dd"));//	,ltrim(rtrim([Employee''s Date of Birth]))
            Map(m => m.ServiceStartDate).Index(59).TypeConverter(new DateFormatConverter("yyyy-MM-dd"));//	,[Date Service Started]
            Map(m => m.PrimaryDiagnosisCode).Index(152);//	,[Diagnosis Code 1]
            Map(m => m.DiagnosisCode2).Index(153);//	,[Diagnosis Code 2]
            Map(m => m.DiagnosisCode3).Index(154);//	,[Diagnosis Code 3]
            Map(m => m.DiagnosisCode4).Index(155);//	,[Diagnosis Code 4]
            Map(m => m.DiagnosisCode5).Index(162);//	,[ICD Procedure Code 1]
            Map(m => m.DiagnosisCode6).Index(163);//	,[ICD Procedure Code 2]
            Map(m => m.DiagnosisCode7).Index(164);//	,[ICD Procedure Code 3]
            Map(m => m.DiagnosisCode8).Index(67);//	,[Line-Level Procedure Code(CPT, HCPCS, ADA, CDT)]
            //amt billed
            Map(m => m.BilledAmount).Index(83);//	,[Net Submitted Expense] 
            Map(m => m.Copay).Index(93);//	,[Copayment Amount]
            Map(m => m.Deductible).Index(95);//	,[Deductible Amount]
            Map(m => m.Coinsurance).Index(96);//	,[Coinsurance]
            //total amt paid
            Map(m => m.NetPaid).Index(99);//	,[Paid Amount]
            Map(m => m.RelationshipToSubscriberCode).Index(30).TypeConverter(new RelationshipToSubscriberCodeConverter());//	,Case when[Member''s Relationship to Employee] = ''S'' then 1 else 0 End
            //Livongo Section
            Map(m => m.LvDiagnosisCode1).Index(152);//	,[Diagnosis Code 1]
            Map(m => m.LvDiagnosisCode2).Index(153);//	,[Diagnosis Code 2]
            Map(m => m.LvDiagnosisCode3).Index(154);//	,[Diagnosis Code 3]
            Map(m => m.LvDiagnosisCode4).Index(155);//	,[Diagnosis Code 4]
            Map(m => m.LvDiagnosisCode5).Index(156);//	,[Diagnosis Code 5]
            Map(m => m.LvDiagnosisCode6).Index(157);//	,[Diagnosis Code 6]
            Map(m => m.LvDiagnosisCode7).Index(158);//	,[Diagnosis Code 7]
            Map(m => m.LvDiagnosisCode8).Index(159);//	,[Diagnosis Code 8]
            Map(m => m.LvDiagnosisCode9).Index(160);//	,[Diagnosis Code 7]
            Map(m => m.LvDiagnosisCode10).Index(161);//	,[Diagnosis Code 8]
            Map(m => m.PaidDate).Index(61);//Date Processed (All)
            Map(m => m.CoverageStartDate).Index(59);//Date Service Started
            Map(m => m.CoverageEndDate).Index(60);//Date Service Stopped
            Map(m => m.ClaimId).Index(32);//Source-Specific Transaction ID Number
            Map(m => m.TypeOfCoverage).Index(9);//General Category of Health Plan
            Map(m => m.GroupId).Index(1);//Hierarchy Level 2
            Map(m => m.MemberNumber).Index(25);//Member's ID (Assigned in Data Warehouse)
            Map(m => m.MemberGender).ConvertUsing(row => MapGender(row.GetField(29)));//Member's Gender(M = male, F = female or U = unknown.)
            Map(m => m.Diagnosis1).Index(152);//	,[Diagnosis Code 1]
            Map(m => m.Diagnosis2).Index(153);//	,[Diagnosis Code 2]
            Map(m => m.Diagnosis3).Index(154);//	,[Diagnosis Code 3]
            Map(m => m.ProcedureCode1).Index(67);//	[Line-Level Procedure Code (CPT, HCPCS, ADA, CDT)]
            Map(m => m.RevenueCode).Index(67);//	[Line-Level Procedure Code (CPT, HCPCS, ADA, CDT)]
            Map(m => m.PatientPayAmount).ConvertUsing(row => PatientPaymentAmount(row.GetField(93), row.GetField(96)));//[Copayment Amount] + [Coinsurance]
        }

        decimal? PatientPaymentAmount(string coPaymentAmount, string coInsurance)
        {
            return Convert.ToDecimal(coPaymentAmount ?? "0") + Convert.ToDecimal(coInsurance ?? "0");
        }
        ClaimsProcessGender MapGender(string gender)
        {
            if (String.IsNullOrEmpty(gender) || gender == "U")
                return ClaimsProcessGender.U;
            else if (gender == "M")
                return ClaimsProcessGender.M;
            else if (gender == "F")
                return ClaimsProcessGender.F;
            return ClaimsProcessGender.U;
        }
    }
}
