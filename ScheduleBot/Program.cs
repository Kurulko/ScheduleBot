using ScheduleBot;
using ScheduleBot.Bot;
using ScheduleBot.Database;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using ScheduleContext db = new();
SeedData.SeedDb(db);

string tgToken = SettingsTelegram.Token;
StartBot bot = new(tgToken);

CancellationTokenSource cts = new();
await bot.StartReceivingAsync(cts);

Console.ReadLine();



