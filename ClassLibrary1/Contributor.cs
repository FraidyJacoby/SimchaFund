using System;
using System.Collections.Generic;
using System.Text;

namespace SimchaFund.Data
{
    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Balance { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal Amount {get; set;}
        public Int64 CellNumber { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
