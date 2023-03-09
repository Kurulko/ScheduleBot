using ScheduleBot.Bot;
using ScheduleBot.Settings;
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
    public string CurrentCommandStr { get; set; } = null!;
    readonly string nameOfBot = $"@{SettingsTelegram.GetCurrentBotName().Result}";


    public bool IsExistCommand(string currentName)
    {
        bool isExistCommand = Command.IsRegEx ? IsCommandByRegex(currentName) : IsCommand(currentName);

        if (isExistCommand)
            CurrentCommandStr = currentName.Contains(nameOfBot) ? currentName.Replace(nameOfBot, null) : currentName;

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

    public virtual async Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, int? replyToMessageId = null)
    {
        string responseStr = ResponseStr();
        return await bot.SendTextMessageAsync(chatId, responseStr, ParseMode.Html, replyToMessageId: replyToMessageId, cancellationToken: cts.Token);
    }
}
