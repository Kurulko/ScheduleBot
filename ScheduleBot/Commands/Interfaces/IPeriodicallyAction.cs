using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot.Commands.Interfaces;

public interface IPeriodicallyAction
{
    void DoPeriodicallyActionInTelegram(ITelegramBotClient botClient, long chatId, CancellationTokenSource cts);
}
