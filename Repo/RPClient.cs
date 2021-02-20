using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiRuleta.Models;

namespace WebApiRuleta.Repo
{
    public class RPClient: IRPClient
    {
        public List<Client> _listClients = new List<Client>()
        {
            new Client() { Id = 1, Name = "Cliente 1" , Subname = "Apellido 1", AmountAvailable = 1000 },
            new Client() { Id = 2, Name = "Cliente 2" , Subname = "Apellido 2", AmountAvailable = 10000 },
            new Client() { Id = 3, Name = "Cliente 3" , Subname = "Apellido 3", AmountAvailable = 250 }
        };
        public Task<List<Client>> ListClients()
        {
            return Task.FromResult(_listClients);
        }
        public Task<Client> GetClient(int id)
        {
            var client = _listClients.Where(cli => cli.Id == id);

            return Task.FromResult(client.FirstOrDefault());
        }
        public Task AddClient(Client newClient)
        {
            int IdClient = _listClients.Count() + 1;
            newClient.Id = IdClient;
            _listClients.Add(newClient);
            
            return Task.CompletedTask;
        }
        public Task AddPrize(int cliId, decimal amount)
        {
            foreach (var cli in _listClients)
            {
                if (cli.Id == cliId)
                {
                    cli.AmountAvailable = cli.AmountAvailable + amount;
                };
            }

            return Task.CompletedTask;
        }

        public Task DiscountAmount(int cliId, decimal amount)
        {
            throw new System.NotImplementedException();
        }
    }
}
