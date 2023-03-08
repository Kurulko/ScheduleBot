using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using ScheduleBot.Bot;

namespace ScheduleBot.Actions;

public record HelpBotCommands : BotCommandsWithAllActions
{
    const string help = "/help";

    public HelpBotCommands() : base(new Command(help, "All bot's commands", IsPopular: true))
        => AllActions = new List<BotCommands>() { this };

    protected override string ResponseStr()
    {
        var fCommand = Commands.First();

        string response = Commands.First().Description + ": \n\n";
        if (AllActions is not null && AllActions.Any())
            foreach (var action in AllActions)
                response += string.Concat(action.Commands.Select(c => DisplayCommand(c) + "\n")) + "\n";
        else
            response += DisplayCommand(fCommand);

        return response;
    }

    string DisplayCommand(Command command)
    {
        string name = command.Name;
        if (command.IsRegEx)
            name = $"<b><code>{name}</code></b>";
        return $"<b>{name}</b> - <b>{command.Description}</b>";
    }
}
