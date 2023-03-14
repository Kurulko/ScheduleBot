using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Database.Models;

public class TimeLesson : DbModelWithToken
{
    public DateTime FirstPartStartTime { get; set; }
    public DateTime FirstPartEndTime { get; set; }
    public DateTime SecondPartStartTime { get; set; }
    public DateTime SecondPartEndTime { get; set; }
    public SchWeek SchWeek { get; set; }
    public DayOfWeek DayOfWeek { get; set; }

    public long ConferenceId { get; set; }
    public Conference? Conference { get; set; }

    public static explicit operator TimeLesson2(TimeLesson timeLesson1)
    {
        TimeLesson2 timeLesson2 = new();

        timeLesson2.Id = timeLesson1.Id;
        timeLesson2.DayOfWeek = timeLesson1.DayOfWeek;
        timeLesson2.SchWeek = timeLesson1.SchWeek;
        timeLesson2.FirstPartStartTime = TimeOnly.FromDateTime(timeLesson1.FirstPartStartTime);
        timeLesson2.FirstPartEndTime = TimeOnly.FromDateTime(timeLesson1.FirstPartEndTime);
        timeLesson2.SecondPartStartTime = TimeOnly.FromDateTime(timeLesson1.SecondPartStartTime);
        timeLesson2.SecondPartEndTime = TimeOnly.FromDateTime(timeLesson1.SecondPartEndTime);
        timeLesson2.ConferenceId = timeLesson1.ConferenceId;
        timeLesson2.Conference = timeLesson1.Conference;
        timeLesson2.TokenId = timeLesson1.TokenId;
        timeLesson2.Token = timeLesson1.Token;

        return timeLesson2;
    }
}

public class TimeLesson2 : DbModelWithToken
{
    public TimeOnly FirstPartStartTime { get; set; }
    public TimeOnly FirstPartEndTime { get; set; }
    public TimeOnly SecondPartStartTime { get; set; }
    public TimeOnly SecondPartEndTime { get; set; }
    public SchWeek SchWeek { get; set; }
    public DayOfWeek DayOfWeek { get; set; }

    public long ConferenceId { get; set; }
    public Conference? Conference { get; set; }
}
public enum SchWeek
{
    Always,
    Numerator,
    Denominator,
}

