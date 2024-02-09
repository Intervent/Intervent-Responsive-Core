using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimDataAnalytics.Claims.DB
{
    [Table("CrothalIDChanges")]
    public class DboCrothalId
    {
        [Key]
        public string OldUniqueId { get; set; }

        public string NewUniqueId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    [Table("Eligibility")]
    public class DboEligibility
    {
        [Key]
        public int Id { get; set; }
        public string UniqueID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Column("DOB")]
        public DateTime? DateOfBirth { get; set; }

        public string SSN { get; set; }

        public string UserEnrollmentType { get; set; }

        public string CompanyName { get; set; }

        public DateTime CreateDate { get; set; }

        public string MedicalPlanCode { get; set; }

    }

    [Table("ExcludeId")]
    public class DboExcludeId
    {
        [Key]
        public int EligibilityId { get; set; }
    }
}
