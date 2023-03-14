using ScheduleBot.Database.Models;
using ScheduleBot.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ScheduleBot.Services.ByToken;

public class ConferenceServiceByToken : ServiceByToken<Conference>
{
    public ConferenceServiceByToken(long tokenId) : base(tokenId) { }

    public override DbSet<Conference> GetAllModels()
        => db.Conferences;

    public IEnumerable<Conference> GetConferencesIncludeTeacherAndSubject()
        => db.Conferences.Include(c => c.Teacher).Include(c => c.Subject).Where(b => b.TokenId == tokenId).ToList();

    public Conference? GetConferencesById(long id)
        => GetModels().FirstOrDefault(b => b.Id == id);
    public Conference? GetConferencesByIdIncludeTeacherAndSubject(long id)
        => GetConferencesIncludeTeacherAndSubject().FirstOrDefault(b => b.Id == id);
}
