namespace Intervent.Web.DTO
{
    public class KitsPortalReport_ResultDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Name1 { get; set; }

        public int QuestionId { get; set; }

        public byte QuestionType { get; set; }

        public string QuestionText { get; set; }

        public int? ParentId { get; set; }

        public int OptionId { get; set; }

        public string OptionText { get; set; }

        public bool? IsAnswer { get; set; }

        public int? Points { get; set; }

        public string TextLangItemCode { get; set; }

        public string Value { get; set; }
    }
}
