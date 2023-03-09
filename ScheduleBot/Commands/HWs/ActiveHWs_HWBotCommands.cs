using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record ActiveHWs_HWBotCommands : HWBotCommands
{
    public ActiveHWs_HWBotCommands() : base(new Command("/active_hws", "Active homeworks", IsPopular: true)) { }
    protected override string ResponseHWsStr()
        => GetHWsStr(hw => hw.Deadline >= DateTime.Now);

}
