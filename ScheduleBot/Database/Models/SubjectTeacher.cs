using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Database.Models;

public class SubjectTeacher
{
    public long Id { get; set; }
    public SchWeekEnum SchWeekEnum { get; set; } = SchWeekEnum.Always;

    public long SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public long TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
    public long TimeLessonId { get; set; }
    public TimeLesson? TimeLesson { get; set; }
    public long ConferenceId { get; set; }
    public Conference? Conference { get; set; }
}
public enum SchWeekEnum
{
    Denominator,
    Numerator,
    Always
}

public static class SchWeekEnumExtensions
{
    public static SchWeekEnum GetSchWeekEnumNow(this SchWeekEnum weekEnum)
    {
        return DateTime.Now.DayOfYear % 7 == 0 ? SchWeekEnum.Numerator : SchWeekEnum.Denominator;
    }
}