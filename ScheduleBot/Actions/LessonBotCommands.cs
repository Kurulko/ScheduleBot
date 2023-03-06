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

namespace ScheduleBot.Actions;

public record LessonBotCommands : BotCommands
{
    const string currentLesson = "/current_lesson", nextLesson = "/next_lesson", previousLesson = "/previous_lesson", allLessons = "/all_lessons", somePreviousLessonRegex = @"\/previous_lesson_(\d)", somePreviousLesson = "/previous_lesson_{number}", someNextLessonRegex = @"\/next_lesson_(\d)", someNextLesson = "/next_lesson_{number}";


    public LessonBotCommands() : base(new Command(currentLesson, "Current lesson", IsPopular: true), new Command(nextLesson, "Next lesson"), new Command(previousLesson, "Previous lesson"), new Command(allLessons, "All lessons"), new Command(somePreviousLesson, "...", somePreviousLessonRegex), new Command(someNextLesson, "...", someNextLessonRegex)) { }

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

        return $"<b>{response}</b>";
    }

    string SomePreviousLesson()
        => SomeLesson(false);
    string SomeNextLesson()
        => SomeLesson(true);
    string SomeLesson(bool isNext)
    {
        Regex regex = new(currentCommand.RegEx!);
        if (!regex.IsMatch(currentCommandStr))
            throw BotScheduleException.IncorrectExpression(); ;

        string numberOfLessonStr = regex.Match(currentCommandStr).Groups[1].Value;
        bool result = int.TryParse(numberOfLessonStr, out int numberOfLesson);
        if (!result)
            throw BotScheduleException.IncorrectExpression(); ;
        return GetSomeLessonByNumberStr(isNext ? numberOfLesson : -1 * numberOfLesson);
    }

    string CurrentLessonStr()
        => GetSomeLessonByNumberStr(0);

    string NextLessonStr()
        => GetSomeLessonByNumberStr(+2);

    string PreviousLessonStr()
        => GetSomeLessonByNumberStr(-2);

    string AllLessonsStr()
    {
        string result = string.Empty;

        var timeLessons = db.TimeLessons.ToList();
        int countOfTimeLessons = timeLessons.Count;

        for (int i = 0; i < countOfTimeLessons; i++)
        {
            var (st, s, t) = GetSubjectsTeachersByTimeLesson(timeLessons[i]);
            string response = LessonStr(st, s, t);
            if (!string.IsNullOrEmpty(response))
            {
                result += response;
                if (i != countOfTimeLessons - 1)
                    result += $"\n{new string('-', 10)}\n";
            }
        }

        return result;
    }

    string GetSomeLessonByNumberStr(int i)
    {
        DateTime? lastLesson = GetDateTimeLastLesson();
        if (lastLesson is not null)
            return GetLessonStrByDateTime(lastLesson.Value.AddHours(i));
        throw BotScheduleException.LessonNotFound(); ;
    }

    string GetLessonStrByDateTime(DateTime date)
    {
        TimeLesson? time = GetTimeLessonByDateTime(date);
        if (time is null)
            throw BotScheduleException.LessonNotFound();
        var (st, s, t) = GetSubjectsTeachersByTimeLesson(time);
        return LessonStr(st, s, t);
    }

    DateTime? GetDateTimeLastLesson()
    {
        DateTime now = DateTime.Now;
        DateTime last = DateTime.MinValue.AddHours(now.Hour).AddMinutes(now.Minute).AddSeconds(now.Second);
        DateTime first = db.TimeLessons.Min(tl => tl.StartTime);
        while (last >= first)
        {
            if (GetTimeLessonByDateTime(last) is not null)
                return last;
            last = last.AddHours(-2);
        }
        return default;
    }

    TimeLesson? GetTimeLessonByDateTime(DateTime time)
        => db.TimeLessons.FirstOrDefault(t => time >= t.StartTime && time <= t.StartTime && time.DayOfWeek == t.DayOfWeek);

    (SubjectTeacher?, Subject?, Teacher?) GetSubjectsTeachersByTimeLesson(TimeLesson time)
    {
        SubjectTeacher? subjectTeacher = default/*db.SubjectTeacher.Include(st => st.Conference).Include(st => st.TimeLesson).FirstOrDefault(st => st.TimeLesson == time)*/;
        if (subjectTeacher is not null)
        {
            Subject? subject = db.Subjects.FirstOrDefault(s => s.Id == subjectTeacher.SubjectId);
            Teacher? teacher = db.Teachers.FirstOrDefault(t => t.Id == subjectTeacher.TeacherId);

            return (subjectTeacher, subject, teacher);
        }
        return default;
    }

    string LessonStr(SubjectTeacher? subjectTeacher, Subject? subject, Teacher? teacher)
    {
        string response = string.Empty;

        if (subject is not null)
        {
            if (!string.IsNullOrEmpty(subject.Name))
                response += $"Subject: {subject.Name}\n";

            if (!string.IsNullOrEmpty(subject.Description))
                response += $"Description: {subject.Description}\n";
        }

        if (teacher is not null)
        {
            if (teacher.LastName is { } teacherLastName)
                response += $"Teacher: {teacherLastName}";

            if (teacher.FirstName is { } teacherFirstName)
                response += $" {teacherFirstName.First()}.";

            if (teacher.FatherName is { } teacherFatherName)
                response += $" {teacherFatherName.First()}.";

            response += '\n';
        }

        if (subjectTeacher is not null)
        {
            if (subjectTeacher.Conference is { } conference && conference.Link is { } link)
                response += $"Link: {link}\n";

            if (subjectTeacher.TimeLesson is { } timeLesson)
            {
                response += "Time: ";

                if (timeLesson.StartTime.DayOfWeek is { } startDay)
                    response += $"{startDay} ";

                response += timeLesson.StartTime is { } start ? start : nameof(start);
                response += " - ";

                if (timeLesson.StartTime.DayOfWeek != timeLesson.EndTime.DayOfWeek)
                    response += $"{timeLesson.EndTime.DayOfWeek} ";
                response += timeLesson.EndTime is { } end ? end : nameof(end);
            }
        }


        return !string.IsNullOrEmpty(response) ? $"<b>{response}</b>" : response;
    }
}
