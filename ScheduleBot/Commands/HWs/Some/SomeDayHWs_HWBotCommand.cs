using ScheduleBot.Bot;
using ScheduleBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs.Some;

public abstract record SomeDayHWs_HWBotCommand : HWBotCommand
{
    public SomeDayHWs_HWBotCommand(Command command) : base(command) { }
    protected string SomeHWs(bool isNext)
    {
        Regex regex = new(Command.RegEx!);
        if (!regex.IsMatch(currentCommandStr))
            throw BotScheduleException.IncorrectExpression(); ;

        string numberOfLessonStr = regex.Match(currentCommandStr).Groups[1].Value;
        bool result = int.TryParse(numberOfLessonStr, out int numberOfLesson);
        if (!result)
            throw BotScheduleException.IncorrectExpression(); ;
        return GetHWsStr(hw => hw.Deadline.Day == DateTime.Now.AddDays(isNext ? numberOfLesson : -1 * numberOfLesson).Day);
    }

}
