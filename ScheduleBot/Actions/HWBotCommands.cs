using ScheduleBot.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using ScheduleBot.Bot;
using ScheduleBot.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using ScheduleBot.Exceptions;

namespace ScheduleBot.Actions;

public record HWBotCommands : BotCommands
{
    const string activeHWs = "/active_hws", inactiveHWs = "/inactive_hws", todayHWs = "/today_hws", yesterdayHWs = "/yesterday_hws", tomorrowHWs = "/tomorrow_hws", allHWs = "/all_hws", dayAfterHWs = "/hws_after_today_{number}", dayBeforeHWs = "/hws_before_today_{number}", dayAfterHWsRegex = @"\/hws_after_today_(\d)", dayBeforeHWsRegex = @"\/hws_before_today_(\d)";


    public HWBotCommands() : base(new Command(activeHWs, "Active homeworks", IsPopular: true), new Command(inactiveHWs, "Inactive homeworks"), new Command(todayHWs, "Deadline is today, homeworks"), new Command(yesterdayHWs, "Deadline was yesterday, homeworks"), new Command(tomorrowHWs, "Deadline'll be tomorrow, homeworks"), new Command(allHWs, "All homeworks"), new Command(dayAfterHWs, "...", dayAfterHWsRegex), new Command(dayBeforeHWs, "...", dayBeforeHWsRegex)) { }

    ScheduleContext db = new();

    protected override string ResponseStr()
    {
        string response = currentCommand.Name switch
        {
            activeHWs => ActiveHWsStr(),
            inactiveHWs => InactiveHWsStr(),
            todayHWs => TodayHWsStr(),
            yesterdayHWs => YesterdayHWsStr(),
            tomorrowHWs => TomorrowHWsStr(),
            allHWs => AllHWsStr(),
            dayAfterHWs => DayAfterHWsStr(),
            dayBeforeHWs => DayBeforeHWsStr(),
            _ => string.Empty
        };

        if(string.IsNullOrEmpty(response))
            throw BotScheduleException.HWsNotFound();

        return response;
    }

    string DayAfterHWsStr()
        => SomeHWs(true);
    string DayBeforeHWsStr()
        => SomeHWs(false);
    string SomeHWs(bool isNext)
    {
        Regex regex = new(currentCommand.RegEx!);
        if (!regex.IsMatch(CurrentCommandStr))
            throw BotScheduleException.IncorrectExpression(); ;

        string numberOfLessonStr = regex.Match(CurrentCommandStr).Groups[1].Value;
        bool result = int.TryParse(numberOfLessonStr, out int numberOfLesson);
        if (!result)
            throw BotScheduleException.IncorrectExpression(); ;
        return GetLessonStr(GetHWs(hw => hw.Deadline.Day == DateTime.Now.AddDays(isNext ? numberOfLesson : -1 * numberOfLesson).Day));
    }


    string ActiveHWsStr()
        => GetLessonStr(GetHWs(hw => hw.Deadline >= DateTime.Now ));
    string InactiveHWsStr()
        => GetLessonStr(GetHWs(hw => hw.Deadline < DateTime.Now));
    string TodayHWsStr()
        => GetLessonStr(GetHWs(hw => hw.Deadline.Day == DateTime.Now.Day));
    string YesterdayHWsStr()
        => GetLessonStr(GetHWs(hw => hw.Deadline.Day == DateTime.Now.Day - 1));
    string TomorrowHWsStr()
        => GetLessonStr(GetHWs(hw => hw.Deadline.Day == DateTime.Now.Day + 1));
    string AllHWsStr() 
        => GetLessonStr(GetHWs(hw => true));


    IEnumerable<(HW, Subject, Teacher)> GetHWs(Func<HW, bool> hwsExpression)
    {
        List<(HW, Subject, Teacher)> result = new();

        var hws = db.HWs.Where(hwsExpression).ToList();

        if (hws is not null)
        {
            foreach (HW hw in hws)
            {
                Subject subject = db.Subjects.First(s => s.Id == hw.SubjectId);
                Teacher teacher = db.Teachers.First(t => t.Id == hw.TeacherId);
                result.Add((hw, subject, teacher));
            }
        }

        return result;
    }

    string GetLessonStr(IEnumerable<(HW, Subject, Teacher)> values)
    {
        string response = string.Empty;

        var valuesList = values.ToList();
        int count = valuesList.Count;
        for (int i = 0; i < count; i++)
        {
            var (hw, subject, teacher) = valuesList[i];

            response += SubjectStr(subject) + TeacherStr(teacher) + HWStr(hw);

            if (i != count - 1)
                response += $"\n{new string('-', 10)}\n";
        }


        return response;
    }
    

    string SubjectStr(Subject? subject)
    {
        string response = string.Empty;

        if (subject is not null && subject.Name is { } subjectName)
            response += $"<b>Subject</b>: {subjectName}\n";

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
    string HWStr(HW? hw)
    {
        string response = string.Empty;

        if (hw is not null)
        {
            if (hw.Description is { } hwDescription)
                response += $"<b>Description</b>: {hwDescription}\n";

            if (hw.Deadline is { } hwDeadline)
                response += $"<b>Deadline</b>: {string.Format("{0: dd.MM.yyyy hh:mm}", hwDeadline)}";
        }

        return response;
    }
}
