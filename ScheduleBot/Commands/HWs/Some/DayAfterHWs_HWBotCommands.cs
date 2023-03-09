using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs.Some;

public record DayAfterHWs_HWBotCommands : SomeDayHWs_HWBotCommands
{
    public DayAfterHWs_HWBotCommands() : base(new Command("/hws_after_today_{number}", "..", @"\/hws_after_today_(\d)")) { }
    protected override string ResponseHWsStr()
        => SomeHWs(true);
}
