using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleBot.Commands.Interfaces;
using ScheduleBot.Commands.Periodically;

namespace ScheduleBot.Commands.Help;

public record HelpBotCommand : OnceBotCommand, IAllActions
{
    const string help = "/help";

    public HelpBotCommand() : base(new Command(help, "All bot's commands", IsPopular: true))
        => AllActions = new List<BotCommand>() { this };

    public IEnumerable<BotCommand> AllActions { get; set; } = null!;

    protected override string ResponseStr()
    {

        string response = Command.Description + ": \n\n";
        if (AllActions is not null && AllActions.Any())
            foreach (var action in AllActions)
                response += action.DisplayCommandStr() + "\n";
        else
            response += DisplayCommandStr();

        return response;
    }
}
