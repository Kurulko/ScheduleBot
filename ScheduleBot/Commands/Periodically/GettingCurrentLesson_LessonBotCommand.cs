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

namespace ScheduleBot.Commands.Periodically;

public record GettingCurrentLesson_LessonBotCommand : PeriodicallyBotCommand
{
    public GettingCurrentLesson_LessonBotCommand() : base(new Command("/getting_current_lesson", "Getting the current lesson and delete when It finishes")) { }


    public override void DoPeriodicallyActionInTelegram(ITelegramBotClient bot, long chatId, CancellationTokenSource cts)
    {
        (this.bot, this.chatId, this.cts) = (bot, chatId, cts);
        SendCurrentLessonAndDeleteWhenFinish();
    }

    ITelegramBotClient bot = null!; long chatId; CancellationTokenSource cts = null!; bool isSend = false; int? messageId = null; TimeLesson2? previousTimeLesson = null; bool isOnceSendBreak = false;

    static object locker = new();
    /*async*/ void SendCurrentLessonAndDeleteWhenFinish()
    {
        lock (locker)
        {
            if (this.cts.IsCancellationRequested)
                return;

            CurrentLesson_LessonBotCommand currentLesson_LessonBotCommand = new();

            bool isNowLesson = currentLesson_LessonBotCommand.IsNowLesson(out TimeLesson2? currentTimeLesson);
            bool isCanSend = true;
            bool isBreakNow = (currentTimeLesson is not null || previousTimeLesson is not null) && currentLesson_LessonBotCommand.IsNowBreak(previousTimeLesson is null ? currentTimeLesson! : previousTimeLesson!);
            if (currentTimeLesson != this.previousTimeLesson || isBreakNow)
            {
                if (this.isSend)
                {
                    TimeOnly endTime = this.previousTimeLesson!.SecondPartEndTime;
                    TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);

                    if (((now.Minute == endTime.Minute && now.Hour == endTime.Hour) || isBreakNow) && !isOnceSendBreak)
                    {
                        if (this.messageId is not null)
                        {
                            this.bot.DeleteMessageAsync(this.chatId, this.messageId.Value, cancellationToken: cts.Token).GetAwaiter().GetResult();
                            //await this.bot.DeleteMessageAsync(this.chatId, this.messageId.Value, cancellationToken: cts.Token);
                            isSend = false;
                        }
                    }
                    if (!this.isSend)
                    {
                        if (!isBreakNow)
                            isCanSend = false;
                        else
                            isOnceSendBreak = true;
                    }
                }

                if (isNowLesson && !this.isSend && isCanSend)
                {
                    string responseStr = currentLesson_LessonBotCommand.ResponseLessonStr();
                    this.messageId = (this.bot.SendTextMessageAsync(this.chatId, responseStr!, ParseMode.Html, cancellationToken: cts.Token).Result).MessageId;
                    //this.messageId = (await this.bot.SendTextMessageAsync(this.chatId, responseStr!, ParseMode.Html, cancellationToken: cts.Token)).MessageId;
                    this.isSend = true;
                    this.previousTimeLesson = currentTimeLesson;
                }
            }
        }
    }
    //async void SendCurrentLessonAndDeleteWhenFinish()
    //{
    //    if (this.cts.IsCancellationRequested)
    //        return;

    //    CurrentLesson_LessonBotCommand currentLesson_LessonBotCommand = new();

    //    bool isNowLesson = currentLesson_LessonBotCommand.IsNowLesson(out TimeLesson2? currentTimeLesson);
    //    bool isCanSend = true;

    //    if (currentTimeLesson != this.previousTimeLesson)
    //    {
    //        if (this.isSend)
    //        {
    //            TimeOnly endTime = this.previousTimeLesson!.SecondPartEndTime;
    //            TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);

    //            if (now.Minute == endTime.Minute && now.Hour == endTime.Hour)
    //            {
    //                if (this.messageId is not null)
    //                {
    //                    await this.bot.DeleteMessageAsync(this.chatId, this.messageId.Value, cancellationToken: cts.Token);
    //                    isSend = false;
    //                }
    //            }
    //            if (!this.isSend)
    //                isCanSend = false;
    //        }

    //        if (isNowLesson && !this.isSend && isCanSend)
    //        {
    //            string responseStr = currentLesson_LessonBotCommand.GetLessonStrByTimeLesson(currentTimeLesson!);
    //            this.messageId = (await this.bot.SendTextMessageAsync(this.chatId, responseStr, ParseMode.Html, cancellationToken: cts.Token)).MessageId;
    //            this.isSend = true;
    //            this.previousTimeLesson = currentTimeLesson;
    //        }
    //    }
    //}

}
