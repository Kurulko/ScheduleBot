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
            DateTime now = DateTime.Now;
            bool isFound = false;
            long counfFromFound = 0;
            bool isNumberMoreThanZero = numberOfLection >= 0;
            while (true)
            {
                SchWeek currentShweek = SchWeekExtensions.GetSchWeekByDate(now);
                var lessonsWithoutSort = timeLessonService.GetModels().Where(m => m.SchWeek == SchWeek.Always || m.SchWeek == currentShweek);
                var lessons = (isNumberMoreThanZero ? lessonsWithoutSort.OrderBy(l => l.DayOfWeek).ThenBy(l => l.FirstPartStartTime) : lessonsWithoutSort.OrderByDescending(l => l.DayOfWeek).ThenByDescending(l => l.FirstPartStartTime)).ToList();

                for (int i = 0; i < lessons.Count; i++)
                {
                    if(isFound)
                    {
                        if (counfFromFound != numberOfLection)
                            counfFromFound = isNumberMoreThanZero ? counfFromFound + 1 : counfFromFound - 1;
                        else
                            return (TimeLesson2)lessons[i - 1];

                        if (counfFromFound == numberOfLection)
                            return (TimeLesson2)lessons[i];
                    }
                    else if(lastLesson.Id == lessons[i].Id)
                    {
                        isFound = true;
                    }
                }

                int oneWeek = 7;
                now.AddDays(isNumberMoreThanZero ? oneWeek : -1 * oneWeek);
            }

            //TimeLesson2 searchLesson = (TimeLesson2)timeLessonService.GetModelById(lastLesson!.Id + numberOfLection)!;
            //TimeLesson2 searchLesson = (TimeLesson2)timeLessonService.GetModels().Where(m => m.SchWeek == SchWeek.Always || m.SchWeek == SchWeekExtensions.GetSchWeekNow()).First(lt => lt.Id == lastLesson!.Id + numberOfLection)!;
            //return searchLesson!;
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
        DateTime first = timeLessonService.GetModels().Min(tl => tl.FirstPartStartTime);
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
            Subject? subject = conference.SubjectId is not null ? subjectService.GetModelById(conference.SubjectId!.Value) : null;
            Teacher? teacher = conference.TeacherId is not null ? teacherService.GetModelById(conference.TeacherId!.Value) : null;

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
