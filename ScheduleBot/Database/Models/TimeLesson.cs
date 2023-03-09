using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Database.Models;

public class TimeLesson
{
    public long Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public SchWeekEnum SchWeekEnum { get; set; }

    public long ConferenceId { get; set; }
    public Conference? Conference { get; set; }

    public static explicit operator TimeLesson2(TimeLesson timeLesson1)
    {
        TimeLesson2 timeLesson2 = new();

        timeLesson2.Id = timeLesson1.Id;
        timeLesson2.DayOfWeek = timeLesson1.DayOfWeek;
        timeLesson2.SchWeekEnum = timeLesson1.SchWeekEnum;
        timeLesson2.StartTime = TimeOnly.FromDateTime(timeLesson1.StartTime);
        timeLesson2.EndTime = TimeOnly.FromDateTime(timeLesson1.EndTime);
        timeLesson2.ConferenceId = timeLesson1.ConferenceId;
        timeLesson2.Conference = timeLesson1.Conference;

        return timeLesson2;
    }
}

public record TimeLesson2
{
    public long Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public SchWeekEnum SchWeekEnum { get; set; }

    public long ConferenceId { get; set; }
    public Conference? Conference { get; set; }
}
public enum SchWeekEnum
{
    Always,
    Numerator,
    Denominator,
}

