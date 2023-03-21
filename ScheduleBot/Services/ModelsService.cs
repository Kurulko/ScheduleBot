using ScheduleBot.Database.Models;
using ScheduleBot.Extensions;
using ScheduleBot.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Services;

public static class ModelsService
{
    public static void DeleteAllModelsByTokenId(long tokenId)
    {
        TokenService tokenService = new();
        Token? token = tokenService.GetTokenByIdIncludeAllModels(tokenId);

        if (token is not null)
        {
            DeleteAllConferencesByToken(token);
            DeleteAllSubjectsByToken(token);
            DeleteAllBreaksByToken(token);
            DeleteAllTeachersByToken(token);
            DeleteAllTimeLessonsByToken(token);
            DeleteAllEventsByToken(token);
        }
    }

    static void DeleteAllConferencesByToken(Token token)
    {
        if (token.Conferences.IsNotNullOrEmpty())
        {
            ConferenceService conferenceService = new();
            conferenceService.RemoveModels(token.Conferences!);
        }
    }
    static void DeleteAllSubjectsByToken(Token token)
    {
        if (token.Subjects.IsNotNullOrEmpty())
        {
            SubjectService subjectService = new();
            subjectService.RemoveModels(token.Subjects!);
        }
    }
    static void DeleteAllBreaksByToken(Token token)
    {
        if (token.Breaks.IsNotNullOrEmpty())
        {
            BreakService breakService = new();
            breakService.RemoveModels(token.Breaks!);
        }
    }
    static void DeleteAllTeachersByToken(Token token)
    {
        if (token.Teachers.IsNotNullOrEmpty())
        {
            TeacherService teacherService = new();
            teacherService.RemoveModels(token.Teachers!);
        }
    }
    static void DeleteAllTimeLessonsByToken(Token token)
    {
        if (token.TimeLessons.IsNotNullOrEmpty())
        {
            TimeLessonService timeLessonService = new();
            timeLessonService.RemoveModels(token.TimeLessons!);
        }
    }
    static void DeleteAllEventsByToken(Token token)
    {
        if (token.Events.IsNotNullOrEmpty())
        {
            EventService eventService = new();
            eventService.RemoveModels(token.Events!);
        }
    }
}
