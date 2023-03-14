using ScheduleBot.Bot;
using ScheduleBot.Commands.Events;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record Meetings_YesterdayEvents_EventBotCommand : YesterdayEvents_EventBotCommand
{
    public Meetings_YesterdayEvents_EventBotCommand() : base(new Command("/yesterday_hws", "Meetings were yesterday"), TypesOfEvents.TypesOfMeetings) { }

}
