﻿using System;

namespace SimchaFund.Data
{
    public class Simcha
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int Contributions { get; set; }
        public decimal Total { get; set; }
    }
}
