using Azure;
using Microsoft.EntityFrameworkCore;
using ScheduleBot.Bot;
using ScheduleBot.Database;
using ScheduleBot.Database.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleBot.Actions;

public record MainBotCommands : BotCommandsWithAllActions
{
    const string start = "/start", stop = "/stop";
    public MainBotCommands() : base(new Command(start, "Start bot"), new Command(stop, "Stop bot")) { }

    ReplyKeyboardMarkup? replyKeyboardMarkup = default;
    protected override string ResponseStr()
    {
        string response = currentCommandStr.ToLower() switch
        {
            start => StartStr(ref replyKeyboardMarkup),
            stop => StopStr(),
            _ => string.Empty
        };

        return response; 
    }

    public override async Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, int replyToMessageId, CancellationTokenSource cts)
    {
        string responseStr = ResponseStr();
        return await bot.SendTextMessageAsync(chatId, $"<b>{responseStr}</b>", ParseMode.Html, replyMarkup: replyKeyboardMarkup, cancellationToken: cts.Token);
    }

    string StartStr(ref ReplyKeyboardMarkup? replyKeyboardMarkup)
    {
        if(AllActions?.Any() ?? false)
        {
            IEnumerable<KeyboardButton> popularActions = AllActions.Select(a => a.Commands.AsEnumerable()).Aggregate ((previousCommands, currentCommands) => previousCommands.Union(currentCommands)).Where(c => c.IsPopular).Select(c => new KeyboardButton(c.Name));
            replyKeyboardMarkup = new(popularActions) { ResizeKeyboard = true };
        }
        return "Welcome to my app";
    }

    string StopStr()
        => "Good bye!";
}
