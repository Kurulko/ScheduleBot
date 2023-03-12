using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs.Some;

public record DayAfterHWs_HWBotCommand : SomeDayHWs_HWBotCommand
{
    public DayAfterHWs_HWBotCommand() : base(new Command("/hws_after_today_{number}", "..", @"\/hws_after_today_(\d)")) { }
    protected override string ResponseHWsStr()
        => SomeHWs(true);
}
