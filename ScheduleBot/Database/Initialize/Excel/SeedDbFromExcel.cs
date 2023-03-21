using Newtonsoft.Json;
using ScheduleBot.Database.Models;
using ScheduleBot.Exceptions;
using ScheduleBot.Services;
using ScheduleBot.Services.ByToken;
using ScheduleBot.Services.Common;
using ScheduleBot.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Database.Initialize.Excel;

public class SeedDbFromExcel
{
    static Dictionary<string, ExcelData> ExcelLessonsByTokens = new();
    static Dictionary<string, ExcelData> ExcelEventsByTokens = new();
    static void IfThrownException(Token token)
    {
        ModelsService.DeleteAllModelsByTokenId(token.Id);
        TokenService tokenService = new();
        tokenService.RemoveModel(token);
    }

    public static async Task SeedDbAsync(Token token, bool isDeletePreviousData = false)
    {
        try
        {
            string tokenName = token.Name;

            HttpClient httpClient = new HttpClient();
            string url = $"https://sheets.googleapis.com/v4/spreadsheets/{tokenName}/values/" + "{0}!A2:" + $"H{ExcelSettings.CountOfRows}?key={ExcelSettings.APIKey}";

            ExcelData? excelLessons = await httpClient.GetFromJsonAsync<ExcelData>(string.Format(url, "Lessons"));
            ExcelData? excelEvents = await httpClient.GetFromJsonAsync<ExcelData>(string.Format(url, "Events"));
            if(excelLessons is null || excelLessons is null)
                throw BotScheduleException.IncorrectToken();

            bool isNotContainsLessons = !ExcelLessonsByTokens.ContainsKey(tokenName), isNotContainsEvents = !ExcelEventsByTokens.ContainsKey(tokenName);
            if (isNotContainsLessons)
                ExcelLessonsByTokens.Add(tokenName, excelLessons);
            if (isNotContainsEvents)
                ExcelEventsByTokens.Add(tokenName, excelEvents!);

            bool isNewLessons =  ExcelLessonsByTokens[tokenName] != excelLessons, isNewEvents = ExcelEventsByTokens[tokenName] != excelEvents;
            await Console.Out.WriteLineAsync($"{isNewLessons || isNotContainsLessons}");
            await Console.Out.WriteLineAsync($"{isNewEvents || isNotContainsEvents}");

            if (isDeletePreviousData && isNewLessons && isNewEvents)
                ModelsService.DeleteAllModelsByTokenId(token.Id);

            if (isNewLessons || isNotContainsLessons)
            {
                ParseLessonsData(excelLessons!, token.Id);
                AddBreaksToDbByToken(token.Id);
                ExcelLessonsByTokens[tokenName] = excelLessons;
            }

            if (isNewEvents || isNotContainsEvents)
            {
                ParseEventsData(excelEvents!, token.Id);
                ExcelEventsByTokens[tokenName] = excelLessons;
            }
        }
        catch(BotScheduleException)
        {
            IfThrownException(token);
            throw;
        }
        catch(Exception)
        {
            IfThrownException(token);
            throw BotScheduleException.IncorrectToken();
        }
    }
    static void AddBreaksToDbByToken(long tokenId)
    {
        BreakServiceByToken breakServiceByToken = new(tokenId);
        TimeLessonServiceByToken timeLessonServiceByToken = new(tokenId);
        
        IList<Break> breaks = new List<Break>();

        IList<TimeLesson> timeLessons = timeLessonServiceByToken.GetModels().OrderBy(tl => tl.DayOfWeek).ToList();
        int countOfTimeLessons = timeLessons.Count;
        for (int i = 0; i < countOfTimeLessons - 1; i++)
        {
            Break rest = new();

            TimeLesson currentTimeLesson = timeLessons[i];

            rest.StartTime = currentTimeLesson.SecondPartEndTime;

            if(i != countOfTimeLessons - 2)
            {
                TimeLesson nextTimeLesson = timeLessons[i + 1];
                if (nextTimeLesson.DayOfWeek == currentTimeLesson.DayOfWeek)
                {
                    rest.EndTime = nextTimeLesson.FirstPartStartTime;
                    rest.DayOfWeek = nextTimeLesson.DayOfWeek;
                }
                else
                    continue;
            }

            rest.TokenId = tokenId;

            breaks.Add(rest);
        }

        if (breaks.Any())
            breakServiceByToken.AddModels(breaks);
    }

    static void ParseEventsData(ExcelData excelEvents, long tokenId)
    {
        var excelEventsStr = excelEvents.Values;
        if (excelEventsStr is null)
            throw BotScheduleException.IncorrectValuesFromExcel();

        foreach (var excelEventStr in excelEventsStr)
        {
            Teacher teacher = ParseTeacher(excelEventStr[1], tokenId);
            teacher = AddTeacherToDbIfNotExist(teacher, tokenId);

            Subject subject = ParseSubject(excelEventStr[2], tokenId);
            subject = AddSubjectToDbIfNotExist(subject, tokenId);

            string? descriptionStr = excelEventStr.Count == 5 ? excelEventStr[4] : null;
            Event _event = ParseEvent(excelEventStr[0], excelEventStr[3], descriptionStr, tokenId);
            AddEventToDbIfNotExist(_event, teacher.Id, subject.Id, tokenId);
        }
    }
    static void ParseLessonsData(ExcelData excelLessons, long tokenId)
    {
        var excelLessonsStr = excelLessons.Values;
        if (excelLessonsStr is null)
            throw BotScheduleException.IncorrectValuesFromExcel();

        foreach (var excelLessonStr in excelLessonsStr)
        {
            Teacher teacher = ParseTeacher(excelLessonStr[1], tokenId);
            teacher = AddTeacherToDbIfNotExist(teacher, tokenId);

            Subject subject = ParseSubject(excelLessonStr[3], tokenId);
            subject = AddSubjectToDbIfNotExist(subject, tokenId);

            Conference conference = ParseConference(excelLessonStr[2], tokenId);
            conference = AddConferenceToDbIfNotExist(conference, teacher.Id, subject.Id, tokenId);

            TimeLesson timeLesson = ParseTimeLesson(excelLessonStr[0], excelLessonStr[7], excelLessonStr[4], excelLessonStr[5], excelLessonStr[6], tokenId);
            AddTimeLessonToDbIfNotExist(timeLesson, conference.Id, tokenId);
        }
        //CHECK LESSONS
    }


    static Teacher AddTeacherToDbIfNotExist(Teacher teacher, long tokenId)
    {
        TeacherServiceByToken teacherServiceByToken = new(tokenId);

        Teacher? _teacher = teacherServiceByToken.GetModels().FirstOrDefault(t => t.FirstName == teacher.FirstName && t.LastName == teacher.LastName && t.FatherName == teacher.FatherName);
        if (_teacher is null)
            teacherServiceByToken.AddModel(teacher);

        return _teacher is not null ? _teacher : teacher;
    }
    static Subject AddSubjectToDbIfNotExist(Subject subject, long tokenId)
    {
        SubjectServiceByToken subjectServiceByToken = new(tokenId);

        Subject? _subject = subjectServiceByToken.GetModels().FirstOrDefault(s => s.Name == subject.Name);
        if (_subject is null)
            subjectServiceByToken.AddModel(subject);

        return _subject is not null ? _subject : subject;
    }
    static Conference AddConferenceToDbIfNotExist(Conference conference, long teacherId, long subjectId, long tokenId)
    {
        ConferenceServiceByToken conferenceServiceByToken = new(tokenId);

        Conference? _conference = conferenceServiceByToken.GetModels().FirstOrDefault(c => c.Link == conference.Link && c.TeacherId == teacherId && c.SubjectId == subjectId);
        if (_conference is null)
        {
            conference.TeacherId = teacherId;
            conference.SubjectId = subjectId;
            conferenceServiceByToken.AddModel(conference);
        }
        else
        {
            if (_conference.TeacherId != teacherId)
            {
                _conference.TeacherId = teacherId;
                conferenceServiceByToken.UpdateModel(_conference);
            }
            if (_conference.SubjectId != subjectId)
            {
                _conference.SubjectId = subjectId;
                conferenceServiceByToken.UpdateModel(_conference);
            }
        }
        return _conference is not null ? _conference : conference;
    }
    static Event AddEventToDbIfNotExist(Event _event, long teacherId, long subjectId, long tokenId)
    {
        EventServiceByToken eventServiceByToken = new(tokenId);

        Event? __event = eventServiceByToken.GetModels().FirstOrDefault(h => h.Deadline == _event.Deadline && h.WasGivenDate == _event.WasGivenDate  && h.Description == _event.Description && h.TypeOfEvent == _event.TypeOfEvent);
        if (__event is null)
        {
            _event.TeacherId = teacherId;
            _event.SubjectId = subjectId;
            eventServiceByToken.AddModel(_event);
        }
        else
        {
            if (__event.TeacherId != teacherId)
            {
                __event.TeacherId = teacherId;
                eventServiceByToken.UpdateModel(__event);
            }
            if (__event.SubjectId != subjectId)
            {
                __event.SubjectId = subjectId;
                eventServiceByToken.UpdateModel(__event);
            }
        }
        return __event is not null ? __event : _event;
    }
    static TimeLesson AddTimeLessonToDbIfNotExist(TimeLesson timeLesson, long conferenceId, long tokenId)
    {
        TimeLessonServiceByToken timeLessonServiceByToken = new(tokenId);

        TimeLesson? _timeLesson = timeLessonServiceByToken.GetModels().FirstOrDefault(tl => tl.FirstPartStartTime == timeLesson.FirstPartStartTime && tl.FirstPartEndTime == timeLesson.FirstPartEndTime && tl.SecondPartStartTime == timeLesson.SecondPartStartTime && tl.SecondPartEndTime == timeLesson.SecondPartEndTime && tl.DayOfWeek == timeLesson.DayOfWeek && tl.SchWeek == timeLesson.SchWeek);
        if (_timeLesson is null)
        {
            timeLesson.ConferenceId = conferenceId;
            timeLessonServiceByToken.AddModel(timeLesson);
        }
        else if (_timeLesson.ConferenceId != conferenceId)
        {
            _timeLesson.ConferenceId = conferenceId;
            timeLessonServiceByToken.UpdateModel(_timeLesson);
        }
        return _timeLesson is not null ? _timeLesson : timeLesson;
    }


    static Event ParseEvent(string deadlineStr, string typeOfEventStr, string? descriptionStr, long tokenId)
    {
        Event _event = new();

        SetDeadline();
        SetTypeOfEvent();
        SetDescription();
        _event.TokenId = tokenId;

        return _event;

        void SetDeadline()
        {
            try
            {
                _event.Deadline = DateTime.Parse(deadlineStr); 
            }
            catch
            {
                throw new BotScheduleException("Inccorect deadline time");
            }
        }

        void SetTypeOfEvent()
        {
            foreach (TypeOfEvent typeOfEvent in Enum.GetValues<TypeOfEvent>())
            {
                if (typeOfEvent.ToString().ToLower() == typeOfEventStr.ToLower())
                {
                    _event.TypeOfEvent = typeOfEvent;
                    return;
                }
            }

            throw new BotScheduleException("Inccorect type of Event");
        }

        void SetDescription()
            => _event.Description = descriptionStr;
    }
    static TimeLesson ParseTimeLesson(string timeLessonStr, string dayOfWeekStr, string isNumeratorStr, string isDenominatorStr, string isAlwaysStr, long tokenId)
    {
        TimeLesson timeLesson = new();

        SetPartsTime();
        SetDayOfWeek();
        SetSchWeek();
        timeLesson.TokenId = tokenId;

        return timeLesson;

        void SetPartsTime()
        {
            Regex regex = new(@"(\d{1,2}):(\d{1,2})(-|–)(\d{1,2}):(\d{1,2})[^0-9]*(\d{1,2}):(\d{1,2})(-|–)(\d{1,2}):(\d{1,2})");
            if (regex.IsMatch(timeLessonStr))
            {
                var groups = regex.Match(timeLessonStr).Groups;

                var firstPartStartTime = DateTime.MinValue.AddHours(int.Parse(groups[1].Value)).AddMinutes(int.Parse(groups[2].Value));
                timeLesson.FirstPartStartTime = firstPartStartTime;

                var firstPartEndTime = DateTime.MinValue.AddHours(int.Parse(groups[4].Value)).AddMinutes(int.Parse(groups[5].Value));
                timeLesson.FirstPartEndTime = firstPartEndTime;

                var secondPartStartTime = DateTime.MinValue.AddHours(int.Parse(groups[6].Value)).AddMinutes(int.Parse(groups[7].Value));
                timeLesson.SecondPartStartTime = secondPartStartTime;

                var secondPartEndTime = DateTime.MinValue.AddHours(int.Parse(groups[9].Value)).AddMinutes(int.Parse(groups[10].Value));
                timeLesson.SecondPartEndTime = secondPartEndTime;

                if(firstPartStartTime > firstPartEndTime || firstPartEndTime > secondPartStartTime || secondPartStartTime > secondPartEndTime)
                    throw new BotScheduleException("Inccorect lesson time");

                return;
            }

            throw new BotScheduleException("Inccorect lesson time");
        }

        void SetDayOfWeek()
        {
            foreach (DayOfWeek dayOfWeek in Enum.GetValues<DayOfWeek>())
            {
                if (dayOfWeek.ToString().ToLower() == dayOfWeekStr.ToLower())
                {
                    timeLesson.DayOfWeek = dayOfWeek;
                    return;
                }
            }

            throw new BotScheduleException("Inccorect day of week");
        }

        void SetSchWeek()
        {
            try
            {
                if (bool.Parse(isAlwaysStr))
                    timeLesson.SchWeek = SchWeek.Always;
                else if (bool.Parse(isNumeratorStr))
                    timeLesson.SchWeek = SchWeek.Numerator;
                else if (bool.Parse(isDenominatorStr))
                    timeLesson.SchWeek = SchWeek.Denominator;
            }
            catch
            {
                throw new BotScheduleException("Inccorect schedule of week");
            }

        }
    }
    static Teacher ParseTeacher(string teacherStr, long tokenId)
    {
        Teacher teacher = new();
        teacher.TokenId = tokenId;

        if (string.IsNullOrEmpty(teacherStr))
            teacher.LastName = "Unknown";
        else
        {
            string characters = @"[a-zA-Zа-яА-ЯёЁЇїІіЄєҐґ\']";
            Regex regex = new("(" + characters + @"{3,}|" + characters + @"{3,}\s+" + characters + @"{3,})\s+(" + characters + @"{1,})\.{0,1}\s*(" + characters + "*)");
            if (regex.IsMatch(teacherStr))
            {
                var groups = regex.Match(teacherStr).Groups;

                teacher.LastName = groups[1].Value;
                teacher.FirstName = groups[2].Value;
                teacher.FatherName = groups[3].Value;
            }
            else
                throw new BotScheduleException("Inccorect teacher's name");
        }

        return teacher;
    }
    static Conference ParseConference(string conferenceStr, long tokenId)
    {
        Conference conference = new();

        conference.Link = string.IsNullOrEmpty(conferenceStr) ? "Unknown" : conferenceStr;
        conference.TokenId = tokenId;

        return conference;
    }
    static Subject ParseSubject(string subjectStr, long tokenId)
    {
        Subject subject = new();

        subject.Name = subjectStr;
        subject.TokenId = tokenId;

        return subject;
    }
}


