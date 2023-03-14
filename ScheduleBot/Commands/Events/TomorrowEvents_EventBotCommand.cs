using ScheduleBot.Bot;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public abstract record TomorrowEvents_EventBotCommand : EventBotCommand
{
    public TomorrowEvents_EventBotCommand(Command command, params TypeOfEvent[] types) : base(command, types) { }

    protected override string ResponseEventsStr()
        => GetEventsStr(hw => hw.Deadline.Day == DateTime.Now.Day + 1);
}
