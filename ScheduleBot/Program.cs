using ScheduleBot.Bot;
using ScheduleBot.Enums;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

string tgToken = "5844406785:AAHA8FKtl-gWsaRJXC8IUP8dkKCid5me-nY";
StartBot bot = new(tgToken);

CancellationTokenSource cts = new();
bot.StartReceivingAsync(cts);

Console.ReadLine();



