using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiRuleta.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subname { get; set; }
        public decimal AmountAvailable { get; set; }
    }
}
