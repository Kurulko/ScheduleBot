using Microsoft.EntityFrameworkCore;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Services.ByToken;

public class TimeLessonServiceByToken : ServiceByToken<TimeLesson>
{
    public TimeLessonServiceByToken(long tokenId) : base(tokenId) { }

    public override DbSet<TimeLesson> GetAllModels()
        => db.TimeLessons;
    public IEnumerable<TimeLesson> GetTimeLessonsIncludeConference()
    => GetAllModels().Include(tl => tl.Conference).Where(b => b.TokenId == tokenId).ToList();

    public TimeLesson? GetTimeLessonByIdIncludeConference(long id)
    => GetTimeLessonsIncludeConference().FirstOrDefault(b => b.Id == id);

}
