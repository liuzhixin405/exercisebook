﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Customers
{
    public class MissingCustomer:Customer
    {
        public static MissingCustomer Instance = new MissingCustomer();
    }
}
