using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record InactiveHWs_HWBotCommand : HWBotCommand
{
    public InactiveHWs_HWBotCommand() : base(new Command("/inactive_hws", "Inactive homeworks")) { }
    protected override string ResponseHWsStr()
        => GetHWsStr(hw => hw.Deadline < DateTime.Now);
}
