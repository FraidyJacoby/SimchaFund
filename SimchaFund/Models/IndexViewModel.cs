﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimchaFund.Data;

namespace SimchaFund.Models
{
    public class IndexViewModel
    {
        public List<Simcha> Simchas { get; set; }
        public int ContributorCount { get; set; }
    }
}
