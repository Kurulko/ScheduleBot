using ScheduleBot.Bot;
using ScheduleBot.Commands.Events;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record Hws_InactiveEvents_EventBotCommand : InactiveEvents_EventBotCommand
{
    public Hws_InactiveEvents_EventBotCommand() : base(new Command("/inactive_hws", "Inactive homeworks"), TypesOfEvents.TypesOfHws) { }
}
