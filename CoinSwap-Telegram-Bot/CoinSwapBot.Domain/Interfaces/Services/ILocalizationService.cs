using CoinSwapBot.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinSwapBot.Domain.Interfaces.Services
{
    public interface ILocalizationService
    {
        Task<string> GetCurrentLanguage(string username);
        Task SetLanguage(string username, string languageCode);
    }
}
