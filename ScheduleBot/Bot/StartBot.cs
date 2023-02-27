using Azure;
using ScheduleBot.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Bot;

public class StartBot
{
    TelegramBotClient bot;
    public StartBot(string tgToken)
		=> bot = new(tgToken);
    
    readonly ReceiverOptions receiverOptions = new ReceiverOptions()
    {
        AllowedUpdates = Array.Empty<UpdateType>()
    };
    

    public async Task StartReceivingAsync(CancellationTokenSource cts)
    {
        bot.StartReceiving(
            updateHandler: HandleUpdateAsync, 
            pollingErrorHandler: (ITelegramBotClient bot, Exception exc, CancellationToken _) => HandlePollingErrorAsync(bot, exc, cts), 
            receiverOptions, 
            cts.Token);

        User me = await bot.GetMeAsync();
        string fName = me.FirstName;
        Console.WriteLine($"\"{fName}\" started listening ...");
    }

    async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken _)
    {
        if (update.Message is not { } message || message.Text is not { } messageText)
            return;
        
        if (Actions.TryParseMode(messageText, out Mode? mode))
        {
            string respond = Actions.DoAction(mode!.Value);
            await bot.SendTextMessageAsync(update.Message.Chat.Id, respond);
        }
        else
        {
            await bot.SendTextMessageAsync(update.Message.Chat.Id, messageText);
        }
    }
    
    async Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exc, CancellationTokenSource cts)
    {
        Console.WriteLine($"Error: {exc.Message}");
        await StopBotAsync(cts);
    }

    async Task StopBotAsync(CancellationTokenSource cts)
    {
        User me = await bot.GetMeAsync();
        string fName = me.FirstName;
        Console.WriteLine($"\"{fName}\" finished listening ...");
        cts.Cancel();
    }
}
