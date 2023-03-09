using ScheduleBot.Bot;
using ScheduleBot.Commands.Interfaces;
using ScheduleBot.Database.Models;
using ScheduleBot.Exceptions;
using ScheduleBot.Timer;
using ServiceStack;
using ServiceStack.Messaging;
using ServiceStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Message = Telegram.Bot.Types.Message;

namespace ScheduleBot.Commands.Lesson.Near;

public record CurrentLesson_LessonBotCommands : NearLesson_LessonBotCommands, IPeriodicallyAction
{
    public CurrentLesson_LessonBotCommands() : base(new Command("/current_lesson", "Current lesson", IsPopular: true)) { }


    public void DoPeriodicallyActionInTelegram(ITelegramBotClient bot, long chatId, CancellationTokenSource cts)
    {
        (this.bot, this.chatId, this.cts) = (bot, chatId, cts);
        SendCurrentLessonAndDeleteWhenFinish();
    }

    ITelegramBotClient bot = null!; long chatId; CancellationTokenSource cts = null!; bool isSend = false; int? messageId = null; TimeLesson2? previousTimeLesson = null;

    async void SendCurrentLessonAndDeleteWhenFinish()
    {
        bool isNowLesson = IsNowLesson(out TimeLesson2? currentTimeLesson);
        bool isCanSend = true;

        if (currentTimeLesson != this.previousTimeLesson)
        {
            if (this.isSend)
            {
                TimeOnly endTime = this.previousTimeLesson!.EndTime;
                TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);

                if (now.Minute == endTime.Minute && now.Hour == endTime.Hour)
                {
                    if (this.messageId is not null)
                    {
                        await this.bot.DeleteMessageAsync(this.chatId, this.messageId.Value, cancellationToken: cts.Token);
                        isSend = false;
                    }
                }
                if (!this.isSend)
                    isCanSend = false;
            }

            if (isNowLesson && !this.isSend && isCanSend)
            {
                string responseStr = GetLessonStrByTimeLesson(currentTimeLesson!);
                this.messageId = (await this.bot.SendTextMessageAsync(this.chatId, responseStr, ParseMode.Html, cancellationToken: cts.Token)).MessageId;
                this.isSend = true;
                this.previousTimeLesson = currentTimeLesson;
            }
        }
    }



    protected override string ResponseLessonStr()
    {
        bool isNowLesson = IsNowLesson(out TimeLesson2? currentTimeLesson);
        if (!isNowLesson)
            throw BotScheduleException.LessonNotFound();
        return GetLessonStrByTimeLesson(currentTimeLesson!);
    }
}
