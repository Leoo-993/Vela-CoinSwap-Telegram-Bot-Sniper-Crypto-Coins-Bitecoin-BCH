using Telegram.Bot;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using CoinSwapBot.Domain.Interfaces.Services;
using CoinSwapBot.Domain.Interfaces.Repositories;
using CoinSwapBot.Domain.Models;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace CoinSwapBot.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IClientRepository _clientRepository;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IMemoryCache _cacheService;
        private readonly IInlineKeyboardMarkupService _inlineKeyboardMarkupService;
        private readonly IUpdateTypeService _updateTypeService;
        private readonly ILocalizationService _localizationService;
        private readonly IStringLocalizer<MenuService> _localizer;

        public MenuService(ITelegramBotClient telegramBotClient, IExchangeRateService exchangeRateService, IInlineKeyboardMarkupService inlineKeyboardMarkupService, 
            IUpdateTypeService updateTypeService, IClientRepository clientRepository, ILocalizationService localizationService, IStringLocalizer<MenuService> localizer, IMemoryCache cacheService)
        {
            this._telegramBotClient = telegramBotClient;
            this._exchangeRateService = exchangeRateService;
            this._inlineKeyboardMarkupService = inlineKeyboardMarkupService;
            this._updateTypeService = updateTypeService;
            this._clientRepository = clientRepository;
            this._localizationService = localizationService;
            this._localizer = localizer;
            this._cacheService = cacheService;
        }

        public async Task HandleUpdate(Update update)
        {
            if (_updateTypeService.IsTextMessage(update))
            {
                await HandleTextMessage(update.Message);
            }
            else if (_updateTypeService.IsCallbackQuery(update))
            {
                await HandleCallbackQuery(update.CallbackQuery);
            }
        }

        private async Task HandleTextMessage(Message message)
        {
            var text = message.Text;
            var username = message.Chat.Username;

            if (text == "/start")
            {
                var client = await _clientRepository.GetByUsername(username);

                if (client == null)
                {
                    client = new Client
                    {
                        Username = message.Chat.Username
                    };

                    await _clientRepository.CreateAsync(client);
                    await _clientRepository.SaveAsync();
                }

                var clientId = client.Id;

                _cacheService.Set("CurrentLanguage", client.Language.ToString());
                await HandleStartCommand(message, clientId);

            }
            else if(_cacheService.Get<string>("Currency") != null)
            {
                await HandleAmountInput(message);
            }
            else
            {
                await HandleOtherTextMessage(message);
            }
        }

        private async Task HandleStartCommand(Message message, string clientId)
        {
            var chatId = message.Chat.Id;
            var inlineKeyboard = _inlineKeyboardMarkupService.CreateStartMenuInlineKeyboard();
            var stickerId = "CAACAgIAAxkBAAHQU6RktRWSdfFrYkdavI_3tN6hS3HPKgAC2A8AAkjyYEsV-8TaeHRrmC8E";
            var startMessage = $"{_localizer["WelcomeMessage"].Value}\n\n{_localizer["YourId"].Value} {clientId}";

            await _telegramBotClient.SendStickerAsync(chatId, InputFile.FromString(stickerId));
            var startMenu = await _telegramBotClient.SendTextMessageAsync(chatId, startMessage, replyMarkup: inlineKeyboard);
            _cacheService.Set("MenuId", startMenu.MessageId);
        }

        private async Task HandleAmountInput(Message message)
        {
            var chatId = message.Chat.Id;
            var inlineKeyboard = _inlineKeyboardMarkupService.CreateBackAndMenuInlineKeyboard();
            var amount = Convert.ToDecimal(message.Text);

            var exchangeRate = await _exchangeRateService.GetExchangeRate(_cacheService.Get<string>("Currency"));
            var baseCurrency = _cacheService.Get<string>("Currency");
            var targetCurrency = baseCurrency == "BTC" ? "ETH" : "BTC";
            var amountInUSD = amount * Math.Round(exchangeRate["USD"], 2);
            var amountInTargetCurrency = amount * Math.Round(exchangeRate[targetCurrency], 8);
            var calculateExchangeMessage = $"{amount} {baseCurrency} = {amountInUSD} USD\n{amount} {baseCurrency} = {amountInTargetCurrency} {targetCurrency}";

            await _telegramBotClient.DeleteMessageAsync(chatId, message.MessageId);

            var calculateExchangeMenu = await _telegramBotClient.EditMessageTextAsync(chatId, _cacheService.Get<int>("MenuId"), calculateExchangeMessage, replyMarkup: inlineKeyboard);
            _cacheService.Set("MenuId", calculateExchangeMenu.MessageId);
            _cacheService.Set("CurrentMenu", "amount_input");
        }

        private async Task HandleOtherTextMessage(Message message)
        {
            var chatId = message.Chat.Id;

            var otherMessage = _localizer["OtherMessage"].Value;
            var telegramButtonKeyboard = _inlineKeyboardMarkupService.CreateTelegramAndMenuButtonInlineKeyboard();

            await _telegramBotClient.DeleteMessageAsync(chatId, message.MessageId);
            var updatedMessage = await _telegramBotClient.EditMessageTextAsync(chatId, _cacheService.Get<int>("MenuId"), otherMessage, replyMarkup: telegramButtonKeyboard);
            _cacheService.Set("MenuId", updatedMessage.MessageId);
        }

        private async Task HandleCallbackQuery(CallbackQuery callbackQuery)
        {
            switch (callbackQuery.Data)
            {
                case "exchange_rate":
                    await HandleExchangeRateQuery(callbackQuery);
                    break;
                case "contacts":
                    await HandleContactsQuery(callbackQuery);
                    break;
                case "menu":
                    await HandleMenuQuery(callbackQuery);
                    break;
                case "calculate_exchange":
                    await HandleCalculateExchangeQuery(callbackQuery);
                    break;
                case "BTC":
                case "ETH":
                    await HandleCurrencySelectionQuery(callbackQuery);
                    break;
                case "change_language":
                    await HandleLanguageChange(callbackQuery);
                    break;
                case "en":
                case "uk":
                case "ja":
                case "pl":
                    await HandleUserLanguage(callbackQuery);
                    break;
                case "back":
                    await HandleBackButton(callbackQuery);
                    break;
            };
        }

        private async Task HandleUserLanguage(CallbackQuery callbackQuery)
        {
            var languageCode = callbackQuery.Data;
            var username = callbackQuery.Message.Chat.Username;
            await _localizationService.SetLanguage(username, languageCode);

            var newCulture = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            _cacheService.Set("CurrentLanguage", languageCode);

            await HandleMenuQuery(callbackQuery);
        }

        private async Task HandleExchangeRateQuery(CallbackQuery callbackQuery)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var inlineKeyboard = _inlineKeyboardMarkupService.CreateCryptocurrencyAndMenuInlineKeyboard();
            var exchangeBTC = await _exchangeRateService.GetExchangeRate("BTC");
            var exchangeETH = await _exchangeRateService.GetExchangeRate("ETH");
            var exchangeMessage = $"{_localizer["BitcoinRate"].Value}:\n1 BTC = ${Math.Round(exchangeBTC["USD"], 2)}\n" +
                $"1 BTC = {Math.Round(exchangeBTC["ETH"], 2)} ETH\n\n{_localizer["EthereumRate"].Value}:\n1 ETH = ${Math.Round(exchangeETH["USD"], 2)}\n1 ETH = {Math.Round(exchangeETH["BTC"], 8)} BTC";

            var exchangeMenu = await _telegramBotClient.EditMessageTextAsync(chatId, _cacheService.Get<int>("MenuId"), exchangeMessage, replyMarkup: inlineKeyboard);
            _cacheService.Set("MenuId", exchangeMenu.MessageId);
            _cacheService.Set("CurrentMenu", "exchange_rate");
        }

        private async Task HandleContactsQuery(CallbackQuery callbackQuery)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var inlineKeyboard = _inlineKeyboardMarkupService.CreateContactsAndMenuInlineKeyboard();
            var parseMode = ParseMode.Markdown;
            var contactMessage = $"{_localizer["OurContacts"].Value}\n{_localizer["Email"].Value} `coinswapbot@gmail.com`\n{_localizer["DeveloperContact"].Value} [matviiv_a](https://t.me/matviiv_a)";

            var contactMenu = await _telegramBotClient.EditMessageTextAsync(chatId, _cacheService.Get<int>("MenuId"), contactMessage, parseMode: parseMode, disableWebPagePreview: true, replyMarkup: inlineKeyboard);
            _cacheService.Set("MenuId", contactMenu.MessageId);
        }

        private async Task HandleCalculateExchangeQuery(CallbackQuery callbackQuery)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var inlineKeyboard = _inlineKeyboardMarkupService.CreateCalculateAndBackInlineKeyboard();
            var calculateMessage = _localizer["CalculateMessage"].Value;

            var calculateMenu = await _telegramBotClient.EditMessageTextAsync(chatId, _cacheService.Get<int>("MenuId"), calculateMessage, replyMarkup: inlineKeyboard);
            _cacheService.Set("MenuId", calculateMenu.MessageId);
            _cacheService.Set("CurrentMenu", "calculate_exchange");
        }

        private async Task HandleMenuQuery(CallbackQuery callbackQuery)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var username = callbackQuery.Message.Chat.Username;
            var client = await _clientRepository.GetByUsername(username);
            var inlineKeyboard = _inlineKeyboardMarkupService.CreateStartMenuInlineKeyboard();
            var startMessage = $"{_localizer["StartMessage"].Value}\n\n{_localizer["YourId"].Value} {client.Id}";

            var startMenu = await _telegramBotClient.EditMessageTextAsync(chatId, _cacheService.Get<int>("MenuId"), startMessage, replyMarkup: inlineKeyboard);
            _cacheService.Set("MenuId", startMenu.MessageId);
        }

        private async Task HandleCurrencySelectionQuery(CallbackQuery callbackQuery)
        {
            var chatId = callbackQuery.Message.Chat.Id;

            _cacheService.Set("CurrentMenu", "BTCorETH");
            _cacheService.Set("Currency", callbackQuery.Data);
            await _telegramBotClient.EditMessageTextAsync(chatId, _cacheService.Get<int>("MenuId"), _localizer["InputAmount"].Value);
        }

        private async Task HandleLanguageChange(CallbackQuery callbackQuery)
        {
            var username = callbackQuery.Message.Chat.Username;
            var chatId = callbackQuery.Message.Chat.Id;
            var inlineKeyboard = _inlineKeyboardMarkupService.CreateLanguagesAndMenuButtonInlineKeyboard();
            var currentLanguage = await _localizationService.GetCurrentLanguage(username);
            var languageMessage = $"{_localizer["CurrentLanguage"].Value} {currentLanguage}.\n\n{_localizer["SelectLanguage"].Value}";

            var languageMenu = await _telegramBotClient.EditMessageTextAsync(chatId, _cacheService.Get<int>("MenuId"), languageMessage, replyMarkup: inlineKeyboard);
            _cacheService.Set("MenuId", languageMenu.MessageId);
        }

        private async Task HandleBackButton(CallbackQuery callbackQuery)
        {
            var currentMenu = _cacheService.Get<string>("CurrentMenu");

            switch (currentMenu)
            {
                case "calculate_exchange":
                    await HandleExchangeRateQuery(callbackQuery);
                    break;
                case "BTCorETH":
                    await HandleCalculateExchangeQuery(callbackQuery);
                    break;
                case "amount_input":
                    await HandleCalculateExchangeQuery(callbackQuery);
                    break;
                default:
                    await HandleMenuQuery(callbackQuery);
                    break;
            }
        }
    }
}
