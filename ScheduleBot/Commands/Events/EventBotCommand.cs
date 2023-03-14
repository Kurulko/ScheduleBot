using ScheduleBot.Database.Models;
using ScheduleBot.Database;
using ScheduleBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleBot.Bot;
using ScheduleBot.Commands.Interfaces;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace ScheduleBot.Commands.HWs;

public abstract record EventBotCommand : OnceBotCommand
{
    protected TypeOfEvent[] types;
    public EventBotCommand(Command command, params TypeOfEvent[] types) : base(command)
        => this.types = types;

    protected abstract string ResponseEventsStr();

    IList<string> eventsStr = new List<string>();
    protected string GetEventsStr(Func<Event, bool> eventsExpression)
    {
        var values = GetEvents(eventsExpression).OrderBy(m => m.e.TypeOfEvent);

        string response = string.Empty;

        var valuesList = values.ToList();
        int count = valuesList.Count;
        if (count > 0)
        {
            var currentTypeOfEvent = valuesList[0].e.TypeOfEvent;
            response = DisplayTypeOfEvent(currentTypeOfEvent);

            for (int i = 0; i < count; i++)
            {
                var (_event, subject, teacher) = valuesList[i];

                if (_event.TypeOfEvent != currentTypeOfEvent)
                {
                    eventsStr.Add(response);
                    currentTypeOfEvent = _event.TypeOfEvent;
                    response = DisplayTypeOfEvent(currentTypeOfEvent);
                }

                response += SubjectStr(subject) + TeacherStr(teacher) + EventStr(_event);

                if (i != count - 1)
                {
                    if (valuesList[i + 1].e.TypeOfEvent == _event.TypeOfEvent)
                        response += $"\n{new string('•', 36)}\n";
                }
                else
                    eventsStr.Add(response);
            }
        }

        return string.Empty;
    }
    string DisplayTypeOfEvent(TypeOfEvent typeOfEvent)
        => $"<b>{typeOfEvent}</b>:\n\n";

    IEnumerable<(Event e, Subject s, Teacher t)> GetEvents(Func<Event, bool> eventsExpression)
    {
        List<(Event, Subject, Teacher)> result = new();

        var events = eventService.GetEventsByType(types).Where(eventsExpression);

        if (events is not null)
        {
            foreach (Event _event in events)
            {
                Subject subject = subjectService.GetModelById(_event.SubjectId)!;
                Teacher teacher = teacherService.GetModelById(_event.TeacherId)!;
                result.Add((_event, subject, teacher));
            }
        }

        return result;
    }

    protected override string ResponseStr()
    {
        string response = ResponseEventsStr();
        if (string.IsNullOrEmpty(response))
            throw BotScheduleException.EventsNotFound(types);
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
    string EventStr(Event? hw)
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

    public override async Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, int? replyToMessageId = null)
    {
        Message? message = null;
        ResponseEventsStr();

        if (eventsStr.Count == 0)
            throw BotScheduleException.EventsNotFound(types);

        for (int i = 0; i < eventsStr.Count; i++)
            message = await SendTextMessagesIfResponseMoreMaxLengthAsync(bot, chatId, cts, eventsStr[i], i == 0 ? replyToMessageId : message!.MessageId);

        return message!;
    }
}
