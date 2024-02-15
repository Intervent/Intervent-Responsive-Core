using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intervent.Web.DTO
{
    public class AddQADOrdersRequest
    {
        public IList<QADOrdersDto> qadOrders { get; set; }
    }
}
