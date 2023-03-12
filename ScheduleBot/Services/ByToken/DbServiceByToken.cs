using ScheduleBot.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Services.ByToken;

public abstract class DbServiceByToken
{
    protected long tokenId;
    protected ScheduleContext db = new();
    public DbServiceByToken(long tokenId)
        => this.tokenId = tokenId;
}
