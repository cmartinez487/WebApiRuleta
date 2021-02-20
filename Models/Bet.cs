using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiRuleta.Models
{
    public class Bet
    {
        public int IdBet { get; set; }
        public int IdRoulette { get; set; }
        public int IdClient { get; set; }
        public int? BetNumber { get; set; }
        public string BetColor { get; set; }
        public decimal BetAmount { get; set; }
        public int? WinNumber { get; set; }
    }
}
