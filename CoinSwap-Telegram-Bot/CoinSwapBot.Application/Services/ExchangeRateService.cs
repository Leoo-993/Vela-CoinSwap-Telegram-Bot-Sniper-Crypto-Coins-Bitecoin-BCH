using CoinSwapBot.Domain.Interfaces.Services;
using CryptoExchange.Net.Authentication;
using Kraken.Net.Clients;
using Kraken.Net.Interfaces.Clients;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinSwapBot.Application.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IKrakenRestClient _krakenClient;
        private readonly IConfiguration _configuration;

        public ExchangeRateService(IConfiguration configuration)
        {
            this._configuration = configuration;

            this._krakenClient = new KrakenRestClient();
            var credentials = new ApiCredentials(_configuration["Kraken:PublicKey"], _configuration["Kraken:PrivateKey"]);
            this._krakenClient.SetApiCredentials(credentials);
        }

        public async Task<Dictionary<string, decimal>> GetExchangeRate(string currency)
        {
            var usdSymbol = "USD";
            var btcSymbol = "BTC";
            var ethSymbol = "ETH";
            var targetCurrencyPair = currency == btcSymbol ? ethSymbol : btcSymbol;
            var exchangeRate = new Dictionary<string, decimal>();
            var currencyUsdtRate = await _krakenClient.SpotApi.ExchangeData.GetTickerAsync($"{currency}/{usdSymbol}");
            var targetCurrencyRate = await _krakenClient.SpotApi.ExchangeData.GetTickerAsync($"{ethSymbol}/{btcSymbol}");


            exchangeRate.Add(usdSymbol, currencyUsdtRate.Data[$"{currency}/{usdSymbol}"].LastTrade.Price);
            exchangeRate.Add(targetCurrencyPair, currency == btcSymbol ? 1 / targetCurrencyRate.Data[$"{ethSymbol}/{btcSymbol}"].LastTrade.Price : targetCurrencyRate.Data[$"{ethSymbol}/{btcSymbol}"].LastTrade.Price);

            return exchangeRate;
        }
    }
}
