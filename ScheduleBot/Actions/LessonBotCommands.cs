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

namespace ScheduleBot.Actions;

public record LessonBotCommands : BotCommands
{
    const string currentLesson = "/current_lesson", nextLesson = "/next_lesson", previousLesson = "/previous_lesson", allLessons = "/all_lessons", somePreviousLessonRegex = @"\/previous_lesson_(\d)", somePreviousLesson = "/previous_lesson_{number}", someNextLessonRegex = @"\/next_lesson_(\d)", someNextLesson = "/next_lesson_{number}";

    public LessonBotCommands() : base(new Command(currentLesson, "Current lesson", IsPopular: true, IsPeriodicallyAction: true), new Command(nextLesson, "Next lesson"), new Command(previousLesson, "Previous lesson"), new Command(allLessons, "All lessons"), new Command(somePreviousLesson, "...", somePreviousLessonRegex), new Command(someNextLesson, "...", someNextLessonRegex)) { }

    ScheduleContext db = new();

    protected override string ResponseStr()
    {
        string response = currentCommand.Name switch
        {
            currentLesson => CurrentLessonStr(),
            nextLesson => NextLessonStr(),
            previousLesson => PreviousLessonStr(),
            allLessons => AllLessonsStr(),
            somePreviousLesson => SomePreviousLesson(),
            someNextLesson => SomeNextLesson(),
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(response))
            throw BotScheduleException.LessonNotFound();

        return response;
    }

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

    bool IsNowLesson(out TimeLesson2? timeLesson)
    {
        timeLesson = GetTimeLessonByDateTime(DateTime.Now);
        return timeLesson is not null;
    }

    string CurrentLessonStr()
    {
        bool isNowLesson = IsNowLesson(out TimeLesson2? currentTimeLesson);
        if(!isNowLesson)
            throw BotScheduleException.LessonNotFound();
        return GetLessonStrByTimeLesson(currentTimeLesson!);
    }

    string NextLessonStr()
        => GetSomeLessonByNumberStr(+1);

    string PreviousLessonStr()
    {
        bool isNowLesson = IsNowLesson(out TimeLesson2? _);
        return GetSomeLessonByNumberStr(isNowLesson ? -1 : 0);
    }

    string AllLessonsStr()
    {
        string result = string.Empty;

        var timeLessons = db.TimeLessons.OrderBy(tl => tl.DayOfWeek).ToList();
        int countOfTimeLessons = timeLessons.Count;

        if(countOfTimeLessons <= 0)
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

    string GetSomeLessonByNumberStr(long numberOfLection)
    {
        TimeLesson2? lastLesson = GetLastLesson();
        if (lastLesson is null)
            throw BotScheduleException.LessonNotFound();

        TimeLesson2? searchLesson = (TimeLesson2?)db.TimeLessons.FirstOrDefault(tl => tl.Id == lastLesson!.Id + numberOfLection)!;
        if (searchLesson is null)
            throw BotScheduleException.LessonNotFound();


        return GetLessonStrByTimeLesson(searchLesson);
    }

    string GetLessonStrByTimeLesson(TimeLesson2 lesson)
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

    TimeLesson2? GetTimeLessonByDateTime(DateTime time)
    {
        TimeLesson? timeLesson = db.TimeLessons.Include(tl => tl.Conference).ToList().FirstOrDefault(t =>
        TimeOnly.FromDateTime(time) >= TimeOnly.FromDateTime(t.StartTime) && TimeOnly.FromDateTime(time) <= TimeOnly.FromDateTime(t.EndTime) && time.DayOfWeek == t.DayOfWeek);
        if(timeLesson is null)
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
