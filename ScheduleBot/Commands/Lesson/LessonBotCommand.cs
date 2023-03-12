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
using ScheduleBot.Extensions;
using ScheduleBot.Commands.Interfaces;

namespace ScheduleBot.Commands.Lesson;

public abstract record LessonBotCommand : OnceBotCommand
{
    public LessonBotCommand(Command command) : base(command) { }

    protected internal abstract string ResponseLessonStr();

    protected string GetSomeLessonByNumberStr(long numberOfLection)
    {
        TimeLesson2? searchLesson = GetSomeLessonByNumber(numberOfLection);
        return GetLessonStrByTimeLesson(searchLesson);
    }
    protected TimeLesson2 GetSomeLessonByNumber(long numberOfLection)
    {
        TimeLesson2? lastLesson = GetLastLesson();
        if (lastLesson is null)
            throw BotScheduleException.LessonNotFound();

        try
        {
            TimeLesson2 searchLesson = (TimeLesson2)timeLessonService.GetTimeLessonById(lastLesson!.Id + numberOfLection)!;
            return searchLesson;
        }
        catch
        {
            throw BotScheduleException.LessonNotFound();
        }

    }

    protected internal string GetLessonStrByTimeLesson(TimeLesson2 lesson)
    {
        var (st, s, t) = GetSubjectsTeachersByTimeLesson(lesson);
        return LessonStr(st, s, t, lesson);
    }

    protected TimeLesson2? GetLastLesson()
    {
        DateTime last = DateTime.Now;
        DateTime first = timeLessonService.GetTimeLessons().Min(tl => tl.FirstPartStartTime);
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
        TimeLesson? timeLesson = timeLessonService.GetTimeLessonsIncludeConference().FirstOrDefault(t =>
        TimeOnly.FromDateTime(time) >= TimeOnly.FromDateTime(t.FirstPartStartTime) && TimeOnly.FromDateTime(time) <= TimeOnly.FromDateTime(t.SecondPartEndTime) && time.DayOfWeek == t.DayOfWeek && (t.SchWeek == SchWeek.Always || t.SchWeek == SchWeekExtensions.GetSchWeekNow()));
        if (timeLesson is null)
            return default;
        return (TimeLesson2)timeLesson;
    }


    (Conference?, Subject?, Teacher?) GetSubjectsTeachersByTimeLesson(TimeLesson2 time)
    {
        Conference? conference = conferenceService.GetConferencesByIdIncludeTeacherAndSubject(time.ConferenceId);
        if (conference is not null)
        {
            Subject? subject = subjectService.GetSubjectById(conference.SubjectId!.Value);
            Teacher? teacher = teacherService.GetTeacherById(conference.TeacherId!.Value);

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

            response += $"{timeLesson.FirstPartStartTime} - {timeLesson.FirstPartEndTime}; {timeLesson.SecondPartStartTime} - {timeLesson.SecondPartEndTime}";
        }

        return response;
    }
}
