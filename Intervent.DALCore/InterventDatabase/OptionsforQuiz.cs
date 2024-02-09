namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OptionsforQuiz")]
    public partial class OptionsforQuiz
    {
        public int Id { get; set; }

        public int QuizId { get; set; }

        [Required]
        [StringLength(500)]
        public string OptionText { get; set; }

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; }

        [StringLength(64)]
        public string? TextLangItemCode { get; set; }

        public virtual QuizinStep QuizinStep { get; set; }
    }
}
