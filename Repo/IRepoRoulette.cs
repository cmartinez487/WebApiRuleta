using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRuleta.Models;

namespace WebApiRuleta.Repo
{
    public interface IRepoRoulette
    {
        Task<List<Roulette>> ListRoulette();
        Task<List<Bet>> ListBet();
        Task<Roulette> ConsultRoulette(int id);
        Task<int> CreateNewRoulette(string RouletteName);
        Task<bool> OpenRoulette(int IdRoulette);
        Task<bool> CloseRoulette(Roulette Rou);
        Task<int> CreateNewBet(Bet newbet);
    }
}
