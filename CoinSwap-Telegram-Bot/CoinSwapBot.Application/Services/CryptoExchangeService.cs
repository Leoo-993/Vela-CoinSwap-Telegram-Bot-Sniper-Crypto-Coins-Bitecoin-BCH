using CoinSwapBot.Domain.Interfaces.Services;
using CryptoExchange.Net.Authentication;
using Kraken.Net.Clients;
using Kraken.Net.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinSwapBot.Application.Services
{
    public class CryptoExchangeService : ICryptoExchangeService
    {
        private readonly IExchangeRateService _exchangeRateService;

        public CryptoExchangeService(IExchangeRateService exchangeRateService)
        {
            this._exchangeRateService = exchangeRateService;
        }

        public async Task<Dictionary<string, decimal>> GetBalance(string publicKey, string privateKey)
        {
            var balances = new Dictionary<string, decimal>();
            var krakenClient = new KrakenRestClient();
            var credentials = new ApiCredentials(publicKey, privateKey);
            krakenClient.SetApiCredentials(credentials);

            var balancesResponse = await krakenClient.SpotApi.Account.GetBalancesAsync();

            foreach (var currency in balancesResponse.Data)
            {
                balances.Add(currency.Key, currency.Value);
            }

            return balances;
        }
    }
}
