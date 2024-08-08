using CoinSwapBot.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinSwapBot.Domain.Interfaces.Repositories
{
    public interface IClientRepository
    {
        Task CreateAsync(Client client);
        Task<Client> FindById(int clientId);
        Task SaveAsync();
        Task UpdateAsync(Client client);
        Task<Client> GetByUsername(string username);
    }
}
