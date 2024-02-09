namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CRM_ChangeLogs")]
    public partial class CRM_ChangeLog
    {
        public int Id { get; set; }

        public DateTime LogDate { get; set; }

        public int ContactId { get; set; }

        public int? RefId { get; set; }

        public string? Changes { get; set; }

        public int? CategoryId { get; set; }

        public int UpdatedBy { get; set; }

        public virtual CRM_Contact CRM_Contacts { get; set; }

        public virtual User User { get; set; }

    }
}
