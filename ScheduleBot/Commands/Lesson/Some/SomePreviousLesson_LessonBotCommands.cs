using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.Lesson.Some;

public record SomePreviousLesson_LessonBotCommands : SomeLesson_LessonBotCommands
{
    public SomePreviousLesson_LessonBotCommands() : base(new Command("/previous_lesson_{number}", "...", @"\/previous_lesson_(\d)")) { }

    protected override string ResponseLessonStr()
        => SomeLesson(false);
}
