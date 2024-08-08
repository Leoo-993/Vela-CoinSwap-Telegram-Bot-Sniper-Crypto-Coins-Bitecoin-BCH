using CoinSwapBot.Domain.Interfaces.Repositories;
using CoinSwapBot.Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CoinSwapBot.Middleware
{
    public class CultureMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly IServiceProvider _serviceProvider;

        public CultureMiddleware(RequestDelegate next, IMemoryCache cache, IServiceProvider serviceProvider)
        {
            this._next = next;
            this._cache = cache;
            this._serviceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();

            if (context.Request.Method == "POST" && context.Request.ContentLength > 0)
            {
                using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
                {
                    var requestBody = await reader.ReadToEndAsync();
                    var update = JsonConvert.DeserializeObject<Update>(requestBody);

                    if (update?.Message?.Chat != null)
                    {
                        var username = update.Message.Chat.Username;
                        var user = await GetUserByUsername(username);

                        if (user != null)
                        {
                            var languageCode = user.Language.ToString();
                            if (!string.IsNullOrEmpty(languageCode))
                            {
                                SetThreadCulture(languageCode);
                            }
                        }
                    }
                }

                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            var currentLanguage = _cache.Get<string>("CurrentLanguage");
            if (!string.IsNullOrEmpty(currentLanguage))
            {
                SetThreadCulture(currentLanguage);
            }

            await _next(context);
        }

        private async Task<Client> GetUserByUsername(string username)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var clientRepository = scope.ServiceProvider.GetRequiredService<IClientRepository>();
                return await clientRepository.GetByUsername(username);
            }
        }

        private void SetThreadCulture(string languageCode)
        {
            var newCulture = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;
        }
    }
}
