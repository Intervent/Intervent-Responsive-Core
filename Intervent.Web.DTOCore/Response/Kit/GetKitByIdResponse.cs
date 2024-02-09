namespace Intervent.Web.DTO
{
    public class GetKitByIdResponse
    {
        public KitsDto EduKit { get; set; }

        public List<PageIdentifier> PageIdentifier { get; set; }

        public List<Step> StepNames { get; set; }

        public int KitsInUserProgramId { get; set; }

        public bool ListenedAudio { get; set; }

        public int PercentCompleted { get; set; }

        public int TotalPages { get; set; }

        public string KitColor { get; set; }

        public string LanguageCode { get; set; }

        public bool success { get; set; }

        public bool preview { get; set; }

        public int? index { get; set; }

        public int? UserinProgramId { get; set; }

        public string pageIdentifier { get; set; }

        public bool ConatinsIdentifier(string pageId)
        {
            return (this.PageIdentifier.FindAll(pd => pd.PageId.Equals(pageId)).Count > 0);
        }

        public KitsinUserProgramGoalDto KitsinUserProgramGoal { get; set; }
    }

    public class PageIdentifier
    {
        public string PageId { get; set; }
        public string RootIdentifier { get; set; }

        public PageIdentifier(string pageId)
        {
            PageId = pageId;
        }

        public PageIdentifier(string pageId, string rootId)
        {
            PageId = pageId;
            RootIdentifier = rootId;
        }
    }

    public class Step
    {
        public string StepName { get; set; }

        public string PageIdentifier { get; set; }

        public bool HideLink { get; set; }

        public bool IsAppendix { get; set; }

        public int Index { get; set; }

        public string NameLangCode { get; set; }

        public Step(string name, bool appendix, string langCode)
        {
            StepName = name;
            IsAppendix = appendix;
            NameLangCode = langCode;
        }

        public Step(string name, string identifier, int index, string langCode)
        {
            StepName = name;
            PageIdentifier = identifier;
            Index = index;
            NameLangCode = langCode;
        }
    }
}
