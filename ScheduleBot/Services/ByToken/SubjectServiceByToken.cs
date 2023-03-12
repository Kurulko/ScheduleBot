using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Services.ByToken;

public class SubjectServiceByToken : DbServiceByToken
{
    public SubjectServiceByToken(long tokenId) : base(tokenId) { }

    public IEnumerable<Subject> GetSubjects()
        => db.Subjects.Where(b => b.TokenId == tokenId).ToList();

    public Subject? GetSubjectById(long id)
        => GetSubjects().FirstOrDefault(b => b.TokenId == tokenId && b.Id == id);
}
