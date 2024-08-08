using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinSwapBot.Domain.Models
{
    public enum CryptoCurrency
    {
        ETH,
        BTC
    }

    public class Currency
    {
        public int Id { get; set; }
        public CryptoCurrency Name { get; set; }
        public decimal CurrentRate { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}
