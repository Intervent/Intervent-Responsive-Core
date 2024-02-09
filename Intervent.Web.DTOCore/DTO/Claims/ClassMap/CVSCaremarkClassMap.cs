using CsvHelper.Configuration;
using Intervent.Web.DTO.DTO.Claims.CustomConverter;
using Intervent.Web.DTO.DTO.Claims.CustomConverter.CVSCaremark;

namespace Intervent.Web.DTO
{
    public sealed class CVSCaremarkClassMap : CsvClassMap<ClaimsInputFlatFileModel>
    {
        public CVSCaremarkClassMap()
        {
            Map(m => m.MemberFirstName).Index(27);//,[Patient First Name]
            Map(m => m.MemberLastName).Index(26);//,[Patient Last Name]
            Map(m => m.MemberDateOfBirth).Index(35).TypeConverter(new DateFormatConverter("yyyyMMdd"));//,[Patient Date of Birth]
            Map(m => m.ServiceStartDate).Index(97).TypeConverter(new DateFormatConverter("yyyyMMdd"));//,[Date Of Service]
            Map(m => m.PrimaryDiagnosisCode).Index(132);//,[Diagnosis Code]

            Map(m => m.GenericDrug).Index(189);//,[Generic Name]
            Map(m => m.DrugName).Index(189);//,[Generic Name]
            Map(m => m.TheraClassCode).Index(211);//,[Therapeutic Class Code - AHFS]
            //having the X fields temp to validate the amount conversion
            Map(m => m.TotalAmountPaidbyAllSourceX).Index(219);//,[Total Amount Paid by All Source]
            Map(m => m.PatientPayAmountX).Index(221);//,[Patient Pay Amount]
            Map(m => m.AmountofCopayX).Index(222);//,[Amount of Copay]
            Map(m => m.AmountofCoinsuranceX).Index(223);//,[Amount of Coinsurance]
            Map(m => m.NetAmountDueX).Index(257);//,[Net Amount Due (Total Amount Billed-Paid)]

            Map(m => m.TotalAmountPaidbyAllSource).Index(219).TypeConverter(new AmountConverter());//,[Total Amount Paid by All Source]
            Map(m => m.PatientPayAmount).Index(221).TypeConverter(new AmountConverter());//,[Patient Pay Amount]
            Map(m => m.AmountofCopay).Index(222).TypeConverter(new AmountConverter());//,[Amount of Copay]
            Map(m => m.AmountofCoinsurance).Index(223).TypeConverter(new AmountConverter());//,[Amount of Coinsurance]
            Map(m => m.NetAmountDue).Index(257).TypeConverter(new AmountConverter());//,[Net Amount Due (Total Amount Billed-Paid)]

            Map(m => m.RelationshipToSubscriberCode).Index(37).TypeConverter(new RelationshipToSubscriberCodeConverter());//,Case	when rtrim(ltrim([Eligibility-Patient Relationship Code]))=''18'' then 0	when rtrim(ltrim([Eligibility-Patient Relationship Code]))=''01'' then 1	else 0	End				
            //Map(m => m.UniqueId).Index().TypeConverter(new ());//,left(isnull([Cardholder ID],''''),8)+case	when rtrim(ltrim([Eligibility-Patient Relationship Code]))=''18'' then ''EH9''	when rtrim(ltrim([Eligibility-Patient Relationship Code]))=''01'' then ''EH8''	else ''XXX''	End
            Map(m => m.UniqueId).ConvertUsing(row => GetUniqueId(row.GetField(10), row.GetField(37)));
            //Livongo
            //Product/Service ID Qualifier - 03 is NDC
            Map(m => m.NDCDrugId).ConvertUsing(row => GetNDCDrugId(row.GetField(95), row.GetField(96)));//,[Product-Service ID]
            Map(m => m.ClaimId).Index(305);//Transaction ID
            Map(m => m.DateOfService).Index(97).TypeConverter(new DateFormatConverter("yyyyMMdd"));//,[Date Of Service]
            Map(m => m.PaidDate).Index(98).TypeConverter(new DateFormatConverter("yyyyMMdd")); ;//Adjudication Date
            Map(m => m.GroupId).Index(43);//Group ID
            Map(m => m.MemberNumber).Index(10);//Cardholder ID
            Map(m => m.MemberGender).ConvertUsing(row => MapGender(row.GetField(36)));//Patient Gender Code 1 = Male, 2 = Female, 0 = Not Specified
            Map(m => m.DrugStrength).Index(190);//Product Strength
            Map(m => m.TotalDaysSupply).Index(127).TypeConverter(new AmountConverter());//Original Day Supply(Pharmacy submitted days supply.  If any reductions or corrections to day supply take place, refer to field 117 Day Supply.  This field remains fixed.)
            Map(m => m.TotalDispensedQuantity).Index(125).TypeConverter(new AmountConverter());//Original Quantity(The original input quantity.  If any reductions or corrections to quantity take place, refer to field 115 Quantity Dispensed.  This field remains fixed.  Submitted on the claim by the pharmacy in metric units.)
            Map(m => m.MemberCost).Index(230).TypeConverter(new AmountConverter());//Out of Pocket Apply Amount(Amount applied to out of pocket expense. Format = s$$$$$$ccs9(6)v99)
            Map(m => m.Diagnosis1).Index(132);//,[Diagnosis Code]
        }

        static string GetNDCDrugId(string productIdQualifier, string productId)
        {
            if (!String.IsNullOrEmpty(productIdQualifier) && productIdQualifier == "03")
                return productId;
            return null;
        }

        static string GetUniqueId(string cardholderId, string relationshipToSubject)
        {
            if (String.IsNullOrEmpty(cardholderId))
                return null;
            var tmpCardholderId = cardholderId;
            if (cardholderId.Length >= 8)
            {
                tmpCardholderId = cardholderId.Substring(0, 8);
            }
            if (String.IsNullOrEmpty(relationshipToSubject))
                return tmpCardholderId + "XXX";
            else if (relationshipToSubject == "18")
                return tmpCardholderId + "EH9";
            else if (relationshipToSubject == "01")
                return tmpCardholderId + "EH8";
            else
                return tmpCardholderId + "XXX";


        }

        ClaimsProcessGender MapGender(string gender)
        {
            if (String.IsNullOrEmpty(gender) || gender == "0")
                return ClaimsProcessGender.U;
            else if (gender == "1")
                return ClaimsProcessGender.M;
            else if (gender == "2")
                return ClaimsProcessGender.F;
            return ClaimsProcessGender.U;
        }
    }
}
