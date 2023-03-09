using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleBot.Commands.Interfaces;

namespace ScheduleBot.Commands.Help;

public record HelpBotCommands : BotCommands, IAllActions
{
    const string help = "/help";

    public HelpBotCommands() : base(new Command(help, "All bot's commands", IsPopular: true))
        => AllActions = new List<BotCommands>() { this };

    public IEnumerable<BotCommands> AllActions { get; set; } = null!;

    protected override string ResponseStr()
    {

        string response = Command.Description + ": \n\n";
        if (AllActions is not null && AllActions.Any())
            foreach (var action in AllActions)
                response += string.Concat(DisplayCommand(action.Command) + "\n\n");
        else
            response += DisplayCommand(Command);

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
