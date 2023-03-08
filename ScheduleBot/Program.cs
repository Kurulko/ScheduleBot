using ScheduleBot;
using ScheduleBot.Commands;
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

StartBot bot = new();

CancellationTokenSource cts = new();

await bot.StartReceivingAsync(cts);

TimeSpan period = new TimeSpan(0, 10, 0);
bot.NotifyNewEvents(period, cts);

Console.ReadLine();



