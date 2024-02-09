using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intervent.DAL
{
    [Table("ClaimLoadExcludeIds")]
    public class ClaimLoadExcludeIds
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        [Key, Column(Order = 1)]
        public string TableName { get; set; }
    }
}
