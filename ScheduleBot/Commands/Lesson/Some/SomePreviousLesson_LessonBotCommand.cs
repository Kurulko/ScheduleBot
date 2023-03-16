using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.Lesson.Some;

public record SomePreviousLesson_LessonBotCommand : SomeLesson_LessonBotCommand
{
    public SomePreviousLesson_LessonBotCommand() : base(new Command("/previous_lesson_{number}", "{number} of the previous lesson", @"\/previous_lesson_(\d{1,3})")) { }

    protected internal override string ResponseLessonStr()
        => SomeLesson(false);
}
