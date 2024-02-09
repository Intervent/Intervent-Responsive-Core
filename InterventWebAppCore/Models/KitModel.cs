using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class KitModel
    {
        public int Id { get; set; }

        public string DateFormat { get; set; }
    }

    public class ActivityModel
    {
        public int KitId { get; set; }

        public int StepId { get; set; }

        public int ActivityId { get; set; }

        public IEnumerable<SelectListItem> LanguageList { get; set; }
    }

    public class QuizModel
    {
        public int stepId { get; set; }
        public IEnumerable<SelectListItem> LanguageList { get; set; }
    }

    public class PromptModel
    {
        public PromptDto Prompt { get; set; }
    }

    public class PromptsinKitsCompletedModel
    {
        public PromptsinKitsCompletedDto CompletePromptsinKit { get; set; }
    }

    public class CompletedKitsModel
    {
        public IList<KitsinUserProgramDto> KitsinUserPrograms { get; set; }
    }

    public class AssignKitModel
    {
        public int KitId { get; set; }

        public string OrganizationIds { get; set; }
    }
}