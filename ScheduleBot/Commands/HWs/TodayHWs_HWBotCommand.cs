using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record TodayHWs_HWBotCommand : HWBotCommand
{
    public TodayHWs_HWBotCommand() : base(new Command("/today_hws", "Deadline is today, homeworks")) { }
    protected override string ResponseHWsStr()
        => GetHWsStr(hw => hw.Deadline.Day == DateTime.Now.Day);
}
