using ScheduleBot.Bot;
using ScheduleBot.Commands.Events;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record Hws_ActiveEvents_EventBotCommand : ActiveEvents_EventBotCommand
{
    public Hws_ActiveEvents_EventBotCommand() : base(new Command("/active_hws", "Active homeworks", IsPopular: true), TypesOfEvents.TypesOfHws) { }
}
