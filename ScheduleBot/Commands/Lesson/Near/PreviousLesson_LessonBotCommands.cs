using ScheduleBot.Bot;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.Lesson.Near;

public record PreviousLesson_LessonBotCommands : NearLesson_LessonBotCommands
{
    public PreviousLesson_LessonBotCommands() : base(new Command("/previous_lesson", "Previous lesson")) { }

    protected override string ResponseLessonStr()
    {
        bool isNowLesson = IsNowLesson(out TimeLesson2? _);
        return GetSomeLessonByNumberStr(isNowLesson ? -1 : 0);
    }
}
