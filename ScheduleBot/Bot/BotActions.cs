using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure;
using ScheduleBot.Commands.Help;
using ScheduleBot.Commands.HWs;
using ScheduleBot.Commands.HWs.Some;
using ScheduleBot.Commands.Interfaces;
using ScheduleBot.Commands.Lesson;
using ScheduleBot.Commands.Lesson.Near;
using ScheduleBot.Commands.Lesson.Some;
using ScheduleBot.Commands.Main;
using ScheduleBot.Commands.Periodically;
using ScheduleBot.Database.Models;
using ScheduleBot.Database;
using ScheduleBot.Exceptions;
using ScheduleBot.Settings;
using ScheduleBot.Timer;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using BotCommand = ScheduleBot.Commands.Interfaces.BotCommand;
using ServiceStack.Model;
using System;
using System.Threading;
using ScheduleBot.Commands.Tokens;
using ScheduleBot.Services.Common;
using ScheduleBot.Services.ByToken;

namespace ScheduleBot.Bot;

public class BotActions
{
    static IEnumerable<BotCommand> ActionsWithoutToken = new List<BotCommand> { new Start_MainBotCommand(), new HelpBotCommand(), new AddTokenBotCommand() };

    static IEnumerable<BotCommand> AllActions
        => new List<BotCommand> { 
            new GettingCurrentLesson_LessonBotCommand(), 
            
            new PreviousLesson_LessonBotCommand(),
            new CurrentLesson_LessonBotCommand(), 
            new NextLesson_LessonBotCommand(), 
            new AllLessons_LessonBotCommand(), 
            new SomePreviousLesson_LessonBotCommand(), 
            new SomeNextLesson_LessonBotCommand(), 
            
            new Hws_ActiveEvents_EventBotCommand(), 
            new Hws_AllEvents_EventBotCommand(), 
            new Hws_InactiveEvents_EventBotCommand(), 
            new Hws_TodayEvents_EventBotCommand(), 
            new Hws_TomorrowEvents_EventBotCommand(), 
            new Hws_YesterdayEvents_EventBotCommand(), 
            new HWs_DayAfterEvents_EventBotCommand(), 
            new HWs_DayBeforeEvents_EventBotCommand(), 
            
            new Meetings_ActiveEvents_EventBotCommand(), 
            new Meetings_AllEvents_EventBotCommand(), 
            new Meetings_InactiveEvents_EventBotCommand(), 
            new Meetings_TodayEvents_EventBotCommand(), 
            new Meetings_TomorrowEvents_EventBotCommand(), 
            new Meetings_YesterdayEvents_EventBotCommand(), 
            new Meetings_DayAfterEvents_EventBotCommand(), 
            new Meetings_DayBeforeEvents_EventBotCommand(), 
            
            new Stop_MainBotCommand(),
            new UpdateDbBotCommand() 
        
        }.Union(ActionsWithoutToken);

    static BotCommand? GetActionByName(string actionName)
        => AllActions.FirstOrDefault(m => m.IsExistCommand(actionName));

    public static async Task DoActionAsync(ITelegramBotClient bot, string actionName, ChatId chatId, int replyToMessageId, CancellationTokenSource cts)
    {
        BotCommand? action = GetActionByName(actionName);

        if (!actionName.StartsWith('/'))
            return;

        if (action is null)
            throw BotScheduleException.ActionNotExist(actionName);

        bool isExistToken = await CheckToken(bot, chatId, replyToMessageId, cts, action!);
        if (!isExistToken)
            return;

        if (action is IAllActions botWithAllActions)
            botWithAllActions.AllActions = AllActions;

        SetServicesByChatId(action, chatId);

        await action.SendResponseHtml(bot, chatId, cts, replyToMessageId);
    }
    static void SetServicesByChatId(BotCommand botCommand, ChatId chatId)
    {
        TokenService tokenService = new();
        Token? token = tokenService.GetTokenByChat(chatId.Identifier!.Value);

        if (token is null)
            return;

        long tokenId = token!.Id;
        botCommand.breakService = new BreakServiceByToken(tokenId);
        botCommand.conferenceService = new ConferenceServiceByToken(tokenId);
        botCommand.eventService = new EventServiceByToken(tokenId);
        botCommand.subjectService = new SubjectServiceByToken(tokenId);
        botCommand.teacherService = new TeacherServiceByToken(tokenId);
        botCommand.timeLessonService = new TimeLessonServiceByToken(tokenId);
    }

    static async Task<bool> CheckToken(ITelegramBotClient bot, ChatId chatId, int replyToMessageId, CancellationTokenSource cts, BotCommand action)
    {
        TokenService tokenService = new();
        Token? token = tokenService.GetTokenByChat(chatId.Identifier!.Value);
        bool result = token is not null || ActionsWithoutToken.Any(a => action.GetType() == a.GetType());

        if (!result)
        {
            AddTokenBotCommand tokenBot = new();
            string description = $"<b>Please, write your token before using this bot, like it</b>: \n<code>{tokenBot.Command.Name}</code>";
            await bot.SendTextMessageAsync(chatId, description, ParseMode.Html, replyToMessageId: replyToMessageId, cancellationToken: cts.Token);
        }

        return result;
    }
}

