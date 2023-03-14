using ScheduleBot.Bot;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs.Some;

public abstract record DayAfterEvents_EventBotCommand : SomeDayEvents_EventBotCommand
{
    public DayAfterEvents_EventBotCommand(Command command, params TypeOfEvent[] types) : base(command, types) { }

    protected override string ResponseEventsStr()
        => SomeEvents(true);
}
