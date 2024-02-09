namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;

    public partial class ReferralTypes
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Type { get; set; }
    }
}