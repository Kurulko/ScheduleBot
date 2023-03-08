using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands;

public abstract record BotCommandsWithAllActions : BotCommands
{
    public BotCommandsWithAllActions(Command command) : base(command) { }
    public IEnumerable<BotCommands> AllActions { get; set; } = null!;
}
