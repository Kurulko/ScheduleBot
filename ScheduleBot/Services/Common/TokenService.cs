using Microsoft.EntityFrameworkCore;
using ScheduleBot.Database;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Services.Common;

public class TokenService
{
    ScheduleContext db = new();

    public IEnumerable<Token> GetTokens()
        => db.Tokens.ToList();
    public Token? GetTokenByChat(long chat)
        => db.Tokens.Include(t => t.Chats).FirstOrDefault(b => b.Chats!.Select(c => c.Chat).Contains(chat));
    public Token? GetTokensByTokenName(string tokenName)
        => GetTokens().FirstOrDefault(t => t.Name == tokenName);
    public void AddToken(Token token)
    {
        db.Tokens.Add(token);
        db.SaveChanges();
    }
    public void UpdateToken(Token token)
    {
        db.Tokens.Update(token);
        db.SaveChanges();
    }
}
