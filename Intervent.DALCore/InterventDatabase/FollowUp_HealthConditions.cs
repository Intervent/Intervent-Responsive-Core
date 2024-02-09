namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class FollowUp_HealthConditions
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public byte? StateOfHealth { get; set; }

        public byte? ProductivityLoss { get; set; }

        public virtual FollowUp FollowUp { get; set; }
    }
}
