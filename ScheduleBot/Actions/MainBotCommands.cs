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

public abstract record MainBotCommands : BotCommandsWithAllActions
{
    public MainBotCommands(Command command) : base(command) { }
}

public record Start_MainBotCommands : MainBotCommands
{
    const string start = "/start";
    public Start_MainBotCommands() : base(new Command(start, "Start bot")) { }

    protected override string ResponseStr()
        => "Welcome to my app";
    //string StartStr(ref ReplyKeyboardMarkup? replyKeyboardMarkup)
    //{
    //    if (AllActions?.Any() ?? false)
    //    {
    //        IEnumerable<KeyboardButton> popularActions = AllActions.Select(a => a.Commands.AsEnumerable()).Aggregate((previousCommands, currentCommands) => previousCommands.Union(currentCommands)).Where(c => c.IsPopular).Select(c => new KeyboardButton(c.Name));
    //        replyKeyboardMarkup = new(popularActions) { ResizeKeyboard = true };
    //    }
    //    return "Welcome to my app";
    //}

    //public override async Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, int? replyToMessageId = null)
    //{
    //    string responseStr = ResponseStr();
    //    if (replyToMessageId is not null)
    //        return await bot.SendTextMessageAsync(chatId, $"<b>{responseStr}</b>", ParseMode.Html, replyMarkup: replyKeyboardMarkup, replyToMessageId: replyToMessageId, cancellationToken: cts.Token);
    //    return await bot.SendTextMessageAsync(chatId, $"<b>{responseStr}</b>", ParseMode.Html, replyMarkup: replyKeyboardMarkup, cancellationToken: cts.Token);
    //}
}
public record Stop_MainBotCommands : MainBotCommands
{
    const string start = "/start";
    public Stop_MainBotCommands() : base(new Command(start, "Start bot")) { }

    protected override string ResponseStr()
    => "Good bye!";
}
