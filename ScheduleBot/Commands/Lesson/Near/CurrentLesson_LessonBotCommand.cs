using ScheduleBot.Bot;
using ScheduleBot.Commands.Interfaces;
using ScheduleBot.Database;
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

namespace ScheduleBot.Commands.Lesson.Near;

public record CurrentLesson_LessonBotCommand : NearLesson_LessonBotCommand
{
    public CurrentLesson_LessonBotCommand() : base(new Command("/current_lesson", "Current lesson", IsPopular: true)) { }

    protected internal override string ResponseLessonStr()
    {

        if (IsNowLesson(out TimeLesson2? currentTimeLesson))
        {
            string currentLessonStr = string.Empty;

            currentLessonStr += IfNowShortBreak(currentTimeLesson!);
            currentLessonStr += GetLessonStrByTimeLesson(currentTimeLesson!);

            return currentLessonStr;
        }

        string? nextLessonStr = IfNowBreak();
        if(!string.IsNullOrEmpty(nextLessonStr))
            return nextLessonStr!;

        throw BotScheduleException.LessonNotFound();
    }
    string? IfNowBreak()
    {
        TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);

        Break2? rest = breakService.GetBreaks().Select(b => (Break2)b).FirstOrDefault(b => now >= b.StartTime && now <= b.EndTime);
        if (rest is not null)
        {
            TimeLesson2? nextLesson = GetSomeLessonByNumber(+1);
            if (nextLesson.FirstPartStartTime == rest.EndTime)
                return $"Now is a break({rest.StartTime}-{rest.EndTime}), but after it, it'll be:\n\n{GetLessonStrByTimeLesson(nextLesson)}";
        }
        return default;
    }
    string IfNowShortBreak(TimeLesson2 currentTimeLesson)
    {
        TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);

        TimeOnly firstPartEndTime = currentTimeLesson!.FirstPartEndTime, secondPartStartTime = currentTimeLesson.SecondPartStartTime;
        if (now >= firstPartEndTime && now <= secondPartStartTime)
            return $"We have a shortly break now({firstPartEndTime}-{secondPartStartTime}), but after it, we'll continue:\n\n";
        return string.Empty;
    }
    internal bool IsNowBreak(TimeLesson2 currentTimeLesson)
        => !string.IsNullOrEmpty(IfNowShortBreak(currentTimeLesson)) || !string.IsNullOrEmpty(IfNowShortBreak(currentTimeLesson));
}
