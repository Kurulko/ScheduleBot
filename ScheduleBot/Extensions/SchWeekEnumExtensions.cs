using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Extensions;

public static class SchWeekEnumExtensions
{
    public static SchWeekEnum GetSchWeekEnumNow()
    {
        return (DateTime.Now.DayOfYear / 7) % 2 == 0 ? SchWeekEnum.Numerator : SchWeekEnum.Denominator;
    }
}
