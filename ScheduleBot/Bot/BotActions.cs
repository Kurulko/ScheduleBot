﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure;
using ScheduleBot.Commands;
using ScheduleBot.Commands.Help;
using ScheduleBot.Commands.HWs;
using ScheduleBot.Commands.HWs.Some;
using ScheduleBot.Commands.Lesson;
using ScheduleBot.Commands.Lesson.Near;
using ScheduleBot.Commands.Lesson.Some;
using ScheduleBot.Commands.Main;
using ScheduleBot.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Bot;

public class BotActions
{
    static IEnumerable<BotCommands> AllActions
        => new List<BotCommands> { new HelpBotCommands(), new DayAfterHWs_HWBotCommands(), new DayBeforeHWs_HWBotCommands(), new ActiveHWs_HWBotCommands(), new AllHWs_HWBotCommands(), new InactiveHWs_HWBotCommands(), new TodayHWs_HWBotCommands(), new TomorrowHWs_HWBotCommands(), new YesterdayHWs_HWBotCommands(), new SomeNextLesson_LessonBotCommands(), new SomePreviousLesson_LessonBotCommands(), new AllLessons_LessonBotCommands(), new CurrentLesson_LessonBotCommands(), new NextLesson_LessonBotCommands(), new PreviousLesson_LessonBotCommands(), new Start_MainBotCommands(), new Stop_MainBotCommands(), };

    static BotCommands? GetActionByName(string actionName)
        => AllActions.FirstOrDefault(m => m.Command.Name == actionName);

    public static async Task DoActionAsync(ITelegramBotClient bot, string actionName, ChatId chatId, int replyToMessageId, CancellationTokenSource cts)
    {
        BotCommands? action = GetActionByName(actionName);

        if (action is null)
            throw BotScheduleException.ActionNotExist(actionName);

        if (action is BotCommandsWithAllActions botWithAllActions)
            botWithAllActions.AllActions = AllActions;

        await action.SendResponseHtml(bot, chatId, cts, replyToMessageId);
    }

    public static async Task DoPeriodicallyActionsAsync(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts)
    {
        List<Command> commands = new();
        var allActions = AllActions;

        //var allCommands = allActions.Select(actions => actions.Commands.Where(c => c.IsPeriodicallyAction));
        //foreach (var allCommand in allCommands)
        //    commands.AddRange(allCommand);

        //foreach (var command in commands)
        //    await allActions.First(m => m.IsExistCommand(command.Name)).SendResponseHtml(bot, chatId, cts);
    }
}

