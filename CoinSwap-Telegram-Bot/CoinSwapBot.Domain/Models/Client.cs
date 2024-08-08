using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinSwapBot.Domain.Models
{
    public enum Language
    {
        en,
        uk,
        pl,
        ja,
    }

    public class Client
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public Language Language { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}
