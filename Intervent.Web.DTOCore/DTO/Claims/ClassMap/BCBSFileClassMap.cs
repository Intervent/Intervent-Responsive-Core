using CsvHelper.Configuration;
using Intervent.Web.DTO.DTO.Claims.CustomConverter;
using Intervent.Web.DTO.DTO.Claims.CustomConverter.BCBS;

namespace Intervent.Web.DTO
{
    public sealed class BCBSFileClassMap : CsvClassMap<ClaimsInputFlatFileModel>
    {
        static readonly string dateFormat = "yyyyMMdd";
        public BCBSFileClassMap()
        {
            Map(m => m.MemberSSN).Index(6);//ltrim(rtrim(isnull([Member SSN],'''')))
            Map(m => m.MemberFirstName).Index(7);//,[Member First Name]
            Map(m => m.MemberLastName).Index(9);//,[Member Last Name] BCBS doc has middle name after last name but in the file it is reverse
            Map(m => m.MemberDateOfBirth).Index(13).TypeConverter(new DateFormatConverter(dateFormat));//,ltrim(rtrim([Member Date of Birth]))
            Map(m => m.ServiceStartDate).Index(52).TypeConverter(new DateFormatConverter(dateFormat)); //,[Service Start Date]
            Map(m => m.PrimaryDiagnosisCode).Index(39);//,[Primary Diagnosis Code]
            Map(m => m.DiagnosisCode2).Index(40);//,[Diagnosis Code 2]
            Map(m => m.DiagnosisCode3).Index(41);//,[Diagnosis Code 3]
            Map(m => m.DiagnosisCode4).Index(42);//,[Diagnosis Code 4]
            Map(m => m.DiagnosisCode5).Index(47);//,[ICD9 Procedure Code 1]
            Map(m => m.DiagnosisCode6).Index(48);//,[ICD9 Procedure Code 2]
            Map(m => m.DiagnosisCode7).Index(49);//,[ICD9 Procedure Code 3]
            Map(m => m.DiagnosisCode8).Index(45);//,[Procedure Code]
            Map(m => m.BilledAmount).Index(63);//,[CHARGED AMOUNT]
            Map(m => m.Copay).Index(65);//,[COPAY AMOUNT]
            Map(m => m.Deductible).Index(66);//,[DEDUCTIBLE AMOUNT]
            Map(m => m.Coinsurance).Index(67);//,[COINSURANCE AMOUNT]
            Map(m => m.NetPaid).Index(71);//,[PAID AMOUNT]
                                          //  Map(m => m.IsSpouse).Index(11).TypeConverter(new RelationshipToSubscriberCodeConverter());//,Case when[Rlnshp to Subscriber Code] = ''SPS'' then 1 else 0 End
            Map(m => m.RelationshipToSubscriberCode).Index(11).TypeConverter(new RelationshipToSubscriberCodeConverter());
            //Livongo codes
            Map(m => m.LvDiagnosisCode1).Index(39);//,[Primary Diagnosis Code]
            Map(m => m.LvDiagnosisCode2).Index(40);//,[Diagnosis Code 2]
            Map(m => m.LvDiagnosisCode3).Index(41);//,[Diagnosis Code 3]
            Map(m => m.ClaimId).Index(37);//Claim Number
            Map(m => m.CoverageStartDate).Index(20).TypeConverter(new DateFormatConverter(dateFormat));//Member Effective Date
            Map(m => m.PaidDate).Index(51).TypeConverter(new DateFormatConverter(dateFormat));//Claim Process Date
            Map(m => m.CoverageEndDate).Index(21).TypeConverter(new DateFormatConverter(dateFormat));//Member Termination Date
            Map(m => m.GroupId).Index(0);//Subgroup Number. It is 0
            Map(m => m.MemberNumber).Index(5);//Member Number
            Map(m => m.MemberGender).ConvertUsing(row => MapGender(row.GetField(12)));//Member Gender
            Map(m => m.ProcedureCode).Index(45);//Procedure Code
            Map(m => m.ProcedureCode1).Index(47);//ICD9 Procedure Code 1
                                                 // Map(m => m.BilledAmount).Index(0);//[Charged Amount]
                                                 // Map(m => m).Index(0);//[Paid Amount]
            Map(m => m.PatientPayAmount).ConvertUsing(row => PatientPaymentAmount(row.GetField(65), row.GetField(67), row.GetField(66)));//[Copay Amount] + [Deductible Amount] + [Coinsurance Amount]
            Map(m => m.Diagnosis1).Index(39);//,[Primary Diagnosis Code]
            Map(m => m.Diagnosis2).Index(40);//,[Diagnosis Code 2]
            Map(m => m.Diagnosis3).Index(41);//,[Diagnosis Code 3]
            Map(m => m.GenericDrug).Index(87);
            Map(m => m.TheraClassCode).Index(88);
        }

        decimal? PatientPaymentAmount(string coPaymentAmount, string coInsurance, string deductible)
        {
            return Convert.ToDecimal(coPaymentAmount ?? "0") + Convert.ToDecimal(coInsurance ?? "0") + Convert.ToDecimal(deductible ?? "0");
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
