using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure;
using ScheduleBot.Commands;
using ScheduleBot.Commands.Help;
using ScheduleBot.Commands.HWs;
using ScheduleBot.Commands.HWs.Some;
using ScheduleBot.Commands.Interfaces;
using ScheduleBot.Commands.Lesson;
using ScheduleBot.Commands.Lesson.Near;
using ScheduleBot.Commands.Lesson.Some;
using ScheduleBot.Commands.Main;
using ScheduleBot.Exceptions;
using ScheduleBot.Timer;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Bot;

public class BotActions
{
    static IEnumerable<BotCommands> AllActions
        => new List<BotCommands> { new PreviousLesson_LessonBotCommands(), new CurrentLesson_LessonBotCommands(), new NextLesson_LessonBotCommands(), new AllLessons_LessonBotCommands(), new SomePreviousLesson_LessonBotCommands(), new SomeNextLesson_LessonBotCommands(), new TodayHWs_HWBotCommands(), new ActiveHWs_HWBotCommands(), new InactiveHWs_HWBotCommands(), new AllHWs_HWBotCommands(), new YesterdayHWs_HWBotCommands(), new TomorrowHWs_HWBotCommands(), new DayBeforeHWs_HWBotCommands(), new DayAfterHWs_HWBotCommands(), new   Start_MainBotCommands(), new Stop_MainBotCommands(), new HelpBotCommands() };

    static BotCommands? GetActionByName(string actionName)
        => AllActions.FirstOrDefault(m => m.IsExistCommand(actionName));

    public static async Task DoActionAsync(ITelegramBotClient bot, string actionName, ChatId chatId, int replyToMessageId, CancellationTokenSource cts)
    {
        BotCommands? action = GetActionByName(actionName);

        if (action is null)
            throw BotScheduleException.ActionNotExist(actionName);

        if (action is IAllActions botWithAllActions)
            botWithAllActions.AllActions = AllActions;

        await action.SendResponseHtml(bot, chatId, cts, replyToMessageId);
    }

    public static void DoPeriodicallyActions(ITelegramBotClient bot, long chatId, CancellationTokenSource cts)
    {
        TimeSpan period = new(0, 0, 10);

        foreach (var action in AllActions)
        {
            if (action is IPeriodicallyAction periodicallyAction)
            {
                TimerAsync timerAsync = new(() => periodicallyAction.DoPeriodicallyActionInTelegram(bot, chatId, cts), period, cts);
                try
                {
                    timerAsync.Start();
                }
                catch
                {
                    timerAsync.Stop();
                    throw;
                }
            }
        }
    }
}

