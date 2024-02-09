namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AWV_AlcoholUse
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public byte? HowOften { get; set; }

        public byte? HowMany { get; set; }

        public byte? SixorMore { get; set; }

        public byte? CantStop { get; set; }

        public byte? FailtoDo { get; set; }

        public byte? MorDrink { get; set; }

        public byte? FeelGuilt { get; set; }

        public byte? DontRem { get; set; }

        public byte? Injured { get; set; }

        public byte? Concerned { get; set; }

        public byte? AlcoholScore { get; set; }

        public virtual AWV AWV { get; set; }
    }
}
