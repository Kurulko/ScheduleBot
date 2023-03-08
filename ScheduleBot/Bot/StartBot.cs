using Azure;
using ScheduleBot.Database;
using ScheduleBot.Database.Models;
using ScheduleBot.Exceptions;
using ServiceStack.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleBot.Bot;

public class StartBot
{
    readonly ITelegramBotClient bot = SettingsTelegram.CurrentBot();
    ScheduleContext db = new();

    public async Task StartReceivingAsync(CancellationTokenSource cts)
    {
        ReceiverOptions receiverOptions = new ReceiverOptions() { AllowedUpdates = Array.Empty<UpdateType>() };

        bot.StartReceiving(
            updateHandler: (ITelegramBotClient bot, Update update, CancellationToken _) => HandleUpdateAsync(update, cts), 
            pollingErrorHandler: (ITelegramBotClient bot, Exception exc, CancellationToken _) => HandlePollingErrorAsync(exc, cts), 
            receiverOptions, 
            cts.Token);

        User me = await bot.GetMeAsync();
        string fName = me.FirstName;
        Console.WriteLine($"\"{fName}\" started listening ...");
    }

    void AddTgChatToDbIfNotExist(TelegramChat tgChat)
    {
        if (db.Chats.FirstOrDefault(c => c.Chat == tgChat.Chat) is null)
        {
            db.Chats.Add(tgChat);
            db.SaveChanges();
        }
    }

    async Task HandleUpdateAsync(Update update, CancellationTokenSource cts)
    {
        if (update.Message is { } message && message.Text is { } messageText)
        {
            int replyToMessageId = message.MessageId;
            long chatId = message.Chat.Id;
            try
            {
                TelegramChat tgChat = (TelegramChat)message.Chat;
                AddTgChatToDbIfNotExist(tgChat);

                await BotActions.DoActionAsync(bot, messageText, chatId, replyToMessageId,  cts);
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
    
    async Task HandlePollingErrorAsync(Exception exc, CancellationTokenSource cts)
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

    public void NotifyNewEvents(TimeSpan period, CancellationTokenSource cts)
    {
        TimerCallback tm = async (object state) =>
        {
            foreach (var chat in db.Chats.Select(c => c.Chat))
            {
                await BotActions.DoPeriodicallyActionsAsync(bot, chat, cts);
            }
        };
        System.Threading.Timer timer = new(tm, null, 0, (int)period.TotalMilliseconds);
    }
}
