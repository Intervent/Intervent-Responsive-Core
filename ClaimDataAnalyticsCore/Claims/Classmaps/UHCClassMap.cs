using ClaimDataAnalytics.Claims.Converters;
using ClaimDataAnalytics.Claims.Converters.UHC;
using ClaimDataAnalytics.Claims.Model;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimDataAnalytics.Claims.Classmaps
{
    public sealed class UHCClassMap : CsvClassMap<ClaimsInputFlatFileModel>
    {
        static readonly string dateFormat = "yyyyMMdd";
        public UHCClassMap()
        {
            Map(m => m.MemberSSN).Name("Employee ID").TypeConverter(new SSNConverter());//case when len(ltrim(rtrim([Employee ID])))=11 then RIGHT(ltrim(rtrim([Employee ID])),9) else ltrim(rtrim([Employee ID])) End
            Map(m => m.MemberFirstName).Name("Member First Name");//,[Member First Name]
            Map(m => m.MemberLastName).Name("Member Last Name");//,[Member Last Name]
            Map(m => m.MemberDateOfBirth).Name("Member Date of Birth").TypeConverter(new DateFormatConverter(dateFormat));//,ltrim(rtrim([Member Date of Birth]))
            Map(m => m.ServiceStartDate).Name("Date Of Service From").TypeConverter(new DateFormatConverter(dateFormat));//,[Date Of Service From]
            Map(m => m.ServiceEndDate).Name("Date of Service to").TypeConverter(new DateFormatConverter(dateFormat));//,[Date of Service to]
            //Map(m => m.PrimaryDiagnosisCode).Name("Primary Diagnosis Code");//,[Primary Diagnosis Code]
            //Map(m => m.DiagnosisCode2).Name("Secondary Diagnosis Code");//,[Secondary Diagnosis Code]
            //Map(m => m.DiagnosisCode3).Name("Tertiary Diagnosis Code");//,[Tertiary Diagnosis Code]
            //Map(m => m.DiagnosisCode5).Name("ICD Version Code");//,[ICD Version Code]
            //Map(m => m.DiagnosisCode6).Name("Revenue Code-1");//,[Revenue Code-1]
            //Map(m => m.DiagnosisCode7).Name("Service Code");//, [Service Code]
            //Map(m => m.DiagnosisCode8).Name("Service Type Code");//,[Service Type 9Code]
            Map(m => m.AllowedAmount).Name("Covered Amount");//,[Covered Amount]
            Map(m => m.Copay).Name("Copay Amount");//,[Copay Amount]
            Map(m => m.Deductible).Name("Deductible Amount");//,[Deductible Amount],
            Map(m => m.Coinsurance).Name("Coinsurance Amount");//,[Coinsurance Amount]
            Map(m => m.NetPaid).Name("Net Paid Amount");//,[Net Paid Amount]
            Map(m => m.RelationshipToSubscriberCode).Name("Member Relationship Code").TypeConverter(new RelationshipToSubscriberCodeConverter());//,Case when ltrim(rtrim([Member Relationship Code])) = ''SP'' then 1 else 0 end
            //Map(m => m.GenericDrug).Name("Generic Name");
            //Map(m => m.TheraClassCode).Name("Therapeutic Class Code - AHFS");
            //Livongo
            //Map(m => m.LvDiagnosisCode1).Name("Primary Diagnosis Code");//,[Primary Diagnosis Code]
            //Map(m => m.LvDiagnosisCode2).Name("Secondary Diagnosis Code");//,[Secondary Diagnosis Code]
            //Map(m => m.LvDiagnosisCode3).Name("Tertiary Diagnosis Code");//,[Tertiary Diagnosis Code]
            //Map(m => m.ClaimId).Name("Claim Reference Number");//Claim Reference Number
            //Map(m => m.CoverageStartDate).Name("Date of Service From").TypeConverter(new DateFormatConverter(dateFormat));//Date of Service From
            //Map(m => m.CoverageEndDate).Name("Date of Service To").TypeConverter(new DateFormatConverter(dateFormat));//Date of Service To
            //Map(m => m.GroupId).Name("Report Code");//Report Code
            Map(m => m.MemberNumber).Name("Employee ID");//Employee ID
            Map(m => m.MemberGender).ConvertUsing(row => MapGender(row.GetField("Member Sex")));//Member Sex
            //Map(m => m.Diagnosis1).Name("Primary Diagnosis Code");//,[Primary Diagnosis Code]
            //Map(m => m.Diagnosis2).Name("Secondary Diagnosis Code");//,[Secondary Diagnosis Code]
            //Map(m => m.Diagnosis3).Name("Tertiary Diagnosis Code");//,[Tertiary Diagnosis Code]
            //Map(m => m.ProcedureCode).Name("Service Code");//Service Code
            //Map(m => m.RevenueCode).Name("Revenue Code-1");//Revenue Code-1
            //Map(m => m.PatientPayAmount).Name("Net Paid Amount");//,[Net Paid Amount]
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
