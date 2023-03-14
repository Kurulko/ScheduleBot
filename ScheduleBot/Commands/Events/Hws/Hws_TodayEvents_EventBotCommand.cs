using ScheduleBot.Bot;
using ScheduleBot.Commands.Events;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record Hws_TodayEvents_EventBotCommand : TodayEvents_EventBotCommand
{
    public Hws_TodayEvents_EventBotCommand() : base(new Command("/today_hws", "Deadline is today, homeworks"), TypesOfEvents.TypesOfHws) { }
}
