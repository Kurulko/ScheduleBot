using ScheduleBot.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using ScheduleBot.Commands.Interfaces;

namespace ScheduleBot.Commands.Main;

public record Stop_MainBotCommand : OnceBotCommand
{
    const string stop = "/stop";
    public Stop_MainBotCommand() : base(new Command(stop, "Stop bot")) { }

    protected override string ResponseStr()
        => "Good bye!";
}
