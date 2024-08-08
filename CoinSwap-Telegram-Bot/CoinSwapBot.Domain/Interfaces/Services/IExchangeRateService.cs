using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinSwapBot.Domain.Interfaces.Services
{
    public interface IExchangeRateService
    {
        Task<Dictionary<string, decimal>> GetExchangeRate(string currency);
    }
}
