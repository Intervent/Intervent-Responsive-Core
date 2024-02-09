namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UserHistory")]
    public partial class UserHistory
    {
        public long Id { get; set; }

        public DateTime LogDate { get; set; }

        public int UserId { get; set; }

        public int UpdatedBy { get; set; }

        public string? Changes { get; set; }

        public int UserHistoryCategoryId { get; set; }

        public virtual UserHistoryCategory UserHistoryCategory { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
