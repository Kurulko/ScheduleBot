using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Database.Models;

public class DbModelWithToken : DbModel
{
    public long? TokenId { get; set; }
    public Token? Token { get; set; }
}
