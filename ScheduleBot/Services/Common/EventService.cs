using ScheduleBot.Database.Models;
using ScheduleBot.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ScheduleBot.Services.Common;

public class EventService : Service<Event>
{
    public override DbSet<Event> GetAllModels()
        => db.Events;
}