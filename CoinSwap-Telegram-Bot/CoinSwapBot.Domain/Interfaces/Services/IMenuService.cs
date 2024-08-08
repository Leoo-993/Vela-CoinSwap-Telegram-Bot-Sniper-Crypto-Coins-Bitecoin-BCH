using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace CoinSwapBot.Domain.Interfaces.Services
{
    public interface IMenuService
    {
        Task HandleUpdate(Update update);
    }
}
