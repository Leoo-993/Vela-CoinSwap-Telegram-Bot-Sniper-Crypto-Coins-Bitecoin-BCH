using CoinSwapBot.Domain.Interfaces.Services;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace CoinSwapBot.Application.Services
{
    public class InlineKeyboardMarkupService : IInlineKeyboardMarkupService
    {
        private readonly IStringLocalizer<InlineKeyboardMarkupService> _localizer;

        public InlineKeyboardMarkupService(IStringLocalizer<InlineKeyboardMarkupService> localizer)
        {
            this._localizer = localizer;
        }

        public InlineKeyboardMarkup CreateStartMenuInlineKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["BuyingCryptocurrency"].Value, "buying_cryptocurrency") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["CryptocurrencyExchange"].Value, "cryptocurrency_exchange") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["ExchangeRate"].Value, "exchange_rate") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["TransactionHistory"].Value, "transaction_history") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["ChangeLanguage"].Value, "change_language") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["Contacts"].Value, "contacts") }
            });
        }

        public InlineKeyboardMarkup CreateBackAndMenuInlineKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["Back"].Value, "back") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["Menu"].Value, "menu") }
            });
        }

        public InlineKeyboardMarkup CreateCalculateAndBackInlineKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["Bitcoin"].Value, "BTC") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["Ethereum"].Value, "ETH") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["Menu"].Value, "menu") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["Back"].Value, "back") }
            });
        }

        public InlineKeyboardMarkup CreateCryptocurrencyAndMenuInlineKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["CalculateExchange"].Value, "calculate_exchange") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["Menu"].Value, "menu") }
            });
        }

        public InlineKeyboardMarkup CreateContactsAndMenuInlineKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithUrl(_localizer["Github"].Value, "https://github.com/matviiv8") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["Menu"].Value, "menu") }
            });
        }

        public InlineKeyboardMarkup CreateTelegramAndMenuButtonInlineKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithUrl(_localizer["Telegram"].Value, "https://t.me/matviiv_a") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["Menu"].Value, "menu") }
            });
        }

        public InlineKeyboardMarkup CreateLanguagesAndMenuButtonInlineKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["English"].Value, "en") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["Ukrainian"].Value, "uk") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["Polish"].Value, "pl") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["Japanese"].Value, "ja") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizer["Menu"].Value, "menu") }
            });
        }
    }
}
