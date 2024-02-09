namespace Intervent.Web.DTO
{
    public class PortalIncentiveDto
    {
        public DateTime DateCreated { get; set; }

        public int Id { get; set; }

        public IncentiveTypes IncentiveTypes { get; set; }

        public string Name { get; set; }

        public string IncentiveTypeName
        {
            get
            {
                return ((IncentiveTypes)IncentiveTypeId).ToString();
            }
        }

        public int IncentiveTypeId { get; set; }

        public bool IsActive { get; set; }

        public bool IsCompanyIncentive { get; set; }

        public bool IsPoint { get; set; }

        public double Points { get; set; }
        public string PointsText { get; set; }

        public int PortalId { get; set; }

        public int? RefId { get; set; }

        public int? RefValue { get; set; }

        public int? RefValue2 { get; set; }

        public int? RefValue3 { get; set; }

        public string Attachment { get; set; }

        public string MoreInfo { get; set; }

        public string ImageUrl { get; set; }

        public string LanguageItemName { get; set; }

        public string LanguageItemMoreInfo { get; set; }

        public bool isGiftCard { get; set; }

        public bool removeSurcharge { get; set; }

        public IncentiveTypeDto IncentiveType { get; set; }

        public IList<UserIncentiveDto> UserIncentives { get; set; }
    }
}