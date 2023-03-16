using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Settings;

public static class ScheduleSettings
{
    public static TimeSpan DefaultLongLessonHours => new(1, 30, 0);
    public static TimeSpan PeriodOfUpdatingData => new(0, 0, 5);
    public static DateTime DefaultStartedSemester => new(2023, 1, 23);
}
