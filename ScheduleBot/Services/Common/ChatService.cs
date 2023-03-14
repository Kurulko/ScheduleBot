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

public class ChatService : Service<TelegramChat>
{
    public override DbSet<TelegramChat> GetAllModels()
        => db.Chats;

    public TelegramChat? GetChatByChat(long chat)
    => GetModels().FirstOrDefault(b => b.Chat == chat);

}
