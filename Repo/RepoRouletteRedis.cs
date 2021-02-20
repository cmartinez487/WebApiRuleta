using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiRuleta.Models;

namespace WebApiRuleta.Repo
{
    public class RepoRouletteRedis: IRepoRoulette
    {
        #region Instance
        private readonly IConnectionMultiplexer connection;
        private readonly IRPClient instacli;
        private readonly IDatabase db;
        public RepoRouletteRedis(IConnectionMultiplexer connection, IRPClient Instacli)
        {
            this.connection = connection;
            instacli = Instacli;
            this.db = connection.GetDatabase();
        }
        #endregion

        public async Task<List<Roulette>> ListRoulette()
        {
            List<Roulette> _listRoulette = new List<Roulette>();
            var List = await this.db.ListRangeAsync("Roulettes");
            if (List == null || !List.Any())
            {
                return null;
            }
            foreach (var rou in List)
            {
                var json = (string)rou;
                var Roudes = JsonSerializer.Deserialize<Roulette>(json);
                _listRoulette.Add(Roudes);
            }

            return _listRoulette;
        }

        public async Task<List<Bet>> ListBet()
        {
            List<Bet> _listBet = new List<Bet>();
            var List = await this.db.ListRangeAsync("Bets");
            if (List == null || !List.Any())
            {
                return null;
            }
            foreach (var bet in List)
            {
                var json = (string)bet;
                var Betdes = JsonSerializer.Deserialize<Bet>(json);
                _listBet.Add(Betdes);
            }

            return _listBet;
        }

        public async Task<Roulette> ConsultRoulette(int id)
        {
            var Rou = await ListRoulette();
            if (Rou == null)
            {
                return null;
            }

            return Rou.Where(Rou => Rou.IdRoulette == id).FirstOrDefault();
        }

        public async Task<int> CreateNewRoulette(string RouletteName)
        {
            int IdRoulette = 1;
            var Roulettes = await ListRoulette();
            if (Roulettes!= null)
            {
                IdRoulette = Roulettes.Count() + 1;
            }
            
            Roulette newroulette = new Roulette()
            {
                IdRoulette = IdRoulette,
                RouletteName = RouletteName,
                RouletteState = false
            };
            var json = JsonSerializer.Serialize(newroulette);

            await this.db.ListLeftPushAsync("Roulettes", json);

            return IdRoulette;
        }

        public async Task<bool> OpenRoulette(int IdRoulette)
        {
            List<RedisValue> ListRouRedis = new List<RedisValue>();
            var Roulettes = await ListRoulette();

            bool state = false;
            foreach (var rou in Roulettes)
            {
                if (rou.IdRoulette == IdRoulette)
                {
                    rou.RouletteState = true;
                    state = true;
                }
                ListRouRedis.Add((RedisValue)JsonSerializer.Serialize(rou));
            }
            bool delete = await this.db.KeyDeleteAsync("Roulettes");
            await this.db.ListLeftPushAsync("Roulettes", ListRouRedis.ToArray());
            
            return state;
        }

        public async Task<int> CreateNewBet(Bet newbet)
        {
            var _listBet = await ListBet();
            newbet.IdBet = 1;
            newbet.BetColor = "Black";
            if (_listBet != null)
            {
                newbet.IdBet = _listBet.Count() + 1;
            }
            if ((newbet.BetNumber % 2) == 0)
            {
                newbet.BetColor = "Red";
            }
            var json = JsonSerializer.Serialize(newbet);
            await this.db.ListLeftPushAsync("Bets", json);
            await instacli.DiscountAmount(newbet.IdClient, newbet.BetAmount);

            return newbet.IdBet;
        }

        public async Task<bool> CloseRoulette(Roulette Rou)
        {
            List<RedisValue> ListRouRedis = new List<RedisValue>();
            var Roulettes = await ListRoulette();

            bool state = false;
            foreach (var rou in Roulettes)
            {
                if (rou.IdRoulette == Rou.IdRoulette)
                {
                    rou.RouletteState = false;
                    rou.WiningBets = Rou.WiningBets;
                    rou.NumWinner = Rou.NumWinner;
                    state = true;
                }
                ListRouRedis.Add((RedisValue)JsonSerializer.Serialize(rou));
            }

            bool delete = await this.db.KeyDeleteAsync("Roulettes");
            await this.db.ListLeftPushAsync("Roulettes", ListRouRedis.ToArray());

            return state;
        }
    }
}
