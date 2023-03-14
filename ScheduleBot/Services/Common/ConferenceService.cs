using ScheduleBot.Database.Models;
using ScheduleBot.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ScheduleBot.Services.Common;

public class ConferenceService : Service<Conference>
{
    public override DbSet<Conference> GetAllModels()
        => db.Conferences;
}