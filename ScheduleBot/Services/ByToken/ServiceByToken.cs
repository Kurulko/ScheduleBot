using ScheduleBot.Database;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Services.ByToken;

public abstract class ServiceByToken<T> :Service<T> where T : DbModelWithToken
{
    protected long tokenId;
    public ServiceByToken(long tokenId)
        => this.tokenId = tokenId;

    public override IEnumerable<T> GetModels()
        => GetAllModels().Where(m => m.TokenId == tokenId).ToList();
}
