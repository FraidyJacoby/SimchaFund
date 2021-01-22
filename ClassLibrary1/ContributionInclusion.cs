using System;
using System.Collections.Generic;
using System.Text;

namespace SimchaFund.Data
{
    public class ContributionInclusion
    {
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
        public bool Include { get; set; }
    }
}
