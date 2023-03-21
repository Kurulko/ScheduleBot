using Azure;
using ScheduleBot.Database;
using ScheduleBot.Database.Models;
using ScheduleBot.Exceptions;
using ScheduleBot.Extensions;
using ScheduleBot.Services.Common;
using ScheduleBot.Settings;
using ServiceStack.Messaging;
using ServiceStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleBot.Bot;

public class StartBot
{
    readonly ITelegramBotClient bot = TelegramSettings.CurrentBot();

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
        ChatService chatService = new();
        if (chatService.GetChatByChat(tgChat.Chat) is null)
            chatService.AddModel(tgChat);
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

                await BotActions.DoActionAsync(bot, messageText, chatId, replyToMessageId, cts);
            }
            catch (BotScheduleException botExc)
            {
                string messageExc = botExc.Message;
                await SendMessageAsync(messageExc, chatId, replyToMessageId, cts);
            }
            catch (ApiRequestException ex)
            {
                int delay = ex.Parameters?.RetryAfter ?? 0;
                if (delay > 0)
                {
                    ConsoleExtensions.WriteLineWithColor($"Error: Too many requests. Waiting for {delay} seconds.", ConsoleColor.DarkYellow);
                    Task.Delay(delay * 1000).Wait();
                }

                await SendMessageAsync(messageText, chatId, replyToMessageId, cts);
            }
            catch (Exception exc)
            {
                ConsoleExtensions.WriteLineWithColor($"Error: {exc.Message}", ConsoleColor.DarkBlue);
                //await StopBotAsync(cts);
            }
        }
    }
    async Task SendMessageAsync(string messageStr, long chatId, int replyToMessageId, CancellationTokenSource cts)
    {
        int maxLength = TelegramSettings.MaxLengthOfMessage;
        var responsesStr = messageStr.DevideStrIfMoreMaxLength(maxLength).ToList();
        Telegram.Bot.Types.Message message = new();
        for (int i = 0; i < responsesStr.Count(); i++)
        {
            message = await bot.SendTextMessageAsync(chatId, $"<b>{responsesStr[i]}</b>", ParseMode.Html, replyToMessageId: i == 0 ? replyToMessageId : message.MessageId, cancellationToken: cts.Token);
        }
    }

    Task HandlePollingErrorAsync(Exception exc, CancellationTokenSource cts)
    {
        if (exc is ApiRequestException ex)
        {
            int delay = ex.Parameters?.RetryAfter ?? 0;
            if (delay > 0)
            {
                ConsoleExtensions.WriteLineWithColor($"Error: Too many requests. Waiting for {delay} seconds.", ConsoleColor.Red);
                Task.Delay(delay * 1000).Wait();
            }
        }
        else
            ConsoleExtensions.WriteLineWithColor($"Error: {exc.Message}", ConsoleColor.Red);

        return Task.CompletedTask;
        //await StopBotAsync(cts);
    }

    async Task StopBotAsync(CancellationTokenSource cts)
    {
        User me = await bot.GetMeAsync();
        string fName = me.FirstName;
        Console.WriteLine($"\"{fName}\" finished listening ...");
        cts.Cancel();
    }
}
