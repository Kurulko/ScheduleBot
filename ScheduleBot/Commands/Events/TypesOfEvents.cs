using ScheduleBot.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleBot.Commands.Events;

public static class TypesOfEvents
{
    public static TypeOfEvent[] TypesOfHws => new TypeOfEvent[] { TypeOfEvent.Homework, TypeOfEvent.LaboratoryWork };
    public static TypeOfEvent[] TypesOfMeetings => new TypeOfEvent[] { TypeOfEvent.ControlWork, TypeOfEvent.Test, TypeOfEvent.Exam, TypeOfEvent.Examination, TypeOfEvent.Credit, TypeOfEvent.Meeting };
}
