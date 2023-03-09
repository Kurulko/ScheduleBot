using ScheduleBot.Commands;
using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.Main;

public abstract record MainBotCommands : BotCommands
{
    public MainBotCommands(Command command) : base(command) { }
}
