using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiRuleta.Models;

namespace WebApiRuleta.Repo
{
    public class RepoRoulette:IRepoRoulette
    {
        #region Instance

        private readonly IRPClient instacli;

        public RepoRoulette(IRPClient Instacli)
        {
            this.instacli = Instacli;
        }

        #endregion

        #region List Roulette and Bet
        public static List<Roulette> _listRoulette = new List<Roulette>();
        public static List<Bet> _listBet = new List<Bet>();
        #endregion

        public Task<List<Roulette>> ListRoulette()
        {
            return Task.FromResult(_listRoulette);
        }

        public Task<Roulette> ConsultRoulette(int id)
        {
            var Roulette = _listRoulette.Where(rou => rou.IdRoulette == id);

            return Task.FromResult(Roulette.FirstOrDefault());
        }

        public Task<int> CreateNewRoulette(string RouletteName)
        {
            int IdRoulette = _listRoulette.Count() + 1;

            Roulette newroulette = new Roulette()
            {
                IdRoulette = IdRoulette,
                RouletteName = RouletteName,
                RouletteState = false
            };

            _listRoulette.Add(newroulette);

            return Task.FromResult(IdRoulette);
        }

        public Task<bool> OpenRoulette(int IdRoulette)
        {
            bool state = false;
            foreach (var rou in _listRoulette)
            {
                if (rou.IdRoulette == IdRoulette)
                {
                    rou.RouletteState = true;
                    state = true;
                }
            }

            return Task.FromResult(state);
        }

        public Task<List<Bet>> ListBet()
        {
            return Task.FromResult(_listBet);
        }

        public async Task<int> CreateNewBet(Bet newbet)
        {
            if ((newbet.BetNumber % 2) == 0)
            {
                newbet.BetColor = "Red";
            }
            else
            {
                newbet.BetColor = "Black";
            }
            newbet.IdBet = _listBet.Count() + 1;
            _listBet.Add(newbet);

            var Listcli = await instacli.ListClients();

            foreach (var cli in Listcli)
            {
                if (newbet.IdClient == cli.Id)
                {
                    cli.AmountAvailable = cli.AmountAvailable - newbet.BetAmount;
                }
            }

            return newbet.IdBet;
        }

        public Task<bool> CloseRoulette(Roulette Rou)
        {
            bool state = false;
            foreach (var rou in _listRoulette)
            {
                if (rou.IdRoulette == Rou.IdRoulette)
                {
                    rou.RouletteState = false;
                    rou.WiningBets = Rou.WiningBets;
                    rou.NumWinner = Rou.NumWinner;
                    state = true;
                }
            }

            return Task.FromResult(state);
        }
    }
}
