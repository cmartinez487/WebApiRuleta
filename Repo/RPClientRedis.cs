using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiRuleta.Models;

namespace WebApiRuleta.Repo
{
    public class RPClientRedis:IRPClient
    {
        #region Instance
        private readonly IConnectionMultiplexer connection;
        private readonly IDatabase db;
        public RPClientRedis(IConnectionMultiplexer connection)
        {
            this.connection = connection;
            this.db = connection.GetDatabase();
        }
        #endregion

        public async Task<List<Client>> ListClients()
        {
            List<Client> _listClients = new List<Client>();
            var List = await this.db.ListRangeAsync("Clients");
            if (List == null || !List.Any())
            {
                return null;
            }
            foreach (var client in List)
            {
                var json = (string)client;
                var Clientdes = JsonSerializer.Deserialize<Client>(json);

                _listClients.Add(Clientdes);
            }

            return _listClients;
        }
        public async Task<Client> GetClient(int id)
        {
            var client = await ListClients();
            if (client == null)
            {
                return null;
            }

            return client.Where(cli => cli.Id == id).FirstOrDefault();
        }
        public async Task AddClient(Client newClient)
        {
            var Clients = await ListClients();
            int IdClient = 1;
            if (Clients != null)
            {
                IdClient = Clients.Count()+1;
            }
            newClient.Id = IdClient;
            var json = JsonSerializer.Serialize(newClient);
            await this.db.ListLeftPushAsync("Clients", json);
        }
        public async Task AddPrize(int cliId, decimal amount)
        {
            var _listClients = await ListClients();
            List<RedisValue> ListClientsRedis = new List<RedisValue>();
            foreach (var cli in _listClients)
            {
                if (cli.Id == cliId)
                {
                    cli.AmountAvailable = cli.AmountAvailable + amount;
                };
                ListClientsRedis.Add((RedisValue)JsonSerializer.Serialize(cli));
            }
            bool delete = await this.db.KeyDeleteAsync("Clients");
            await this.db.ListLeftPushAsync("Clients", ListClientsRedis.ToArray());
        }
        public async Task DiscountAmount(int cliId, decimal amount)
        {
            var _listClients = await ListClients();
            List<RedisValue> ListClientsRedis = new List<RedisValue>();
            foreach (var cli in _listClients)
            {
                if (cli.Id == cliId)
                {
                    cli.AmountAvailable = cli.AmountAvailable - amount;
                };
                ListClientsRedis.Add((RedisValue)JsonSerializer.Serialize(cli));
            }
            bool delete = await this.db.KeyDeleteAsync("Clients");
            await this.db.ListLeftPushAsync("Clients", ListClientsRedis.ToArray());
        }
    }
}
