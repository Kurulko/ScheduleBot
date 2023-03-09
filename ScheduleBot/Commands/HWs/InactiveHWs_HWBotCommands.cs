using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record InactiveHWs_HWBotCommands : HWBotCommands
{
    public InactiveHWs_HWBotCommands() : base(new Command("/inactive_hws", "Inactive homeworks")) { }
    protected override string ResponseHWsStr()
        => GetHWsStr(hw => hw.Deadline < DateTime.Now);
}
