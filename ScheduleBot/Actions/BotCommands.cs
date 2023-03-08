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

namespace ScheduleBot.Actions;

public abstract record BotCommands(params Command[] Commands)
{
    public string CurrentCommandStr { get; set;  } = null!;
    protected Command currentCommand = null!;
    readonly string nameOfBot = $"@{SettingsTelegram.GetCurrentBotName().Result}";

    Command? GetCurrentCommand(string currentName)
        => Commands.FirstOrDefault(c => c.Name.ToLower() == currentName.ToLower() || currentName.ToLower() == (c.Name + nameOfBot).ToLower());
    Command? GetCurrentCommandByRegex(string currentName)
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
            this.currentCommand = currentCommand!;
        }

        return result;
    }

    protected abstract string ResponseStr();

    public virtual async Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, int? replyToMessageId = null)
    {
        string responseStr = ResponseStr();
        if(replyToMessageId is not null)
            return await bot.SendTextMessageAsync(chatId, responseStr, ParseMode.Html, replyToMessageId: replyToMessageId, cancellationToken: cts.Token);
        return await bot.SendTextMessageAsync(chatId, responseStr, ParseMode.Html, cancellationToken: cts.Token);
    }
}
