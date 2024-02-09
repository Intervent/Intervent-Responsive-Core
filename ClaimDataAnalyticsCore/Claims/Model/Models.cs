using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimDataAnalytics.Claims.Model
{
    public class ClaimProcessEligibilityDto
    {
        public int EligibilityId { get; set; }
        public string UniqueID { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string SSN { get; set; }

        public string UserEnrollmentType { get; set; }

        public string MedicalPlanCode { get; set; }

    }

    public class CrothalIdDto
    {
        public string OldUniqueId { get; set; }

        public string NewUniqueId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}
