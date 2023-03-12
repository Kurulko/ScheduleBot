using ScheduleBot.Database;
using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ScheduleBot.Services.Common;

public class ChatService
{
    ScheduleContext db = new();

    public IEnumerable<TelegramChat> GetChats()
        => db.Chats.ToList();

    public TelegramChat? GetChatById(long id)
        => GetChats().FirstOrDefault(b => b.Id == id);
    public TelegramChat? GetChatByChat(long chat)
        => GetChats().FirstOrDefault(b => b.Chat == chat);

    public void UpdateChat(TelegramChat chat)
    {
        db.Chats.Update(chat);
        db.SaveChanges();
    }

    public void AddChat(TelegramChat chat)
    {
        db.Chats.Add(chat);
        db.SaveChanges();
    }
}
