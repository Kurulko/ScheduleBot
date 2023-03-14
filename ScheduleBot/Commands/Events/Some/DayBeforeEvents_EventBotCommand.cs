using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ScheduleBot.Bot;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs.Some;

public abstract record DayBeforeEvents_EventBotCommand : SomeDayEvents_EventBotCommand
{
    public DayBeforeEvents_EventBotCommand(Command command, params TypeOfEvent[] types) : base(command, types) { }

    protected override string ResponseEventsStr()
        => SomeEvents(false);
}
