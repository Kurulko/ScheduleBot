using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace ScheduleBot.Database.Models;

public class TelegramChat
{
    public long Id { get; set; }
    public long Chat { get; set; }
    public ChatType Type { get; set; }
    public string? Title { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Bio { get; set; }
    public bool? HasPrivateForwards { get; set; }
    public string? Description { get; set; }
    public string? InviteLink { get; set; }
    public int? SlowModeDelay { get; set; }
    public int? MessageAutoDeleteTime { get; set; }
    public bool? HasProtectedContent { get; set; }
    public string? StickerSetName { get; set; }
    public bool? CanSetStickerSet { get; set; }
    public long? LinkedChatId { get; set; }

    public static explicit operator TelegramChat(Chat chat)
    {
        TelegramChat tgChat = new();

        tgChat.Chat = chat.Id;
        tgChat.Type = chat.Type;
        tgChat.Title = chat.Title;
        tgChat.Username = chat.Username;
        tgChat.FirstName = chat.FirstName;
        tgChat.LastName = chat.LastName;
        tgChat.Bio = chat.Bio;
        tgChat.HasPrivateForwards = chat.HasPrivateForwards;
        tgChat.Description = chat.Description;
        tgChat.InviteLink = chat.InviteLink;
        tgChat.SlowModeDelay = chat.SlowModeDelay;
        tgChat.MessageAutoDeleteTime = chat.MessageAutoDeleteTime;
        tgChat.HasProtectedContent = chat.HasProtectedContent;
        tgChat.StickerSetName = chat.StickerSetName;
        tgChat.CanSetStickerSet = chat.CanSetStickerSet;
        tgChat.LinkedChatId = chat.LinkedChatId;

        return tgChat;
    }

    public long? TokenId { get; set; }
    public Token? Token { get; set; }
}
