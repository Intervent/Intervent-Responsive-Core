using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp.Models
{
    public class EligibilitySearchModel
    {
        public IEnumerable<SelectListItem> TrackingModes { get; set; }

        public IEnumerable<SelectListItem> DiagnosisCodes { get; set; }

        public IEnumerable<SelectListItem> OrganizationList { get; set; }

        public string DateFormat { get; set; }

        public string Org { get; set; }
    }
}