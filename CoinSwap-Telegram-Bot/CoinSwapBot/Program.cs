using CoinSwapBot.Application.Services;
using CoinSwapBot.Domain.Interfaces.Repositories;
using CoinSwapBot.Domain.Interfaces.Services;
using CoinSwapBot.Infrastructure.Context;
using CoinSwapBot.Infrastructure.Repositories;
using CoinSwapBot.Middleware;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CoinSwapBotDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("uk"),
        new CultureInfo("pl"),
        new CultureInfo("ja"),
    };

    options.DefaultRequestCulture = new RequestCulture(supportedCultures.FirstOrDefault());
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var telegramBotClient = new TelegramBotClient(builder.Configuration["7165442666:AAE4waIA90yiB0m1f9_1Q6Jr0ngQ6U_g9y0"]);

builder.Services.AddSingleton<ITelegramBotClient>(telegramBotClient);

builder.Services.AddMemoryCache();

builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<ICryptoExchangeService, CryptoExchangeService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IInlineKeyboardMarkupService, InlineKeyboardMarkupService>();
builder.Services.AddScoped<IUpdateTypeService, UpdateTypeService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();

builder.Services.AddScoped<IClientRepository, ClientRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRequestLocalization();

app.UseMiddleware<CultureMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
