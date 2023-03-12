using Microsoft.EntityFrameworkCore;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Services.ByToken;

public class TimeLessonServiceByToken : DbServiceByToken
{
    public TimeLessonServiceByToken(long tokenId) : base(tokenId) { }

    public IEnumerable<TimeLesson> GetTimeLessons()
        => db.TimeLessons.Where(b => b.TokenId == tokenId).ToList();
    public IEnumerable<TimeLesson> GetTimeLessonsIncludeConference()
        => db.TimeLessons.Include(tl => tl.Conference).Where(b => b.TokenId == tokenId).ToList();

    public TimeLesson? GetTimeLessonById(long id)
        => GetTimeLessons().FirstOrDefault(b => b.Id == id);
    public TimeLesson? GetTimeLessonByIdIncludeConference(long id)
        => GetTimeLessonsIncludeConference().FirstOrDefault(b => b.Id == id);
}
