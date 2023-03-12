using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Database.Models;

public class Break
{
    public long Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public static explicit operator Break2(Break rest)
    {
        Break2 rest2 = new();

        rest2.Id = rest.Id;
        rest2.StartTime = TimeOnly.FromDateTime(rest.StartTime);
        rest2.EndTime = TimeOnly.FromDateTime(rest.EndTime);
        rest2.TokenId = rest.TokenId;
        rest2.Token = rest.Token;

        return rest2;
    }
    public long TokenId { get; set; }
    public Token? Token { get; set; }
}
public class Break2
{
    public long Id { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public long TokenId { get; set; }
    public Token? Token { get; set; }
}
