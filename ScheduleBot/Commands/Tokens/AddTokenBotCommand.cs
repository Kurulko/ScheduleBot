using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleBot.Commands.Interfaces;
using ScheduleBot.Commands.Periodically;
using ScheduleBot.Exceptions;
using System.Text.RegularExpressions;
using ScheduleBot.Database;
using ScheduleBot.Database.Models;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using ScheduleBot.Database.Initialize.Excel;
using ScheduleBot.Services.Common;

namespace ScheduleBot.Commands.Tokens;

public record AddTokenBotCommand : OnceBotCommand
{
    public AddTokenBotCommand() : base(new Command("/add_token_{YOUR_TOKEN}", "Your token for using this bot", @"\/add_token_(.+)")) { }

    string tokenStr = null!;
    protected override string ResponseStr()
    {
        tokenStr = GetTokenStr();
        if (string.IsNullOrEmpty(tokenStr))
            throw BotScheduleException.IncorrectExpression(); 

        return "<b>Thanks, welcome!</b>\nRead more about <a href=\"https://github.com/Kurulko/ScheduleBot/blob/master/README.md\">the token</a>";
    }

    string GetTokenStr()
    {
        Regex regex = new(Command.RegEx!);
        if (!regex.IsMatch(currentCommandStr))
            throw BotScheduleException.IncorrectExpression(); ;
        
        return regex.Match(currentCommandStr).Groups[1].Value;
    }

    async Task AddTokenToChatAsync(ChatId chatId)
    {
        ChatService chatService = new();
        TelegramChat chat = chatService.GetChatByChat(chatId.Identifier!.Value)!;

        TokenService tokenService = new();
        Token? token = tokenService.GetTokenByTokenName(tokenStr);
        if (token is null)
        {
            token = new() { Name = tokenStr };
            tokenService.AddModel(token);

            await SeedDbFromExcel.SeedDbAsync(token);
        }

        chat.TokenId = token!.Id;
        chatService.UpdateModel(chat);
    }

    public override async Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, int? replyToMessageId = null)
    {
        string responseStr = ResponseStr();
        await AddTokenToChatAsync(chatId);
        return await SendTextMessagesIfResponseMoreMaxLengthAsync(bot, chatId, cts, responseStr, replyToMessageId); ;
    }
}
