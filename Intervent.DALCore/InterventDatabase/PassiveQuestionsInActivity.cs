namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class PassiveQuestionsInActivity
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ActivityId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int QuestionId { get; set; }

        public short SequenceNo { get; set; }

        [Required]
        [StringLength(500)]
        public string QuestionText { get; set; }

        public bool IsActive { get; set; }

        [StringLength(64)]
        public string? TextLangItemCode { get; set; }

        public virtual ActivitiesinStep ActivitiesinStep { get; set; }

        public virtual QuestionsinActivity QuestionsinActivity { get; set; }
    }
}
