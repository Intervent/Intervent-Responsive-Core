using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class SpouseEligibilityModel
    {
        public string Address { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public DateTime? DOB { get; set; }

        public string Email { get; set; }

        public bool? hasEmail { get; set; }

        public string EmployeeUniqueId { get; set; }

        public string UniqueId { get; set; }

        public string FirstName { get; set; }

        public int Gender { get; set; }

        public string HomeNumber { get; set; }

        public string LastName { get; set; }

        public string State { get; set; }

        public string WorkNumber { get; set; }

        public string Zip { get; set; }

        public int PrimaryEligibilityId { get; set; }

        public string DateFormat { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }

    }
}