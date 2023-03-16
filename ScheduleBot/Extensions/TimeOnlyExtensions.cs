using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Extensions;

public static class TimeOnlyExtensions
{
    public static bool CompareWithNowByHoursAndMinutes(this TimeOnly t)
    {
        TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);
        return now.Minute == t.Minute && now.Hour == t.Hour;
    }
    public static bool CompareTimeByHoursAndMinutes(TimeOnly t1, TimeOnly t2)
        => t1.Minute == t2.Minute && t1.Hour == t2.Hour;
}
