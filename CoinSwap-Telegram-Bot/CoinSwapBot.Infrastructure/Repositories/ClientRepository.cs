using CoinSwapBot.Domain.Interfaces.Repositories;
using CoinSwapBot.Domain.Models;
using CoinSwapBot.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinSwapBot.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private CoinSwapBotDbContext _context;
        private bool disposed = false;

        public ClientRepository(CoinSwapBotDbContext context)
        {
            this._context = context;
        }

        public async Task CreateAsync(Client client)
        {
            client.Id = GenerateUniqueIdentifier();

            await _context.Clients.AddAsync(client);
        }

        public async Task<Client> FindById(int clientId)
        {
            return await _context.Clients.FindAsync(clientId);
        }

        public async Task UpdateAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Client> GetByUsername(string username)
        {
            return await _context.Clients.FirstOrDefaultAsync(client => client.Username == username);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private string GenerateUniqueIdentifier()
        {
            Guid guid = Guid.NewGuid();

            string uniqueId = guid.ToString("N").Substring(0, 10);
            uniqueId = "@coin" + uniqueId;

            return uniqueId;
        }
    }
}
