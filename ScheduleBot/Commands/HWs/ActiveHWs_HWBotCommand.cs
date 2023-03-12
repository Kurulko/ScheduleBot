using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record ActiveHWs_HWBotCommand : HWBotCommand
{
    public ActiveHWs_HWBotCommand() : base(new Command("/active_hws", "Active homeworks", IsPopular: true)) { }
    protected override string ResponseHWsStr()
        => GetHWsStr(hw => hw.Deadline >= DateTime.Now);

}
