namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class UserLogin
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        public string LoginProvider { get; set; }

        [Key]
        [Column(Order = 2)]
        public string ProviderKey { get; set; }

        public virtual User User { get; set; }
    }
}
