using Microsoft.EntityFrameworkCore;
using ScheduleBot.Database;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Services.Common;

public class TokenService : Service<Token>
{
    public override DbSet<Token> GetAllModels()
        => db.Tokens;

    public Token? GetTokenByIdIncludeAllModels(long id)
        => GetAllModels().Include(t => t.Conferences).Include(t => t.Subjects).Include(t => t.Breaks).Include(t => t.Teachers).Include(t => t.TimeLessons).Include(t => t.Events).FirstOrDefault(t => t.Id == id);

    public Token? GetTokenByTokenName(string tokenName)
        => GetModels().FirstOrDefault(t => t.Name == tokenName);
    public Token? GetTokenByChat(long chat)
        => GetAllModels().Include(t => t.Chats).FirstOrDefault(b => b.Chats!.Select(c => c.Chat).Contains(chat));
}
