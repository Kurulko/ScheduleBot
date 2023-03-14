using ScheduleBot.Bot;
using ScheduleBot.Commands.Events;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record Hws_TomorrowEvents_EventBotCommand : TomorrowEvents_EventBotCommand
{
    public Hws_TomorrowEvents_EventBotCommand() : base(new Command("/tomorrow_hws", "Deadline'll be tomorrow, homeworks"), TypesOfEvents.TypesOfHws) { }
}
