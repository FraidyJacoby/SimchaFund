using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimchaFund.Data;

namespace SimchaFund.Models
{
    public class ShowHistoryViewModel
    {
        public Contributor Contributor { get; set; }
        public List<SimchaFund.Data.Action> Actions { get; set; }
    }
}
