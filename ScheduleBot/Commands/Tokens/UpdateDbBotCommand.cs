using ScheduleBot.Database.Initialize.Excel;
using ScheduleBot.Database.Models;
using ScheduleBot.Database;
using ScheduleBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using ScheduleBot.Commands.Interfaces;
using ScheduleBot.Bot;
using Microsoft.EntityFrameworkCore;
using ScheduleBot.Services.Common;

namespace ScheduleBot.Commands.Tokens;

public record UpdateDbBotCommand : OnceBotCommand
{
    public UpdateDbBotCommand() : base(new Command("/update_db", "Update db")) { }

    protected override string ResponseStr()
         => "<b>Database successfully updated!</b>";

    async Task UpdateDbByTokenAsync(ChatId chatId)
    {
        using ScheduleContext db = new();

        ChatService chatService = new();
        TelegramChat chat = chatService.GetChatByChat(chatId.Identifier!.Value)!;

        Token token = db.Tokens.Include(t => t.Conferences).Include(t => t.Subjects).Include(t => t.Breaks).Include(t => t.Teachers).Include(t => t.TimeLessons).Include(t => t.HWs).First(t => t.Id == chat.TokenId);

        if (token.Conferences is not null)
        {
            db.Conferences.RemoveRange(token.Conferences);
            db.SaveChanges();
        }
        if (token.Subjects is not null)
        {
            db.Subjects.RemoveRange(token.Subjects);
            db.SaveChanges();
        }
        if (token.Breaks is not null)
        {
            db.Breaks.RemoveRange(token.Breaks);
            db.SaveChanges();
        }
        if (token.Teachers is not null)
        {
            db.Teachers.RemoveRange(token.Teachers);
            db.SaveChanges();
        }
        if (token.TimeLessons is not null)
        {
            db.TimeLessons.RemoveRange(token.TimeLessons);
            db.SaveChanges();
        }
        if (token.HWs is not null)
        {
            db.HWs.RemoveRange(token.HWs);
            db.SaveChanges();
        }

        await SeedDbFromExcel.SeedDbAsync(token);
    }

    public override async Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, int? replyToMessageId = null)
    {
        string responseStr = ResponseStr();
        await UpdateDbByTokenAsync(chatId);
        return await bot.SendTextMessageAsync(chatId, responseStr, ParseMode.Html, replyToMessageId: replyToMessageId, cancellationToken: cts.Token);
    }
}
