using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace CoinSwapBot.Domain.Interfaces.Services
{
    public interface IInlineKeyboardMarkupService
    {
        InlineKeyboardMarkup CreateStartMenuInlineKeyboard();
        InlineKeyboardMarkup CreateBackAndMenuInlineKeyboard();
        InlineKeyboardMarkup CreateCalculateAndBackInlineKeyboard();
        InlineKeyboardMarkup CreateCryptocurrencyAndMenuInlineKeyboard();
        InlineKeyboardMarkup CreateContactsAndMenuInlineKeyboard();
        InlineKeyboardMarkup CreateTelegramAndMenuButtonInlineKeyboard();
        InlineKeyboardMarkup CreateLanguagesAndMenuButtonInlineKeyboard();
    }
}
