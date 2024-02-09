using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intervent.DAL
{
    [Table("CrothalIDChanges")]
    public class CrothalIDChanges
    {
        [Key]
        public string? OldUniqueId { get; set; }

        public string? NewUniqueId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

    }
}
