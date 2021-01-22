using System;
using System.Collections.Generic;
using System.Text;

namespace SimchaFund.Data
{
    public class Contribution
    {
        public int SimchaId { get; set; }
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
    }
}
