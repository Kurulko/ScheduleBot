using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Services.ByToken;

public class HWServiceByToken : DbServiceByToken
{
    public HWServiceByToken(long tokenId) : base(tokenId) { }

    public IEnumerable<HW> GetHWs()
        => db.HWs.Where(b => b.TokenId == tokenId).ToList();

    public HW? GetHWById(long id)
        => GetHWs().FirstOrDefault(b => b.TokenId == tokenId && b.Id == id);
}
