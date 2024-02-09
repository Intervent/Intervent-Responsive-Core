namespace Intervent.Web.DTO
{
    public sealed class LivongoOutputModel
    {
        const string dateFormat = "yyyy-MM-dd";
        public string Company { get; set; }//OrgName
        public string Carrier { get; set; }//CarrierName
        public string AssociateID { get; set; }//AssociateId
        public string Claim_ID { get; set; }//ClaimId
        public string Date_of_Service { get; set; }//Service Start Date
        public string Paid_Date { get; set; }//Paid Date
        public string Coverage_Start_Date { get; set; }//Coverage Start Date
        public string Coverage_End_Date { get; set; }//Coverage End Date
        public string Type_of_Coverage { get; set; }//TypeOfCoverage
        public string Member_First_Name { get; set; }//MemberFirstName
        public string Member_Last_Name { get; set; }//MemberLastName
        public string Group_ID { get; set; }//GroupId
        public string SSN { get; set; }
        public string Member_Number { get; set; }//MemberNumber
        public string Rlnshp_to_Subscriber_Code { get; set; }//RelationshipToSubscriberCode
        public string Member_Gender { get; set; }
        public string Member_Date_of_Birth { get; set; }
        public string Member_Phone { get; set; }
        public string Member_Email { get; set; }
        public string Member_Address1 { get; set; }
        public string Member_Address2 { get; set; }
        public string Member_City { get; set; }
        public string Member_State { get; set; }
        public string Member_Zip { get; set; }
        public string Diagnosis1 { get; set; }//Diagnosis1
        public string Diagnosis2 { get; set; }//Diagnosis2
        public string Diagnosis3 { get; set; }//Diagnosis3
        public string Procedure_Code { get; set; }//ProcedureCode
        public string Procedure_Code1 { get; set; }//ProcedureCode1
        public string Revenue_Code { get; set; }//RevenueCode
        public string Amount_Billed { get; set; }//AmountBilled
        public string Total_Amount_Paid { get; set; }//TotalAmountPaid
        public string Member_Amount_Paid { get; set; }//MemberAmountPaid
        public string Drug_ID_NDC { get; set; }//NDCDrugId
        public string Drug_Name { get; set; }//DrugName
        public string Drug_Strength { get; set; }//DrugStrength
        public string Total_Days_Supply { get; set; }//TotalDaysSupply
        public string Total_Dispensed_Quantity { get; set; }//TotalDispensedQuantity
        public string Member_Cost { get; set; }//MemberCost
        public string MedPlanTerminationDate { get; set; }//MedPlanTerminationDate
        public string TerminatedStatus { get; set; }//TerminatedStatus
        public string LastDateProcessed { get; set; }//LastDateProcessed
        public string LvCode1 { get; set; }//LvDiagnosisCode1
        public string LvCode2 { get; set; }//LvDiagnosisCode2
        public string LvCode3 { get; set; }//LvDiagnosisCode3
        public string LvCode4 { get; set; }//LvDiagnosisCode4
        public string LvCode5 { get; set; }//LvDiagnosisCode5
        public string LvCode6 { get; set; }//LvDiagnosisCode6
        public string LvCode7 { get; set; }//LvDiagnosisCode7
        public string LvCode8 { get; set; }//LvDiagnosisCode8
        public string LvCode9 { get; set; }//LvDiagnosisCode9
        public string LvCode10 { get; set; }//LvDiagnosisCode10
        public string LcoRef { get; set; }//LcoRef
        public string Business_Unit { get; set; }//BusinessUnit
    }
}
