using CoinSwapBot.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinSwapBot.Infrastructure.Context
{
    public class CoinSwapBotDbContext : DbContext
    {
        public CoinSwapBotDbContext(DbContextOptions<CoinSwapBotDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
    }
}
