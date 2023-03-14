using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Extensions;

public static class DateTimeExtensions
{
    public static SchWeek GetSchWeekNowByStartSemester(this DateTime startSemester)
        => ((DateTime.Now - startSemester).TotalDays / 7) % 2 == 0 ? SchWeek.Numerator : SchWeek.Denominator;
    public static SchWeek GetSchWeekDateByStartSemester(this DateTime startSemester, DateTime date)
        => ((date - startSemester).TotalDays / 7) % 2 == 0 ? SchWeek.Numerator : SchWeek.Denominator;
}
