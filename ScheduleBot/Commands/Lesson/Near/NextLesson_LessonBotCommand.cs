using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.Lesson.Near;

public record NextLesson_LessonBotCommand : NearLesson_LessonBotCommand
{
    public NextLesson_LessonBotCommand() : base(new Command("/next_lesson", "Next lesson")) { }

    protected internal override string ResponseLessonStr()
        => GetSomeLessonByNumberStr(+1);
}
