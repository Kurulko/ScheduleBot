using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Commands;

public abstract record BotCommands(Command Command)
{

    Command? IsCommand(string currentName)
        => Command.Name.ToLower() == currentName.ToLower() || currentName.ToLower() == (Command.Name + nameOfBot).ToLower());
    bool IsCommandByRegex(string currentName)
    {
        Command? currentCommand = null;
        var regexCommands = Commands.Where(c => c.IsRegEx);
        foreach (var regexCommand in regexCommands)
        {
            Regex regex = new(regexCommand.RegEx!);
            if (regex.IsMatch(currentName))
            {
                currentCommand = regexCommand;
                break;
            }
        }
        return currentCommand;
    }
    public bool IsExistCommand(string currentName)
    {
        Command? currentCommand = GetCurrentCommand(currentName);

        if (currentCommand is null)
            currentCommand = GetCurrentCommandByRegex(currentName);

        bool result = currentCommand is not null;

        if (result)
        {
            CurrentCommandStr = currentName.Contains(nameOfBot) ? currentName.Replace(nameOfBot, null) : currentName;
        }

        return result;
    }

    protected abstract string ResponseStr();

    public virtual async Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, int? replyToMessageId = null)
    {
        string responseStr = ResponseStr();
        return await bot.SendTextMessageAsync(chatId, responseStr, ParseMode.Html, replyToMessageId: replyToMessageId, cancellationToken: cts.Token);
    }
}
