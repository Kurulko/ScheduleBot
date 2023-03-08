using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record AllHWs_HWBotCommands : HWBotCommands
{
    public AllHWs_HWBotCommands() : base(new Command("/all_hws", "All homeworks")) { }
    protected override string ResponseHWsStr()
        => GetHWsStr(hw => true);
}
