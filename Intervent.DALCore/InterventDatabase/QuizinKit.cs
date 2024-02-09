namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;

    public partial class QuizinKit
    {
        public int Id { get; set; }

        public int KitId { get; set; }

        [Required]
        [StringLength(500)]
        public string QuizText { get; set; }

        public byte QuizType { get; set; }

        public bool IsActive { get; set; }

        [StringLength(64)]
        public string? TextLangItemCode { get; set; }

        public virtual Kit Kit { get; set; }
    }
}
