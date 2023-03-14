using ScheduleBot.Bot;
using ScheduleBot.Extensions;
using ScheduleBot.Settings;
using ServiceStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Commands.Interfaces;

public abstract record OnceBotCommand : BotCommand
{
    public OnceBotCommand(Command command) : base(command) { }

    protected string currentCommandStr = null!;

    public override bool IsExistCommand(string currentName)
    {
        bool isExistCommand = Command.IsRegEx ? IsCommandByRegex(currentName) : IsCommand(currentName);

        if (isExistCommand)
            currentCommandStr = currentName.Contains(nameOfBot) ? currentName.Replace(nameOfBot, null) : currentName;

        return isExistCommand;
    }
    bool IsCommand(string currentName)
    {
        string commandNameLow = Command.Name.ToLower(), currentNameLow = currentName.ToLower();
        return commandNameLow == currentNameLow || currentNameLow == (commandNameLow + nameOfBot).ToLower();
    }
    bool IsCommandByRegex(string currentName)
        => new Regex(Command.RegEx!, RegexOptions.IgnoreCase).IsMatch(currentName);

    protected abstract string ResponseStr();

    public override async Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, int? replyToMessageId = null)
    {
        string responseStr = ResponseStr();
        return await SendTextMessagesIfResponseMoreMaxLengthAsync(bot, chatId, cts, responseStr, replyToMessageId);
    }
}
