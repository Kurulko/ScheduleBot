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

namespace ScheduleBot.Commands.Interfaces;

public abstract record BotCommand(Command Command)
{
    public abstract bool IsExistCommand(string currentName);
    public abstract Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, int? replyToMessageId = null);
    protected readonly string nameOfBot = $"@{TelegramSettings.GetCurrentBotName().Result}";

    protected internal BreakServiceByToken breakService = null!;
    protected internal ConferenceServiceByToken conferenceService = null!;
    protected internal HWServiceByToken hWService = null!;
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
}
