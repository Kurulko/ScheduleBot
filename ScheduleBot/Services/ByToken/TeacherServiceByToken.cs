using Microsoft.EntityFrameworkCore;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Services.ByToken;

public class TeacherServiceByToken : ServiceByToken<Teacher>
{
    public TeacherServiceByToken(long tokenId) : base(tokenId) { }

    public override DbSet<Teacher> GetAllModels()
        => db.Teachers;
}
