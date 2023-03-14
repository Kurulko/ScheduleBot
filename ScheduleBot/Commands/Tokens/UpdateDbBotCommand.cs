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
using ScheduleBot.Services;

namespace ScheduleBot.Commands.Tokens;

public record UpdateDbBotCommand : OnceBotCommand
{
    public UpdateDbBotCommand() : base(new Command("/update_db", "Update database")) { }

    protected override string ResponseStr()
         => "<b>Database successfully updated!</b>";

    async Task UpdateDbByTokenAsync(ChatId chatId)
    {
        ChatService chatService = new();
        TelegramChat chat = chatService.GetChatByChat(chatId.Identifier!.Value)!;

        ModelsService.DeleteAllModelsByTokenId(chat.TokenId!.Value);

        TokenService tokenService = new();
        Token token = tokenService.GetTokenByIdIncludeAllModels(chat.TokenId!.Value)!;

        await SeedDbFromExcel.SeedDbAsync(token);
    }

    public override async Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, int? replyToMessageId = null)
    {
        string responseStr = ResponseStr();
        await UpdateDbByTokenAsync(chatId);
        return await SendTextMessagesIfResponseMoreMaxLengthAsync(bot, chatId, cts, responseStr, replyToMessageId); ;
    }
}
