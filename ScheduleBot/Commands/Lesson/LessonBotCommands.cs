using ScheduleBot.Database.Models;
using ScheduleBot.Database;
using ScheduleBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleBot.Bot;
using ScheduleBot.Settings;

namespace ScheduleBot.Commands.Lesson;

public abstract record LessonBotCommands : BotCommands
{
    public LessonBotCommands(Command command) : base(command) { }

    protected ScheduleContext db = new();

    protected abstract string ResponseLessonStr();

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

    protected TimeLesson2? GetLastLesson()
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

    protected override string ResponseStr()
    {
        string response = ResponseLessonStr();
        if (string.IsNullOrEmpty(response))
            throw BotScheduleException.LessonNotFound();
        return response;
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
