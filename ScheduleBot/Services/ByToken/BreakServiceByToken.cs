using ScheduleBot.Database;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Services.ByToken;

public class BreakServiceByToken : DbServiceByToken
{
    public BreakServiceByToken(long tokenId) : base(tokenId) { }

    public IEnumerable<Break> GetBreaks()
        => db.Breaks.Where(b => b.TokenId == tokenId).ToList();

    public Break? GetBreakById(long id)
        => GetBreaks().FirstOrDefault(b => b.TokenId == tokenId && b.Id == id);
}
