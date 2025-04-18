﻿using Architecture.Domain.Products;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Services.DTO
{
    public class LowStockReport
    {
        public LowStockReport()
        {
            Insufficient = new Collection<Product>();
            Low = new Collection<Product>();
        }
        public ICollection<Product> Insufficient { get; private set; }
        public ICollection<Product> Low { get; private set; }
    }
}
