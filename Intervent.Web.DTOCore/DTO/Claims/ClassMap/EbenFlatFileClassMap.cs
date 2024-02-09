using CsvHelper.Configuration;
using Intervent.Web.DTO.DTO.Claims.CustomConverter;

namespace Intervent.Web.DTO
{
    public sealed class EbenFlatFileClassMap : CsvClassMap<EbenFlatFileModel>
    {
        static readonly string dateFormat = "yyyyMMdd";

        public EbenFlatFileClassMap()
        {
            Map(m => m.MemberDateOfBirth).Name("BIRTH_DATE").TypeConverter(new DateFormatConverter(dateFormat));
            Map(m => m.ServiceStartDate).Name("DOS_FROM").TypeConverter(new DateFormatConverter(dateFormat));
            Map(m => m.DiagnosisCode).Name("ICD-10_Code");
            Map(m => m.ProcedureCode).Name("PRC_CODE");
            Map(m => m.BilledAmount).Name("AMOUNT_CHARGED");
            Map(m => m.Copay).Name("COPAY_AMOUNT");
            Map(m => m.Deductible).Name("DEDUCTIBLE_AMOUNT");
            Map(m => m.Coinsurance).Name("CO_INSURANCE_AMOUNT");
            Map(m => m.NetPaid).Name("AMOUNT_PAID");
            Map(m => m.RelationshipToSubscriberCode).Name("RELATIONSHIP").TypeConverter(new EbenCustomConverter());
            Map(m => m.ClaimId).Name("CLAIM_REF");
            Map(m => m.PaidDate).Name("PAID_DATE").TypeConverter(new DateFormatConverter(dateFormat));
            Map(m => m.GroupId).Name("GRP_NUM");
            Map(m => m.UniqueId).Name("UniqueID");
            //Map(m => m.PatientPayAmount).ConvertUsing(row => PatientPaymentAmount(row.GetField(28), row.GetField(25), row.GetField(30)));//[Copay Amount] + [Deductible Amount] + [Coinsurance Amount]
        }

        decimal? PatientPaymentAmount(string coPaymentAmount, string coInsurance, string deductible)
        {
            return Convert.ToDecimal(coPaymentAmount ?? "0") + Convert.ToDecimal(coInsurance ?? "0") + Convert.ToDecimal(deductible ?? "0");
        }
    }
}
