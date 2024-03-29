﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ScheduleBot.Bot;
using ScheduleBot.Commands.Events;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs.Some;

public record HWs_DayBeforeEvents_EventBotCommand : DayBeforeEvents_EventBotCommand
{
    public HWs_DayBeforeEvents_EventBotCommand() : base(new Command("/hws_before_today_{number}", "Deadline was {number} days ago, homeworks", @"\/hws_before_today_(\d{1,3})"), TypesOfEvents.TypesOfHws) { }
}
