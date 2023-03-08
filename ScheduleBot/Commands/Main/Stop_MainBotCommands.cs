using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.Main;

public record Stop_MainBotCommands : MainBotCommands
{
    const string stop = "/stop";
    public Stop_MainBotCommands() : base(new Command(stop, "Stop bot")) { }

    protected override string ResponseStr()
    => "Good bye!";
}
