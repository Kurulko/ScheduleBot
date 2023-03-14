using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Exceptions;

public class BotScheduleException : Exception
{
    public BotScheduleException(string name) : base(name) { }

    public static BotScheduleException EventsNotFound(IEnumerable<TypeOfEvent> types)
    {
        string typeStr = types.Contains(TypeOfEvent.Homework) || types.Contains(TypeOfEvent.LaboratoryWork) ? "homeworks" : "meetings";
        return new($"The {typeStr} aren't found");
    }

    public static BotScheduleException LessonNotFound()
        => new("The lecture ins't found");
    public static BotScheduleException IncorrectExpression()
        => new("Incorrect expression");
    public static BotScheduleException IncorrectToken()
        => new("Incorrect token");
    public static BotScheduleException IncorrectValuesFromExcel()
    => new("Incorrect values in excel");
    public static BotScheduleException ActionNotExist(string actionName)
        => new($"Such an action \"{actionName}\" doesn't exist");
}
