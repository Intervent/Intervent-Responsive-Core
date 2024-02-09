namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class OptionsforActivityQuestion
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        [Required]
        [StringLength(500)]
        public string OptionText { get; set; }

        public bool? IsAnswer { get; set; }

        public bool IsActive { get; set; }

        public int? Points { get; set; }

        public short? SequenceNo { get; set; }

        [StringLength(64)]
        public string? TextLangItemCode { get; set; }

        public virtual QuestionsinActivity QuestionsinActivity { get; set; }

        public virtual List<QuestionsinActivity> SubQuestions { get; set; }
    }
}
