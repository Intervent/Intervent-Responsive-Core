using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class TeamsBP_PPRModel
    {
        public IList<MeasurementsDto> Measurements { get; set; }

        public TeamsBP_PPRDto TeamsBP_PPR { get; set; }

        public IEnumerable<SelectListItem> UnableToMonitorBPList { get; set; }

        public IEnumerable<SelectListItem> FactorsInMonitoringBPList { get; set; }

        public IEnumerable<SelectListItem> NotTakingMedicationsList { get; set; }

        public IEnumerable<SelectListItem> AchievePAGoalList { get; set; }

        public IEnumerable<SelectListItem> UnableToAchievePAGoalList { get; set; }

        public IEnumerable<SelectListItem> UnableToFollowHDGoalList { get; set; }

        public IEnumerable<SelectListItem> InjuryList { get; set; }

        public IEnumerable<SelectListItem> ExerIntList { get; set; }

        public IEnumerable<SelectListItem> UnableToAttendTherapyList { get; set; }

        public IEnumerable<SelectListItem> Ratings { get; set; }

        public bool IsAlcoholicUser { get; set; }

        public bool IsTobaccoUser { get; set; }

        public bool IsReadOnly { get; set; }

        public string WeightText { get; set; }

        public string NotMonitoredReason { get; set; }

        public string MonitoredBPHelpful { get; set; }

        public string ReasonNotTakingMed { get; set; }

        public string YAcheivedGoal { get; set; }

        public string YNotAcheivedGoal { get; set; }

        public string ReasonNotFollowedLSD { get; set; }

        public string HowSeriousWasInjury { get; set; }

        public string NotAttendingReason { get; set; }

        public int userId { get; set; }

        public string dataFormat { get; set; }
    }
}