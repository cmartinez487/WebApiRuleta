using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRuleta.Models;

namespace WebApiRuleta.Repo
{
    public interface IRPClient
    {
        Task<List<Client>> ListClients();
        Task<Client> GetClient(int id);
        Task AddClient(Client newClient);
        Task AddPrize(int cliId, decimal amount);
        Task DiscountAmount(int cliId, decimal amount);
    }
}
