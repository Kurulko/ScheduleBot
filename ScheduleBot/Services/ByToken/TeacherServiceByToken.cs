using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Services.ByToken;

public class TeacherServiceByToken : DbServiceByToken
{
    public TeacherServiceByToken(long tokenId) : base(tokenId) { }

    public IEnumerable<Teacher> GetTeachers()
        => db.Teachers.Where(b => b.TokenId == tokenId).ToList();

    public Teacher? GetTeacherById(long id)
        => GetTeachers().FirstOrDefault(b => b.TokenId == tokenId && b.Id == id);
}
