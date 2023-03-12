using ScheduleBot.Bot;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.Lesson.Near;

public abstract record NearLesson_LessonBotCommand : LessonBotCommand
{
    public NearLesson_LessonBotCommand(Command command) : base(command) { }

    protected internal bool IsNowLesson(out TimeLesson2? timeLesson)
    {
        timeLesson = GetTimeLessonByDateTime(DateTime.Now);
        return timeLesson is not null;
    }
}
