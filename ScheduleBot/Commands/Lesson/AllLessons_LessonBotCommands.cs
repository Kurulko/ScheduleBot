using ScheduleBot.Bot;
using ScheduleBot.Database.Models;
using ScheduleBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.Lesson;

public record AllLessons_LessonBotCommands : LessonBotCommands
{
    public AllLessons_LessonBotCommands() : base(new Command("/all_lessons", "All lessons")) { }

    protected override string ResponseLessonStr()
    {
        string result = string.Empty;

        var timeLessons = db.TimeLessons.OrderBy(tl => tl.DayOfWeek).ToList();
        int countOfTimeLessons = timeLessons.Count;

        if (countOfTimeLessons <= 0)
            throw BotScheduleException.LessonNotFound();

        DayOfWeek currentDayOfWeek = timeLessons[0].DayOfWeek;
        result += $"\n<b>{currentDayOfWeek}</b>\n\n";
        for (int i = 0; i < countOfTimeLessons; i++)
        {
            TimeLesson2 timeLesson = (TimeLesson2)timeLessons[i];

            if (timeLesson.DayOfWeek != currentDayOfWeek)
            {
                currentDayOfWeek = timeLesson.DayOfWeek;
                result += $"\n<b>{currentDayOfWeek}</b>\n\n";
            }

            string response = GetLessonStrByTimeLesson(timeLesson);
            if (!string.IsNullOrEmpty(response))
            {
                result += response;

                if (i != countOfTimeLessons - 1)
                {
                    if (timeLessons[i + 1].DayOfWeek != currentDayOfWeek)
                        result += "\n";
                    else
                        result += $"\n{new string('•', 36)}\n";
                }//ᐧ
            }
        }

        return result;

    }
}
