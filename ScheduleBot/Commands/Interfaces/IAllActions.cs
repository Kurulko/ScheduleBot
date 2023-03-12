using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.Interfaces;

public interface IAllActions
{
    IEnumerable<BotCommand> AllActions { get; set; }
}
