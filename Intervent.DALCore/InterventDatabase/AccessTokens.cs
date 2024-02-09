namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class AccessTokens
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string OrgName { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Token { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}
