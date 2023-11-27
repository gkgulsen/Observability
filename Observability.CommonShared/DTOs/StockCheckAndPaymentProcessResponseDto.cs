using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.CommonShared.DTOs
{
    public record StockCheckAndPaymentProcessResponseDto
    {
        public string Descripton { get; set; } = null!;
    }
}
