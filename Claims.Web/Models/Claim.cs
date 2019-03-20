﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Claims.Web.Models
{
    public class Claim
    {
        public string Id { get; set; }

        public Expense Expense { get; set; }

        public Event Event { get; set; }

    }
}
