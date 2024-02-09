namespace Intervent.Web.DTO.AWV
{
    /// <summary>
    /// Annual Wellness Visit General Information
    /// </summary>
    public class AnnulWellnessVisitGeneralInfoDto
    {
        /// <summary>
        /// Mandatory: Assessment Date (UTC Format)
        /// </summary>
        public DateTime AssessmentDate { get; set; }

        /// <summary>
        /// Mandatory: FirstName of the patient
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Mandatory: LastName of the patient
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Mandatory: Date of Birth of the patient (UTC Format)
        /// </summary>
        public DateTime DOB { get; set; }

        /// <summary>
        /// Mandatory: PhoneNumber of the patient
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Mandatory: Name of the Staff conducted the exam
        /// </summary>
        public string StaffName { get; set; }

        /// <summary>
        /// Optional: Last Assessment Date (UTC Format)
        /// </summary>
        public DateTime? DateOfLastAssessment { get; set; }

        /// <summary>
        /// Optional: Medicate Eligibility Date (UTC Format)
        /// </summary>
        public DateTime? MedicareEligibilityDate { get; set; }

        /// <summary>
        /// Optional : Is Initial Preventive Physical Exam
        /// </summary>
        public bool? IsIPPE { get; set; }

        /// <summary>
        /// Optional : Is Anual Wellness Visit
        /// </summary>
        public bool? IsAWV { get; set; }

        /// <summary>
        /// Optional : Is Subsequent Wellness Visit
        /// </summary>
        public bool? IsSubsequentAWV { get; set; }

        /// <summary>
        /// Mandatory: 1. Male 2. Female
        /// </summary>
        public byte Gender { get; set; }

        /// <summary>
        /// Optional: 1. Caucasian 2. African American 3.American Indian/Native American 4. Asian 5. Hispanic 6. Pacific Islander 7. Other
        /// </summary>
        public byte? Race { get; set; }

        /// <summary>
        /// Patient's Address.AddressLine1
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Patient's Address.City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Patient's Address.Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Patient's Address.ZipCode
        /// </summary>        
        public string Zip { get; set; }

        /// <summary>
        /// Patient's Address.State
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Practice Name
        /// </summary>
        public string PracticeName { get; set; }

        /// <summary>
        /// Practice Address
        /// </summary>
        public string PracticeAddress { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validationResults)
        {
            if (string.IsNullOrEmpty(FirstName))
                validationResults.Add("FirstName", "Mandatory");
            if (string.IsNullOrEmpty(LastName))
                validationResults.Add("LastName", "Mandatory");
            if (AssessmentDate == DateTime.MinValue)
                validationResults.Add("AssessmentDate", "Mandatory");
            if (DOB == DateTime.MinValue)
                validationResults.Add("DOB", "Mandatory");
            if (string.IsNullOrEmpty(StaffName))
                validationResults.Add("StaffName", "Mandatory");
            if (string.IsNullOrEmpty(PhoneNumber))
                validationResults.Add("PhoneNumber", "Mandatory");
            ValidationUtility.Validate(Gender, 1, 2, "Gender", validationResults);
            ValidationUtility.Validate(Race, 1, 7, "Race", validationResults);
            return validationResults;

        }

    }
}
