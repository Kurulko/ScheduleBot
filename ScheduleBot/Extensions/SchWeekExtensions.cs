using ScheduleBot.Database.Models;
using ScheduleBot.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Extensions;

public static class SchWeekExtensions
{
    public static SchWeek GetSchWeekNow()
        => DateTimeExtensions.GetSchWeekNowByStartSemester(ScheduleSettings.DefaultStartedSemester);
    public static SchWeek GetSchWeekByDate(DateTime dateTime)
        => DateTimeExtensions.GetSchWeekDateByStartSemester(ScheduleSettings.DefaultStartedSemester, dateTime);
}
