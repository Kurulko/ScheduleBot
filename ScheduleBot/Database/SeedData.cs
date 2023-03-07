using Microsoft.EntityFrameworkCore;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Database;

public static class SeedData
{
    public static void SeedDb(ScheduleContext db)
    {
        if (!db.Teachers.Any())
        {
            IEnumerable<Teacher> teachers = GetTeachers();
            db.Teachers.AddRange(teachers);
            db.SaveChanges();
        }

        if (!db.Subjects.Any())
        {
            IEnumerable<Subject> subjects = GetSubjects();
            db.Subjects.AddRange(subjects);
            db.SaveChanges();
        }

        if (!db.Conferences.Any())
        {
            IEnumerable<Conference> conferences = GetConferences();
            db.Conferences.AddRange(conferences);
            db.SaveChanges();
        }

        if (!db.TimeLessons.Any())
        {
            IEnumerable<TimeLesson> timeLessons = GetAllTimeLessons();
            db.TimeLessons.AddRange(timeLessons);
            db.SaveChanges();

            IEnumerable<TimeLesson> newTimeLessons = UpdateTimeLessons(db.TimeLessons.ToList());
            db.TimeLessons.UpdateRange(newTimeLessons);
            db.SaveChanges();
        }

        //if (/*!db.SubjectTeacher.Any()*/false)
        //{
        //    IEnumerable<SubjectTeacher> subjectTeachers = GetSubjectsTeachers();
        //    db.SubjectTeacher.AddRange(subjectTeachers);
        //    db.SaveChanges();
        //}

        if (!db.HWs.Any())
        {
            IEnumerable<HW> hws = GetHWs();
            db.HWs.AddRange(hws);
            db.SaveChanges();
        }
    }
    static IEnumerable<Teacher> GetTeachers()
    {
        IEnumerable<Teacher> teachers = new List<Teacher>()
        {
            new Teacher(){/* Id = 1, */LastName = "Жиров", FirstName = "Г", FatherName = "Б"},
            new Teacher(){/* Id = 2, */LastName = "Котов", FirstName = "М", FatherName = "М"},
            new Teacher(){/* Id = 3, */LastName = "Ольшевский", FirstName = "С", FatherName = "В"},
            new Teacher(){/* Id = 4, */LastName = "Безпаленко", FirstName = "А", FatherName = "М"},
            new Teacher(){/* Id = 5, */LastName = "Борецкий", FirstName = "В", FatherName = "Ф"},
            new Teacher(){/* Id = 6, */LastName = "Кузьменко", FirstName = "Г", FatherName = "В"},
            new Teacher(){/* Id = 7, */LastName = "Костюк", FirstName = "Ю", FatherName = "В"},
            new Teacher(){/* Id = 8, */LastName = "Аль Шурайфи", FirstName = "М", FatherName = "Т"},
         };
        return teachers;
    }
    static IEnumerable<Subject> GetSubjects()
    {
        IEnumerable<Subject> subjects = new List<Subject>()
        {
            new Subject(){/* Id = 1,*/ Name = "Основи радіолокації та радіонавігації"},
            new Subject(){/* Id = 2,*/ Name = "Практика"},
            new Subject(){/* Id = 3,*/ Name = "Основи теорії передавання інформації"},
            new Subject(){/* Id = 4,*/ Name = "Англ"},
            new Subject(){/* Id = 5,*/ Name = "Засоби мікрохвильового діапазону та антенні системи телекомунікацій"},
            new Subject(){ /*Id = 6,*/ Name = "Вибрані розділи трудового права і основ підприємницької діяльності"},
            new Subject(){ /*Id = 7,*/ Name = "Генерування, формування та передавання сигналів"},
            new Subject(){ /*Id = 8,*/ Name = "Телекомунікаційні мережі"},
        };
        return subjects;
    }
    static IEnumerable<Conference> GetConferences()
    {
        IEnumerable<Conference> conferences = new List<Conference>()
        {
            new Conference(){ /*Id = 1,*/SubjectId = 1, TeacherId = 1, Link = "https://meet.google.com/kry-bfkp-uzd" },
            new Conference(){ /*Id = 2,*/SubjectId = 1, TeacherId = 2, Link = "https://knu-ua.zoom.us/j/83654313639?pwd=Qk5adnhnRWpEUWp6UU1rZUd4Z2xZZz09"},
            new Conference(){ /*Id = 3,*/SubjectId = 3, TeacherId = 3, Link = "https://us04web.zoom.us/j/7996346642?pwd=U1RLSmdKL0dzdGthNzlMQmo5NlBidz09"},
            new Conference(){ /*Id = 4,*/SubjectId = 4, TeacherId = 4, Link = "https://us04web.zoom.us/wc/7161537424/join"},
            new Conference(){ /*Id = 5,*/SubjectId = 5, TeacherId = 5, Link = "https://meet.google.com/dff-dvpn-iqw"},
            new Conference(){ /*Id = 6,*/SubjectId = 6, TeacherId = 6, Link = "https://us02web.zoom.us/j/9714836644?pwd=LzVLb1JmVlM4UkZRd3Q5dG92UzlHZz09"},
            new Conference(){ /*Id = 7,*/SubjectId = 6, TeacherId = 7, Link = "https://us04web.zoom.us/j/5609464030?pwd=jSMPBx0G8OKgpacftvUKaqZFht7P70.1"},
            new Conference(){ /*Id = 8,*/SubjectId = 8, TeacherId = 8, Link = "https://meet.google.com/ezj-uhsi-zud"},
            new Conference(){ /*Id = 9,*/SubjectId = 7, TeacherId = 5, Link = "https://meet.google.com/xqw-ccge-rip"},
        };
        return conferences;
    }
    static IEnumerable<TimeLesson> GetAllTimeLessons()
    {

        IList<TimeLesson> timeLessons = new List<TimeLesson>();

        TimeLesson firstLesson = new() { StartTime = DateTime.MinValue.AddHours(8).AddMinutes(40), EndTime = DateTime.MinValue.AddHours(10).AddMinutes(15), };
        TimeLesson secondLesson = new() { StartTime = DateTime.MinValue.AddHours(10).AddMinutes(35), EndTime = DateTime.MinValue.AddHours(12).AddMinutes(10), };
        TimeLesson thirdLesson = new() { StartTime = DateTime.MinValue.AddHours(12).AddMinutes(20), EndTime = DateTime.MinValue.AddHours(13).AddMinutes(55), };
        TimeLesson fourthLesson = new() { StartTime = DateTime.MinValue.AddHours(14).AddMinutes(05), EndTime = DateTime.MinValue.AddHours(15).AddMinutes(40), };

        var schWeeks = Enum.GetValues<SchWeekEnum>();
        foreach (SchWeekEnum schWeek in schWeeks)
        {
            var days = Enum.GetValues<DayOfWeek>();
            for (int i = 0, j = 1; i < days.Length; i++, j += 4)
            {
                timeLessons.Add(new TimeLesson() { /*Id = j,*/ StartTime = firstLesson.StartTime, EndTime = firstLesson.EndTime, DayOfWeek = days[i], SchWeekEnum = schWeek });
                timeLessons.Add(new TimeLesson() { /*Id = j + 1,*/ StartTime = secondLesson.StartTime, EndTime = secondLesson.EndTime, DayOfWeek = days[i], SchWeekEnum = schWeek });
                timeLessons.Add(new TimeLesson() { /*Id = j + 2,*/ StartTime = thirdLesson.StartTime, EndTime = thirdLesson.EndTime, DayOfWeek = days[i], SchWeekEnum = schWeek });
                timeLessons.Add(new TimeLesson() { /*Id = j + 3,*/ StartTime = fourthLesson.StartTime, EndTime = fourthLesson.EndTime, DayOfWeek = days[i], SchWeekEnum = schWeek });
            }
        }
        return timeLessons;
    }
    static IEnumerable<TimeLesson> UpdateTimeLessons(IEnumerable<TimeLesson> timeLessons)
    {
        IList<TimeLesson> timeLessonsList = timeLessons.ToList();
        int numerator = 28, denominator = 28 * 2;

        timeLessonsList[0] = GetTimeLesson(timeLessonsList[0], 1, 1, 1);
        timeLessonsList[1 + numerator] = GetTimeLesson(timeLessonsList[1 + numerator], 1, 1, 1);
        timeLessonsList[1 + denominator] = GetTimeLesson(timeLessonsList[1 + denominator], 1, 2, 2);
        timeLessonsList[2] = GetTimeLesson(timeLessonsList[2], 3, 3, 3);
        timeLessonsList[3] = GetTimeLesson(timeLessonsList[3], 2, 0, 0);
        timeLessonsList[4] = GetTimeLesson(timeLessonsList[4], 4, 4, 4);
        timeLessonsList[5] = GetTimeLesson(timeLessonsList[5], 1, 1, 1);
        timeLessonsList[6] = GetTimeLesson(timeLessonsList[6], 5, 5, 5);
        timeLessonsList[7] = GetTimeLesson(timeLessonsList[7], 3, 3, 3);
        timeLessonsList[8] = GetTimeLesson(timeLessonsList[8], 1, 2, 2);
        timeLessonsList[9] = GetTimeLesson(timeLessonsList[9], 5, 5, 5);
        timeLessonsList[10 + numerator] = GetTimeLesson(timeLessonsList[10 + numerator], 7, 0, 0);
        timeLessonsList[10 + denominator] = GetTimeLesson(timeLessonsList[10 + denominator], 3, 0, 0);
        timeLessonsList[12] = GetTimeLesson(timeLessonsList[12], 6, 6, 6);
        timeLessonsList[13 + denominator] = GetTimeLesson(timeLessonsList[13 + denominator], 6, 7, 7);
        timeLessonsList[14] = GetTimeLesson(timeLessonsList[14], 7, 5, 9);
        timeLessonsList[15] = GetTimeLesson(timeLessonsList[15], 8, 8, 8);
        timeLessonsList[16] = GetTimeLesson(timeLessonsList[16], 8, 8, 8);
        timeLessonsList[17 + numerator] = GetTimeLesson(timeLessonsList[17 + numerator], 8, 8, 8);
        timeLessonsList[17 + denominator] = GetTimeLesson(timeLessonsList[17 + denominator], 7, 5, 9);
        timeLessonsList[18] = GetTimeLesson(timeLessonsList[18], 7, 5, 9);

        return timeLessonsList;
    }
    static TimeLesson GetTimeLesson(TimeLesson timeLesson, long subjectId, long teacherId, long conferenceId)
    {
        timeLesson.TeacherId = teacherId;
        timeLesson.SubjectId = subjectId;
        timeLesson.ConferenceId = conferenceId;
        return timeLesson;
    }
    static IEnumerable<SubjectTeacher> GetSubjectsTeachers()
    {
        IEnumerable<SubjectTeacher> subjectTeachers = new List<SubjectTeacher>()
        {
            new(){ /*Id = 1,*/ SubjectId = 1, TeacherId = 1, ConferenceId = 1, TimeLessonId = 1 },
            new(){ /*Id = 2,*/ SubjectId = 1, TeacherId = 1, ConferenceId = 1, TimeLessonId = 2, SchWeekEnum = SchWeekEnum.Numerator },
            new(){ /*Id = 3,*/ SubjectId = 1, TeacherId = 2, ConferenceId = 2, TimeLessonId = 2, SchWeekEnum = SchWeekEnum.Denominator },
            new(){ /*Id = 4, */SubjectId = 3, TeacherId = 3, ConferenceId = 3, TimeLessonId = 3 },
            new(){ /*Id = 5, */SubjectId = 2, TimeLessonId = 4 },
            new(){ /*Id = 6, */SubjectId = 4, TeacherId = 4, ConferenceId = 4, TimeLessonId = 5 },
            new(){ /*Id = 7, */SubjectId = 1, TeacherId = 1, ConferenceId = 1, TimeLessonId = 6 },
            new(){ /*Id = 8, */SubjectId = 5, TeacherId = 5, ConferenceId = 5, TimeLessonId = 7 },
            new(){ /*Id = 9, */SubjectId = 3, TeacherId = 3, ConferenceId = 3, TimeLessonId = 8 },
            new(){ /*Id = 10,*/ SubjectId = 1, TeacherId = 2, ConferenceId = 2, TimeLessonId = 9 },
            new(){ /*Id = 11,*/ SubjectId = 5, TeacherId = 5, ConferenceId = 5, TimeLessonId = 10 },
            new(){ /*Id = 12,*/ SubjectId = 7, TimeLessonId = 11, SchWeekEnum = SchWeekEnum.Numerator },
            new(){ /*Id = 13,*/ SubjectId = 3, TimeLessonId = 11, SchWeekEnum = SchWeekEnum.Denominator },
            new(){ /*Id = 14,*/ SubjectId = 6, TeacherId = 6, ConferenceId = 6, TimeLessonId = 13 },
            new(){ /*Id = 15,*/ SubjectId = 6, TeacherId = 7, ConferenceId = 7, TimeLessonId = 14, SchWeekEnum = SchWeekEnum.Denominator },
            new(){ /*Id = 16,*/ SubjectId = 7, TeacherId = 5, ConferenceId = 9, TimeLessonId = 15 },
            new(){ /*Id = 17,*/ SubjectId = 8, TeacherId = 8, ConferenceId = 8, TimeLessonId = 16 },
            new(){ /*Id = 18,*/ SubjectId = 8, TeacherId = 8, ConferenceId = 8, TimeLessonId = 17 },
            new(){ /*Id = 19,*/ SubjectId = 8, TeacherId = 8, ConferenceId = 8, TimeLessonId = 18, SchWeekEnum = SchWeekEnum.Numerator },
            new(){ /*Id = 20,*/ SubjectId = 7, TeacherId = 5, ConferenceId = 9, TimeLessonId = 18, SchWeekEnum = SchWeekEnum.Denominator },
            new(){ /*Id = 21,*/ SubjectId = 7, TeacherId = 5, ConferenceId = 9, TimeLessonId = 19 },

        };
        return subjectTeachers;
    }
    static IEnumerable<HW> GetHWs()
    {
        IEnumerable<HW> hws = new List<HW>()
        {
            new(){Deadline = new DateTime(2023, 2, 15, 8, 40, 0), Description = "https://docs.google.com/document/d/11lT0V2I9kh7jMsXQaWnbWrlhMbZVeDZzq_kI-07tgvA/view",  SubjectId = 1, TeacherId= 2},
            new(){Deadline = new DateTime(2023, 2, 21, 12, 20, 0), Description = "https://docs.google.com/document/d/1FrDVMx_Wvovon_yvnp-HMmiIv5L5svXi0wHsIsCBkJA/view",  SubjectId = 5, TeacherId= 5},
            new(){Deadline = new DateTime(2023, 2, 23, 12, 20, 0), Description = "https://docs.google.com/document/d/15AvpkZaKNQ7BEQQ4tfBgOyn19GUlh4lgMyiB882KEb8/view",  SubjectId = 7, TeacherId= 5},
            new(){Deadline = new DateTime(2023, 2, 27, 12, 20, 0), Description = "https://docs.google.com/document/d/1CQSeGBY55MlIpU1x9tinaK6MVWX3bXlCLMDZEmij-TU/view",  SubjectId = 1, TeacherId= 2},
            new(){Deadline = new DateTime(2023, 2, 28, 8, 0, 0), Description = "https://docs.google.com/document/d/1Y8dt7ib61f4JUDz2LjwNaqcWU4LAIympTsBiEnd_LpQ/edit",  SubjectId = 4, TeacherId= 4},
            new(){Deadline = new DateTime(2023, 3, 1, 10, 35, 0), Description = "https://docs.google.com/document/d/1aYi7CPKE2bsjaj68LuFcfGtAwwEEdPSZwo_ys6471yE/view",  SubjectId = 5, TeacherId= 5},
            new(){Deadline = new DateTime(2023, 3, 3, 10, 35, 0), Description = "Диференціальний підсилювач, як він працює, навіщо на виході ставиться ще один транзистор і як рахувати коефіцієнти.",  SubjectId = 7, TeacherId= 5},
            new(){Deadline = new DateTime(2023, 3, 7, 8, 40, 0), Description = "https://docs.google.com/document/d/1delIunATe85bPZ-wrccgAL6NlAeRz8z3pbxaSlOof9I/view",  SubjectId = 4, TeacherId= 4},
            new(){Deadline = new DateTime(2023, 3, 9, 14, 05, 0), Description = "https://docs.google.com/document/d/1lwKkQbPTSdEEaaGdUXwTGD4JexNBWC8a9cCu4fmkEag/view",  SubjectId = 3, TeacherId= 3},
        };
        return hws;
    }
}
