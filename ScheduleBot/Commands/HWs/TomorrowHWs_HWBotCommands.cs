using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record TomorrowHWs_HWBotCommands : HWBotCommands
{
    public TomorrowHWs_HWBotCommands() : base(new Command("/tomorrow_hws", "Deadline'll be tomorrow, homeworks")) { }
    protected override string ResponseHWsStr()
        => GetHWsStr(hw => hw.Deadline.Day == DateTime.Now.Day + 1);
}
