using Microsoft.EntityFrameworkCore;
using ScheduleBot.Database;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Services.ByToken;

public class BreakServiceByToken : ServiceByToken<Break>
{
    public BreakServiceByToken(long tokenId) : base(tokenId) { }

    public override DbSet<Break> GetAllModels()
        => db.Breaks;
}
