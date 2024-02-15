using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intervent.Web.DTO
{
    public class QADOrdersDto
    {
        public int Id { get; set; }

        public string Order { get; set; }

        public DateTime? OrderDate { get; set; }

        public int? MeterQuantity { get; set; }

        public string ItemNumber { get; set; }

        public string QtyOrdered { get; set; }
    }
}