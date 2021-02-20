using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiRuleta.Models
{
    public class Roulette
    {
        public int IdRoulette { get; set; }
        public string RouletteName { get; set; }
        public List<Bet> WiningBets { get; set; }
        public int NumWinner { get; set; }
        public bool RouletteState { get; set; }
    }
}
