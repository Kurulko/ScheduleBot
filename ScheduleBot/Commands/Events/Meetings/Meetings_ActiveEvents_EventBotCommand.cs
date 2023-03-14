using ScheduleBot.Bot;
using ScheduleBot.Commands.Events;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record Meetings_ActiveEvents_EventBotCommand : ActiveEvents_EventBotCommand
{
    public Meetings_ActiveEvents_EventBotCommand() : base(new Command("/active_meetings", "Active meetings", IsPopular: true), TypesOfEvents.TypesOfMeetings) { }
}
