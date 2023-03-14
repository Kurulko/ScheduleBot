using ScheduleBot.Bot;
using ScheduleBot.Commands.Events;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record Meetings_TodayEvents_EventBotCommand : TodayEvents_EventBotCommand
{
    public Meetings_TodayEvents_EventBotCommand() : base(new Command("/today_meetings", "Meetings are today"), TypesOfEvents.TypesOfMeetings) { }
}
