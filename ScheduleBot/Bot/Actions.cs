using ScheduleBot.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Bot;

public class Actions
{
    public static bool TryParseMode(string modeStr, out Mode? mode)
    {
        mode = Enum.GetValues<Mode>().FirstOrDefault(m => $"/{m.ToString().ToLower()}" == modeStr.ToLower());
        return mode is not null;
    }

    public static string DoAction(Mode mode)
        => mode switch
        {
            Mode.Start => "Hello",
            Mode.Stop => "Bye",
            _ => "hz what you want"
        };
    
}
