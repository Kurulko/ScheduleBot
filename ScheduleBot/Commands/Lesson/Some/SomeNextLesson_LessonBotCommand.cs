using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.Lesson.Some;

public record SomeNextLesson_LessonBotCommand : SomeLesson_LessonBotCommand
{
    public SomeNextLesson_LessonBotCommand() : base(new Command("/next_lesson_{number}", "...", @"\/next_lesson_(\d)")) { }

    protected internal override string ResponseLessonStr()
        => SomeLesson(true);
}
