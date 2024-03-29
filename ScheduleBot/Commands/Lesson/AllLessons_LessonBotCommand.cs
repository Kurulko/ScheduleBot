﻿using ScheduleBot.Bot;
using ScheduleBot.Database.Models;
using ScheduleBot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ScheduleBot.Commands.Lesson;

public record AllLessons_LessonBotCommand : LessonBotCommand
{
    public AllLessons_LessonBotCommand() : base(new Command("/all_lessons", "All lessons")) { }


    IList<string> daysLessonsStr = new List<string>();
    protected internal override string ResponseLessonStr()
    {
        string result = string.Empty;

        var timeLessons = timeLessonService.GetModels().OrderBy(tl => tl.DayOfWeek).ToList();
        int countOfTimeLessons = timeLessons.Count;

        if (countOfTimeLessons <= 0)
            throw BotScheduleException.LessonNotFound();

        DayOfWeek todayDayOfWeek = DateTime.Now.DayOfWeek;

        DayOfWeek currentDayOfWeek = timeLessons[0].DayOfWeek;
        result += DisplayDayOfWeek(currentDayOfWeek);
        for (int i = 0; i < countOfTimeLessons; i++)
        {
            TimeLesson2 timeLesson = (TimeLesson2)timeLessons[i];

            if (timeLesson.DayOfWeek != currentDayOfWeek)
            {
                daysLessonsStr.Add(result);
                result = string.Empty;
                currentDayOfWeek = timeLesson.DayOfWeek;
                result += DisplayDayOfWeek(currentDayOfWeek);
            }

            string response = GetLessonStrByTimeLesson(timeLesson);
            if (!string.IsNullOrEmpty(response))
            {
                int countOfSeparateElements = 36;
                char betweenLessons = '•', betweenSchWeeks = 'ᐧ';

                if (i != 0 && timeLessons[i - 1].SchWeek != SchWeek.Numerator && timeLesson.SchWeek == SchWeek.Denominator)
                    result += new string(betweenSchWeeks, countOfSeparateElements) + "\n";

                result += response;

                if (i != countOfTimeLessons - 1)
                {
                    var nextTimeLesson = timeLessons[i + 1];
                    if (nextTimeLesson.DayOfWeek != currentDayOfWeek)
                        result += "\n";
                    else
                    {
                        if (timeLesson.SchWeek == SchWeek.Numerator && nextTimeLesson.SchWeek == SchWeek.Denominator)
                            result += DisplaySeparator(betweenSchWeeks, countOfSeparateElements);
                        else
                            result += DisplaySeparator(betweenLessons, countOfSeparateElements);
                    }
                }
            }

            if (i == countOfTimeLessons - 1)
                daysLessonsStr.Add(result);
        }

        return string.Empty;
    }

    public override async Task<Message> SendResponseHtml(ITelegramBotClient bot, ChatId chatId, CancellationTokenSource cts, int? replyToMessageId = null)
    {
        Message? message = null;
        ResponseLessonStr();
        int count = daysLessonsStr.Count;
        for (int i = 0; i < count; i++)
        {
            string dayLessonStr = daysLessonsStr[i];
            message = await SendTextMessagesIfResponseMoreMaxLengthAsync(bot, chatId, cts, dayLessonStr, i == 0 ? replyToMessageId : message!.MessageId);           
        }
        return message!;
    }

    string DisplaySeparator(char symbol, int count)
        => $"\n{new string(symbol, count)}\n";
    string DisplayDayOfWeek(DayOfWeek dayOfWeek)
        => $"\n<b>{dayOfWeek}</b>\n\n";
}
