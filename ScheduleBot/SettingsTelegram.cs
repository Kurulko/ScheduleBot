using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot;

public class SettingsTelegram
{
    static string Token => "5844406785:AAHA8FKtl-gWsaRJXC8IUP8dkKCid5me-nY";
    static ITelegramBotClient bot = null!;
    public static ITelegramBotClient CurrentBot()
    {
        if(bot == null)
            bot = new TelegramBotClient(Token);
        return bot;
    }

    public static async Task<string?> GetCurrentBotName()
    {
        var bot = CurrentBot();
        User user = await bot.GetMeAsync();
        return user.Username;
    }
}
