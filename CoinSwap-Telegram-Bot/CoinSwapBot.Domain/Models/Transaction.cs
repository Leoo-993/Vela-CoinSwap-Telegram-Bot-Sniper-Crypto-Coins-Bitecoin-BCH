using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinSwapBot.Domain.Models
{
    public enum TransactionType
    {
        Purchase,
        Exchange
    }

    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CurrencyId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }


        public Client Client { get; set; }
        public Currency Currency { get; set; }
    }
}
