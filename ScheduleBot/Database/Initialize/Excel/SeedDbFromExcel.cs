using Newtonsoft.Json;
using ScheduleBot.Database.Models;
using ScheduleBot.Exceptions;
using ScheduleBot.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScheduleBot.Database.Initialize.Excel;

public static class SeedDbFromExcel
{
    public static async Task SeedDbAsync(Token token)
    {
        try
        {
            HttpClient httpClient = new HttpClient();
            string url = $"https://sheets.googleapis.com/v4/spreadsheets/{token.Name}/values/" + "{0}!A2:H100?key=" + ExcelSettings.APIKey;

            ExcelData? excelLessons = await httpClient.GetFromJsonAsync<ExcelData>(string.Format(url, "Lessons"));
            ParseLessonsData(excelLessons!, token.Id);

            ExcelData? excelHWs = await httpClient.GetFromJsonAsync<ExcelData>(string.Format(url, "Homeworks"));
            ParseHWsData(excelHWs!, token.Id);

            AddBreaksToDbByToken(token.Id);
        }
        catch(BotScheduleException)
        {
            throw;
        }
        catch(Exception)
        {
            throw BotScheduleException.IncorrectToken();
        }
    }
    static void AddBreaksToDbByToken(long tokenId)
    {
        using ScheduleContext db = new();

        IList<Break> breaks = new List<Break>();

        IList<TimeLesson> timeLessons = db.TimeLessons.Where(tl => tl.TokenId == tokenId).OrderBy(tl => tl.DayOfWeek).ToList();
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
                    rest.EndTime = nextTimeLesson.FirstPartStartTime;
                else
                    continue;
            }

            rest.TokenId = tokenId;

            breaks.Add(rest);
        }

        if (breaks.Any())
        {
            db.Breaks.AddRange(breaks);
            db.SaveChanges();
        }
    }

    static void ParseHWsData(ExcelData excelHWs, long tokenId)
    {
        using ScheduleContext db = new();

        var excelHWsStr = excelHWs.Values;
        if (excelHWsStr is null)
            throw BotScheduleException.IncorrectValuesFromExcel();

        foreach (var excelHWStr in excelHWsStr)
        {
            Teacher teacher = ParseTeacher(excelHWStr[1], tokenId);
            teacher = AddTeacherToDbIfNotExist(db, teacher, tokenId);

            Subject subject = ParseSubject(excelHWStr[2], tokenId);
            subject = AddSubjectToDbIfNotExist(db, subject, tokenId);

            string? descriptionStr = excelHWStr.Count == 5 ? excelHWStr[4] : null;
            //string? typeOfHWStr = excelHWStr.Count == 5 ? excelHWStr[3] : null;
            HW hw = ParseHW(excelHWStr[0], excelHWStr[3], descriptionStr, tokenId);
            AddHWToDbIfNotExist(db, hw, teacher.Id, subject.Id, tokenId);
        }
    }
    static void ParseLessonsData(ExcelData excelLessons, long tokenId)
    {
        using ScheduleContext db = new();

        var excelLessonsStr = excelLessons.Values;
        if (excelLessonsStr is null)
            throw BotScheduleException.IncorrectValuesFromExcel();

        foreach (var excelLessonStr in excelLessonsStr)
        {
            Teacher teacher = ParseTeacher(excelLessonStr[1], tokenId);
            teacher = AddTeacherToDbIfNotExist(db, teacher, tokenId);

            Subject subject = ParseSubject(excelLessonStr[3], tokenId);
            subject = AddSubjectToDbIfNotExist(db, subject, tokenId);

            Conference conference = ParseConference(excelLessonStr[2], tokenId);
            conference = AddConferenceToDbIfNotExist(db, conference, teacher.Id, subject.Id, tokenId);

            TimeLesson timeLesson = ParseTimeLesson(excelLessonStr[0], excelLessonStr[7], excelLessonStr[4], excelLessonStr[5], excelLessonStr[6], tokenId);//TODO with current time
            AddTimeLessonToDbIfNotExist(db, timeLesson, conference.Id, tokenId);
        }
    }


    static Teacher AddTeacherToDbIfNotExist(ScheduleContext db, Teacher teacher, long tokenId)
    {
        Teacher? _teacher = db.Teachers.FirstOrDefault(t => t.FirstName == teacher.FirstName && t.LastName == teacher.LastName && t.FatherName == teacher.FatherName && t.TokenId == tokenId);
        if (_teacher is null)
        {
            db.Teachers.Add(teacher);
            db.SaveChanges();
        }
        return _teacher is not null ? _teacher : teacher;
    }
    static Subject AddSubjectToDbIfNotExist(ScheduleContext db, Subject subject, long tokenId)
    {
        Subject? _subject = db.Subjects.FirstOrDefault(s => s.Name == subject.Name && s.TokenId == tokenId);
        if (_subject is null)
        {
            db.Subjects.Add(subject);
            db.SaveChanges();
        }
        return _subject is not null ? _subject : subject;
    }
    static Conference AddConferenceToDbIfNotExist(ScheduleContext db, Conference conference, long teacherId, long subjectId, long tokenId)
    {
        Conference? _conference = db.Conferences.FirstOrDefault(c => c.Link == conference.Link && c.TokenId == tokenId);
        if (_conference is null)
        {
            conference.TeacherId = teacherId;
            conference.SubjectId = subjectId;
            db.Conferences.Add(conference);
            db.SaveChanges();
        }
        else
        {
            if (_conference.TeacherId != teacherId)
            {
                _conference.TeacherId = teacherId;
                db.Conferences.Update(_conference);
                db.SaveChanges();
            }
            if (_conference.SubjectId != subjectId)
            {
                _conference.SubjectId = subjectId;
                db.Conferences.Update(_conference);
                db.SaveChanges();
            }
        }
        return _conference is not null ? _conference : conference;
    }
    static HW AddHWToDbIfNotExist(ScheduleContext db, HW hw, long teacherId, long subjectId, long tokenId)
    {
        HW? _hw = db.HWs.FirstOrDefault(h => h.Deadline == hw.Deadline && h.WasGivenDate == hw.WasGivenDate  && h.Description == hw.Description && h.TypeOfHW == hw.TypeOfHW && h.TokenId == tokenId);
        if (_hw is null)
        {
            hw.TeacherId = teacherId;
            hw.SubjectId = subjectId;
            db.HWs.Add(hw);
            db.SaveChanges();
        }
        else
        {
            if (_hw.TeacherId != teacherId)
            {
                _hw.TeacherId = teacherId;
                db.HWs.Update(_hw);
                db.SaveChanges();
            }
            if (_hw.SubjectId != subjectId)
            {
                _hw.SubjectId = subjectId;
                db.HWs.Update(_hw);
                db.SaveChanges();
            }
        }
        return _hw is not null ? _hw : hw;
    }
    static TimeLesson AddTimeLessonToDbIfNotExist(ScheduleContext db, TimeLesson timeLesson, long conferenceId, long tokenId)
    {
        TimeLesson? _timeLesson = db.TimeLessons.FirstOrDefault(tl => tl.FirstPartStartTime == timeLesson.FirstPartStartTime && tl.FirstPartEndTime == timeLesson.FirstPartEndTime && tl.SecondPartStartTime == timeLesson.SecondPartStartTime && tl.SecondPartEndTime == timeLesson.SecondPartEndTime && tl.DayOfWeek == timeLesson.DayOfWeek && tl.SchWeek == timeLesson.SchWeek && tl.TokenId == tokenId);
        if (_timeLesson is null)
        {
            timeLesson.ConferenceId = conferenceId;
            db.TimeLessons.Add(timeLesson);
            db.SaveChanges();
        }
        else if (_timeLesson.ConferenceId != conferenceId)
        {
            _timeLesson.ConferenceId = conferenceId;
            db.TimeLessons.Update(_timeLesson);
            db.SaveChanges();
        }
        return _timeLesson is not null ? _timeLesson : timeLesson;
    }


    static HW ParseHW(string deadlineStr, string typeOfHWStr, string? descriptionStr, long tokenId)
    {
        HW hw = new();

        SetDeadline();
        SetTypeOfHW();
        SetDescription();
        hw.TokenId = tokenId;

        return hw;

        void SetDeadline()
        {
            try
            {
                hw.Deadline = DateTime.Parse(deadlineStr); //CHECK IT
            }
            catch
            {
                //hw.Deadline = DateTime.MaxValue;
                throw new BotScheduleException("Inccorect deadline time");
            }
        }

        void SetTypeOfHW()
        {
            foreach (TypeOfHW typeOfHW in Enum.GetValues<TypeOfHW>())
            {
                if (typeOfHW.ToString().ToLower() == typeOfHWStr.ToLower())
                {
                    hw.TypeOfHW = typeOfHW;
                    return;
                }
            }

            throw new BotScheduleException("Inccorect type of hw");
        }

        void SetDescription()
            => hw.Description = descriptionStr;
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

        string characters = "[a-zA-Zа-яА-ЯёЁЇїІіЄєҐґ]";
        Regex regex = new(@"(" + characters + @"{3,}\s+" + characters + "{3,}|" + characters + @"{3,})\s+(" + characters + @"{1,})\.{0,1}\s*(" + characters + "*)");
        if (regex.IsMatch(teacherStr))
        {
            var groups = regex.Match(teacherStr).Groups;

            teacher.LastName = groups[1].Value;
            teacher.FirstName = groups[2].Value;
            teacher.FatherName = groups[3].Value;
        }
        else
            teacher.LastName = "Unknown";

        return teacher;
    }
    static Conference ParseConference(string conferenceStr, long tokenId)
    {
        Conference conference = new();

        conference.Link = conferenceStr;
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


