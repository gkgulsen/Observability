using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.CommonShared.DTOs
{
    public class PaymentCreateRequestDto
    {
        public string OrderCode { get; set; } = null!;
        public decimal TotalPrice { get; set; }
    }
}
