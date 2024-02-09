namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class AWV_PreventiveServicesType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AWV_PreventiveServicesType()
        {
            AWV_PreventiveServices = new HashSet<AWV_PreventiveServices>();
        }

        public int Id { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AWV_PreventiveServices> AWV_PreventiveServices { get; set; }
    }
}
