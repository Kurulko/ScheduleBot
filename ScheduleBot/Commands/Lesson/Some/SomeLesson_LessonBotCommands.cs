using ScheduleBot.Bot;
using ScheduleBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.Lesson.Some;

public abstract record SomeLesson_LessonBotCommands : LessonBotCommands
{
    public SomeLesson_LessonBotCommands(Command command) : base(command) { }

    protected string SomeLesson(bool isNext)
    {
        Regex regex = new(Command.RegEx!);
        if (!regex.IsMatch(CurrentCommandStr))
            throw BotScheduleException.IncorrectExpression(); ;

        string numberOfLessonStr = regex.Match(CurrentCommandStr).Groups[1].Value;
        bool result = int.TryParse(numberOfLessonStr, out int numberOfLesson);
        if (!result)
            throw BotScheduleException.IncorrectExpression();
        numberOfLesson++;
        return GetSomeLessonByNumberStr(isNext ? numberOfLesson : -1 * numberOfLesson);
    }

}
