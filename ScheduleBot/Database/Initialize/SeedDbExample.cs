using Microsoft.EntityFrameworkCore;
using ScheduleBot.Database.Models;
using ScheduleBot.Services.ByToken;
using ScheduleBot.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Database.Initialize;

public static class SeedDbExample
{

    public static void SeedExampleData()
    {
        TokenService tokenService = new();
        if (!tokenService.AnyModels())
        {
            string token = "kurulkofather";
            tokenService.AddModel(new Token() { Name = token });
        }

        BreakService breakService = new();
        if (!breakService.AnyModels())
        {
            IEnumerable<Break> breaks = GetBreaks();
            breakService.AddModels(breaks);
        }

        TeacherService teacherService = new();
        if (!teacherService.AnyModels())
        {
            IEnumerable<Teacher> teachers = GetTeachers();
            teacherService.AddModels(teachers);
        }

        SubjectService subjectService = new();
        if (!subjectService.AnyModels())
        {
            IEnumerable<Subject> subjects = GetSubjects();
            subjectService.AddModels(subjects);
        }

        ConferenceService conferenceService = new();
        if (!conferenceService.AnyModels())
        {
            IEnumerable<Conference> conferences = GetConferences();
            conferenceService.AddModels(conferences);
        }

        TimeLessonService timeLessonService = new();
        if (!timeLessonService.AnyModels())
        {
            //IEnumerable<TimeLesson> timeLessons = GetTimeLessons();
            IEnumerable<TimeLesson> timeLessons = GetTESTTimeLessons();
            timeLessonService.AddModels(timeLessons);
        }

        EventService eventService = new();
        if (!eventService.AnyModels())
        {
            IEnumerable<Event> events = GetEvents();
            eventService.AddModels(events);
        }
    }

    static IEnumerable<Break> GetBreaks()
    {
        long tokenId = 1;
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

            if (i != countOfTimeLessons - 2)
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

        return breaks;

        //IList<Break> breaks = new List<Break>();

        //foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
        //{
        //    breaks.Add( new Break() { StartTime = DateTime.MinValue.AddHours(10).AddMinutes(15), EndTime = DateTime.MinValue.AddHours(10).AddMinutes(35), TokenId = 1, DayOfWeek = day });
        //    breaks.Add( new Break() { StartTime = DateTime.MinValue.AddHours(12).AddMinutes(10), EndTime = DateTime.MinValue.AddHours(12).AddMinutes(20), TokenId = 1, DayOfWeek = day });
        //    breaks.Add( new Break() { StartTime = DateTime.MinValue.AddHours(13).AddMinutes(55), EndTime = DateTime.MinValue.AddHours(14).AddMinutes(05), TokenId = 1, DayOfWeek = day });
        //}

        //return breaks;
    }
    static IEnumerable<Teacher> GetTeachers()
    {
        IEnumerable<Teacher> teachers = new List<Teacher>()
        {
            new Teacher(){ LastName = "Жиров", FirstName = "Г", FatherName = "Б", TokenId = 1},
            new Teacher(){ LastName = "Котов", FirstName = "М", FatherName = "М", TokenId = 1},
            new Teacher(){ LastName = "Ольшевский", FirstName = "С", FatherName = "В", TokenId = 1},
            new Teacher(){ LastName = "Безпаленко", FirstName = "А", FatherName = "М", TokenId = 1},
            new Teacher(){ LastName = "Борецкий", FirstName = "В", FatherName = "Ф", TokenId = 1},
            new Teacher(){ LastName = "Кузьменко", FirstName = "Г", FatherName = "В", TokenId = 1},
            new Teacher(){ LastName = "Костюк", FirstName = "Ю", FatherName = "В", TokenId = 1},
            new Teacher(){ LastName = "Аль Шурайфи", FirstName = "М", FatherName = "Т", TokenId = 1},
         };
        return teachers;
    }
    static IEnumerable<Subject> GetSubjects()
    {
        IEnumerable<Subject> subjects = new List<Subject>()
        {
            new Subject(){ Name = "Основи радіолокації та радіонавігації", TokenId = 1},
            new Subject(){ Name = "Практика", TokenId = 1},
            new Subject(){ Name = "Основи теорії передавання інформації", TokenId = 1},
            new Subject(){ Name = "Англ", TokenId = 1},
            new Subject(){ Name = "Засоби мікрохвильового діапазону та антенні системи телекомунікацій", TokenId = 1},
            new Subject(){ Name = "Вибрані розділи трудового права і основ підприємницької діяльності", TokenId = 1},
            new Subject(){ Name = "Генерування, формування та передавання сигналів", TokenId = 1},
            new Subject(){ Name = "Телекомунікаційні мережі", TokenId = 1},
        };
        return subjects;
    }
    static IEnumerable<Conference> GetConferences()
    {
        IEnumerable<Conference> conferences = new List<Conference>()
        {
            new Conference(){ SubjectId = 1, TeacherId = 1, Link = "https://meet.google.com/kry-bfkp-uzd" , TokenId = 1},
            new Conference(){ SubjectId = 1, TeacherId = 2, Link = "https://knu-ua.zoom.us/j/83654313639?pwd=Qk5adnhnRWpEUWp6UU1rZUd4Z2xZZz09", TokenId = 1},
            new Conference(){ SubjectId = 3, TeacherId = 3, Link = "https://us04web.zoom.us/j/7996346642?pwd=U1RLSmdKL0dzdGthNzlMQmo5NlBidz09", TokenId = 1},
            new Conference(){ SubjectId = 4, TeacherId = 4, Link = "https://us04web.zoom.us/wc/7161537424/join", TokenId = 1},
            new Conference(){ SubjectId = 5, TeacherId = 5, Link = "https://meet.google.com/dff-dvpn-iqw", TokenId = 1},
            new Conference(){ SubjectId = 6, TeacherId = 6, Link = "https://us02web.zoom.us/j/9714836644?pwd=LzVLb1JmVlM4UkZRd3Q5dG92UzlHZz09", TokenId = 1},
            new Conference(){ SubjectId = 6, TeacherId = 7, Link = "https://us04web.zoom.us/j/5609464030?pwd=jSMPBx0G8OKgpacftvUKaqZFht7P70.1", TokenId = 1},
            new Conference(){ SubjectId = 8, TeacherId = 8, Link = "https://meet.google.com/ezj-uhsi-zud", TokenId = 1},
            new Conference(){ SubjectId = 7, TeacherId = 5, Link = "https://meet.google.com/xqw-ccge-rip", TokenId = 1},
            new Conference(){ SubjectId = 2, TokenId = 1},
            new Conference(){ SubjectId = 7, TokenId = 1},
            new Conference(){ SubjectId = 1, TokenId = 1},
        };
        return conferences;
    }
    static IEnumerable<TimeLesson> GetTESTTimeLessons()
    {
        IList<TimeLesson> timeLessons = new List<TimeLesson>();

        for (int i = 0; i < 50; i++)
        {
            DateTime now = DateTime.Now;
            DateTime firstPartStartTime = now.AddMinutes(i + 5 * i);
            DateTime secondPartEndTime = firstPartStartTime.AddMinutes(4);
            timeLessons.Add(new() { FirstPartStartTime = firstPartStartTime, FirstPartEndTime = firstPartStartTime.AddMinutes(1), SecondPartStartTime = firstPartStartTime.AddMinutes(3), SecondPartEndTime = secondPartEndTime, DayOfWeek = now.DayOfWeek, SchWeek = SchWeek.Always, ConferenceId = new Random().Next(10) + 1, TokenId = 1 });
        }

        return timeLessons;
    }
    static IEnumerable<TimeLesson> GetTimeLessons()
    {

        IList<TimeLesson> allTimeLessons = new List<TimeLesson>();

        TimeLesson firstLesson = new() { FirstPartStartTime = DateTime.MinValue.AddHours(8).AddMinutes(40), FirstPartEndTime = DateTime.MinValue.AddHours(9).AddMinutes(25), SecondPartStartTime = DateTime.MinValue.AddHours(9).AddMinutes(30), SecondPartEndTime = DateTime.MinValue.AddHours(10).AddMinutes(15) };
        TimeLesson secondLesson = new() { FirstPartStartTime = DateTime.MinValue.AddHours(10).AddMinutes(35), FirstPartEndTime = DateTime.MinValue.AddHours(11).AddMinutes(20), SecondPartStartTime = DateTime.MinValue.AddHours(11).AddMinutes(25), SecondPartEndTime = DateTime.MinValue.AddHours(12).AddMinutes(10), };
        TimeLesson thirdLesson = new() { FirstPartStartTime = DateTime.MinValue.AddHours(12).AddMinutes(20), FirstPartEndTime = DateTime.MinValue.AddHours(13).AddMinutes(05), SecondPartStartTime = DateTime.MinValue.AddHours(13).AddMinutes(10), SecondPartEndTime = DateTime.MinValue.AddHours(13).AddMinutes(55), };
        TimeLesson fourthLesson = new() { FirstPartStartTime = DateTime.MinValue.AddHours(14).AddMinutes(05), FirstPartEndTime = DateTime.MinValue.AddHours(14).AddMinutes(50), SecondPartStartTime = DateTime.MinValue.AddHours(14).AddMinutes(55), SecondPartEndTime = DateTime.MinValue.AddHours(15).AddMinutes(40), };

        var schWeeks = Enum.GetValues<SchWeek>();
        foreach (SchWeek schWeek in schWeeks)
        {
            var temporaryDays = Enum.GetValues<DayOfWeek>();
            var days = temporaryDays.Skip(1).Take(temporaryDays.Length - 1).Union(temporaryDays.Take(1)).ToList();

            for (int i = 0, j = 1; i < days.Count; i++, j += 4)
            {
                allTimeLessons.Add(new TimeLesson() { FirstPartStartTime = firstLesson.FirstPartStartTime, FirstPartEndTime = firstLesson.FirstPartEndTime, SecondPartStartTime = firstLesson.SecondPartStartTime, SecondPartEndTime = firstLesson.SecondPartEndTime, SchWeek = schWeek, DayOfWeek = days[i], TokenId = 1 });

                allTimeLessons.Add(new TimeLesson() { FirstPartStartTime = secondLesson.FirstPartStartTime, FirstPartEndTime = secondLesson.FirstPartEndTime, SecondPartStartTime = secondLesson.SecondPartStartTime, SecondPartEndTime = secondLesson.SecondPartEndTime, SchWeek = schWeek, DayOfWeek = days[i], TokenId = 1 });

                allTimeLessons.Add(new TimeLesson() { FirstPartStartTime = thirdLesson.FirstPartStartTime, FirstPartEndTime = thirdLesson.FirstPartEndTime, SecondPartStartTime = thirdLesson.SecondPartStartTime, SecondPartEndTime = thirdLesson.SecondPartEndTime, SchWeek = schWeek, DayOfWeek = days[i], TokenId = 1 });

                allTimeLessons.Add(new TimeLesson() { FirstPartStartTime = fourthLesson.FirstPartStartTime, FirstPartEndTime = fourthLesson.FirstPartEndTime, SecondPartStartTime = fourthLesson.SecondPartStartTime, SecondPartEndTime = fourthLesson.SecondPartEndTime, SchWeek = schWeek, DayOfWeek = days[i], TokenId = 1 });
            }
        }

        IList<TimeLesson> timeLessons = new List<TimeLesson>();
        int numerator = 28, denominator = 28 * 2;

        timeLessons.Add(GetTimeLesson(allTimeLessons[0], 1));
        timeLessons.Add(GetTimeLesson(allTimeLessons[1 + numerator], 1));
        timeLessons.Add(GetTimeLesson(allTimeLessons[1 + denominator], 2));
        timeLessons.Add(GetTimeLesson(allTimeLessons[2], 3));
        timeLessons.Add(GetTimeLesson(allTimeLessons[3], 10));
        timeLessons.Add(GetTimeLesson(allTimeLessons[4], 4));
        timeLessons.Add(GetTimeLesson(allTimeLessons[5], 1));
        timeLessons.Add(GetTimeLesson(allTimeLessons[6], 5));
        timeLessons.Add(GetTimeLesson(allTimeLessons[7], 3));
        timeLessons.Add(GetTimeLesson(allTimeLessons[8], 2));
        timeLessons.Add(GetTimeLesson(allTimeLessons[9], 5));
        timeLessons.Add(GetTimeLesson(allTimeLessons[10 + numerator], 11));
        timeLessons.Add(GetTimeLesson(allTimeLessons[10 + denominator], 12));
        timeLessons.Add(GetTimeLesson(allTimeLessons[12], 6));
        timeLessons.Add(GetTimeLesson(allTimeLessons[13 + denominator], 7));
        timeLessons.Add(GetTimeLesson(allTimeLessons[14], 9));
        timeLessons.Add(GetTimeLesson(allTimeLessons[15], 8));
        timeLessons.Add(GetTimeLesson(allTimeLessons[16], 8));
        timeLessons.Add(GetTimeLesson(allTimeLessons[17 + numerator], 8));
        timeLessons.Add(GetTimeLesson(allTimeLessons[17 + denominator], 9));
        timeLessons.Add(GetTimeLesson(allTimeLessons[18], 9));

        return timeLessons;
    }
    static TimeLesson GetTimeLesson(TimeLesson timeLesson, long conferenceId)
    {
        timeLesson.ConferenceId = conferenceId;
        return timeLesson;
    }

    static IEnumerable<Event> GetEvents()
    {
        IEnumerable<Event> events = new List<Event>()
        {
            new(){Deadline = new DateTime(2023, 2, 15, 8, 40, 0), Description = "https://docs.google.com/document/d/11lT0V2I9kh7jMsXQaWnbWrlhMbZVeDZzq_kI-07tgvA/view",  SubjectId = 1, TeacherId= 2, TokenId = 1, TypeOfEvent = TypeOfEvent.Homework},
            new(){Deadline = new DateTime(2023, 2, 21, 12, 20, 0), Description = "https://docs.google.com/document/d/1FrDVMx_Wvovon_yvnp-HMmiIv5L5svXi0wHsIsCBkJA/view",  SubjectId = 5, TeacherId= 5, TokenId = 1, TypeOfEvent = TypeOfEvent.Homework},
            new(){Deadline = new DateTime(2023, 2, 23, 12, 20, 0), Description = "https://docs.google.com/document/d/15AvpkZaKNQ7BEQQ4tfBgOyn19GUlh4lgMyiB882KEb8/view",  SubjectId = 7, TeacherId= 5, TokenId = 1, TypeOfEvent = TypeOfEvent.Homework},
            new(){Deadline = new DateTime(2023, 2, 27, 12, 20, 0), Description = "https://docs.google.com/document/d/1CQSeGBY55MlIpU1x9tinaK6MVWX3bXlCLMDZEmij-TU/view",  SubjectId = 1, TeacherId= 2, TokenId = 1, TypeOfEvent = TypeOfEvent.Homework},
            new(){Deadline = new DateTime(2023, 2, 28, 8, 0, 0), Description = "https://docs.google.com/document/d/1Y8dt7ib61f4JUDz2LjwNaqcWU4LAIympTsBiEnd_LpQ/edit",  SubjectId = 4, TeacherId= 4, TokenId = 1, TypeOfEvent = TypeOfEvent.Homework},
            new(){Deadline = new DateTime(2023, 3, 1, 10, 35, 0), Description = "https://docs.google.com/document/d/1aYi7CPKE2bsjaj68LuFcfGtAwwEEdPSZwo_ys6471yE/view",  SubjectId = 5, TeacherId= 5, TokenId = 1, TypeOfEvent = TypeOfEvent.Homework},
            new(){Deadline = new DateTime(2023, 3, 3, 10, 35, 0), Description = "Диференціальний підсилювач, як він працює, навіщо на виході ставиться ще один транзистор і як рахувати коефіцієнти.",  SubjectId = 7, TeacherId= 5, TokenId = 1, TypeOfEvent = TypeOfEvent.Homework},
            new(){Deadline = new DateTime(2023, 3, 7, 8, 40, 0), Description = "https://docs.google.com/document/d/1delIunATe85bPZ-wrccgAL6NlAeRz8z3pbxaSlOof9I/view",  SubjectId = 4, TeacherId= 4, TokenId = 1, TypeOfEvent = TypeOfEvent.Homework},
            new(){Deadline = new DateTime(2023, 3, 9, 14, 05, 0), Description = "https://docs.google.com/document/d/1lwKkQbPTSdEEaaGdUXwTGD4JexNBWC8a9cCu4fmkEag/view",  SubjectId = 3, TeacherId= 3, TokenId = 1, TypeOfEvent = TypeOfEvent.Homework},
        };
        return events;
    }
}
