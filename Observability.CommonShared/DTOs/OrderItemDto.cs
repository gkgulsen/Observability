﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.CommonShared.DTOs
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
