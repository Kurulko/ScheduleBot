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
using ScheduleBot.Database.Initialize;
using ScheduleBot.Extensions;

SeedDbExample.SeedExampleData();

StartBot bot = new();

CancellationTokenSource cts = new();

await bot.StartReceivingAsync(cts);

ConsoleExtensions.WriteLineWithColor("Write something to finish the app", ConsoleColor.Green);
Console.ReadLine();



