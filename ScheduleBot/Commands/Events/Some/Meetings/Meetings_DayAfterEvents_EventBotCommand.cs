using ScheduleBot.Bot;
using ScheduleBot.Commands.Events;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs.Some;

public record Meetings_DayAfterEvents_EventBotCommand : DayAfterEvents_EventBotCommand
{
    public Meetings_DayAfterEvents_EventBotCommand() : base(new Command("/meetings_after_today_{number}", "..", @"\/meetings_after_today_(\d{1,3})"), TypesOfEvents.TypesOfMeetings) { }
}
