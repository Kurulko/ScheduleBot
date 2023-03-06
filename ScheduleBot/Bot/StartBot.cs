using Azure;
using ScheduleBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleBot.Bot;

public class StartBot
{
    readonly TelegramBotClient bot;
    public StartBot(string tgToken)
		=> bot = new(tgToken);
    
    readonly ReceiverOptions receiverOptions = new ReceiverOptions()
    {
        AllowedUpdates = Array.Empty<UpdateType>()
    };
    

    public async Task StartReceivingAsync(CancellationTokenSource cts)
    {
        bot.StartReceiving(
            updateHandler: (ITelegramBotClient bot, Update update, CancellationToken _) => HandleUpdateAsync(bot, update, cts), 
            pollingErrorHandler: (ITelegramBotClient bot, Exception exc, CancellationToken _) => HandlePollingErrorAsync(bot, exc, cts), 
            receiverOptions, 
            cts.Token);

        User me = await bot.GetMeAsync();
        string fName = me.FirstName;
        Console.WriteLine($"\"{fName}\" started listening ...");
    }

    async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationTokenSource cts)
    {
        if (update.Message is { } message && message.Text is { } messageText)
        {
            int replyToMessageId = message.MessageId;
            long chatId = message.Chat.Id;
            try
            {
                await BotActions.DoAction(bot, messageText, chatId, replyToMessageId,  cts);
            }
            catch(BotScheduleException botExc)
            {
                await bot.SendTextMessageAsync(chatId, $"<b>{botExc.Message}</b>", ParseMode.Html, replyToMessageId : replyToMessageId, cancellationToken: cts.Token);
            }
            catch(Exception exc)
            {
                Console.WriteLine($"Error: {exc.Message}");
                await StopBotAsync(cts);
            }
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
