using ScheduleBot.Commands.Interfaces;
using ScheduleBot.Commands.Lesson.Near;
using ScheduleBot.Database.Models;
using ScheduleBot.Database;
using ScheduleBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using ScheduleBot.Bot;
using ScheduleBot.Services.ByToken;
using ScheduleBot.Services.Common;
using Telegram.Bot.Types;
using ScheduleBot.Extensions;

namespace ScheduleBot.Commands.Periodically;

public record GettingCurrentLesson_LessonBotCommand : PeriodicallyBotCommand
{
    public GettingCurrentLesson_LessonBotCommand() : base(new Command("/getting_current_lesson", "Getting the current lesson and delete when It finishes")) { }

    static object locker = new();

    public override void DoPeriodicallyActionInTelegram(ITelegramBotClient bot, long chatId, CancellationTokenSource cts)
    {
        lock (locker)
        {
            SetServicesByChatId(currentLesson_LessonBotCommand, chatId);
            (this.bot, this.chatId, this.cts) = (bot, chatId, cts);
            SendCurrentLessonAndDeleteWhenFinish();

            if (cts.IsCancellationRequested)
                return;
        }
    }

    ITelegramBotClient bot = null!; long chatId; CancellationTokenSource cts = null!; bool isSend = false; int? messageId = null; CurrentLesson_LessonBotCommand currentLesson_LessonBotCommand = new(); bool wasPreviuosBreak = false;

    async void SendCurrentLessonAndDeleteWhenFinish()
    {
        if (this.cts.IsCancellationRequested)
            return;

        currentLesson_LessonBotCommand.IsNowLesson(out TimeLesson2? currentTimeLesson);
        bool isBreakNow = currentLesson_LessonBotCommand.IsNowBreak(currentTimeLesson);

        await DeleteMessageAsync(isBreakNow);
        await SendMessageAsync(isBreakNow);

        this.wasPreviuosBreak = isBreakNow;
    }
    async Task DeleteMessageAsync(bool isBreakNow)
    {
        if (this.wasPreviuosBreak != isBreakNow && this.isSend)
        {
            await this.bot.DeleteMessageAsync(this.chatId, this.messageId!.Value, cancellationToken: cts.Token);
            isSend = false;
        }
    }
    async Task SendMessageAsync(bool isBreakNow)
    {
        if (this.wasPreviuosBreak != isBreakNow && !this.isSend )
        {
            string responseStr = currentLesson_LessonBotCommand.ResponseLessonStr();
            this.messageId = (await this.bot.SendTextMessageAsync(this.chatId, responseStr!, ParseMode.Html, cancellationToken: cts.Token)).MessageId;

            this.isSend = true;
        }
    }
    public void SetServicesByChatId(Interfaces.BotCommand botCommand, ChatId chatId)
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
}
