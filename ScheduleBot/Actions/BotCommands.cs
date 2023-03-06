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
    protected string currentCommandStr = null!;
    protected Command currentCommand = null!;
    public bool IsExistCommand(string currentName)
    {
        Command? currentCommand = Commands.FirstOrDefault(c => c.Name.ToLower() == currentName.ToLower());

        if (currentCommand is null)
        {
            var regexCommands = Commands.Where(c => c.IsRegEx);
            foreach (var regexCommand in regexCommands)
            {
                Regex regex = new(regexCommand.RegEx!);
                if(regex.IsMatch(currentName))
                {
                    currentCommand = regexCommand;
                    break;
                }
            }

        }

        bool result = currentCommand is not null;

        if (result)
        {
            currentCommandStr = currentName;
            this.currentCommand = currentCommand!;
        }

        return result;
    }

    protected abstract string ResponseStr();

    public virtual async Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, int replyToMessageId, CancellationTokenSource cts)
    {
        string responseStr = ResponseStr();
        return await bot.SendTextMessageAsync(chatId, responseStr, ParseMode.Html, replyToMessageId: replyToMessageId, cancellationToken: cts.Token);
    }
}
