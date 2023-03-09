using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.Lesson.Near;

public record NextLesson_LessonBotCommands : NearLesson_LessonBotCommands
{
    public NextLesson_LessonBotCommands() : base(new Command("/next_lesson", "Next lesson")) { }

    protected override string ResponseLessonStr()
        => GetSomeLessonByNumberStr(+1);
}
