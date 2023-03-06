using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Actions;

public abstract record BotCommandsWithAllActions : BotCommands
{
    public BotCommandsWithAllActions(params Command[] commands) : base(commands) { }
    public IEnumerable<BotCommands> AllActions { get; set; } = null!;
}
