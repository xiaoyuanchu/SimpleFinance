using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleFinance.Core.Model
{
    public class Owner
    {
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
        public bool Disabled { get; set; }
    }
}
