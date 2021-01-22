using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimchaFund.Data;

namespace SimchaFund.Models
{
    public class ContributionsViewModel
    {
        public Simcha Simcha { get; set; }
        public List<Contributor> Contributors { get; set; }
    }
}
