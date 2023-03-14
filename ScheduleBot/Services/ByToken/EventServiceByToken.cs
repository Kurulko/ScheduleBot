using Microsoft.EntityFrameworkCore;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Services.ByToken;

public class EventServiceByToken : ServiceByToken<Event>
{
    public EventServiceByToken(long tokenId) : base(tokenId) { }
    public IEnumerable<Event> GetEventsByType(IEnumerable<TypeOfEvent> types)
        => GetModels().Where(b => types.Contains(b.TypeOfEvent)).ToList();

    public override DbSet<Event> GetAllModels()
        => db.Events;
}
