using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using ScheduleBot.Settings;
using ScheduleBot.Services.ByToken;
using ScheduleBot.Extensions;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Commands.Interfaces;

public abstract record BotCommand(Command Command)
{
    public abstract bool IsExistCommand(string currentName);
    public abstract Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, int? replyToMessageId = null);
    protected readonly string nameOfBot = $"@{TelegramSettings.GetCurrentBotName().Result}";

    protected internal BreakServiceByToken breakService = null!;
    protected internal ConferenceServiceByToken conferenceService = null!;
    protected internal EventServiceByToken eventService = null!;
    protected internal SubjectServiceByToken subjectService = null!;
    protected internal TeacherServiceByToken teacherService = null!;
    protected internal TimeLessonServiceByToken timeLessonService = null!;

    public virtual string DisplayCommandStr()
    {
        string name = Command.Name;
        if (Command.IsRegEx)
            name = $"<b><code>{name}</code></b>";
        if (Command.Description is not null)
            return $"<b>{name}</b> - <b>{Command.Description}</b>";
        return $"<b>{name}</b>";
    }

    protected async Task<Message> SendTextMessagesIfResponseMoreMaxLengthAsync(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, string responseStr, int? replyToMessageId = null)
    {
        Message message = default!;

        int maxLength = TelegramSettings.MaxLengthOfMessage;
        var responsesStr = responseStr.DevideStrIfMoreMaxLength(maxLength).ToList();
        for (int i = 0; i < responsesStr.Count(); i++)
            message = await SendTextMessageAsync(bot, chatId, cts, $"<b>{responsesStr[i]}</b>", i == 0 ? replyToMessageId : message.MessageId);

        return message;
    }

    protected virtual async Task<Message> SendTextMessageAsync(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, string responseStr, int? replyToMessageId = null)
        => await bot.SendTextMessageAsync(chatId, responseStr, ParseMode.Html, replyToMessageId: replyToMessageId, cancellationToken: cts.Token);
}
