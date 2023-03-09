using ScheduleBot.Bot;
using ScheduleBot.Commands.Interfaces;
using ScheduleBot.Database.Models;
using ScheduleBot.Exceptions;
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

    public async Task DoPeriodicallyActionInTelegramAsync(ITelegramBotClient bot, long chatId, CancellationTokenSource cts)
    {

        await Task.Run(SendCurrentLessonAndDeleteWhenFinish);

        async void SendCurrentLessonAndDeleteWhenFinish()
        {
            bool isSend = false;
            int? messageId = null;
            TimeLesson2? previousTimeLesson = null;

            while (true)
            {
                bool isNowLesson = IsNowLesson(out TimeLesson2? currentTimeLesson);

                if (currentTimeLesson != previousTimeLesson)
                {
                    if (isSend)
                    {
                        TimeOnly endTime = previousTimeLesson!.EndTime;
                        TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);

                        if (now.Minute == endTime.Minute && now.Hour == endTime.Hour)
                        {
                            if (messageId is not null)
                            {
                                await bot.DeleteMessageAsync(chatId, messageId.Value, cancellationToken: cts.Token);
                                isSend = false;
                            }
                        }
                        if (!isSend)
                            continue;
                    }

                    if (isNowLesson && !isSend)
                    {
                        string responseStr = GetLessonStrByTimeLesson(currentTimeLesson!);
                        messageId = (await bot.SendTextMessageAsync(chatId, responseStr, ParseMode.Html, cancellationToken: cts.Token)).MessageId;
                        isSend = true;
                        previousTimeLesson = currentTimeLesson;
                    }
                }
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
