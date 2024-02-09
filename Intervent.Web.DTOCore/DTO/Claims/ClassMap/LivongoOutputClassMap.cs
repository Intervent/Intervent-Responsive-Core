using CsvHelper.Configuration;
using Intervent.Web.DTO.DTO.Claims.CustomConverter;

namespace Intervent.Web.DTO
{
    public sealed class LivongoOutputClassMap : CsvClassMap<ClaimsInputFlatFileModel>
    {
        const string dateFormat = "yyyy-MM-dd";
        public LivongoOutputClassMap()
        {
            int i = 0;
            Map(m => m.OrgName).Index(i++).Name("Company");//Company
            Map(m => m.ProviderName).Index(i++).Name("Carrier");//Carrier
            Map(m => m.UniqueId).Index(i++).Name("AssociateID");//AssociateID
            Map(m => m.ClaimId).Index(i++).Name("Claim_ID");//Claim_ID

            Map(m => m.DateOfService).Index(i++).Name("Date_of_Service").TypeConverter(new DateToStringFormatConverter(dateFormat));//Date_of_Service
            Map(m => m.PaidDate).Index(i++).Name("Paid_Date").TypeConverter(new DateToStringFormatConverter(dateFormat));//Paid_Date
            Map(m => m.CoverageStartDate).Index(i++).Name("Coverage_Start_Date").TypeConverter(new DateToStringFormatConverter(dateFormat));//Coverage_Start_Date
            Map(m => m.CoverageEndDate).Index(i++).Name("Coverage_End_Date").TypeConverter(new DateToStringFormatConverter(dateFormat));//Coverage_End_Date
            Map(m => m.TypeOfCoverage).Index(i++).Name("Type_of_Coverage");//Type_of_Coverage
            Map(m => m.MemberFirstName).Index(i++).Name("Member_First_Name");//Member_First_Name
            Map(m => m.MemberLastName).Index(i++).Name("Member_Last_Name");//Member_Last_Name
            Map(m => m.GroupId).Index(i++).Name("Group_ID");//Group_ID
            Map(m => m.MemberSSN).Index(i++).Name("SSN");//SSN
            Map(m => m.MemberNumber).Index(i++).Name("Member_Number");//Member_Number

            Map(m => m.RelationshipToSubscriberCode).Index(i++).Name("Rlnshp_to_Subscriber_Code");//Rlnshp_to_Subscriber_Code
            Map(m => m.MemberGender).Index(i++).Name("Member_Gender");//Member_Gender

            Map(m => m.MemberDateOfBirth).Index(i++).Name("Member_Date_of_Birth").TypeConverter(new DateToStringFormatConverter(dateFormat));//Member_Date_of_Birth
            Map(m => m.MemberPhone).Index(i++).Name("Member_Phone");//Member_Phone
            Map(m => m.MemberEmail).Index(i++).Name("Member_Email");//Member_Email
            Map(m => m.MemberAddress1).Index(i++).Name("Member_Address1");//Member_Address1
            Map(m => m.MemberAddress2).Index(i++).Name("Member_Address2");//Member_Address2
            Map(m => m.MemberCity).Index(i++).Name("Member_City");//Member_City
            Map(m => m.MemberState).Index(i++).Name("Member_State");//Member_State
            Map(m => m.MemberZip).Index(i++).Name("Member_Zip");//Member_Zip
            Map(m => m.Diagnosis1).Index(i++).Name("Diagnosis1");//Diagnosis1
            Map(m => m.Diagnosis2).Index(i++).Name("Diagnosis2");//Diagnosis2
            Map(m => m.Diagnosis3).Index(i++).Name("Diagnosis3");//Diagnosis3
            Map(m => m.ProcedureCode).Index(i++).Name("Procedure_Code");//Procedure_Code
            Map(m => m.ProcedureCode1).Index(i++).Name("Procedure_Code1");//Procedure_Code1
            Map(m => m.RevenueCode).Index(i++).Name("Revenue_Code");//Revenue_Code
            Map(m => m.BilledAmount).Index(i++).Name("Amount_Billed");//Amount_Billed
            Map(m => m.NetPaid).Index(i++).Name("Total_Amount_Paid");//Total_Amount_Paid
            Map(m => m.PatientPayAmount).Index(i++).Name("Member_Amount_Paid");//Member_Amount_Paid
            Map(m => m.NDCDrugId).Index(i++).Name("Drug_ID_NDC");//Drug_ID_NDC
            Map(m => m.DrugName).Index(i++).Name("Drug_Name");//Drug_Name
            Map(m => m.DrugStrength).Index(i++).Name("Drug_Strength");//Drug_Strength

            Map(m => m.TotalDaysSupply).Index(i++).Name("Total_Days_Supply");//Total_Days_Supply
            Map(m => m.TotalDispensedQuantity).Index(i++).Name("Total_Dispensed_Quantity");//Total_Dispensed_Quantity
            Map(m => m.MemberCost).Index(i++).Name("Member_Cost");//Member_Cost
            Map(m => m.MedPlanTerminationDate).Index(i++).Name("MedPlanTerminationDate").TypeConverter(new DateToStringFormatConverter(dateFormat));//MedPlanTerminationDate
            Map(m => m.TerminatedStatus).Index(i++).Name("TerminatedStatus");//TerminatedStatus
            Map(m => m.LastDateProcessed).Index(i++).Name("LastDateProcessed");//LastDateProcessed
        }
    }
}
