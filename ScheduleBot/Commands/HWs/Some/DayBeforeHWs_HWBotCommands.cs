using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs.Some;

public record DayBeforeHWs_HWBotCommands : SomeDayHWs_HWBotCommands
{
    public DayBeforeHWs_HWBotCommands() : base(new Command("/hws_before_today_{number}", "..", @"\/hws_before_today_(\d)")) { }
    protected override string ResponseHWsStr()
        => SomeHWs(false);
}
