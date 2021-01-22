using System;
using System.Collections.Generic;
using System.Text;

namespace SimchaFund.Data
{
    public class Deposit
    {
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
