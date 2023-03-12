using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleBot.Commands.Interfaces;

namespace ScheduleBot.Commands.Main;

public abstract record MainBotCommand : OnceBotCommand
{
    public MainBotCommand(Command command) : base(command) { }
}
