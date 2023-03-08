using ScheduleBot.Bot;
using ScheduleBot.Database.Models;
using ScheduleBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.Lesson.Near;

public record CurrentLesson_LessonBotCommands : NearLesson_LessonBotCommands
{
    public CurrentLesson_LessonBotCommands() : base(new Command("/current_lesson", "Current lesson", IsPopular: true, IsPeriodicallyAction: true)) { }

    protected override string ResponseLessonStr()
    {
        bool isNowLesson = IsNowLesson(out TimeLesson2? currentTimeLesson);
        if (!isNowLesson)
            throw BotScheduleException.LessonNotFound();
        return GetLessonStrByTimeLesson(currentTimeLesson!);
    }
}
