using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.Lesson.Some;

public record SomeNextLesson_LessonBotCommands : SomeLesson_LessonBotCommands
{
    public SomeNextLesson_LessonBotCommands() : base(new Command("/next_lesson_{number}", "...", @"\/next_lesson_(\d)")) { }

    protected override string ResponseLessonStr()
        => SomeLesson(true);
}
