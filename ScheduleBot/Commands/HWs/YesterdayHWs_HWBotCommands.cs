﻿using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record YesterdayHWs_HWBotCommands : HWBotCommands
{
    public YesterdayHWs_HWBotCommands() : base(new Command("/yesterday_hws", "Deadline was yesterday, homeworks")) { }
    protected override string ResponseHWsStr()
        => GetHWsStr(hw => hw.Deadline.Day == DateTime.Now.Day - 1);
}
