using CoinSwapBot.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CoinSwapBot.Application.Services
{
    public class UpdateTypeService : IUpdateTypeService
    {
        public bool IsTextMessage(Update update)
        {
            return update.Message != null && update.Message.Type == MessageType.Text;
        }

        public bool IsCallbackQuery(Update update)
        {
            return update.Type == UpdateType.CallbackQuery;
        }
    }
}
