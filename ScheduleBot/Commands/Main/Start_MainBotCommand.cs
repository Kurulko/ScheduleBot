using ScheduleBot.Bot;
using ScheduleBot.Commands.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleBot.Commands.Main;

public record Start_MainBotCommand : MainBotCommand, IAllActions
{
    const string start = "/start";
    public Start_MainBotCommand() : base(new Command(start, "Start bot")) { }
    public IEnumerable<Interfaces.BotCommand> AllActions { get; set; } = null!;

    ReplyKeyboardMarkup? replyKeyboardMarkup;
    protected override string ResponseStr()
    {
        SetReplyKeyboardMarkup();
        return "Welcome to my app";
    }

    void SetReplyKeyboardMarkup()
    {
        if (AllActions?.Any() ?? false)
        {
            var popularActions = AllActions.Where(a => a.Command.IsPopular).Select(a => new KeyboardButton(a.Command.Name));
            replyKeyboardMarkup = new(popularActions) { ResizeKeyboard = true };
        }
    }

    public override async Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, int? replyToMessageId = null)
    {
        string responseStr = $"<b>{ResponseStr()}</b>";
        return await bot.SendTextMessageAsync(chatId, responseStr, ParseMode.Html, replyToMessageId: replyToMessageId, cancellationToken: cts.Token, replyMarkup: replyKeyboardMarkup);
    }
}
