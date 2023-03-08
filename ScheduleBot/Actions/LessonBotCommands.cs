using ScheduleBot.Database.Models;
using ScheduleBot.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Microsoft.EntityFrameworkCore;
using Azure;
using Microsoft.IdentityModel.Tokens;
using ScheduleBot.Bot;
using System.Text.RegularExpressions;
using ScheduleBot.Exceptions;
using ScheduleBot.Extensions;
using System.Collections;

namespace ScheduleBot.Actions;

public abstract record LessonBotCommands : BotCommands
{
    public LessonBotCommands(Command command) : base(command) { }

    protected ScheduleContext db = new();

    protected abstract string ResponseLessonStr();

    protected override string ResponseStr()
    {
        string response = ResponseLessonStr();
        if (string.IsNullOrEmpty(response))
            throw BotScheduleException.LessonNotFound();
        return response;
    }

    protected string GetSomeLessonByNumberStr(long numberOfLection)
    {
        TimeLesson2? lastLesson = GetLastLesson();
        if (lastLesson is null)
            throw BotScheduleException.LessonNotFound();

        TimeLesson2? searchLesson = (TimeLesson2?)db.TimeLessons.FirstOrDefault(tl => tl.Id == lastLesson!.Id + numberOfLection)!;
        if (searchLesson is null)
            throw BotScheduleException.LessonNotFound();


        return GetLessonStrByTimeLesson(searchLesson);
    }
    protected string GetLessonStrByTimeLesson(TimeLesson2 lesson)
    {
        var (st, s, t) = GetSubjectsTeachersByTimeLesson(lesson);
        return LessonStr(st, s, t, lesson);
    }

    TimeLesson2? GetLastLesson()
    {
        DateTime last = DateTime.Now;
        DateTime first = db.TimeLessons.Min(tl => tl.StartTime);
        while (last >= first)
        {
            TimeLesson2? timeLesson = GetTimeLessonByDateTime(last);
            if (timeLesson is not null)
                return timeLesson;
            last = last.AddHours(-ScheduleSettings.LongLessonHours.TotalHours);
        }
        return default;
    }

    protected TimeLesson2? GetTimeLessonByDateTime(DateTime time)
    {
        TimeLesson? timeLesson = db.TimeLessons.Include(tl => tl.Conference).ToList().FirstOrDefault(t =>
        TimeOnly.FromDateTime(time) >= TimeOnly.FromDateTime(t.StartTime) && TimeOnly.FromDateTime(time) <= TimeOnly.FromDateTime(t.EndTime) && time.DayOfWeek == t.DayOfWeek);
        if (timeLesson is null)
            return default;
        return (TimeLesson2)timeLesson;
    }


    (Conference?, Subject?, Teacher?) GetSubjectsTeachersByTimeLesson(TimeLesson2 time)
    {
        Conference? conference = db.Conferences.Include(c => c.Subject).Include(c => c.Teacher).FirstOrDefault(c => c.Id == time.ConferenceId);
        if (conference is not null)
        {
            Subject? subject = db.Subjects.FirstOrDefault(s => s.Id == conference.SubjectId);
            Teacher? teacher = db.Teachers.FirstOrDefault(t => t.Id == conference.TeacherId);

            return (conference, subject, teacher);
        }
        return default;
    }

    string LessonStr(Conference? conference, Subject? subject, Teacher? teacher, TimeLesson2? timeLesson)
        => SubjectStr(subject) + TeacherStr(teacher) + ConferenceStr(conference) + TimeLessonStr(timeLesson);

    string SubjectStr(Subject? subject)
    {
        string response = string.Empty;

        if (subject is not null)
        {
            if (!string.IsNullOrEmpty(subject.Name))
                response += $"<b>Subject</b>: {subject.Name}\n";

            if (!string.IsNullOrEmpty(subject.Description))
                response += $"<b>Description</b>: {subject.Description}\n";
        }

        return response;
    }
    string TeacherStr(Teacher? teacher)
    {
        string response = string.Empty;

        if (teacher is not null)
        {
            if (teacher.LastName is { } teacherLastName)
                response += $"<b>Teacher</b>: {teacherLastName}";

            if (teacher.FirstName is { } teacherFirstName)
                response += $" {teacherFirstName.First()}.";

            if (teacher.FatherName is { } teacherFatherName)
                response += $" {teacherFatherName.First()}.";

            response += '\n';
        }

        return response;
    }
    string ConferenceStr(Conference? conference)
    {
        string response = string.Empty;

        if (conference is not null && conference.Link is { } link)
            response += $"<b>Link</b>: {link}\n";

        return response;
    }
    string TimeLessonStr(TimeLesson2? timeLesson)
    {
        string response = string.Empty;

        if (timeLesson is not null)
        {
            response += $"<b>Time</b>: {timeLesson.DayOfWeek} ";

            response += timeLesson.StartTime is { } start ? start : nameof(start);
            response += " - ";
            response += timeLesson.EndTime is { } end ? end : nameof(end);
        }

        return response;
    }
}

public record CurrentLesson_LessonBotCommands : LessonBotCommands
{
    public CurrentLesson_LessonBotCommands() : base(new Command("/current_lesson", "Current lesson", IsPopular: true, IsPeriodicallyAction: true)) { }

    protected override string ResponseLessonStr()
    {
        bool isNowLesson = IsNowLesson(out TimeLesson2? currentTimeLesson);
        if (!isNowLesson)
            throw BotScheduleException.LessonNotFound();
        return GetLessonStrByTimeLesson(currentTimeLesson!);
    }
    bool IsNowLesson(out TimeLesson2? timeLesson)
    {
        timeLesson = GetTimeLessonByDateTime(DateTime.Now);
        return timeLesson is not null;
    }
}
public record NextLesson_LessonBotCommands : LessonBotCommands
{
    public NextLesson_LessonBotCommands() : base(new Command("/next_lesson", "Next lesson")) { }

    protected override string ResponseLessonStr()
        => GetSomeLessonByNumberStr(+1);
}
public record PreviousLesson_LessonBotCommands : LessonBotCommands
{
    public PreviousLesson_LessonBotCommands() : base(new Command("/previous_lesson", "Previous lesson")) { }

    protected override string ResponseLessonStr()
    {
        bool isNowLesson = IsNowLesson(out TimeLesson2? _);
        return GetSomeLessonByNumberStr(isNowLesson ? -1 : 0);
    }
}
public record AllLessons_LessonBotCommands : LessonBotCommands
{
    public AllLessons_LessonBotCommands() : base(new Command("/all_lessons", "All lessons")) { }

    protected override string ResponseLessonStr()
    {
        string result = string.Empty;

        var timeLessons = db.TimeLessons.OrderBy(tl => tl.DayOfWeek).ToList();
        int countOfTimeLessons = timeLessons.Count;

        if (countOfTimeLessons <= 0)
            throw BotScheduleException.LessonNotFound();

        DayOfWeek currentDayOfWeek = timeLessons[0].DayOfWeek;
        result += $"\n<b>{currentDayOfWeek}</b>\n\n";
        for (int i = 0; i < countOfTimeLessons; i++)
        {
            TimeLesson2 timeLesson = (TimeLesson2)timeLessons[i];

            if (timeLesson.DayOfWeek != currentDayOfWeek)
            {
                currentDayOfWeek = timeLesson.DayOfWeek;
                result += $"\n<b>{currentDayOfWeek}</b>\n\n";
            }

            string response = GetLessonStrByTimeLesson(timeLesson);
            if (!string.IsNullOrEmpty(response))
            {
                result += response;

                if (i != countOfTimeLessons - 1)
                {
                    if (timeLessons[i + 1].DayOfWeek != currentDayOfWeek)
                        result += "\n";
                    else
                        result += $"\n{new string('•', 36)}\n";
                }//ᐧ
            }
        }

        return result;

    }
}
public record SomePreviousLesson_LessonBotCommands : LessonBotCommands
{
    public PreviousLesson_LessonBotCommands() : base(new Command("/previous_lesson_{number}", "...", @"\/previous_lesson_(\d)")) { }

    protected override string ResponseLessonStr()
        => GetSomeLessonByNumberStr(-1);
}

public record CurrentLesson_LessonBotCommands2 : LessonBotCommands
{
   someNextLessonRegex = @"\/next_lesson_(\d)", someNextLesson = "/next_lesson_{number}";

    public LessonBotCommands() : base(new Command(someNextLesson, "...", someNextLessonRegex)) { }



    string SomePreviousLesson()
        => SomeLesson(false);
    string SomeNextLesson()
        => SomeLesson(true);


    string SomeLesson(bool isNext)
    {
        Regex regex = new(currentCommand.RegEx!);
        if (!regex.IsMatch(CurrentCommandStr))
            throw BotScheduleException.IncorrectExpression(); ;

        string numberOfLessonStr = regex.Match(CurrentCommandStr).Groups[1].Value;
        bool result = int.TryParse(numberOfLessonStr, out int numberOfLesson);
        if (!result)
            throw BotScheduleException.IncorrectExpression();
        numberOfLesson++;
        return GetSomeLessonByNumberStr(isNext ? numberOfLesson : -1 * numberOfLesson);
    }
}
