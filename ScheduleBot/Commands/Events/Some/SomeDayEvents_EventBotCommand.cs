using ScheduleBot.Bot;
using ScheduleBot.Database.Models;
using ScheduleBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs.Some;

public abstract record SomeDayEvents_EventBotCommand : EventBotCommand
{
    public SomeDayEvents_EventBotCommand(Command command, params TypeOfEvent[] types) : base(command, types) { }

    protected string SomeEvents(bool isNext)
    {
        Regex regex = new(Command.RegEx!);
        if (!regex.IsMatch(currentCommandStr))
            throw BotScheduleException.IncorrectExpression(); ;

        string numberOfLessonStr = regex.Match(currentCommandStr).Groups[1].Value;
        bool result = int.TryParse(numberOfLessonStr, out int numberOfLesson);
        if (!result)
            throw BotScheduleException.IncorrectExpression(); ;
        return GetEventsStr(hw => hw.Deadline.Day == DateTime.Now.AddDays(isNext ? numberOfLesson : -1 * numberOfLesson).Day);
    }
}
