using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Bot;

public record Command(string Name, string? Description, string? RegEx = null, bool IsPopular = false, bool IsPeriodicallyAction = false)
{
    public bool IsRegEx => RegEx is not null;
}
