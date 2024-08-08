using CoinSwapBot.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types;

namespace CoinSwapBot.Controllers
{
    [ApiController]
    [Route("bot")]
    public class CoinSwapBotController : Controller
    {
        private readonly IMenuService _menuService;

        public CoinSwapBotController(IMenuService menuService)
        {
            this._menuService = menuService;
        }

        [HttpPost]
        public async Task<IActionResult> HandleUpdate([FromBody] Update update)
        {
            await _menuService.HandleUpdate(update);

            return Ok();
        }
    }
}
