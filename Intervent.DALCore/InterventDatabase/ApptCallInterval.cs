namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ApptCallInterval")]
    public partial class ApptCallInterval
    {
        public int Id { get; set; }

        public int ApptCallTemplateId { get; set; }

        public int CallNumber { get; set; }

        public int IntervalInDays { get; set; }

        public virtual ApptCallTemplate ApptCallTemplate { get; set; }
    }
}
