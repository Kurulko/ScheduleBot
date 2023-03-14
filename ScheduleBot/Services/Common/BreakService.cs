using ScheduleBot.Database.Models;
using ScheduleBot.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;

namespace ScheduleBot.Services.Common;

public class BreakService : Service<Break>
{
    public override DbSet<Break> GetAllModels()
        => db.Breaks;
}