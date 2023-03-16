using Azure;
using ScheduleBot.Bot;
using ScheduleBot.Commands.Interfaces;
using ScheduleBot.Commands.Lesson.Near;
using ScheduleBot.Exceptions;
using ScheduleBot.Settings;
using ScheduleBot.Timer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using BotCommand = ScheduleBot.Commands.Interfaces.BotCommand;

namespace ScheduleBot.Commands.Periodically;

public abstract record PeriodicallyBotCommand : BotCommand, IPeriodicallyAction
{
    public PeriodicallyBotCommand(Command command) : base(command) { }

    public abstract void DoPeriodicallyActionInTelegram(ITelegramBotClient botClient, long chatId, CancellationTokenSource cts);

    protected PeriodicallyModes mode;
    protected TimerAsync timerAsync = null!;
    public override bool IsExistCommand(string currentName)
    {
        bool result = false;
        string commandNameWithoutSlashLow = GetStrWithoutSlash(Command.Name).ToLower();
        string currentNameLow = currentName.ToLower();

        if (currentNameLow == $"/start_{commandNameWithoutSlashLow}" || currentNameLow == $"/start_{commandNameWithoutSlashLow}{nameOfBot}".ToLower())
        {
            result = true;
            mode = PeriodicallyModes.Start;
        }
        if (currentNameLow == $"/stop_{commandNameWithoutSlashLow}" || currentNameLow == $"/stop_{commandNameWithoutSlashLow}{nameOfBot}".ToLower())
        {
            result = true;
            mode = PeriodicallyModes.Stop;
        }

        return result;
    }
    string GetStrWithoutSlash(string str)
        => string.Concat(str.Skip(1).Take(str.Length - 1));

    void Start()
    {
        try
        {
            timerAsync.Start();
        }
        catch
        {
            timerAsync.Stop();
            throw;
        }
    }
    void Stop()
        => timerAsync.Stop();

    static Dictionary<ChatId, CancellationTokenSource> ctses = new();
    public override async Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, int? replyToMessageId = null)
    {
        string responseStr = $"{mode}: {Command.Description}";

        CancellationTokenSource cts2;
        if (ctses.ContainsKey(chatId))
        {
            cts2 = ctses[chatId];
        }
        else
        {
            cts2 = new CancellationTokenSource();
            ctses.Add(chatId, cts2);
        }

        timerAsync = new(() => DoPeriodicallyActionInTelegram(bot, chatId.Identifier!.Value, cts2), ScheduleSettings.PeriodOfUpdatingData, cts2);

        if (mode == PeriodicallyModes.Start)
            Start();
        else if (mode == PeriodicallyModes.Stop)
            Stop();

        return await SendTextMessagesIfResponseMoreMaxLengthAsync(bot, chatId, cts, responseStr, replyToMessageId); 
    }

    public override string DisplayCommandStr()
    {
        string response = string.Empty;

        string name = Command.Name;
        string? description = Command.Description!;
        bool isRegex = Command.IsRegEx;

        response += DisplayOneCommandStr($"/start_{GetStrWithoutSlash(name)}", $"Start: {description}", isRegex) + "\n";
        response += DisplayOneCommandStr($"/stop_{GetStrWithoutSlash(name)}", $"Stop: {description}", isRegex);

        return response;
    }
    string DisplayOneCommandStr(string name, string description, bool isRegex)
    {
        if (isRegex)
            name = $"<b><code>{name}</code></b>";
        if (description is not null)
            return $"<b>{name}</b> - <b>{description}</b>";
        return $"<b>{name}</b>";
    }

}

public enum PeriodicallyModes
{
    Start, Stop
}
