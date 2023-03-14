﻿using ScheduleBot.Bot;
using ScheduleBot.Commands.Events;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.HWs;

public record Hws_AllEvents_EventBotCommand : AllEvents_EventBotCommand
{
    public Hws_AllEvents_EventBotCommand() : base(new Command("/all_hws", "All homeworks"), TypesOfEvents.TypesOfHws) { }
}
