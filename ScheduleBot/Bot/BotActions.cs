using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ScheduleBot.Actions;
using ScheduleBot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Bot;

public class BotActions
{
    static IEnumerable<BotCommands> GetAllActions()
        => new List<BotCommands> { new MainBotCommands(), new LessonBotCommands(), new HWBotCommands(), new HelpBotCommands()};

    static BotCommands? GetActionByName(string actionName)
    {
        BotCommands? action = GetAllActions().FirstOrDefault(m => m.IsExistCommand(actionName));
        return action;
    }

    public static async Task DoAction(ITelegramBotClient bot, string actionName, ChatId chatId, int replyToMessageId, CancellationTokenSource cts)
    {
        BotCommands? action = GetActionByName(actionName);

        if (action is null)
            throw BotScheduleException.ActionNotExist(actionName);

        if (action is BotCommandsWithAllActions botWithAllActions)
            botWithAllActions.AllActions = GetAllActions();

        await action.SendResponseHtml(bot, chatId, replyToMessageId, cts);
    }
}

