using CoinSwapBot.Domain.Interfaces.Repositories;
using CoinSwapBot.Domain.Interfaces.Services;
using CoinSwapBot.Domain.Models;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinSwapBot.Application.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IStringLocalizer<LocalizationService> _localizer;

        public LocalizationService(IClientRepository clientRepository, IStringLocalizer<LocalizationService> localizer)
        {
            this._clientRepository = clientRepository;
            this._localizer = localizer;
        }

        public async Task<string> GetCurrentLanguage(string username)
        {
            var client = await _clientRepository.GetByUsername(username);
            var currentLanguage = ConvertLanguageCodeToFullName(client.Language);

            return currentLanguage;
        }

        public async Task SetLanguage(string username, string languageCode)
        {
            var client = await _clientRepository.GetByUsername(username);

            if (Enum.TryParse(languageCode, true, out Language selectedLanguage))
            {
                client.Language = selectedLanguage;
                await _clientRepository.UpdateAsync(client);
                await _clientRepository.SaveAsync();
            }
        }

        private string ConvertLanguageCodeToFullName(Language languageCode)
        {
            switch (languageCode)
            {
                case Language.uk:
                    return _localizer["Ukrainian"].Value;
                case Language.pl:
                    return _localizer["Polish"].Value;
                case Language.ja:
                    return _localizer["Japanese"].Value;
            }

            return _localizer["English"].Value;
        }
    }
}
