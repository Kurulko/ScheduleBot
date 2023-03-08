using ScheduleBot.Bot;
using ScheduleBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs.Some;

public abstract record SomeDayHWs_HWBotCommands : HWBotCommands
{
    public SomeDayHWs_HWBotCommands(Command command) : base(command) { }
    protected string SomeHWs(bool isNext)
    {
        Regex regex = new(Command.RegEx!);
        if (!regex.IsMatch(CurrentCommandStr))
            throw BotScheduleException.IncorrectExpression(); ;

        string numberOfLessonStr = regex.Match(CurrentCommandStr).Groups[1].Value;
        bool result = int.TryParse(numberOfLessonStr, out int numberOfLesson);
        if (!result)
            throw BotScheduleException.IncorrectExpression(); ;
        return GetHWsStr(hw => hw.Deadline.Day == DateTime.Now.AddDays(isNext ? numberOfLesson : -1 * numberOfLesson).Day);
    }

}
