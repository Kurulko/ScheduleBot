using ScheduleBot.Database.Models;
using ScheduleBot.Database;
using ScheduleBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleBot.Bot;

namespace ScheduleBot.Commands.HWs;

public abstract record HWBotCommands : BotCommands
{
    public HWBotCommands(Command command) : base(command) { }
    protected ScheduleContext db = new();

    protected abstract string ResponseHWsStr();

    protected string GetHWsStr(Func<HW, bool> hwsExpression)
    {
        var values = GetHWs(hwsExpression);

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

    protected override string ResponseStr()
    {
        string response = ResponseHWsStr();
        if (string.IsNullOrEmpty(response))
            throw BotScheduleException.HWsNotFound();
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
